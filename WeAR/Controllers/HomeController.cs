using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using WeAR.Models;

namespace WeAR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Error()
        {
            return View("pagErro");
        }

        //Redirecionamento ao Index, que sorteia os produtos recomendados automaticamente
        public IActionResult Index()
        {
            //Verifica se os valores da sessão não existem
            if (HttpContext.Session.Get("IndexRandom") == null && HttpContext.Session.Get("IndexRandomIds") == null)
            {
                //Cria um objeto de produto, e uma lista de produtos
                Produto p = new Produto();
                List<Produto> lista = new List<Produto>();

                //Pega todos os produtos existentes
                lista = p.PegarTodosProdutos();

                //Sorteia um numero aleatório com base na quantidade de produtos
                Random rnd = new Random();
                int value = rnd.Next(lista.Count() - 1);

                //Busca o produto com o id sorteado
                p = p.BuscaProduto(value);

                string tabela;
                if (p != null) {
                    //Define a tabela que deverá fazer a buscada do produto
                    tabela = p.tamanho == 0 ? "Anel" : "Oculos";
                }
                else
                {
                    p = new Produto();
                    tabela = "Anel";
                }
                
                //Pega as imagens através do método que sorteia similares
                List<String> imagens = p.ProdutosSimilares(tabela, value);
                //Pega os ids dos similares
                int[] similares = p.similaresid;

                //Envia os ids por ViewBag
                ViewBag.ids = similares;

                //Salva os valores na sessão
                HttpContext.Session.SetString("IndexRandom", JsonConvert.SerializeObject(imagens));
                HttpContext.Session.SetString("IndexRandomIds", JsonConvert.SerializeObject(similares));

                return View(imagens); //Retorna a view e o modelo
            }
            else
            {
                //Deserializa e lê os valores das sessões
                List<String> imagens = JsonConvert.DeserializeObject<List<String>>(HttpContext.Session.GetString("IndexRandom"));
                int[] similares = JsonConvert.DeserializeObject<int[]>(HttpContext.Session.GetString("IndexRandomIds"));
                ViewBag.ids = similares;

                return View(imagens); //Retorna a view e o modelo
            }

            
        }



        /*                  Métodos de Redirecionamento Simples                 */

        public IActionResult Faq()
        {
            return View();
        }

        public IActionResult Anuncio()
        {
            return View();
        }
        public IActionResult TryON(int prod)
        {
            switch (prod)
            {
                case 5:
                    ViewBag.Model3D = "/3dModels/Cipreste.glb";
                    @ViewBag.distancia = 300;
                    break;
                case 7:
                    ViewBag.Model3D = "/3dModels/DeAardappeleters.glb";
                    @ViewBag.distancia = 1150;
                    break;
                case 6:
                    ViewBag.Model3D = "/3dModels/MaratusConstelatus.glb";
                    @ViewBag.distancia = 150;
                    break;
                default:
                    break;
            }
            return View();
        }
        public IActionResult Contato()
        {
            return View();
        }

        public IActionResult pagSucessoForm() {
            
                return View();  
            }

        //[Authorize]
        public IActionResult EnviarImagens()
        {
            return View();

        }
        public IActionResult objeto3D()
        {
            return View();
        }      


        /*                  Outros Métodos                  */
        //Método que cria a página de produto com base no ID
        public IActionResult Produto(int id)
        {
            //Cria objetos de Produto e Imagem
            Imagem img = new Imagem();
            Produto p = new Produto();

            //Busca todos os valores do produto com base no seu ID
            p = p.BuscaProduto(id);
            ViewBag.anel = p.tamanho != 0 ? true : false; //Viewbag que passa o valor de anel caso true se houver tamanho

            //Chama o método que diminui a quantidade de P caso ele já exista
            DecreaseQuantity(p);

            //Ao se abrir a página de produtos, cria-se um objeto na Session que passa a quantidade que existe daquele produto
            HttpContext.Session.SetInt32("Produto_" + id, p.qntd);

            //Se a quantidade for maior que 0, busca-se a imagem do produto normalmente, se for igual, busca-se a imagem do produto sem estoque
            if (p.qntd > 0)
            {
                TempData["ImagemProduto"] = "data:image/jpeg;base64," + img.PegarImagem(id);
            }
            else
            {
                TempData["ImagemProduto"] = "data:image/jpeg;base64," + img.PegarImagemNoStock(id);

            }

            //Retorna a view de nome Produto, junto do modelo P 
            return View(p);
        }


        //Método usado para carregar todos os valores no carrinho,
        
            public IActionResult Catalogo(string categoria, string? busca = null)
            {
                //Lista com todos os produtos
                List<Produto> produtos = new List<Produto>();
                Produto p = new Produto(); //Objeto de produto


                //If que checka qual botão redirecionou para essa função, ou seja, se foi anel, óculos de sol ou de grau, se não foi nenhum , retorna todos os produtos
                if (categoria == "Anel")
                {
                    produtos = p.PegarTodosAneis(); //Chama o método da classe produto, para pegar os aneis

                    //Para cada produto existente, checka se ele já foi comprado anteriormente, se sim, diminui sua quantidade
                    foreach (Produto prod in produtos)
                    {
                        DecreaseQuantity(prod);
                    }

                    //Retona a View de nome Catálogo, junto com o modelo de produtos
                    return View(produtos);


                }
                else if (categoria == "Oculos Sol")
                {
                    produtos = p.PegarTodosOculos("Sol");//Chama o método da classe produto, para pegar os óculos e passa a categoria selecionada

                    //Para cada produto existente, checka se ele já foi comprado anteriormente, se sim, diminui sua quantidade
                    foreach (Produto prod in produtos)
                    {
                        DecreaseQuantity(prod);
                    }

                    //Retona a View de nome Catálogo, junto com o modelo de produtos
                    return View(produtos);

                }
                else if (categoria == "Oculos Grau")
                {
                    produtos = p.PegarTodosOculos("Grau");//Chama o método da classe produto, para pegar os óculos e passa a categoria selecionada

                    //Para cada produto existente, checka se ele já foi comprado anteriormente, se sim, diminui sua quantidade
                
                    foreach (Produto prod in produtos)
                    {                        
                        DecreaseQuantity(prod);
                    }

                    //Retona a View de nome Catálogo, junto com o modelo de produtos
                    return View(produtos);

                }
                else
                {
                    if (busca != null)
                    {
                        produtos = p.PegarTodosProdutos(); //Chama o método da classe produto, para pegar todos os produtos

                        //Para cada produto existente, checka se ele já foi comprado anteriormente, se sim, diminui sua quantidade
                        for (int i = 0; i < produtos.Count; i++)

                        {
                            if (RemoveAccents(produtos[i].nome).ToLower().Contains(RemoveAccents(busca).ToLower())) //Como busca não é null, se o nome do produto tiver algo que tem na pesquisa, retorna normalmente
                                                            //Caso contrario, remove este produto
                            {
                                DecreaseQuantity(produtos[i]);
                            }
                            else
                            {
                                produtos.Remove(produtos[i]);
                                i--;
                            }
                        }

                        //Retona a View de nome Catálogo, junto com o modelo de produtos
                        return View(produtos);

                    }
                    else
                    {
                        produtos = p.PegarTodosProdutos(); //Chama o método da classe produto, para pegar todos os produtos

                        //Para cada produto existente, checka se ele já foi comprado anteriormente, se sim, diminui sua quantidade
                        foreach (Produto prod in produtos)
                        {
                            DecreaseQuantity(prod);
                        }

                        //Retona a View de nome Catálogo, junto com o modelo de produtos
                        return View(produtos);
                    }
                }


            

        
            }

   


        /*              Métodos que modificam o Banco, usa-se [Authorize] para não permitir que sejam acessados                 */

        [Authorize]
        //Método que lista todas as imagens contidas no banco
        public IActionResult ListarImagens()
        {
            //Cria-se um objeto de imagem, e busca-se tudo do banco através do método
            Imagem banco = new Imagem();
            List<String> imagens = banco.PegarTudo();

            //Retorna a view de nome ListarImagens e o modelo das imagens
            return View(imagens);
        }

        [Authorize]
        //Método que Envia as imagens para o banco
        public String EnviarImagem(IFormCollection form)
        {
            //Lê o arquivo que foi inserido no formulário
            IFormFile arq = form.Files.First();
            //Pega o ID do produto que será adicionado ao banco
            int id = int.Parse(form["id"]);
            //Cria um objeto de imagem
            Imagem cadastro = new Imagem();

            //Adiciona a imagem, e pega o resultado do método em uma String
            string resultado = cadastro.addImagem(id, arq);
            //Cria uma ViewBag que possui o valor do resultado para usar na View 
            ViewBag.Alert = resultado;

            //Retorna essa String
            return resultado;

        }

        /*Método auxiliar que diminui a quantidade do produto de acordo com o banco*/
        public void DecreaseQuantity(Produto p)
        {
            //Se já houver produtos checkados na Session
            if (HttpContext.Session.Get("CheckedOutProducts") != null)
            {
                //Se deserializa esses proodutos que já foram comprados
                List<Produto> produtos = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CheckedOutProducts"));

                //Se, um dos produtos que ja foi comprado, tiver o ID igual ao ID passado pelo método
                if (produtos.FirstOrDefault(prod => prod.id == p.id) != null)
                {
                    //Define-se que a quantidade de produtos, é a sua quantidade da classe (buscado do banco) - a quantidade que já foi comprada
                    Produto p2 = produtos.FirstOrDefault(prod => prod.id == p.id);
                    p.qntd -= p2.qntd;
                }

            }
        }


        //Método que remove os acentos das palavras, para checkar elas na barra de pesquisa
        private string RemoveAccents(string nome)
        {
            var normalizedString = nome.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

    }
}
