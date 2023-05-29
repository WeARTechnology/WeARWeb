using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WeAR.Models;

namespace WeAR.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class WebServiceController : Controller
    {

        [HttpGet]
        [Route("/api/[controller]/produtos")]
        //Método que pega todos os produtos existentes
        public String pegarTudoProdutos()
        {
            //Cria-se objetos da classe, e uma lista de objetos
            Produto p = new Produto();
            List<Produto> productList;

            //Pega os valores da lista através do método no Model Produto.cs
            productList = p.PegarTodosProdutos();

            //Cria-se o objeto de Imagem, para chamar os métodos posteriormente
            Imagem img = new Imagem();


            //Cria uma lista de produto com imagem
            List<ProdutoComImagem> combinedList = new List<ProdutoComImagem>();

            if (productList.Count > 1)
            {
                //Para cada objeto existente na lista de produto, cria um novo objeto da lista combinada
                for (int i = 0; i < productList.Count; i++)
                {
                    combinedList.Add(new ProdutoComImagem
                    {
                        id = productList[i].id,
                        nome = productList[i].nome,
                        preco = productList[i].preco,
                        desc = productList[i].desc,
                        tipo = productList[i].tipo,
                        qntd = productList[i].qntd,
                        tamanho = productList[i].tamanho,
                        modelo3d = productList[i].modelo3d,
                        imagem = img.PegarImagem(productList[i].id) //Pega a imagem através do método usando o id do produto atual
                    });
                }
            }

            //Se houver algo na lista, a retorna, caso contrário, retorna uma lista vazia
            if (combinedList.Count > 0)
            {
                return JsonConvert.SerializeObject(combinedList);

            }
            else
            {
                return "";
            }


        }

        [HttpGet]
        [Route("/api/[controller]/aneisTamanho")]
        public int[] pegarAneisTamanho(int id)
        {
            //Cria-se objetos da classe, e uma array de inteiros
            Produto p = new Produto();
            int[] tamanhos;

            //Pega os valores da array através do método no Model Produto.cs
            tamanhos = p.PegarTamanhoAneis(id);

            if (tamanhos.Length > 0)
            {
                return tamanhos;

            }
            else
            {
                int[] semTamanho = new int[1];
                semTamanho.SetValue(0, 1);
                return semTamanho;
            }



        }

        [HttpGet]
        [Route("/api/[controller]/oculos")]
        public String pegarOculos(string categoria)
        {
            //Cria-se objetos da classe, e uma lista de objetos
            Produto p = new Produto();
            List<Produto> productList;

            //Pega os valores da lista através do método no Model Produto.cs
            productList = p.PegarTodosOculos(categoria);

            //Cria-se o objeto de Imagem, para chamar os métodos posteriormente
            Imagem img = new Imagem();


            //Cria uma lista de produto com imagem
            List<ProdutoComImagem> combinedList = new List<ProdutoComImagem>();

            if (productList.Count > 1)
            {
                //Para cada objeto existente na lista de produto, cria um novo objeto da lista combinada
                for (int i = 0; i < productList.Count; i++)
                {
                    combinedList.Add(new ProdutoComImagem
                    {
                        id = productList[i].id,
                        nome = productList[i].nome,
                        preco = productList[i].preco,
                        desc = productList[i].desc,
                        tipo = productList[i].tipo,
                        qntd = productList[i].qntd,
                        tamanho = productList[i].tamanho,
                        modelo3d = productList[i].modelo3d,
                        imagem = img.PegarImagem(productList[i].id) //Pega a imagem através do método usando o id do produto atual
                    });
                }
            }

            //Se houver algo na lista, a retorna, caso contrário, retorna uma lista vazia
            if (combinedList.Count > 0)
            {
                return JsonConvert.SerializeObject(combinedList);

            }
            else
            {
                return "";
            }


        }

        [HttpGet]
        [Route("/api/[controller]/produto")]
        public String pegarProduto(int id)
        {
            //Cria-se objetos da classe
            Produto p = new Produto();

            //Pega o produto desejado através do seu ID
            p = p.BuscaProduto(id);

            //Cria-se o objeto de Imagem, e uma String para salvar os valores das imagens
            Imagem img = new Imagem();
            string imgResult;

            imgResult = img.PegarImagem(id);


            //Cria uma lista de produto com imagem
            ProdutoComImagem combinedProd = new ProdutoComImagem();

            //Para cada objeto existente na lista de produto, cria um novo objeto da lista combinada

            combinedProd = new ProdutoComImagem
            {
                id = p.id,
                nome = p.nome,
                preco = p.preco,
                desc = p.desc,
                tipo = p.tipo,
                qntd = p.qntd,
                tamanho = p.tamanho,
                modelo3d = p.modelo3d,
                imagem = imgResult
            };


            //Se houver algo na lista, a retorna, caso contrário, retorna uma lista vazia
            if (combinedProd.id != 1)
            {
                return JsonConvert.SerializeObject(combinedProd);

            }
            else
            {
                return "";
            }
        }


        [HttpGet]
        [Route("/api/[controller]/imagem")]
        public String pegarImagem(int id)
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
        [Route("/api/[controller]/imagemNoStock")]
        public String pegarImagemNoStock(int id)
        {
            //Cria-se objetos da classe imagem, e a string que pega o resultado
            Imagem img = new Imagem();
            string resultado;

            //Pega a imagem através do ID
            resultado = img.PegarImagemNoStock(id);

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
        [Route("/api/[controller]/imagemRound")]
        public String pegarImagemRound(int id)
        {
            //Cria-se objetos da classe imagem, e a string que pega o resultado
            Imagem img = new Imagem();
            string resultado;

            //Pega a imagem através do ID
            resultado = img.PegarImagemRound(id);

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
        [Route("/api/[controller]/similaresID")]
        public List<ProdutoComImagem> pegarImagem(int id, string tabela)
        {
            Produto p = new Produto(); //Cria um objeto de produto
            List<ProdutoComImagem> produtoComImagem = new List<ProdutoComImagem>(); //Cria uma lista de Produto com imagem
            List<string> imagens = new List<string>(); //Cria uma lista de string para armazenar os valores

            imagens = p.ProdutosSimilares(tabela, id); //Faz a busca no banco

            int[] ids = p.similaresid; //Pega os ids dos produtos similares            

            //Para cada imagem, troca o valor inicial atribuido pelo método, que é usado no cshtml, por vazio, pois no Android ele não tem uso
            for (int i = 0; i < imagens.Count; i++)
            {
                imagens[i] = imagens[i].Replace("data:image/jpeg;base64,", "");
            }


            for (int i = 0; i < ids.Length; i++)
            {
                produtoComImagem.Add(new ProdutoComImagem(imagens[i], ids[i]));
            }

            return produtoComImagem;//Retorna o que é buscado do banco
        }


    }
}
