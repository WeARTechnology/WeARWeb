using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.IO;

namespace WeAR.Models
{
    public class CreateConnection
    {
        public static SqlConnection getAzureConnection()
        {
            return new SqlConnection(Configuration().GetConnectionString("Azure"));
        }

        public static SqlConnection getAzure2Connection()
        {
            return new SqlConnection(Configuration().GetConnectionString("Azure2"));
        }

        private static IConfigurationRoot Configuration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            return builder.Build();
        }
    }
}
