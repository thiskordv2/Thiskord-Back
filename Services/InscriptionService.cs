using Microsoft.Data.SqlClient;

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

        public async Task<int> InscriptionUser(string user_name, string user_password)
        {
            
        }   