using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thiskord_Back.Models.Project;

namespace Thiskord_Back.Services
{
    public interface IProjectService
    {
        Task<Project> Create(string project_name, string project_desc, int userId);
        void DeleteById(int projectId);
        Project Update(Project updatedProject);
        List<Project> GetAll();
        Task<List<Project>> GetAllProjectsForUser(int userId);
    }
    public class ProjectService : IProjectService
    {
        private readonly IDbConnectionService _dbService;

        private readonly ILogService _logService;
        private readonly IChannelService _channelService;

        public ProjectService(IDbConnectionService dbService, ILogService logService, IChannelService channelService)
        {
            _dbService = dbService;
            _logService = logService;
            _channelService = channelService;
        }
         public async Task<Project> Create(string projectName, string projectDesc, int userId)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                throw new ArgumentException("Le nom du projet ne peut pas être vide.", nameof(projectName));

            var project = new Project { name = projectName, description = projectDesc };

            await using var connection = _dbService.CreateConnection();
            await connection.OpenAsync();

            await using var transaction = connection.BeginTransaction();

            try
            {
                const string projectQuery = @"
                    INSERT INTO Project (project_name, project_desc) 
                    VALUES (@Name, @Description); 
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using var projectCmd = new SqlCommand(projectQuery, connection, transaction);
                projectCmd.Parameters.AddWithValue("@Name", projectName);
                projectCmd.Parameters.AddWithValue("@Description", projectDesc);

                project.id = (int)await projectCmd.ExecuteScalarAsync();

                const string accessQuery = @"
                    INSERT INTO dbo.ACCESS (is_admin, is_root, id_account, id_project_account) 
                    VALUES (1, 1, @UserId, @ProjectId);";

                using var accessCmd = new SqlCommand(accessQuery, connection, transaction);
                accessCmd.Parameters.AddWithValue("@UserId", userId);
                accessCmd.Parameters.AddWithValue("@ProjectId", project.id);

                await accessCmd.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                await _channelService.Create("Général", "Votre premier channel", project.id.Value, userId);
                return project;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logService.CreateLog($"Erreur création projet: {ex.Message}");
                throw; 
            }
        }
        public void DeleteById(int projectId)
        {
            try
            {
                using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();
                    string deleteQuery = "DELETE FROM Project WHERE project_id = @Id";
                    using var deleteCommand = new SqlCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@Id", projectId);
                    deleteCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logService.CreateLog(ex.Message);
            }
        }
        public Project Update(Project updatedProject)
        {

            if (string.IsNullOrWhiteSpace(updatedProject.name))
                throw new ArgumentException("Le nom du canal ne peut pas être vide.", nameof(updatedProject.name));
            
            try
            {
                using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();

                    string query = @"UPDATE Project SET project_name = @Name , project_desc = @Description, modified_at = @date WHERE project_id = @Id";

                    using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", updatedProject.id);
                    command.Parameters.AddWithValue("@Name", updatedProject.name);
                    command.Parameters.AddWithValue("@Description", updatedProject.description);
                    command.Parameters.AddWithValue("@date", DateTime.UtcNow);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logService.CreateLog(ex.Message);

            }
            ;
            return updatedProject;
        }

        public List<Project> GetAll()
        {
            var projects = new List<Project>();

            try
            {
                using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();

                    const string query = @"
                        SELECT project_id, project_name, project_desc
                        FROM Project;";

                    using var command = new SqlCommand(query, connection);
                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var project = new Project
                        {
                            id = reader.IsDBNull(0) ? null : reader.GetInt32(0),
                            name = reader.IsDBNull(1) ? null : reader.GetString(1),
                            description = reader.IsDBNull(2) ? null : reader.GetString(2)
                        };

                        projects.Add(project);
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.CreateLog($"Erreur lors de la récupération des projets : {ex.Message}");
            }
            return projects;
        }

        public async Task<List<Project>> GetAllProjectsForUser(int userId)
        {
            var projects =  new List<Project>();

            try
            { 
                using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();

                    const string query = @"
                    SELECT p.project_id, p.project_name, p.project_desc
                    FROM Project p
                    INNER JOIN ACCESS a ON a.id_project_account = p.project_id
                    WHERE a.id_account = @UserId";
                    
                    await using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserId", userId);
                    await using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var project = new Project
                        {
                            id = reader.IsDBNull(0) ? null : reader.GetInt32(0),
                            name = reader.IsDBNull(1) ? null : reader.GetString(1),
                            description = reader.IsDBNull(2) ? null : reader.GetString(2)
                        };
                        projects.Add(project);
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.CreateLog($"Erreur lors de la récupération des projets : {ex.Message}");
            }
            return projects;
        }
    }
}