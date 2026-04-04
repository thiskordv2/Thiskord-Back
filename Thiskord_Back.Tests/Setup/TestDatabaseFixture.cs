using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace Thiskord_Back.Tests.Setup
{
    // TestDatabaseFixture.cs
    public class TestDatabaseFixture : IDisposable
    {
        public string ConnectionString { get; }
        private readonly SqlConnection _dbConn;
        
        public TestDatabaseFixture()
        {
            string dbName = $"Thiskord_Test_{Guid.NewGuid():N}";
            ConnectionString = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Integrated Security=true;";

            using var masterConn = new SqlConnection("Server=(localdb)\\mssqllocaldb;Integrated Security=true;");
            masterConn.Open();
            new SqlCommand($"CREATE DATABASE [{dbName}]", masterConn).ExecuteNonQuery();

            string script = File.ReadAllText("Scripts/Thiskord_db_tests.sql");

            // Split sur END suivi d'un saut de ligne (fin de bloc IF/BEGIN/END)
            var statements = Regex.Split(script, @"(?<=\bEND\b)\s*\n")
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s));

            var dbConn = new SqlConnection(ConnectionString);
            dbConn.Open();

            foreach (var statement in statements)
            {
                try
                {
                    new SqlCommand(statement, dbConn).ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"SQL Warning: {ex.Message}");
                }
            }
            _dbConn = dbConn;
        }
        
        public void Dispose()
        {
            _dbConn?.Close();
            _dbConn?.Dispose();

            SqlConnection.ClearAllPools();

            using var conn = new SqlConnection("Server=(localdb)\\mssqllocaldb;Integrated Security=true;");
            conn.Open();
            string dbName = new SqlConnectionStringBuilder(ConnectionString).InitialCatalog;
            new SqlCommand($"ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{dbName}]", conn).ExecuteNonQuery();
        }
    }
}

