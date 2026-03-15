using BCrypt.Net;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Thiskord_Back.Models.Auth;
using Thiskord_Back.Models.Account;
using Microsoft.Extensions.Configuration;

namespace Thiskord_Back.Services
{
    public class AuthService
    {
        private readonly IDbConnectionService _dbService;
        private readonly IConfiguration _configuration;

        public AuthService(IDbConnectionService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            _configuration = configuration;
        }
        public AuthenticatedUser AuthLogin(string user_auth, string user_password)
        {
            // requete en base pour chopper les infos utilisateurs : user_auth
            SqlConnection conn = _dbService.CreateConnection();
            conn.Open();
            string pwd = "";
            string query = "SELECT user_password FROM dbo.Account WHERE user_name = @user_auth";
            using var command = new SqlCommand(query, conn);
            command.Parameters.AddWithValue("@user_auth", user_auth);
            using var res = command.ExecuteReader();
            res.Read();
            pwd = res.GetString(0);
            res.Close();
            // à partir du résultat, on compare le password en base avec : user_encrypted password
            bool ok = BCrypt.Net.BCrypt.Verify(user_password, pwd);
            // si true : 
            if (ok)
            {
                // requete pour aller chercher les infos utilisateur
                int user_id;
                query = "SELECT user_id, user_name, user_mail, user_picture FROM Account WHERE user_name = @user_auth;";
                using var command1 = new SqlCommand(query, conn);
                command1.Parameters.AddWithValue("@user_auth", user_auth);
                using var res1 = command1.ExecuteReader();
                res1.Read();
                user_id = res1.GetInt32(0);
                UserAccount user = new UserAccount(res1.GetInt32(0), res1.GetString(1), res1.GetString(2),res1.GetString(3));
                res1.Close();
                var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user_id.ToString()), new Claim(ClaimTypes.Name, user_auth) }; 
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)); 
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); 
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"], 
                    audience: _configuration["Jwt:Audience"], 
                    claims: claims, 
                    expires: DateTime.UtcNow.AddHours(2), 
                    signingCredentials: creds
                ); 
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return new AuthenticatedUser(user, tokenString);
            }
            else
            {
                return null;
            }
        }
    }
}
