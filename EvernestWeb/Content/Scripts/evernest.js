$(document).ready(function () {

    /* Smooth move between home page slides */
    $(".smooth-link").each(function (link) {
        link.click(function (e) {
            $('body,html').animate({
                scrollTop: $(this.attr('href')).offset().top
            }, 1000);
            window.location.hash = this.attr('href');
            return false;
        });
    });
});