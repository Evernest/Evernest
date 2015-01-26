$(document).ready(function () {

    /* Smooth move between home page slides */
    $(".smooth-link").each(function (id, link) {
        $(link).click(function (e) {
            var target = $(this).attr('href');
            $('body,html').animate({
                scrollTop: $(target).offset().top
            }, 'slow', function() {
                window.location.hash = target;
            });
            return false;
        });
    });
});