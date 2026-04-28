using System.Data;
using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Thiskord_Back.Services
{
    public interface ILogService
    {
        void AddLog(int userId, string message);
        void CreateLog(string message);
    }
    public class LogService : ILogService
    {
        private readonly string _connectionString;
        public LogService(IConfiguration config)
        {
            // Récupération de la chaîne de connexion via l'injection de IConfiguration
            _connectionString = config.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("La chaîne de connexion 'Default' est introuvable.");
        }

        // Implémentation de la méthode synchrone de l'interface
        public void AddLog(int userId, string message)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    // Opération synchrone
                    conn.Open();

                    string query = @"
                        INSERT INTO logs (user_id, message, created_at)
                        VALUES (@user_id, @message, GETDATE());";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@user_id", SqlDbType.Int).Value = userId;
                        // Gestion si le message est null
                        cmd.Parameters.Add("@message", SqlDbType.NVarChar).Value = (object)message ?? DBNull.Value;

                        // Opération synchrone
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // si il y'a une erreure lors de l'écriture du log  
                Console.WriteLine($"Erreur lors de l'écriture du log : {ex.Message}");
            }
        }

        public void CreateLog(string message)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("logs.txt", true))
                {
                    sw.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Log error pour les logs " + e.Message);
            }
        }

    }
}