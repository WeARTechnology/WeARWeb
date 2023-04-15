using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public IActionResult Privacy()
        {
            return View();
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
        public IActionResult indexFiltro()
        {
            return View();
        }
        public IActionResult contato()
        {
            return View();
        }
        public IActionResult EnviarImagens()
        {
            return View();
        }
        public IActionResult objeto3D()
        {
            return View();
        }
        public IActionResult produto(int id)
        {
            Imagem img = new Imagem();
            Produto p = new Produto();

            TempData["ImagemProduto"] = "data:image/jpeg;base64," + img.PegarImagem(id);
            p = p.buscaProduto(id);
            ViewBag.modelo3D = p.valorModelo3d(id);

            return View(p);
        }

        public IActionResult ListarImagens()
        {
            Imagem banco = new Imagem();
            List<String> imagens = banco.PegarTudo();

            return View(imagens);
        }


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
            if (categoria == "Anel") {
                produtos = p.pegarTodosAneis(); //Chama o método da classe produto, para pegar os aneis
                return View(produtos);

            }
            else if(categoria == "Oculos Sol")
            {
                produtos = p.pegarTodosOculos("Sol");//Chama o método da classe produto, para pegar os óculos e passa a categoria selecionada
                return View(produtos);

            }else if(categoria == "Oculos Grau")
            {
                produtos = p.pegarTodosOculos("Grau");//Chama o método da classe produto, para pegar os óculos e passa a categoria selecionada
                return View(produtos);

            }
            else
            {
                produtos = p.pegarTodosProdutos();
                return View(produtos);
            }
        }

       

      
    }
}
