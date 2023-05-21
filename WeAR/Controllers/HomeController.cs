using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Faq()
        {
            return View();
        }
        public IActionResult Carrinho()
        {

            if (HttpContext.Session.Get("CartItens") == null)
            {
                List<Produto> lista = new List<Produto>();
                return View(lista);
            }
            else
            {
                List<Produto> products = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CartItens"));
                return View(products);
            }
        }

        public IActionResult Anuncio()
        {
            return View();
        }
        public IActionResult indexFiltro()
        {
            return View();
        }
        public IActionResult contato()
        {
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

        [HttpPost]
        public IActionResult Carrinho(int qntd, int id)
        {
            Produto p = new Produto();
            p = p.buscaProduto(id);
            p.id = id;


            if (HttpContext.Session.Get("CheckedOutProducts") != null)
            {
                List<Produto> produtos = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CheckedOutProducts"));
                if (produtos.FirstOrDefault(p => p.id == id) != null)
                {
                    Produto p2 = produtos.FirstOrDefault(p => p.id == id);
                    p.qntd -= p2.qntd;
                }

                HttpContext.Session.SetInt32("Produto_" + id, p.qntd);

            }
            else
            {
                HttpContext.Session.SetInt32("Produto_" + id, p.qntd);

            }


            p.qntd = qntd;


            if (HttpContext.Session.Get("CartItens") == null)
            {
                List<Produto> products = new List<Produto>();
                products.Add(p);
                HttpContext.Session.SetString("CartItens", JsonConvert.SerializeObject(products));
                return RedirectToAction("Carrinho");
            }
            else
            {
                List<Produto> products = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CartItens"));
                var existingProduct = products.FirstOrDefault(prod => prod.id == id);

                if (existingProduct != null)
                {
                    existingProduct.qntd += qntd;
                }
                else
                {
                    products.Add(p);
                }

                HttpContext.Session.SetString("CartItens", JsonConvert.SerializeObject(products));
                return RedirectToAction("Carrinho");
            }
        }


        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            if (HttpContext.Session.Get("CartItens") != null)
            {
                List<Produto> products = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CartItens"));
                var product = products.FirstOrDefault(p => p.id == id);
                if (product != null && HttpContext.Session.GetInt32("Produto_" + id).Value >= quantity)
                {
                    product.qntd = quantity;
                    HttpContext.Session.SetString("CartItens", JsonConvert.SerializeObject(products));
                }
                else
                {
                    return Json(new { success = false, message = "Valor maior do que disponível em estoque" });
                }
            }

            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            if (HttpContext.Session.Get("CartItens") != null)
            {
                List<Produto> products = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CartItens"));
                var product = products.FirstOrDefault(p => p.id == id);
                if (product != null)
                {
                    products.Remove(product);
                    HttpContext.Session.SetString("CartItens", JsonConvert.SerializeObject(products));
                }
            }

            return RedirectToAction("Carrinho");
        }


        [HttpPost]
        public IActionResult Checkout(List<int> ids, List<int> qntds)
        {

            if (HttpContext.Session.Get("CartItens") != null)
            {
                List<Produto> products = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CartItens"));

                // Remove products from the cart
                products.RemoveAll(p => ids.Contains(p.id));

                HttpContext.Session.SetString("CartItens", JsonConvert.SerializeObject(products));

                // Store the information in the session
                List<Produto> checkedOutProducts = new List<Produto>();
                for (int i = 0; i < ids.Count; i++)
                {
                    checkedOutProducts.Add(new Produto { id = ids[i], qntd = qntds[i] });
                }

                if (HttpContext.Session.Get("CheckedOutProducts") == null)
                {
                    HttpContext.Session.SetString("CheckedOutProducts", JsonConvert.SerializeObject(checkedOutProducts));
                }
                else
                {
                    List<Produto> existingCheckedProducts = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CheckedOutProducts"));
                    foreach (Produto atual in checkedOutProducts)
                    {
                        Produto existingProduct = existingCheckedProducts.FirstOrDefault(p => p.id == atual.id);
                        if (existingProduct != null)
                        {
                            existingProduct.qntd += atual.qntd;
                        }
                        else
                        {
                            existingCheckedProducts.Add(atual);
                        }
                    }

                    HttpContext.Session.SetString("CheckedOutProducts", JsonConvert.SerializeObject(existingCheckedProducts));

                }

            }

            return Json(new { success = true });
        }

        public IActionResult produto(int id)
        {
            Imagem img = new Imagem();
            Produto p = new Produto();

            p = p.buscaProduto(id);
            p.id = id;
            ViewBag.modelo3D = p.valorModelo3d(id);



            if (HttpContext.Session.Get("CheckedOutProducts") != null)
            {
                List<Produto> produtos = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CheckedOutProducts"));
                if (produtos.FirstOrDefault(p => p.id == id) != null)
                {
                    Produto p2 = produtos.FirstOrDefault(p => p.id == id);
                    p.qntd -= p2.qntd;
                }

            }

            HttpContext.Session.SetInt32("Produto_" + id, p.qntd);

            if (p.qntd > 0)
            {
                TempData["ImagemProduto"] = "data:image/jpeg;base64," + img.PegarImagem(id);
            }
            else
            {
                TempData["ImagemProduto"] = "data:image/jpeg;base64," + img.PegarImagemNoStock(id);

            }


            return View(p);
        }

        //[Authorize]
        public IActionResult ListarImagens()
        {
            Imagem banco = new Imagem();
            List<String> imagens = banco.PegarTudo();

            return View(imagens);
        }

        //[Authorize]
        public String EnviarImagem(IFormCollection form)
        {

            IFormFile arq = form.Files.First();
            int id = int.Parse(form["id"]);
            Imagem cadastro = new Imagem();

            //Cadastra o cliente
            string resultado = cadastro.addImagem(id, arq);
            ViewBag.Alert = resultado;
            return resultado;


        }

        public IActionResult catalogo(string categoria)
        {
            //Lista com todos os produtos
            List<Produto> produtos = new List<Produto>();
            Produto p = new Produto(); //Objeto de produto


            //If que checka qual botão redirecionou para essa função, ou seja, se foi anel, óculos de sol ou de grau
            if (categoria == "Anel")
            {
                produtos = p.pegarTodosAneis(); //Chama o método da classe produto, para pegar os aneis
                if (HttpContext.Session.Get("CheckedOutProducts") != null)
                {
                    List<Produto> checkedProdutos = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CheckedOutProducts"));
                    if (checkedProdutos.FirstOrDefault(prod => prod.id == p.id) != null)
                    {
                        Produto p2 = checkedProdutos.FirstOrDefault(prod => prod.id == p.id);
                        p.qntd -= p2.qntd;
                    }

                }

                return View(produtos);


            }
            else if (categoria == "Oculos Sol")
            {
                produtos = p.pegarTodosOculos("Sol");//Chama o método da classe produto, para pegar os óculos e passa a categoria selecionada
                if (HttpContext.Session.Get("CheckedOutProducts") != null)
                {
                    List<Produto> checkedProdutos = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CheckedOutProducts"));
                    if (checkedProdutos.FirstOrDefault(prod => prod.id == p.id) != null)
                    {
                        Produto p2 = checkedProdutos.FirstOrDefault(prod => prod.id == p.id);
                        p.qntd -= p2.qntd;
                    }

                }
                return View(produtos);

            }
            else if (categoria == "Oculos Grau")
            {
                produtos = p.pegarTodosOculos("Grau");//Chama o método da classe produto, para pegar os óculos e passa a categoria selecionada
                if (HttpContext.Session.Get("CheckedOutProducts") != null)
                {
                    List<Produto> checkedProdutos = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CheckedOutProducts"));
                    if (checkedProdutos.FirstOrDefault(prod => prod.id == p.id) != null)
                    {
                        Produto p2 = checkedProdutos.FirstOrDefault(prod => prod.id == p.id);
                        p.qntd -= p2.qntd;
                    }

                }
                return View(produtos);

            }
            else
            {
                produtos = p.pegarTodosProdutos();
                if (HttpContext.Session.Get("CheckedOutProducts") != null)
                {
                    List<Produto> checkedProdutos = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CheckedOutProducts"));
                    if (checkedProdutos.FirstOrDefault(prod => prod.id == p.id) != null)
                    {
                        Produto p2 = checkedProdutos.FirstOrDefault(prod => prod.id == p.id);
                        p.qntd -= p2.qntd;
                    }

                }
                return View(produtos);
            }

        }




    }
}
