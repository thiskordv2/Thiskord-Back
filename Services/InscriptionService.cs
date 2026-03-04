using Microsoft.Data.SqlClient;
using Thiskord_Back.Models.Account;
using BCrypt.Net;

namespace Thiskord_Back.Services
{
    public class InscriptionService
    {
        private readonly IDbConnectionService _dbService;
        private readonly LogService _logService;

        public InscriptionService(IDbConnectionService dbService, LogService logService)
        {
            _dbService = dbService;
            _logService = logService;
        }

        public async Task<string> InscriptionUser(string user_name, string user_mail, string user_password, string user_picture)
        {
            var pwd = BCrypt.Net.BCrypt.HashPassword(user_password);
            var user = new UserAccount(user_name, user_mail, pwd, user_picture);

            try
            {
                using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();

                    string query = @"INSERT INTO Account (user_name, user_mail, user_password, user_picture) 
                                     VALUES (@Name, @Mail, @Password, @Picture); 
                                     SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", user.user_name);
                    command.Parameters.AddWithValue("@Mail", user.user_mail);
                    command.Parameters.AddWithValue("@Password", user.user_password);
                    command.Parameters.AddWithValue("@Picture", user.user_picture);

                    user.user_id = (int)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                _logService.CreateLog(ex.Message);
            }

            return "User registered successfully";
        }
    }
}  