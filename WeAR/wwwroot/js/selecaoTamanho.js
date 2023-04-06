const selecionado = document.querySelector(".selecionado");
const containerOpcoes = document.querySelector(".containerOpcoes");

const optionsList = document.querySelectorAll(".opcao");

selecionado.addEventListener("click", () => {
    containerOpcoes.classList.toggle("active");
});

optionsList.forEach( o => {
    o.addEventListener("click", () => {
        selecionado.innerHTML = o.querySelector("label").innerHTML;
        containerOpcoes.classList.remove("active");
    });
});
