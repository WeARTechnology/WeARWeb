using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
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
        //Valores do objeto de Produto que é retornado do banco, como nome, quantidade, etc..
        public String desc, nome, tipo;
        public Double preco;
        public int qntd;
        public int tamanho = 0;
        public bool modelo3d;
        public int id;

        //Array de inteiros com os ID's de produtos similares (Método produtosSimilares())
        public int[] similaresid = new int[4];
        MySqlConnection conecta = CreateConnection.getFreeSQLConnection(); //Variavel que faz a conexão com o banco
        MySqlCommand query; //Variavel que faz os comandos


        //Construtor vazio
        public Produto()
        {
        }



        //Construtor do Anel (Somente tamanho, sem tipo)
        public Produto(int id, string desc, string nome, double preco, int qntd, int tamanho, bool modelo3d)
        {
            this.id = id;
            this.desc = desc;
            this.nome = nome;
            this.preco = preco;
            this.qntd = qntd;
            this.tamanho = tamanho;
            this.modelo3d = modelo3d;
        }

        //Construtor do Óculos (Somente tipo, sem tamanho)
        public Produto(int id, string desc, string nome, string tipo, double preco, int qntd, bool modelo3d)
        {
            this.id = id;
            this.desc = desc;
            this.nome = nome;
            this.tipo = tipo;
            this.preco = preco;
            this.qntd = qntd;
            this.modelo3d = modelo3d;
        }

        //Construtor do Anel (Somente tamanho, sem tipo)
        public Produto(int id, string desc, string nome, double preco, int qntd, int tamanho, bool modelo3d, string tipo)
        {
            this.id = id;
            this.desc = desc;
            this.nome = nome;
            this.preco = preco;
            this.qntd = qntd;
            this.tamanho = tamanho;
            this.modelo3d = modelo3d;
            this.tipo = tipo;
        }


        //Busca um produto específico através do  ID
        public Produto BuscaProduto(int id)
        {
            try
            {
                conecta.Open(); //Abre a conexão
                query = new MySqlCommand("SELECT * from Produto where id=@id", conecta); //Query que pega os produtos conforme o ID
                query.Parameters.AddWithValue("@id", id);  //Parametros para evitar SQLInjection
                MySqlDataReader leitor = query.ExecuteReader();

                if (leitor.HasRows) // Se o leitor retornar algo ele executa a função
                {
                    Produto p = new Produto(); //Objeto da classe Produto


                    //Query que faz a leitura da tabela Oculos em busca de algum produto com o id inserido
                    MySqlConnection conecta2 = CreateConnection.getFreeSQLConnection(); //Variavel que faz a conexão com o banco
                    conecta2.Open();
                    MySqlCommand query2 = new MySqlCommand("Select * from Oculos where fk_Produto_id=@id", conecta2);
                    query2.Parameters.AddWithValue("@id", id);
                    MySqlDataReader leitor2 = query2.ExecuteReader();


                    if (leitor2.HasRows) //Se possui linhas, significa que é um Óculos, e não um Anel, pois o query retornou algo
                    {
                        if (leitor.Read() && leitor2.Read()) //Se eles lerem
                        {
                            //Constroi o objeto P de produto, com base nas informações retornadas da tabela Produto e da tabela Óculos
                            p = new Produto((int)leitor["id"], leitor["descricao"].ToString(), leitor["nome"].ToString(),
                                leitor2["categoria"].ToString(), Double.Parse(leitor["preco"].ToString()), (int)leitor["quantidade"], leitor.GetBoolean("modelo3d"));

                        }
                        conecta2.Close(); //Fecha a conexão 2
                        return p; //Retorna o produto P construído
                    }
                    else //Se não possui linhas, é um anel
                    {
                        //Query que faz a leitura da tabela Anel
                        MySqlConnection conecta3 = CreateConnection.getFreeSQLConnection(); //Variavel que faz a conexão com o banco
                        conecta3.Open();
                        MySqlCommand query3 = new MySqlCommand("Select * from Anel where fk_Produto_id=@id", conecta3);
                        query3.Parameters.AddWithValue("@id", id);
                        MySqlDataReader leitor3 = query3.ExecuteReader();

                        //Constroi o objeto P de produto, com base nas informações da tabela Produto e Anel
                        if (leitor.Read() && leitor3.Read())
                        {
                            p = new Produto((int)leitor["id"], leitor["descricao"].ToString(), leitor["nome"].ToString(),
                                 Double.Parse(leitor["preco"].ToString()), (int)leitor["quantidade"], int.Parse(leitor3["tamanho"].ToString()), leitor.GetBoolean("modelo3d"));

                        }

                        //Fecha todas as conexões
                        conecta3.Close();
                        conecta2.Close();
                        return p; //Retorna o produto P
                    }

                }
                else //Se o primeiro leitor não ler nada, significa que não há produto
                {
                    return null;
                }


            }
            catch (Exception e) //Caso dê erro na conexão
            {
                return null;
                throw;
            }
            finally //Independente do que ocorra, fecha a conexão
            {
                conecta.Close();


            }
        }


        //Método para pegar todos os valores de tamanho do anel
        public int[] PegarTamanhoAneis(int id)
        {
            try
            {
                int[] tamanhos = new int[20]; //Array com os tamanhos dos aneis
                int contador = 0; //Contator para ser usado no while

                conecta.Open(); //Abre a conexão
                query = new MySqlCommand("SELECT * from Anel where fk_Produto_id=@id", conecta); //Query que pega os aneis conforme o ID de Produto
                query.Parameters.AddWithValue("@id", id);  //Parametros para evitar SQLInjection
                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read()) //Enquanto conseguir ler, ele adiciona o valor do leitor no Array
                {
                    tamanhos[contador] = (int)leitor["tamanho"];
                    contador++;
                }

                return tamanhos;

            }
            catch (Exception e)
            {
                return null;
                throw;
            }
            finally //Independente do que ocorra, fecha a conexão
            {
                conecta.Close();


            }
        }

        //Método que pega todas as informações de todos os óculos

        public List<Produto> PegarTodosOculos(String categoria)
        {
            try
            {
                List<Produto> produtos = new List<Produto>(); //Lista de produtos que será retornada pelo método
                Produto p; //Objeto de produto


                conecta.Open(); //Abrindo conexão

                /*Query que pega todos os valores de produtos, quando o id for correspondente a FK de Óculos, e, filtra pela categoria que foi inserida*/
                query = new MySqlCommand("SELECT * FROM Produto INNER JOIN Oculos ON Produto.id = Oculos.fk_Produto_id WHERE categoria = @categoria", conecta);

                query.Parameters.AddWithValue("@categoria", categoria); //Parameters para evitar SQL INJECTION
                MySqlDataReader leitor = query.ExecuteReader();



                while (leitor.Read()) //Enquanto houverem produtos para serem lidos, cria o objeto P
                {

                    p = new Produto((int)leitor["id"], leitor["descricao"].ToString(), leitor["nome"].ToString(),
                                                leitor["categoria"].ToString(), Double.Parse(leitor["preco"].ToString()), (int)leitor["quantidade"], leitor.GetBoolean("modelo3d"));
                    produtos.Add(p);
                }

                return produtos; //Retorna o objeto P

            }
            catch (Exception e) //Caso dê erro
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
        public List<Produto> PegarTodosAneis()
        {
            try
            {
                List<Produto> produtos = new List<Produto>(); //Lista de produtos que será retornada pelo método
                Produto p; //Objeto de produto

                /*Query que pega todos os valores de produto, e seleciona os valores correspondentes entre as duas tabelas (ou seja, exclui
                 * os valores quando, na tabela de produto, não for um anel), e, seleciona apenas um tamanho, para que seja puxado 1 exemplar
                 de cada produto, pois, para o catálogo, não é necessário todos os tamanhos disponíveis para todos os produtos*/
                conecta.Open();
                query = new MySqlCommand("SELECT * FROM Produto INNER JOIN Anel ON Produto.id = Anel.fk_Produto_id WHERE tamanho = 10", conecta);
                MySqlDataReader leitor = query.ExecuteReader();


                while (leitor.Read())
                {
                    p = new Produto((int)leitor["id"], leitor["descricao"].ToString(), leitor["nome"].ToString(),
                                          Double.Parse(leitor["preco"].ToString()), (int)leitor["quantidade"], (int)leitor["tamanho"], leitor.GetBoolean("modelo3d"));

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
        public List<Produto> PegarTodosProdutos()
        {
            try
            {
                List<Produto> produtos = new List<Produto>(); //Lista de produtos que será retornada pelo método
                Produto p; //Objeto de produto

                conecta.Open(); //Abrindo conexão

                /*Comando que pega todos os valores da tabela de produto, da tabela de oculos, e todos os valores distintos da tabela de anel
                 junta eles, e devolve somente quanto  o id for maior que 0, e o tamanho for 10 ou nulo*/
                query = new MySqlCommand("SELECT DISTINCT Anel.tamanho, Produto.*, Oculos.* FROM Produto LEFT JOIN Oculos ON Oculos.fk_Produto_id = Produto.id " +
                    "LEFT JOIN Anel ON Anel.fk_Produto_id = Produto.id WHERE Produto.id > 0 AND(Anel.tamanho = 10 OR Anel.tamanho IS NULL)", conecta);
                MySqlDataReader leitor = query.ExecuteReader();

                while (leitor.Read()) //Pega todos os valores que vem do query, e adiciona a lista de produtos
                {
                    p = new Produto((int)leitor["id"], leitor["descricao"].ToString(), leitor["nome"].ToString(), Double.Parse(leitor["preco"].ToString()),
                        (int)leitor["quantidade"], leitor["tamanho"] == DBNull.Value ? 0 : (int)leitor["tamanho"], leitor.GetBoolean("modelo3d"),
                        leitor["categoria"] == DBNull.Value ? "" : leitor["categoria"].ToString());


                    produtos.Add(p);
                }

                return produtos;
            }
            catch (Exception) //Caso dê erro
            {
                return null;
                throw;
            }
            finally //Fecha a conexão independente do que ocorra
            {
                conecta.Close();

            }


        }



        /*Método que sorteia id's aleatórios dentro da mesma categoria do produto desejado (se anel, devolve vários ids de aneis, se oculos devolve id's de óculos)
         e retorna a lista de imagens desses produtos, em ordem crescente de acordo com o ID, alem de definir a variavel int[] similaresid com os valores que foram
        sorteados pelo método
         */
        public List<String> ProdutosSimilares(string tabela, int id)
        {
            try
            {

                List<String> similares = new List<String>(); //Lista que salvará as imagens
                List<int> valores = new List<int>(); //Lista usada para descobrir quais os ids que possuem o tipo de produto desejado (anel ou oculos)
                int[] produtos = new int[4]; //Lista que irá possuir os valores sorteados no random
                String imagem = null; //Variavel com a imagem puxada do banco pelo método da classe Imagem
                Random rnd = new Random(); //Variavel que sorteará numeros


                //Query que pega os valores dos ID's dos produtos, quando ele está na tabela de produtos e na tabela desejada (anel ou oculos)
                conecta.Open();

                /*Neste query, tabela não pode ser passado como Parametro, pois se refere ao nome de uma tabela, 
                 * e, como não é possivel SQLInjection, pelo fato de o usuário não ter como mudar esse valor, o 
                 * código segue seguro mesmo com essa declaração explicita*/
                query = new MySqlCommand("SELECT Produto.id from Produto INNER JOIN " + tabela + " on Produto.id = " + tabela + ".fk_Produto_id GROUP BY Produto.id", conecta);
                MySqlDataReader leitor = query.ExecuteReader();


                //Função para pegar os valores dos id's dados pelo query
                while (leitor.Read())
                {
                    valores.Add((int)leitor["id"]);
                }
                leitor.Close();

                valores.Sort(); //Coloca a lista de valores em ordem crescente

                //Função para sortear 4 numeros aleatórios, sem repeti-los, e, sem que sejam iguais ao id inicial que foi enviado através do método
                for (int i = 0; i < 4; i++)
                {
                    produtos[i] = rnd.Next(valores[0], valores[valores.Count - 1]);
                    for (int j = 0; j < 4; j++)
                    {
                        if (produtos[j] == produtos[i] && j != i || produtos[i] == id)
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
                    for (int i = 0; i < produtos.Length; i++)
                    {
                        if (leitor.GetInt32("id") == produtos[i])
                        {
                            Imagem img = new Imagem();
                            imagem = img.PegarImagem(produtos[i]);

                            similares.Add("data:image/jpeg;base64," + imagem);

                        }
                    }
                }


                similaresid = produtos; //Define os id's dos similares com os valores obtidos randomicamente

                return similares; //return

            }
            catch (Exception) //Caso dê erro
            {

                throw;
            }
            finally //Fecha a conexão independente
            {
                conecta.Close();
            }


        }



    }
}
