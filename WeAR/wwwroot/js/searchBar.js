var searchbar = document.getElementById("divBusca");
var buttonPesquisar = document.getElementById("btnPesquisar");
var navList = document.getElementById("nav-list");
searchbar.style.display = "none";

buttonPesquisar.addEventListener("click", function () {
    // Check if searchbar is already displayed
    if (searchbar.style.display === "none") {
        // Set searchbar display to block
        searchbar.style.display = "block";
        // Get parent element of navList
        var parent = navList.parentNode;
        // Replace navList with searchbar
        parent.replaceChild(searchbar, navList);
    } else {
        // Hide searchbar if it's already visible
        searchbar.style.display = "none";
        // Get parent element of searchbar
        var parent = searchbar.parentNode;
        // Replace searchbar with navList
        parent.replaceChild(navList, searchbar);
    }
});
var btnBusca = document.getElementById("btnFecharBusca");
btnBusca.addEventListener("click", function () {
    // Hide searchbar
    searchbar.style.display = "none";
    // Get parent element of searchbar
    var parent = searchbar.parentNode;
    // Replace searchbar with navList
    parent.replaceChild(navList, searchbar);
});