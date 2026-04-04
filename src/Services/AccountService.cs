    using BCrypt.Net;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Identity.Client;
    using Microsoft.IdentityModel.Tokens;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Thiskord_Back.Models.Account;
    using Thiskord_Back.Models.Auth;

    namespace Thiskord_Back.Services
    {
        public class AccountService
        {
            private readonly IDbConnectionService _dbService;
            private readonly IConfiguration _configuration;

            public AccountService(IDbConnectionService dbService, IConfiguration configuration)
            {
                _dbService = dbService;
                _configuration = configuration;  
            }

            public UserAccount getAccount(int id)
            {
                SqlConnection conn = _dbService.CreateConnection();
                conn.Open();
                string query = "SELECT user_id, user_name, user_mail, user_picture FROM Account WHERE user_id = @user_id;";
                using var command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@user_id", id);
                using var res = command.ExecuteReader();
                res.Read();
                return new UserAccount(res.GetInt32(0), res.GetString(1), res.GetString(2), res.GetString(3));
            }

            public int patchAccount(UserAccount req)
            {
                SqlConnection conn = _dbService.CreateConnection();
                conn.Open();
                string query = "UPDATE Account SET user_name = @user_name, " +
                                                "user_mail = @user_mail, " +
                                                "user_picture = @user_picture, " +
                                                "modified_at = @modified_at " +
                                            "WHERE user_id = @user_id;";
                using var command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@user_id", req.user_id);
                command.Parameters.AddWithValue("@user_name", req.user_name);
                command.Parameters.AddWithValue("@user_mail", req.user_mail);
                command.Parameters.AddWithValue("@user_picture", req.user_picture);
                command.Parameters.AddWithValue("@modified_at", DateTime.UtcNow);
                int res = command.ExecuteNonQuery();
                return res;
            }

            public int patchAccountPassword(UserAccount req)
            {
                SqlConnection conn = _dbService.CreateConnection();
                conn.Open();
                string newCryptedPassword = BCrypt.Net.BCrypt.HashPassword(req.user_password);
                string query = "UPDATE Account SET user_password = @user_password, modified_at = @modified_at " +
                                            "WHERE user_id = @user_id;";
                using var command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@user_password", newCryptedPassword);
                command.Parameters.AddWithValue("@user_id", req.user_id);
                command.Parameters.AddWithValue("@modified_at", DateTime.UtcNow);
                int res = command.ExecuteNonQuery();
                return res;
            }
        }
    }
