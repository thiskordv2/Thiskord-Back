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
        Project Create(string project_name, string project_desc);
        void DeleteById(int projectId);
        Project Update(Project updatedProject);
        List<Project> GetAll();
    }
    public class ProjectService
    {
        private readonly IDbConnectionService _dbService;

        private readonly LogService logService;

        public ProjectService(IDbConnectionService dbService, LogService logService)
        {
            this._dbService = dbService;
            this.logService = logService;
        }
        public Project Create(string project_name, string project_desc)
        {

            if (string.IsNullOrWhiteSpace(project_name))
                throw new ArgumentException("Le nom du canal ne peut pas être vide.", nameof(project_name));

            var project = new Project
            {
                name = project_name,
                description = project_desc
            };

            try
            {
                using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();

                    string query = @"INSERT INTO Project (project_name, project_desc) 
                                     VALUES (@Name, @Description); 
                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", project_name);
                    command.Parameters.AddWithValue("@Description", project_desc);

                    project.id = (int)command.ExecuteScalar();

                }
            } catch (Exception ex)
            {
                logService.CreateLog(ex.Message);
                
            };
            return project;


       
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
                logService.CreateLog(ex.Message);
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
                logService.CreateLog(ex.Message);

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
                logService.CreateLog($"Erreur lors de la récupération des projets : {ex.Message}");
            }
            return projects;
        }
    }
}