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
        public IActionResult catalogo()
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
        public IActionResult produto()
        {
            return View();
        }

        public IActionResult ListarImagens()
        {
            Imagem banco = new Imagem();
            List<String> imagens = banco.PegarTudoImagens();

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

       

      
    }
}
