using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Thiskord_Back.Services;

namespace Thiskord_Back.Tests.Helpers
{
    public class TestWebAppFactory : WebApplicationFactory<Program>
    {
        public Mock<IDbConnectionService> MockDbService { get; } = new Mock<IDbConnectionService>();
        public string ConnectionString { get; set; } = "";
        
        public const string TestJwtKey = "THIS_IS_A_TEST_SECRET_KEY_32CHARS!";
        public const string TestJwtIssuer = "TestIssuer";
        public const string TestJwtAudience = "TestAudience";
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"]      = TestJwtKey,
                    ["Jwt:Issuer"]   = TestJwtIssuer,
                    ["Jwt:Audience"] = TestJwtAudience,
                });
            });
            
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IDbConnectionService));
                if (descriptor != null)
                    services.Remove(descriptor);

                MockDbService
                    .Setup(db => db.CreateConnection())
                    .Returns(() => new Microsoft.Data.SqlClient.SqlConnection(ConnectionString));

                services.AddSingleton<IDbConnectionService>(MockDbService.Object);
            });
            
            builder.UseSetting("ASPNETCORE_URLS", "http://+");
        }
    }
}