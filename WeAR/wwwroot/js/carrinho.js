
$(document).ready(function () {
    calc();
})

document.getElementById("finalizar").addEventListener("click", checkout);

function calc() {
    var allInputs = document.querySelectorAll("input[id^='quantity_']");

    allInputs.forEach(input => {
        input.addEventListener("focusin", caclAll);
        input.addEventListener("focusout", caclAll);
        input.addEventListener("change", caclAll);
    });
}



function checkout() {
    var quantities = document.querySelectorAll("input[id^='quantity_']");
    var productIds = [];
    var quantitiesList = [];
    var url = checkoutURL;


    for (var i = 0; i < quantities.length; i++) {
        productIds.push(parseInt(quantities[i].id.replace("quantity_", "")));
        quantitiesList.push(parseInt(quantities[i].value));
        if (updateSessionQuantity(ids[i], quantitiesList[i]) == false) {
            return false;
        }
    }

 

    var data = {
        ids: productIds,
        qntds: quantitiesList
    };

    $.ajax({
        type: 'POST',
        url: url,
        data: data,
        success: function (response) {
            location.reload();
        },
        error: function () {
            console.error('Failed to checkout');
        }
    });

    return true;
}

function caclAll() {
    var subtotals = document.querySelectorAll("h3[id^='subtotal_']");
    var ids = [];
    var values = 0;
    var quantity = 0;
    var preco = 0.00;
    var total = 0;

    for (var i = 0; i < subtotals.length; i++) {
        ids.push(subtotals[i].id.replace("subtotal_", ""));

        quantity = parseInt(document.getElementById("quantity_" + ids[i]).value);
        preco = parseFloat(document.getElementById("preco_" + ids[i]).textContent.replace(",", "."))

        values = preco * quantity;
        total += values;

        document.getElementById("subtotal_" + ids[i]).textContent = "Subtotal:" + values.toFixed(2);
        updateSessionQuantity(ids[i],quantity)
    }

    document.getElementById("valor").textContent = total.toFixed(2);
}



function updateSessionQuantity(productId, newQuantity) {
    var url = updateQuantityUrl;
    var data = { id: productId, quantity: newQuantity };

    $.ajax({
        type: 'POST',
        url: url,
        data: data,
        success: function (response) {
            if (!response.success) {
                console.error('Failed to update session quantity');
                window.alert(response.message);
                document.getElementById("quantity_" + productId).value = 1;
                updateSessionQuantity(productId, 1);
                calc();
                return false;

            }
        },
        error: function () {
            console.error('Failed to update session quantity');
        }
    });

    return true;
}


function removeProduct(productId) {
    var confirmation = confirm("Deseja realmente excluir o produto?");
    if (confirmation) {
        var url = removeFromCartUrl;
        var data = { id: productId };

        $.ajax({
            type: 'POST',
            url: url,
            data: data,
            success: function (response) {
                location.reload();
            },
            error: function () {
                console.error('Failed to remove product from cart');
            }
        });
    }
}



function mais(productID) {
    var total = parseInt(document.getElementById("quantity_" + productID).value);
    var novo = total + 1;
    document.getElementById("quantity_" + productID).value = novo;
    updateSessionQuantity(productID, novo);
    caclAll();
}

function menos(productID) {
    var total = parseInt(document.getElementById("quantity_" + productID).value);
    if (total - 1 > 0) {
        var novo = total - 1;
        document.getElementById("quantity_" + productID).value = novo;
        updateSessionQuantity(productID, novo);
        caclAll();

    }
    else {
        removeProduct(productID);
    }

}