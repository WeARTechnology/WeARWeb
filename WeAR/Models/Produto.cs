using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeAR.Models
{
    public class Produto
    {
        //Definição de atributos da classe
        String desc, nome, tipo;
        Double preco;
        int qntd, tamanho;

        //Construtor do Anel (Somente tamanho, sem tipo)
        public Produto(string desc, string nome, double preco, int qntd, int tamanho)
        {
            this.desc = desc;
            this.nome = nome;
            this.preco = preco;
            this.qntd = qntd;
            this.tamanho = tamanho;
        }

        //Construtor do Óculos (Somente tipo, sem tamanho)
        public Produto(string desc, string nome, string tipo, double preco, int qntd)
        {
            this.desc = desc;
            this.nome = nome;
            this.tipo = tipo;
            this.preco = preco;
            this.qntd = qntd;
        }


        


    }
}
