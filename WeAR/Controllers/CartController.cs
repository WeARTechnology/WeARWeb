using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using WeAR.Models;

namespace WeAR.Controllers
{
    public class CartController : Controller
    {


        /*No método do carrinho, independente se foi acessado através da inserção de um Produto (Post) ou não, é necessário ler o valor dentro da Session
         para conseguir obter quais objetos já estão no carrinho*/
        public IActionResult Carrinho()
        {

            if (HttpContext.Session.Get("CartItens") == null) //Se ainda nada foi adicionado ao carrinho, ou seja, o valor na sessão não existe
            {
                List<Produto> lista = new List<Produto>(); //Cria uma lista vazia para retornar, apeans para não dar NullException no Model do carrinho
                return View(lista);
            }
            else
            {
                /*Caso contrário, lê a session que já existe, e retorna o seu valor, com os produtos que já estão no carrinho*/
                List<Produto> products = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CartItens"));
                return View(products);
            }
        }



        /*                  Métodos por Post                    */
        [HttpPost]
        /*Método que é acionado ao se adicionar um produto ao carrinho na página de Produto.cshtml*/
        public IActionResult Carrinho(int qntd, int id, int? tamanho = null)
        {
            //Cria um objeto da classe Produto, e busca todos os dados no banco com base no ID
            Produto p = new Produto();
            p = p.BuscaProduto(id);
            p.id = id;

            if (tamanho.HasValue)
            {
                p.tamanho = tamanho.Value;
            }

            //Se já houverem produtos comprados, que é verdadeiro caso já exista algum valor na Session de CheckedOutProducts
            if (HttpContext.Session.Get("CheckedOutProducts") != null)
            {
                //Deserializa o valor contido na Sessão, para uma lista de produtos
                List<Produto> produtos = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CheckedOutProducts"));

                //Se, houver algum valor onde, o id de produtos == id da classe, ele me retornará qual é esse objeto de produtos, caso contrário, retorna o seu Default(Null)
                if (produtos.FirstOrDefault(p => p.id == id) != null)
                {
                    Produto p2 = produtos.FirstOrDefault(p => p.id == id);
                    p.qntd -= p2.qntd; //Define-se que a quantidade existente do produto buscado pelo ID, é sua quantidade original - quanto ja foi comprado anteriormente
                }

                //Adiciona-se para uma sessão de valor "Produto_ID" a quantidade deste produto, o ID varia de acordo com os dados inseridos no método
                HttpContext.Session.SetInt32("Produto_" + id, p.qntd);

            }
            else
            {
                //Se não houver nenhum produto comprado, a quantidade total daquele produto, é a quantidade contida na classe/banco
                HttpContext.Session.SetInt32("Produto_" + id, p.qntd);

            }


            /*Define que a quantidade de P, é a quantidade que foi comprada, para que possa se passar um objeto para a classe de Carrinho, onde a quantidade que será
             mostrada, será a quantidade que foi escolhida na página de Produto*/

            p.qntd = qntd;

            /*Se ainda não houver nenhum produto na Session de CartItens, significa que o carrinho está vazio*/
            if (HttpContext.Session.Get("CartItens") == null)
            {
                /*Cria uma nova lista de Produto, e adiciona a ela o valor do produto P que foi buscado no Banco*/
                List<Produto> products = new List<Produto>();
                products.Add(p);

                /*Salva na Session esta lista onde foi adicionado o produto P*/
                HttpContext.Session.SetString("CartItens", JsonConvert.SerializeObject(products));

                /*Aqui, há um RedirectToAction e não um Return View, para evitar que, uma vez na página de carrinho após um redirecionamento, ao dar F5 o método
                 seja chamado novamente ,adicionadno mais produtos ao carrinho, fazendo assim, é redirecionado ao método que não possui POST, então, apenas se é 
                lido o carrinho*/
                return RedirectToAction("Carrinho");
            }
            else //Caso já exista
            {
                /*Cria uma lista de Produtos deserializando o que já estava na Session*/
                List<Produto> products = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CartItens"));

                /*Cria uma variavel que, possui o valor de um Produto, caso, algum produto que já exista na Session, for igual ao produto que está tentando se inserir,
                 desta forma, quando se tentar adicionar o mesmo produto 2 vezes, ele não ficará duplicado, apenas serão somadas as quantidades*/
                var existingProduct = products.FirstOrDefault(prod => prod.id == id);

                if (existingProduct != null)
                //Se ele não for null, a quantidade = ela mesma + quantidade inserida
                {
                    //Se o tamanho for igual ao tamanho ja existente, se soma normalmente
                    if (tamanho.HasValue && existingProduct.tamanho == tamanho.Value)
                    {
                        existingProduct.qntd += qntd;
                    }
                    else if (tamanho.HasValue) //Caso contrário, se adiciona um novo produto p com o tamanho passado no método
                    {
                        p.tamanho = tamanho.Value;
                        products.Add(p);
                    }
                    else
                    { //Se não possuir tamanho, apenas se soma
                        existingProduct.qntd += qntd;

                    }

                }
                else //Caso contrário, apenas se adiciona o objeto P a lista
                {
                    products.Add(p);
                }

                //Se serializa novamente a lista de Produtos para a Sessão em CartItens
                HttpContext.Session.SetString("CartItens", JsonConvert.SerializeObject(products));
                return RedirectToAction("Carrinho");
            }
        }



        [HttpPost]
        /*Método que atualiza a quantidade de produtos contidos na Sessão, quando o usuário atualiza a quantidade no Carrinho */
        public IActionResult UpdateQuantity(int id, int quantity, int tamanho)
        {
            //O método só é acessado se realmente houverem produtos no Carrinho
            if (HttpContext.Session.Get("CartItens") != null)
            {
                //Cria uma lista de produtos com base nos valores contidos na Sessão de Chave CartItens
                List<Produto> products = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CartItens"));

                //Cria uma variavel que aponta para a refêrencia contida na lista, caso o id do produto na lista for igual ao id inserido pelo usuário
                //Esta retorna todos os produtos com id igual
                var product = products.FindAll(p => p.id == id);

                /*Caso, product não seja null, ou seja, foi encontrado algum produto, e o valor contido na Sessão "Produto_ID", que possui o valor
                 da quantidade de produtos existêntes para aquele ID específico, for maior do que a quantidade que o usuário deseja*/
                if (product != null && HttpContext.Session.GetInt32("Produto_" + id).Value >= quantity)
                {
                    //Para cada produto encontrado na lista products
                    foreach (Produto subp in product)
                    {

                        //Se houver um tamanho, e ele for igual ao inserido, remove-se
                        if (tamanho != 0 && subp.tamanho == tamanho)
                        {
                            //Define que a nova quantidade é a quantidade que o usuário deseja, e, serializa o objeto novmaente para a Session
                            subp.qntd = quantity;
                            HttpContext.Session.SetString("CartItens", JsonConvert.SerializeObject(products));
                        }
                        else if (subp.tamanho == tamanho) //Se não houver um tamanho, remove-se da mesma forma
                        {
                            //Define que a nova quantidade é a quantidade que o usuário deseja, e, serializa o objeto novmaente para a Session
                            subp.qntd = quantity;
                            HttpContext.Session.SetString("CartItens", JsonConvert.SerializeObject(products));
                        }

                    }
                }
                else //Caso contrário significa que a quantidade desejada é maior que a disponível, e retorna um JSON com sucess=false e uma mensagem de erro
                {
                    return Json(new { success = false, message = "Valor maior do que disponível em estoque" });
                }
            }

            //Caso tudo dê certo, retorna um JSON com sucess= true
            return Json(new { success = true });
        }


        [HttpPost]
        /*Método que remove o produto indicado do carrinho, sumindo com sua refêrencia na Session*/
        public IActionResult RemoveFromCart(int id, int tamanho)
        {
            //O método só é acessado se realmente houverem produtos no Carrinho
            if (HttpContext.Session.Get("CartItens") != null)
            {
                //Deserializa a lista de produtos contida na sessão
                List<Produto> products = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CartItens"));

                //Cria uma variável que pega uma refêrencia a um objeto de produto, quando o ID dele for igual ao inserido pelo método
                var product = products.FindAll(p => p.id == id);

                //Se ele não for nulo, se existir objeto
                if (product != null)
                {
                    //Paraa cada produto achado na lista com id igual ao passado
                    foreach (Produto subp in product)
                    {
                        //Se houver um tamanho, e ele for igual ao inserido, remove-se
                        if (tamanho != 0 && subp.tamanho == tamanho)
                        {
                            //Remove da lista de produtos, o produto que foi encontrado
                            products.Remove(subp);
                            //Serializa de volta a lista de produtos na Session
                            HttpContext.Session.SetString("CartItens", JsonConvert.SerializeObject(products));
                        }
                        else if (subp.tamanho == tamanho) //Se não houver um tamanho, remove-se da mesma forma
                        {
                            //Remove da lista de produtos, o produto que foi encontrado
                            products.Remove(subp);
                            //Serializa de volta a lista de produtos na Session
                            HttpContext.Session.SetString("CartItens", JsonConvert.SerializeObject(products));
                        }
                    }
                }
            }

            //Retorna o método de carrinho, que irá recarregar a página
            return RedirectToAction("Carrinho");
        }


        [HttpPost]
        /*Método que realiza a compra, ao se clicar no botão de finalizar compra*/
        public IActionResult Checkout(List<int> parameterIds, List<int> parameterQntds)
        {
            //Dicionário, que possui chave e valor, para permitir a mesclagem de dados no caso de ids duplicados (aneis com mesmo id e tamanho diferente)
            Dictionary<int, int> idQuantities = new Dictionary<int, int>();

            //Itera cada valor de id
            for (int i = 0; i < parameterIds.Count; i++)
            {
                //Se o dicionário ja tiver uma chave com o mesmo id 
                if (idQuantities.ContainsKey(parameterIds[i]))
                {
                    idQuantities[parameterIds[i]] += parameterQntds[i]; //A quantidade sera somada a quantidade de mesmo indice
                }
                else //Caso contrário se adiciona um novo objeto ao dictionary
                {
                    idQuantities.Add(parameterIds[i], parameterQntds[i]);
                }
            }

            //Cria a lista com os ids e qntds atualizados
            List<int> ids = new List<int>(idQuantities.Keys);
            List<int> qntds = new List<int>(idQuantities.Values);


            //Verificação Server-side para ver se a quantidade insirida é negativa ou menor que a disponível em estoque
            for (int i = 0; i < ids.Count; i++)
            {
                if (qntds[i] < 1 || HttpContext.Session.GetInt32("Product_" + ids[i]) < qntds[i])
                {
                    return Json(new { sucess = false, message = "A quantidade desejada é impossível" });
                }
            }


            //O método só é acessado se realmente houverem produtos no Carrinho
            if (HttpContext.Session.Get("CartItens") != null)
            {

                if (ids.Distinct().Count() != ids.Count)
                {
                }
                //Deserializa a lista de produtos da Session
                List<Produto> products = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CartItens"));

                //Remove todos os produtos do carrinho que possuem os ids compatíveis com os IDS da lista enviada pelo método
                products.RemoveAll(p => ids.Contains(p.id));

                //Serializa o novo objeto de produtos, agora sem os valores dos produtos removidos
                HttpContext.Session.SetString("CartItens", JsonConvert.SerializeObject(products));

                //Cria uma nova lista de Produtos, para salvar os produtos que foram comprados, e diminuir sua quantidade
                List<Produto> checkedOutProducts = new List<Produto>();

                //Adiciona todos os IDS e Quantidades que foram enviadas pelo método para dentro dessa lista
                for (int i = 0; i < ids.Count; i++)
                {
                    checkedOutProducts.Add(new Produto { id = ids[i], qntd = qntds[i] });
                }

                //Se ela ainda não existia, apenas se Serializa a lista na Session com chave CheckedOutProducts
                if (HttpContext.Session.Get("CheckedOutProducts") == null)
                {
                    HttpContext.Session.SetString("CheckedOutProducts", JsonConvert.SerializeObject(checkedOutProducts));
                }
                else //Caso já existisse
                {
                    //Cria uma nova lista que deserializa os valores ja existentes de produtos comprados
                    List<Produto> existingCheckedProducts = JsonConvert.DeserializeObject<List<Produto>>(HttpContext.Session.GetString("CheckedOutProducts"));

                    //Para cada produto que exista na lista nova de produtos comprados
                    foreach (Produto atual in checkedOutProducts)
                    {
                        //Se houver um correspondente desse produto atual, dentro da lista antiga, que foi deserializada
                        Produto existingProduct = existingCheckedProducts.FirstOrDefault(p => p.id == atual.id);
                        if (existingProduct != null)
                        {
                            //Define a quantidade como a soma da antiga com a nova
                            existingProduct.qntd += atual.qntd;
                        }
                        else //Se não houver correspondente antigo, apenas se adiciona o produto atual na lista de produtos existentes
                        {
                            existingCheckedProducts.Add(atual);
                        }
                    }

                    //Novamente se serializa o objeto da lista de produtos que existem
                    HttpContext.Session.SetString("CheckedOutProducts", JsonConvert.SerializeObject(existingCheckedProducts));

                }

            }

            //Retorna um JSON com sucess= true
            return Json(new { success = true });
        }
    }
}
