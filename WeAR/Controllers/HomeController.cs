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

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult indexFiltro()
        {
            return View();
        }
        public IActionResult Listar()
        {
            Login banco = new Login();
            List<String> imagens = banco.PegarImagem();

            return View(imagens);
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

        public String EnviarImagem(IFormCollection form)
        {

            IFormFile arq = form.Files.First();
            int id = int.Parse(form["id"]);

            Login cadastro = new Login();

            //Cadastra o cliente
            string resultado = cadastro.addImagem(id, arq);
            ViewBag.Alert = resultado;
            return resultado;

           
        }

       

      
    }
}
