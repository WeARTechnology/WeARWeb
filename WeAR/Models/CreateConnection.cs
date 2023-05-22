using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.IO;

namespace WeAR.Models
{

    /*Classe que cria uma String de conexão com o banco com base nos valores contidos no appSetings.json*/
    public class CreateConnection
    {

        //Método que pega a conexão de nome Azure
        public static SqlConnection getAzureConnection()
        {
            return new SqlConnection(Configuration().GetConnectionString("Azure"));
        }

        //Método que pega a conexao de nome Azure2
        public static SqlConnection getAzure2Connection()
        {
            return new SqlConnection(Configuration().GetConnectionString("Azure2"));
        }

        //Método que constroi uma configuração do aplicativo, definindo como caminho o objeto JSON appsetings
        private static IConfigurationRoot Configuration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            return builder.Build();
        }
    }
}
