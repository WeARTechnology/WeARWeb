//Função JQuery, que, quando a View terminar de renderizar, quando estiver pronta, ele chama o método calc();
$(document).ready(function () {
    calc();

    //Quando se clica no botão finalizar, ele realiza a função Checkout
    var finalizarButton = document.getElementById("finalizar");
    if (finalizarButton) { //Se realmente houver esse objeto no DOM
        finalizarButton.addEventListener("click", checkout);
    }
})



//Função que pega todos os inputs de valor quantidade, e adiciona um EventListener neles, sempre que forem modificados, chamam a função calcAll
function calc() {
    var allInputs = document.querySelectorAll(".quantity_"); //Pega todos os inputs com o valor quantity_ID
    if (allInputs) {                //Se houver esse objeto no DOM
        allInputs.forEach(input => { //Para cada input, adiciona o Listener
            input.addEventListener("change", calcAll);
        });
    }
}

//Função chamada ao clicar no botão para finalizar a compra
function checkout() {
    //Definição de variáveis
    var quantities = document.querySelectorAll(".quantity_"); //Pega todos os inputs de class quantity_ID
    var productIds = [];
    var quantitiesList = [];
    var tamanho = [];
    var url = checkoutURL;


    //For que roda a quantidade de vezes equivalente a quantidade de inputs obtidos
    for (var i = 0; i < quantities.length; i++) {
        //Adiciona as Arrays productIds e quantitiesList, os valores do ID obtido pelo class do quantities, a quantidade e o tamanho
        productIds.push(parseInt(quantities[i].className.replace("quantity_", "")));
        quantitiesList.push(parseInt(quantities[i].value));
        tamanho.push(parseInt(quantities[i].getAttribute("data-tamanho")));

        //Se, ao tentar dar update nesses valores no banco, retornar falso (simbolizando que os valores desejados são impossiveis)
        if (!updateSessionQuantity(productIds[i], quantitiesList[i],tamanho)) {
            return false;
        }
    } 

    //Define os dados que serão passados na requisição AJAX, como os Ids e Quantidades
    var data = {
        parameterIds: productIds,
        parameterQntds: quantitiesList
    };



    //Inicia uma requisição AJAX
    $.ajax({
        type: 'POST', //Define o tipo da requisição como POST
        url: url, //A url será a URL da variavel
        data: data, //Os dados serão da variável data
        success: function (response) { //Cria uma função para pegar o resultado
            if (!response.success) //Se a resposta não for sucesso
            {
                console.error('Failed to update session quantity');
                window.alert(response.message); //Envia a mensagem de erro
                return false; //Volta falso, terminando a função
            }
            else
            {
                location.reload(); //Reinicia a página
            }
            
        },
        error: function () { //Caso dê erro, ele dispara um alerta para o usuário
            alert('Ocorreu um erro ao tentar realizar o checkout. Por favor, tente novamente.');
        }
    });

    return true; //Volta true
}

//Função Assincrona que calcula o subtotal e o total
async function calcAll() {
    var subtotals = document.querySelectorAll(".subtotal_"); //Pega todos os textos com class subtotal_ID
    var ids = [];
    var values = 0;
    var quantity = 0;
    var preco = 0.00;
    var total = 0;
    var tamanho;
    var subtotalsID = [];

    //Para cada texto que existir com o id subtotal
    for (var i = 0; i < subtotals.length; i++) {
        //Pega o id, usando o class de subtotal, e tirando a parte escrita subtotal_, sobrando apenas o ID
        ids.push(subtotals[i].className.replace("subtotal_", ""));        
    }
    var idsUnicos = ids.filter((elemento, index, array) => {
        return array.indexOf(elemento) === index;
    });

    for (var i = 0; i < idsUnicos.length; i++) {
        subtotalID.push(document.querySelectorAll(".subtotal_" + ids[i]));        
    }

    
    for (var i = 0; i < subtotalsID.length; i++) {
        //Pega a quantidade, puxando do elemento quantity_ID, usando o id obtido acima
        quantity = parseInt(document.querySelector(".quantity_" + ids[i]).value);

        //Pega o tamanho, com o data-tamanho da quantidade
        tamanho = parseInt(document.querySelector(".quantity_" + ids[i]).getAttribute("data-tamanho"));


        //Se a quantidade for negativa, ou null, define ela como 1, e passa o valor para a View
        if (isNaN(quantity) || quantity < 1) {
            quantity = 1;
            document.querySelector(".quantity_" + ids[i]).value = quantity;
        }

        //Caso a função updateSessionQuantity retornar null, ele quebra o método, mostrando a mensagem de erro da função, caso contrário, continua a execução normalmente
        if (await updateSessionQuantity(ids[i], quantity, tamanho)) {
            preco = parseFloat(document.querySelector(".preco_" + ids[i]).textContent.replace(",", ".")); //Pega o preço, em decimal, trocando , por .
            values = preco * quantity;
            total += values;
            subtotalsID[i].textContent = "Subtotal:" + values.toFixed(2);
        }
        else {
            break;
        }  
    }

    //Ao final, define o total como o total obtido nas contas
    document.getElementById("valor").textContent = total.toFixed(2);
}


//Função que atualiza a quantidade que será comprada, valor armazenado na Session
function updateSessionQuantity(productId, newQuantity, newSize) {
    var url = updateQuantityUrl; //Pega a URL
    var data = { id: productId, quantity: newQuantity , tamanho: newSize}; //Define os dados que serão passados, como o id e a quantidade passadas na função

    //Cria um Promise, pois o método a partir daqui será assincrono por conta do AJAX, a promise será o resultado dessa operação
    return new Promise((resolve, reject) => {
        //Cria um ajax
        $.ajax({
            type: 'POST', //Tipo de operação como POST
            url: url, //Url como a var URL
            data: data, //Dados como a var Data

            success: function (response) { //Função que pega o resultado da operação
                if (!response.success) { // Caso a resposta *não* seja sucesso
                    //Envia uma mensagem na tela
                    window.alert(response.message);
                    //Define a quantidade como 1
                    document.querySelector(".quantity_" + productId).value = 1;
                    //Atualiza novamente a session, definindo a quantidade como 1
                    updateSessionQuantity(productId, 1);
                    //Chama o método que calcula o Total e Subtotal novamente
                    calcAll();
                    //Retorna falso no promise
                    resolve(false);
                } else {
                    //Caso contrário, retorna true no promise
                    resolve(true);
                }
            },
            error: function () { //Função em caso de erro
                //Envia uma alert para o usuário
                alert('Ocorreu um erro ao tentar atualizar a quantidade. Por favor, tente novamente.');
                reject(); //Retorna o promise com a informação de que a operação falhou
            }
        });
    });
}


//Função que remove o produto do carrinho
function removeProduct(productId,size) {
    //Pergunta se o usuário realmente deseja excluir o produto do carrinho
    var confirmation = confirm("Deseja realmente excluir o produto?");

    //Se a resposta for sim
    if (confirmation) {
        var url = removeFromCartUrl; //Url como variavel global
        var data = { id: productId , tamanho: size}; //Dados passados são o id pego através da função

        //Inicio da chamada AJAX
        $.ajax({
            type: 'POST', //Define o tipo como POST
            url: url, //URL como a var URL
            data: data, //Dados como a var Data

            success: function (response) {//Função que pega a resposta
                location.reload(); //Recarrega a página, caso for sucesso
            },
            error: function () { //Função que pega o erro
                //Envia um alert ao usuário e manda uma mensagem no console
                alert("Algo deu errado, tente novamente");
                console.error('Falha ao remover o produto do carrinho');
            }
        });
    }
}


//Função chamada no click do botão +, do input de quantidade
function mais(productID, tamanho) {
    //Caso o tamanho for diferente de 0, o que indica que possui tamanho, ou seja, pode-se possuir mais de um produto com o mesmo id
    if (tamanho != 0) {

        //Pega todos os elementos com o class quantity_ID
        var elemento = document.querySelectorAll(".quantity_" + productID);      

        //Para cada um
        for (var i = 0; i < elemento.length; i++) {
            //Pega o atributo de data
            var tamanhoElement = elemento[i].getAttribute("data-tamanho");

            if (tamanhoElement == tamanho) { //Se a data, for igual ao tamanho passado no método
                var total = parseInt(elemento[i].value); //Pega o valor da quantidade do elemento
                if (isNaN(total) || total < 1) { //Caso o total for null ou negativo, define-o como 1
                    total = 1;
                }
                var novo = total + 1; //Define o novo valor como o total +1

                elemento[i].value = novo; //Passa para a View
                updateSessionQuantity(productID, novo, tamanho); //Chama o método para atualizar o valor na Sessão
                calcAll(); //Calcula o total e subtotal
                return;
            }
        }

    }
    var total = parseInt(document.querySelector(".quantity_" + productID).value); //Pega o valor da quantidade através do input usando o ID
    if (isNaN(total) || total < 1) { //Caso o total for null ou negativo, define-o como 1
        total = 1;
    }
    var novo = total + 1; //Define o novo valor como o total +1

    document.querySelector(".quantity_" + productID).value = novo; //Passa para a View
    updateSessionQuantity(productID, novo, tamanho); //Chama o método para atualizar o valor na Sessão
    calcAll(); //Calcula o total e subtotal
}

//Função chamada no click do botão -, do input de quantidade
function menos(productID,tamanho) {
    //Caso o tamanho for diferente de 0, o que indica que possui tamanho, ou seja, pode-se possuir mais de um produto com o mesmo id
    if (tamanho != 0) {

        //Pega todos os elementos com o class quantity_ID
        var elemento = document.querySelectorAll(".quantity_" + productID);

        //Para cada um
        for (var i = 0; i < elemento.length ; i++) {
            //Pega o atributo de data
            var tamanhoElement = elemento[i].getAttribute("data-tamanho");

            if (tamanhoElement == tamanho) { //Se a data, for igual ao tamanho passado no método
                var total = parseInt(elemento[i].value); //Pega o valor da quantidade do elemento
                if (isNaN(total)) { //Caso o total for null, define-o como 1
                    total = 1;
                }

                if (total - 1 > 0) { //Se o total -1 for maior que 0, ou seja, positivo, roda a função

                    var novo = total - 1; //Define o novo valor como o total -1
                    elemento[i].value = novo;//Passa para a View
                    updateSessionQuantity(productID, novo, tamanho);//Chama o método para atualizar o valor na Sessão
                    calcAll(); //Calcula o total e subtotal
                    return;

                }
                else //Caso seja menor ou igual a 0, roda a função para excluir o produto do carrinho
                {
                    removeProduct(productID,tamanho);
                    return;
                }
            }
        }

    }


    var total = parseInt(document.querySelector(".quantity_" + productID).value); //Pega o valor da quantidade através do input usando o ID

    if (isNaN(total)) { //Caso o total for null, define-o como 1
        total = 1;
    }

    if (total - 1 > 0) { //Se o total -1 for maior que 0, ou seja, positivo, roda a função

        var novo = total - 1; //Define o novo valor como o total -1
        document.querySelector(".quantity_" + productID).value = novo;//Passa para a View
        updateSessionQuantity(productID, novo, tamanho);//Chama o método para atualizar o valor na Sessão
        calcAll(); //Calcula o total e subtotal

    }
    else //Caso seja menor ou igual a 0, roda a função para excluir o produto do carrinho
    {
        removeProduct(productID, tamanho);
    }

}