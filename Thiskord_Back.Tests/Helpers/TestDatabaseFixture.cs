using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace Thiskord_Back.Tests.Helpers
{
    public class TestDatabaseFixture : IDisposable
    {
        public string ConnectionString { get; }
        private readonly SqlConnection _dbConn;

        public TestDatabaseFixture()
        {
            string dbName = $"Thiskord_Test_{Guid.NewGuid():N}";

            string baseConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? "Server=localhost,1433;User Id=sa;Password=YourLocalPassword;TrustServerCertificate=True;";

            var masterConnectionString = Regex.Replace(
                baseConnectionString,
                @"Database=[^;]+;?",
                "",
                RegexOptions.IgnoreCase
            ) + $";Database=master;";

            ConnectionString = Regex.Replace(
                baseConnectionString,
                @"Database=[^;]+",
                $"Database={dbName}",
                RegexOptions.IgnoreCase
            );

            using var masterConn = new SqlConnection(masterConnectionString);
            masterConn.Open();
            new SqlCommand($"CREATE DATABASE [{dbName}]", masterConn).ExecuteNonQuery();

            string script = File.ReadAllText("Scripts/Thiskord_db_tests.sql");

            var statements = Regex.Split(script, @"(?<=\bEND\b)\s*\n")
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s));

            _dbConn = new SqlConnection(ConnectionString);
            _dbConn.Open();

            foreach (var statement in statements)
            {
                new SqlCommand(statement, _dbConn).ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            _dbConn?.Close();
            _dbConn?.Dispose();

            string masterConnectionString = Regex.Replace(
                ConnectionString,
                @"Database=[^;]+",
                "Database=master",
                RegexOptions.IgnoreCase
            );

            using var masterConn = new SqlConnection(masterConnectionString);
            masterConn.Open();

            string dbName = Regex.Match(ConnectionString, @"Database=([^;]+)", RegexOptions.IgnoreCase).Groups[1].Value;
            new SqlCommand($"ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{dbName}]", masterConn).ExecuteNonQuery();
        }
    }
}
