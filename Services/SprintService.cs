using Microsoft.Data.SqlClient;
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
            string query = "INSERT INTO Sprint (sprint_goal, sprint_begin_date, sprint_end_date, created_at, modified_at) " 
                                + "VALUES (@sprint_goal, @sprint_begin_date, @sprint_end_date, @created_at, @modified_at)";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@sprint_goal", req.sprint_goal);
            command.Parameters.AddWithValue("@sprint_begin_date", req.sprint_begin_date);
            command.Parameters.AddWithValue("@sprint_end_date", req.sprint_end_date);
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
            string query = "UPDATE FROM Sprint SET (sprint_goal, sprint_begin_date, sprint_end_date, created_at, modified_at) "                            
                         + "VALUES (@sprint_goal, @sprint_begin_date, @sprint_end_date, @created_at, @modified_at)";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@sprint_goal", req.sprint_goal);
            command.Parameters.AddWithValue("@sprint_begin_date", req.sprint_begin_date);
            command.Parameters.AddWithValue("@sprint_end_date", req.sprint_end_date);
            command.Parameters.AddWithValue("@modified_at", timestamp);
            command.Parameters.AddWithValue("@created_at", timestamp);
            res = command.ExecuteNonQuery();
            return res;
        }

        public Sprint getSprint(int id)
        {
            var conn = _dbService.CreateConnection();
            conn.Open();
            string query = "SELECT * FROM Sprint WHERE id_project_sprint = @project_id";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@project_id", id);
            var reader = command.ExecuteReader();
            reader.Read();
            Sprint res = new Sprint(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
            return res;
        }

    }
}
