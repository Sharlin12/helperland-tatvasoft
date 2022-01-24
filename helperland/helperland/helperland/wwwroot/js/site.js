// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// equal height

function equalHeight() {
    jQuery.fn.extend({
        equalHeight: function () {
            var top = 0;
            var row = [];
            var classname = ('equalHeight' + Math.random()).replace('.', '');
            jQuery(this).each(function () {
                var thistop = jQuery(this).offset().top;
                if (thistop > top) {
                    jQuery('.' + classname).removeClass(classname);
                    top = thistop;
                }
                jQuery(this).addClass(classname);
                jQuery(this).height('auto');
                var h = (Math.max.apply(null, jQuery('.' + classname).map(function () {
                    return jQuery(this).outerHeight();
                }).get()));
                jQuery('.' + classname).height(h);
            }).removeClass(classname);
        }
    });
    jQuery('.why-helperhand .helperhand-wrapper h3').equalHeight();

}

function fixed_header() {
    if (jQuery(window).scrollTop() > 0) {
        jQuery('.nav-section').addClass('fixed');
    }
    else {
        jQuery('.nav-section').removeClass('fixed');
    }
}
jQuery(document).ready(function () {

    equalHeight();
    fixed_header();
    var a = $(".nav-section .navbar").offset().top;

    $(document).scroll(function () {
        $('.nav-section .navbar').toggleClass('navbar-scroll', $(this).scrollTop() > a);
    });

    $(window).scroll(function () {
        var scroll = getCurrentScroll();
        if (scroll >= a) {
            $('.nav-section .navbar').addClass('shrink');
        }
        else {
            $('.nav-section .navbar').removeClass('shrink');
        }
    });
    function getCurrentScroll() {
        return window.pageYOffset || document.documentElement.scrollTop;
    }


    jQuery('.scroll-link').click(function (e) {
        e.preventDefault();
        jQuery('html,body').animate({
            scrollTop: jQuery('.why-helperhand').offset().top - jQuery('.nav-section .navbar').outerHeight()
        }, 1500);
    });
    jQuery('.scroll-1').click(function (e) {
        e.preventDefault();
        jQuery('html,body').animate({
            scrollTop: jQuery('.how-it-works').offset().top - jQuery('.nav-section .navbar').outerHeight()
        }, 1500);
    });
    jQuery('.scroll-top').click(function (e) {
        e.preventDefault();
        jQuery('html,body').animate({
            scrollTop: 0
        }, 1000);
    });
    jQuery('.ok-btn').click(function (e) {
        e.preventDefault();
        var _this = jQuery(this);
        _this.closest('.footer-bottom').slideUp();

    });


});
jQuery(window).resize(function () {
    equalHeight();
});
jQuery(window).on('load', function () {
    setTimeout(function () {
        equalHeight();
    }, 500)
});
jQuery(window).scroll(function () {
    fixed_header();
});
