using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace WeAR.Models
{
    public class Produto
    {
        //Definição de atributos da classe
        public String desc, nome, tipo;
        public Double preco;
        public int qntd;
        public int tamanho = 0;
        public int[] similaresid = new int[4];
        public bool modelo3d;
        //Faz a conexão com o banco de dados
        const string stringConexao2 = "Data Source=tcp:weardbserver.database.windows.net,1433;Initial Catalog=WeAR_db;User Id=WeARTech@weardbserver;Password=WearTec1234";
        const string stringConexao = "Server=tcp:weardbserver.database.windows.net,1433;Initial Catalog=WeAR_db;Persist Security Info=False;User ID=WeARTech;Password=WearTec1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=10;";
        SqlConnection conecta = new SqlConnection(stringConexao2); //Variavel que faz a conexão com o banco
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
            try
            {
                conecta.Open();

                //Comando para pegar o id
                query = new SqlCommand("SELECT id FROM Produto where nome=@nome", conecta);
                query.Parameters.AddWithValue("@nome", nome);
                SqlDataReader leitor = query.ExecuteReader();

                leitor.Read(); //Faz a leitura dos dados

                //Pega o ID
                int id = int.Parse(leitor["id"].ToString());

                return id;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                conecta.Close();
            }
        }

        //Busca um produto específico através do  ID
        public Produto buscaProduto(int id)
        {
            try
            {
                conecta.Open(); //Abre a conexão
                query = new SqlCommand("SELECT * from Produto where id=@id", conecta); //Query que pega os produtos conforme o ID
                query.Parameters.AddWithValue("@id", id);  //Parametros para evitar SQLInjection
                SqlDataReader leitor = query.ExecuteReader();

                if (leitor.HasRows) // Se o leitor retornar algo ele executa a função
                {
                    Produto p = new Produto(); //Objeto da classe Produto



                    //Query que faz a leitura da tabela Oculos em busca de algum produto com o id inserido
                    SqlConnection conecta2 = new SqlConnection(stringConexao); //Variavel que faaz a conexão com o banco
                    conecta2.Open();
                    SqlCommand query2 = new SqlCommand("Select * from Oculos where fk_Produto_id=@id", conecta2);
                    query2.Parameters.AddWithValue("@id", id);
                    SqlDataReader leitor2 = query2.ExecuteReader();


                    if (leitor2.HasRows) //Se possui linhas, significa que é um Óculos, e não um Anel, pois o query retornou algo
                    {
                        if (leitor.Read() && leitor2.Read())
                        {
                            //Constroi o objeto P de produto, com base nas informações retornadas da tabela Produto e da tabela Óculos
                            p = new Produto(leitor["descricao"].ToString(), leitor["nome"].ToString(),
                                leitor2["categoria"].ToString(), Double.Parse(leitor["preco"].ToString()), (int)leitor["quantidade"]);

                        }
                        conecta2.Close();
                        return p;
                    }
                    else //Se não possui linhas, é um anel
                    {
                        //Query que faz a leitura da tabela Anel
                        SqlConnection conecta3 = new SqlConnection(stringConexao); //Variavel que faaz a conexão com o banco
                        conecta3.Open();
                        SqlCommand query3 = new SqlCommand("Select * from Anel where fk_Produto_id=@id", conecta3);
                        query3.Parameters.AddWithValue("@id", id);
                        SqlDataReader leitor3 = query3.ExecuteReader();

                        //Constroi o objeto P de produto, com base nas informações da tabela Produto e Anel
                        if (leitor.Read() && leitor3.Read())
                        {
                            p = new Produto(leitor["descricao"].ToString(), leitor["nome"].ToString(),
                                 Double.Parse(leitor["preco"].ToString()), (int)leitor["quantidade"], int.Parse(leitor3["tamanho"].ToString()));

                        }
                        conecta3.Close();
                        conecta2.Close();
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


        //Método para pegar todos os valores de tamanho do anel
        public int[] pegarTamanhoAneis(int id)
        {
            try
            {
                int[] tamanhos = new int[20]; //Array com os tamanhos dos aneis
                int contador = 0; //Contator para ser usado no while

                conecta.Open(); //Abre a conexão
                query = new SqlCommand("SELECT * from Anel where fk_Produto_id=@id", conecta); //Query que pega os aneis conforme o ID de Produto
                query.Parameters.AddWithValue("@id", id);  //Parametros para evitar SQLInjection
                SqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read()) //Enquanto conseguir ler, ele adiciona o valor do leitor no Array
                {
                    tamanhos[contador] = (int)leitor["tamanho"];
                    contador++;
                }

                return tamanhos;

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


                conecta.Open(); //Abrindo conexão

                /*Query que pega todos os valores de produtos, quando o id for correspondente a FK de Óculos, e, filtra pela categoria que foi inserida*/
                query = new SqlCommand("SELECT * FROM [dbo].[Produto] inner join [dbo].[Oculos] on [dbo].[Produto].id = " +
                    "[dbo].[Oculos].fk_Produto_id WHERE categoria = @categoria", conecta);

                query.Parameters.AddWithValue("@categoria", categoria); //Parameters para evitar SQL INJECTION
                SqlDataReader leitor = query.ExecuteReader();



                while (leitor.Read())
                {

                    p = new Produto(leitor["descricao"].ToString(), leitor["nome"].ToString(),
                                                leitor["categoria"].ToString(), Double.Parse(leitor["preco"].ToString()), (int)leitor["quantidade"]);
                    produtos.Add(p);
                }

                return produtos;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "---------------------------------------------------------------------");
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

                /*Query que pega todos os valores de produto, e seleciona os valores correspondentes entre as duas tabelas (ou seja, exclui
                 * os valores quando, na tabela de produto, não for um anel), e, seleciona apenas um tamanho, para que seja puxado 1 exemplar
                 de cada produto, pois, para o catálogo, não é necessário todos os tamanhos disponíveis para todos os produtos*/
                conecta.Open();
                query = new SqlCommand("SELECT * from [dbo].[Produto] INNER JOIN [dbo].[Anel] on [dbo].[Produto].id = [dbo].[Anel].fk_Produto_id WHERE tamanho = 10", conecta);
                SqlDataReader leitor = query.ExecuteReader();


                while (leitor.Read())
                {
                    p = new Produto(leitor["descricao"].ToString(), leitor["nome"].ToString(),
                                          Double.Parse(leitor["preco"].ToString()), (int)leitor["quantidade"], (int)leitor["tamanho"]);

                    produtos.Add(p);
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

        //Método que pega todos os produtos para fazer o catálogo geral
        public List<Produto> pegarTodosProdutos()
        {
            try
            {
                List<Produto> produtos = new List<Produto>(); //Lista de produtos que será retornada pelo método
                Produto p; //Objeto de produto

                conecta.Open(); //Abrindo conexão

                //Comando para pegar tudo
                query = new SqlCommand("Select * from Produto", conecta);
                SqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read()) //Pega todos os valores que vem do query, e adiciona a lista de produtos
                {
                    p = new Produto(leitor["descricao"].ToString(), leitor["nome"].ToString(),
                                          Double.Parse(leitor["preco"].ToString()), (int)leitor["quantidade"], 0);

                    produtos.Add(p);
                }

                return produtos;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
            finally
            {
                conecta.Close();

            }


        }

        //Metodo que devolve true or false, dependendo se o produto possui ou não modelo3d, usado para ativar ou desativar botão de tryon
        public bool valorModelo3d(int id)
        {
            try
            {
                bool resposta = new bool(); //valor bool da resposta

                //Query que pesquisa se o modelo3d do id inserido é true ou false
                conecta.Open();
                query = new SqlCommand("Select modelo3d from Produto where id=@id", conecta);
                query.Parameters.AddWithValue("@Id", id);
                SqlDataReader leitor = query.ExecuteReader();

                //Leitura do query
                if (leitor.Read())
                {
                    if (leitor.GetBoolean("modelo3d") == true)
                    {
                        resposta = leitor.GetBoolean("modelo3d");
                    }
                }

                return resposta; //return

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                conecta.Close();
            }
        }



        /*Método que sorteia id's aleatórios dentro da mesma categoria do produto desejado (se anel, devolve vários ids de aneis, se oculos devolve id's de óculos)
         e retorna a lista de imagens desses produtos, em ordem crescente de acordo com o ID, alem de definir a variavel int[] similaresid com os valores que foram
        sorteados pelo método
         */
        public List<String> produtosSimilares(string tabela, int id)
        {
            try
            {

                List<String> similares = new List<String>(); //Lista que salvará as imagens
                int[] valores; //Lista usada para descobrir quais os ids que possuem o tipo de produto desejado (anel ou oculos)
                int[] produtos = new int[4]; //Lista que irá possuir os valores sorteados no random
                String imagem = null; //Variavel com a imagem puxada do banco pelo método da classe Imagem
                Random rnd = new Random(); //Variavel que sorteará numeros


                //Query que pega os valores dos ID's dos produtos, quando ele está na tabela de produtos e na tabela desejada (anel ou oculos)
                conecta.Open();

                /*Neste query, tabela não pode ser passado como Parametro, pois se refere ao nome de uma tabela, 
                 * e, como não é possivel SQLInjection, pelo fato de o usuário não ter como mudar esse valor, o 
                 * código segue seguro mesmo com essa declaração explicita*/
                query = new SqlCommand("SELECT Produto.id from Produto INNER JOIN " + tabela +" on Produto.id = " + tabela + ".fk_Produto_id GROUP BY Produto.id", conecta);
                SqlDataReader leitor = query.ExecuteReader();

                int size = 0;//Variavel usada para descobrir o tamanho do retorno do query

                //Função para descobrir o tamanho total do retorno query
                while (leitor.Read())
                {                    
                    size++;
                }
                leitor.Close();


                valores = new int[size];//Define a variavel de valores com o tamanho do retorno              
                size = 0; //Zera novamente o valor para ser usado denovo

                //Função para associar cada valor da Array valores, com seu respectivo valor do banco de dados
                leitor = query.ExecuteReader();
                while (leitor.Read())
                {
                    valores[size] = (int)leitor["id"];
                    size++;
                }
                leitor.Close();


                //Função para sortear 4 numeros aleatórios, sem repeti-los, e, sem que sejam iguais ao id inicial que foi enviado através do método
                for (int i = 0; i < 4; i++)
                {
                    produtos[i] = rnd.Next(valores[0], valores[valores.Length - 1] + 1);
                    for (int j = 0; j < 4; j++)
                    {
                        if (produtos[j] == produtos[i] && j!=i || produtos[i] == id)
                        {
                            i--;
                            break;
                        }
                    }
                    
                }

                Array.Sort(produtos);//Coloca a array em ordem crescente de valores

                //Função que, para cada um dos valores do Array, busca sua respectiva imagem no banco de dados, e adiciona a List<String> similares
                leitor = query.ExecuteReader();
                while (leitor.Read())
                {
                    size = 0;
                    foreach (int i in produtos)
                    {                       
                        if (leitor.GetInt32("id") == produtos[size])
                        {
                            Imagem img = new Imagem();
                            imagem = img.PegarImagem(produtos[size]);

                            similares.Add("data:image/jpeg;base64," + imagem);

                        }
                        size++;
                    }
                }


                similaresid = produtos; //Define os id's dos similares com os valores obtidos randomicamente

                return similares; //return

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                conecta.Close();
            }


        }



    }
}
