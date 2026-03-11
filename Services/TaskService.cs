using Microsoft.Data.SqlClient;
using Thiskord_Back.Models.GestionProjet;
namespace Thiskord_Back.Services
{
    public class TaskService
    {
        private readonly IDbConnectionService _dbService;
        private readonly IConfiguration _configuration;

        public TaskService(IDbConnectionService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            _configuration = configuration;
        }
        
        public void createTask(SprintTask req)
        {
            string timestamp = new DateTime().ToString("yyyyMMddHHmmssffff");
            var conn = _dbService.CreateConnection();
            conn.Open();
            string query = "INSERT INTO Task"
                + " SET (task_title, task_desc, is_subtask, task_status, created_at, modified_at, id_creator, id_resp, id_project_task, id_parent_task)"
                + " VALUES (@taskTitle, @taskDesc, @isSubtask, @taskStatus, @createdAt, @modifiedAt, @idCreator, @idResp, @idProject, @idParent);";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("taskTitle", req.task_title);
            command.Parameters.AddWithValue("taskDesc", req.task_desc);
            command.Parameters.AddWithValue("isSubtask", req.is_subtask);
            command.Parameters.AddWithValue("taskStatus", req.task_status);
            command.Parameters.AddWithValue("createdAt", timestamp);
            command.Parameters.AddWithValue("modifiedAt", timestamp);
            command.Parameters.AddWithValue("idCreator", req.id_creator);
            command.Parameters.AddWithValue("idResp", req.id_resp);
            command.Parameters.AddWithValue("idProject", req.id_project_task);
            command.Parameters.AddWithValue("idParent", req.id_parent_task);
            command.ExecuteNonQuery();
        }
        /**
         * @Param id de la tâche
         * Supprime la tâche en question
         */
        public int deleteTask(int id)
        {
            int res = 0;
            var conn = _dbService.CreateConnection();
            conn.Open();
            string query = "DELETE FROM Task WHERE task_id = @idTache;";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("idTache", id);
            res = command.ExecuteNonQuery();
            return res;
        }

        public int updateTask(SprintTask req)
        {
            int res = 0;
            string timestamp = new DateTime().ToString("yyyyMMddHHmmssffff");
            var conn = _dbService.CreateConnection();
            conn.Open();
            string query = "INSERT INTO Task"
                + " SET (task_title, task_desc, is_subtask, task_status, modified_at, id_creator, id_resp, id_project_task, id_parent_task)"
                + " VALUES (@taskTitle, @taskDesc, @isSubtask, @taskStatus, @modifiedAt, @idCreator, @idResp, @idProject, @idParent) WHERE task_id = @idTask;";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("taskTitle", req.task_title);
            command.Parameters.AddWithValue("taskDesc", req.task_desc);
            command.Parameters.AddWithValue("isSubtask", req.is_subtask);
            command.Parameters.AddWithValue("taskStatus", req.task_status);
            command.Parameters.AddWithValue("modifiedAt", timestamp);
            command.Parameters.AddWithValue("idCreator", req.id_creator);
            command.Parameters.AddWithValue("idResp", req.id_resp);
            command.Parameters.AddWithValue("idProject", req.id_project_task);
            command.Parameters.AddWithValue("idParent", req.id_parent_task);
            command.Parameters.AddWithValue("idTask", req.task_id);
            res = command.ExecuteNonQuery();
            return res;
        }

        /*
         * @Param id du sprint de la tâche
         */
        public List<SprintTask> getTaskBySprint(int id)
        {
            List<SprintTask> res = new List<SprintTask>(); 
            var conn = _dbService.CreateConnection();
            conn.Open();
            string query = "SELECT * FROM Task WHERE id_project_task = @idProject;";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("idProject", id);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                res.Add(new SprintTask(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), (Boolean)reader.GetSqlBoolean(3), reader.GetString(4)));
            }
            return res;
        }

        /**
         * @Param id de la tâche parente, renvois les substask
         */
        public List<SprintTask> getTaskByParentTask(int id)
        {
            List<SprintTask> res = new List<SprintTask>();
            var conn = _dbService.CreateConnection();
            conn.Open();
            string query = "SELECT * FROM Task WHERE id_parent_task = @idParentTask;";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("idParentTask", id);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                res.Add(new SprintTask(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), (Boolean)reader.GetSqlBoolean(3), reader.GetString(4)));

            }
            return res;
        }

    }
}