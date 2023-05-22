using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace WeAR.Models
{
    /*Classe que lida com as imagens do banco de dados*/
    public class Imagem
    {
        //Faz a conexão com o banco de dados
        SqlConnection conecta = CreateConnection.getAzureConnection(); //Variavel que faz a conexão com o banco
        SqlCommand query; //Variavel que faz os comandos


        /*                                      Métodos                                     */

        //Metodo que adiciona uma imagem ao banco de dados
        public string addImagem(int id, IFormFile arq)
        {
            //Descobre qual o tipo de arquivo foi enviado
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
            List<String> listaImagens = new List<string>(); //Cria uma lista de string para as imagens
            try
            {
                conecta.Open(); //Abre a conexão
                query = new SqlCommand("Select  * from Imagem", conecta); //Define o comando SQL
                SqlDataReader leitor = query.ExecuteReader(); //Define o leitor do comando
                while (leitor.Read()) //Enquanto ele ler, ou seja, enquanto ainda houverem valores
                {
                    byte[] bytesArquivo = (byte[])leitor["imagem"]; //Transforma o retorno do leitor, em uma sequencia de bytes
                    listaImagens.Add(Convert.ToBase64String(bytesArquivo)); //Adiciona estes bytes a lista
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

            return listaImagens; //Retorna a lista de imagens, independente se conseguiu buscar no banco ou não
        }

        //Método que pega uma imagem específica através do seu ID
        public String PegarImagem(int id)
        {
            String imagem = null; //Define a imagem

            try
            {
                conecta.Open(); //Abre a conexão
                query = new SqlCommand("Select  * from Imagem where fk_Produto_id = @id and noStock = 0 and round_android = 0", conecta); //Define o comando SQL
                //Parameters para evitar SQLINjection
                query.Parameters.AddWithValue("@id", id);
                
                SqlDataReader leitor = query.ExecuteReader();   //Define um leitor
                while (leitor.Read()) //Enquanto o leitor ler, ou seja, enquanto houverem valores a serem lidos
                {
                    byte[] bytesArquivo = (byte[])leitor["imagem"]; //Transforma a imagem obtida do leitor em um array de bytes
                    imagem = (Convert.ToBase64String(bytesArquivo)); //Define o valor da string imagem como o array de bytes convertido em Base64
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
            //Retorna a imagem, independente se conseguiu buscar ou não
            return imagem;
        }

        //Método que pega uma imagem específica através do seu ID, quando o produto está sem estoque
        public String PegarImagemNoStock(int id)
        {
            String imagem = null; //Define a imagem

            try
            {
                conecta.Open(); //Abre a conexão
                query = new SqlCommand("Select  * from Imagem where fk_Produto_id = @id and noStock = 1", conecta); //Define o comando SQL
                //Parameters para evitar SQLINjection
                query.Parameters.AddWithValue("@id", id);

                SqlDataReader leitor = query.ExecuteReader();   //Define um leitor
                while (leitor.Read()) //Enquanto o leitor ler, ou seja, enquanto houverem valores a serem lidos
                {
                    byte[] bytesArquivo = (byte[])leitor["imagem"]; //Transforma a imagem obtida do leitor em um array de bytes
                    imagem = (Convert.ToBase64String(bytesArquivo)); //Define o valor da string imagem como o array de bytes convertido em Base64
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
            //Retorna a imagem, independente se conseguiu buscar ou não
            return imagem;
        }


        //Método que pega uma imagem específica através do seu ID, quando a imagem é no formato redondo (Android)
        public String PegarImagemRound(int id)
        {
            String imagem = null; //Define a imagem

            try
            {
                conecta.Open(); //Abre a conexão
                query = new SqlCommand("Select  * from Imagem where fk_Produto_id = @id and round_android = 1", conecta); //Define o comando SQL
                //Parameters para evitar SQLINjection
                query.Parameters.AddWithValue("@id", id);

                SqlDataReader leitor = query.ExecuteReader();   //Define um leitor
                while (leitor.Read()) //Enquanto o leitor ler, ou seja, enquanto houverem valores a serem lidos
                {
                    byte[] bytesArquivo = (byte[])leitor["imagem"]; //Transforma a imagem obtida do leitor em um array de bytes
                    imagem = (Convert.ToBase64String(bytesArquivo)); //Define o valor da string imagem como o array de bytes convertido em Base64
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
            //Retorna a imagem, independente se conseguiu buscar ou não
            return imagem;
        }
    }
}
