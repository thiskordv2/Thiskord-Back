using Microsoft.Data.SqlClient;
using System.Runtime.ConstrainedExecution;
namespace Thiskord_Back.Services
{

    public interface IDbConnectionService
    {
        SqlConnection CreateConnection();
    }
    public class DBService : IDbConnectionService
    {
        private readonly IConfiguration _config;
        private readonly IDbConnectionService _db;

        public DBService(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

    }
}
