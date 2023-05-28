using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Collections.Generic;

namespace WeAR.Models
{
    public class ProdutoComImagem : Produto
    {
        public string imagem { get; set; }
        public int idSimilar { get; set; }

        public ProdutoComImagem(string imagem, int idSimilar)
        {
            this.imagem = imagem;
            this.idSimilar = idSimilar;
        }

        public ProdutoComImagem()
        {            
            
        }
    }
}
