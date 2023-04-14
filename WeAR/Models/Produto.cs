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
        public String desc, nome, tipo;
        public Double preco;
        public int qntd, tamanho;
        //Faz a conexão com o banco de dados
        const string stringConexao = "Data Source=tcp:weardbserver.database.windows.net,1433;Initial Catalog=WeAR_db;User Id=WeARTech@weardbserver;Password=WearTec1234";
        SqlConnection conecta = new SqlConnection(stringConexao); //Variavel que faaz a conexão com o banco
        SqlCommand query; //Variavel que faz os comandos


        //Construtor vazio
        public Produto()
        {
        }



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

        //Pega id do produto com base no seu nome
        public int pegarID(string nome)
        {
            //Comando para pegar o id
            query = new SqlCommand("SELECT id FROM Produto where nome=@nome");
            query.Parameters.AddWithValue("@nome", nome);
            SqlDataReader leitor = query.ExecuteReader();

            int id = int.Parse(leitor["id"].ToString());

            return id;
        }

        //Busca um produto específico através do  ID
        public Produto buscaProduto(int id)
        {
            try
            {
                conecta.Open(); //Abre a conexão
                query = new SqlCommand("SELECT * from Produto where id=@id"); //Query que pega os produtos conforme o ID
                query.Parameters.AddWithValue("@id", id);  //Parametros para evitar SQLInjection
                SqlDataReader leitor = query.ExecuteReader();

                if (leitor.HasRows) // Se o leitor retornar algo ele executa a função
                {
                    Produto p; //Objeto da classe Produto

                    //Query que faz a leitura da tabela Oculos em busca de algum produto com o id inserido
                    SqlCommand query2 = new SqlCommand("Select * from Oculos where fk_Produto_id=@id");
                    query2.Parameters.AddWithValue("@id", id);
                    SqlDataReader leitor2 = query2.ExecuteReader();


                    if (leitor2.HasRows) //Se possui linhas, significa que é um Óculos, e não um Anel, pois o query retornou algo
                    {
                        //Constroi o objeto P de produto, com base nas informações retornadas da tabela Produto e da tabela Óculos
                        p = new Produto(leitor["descricao"].ToString(), leitor["nome"].ToString(), 
                            leitor2["categoria"].ToString(), (double)leitor["preco"], (int)leitor["quantidade"]);

                        return p;
                    }
                    else //Se não possui linhas, é um anel
                    {
                        //Query que faz a leitura da tabela Anel
                        SqlCommand query3 = new SqlCommand("Select * from Anel where fk_Produto_id=@id");
                        query2.Parameters.AddWithValue("@id", id);
                        SqlDataReader leitor3 = query2.ExecuteReader();

                        //Constroi o objeto P de produto, com base nas informações da tabela Produto e Anel
                        p = new Produto(leitor["descricao"].ToString(), leitor["nome"].ToString(), 
                            (double)leitor["preco"], (int)leitor["quantidade"], (int)leitor2["tamanho"]);

                        return p;
                    }
                }
                else
                {
                    return null;
                }


            }
            catch (Exception e)
            {

                throw;
            }
            finally //Independente do que ocorra, fecha a conexão
            {
                conecta.Close();
            }
        }

        //Método que pega todas as informações de todos os óculos

        public List<Produto> pegarTodosOculos(String categoria)
        {
            try
            {
                List<Produto> produtos = new List<Produto>(); //Lista de produtos que será retornada pelo método
                Produto p; //Objeto de produto

                //Query que pega tudo de produto
                conecta.Open();
                query = new SqlCommand("Select * from Produto");
                SqlDataReader leitor = query.ExecuteReader();
                

                //Query que pega tudo de óculos quando a categoria for a mesma que a selecionada
                SqlCommand query2 = new SqlCommand("Select * from Oculos where categoria=@categoria");
                query2.Parameters.AddWithValue("@categoria", categoria);
                SqlDataReader leitor2 = query2.ExecuteReader();

                    while (leitor.Read())
                {
                    if (leitor["id"] == leitor2["fk_Produto_id"])
                    {
                        p = new Produto(leitor["descricao"].ToString(), leitor["nome"].ToString(),
                                                    leitor2["categoria"].ToString(), (double)leitor["preco"], (int)leitor["quantidade"]); produtos.Add(p);
                        produtos.Add(p);
                    }
                    else
                    {

                    }
                }              
                   
                return produtos;

            }
            catch (Exception e)
            {
                return null;
                throw;
            }
            finally
            {
                conecta.Close();
            }



        }

        //Método que pega todas as informações de todos os aneis 
        public List<Produto> pegarTodosAneis()
        {
            try
            {
                List<Produto> produtos = new List<Produto>(); //Lista de produtos que será retornada pelo método
                Produto p; //Objeto de produto

                //Query que pega tudo de produto
                conecta.Open();
                query = new SqlCommand("Select * from Produto");
                SqlDataReader leitor = query.ExecuteReader();

                //Query que pega tudo de Anel
                SqlCommand query2 = new SqlCommand("Select * from Anel");
                SqlDataReader leitor2 = query2.ExecuteReader();

                while (leitor.Read())
                {
                    if (leitor["id"] == leitor2["fk_Produto_id"])
                    {
                        p = new Produto(leitor["descricao"].ToString(), leitor["nome"].ToString(),
                                         (double)leitor["preco"], (int)leitor["quantidade"], (int)leitor2["tamanho"]);

                        produtos.Add(p);
                    }
                    else
                    {

                    }
                }

                return produtos;

            }
            catch (Exception e)
            {
                return null;
                throw;
            }
            finally
            {
                conecta.Close();
            }



        }









    }
}
