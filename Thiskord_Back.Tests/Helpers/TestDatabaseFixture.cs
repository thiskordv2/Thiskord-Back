using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Thiskord_Back.Tests.Helpers
{
    public class TestDatabaseFixture : IDisposable
    {
        public string ConnectionString { get; }
        private readonly string _dbName;
        private readonly string _masterConnectionString;

        public TestDatabaseFixture()
        {
            _dbName = $"Thiskord_Test_{Guid.NewGuid():N}";

            var baseConnectionString =
                Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? "Server=localhost,1433;User Id=sa;Password=YourLocalPassword;TrustServerCertificate=True;Encrypt=False;";

            var baseBuilder = new SqlConnectionStringBuilder(baseConnectionString);

            _masterConnectionString = new SqlConnectionStringBuilder(baseConnectionString)
            {
                InitialCatalog = "master"
            }.ConnectionString;

            baseBuilder.InitialCatalog = _dbName;
            ConnectionString = baseBuilder.ConnectionString;

            using (var masterConn = new SqlConnection(_masterConnectionString))
            {
                masterConn.Open();
                using var createDbCmd = new SqlCommand($"CREATE DATABASE [{_dbName}]", masterConn);
                createDbCmd.ExecuteNonQuery();
            }

            var scriptPath = Path.Combine(AppContext.BaseDirectory, "Scripts", "Thiskord_db_tests.sql");
            string script = File.ReadAllText(scriptPath);

            var statements = Regex.Split(script, @"(?<=\bEND\b)\s*\n")
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s));

            using (var dbConn = new SqlConnection(ConnectionString))
            {
                dbConn.Open();

                foreach (var statement in statements)
                {
                    using var cmd = new SqlCommand(statement, dbConn);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Dispose()
        {
            using var masterConn = new SqlConnection(_masterConnectionString);
            masterConn.Open();

            using var cmd = new SqlCommand($@"
                IF DB_ID('{_dbName}') IS NOT NULL
                BEGIN
                    ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{_dbName}];
                END", masterConn);

            cmd.ExecuteNonQuery();
        }
    }
}
