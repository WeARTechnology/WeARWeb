using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace WeAR.Models
{
    public class Imagem
    {

        //Faz a conexão com o banco de dados
        const string stringConexao = "Server=tcp:weardbserver.database.windows.net,1433;Initial Catalog=WeAR_db;Persist Security Info=False;User ID=WeARTech;Password=WearTec1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        const string stringConexao2 = "Data Source=tcp:weardbserver.database.windows.net,1433;Initial Catalog=WeAR_db;User Id=WeARTech@weardbserver;Password=WearTec1234";
        SqlConnection conecta = new SqlConnection(stringConexao2); //Variavel que faaz a conexão com o banco
        SqlCommand query; //Variavel que faz os comandos



        //Métodos
        //Metodo que cadastra o cliente
        public string addImagem(int id, IFormFile arq)
        {
            String tipoArquivo = arq.ContentType;

            if (tipoArquivo.Contains("image")) //Vê se o arquivo enviado é uma imagem
            {
                {//se for imagem eu vou gravar no banco
                    MemoryStream s = new MemoryStream();
                    arq.CopyTo(s);
                    byte[] bytesArquivo = s.ToArray(); //Transforma o arquivo em uma sequencia de bytes para enviar ao banco

                    try
                    {
                        conecta.Open();
                        query = new SqlCommand("Insert into Imagem(fk_Produto_id,imagem) VALUES (@id2,@imagem)", conecta); //Define o comando SQL
                        //Parameters para evitar SQLInjection
                        query.Parameters.AddWithValue("@imagem", bytesArquivo);
                        query.Parameters.AddWithValue("@id2", id);
                       

                        query.ExecuteNonQuery(); //Executa o comando
                        return "Sucesso";
                    }
                    catch (Exception f) //Em caso de erro, retorna o erro
                    {
                        return "Erro" + f.ToString();

                    }
                    finally
                    {
                        conecta.Close(); //Fecha a conexão independente do caso

                    }
                }

            }
            return null;
        }

        
        public List<String> PegarTudo()
        {
            List<String> listaImagens = new List<string>();
            try
            {
                conecta.Open();
                query = new SqlCommand("Select  * from Imagem", conecta); //Define o comando SQL
                SqlDataReader leitor = query.ExecuteReader();
                while (leitor.Read())
                {
                    byte[] bytesArquivo = (byte[])leitor["imagem"];
                    listaImagens.Add(Convert.ToBase64String(bytesArquivo));
                }
                
            }
            catch (Exception f) //Em caso de erro, retorna o erro
            {
                throw;

            }
            finally
            {
                conecta.Close(); //Fecha a conexão independente do caso

            }
            return listaImagens;
        }

        public String PegarImagem(int id)
        {
            String imagem = null;
            try
            {
                conecta.Open();
                query = new SqlCommand("Select  * from Imagem where fk_Produto_id = @id", conecta); //Define o comando SQL
                query.Parameters.AddWithValue("@id", id);
                SqlDataReader leitor = query.ExecuteReader();
                while (leitor.Read())
                {
                    byte[] bytesArquivo = (byte[])leitor["imagem"];
                    imagem = (Convert.ToBase64String(bytesArquivo));
                }

            }
            catch (Exception f) //Em caso de erro, retorna o erro
            {
                throw;

            }
            finally
            {
                conecta.Close(); //Fecha a conexão independente do caso

            }
            return imagem;
        }
    }
}
