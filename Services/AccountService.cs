using BCrypt.Net;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Collections.Generic;
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
            //return "va te faire foutre dotnet";
        }

        public int patchAccount(UserAccount req)
        {
            SqlConnection conn = _dbService.CreateConnection();
            conn.Open();
            string timestamp = new DateTime().ToString("yyyyMMddHHmmssffff");
            Debug.WriteLine(timestamp);
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
            command.Parameters.AddWithValue("@modified_at", timestamp);
            int res = command.ExecuteNonQuery();
            return res;
        }

        public int patchAccountPassword(UserAccount req)
        {
            SqlConnection conn = _dbService.CreateConnection();
            conn.Open();
            string timestamp = new DateTime().ToString("yyyyMMddHHmmssffff");
            string newCryptedPassword = BCrypt.Net.BCrypt.HashPassword(req.user_password);
            string query = "UPDATE Account SET user_password = @user_password" +
                                        "WHERE user_id = @user_id;";
            using var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@user_password", newCryptedPassword);
            command.Parameters.AddWithValue("@user_id", req.user_id);
            int res = command.ExecuteNonQuery();
            return res;
        }
        public async Task DeleteAccount(int user_id) //Suppression de compte z
        {
            try
            {
                await using (var connection = _dbService.CreateConnection())
                {
                    connection.Open();
                    string deleteRelatedData = @"
                        DELETE FROM Account WHERE user_id = @user_id;
                        DELETE FROM Account WHERE user_password = @user_password;
                        DELETE FROM Account WHERE user_mail = @user_mail
                        DELETE FROM Account WHERE user_picture = @user_picture;";
                    using (var command = new SqlCommand(deleteRelatedData, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", user_id);
                        await command.ExecuteNonQueryAsync();
                    }
                    string deleteAccount = "DELETE FROM Account WHERE user_id = @user_id;";

                    using (var command = new SqlCommand(deleteAccount, connection))
                    {
                        command.Parameters.AddWithValue("@user_id", user_id);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
