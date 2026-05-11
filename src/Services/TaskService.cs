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
            int taskId = 0;
            DateTime now = DateTime.Now;
            using var conn = _dbService.CreateConnection();
            conn.Open();
            string query = "INSERT INTO dbo.Task (task_title, task_desc, is_subtask, task_status, created_at, modified_at, id_creator, id_resp, id_project_task, id_parent_task) "
                + "OUTPUT INSERTED.task_id "
                + "VALUES (@taskTitle, @taskDesc, @isSubtask, @taskStatus, @createdAt, @modifiedAt, @idCreator, @idResp, @idProject, @idParent);";
            using var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@taskTitle", req.task_title);
            command.Parameters.AddWithValue("@taskDesc", req.task_desc);
            command.Parameters.AddWithValue("@isSubtask", req.is_subtask);
            command.Parameters.AddWithValue("@taskStatus", req.task_status);
            command.Parameters.AddWithValue("@createdAt", now);
            command.Parameters.AddWithValue("@modifiedAt", now);
            command.Parameters.AddWithValue("@idCreator", req.id_creator);
            command.Parameters.AddWithValue("@idResp", req.id_resp);
            command.Parameters.AddWithValue("@idProject", req.id_project_task);
            command.Parameters.AddWithValue("@idParent", req.id_parent_task.HasValue ? req.id_parent_task.Value : DBNull.Value);
            taskId = (int)command.ExecuteScalar();
            string queryInclude = "INSERT INTO dbo.INCLUDE (id_sprint, id_task) VALUES (@idSprint, @idTask);";
            using var commandInclude = new SqlCommand(queryInclude, conn);
            commandInclude.Parameters.AddWithValue("@idSprint", req.id_sprint);
            commandInclude.Parameters.AddWithValue("@idTask", taskId);
            commandInclude.ExecuteNonQuery();
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
            string queryInclude = "DELETE FROM INCLUDE WHERE id_task = @idTache;";
            var commandInclude = new SqlCommand(queryInclude, conn);
            commandInclude.Parameters.AddWithValue("idTache", id);
            commandInclude.ExecuteNonQuery();
            string query = "DELETE FROM Task WHERE task_id = @idTache;";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("idTache", id);
            res = command.ExecuteNonQuery();
            
            return res;
        }

        public int updateTask(SprintTask req)
        {
            int res = 0;
            DateTime now = DateTime.Now;
            using var conn = _dbService.CreateConnection();
            conn.Open();
            string query = "UPDATE dbo.Task SET task_title = @taskTitle, task_desc = @taskDesc, is_subtask = @isSubtask, task_status = @taskStatus, modified_at = @modifiedAt, id_creator = @idCreator, id_resp = @idResp, id_project_task = @idProject, id_parent_task = @idParent WHERE task_id = @idTask;";
            using var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@taskTitle", req.task_title);
            command.Parameters.AddWithValue("@taskDesc", req.task_desc);
            command.Parameters.AddWithValue("@isSubtask", req.is_subtask);
            command.Parameters.AddWithValue("@taskStatus", req.task_status);
            command.Parameters.AddWithValue("@modifiedAt", now);
            command.Parameters.AddWithValue("@idCreator", req.id_creator);
            command.Parameters.AddWithValue("@idResp", req.id_resp);
            command.Parameters.AddWithValue("@idProject", req.id_project_task);
            command.Parameters.AddWithValue("@idParent", req.id_parent_task.HasValue ? req.id_parent_task.Value : DBNull.Value);
            command.Parameters.AddWithValue("@idTask", req.task_id);
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
            string query = "SELECT Task.* FROM Task, INCLUDE WHERE  Task.is_subtask = @false AND Task.task_id = INCLUDE.id_task AND INCLUDE.id_sprint = @id_sprint;";
            var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("id_sprint", id);
            command.Parameters.AddWithValue("false", false);
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