$(document).ready(function () {

    /* Smooth move between home page slides */
    $(".smooth-link").each(function (id, link) {
        console.log(link);
        $(link).click(function (e) {
            console.log("test");
            $('body,html').animate({
                scrollTop: $($(this).attr('href')).offset().top
            }, 'slow', function() {
                window.location.hash = $(this).attr('href');
            });
            return false;
        });
    });
});