using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Thiskord_Back.Models.GestionProjet;
namespace Thiskord_Back.Services
{
    public class SprintService
    {
        private readonly IDbConnectionService _dbService;
        private readonly IConfiguration _configuration;

        public SprintService(IDbConnectionService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            _configuration = configuration;
        }

        public void createSprint(Sprint req)
        {
            var conn = _dbService.CreateConnection();
            conn.Open();



            string timestamp = new DateTime().ToString("yyyyMMddHHmmssffff");
            string query = "INSERT INTO Sprint (sprint_goal, sprint_begin_date, sprint_end_date, id_project_sprint, created_at, modified_at) "
                                + "VALUES (@sprint_goal, @sprint_begin_date, @sprint_end_date, @id_project_sprint, @created_at, @modified_at)";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@sprint_goal", req.sprint_goal);
            command.Parameters.AddWithValue("@sprint_begin_date", req.sprint_begin_date);
            command.Parameters.AddWithValue("@sprint_end_date", req.sprint_end_date);
            command.Parameters.AddWithValue("@id_project_sprint", req.id_project_sprint);
            command.Parameters.AddWithValue("@modified_at", timestamp);
            command.Parameters.AddWithValue("@created_at", timestamp);
            command.ExecuteNonQuery();
            conn.Close();
        }

        public int deleteSprint(int id)
        {
            var conn = _dbService.CreateConnection();
            int res = 0;
            conn.Open();
            string query = "DELETE FROM Sprint WHERE sprint_id = @sprint_id";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@sprint_id", id);
            res = command.ExecuteNonQuery();
            return res;
        }

        public int updateSprint(Sprint req)
        {
            int res = 0;
            string timestamp = new DateTime().ToString("yyyyMMddHHmmssffff");
            var conn = _dbService.CreateConnection();
            conn.Open();
            string query = "UPDATE Sprint SET sprint_goal = @sprint_goal, sprint_begin_date = @sprint_begin_date, sprint_end_date = @sprint_end_date, modified_at = @modified_at WHERE sprint_id = @sprint_id";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@sprint_goal", req.sprint_goal);
            command.Parameters.AddWithValue("@sprint_begin_date", req.sprint_begin_date);
            command.Parameters.AddWithValue("@sprint_end_date", req.sprint_end_date);
            command.Parameters.AddWithValue("@modified_at", timestamp);
            command.Parameters.AddWithValue("@sprint_id", req.sprint_id);
            res = command.ExecuteNonQuery();
            return res;
        }

        public Sprint getSprint(int id)
        {
            var conn = _dbService.CreateConnection();
            conn.Open();
            string query = "SELECT * FROM Sprint WHERE sprint_id = @sprint_id";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@sprint_id", id);
            var reader = command.ExecuteReader();
            reader.Read();
            Sprint res = new Sprint(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(5));
            return res;
        }

        public Sprint[] getSprintByProject(int id)
        {
            var sprints = new List<Sprint>();
            using (var conn = _dbService.CreateConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Sprint WHERE id_project_sprint = @id_project_sprint";
                using (var command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@id_project_sprint", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sprint = new Sprint(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(5));
                            sprints.Add(sprint);
                        }
                    }
                }
            }
            return sprints.ToArray();
        }
    }
}
