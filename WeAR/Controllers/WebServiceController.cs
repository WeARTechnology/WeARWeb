using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Collections.Generic;
using WeAR.Models;

namespace WeAR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebServiceController : Controller
    {

        [HttpGet]
        [Route("api/[controller]/produtos")]
        //Método que pega todos os produtos existentes
        public List<Produto> pegarProdutos()
        {
            //Cria-se objetos da classe, e uma lista de objetos
            Produto p = new Produto();
            List<Produto> finalList;

            //Pega os valores da lista através do método no Model Produto.cs
            finalList = p.PegarTodosProdutos();

            //Se houver algo na lista, a retorna, caso contrário, retorna uma lista vazia
            if(finalList.Count > 0)
            {
                return finalList;

            }
            else
            {
                return new List<Produto>();
            }


        }

        [HttpGet]
        [Route("api/[controller]/aneis")]
        public List<Produto> pegarAneis()
        {
            //Cria-se objetos da classe, e uma lista de objetos
            Produto p = new Produto();
            List<Produto> finalList;

            //Pega os valores da lista através do método no Model Produto.cs
            finalList = p.PegarTodosAneis();

            //Se houver algo na lista, a retorna, caso contrário, retorna uma lista vazia
            if (finalList.Count > 0)
            {
                return finalList;

            }
            else
            {
                return new List<Produto>();
            }


        }

        [HttpGet]
        [Route("api/[controller]/oculos")]
        public List<Produto> pegarOculos(string categoria)
        {
            //Cria-se objetos da classe, e uma lista de objetos
            Produto p = new Produto();
            List<Produto> finalList;

            //Pega os valores da lista através do método no Model Produto.cs
            finalList = p.PegarTodosOculos(categoria);

            //Se houver algo na lista, a retorna, caso contrário, retorna uma lista vazia
            if (finalList.Count > 0)
            {
                return finalList;

            }
            else
            {
                return new List<Produto>();
            }
        }

        [HttpGet]
        [Route("api/[controller]/produto")]
        public Produto pegarProduto(int id)
        {
            //Cria-se objetos da classe
            Produto p = new Produto();

            //Pega o produto desejado através do seu ID
            p = p.BuscaProduto(id);

            //Se houver algum valor (neste caso, o nome), ele envia o produto, caso contrário, envia um produto vazio
            if (p.nome != null)
            {
                return p;

            }
            else
            {
                return new Produto();
            }
        }


        [HttpGet]
        [Route("api/[controller]/imagens")]
        public List<string> pegarImagens()
        {
            //Cria-se o objeto de Imagem, e uma lista de String para salvar os valores das imagens
            Imagem img = new Imagem();
            List<string> resultado;

            //Pega=se todas as imagens do banco de dados através do método
            resultado = img.PegarTudo();

            //Se a lista de resultado tiver valores, a retorna, caso contrário, retorna um lista vazia
            if(resultado.Count > 0)
            {
                return resultado;
            }
            else
            {
                return new List<string>();
            }

        }

        [HttpGet]
        [Route("api/[controller]/imagem")]
        public string pegarImagem(int id)
        {
            //Cria-se objetos da classe imagem, e a string que pega o resultado
            Imagem img = new Imagem();
            string resultado;

            //Pega a imagem através do ID
            resultado = img.PegarImagem(id);

            //Se o resultado for null, retorna erro, caso contrário, retorna o resultado
            if (resultado == null)
            {
                return "Error";

            }
            else
            {
                return resultado;
            }
        }

        [HttpGet]
        [Route("api/[controller]/similares")]
        public List<Produto> Similares() { 
        }


    }
}
