using Microsoft.Data.SqlClient;
using Thiskord_Back.Models.Project;

namespace Thiskord_Back.Services
{
    public class ProjectService
    {
        private readonly IDbConnectionService _dbService;
        private readonly LogService _logService;

        public ProjectService(IDbConnectionService dbService, LogService logService)
        {
            _dbService = dbService;
            _logService = logService;
        }

        public int CreateProject(string project_name, string project_desc)
        {
            DateTime modified_at = DateTime.Now;
            try
            {
                using (SqlConnection conn = _dbService.CreateConnection())
                {
                    SqlTransaction transaction;
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    string query = "INSERT INTO Project (project_name, project_desc, modified_at) VALUES (@project_name, @project_desc, @modified_at); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Transaction = transaction;
                        try
                        {
                            command.Parameters.AddWithValue("@project_name", project_name);
                            command.Parameters.AddWithValue("@project_desc", project_desc);
                            command.Parameters.AddWithValue("@modified_at", modified_at);
                            object result = command.ExecuteScalar();
                            transaction.Commit();
                            return result != null ? Convert.ToInt32(result) : -1;
                        }
                        catch (Exception ex2)
                        {
                            _logService.CreateLog($"Erreur lors de l'exécution de la commande SQL : {ex2.Message}");
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception exRollback)
                            {
                                _logService.CreateLog($"Erreur lors du rollback de la transaction : {exRollback.Message}");
                            }
                            return -2;
                        };
                    };
                }
            }
            catch (Exception ex3)
            {
                _logService.CreateLog($"Erreur lors de la création du projet : {ex3.Message}");
                return -3;
            }
        }
        
        public List<Project> GetAllProjects()
        {
            var projects = new List<Project>();

            try
            {
                using (SqlConnection conn = _dbService.CreateConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT
                            [project_id] as [id], 
                            [project_name], 
                            [project_desc] 
                        FROM [dbo].[Project]";

                    using (SqlCommand command = new SqlCommand(query, conn))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["id"]);
                            string name = reader["project_name"]?.ToString() ?? string.Empty;
                            string description = reader["project_desc"]?.ToString() ?? string.Empty;

                            projects.Add(new Project(id, name, description));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.CreateLog($"Erreur lors de la récupération des projets : {ex}");
                throw;
            }

            return projects;
        }

    }
}
