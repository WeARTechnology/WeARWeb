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
        SqlConnection conecta = CreateConnection.getAzureConnection(); //Variavel que faaz a conexão com o banco
        SqlCommand query; //Variavel que faz os comandos


        /*                                      Métodos                                     */

        //Metodo que adiciona uma imagem ao banco de dadosZ
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
                        query = new SqlCommand("Insert into Imagem(fk_Produto_id) VALUES (@id,@imagem)", conecta); //Define o comando SQL
                        //Parameters para evitar SQLInjection
                        query.Parameters.AddWithValue("@imagem", bytesArquivo);
                        query.Parameters.AddWithValue("@id", id);
                       

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


        //Método que pega todas as imagens do Banco de dados
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

        //Método que pega uma imagem específica através do seu ID
        public String PegarImagem(int id)
        {
            String imagem = null;
            try
            {
                conecta.Open();
                query = new SqlCommand("Select  * from Imagem where fk_Produto_id = @id and noStock = 0 and round_android = 0", conecta); //Define o comando SQL
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

        //Método que pega uma imagem específica através do seu ID
        public String PegarImagemNoStock(int id)
        {
            String imagem = null;
            try
            {
                conecta.Open();
                query = new SqlCommand("Select  * from Imagem where fk_Produto_id = @id and noStock = 1", conecta); //Define o comando SQL
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

        public String PegarImagemRound(int id)
        {
            String imagem = null;
            try
            {
                conecta.Open();
                query = new SqlCommand("Select  * from Imagem where fk_Produto_id = @id and round_android = 1", conecta); //Define o comando SQL
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
