$('.owl-carousel').owlCarousel({
    loop: true,
    margin: 50,
    nav: false,
    dots: true,
    // autoplay: true,
    // autoplayTimeout: 5000,
    stagePadding: 40,
    responsive: {
        0: {
            items: 1
        },
        600: {
            items: 2
        },
        1000: {
            items: 3
        },
        1400: {
            items: 4
        }
    }
})