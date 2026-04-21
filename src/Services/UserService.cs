using Microsoft.Data.SqlClient;
using Thiskord_Back.Models.Account;

namespace Thiskord_Back.Services
{
    public interface IUserService
    {
        List<UserAccount> GetAllUsers();
        Task<List<UserAccount>> GetAllUsersForProject(int projectId);
    }
    public class UserService : IUserService
    {
        private readonly IDbConnectionService _dbService;
        private readonly ILogService logService;
        
        public UserService(IDbConnectionService dbService, ILogService logService)
        {
            this._dbService = dbService;
            this.logService = logService;
        }

        public List<UserAccount> GetAllUsers()
        {
            var users = new List<UserAccount>();

            try
            {
                using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();

                    const string query = @"
                        SELECT user_id, user_name, user_mail, user_picture
                        FROM Account;";

                    using var command = new SqlCommand(query, connection);
                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var user = new UserAccount(
                             reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                             reader.IsDBNull(1) ? null : reader.GetString(1),
                             reader.IsDBNull(2) ? null : reader.GetString(2),
                             reader.IsDBNull(3) ? null : reader.GetString(3)
                        );
                        users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                logService.CreateLog($"Erreur lors de la récupération des projets : {ex.Message}");
            }
            return users;
        
        }
        public async Task<List<UserAccount>> GetAllUsersForProject(int projectId)
        {
            var users = new List<UserAccount>();

            try
            {
                using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();

                    const string query = @"
                        SELECT u.user_id, u.user_name, u.user_mail, u.user_picture
                        FROM Account u
                        INNER JOIN ACCESS a ON a.id_account = u.user_id
                        WHERE a.id_project_account = @projectId;";

                    await using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@projectId", projectId);
                    await using var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var user = new UserAccount(
                            reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                            reader.IsDBNull(1) ? null : reader.GetString(1),
                            reader.IsDBNull(2) ? null : reader.GetString(2),
                            reader.IsDBNull(3) ? null : reader.GetString(3)
                        );
                        users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                logService.CreateLog($"Erreur lors de la récupération des projets : {ex.Message}");
            }
            return users;
        
        }
    }
}

