using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeAR.Models
{
    public class Produto
    {
        //Definição de atributos da classe
        String desc, nome, tipo;
        Double preco;
        int qntd, tamanho;
        //Faz a conexão com o banco de dados
        const string stringConexao = "Server=tcp:weardbserver.database.windows.net,1433;Initial Catalog=WeAR_db;Persist Security Info=False;User ID=WeARTech;Password=WearTec1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        SqlConnection conecta = new SqlConnection(stringConexao); //Variavel que faaz a conexão com o banco
        SqlCommand query; //Variavel que faz os comandos

        //Construtor do Anel (Somente tamanho, sem tipo)
        public Produto(string desc, string nome, double preco, int qntd, int tamanho)
        {
            this.desc = desc;
            this.nome = nome;
            this.preco = preco;
            this.qntd = qntd;
            this.tamanho = tamanho;
        }

        //Construtor do Óculos (Somente tipo, sem tamanho)
        public Produto(string desc, string nome, string tipo, double preco, int qntd)
        {
            this.desc = desc;
            this.nome = nome;
            this.tipo = tipo;
            this.preco = preco;
            this.qntd = qntd;
        }

        public Produto buscaProduto(int id )
        {
            try
            {
                conecta.Open();
                query = new SqlCommand("SELECT * from Produto where id=@id");
                query.Parameters.AddWithValue("@id", id);
                SqlDataReader leitor = query.ExecuteReader();

                if (leitor.HasRows)
                {
                    Produto p;
                                        

                    if (id < 8 || id > 11 && id < 15)
                    {
                        SqlCommand query2 = new SqlCommand("Select * from Oculos where id=@id");
                        query2.Parameters.AddWithValue("@id", id);
                        SqlDataReader leitor2 = query2.ExecuteReader();

                        p = new Produto(leitor["descricao"].ToString(), leitor["nome"].ToString(),leitor2["categoria"].ToString(), (double)leitor["preco"], (int)leitor["quantidade"]);

                        return p;
                    }
                    else
                    {
                        SqlCommand query2 = new SqlCommand("Select * from Oculos where id=@id");
                        query2.Parameters.AddWithValue("@id", id);
                        SqlDataReader leitor2 = query2.ExecuteReader();

                        p = new Produto(leitor["descricao"].ToString(), leitor["nome"].ToString(), (double)leitor["preco"], (int)leitor["quantidade"], (int) leitor2["tamanho"]);

                        return p;
                    }
                }
                else
                {
                    return null;
                }


            }
            catch (Exception e )
            {
                
                throw;
            }
            finally
            {
                conecta.Close();
            }
        }

        public List<Produto> pegarTudo(string categoria)
        {
            return null;



        }



        


    }
}
