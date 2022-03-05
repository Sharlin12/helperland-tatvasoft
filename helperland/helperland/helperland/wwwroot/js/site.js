// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// equal height
function BookCleaner() {
    document.getElementById('id01').style.display = 'block';
    document.getElementById('#hiddenfield').value = 'bookcleaner'
}
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


var totaltime;
var totalprice;
var Effectiveprice;
var extrahrs=0;
jQuery(document).ready(function () {
    $(function () {
        $("#txtServiceStartDate").datepicker({ dateFormat: 'dd-mm-yy', minDate: 0 }).val();
        $("#txtServiceStartDate").datepicker("setDate", "0");

    });
    $("#txtServiceStartDate").change(function () {
        var x = $("#txtServiceStartDate").val();
        $(".dateinsummary").text(x);
    });
    $("#timeselect").change(function () {
        var x = $("#timeselect").val();
        $(".timeinsummary").text(x);
    });
    $("#totaltime").change(function () {
        var x = $("#totaltime").val();
        $(".basicHr").text(x);


        totaltime = parseFloat(x);
        if ($('#chbox1').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox2').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox3').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox4').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox5').is(":checked")) {
            totaltime = totaltime + 0.5;
        }

        $(".tst").text(totaltime);
        totalprice = totaltime * 18;
        $(".totalprice").text(totalprice);
        $(".totalprice1").text(totalprice);
        Effectiveprice = (totalprice * 80) / 100;
        $(".Effectiveprice").text(Effectiveprice);
    });


    
    $("#chbox1").unbind('click').click(function (e) {
        if ($(this).is(":checked")) {
            $(".basicHr1").append('<tr id=\'cabinate\'><td>Inside Cabinets(Extra)</td><td >30 Mins</td></tr>');
            $('#totaltime option:selected').next().prop('selected', true);
            extrahrs = extrahrs + 0.5;
            $("#extraHrs").val(extrahrs);
            $("#extra1").val(true);
        }
        else {
            $("#cabinate").remove();
            $('#totaltime option:selected').prev().prop('selected', true);
            extrahrs = extrahrs - 0.5;
            $("#extraHrs").val(extrahrs);
            $("#extra1").val(false);
        }

        var x = $("#basicHr").text();
        totaltime = parseFloat(x);
        if ($('#chbox1').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox2').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox3').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox4').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox5').is(":checked")) {
            totaltime = totaltime + 0.5;
        }

        $(".tst").text(totaltime);
        totalprice = totaltime * 18;
        $(".totalprice").text(totalprice);
        $(".totalprice1").text(totalprice);
        Effectiveprice = (totalprice * 80) / 100;
        $(".Effectiveprice").text(Effectiveprice);
    });
    $("#chbox2").unbind('click').click(function (e) {
        if ($("#chbox2").is(":checked")) {
            $(".basicHr1").append('<tr id=\'fridge\'><td>Inside fridge(Extra)</td><td >30 Mins</td></tr>');
            $('#totaltime option:selected').next().prop('selected', true);
            extrahrs = extrahrs + 0.5;
            $("#extraHrs").val(extrahrs);
            $("#extra2").val(true);
        }
        else {
            $("#fridge").remove();
            $('#totaltime option:selected').prev().prop('selected', true);
            extrahrs = extrahrs - 0.5;
            $("#extraHrs").val(extrahrs);
            $("#extra2").val(false);
        }
        var x = $("#basicHr").text();
        totaltime = parseFloat(x);
        if ($('#chbox1').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox2').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox3').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox4').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox5').is(":checked")) {
            totaltime = totaltime + 0.5;
        }

        $(".tst").text(totaltime);
        totalprice = totaltime * 18;
        $(".totalprice").text(totalprice);
        $(".totalprice1").text(totalprice);
        Effectiveprice = (totalprice * 80) / 100;
        $(".Effectiveprice").text(Effectiveprice);
    });
    
    $("#chbox3").unbind('click').click(function (e) {
        if ($("#chbox3").is(":checked")) {
            $(".basicHr1").append('<tr id=\'Oven\'><td>Inside Oven(Extra)</td><td >30 Mins</td></tr>');
            $('#totaltime option:selected').next().prop('selected', true);
            extrahrs = extrahrs + 0.5;
            $("#extraHrs").val(extrahrs);
            $("#extra3").val(true);
        }
        else {
            $("#Oven").remove();
            $('#totaltime option:selected').prev().prop('selected', true);
            extrahrs = extrahrs - 0.5;
            $("#extraHrs").val(extrahrs);
            $("#extra3").val(false);
        }

        var x = $("#basicHr").text();
        totaltime = parseFloat(x);
        if ($('#chbox1').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox2').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox3').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox4').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox5').is(":checked")) {
            totaltime = totaltime + 0.5;
        }

        $(".tst").text(totaltime);
        totalprice = totaltime * 18;
        $(".totalprice").text(totalprice);
        $(".totalprice1").text(totalprice);
        Effectiveprice = (totalprice * 80) / 100;
        $(".Effectiveprice").text(Effectiveprice);
    });
    
    $("#chbox4").unbind('click').click(function (e) {
        if ($("#chbox4").is(":checked")) {
            $(".basicHr1").append('<tr id=\'Laundry\'><td>Laundry & Wash(Extra)</td><td >30 Mins</td></tr>');
            $('#totaltime option:selected').next().prop('selected', true);
            extrahrs = extrahrs + 0.5;
            $("#extraHrs").val(extrahrs);
            $("#extra4").val(true);
        }
        else {
            $("#Laundry").remove();
            $('#totaltime option:selected').prev().prop('selected', true);
            extrahrs = extrahrs - 0.5;
            $("#extraHrs").val(extrahrs);
            $("#extra4").val(false);
        }
        var x = $("#basicHr").text();
        totaltime = parseFloat(x);
        if ($('#chbox1').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox2').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox3').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox4').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox5').is(":checked")) {
            totaltime = totaltime + 0.5;
        }

        $(".tst").text(totaltime);
        totalprice = totaltime * 18;
        $(".totalprice").text(totalprice);
        $(".totalprice1").text(totalprice);
        Effectiveprice = (totalprice * 80) / 100;
        $(".Effectiveprice").text(Effectiveprice);
    });
    
    $("#chbox5").unbind('click').click(function (e) {
        if ($("#chbox5").is(":checked")) {
            $(".basicHr1").append('<tr id=\'Window\'><td>Interior Windows(Extra)</td><td >30 Mins</td></tr>');
            $('#totaltime option:selected').next().prop('selected', true);
            extrahrs = extrahrs + 0.5;
            $("#extraHrs").val(extrahrs);
            $("#extra5").val(true);
        }
        else {
            $("#Window").remove();
            $('#totaltime option:selected').prev().prop('selected', true);
            extrahrs = extrahrs - 0.5;
            $("#extraHrs").val(extrahrs);
            $("#extra1").val(false);
        }
        var x = $("#basicHr").text();
        totaltime = parseFloat(x);
        if ($('#chbox1').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox2').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox3').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox4').is(":checked")) {
            totaltime = totaltime + 0.5;
        }
        if ($('#chbox5').is(":checked")) {
            totaltime = totaltime + 0.5;
        }

        $(".tst").text(totaltime);
        totalprice = totaltime * 18;
        $(".totalprice").text(totalprice);
        $(".totalprice1").text(totalprice);
        Effectiveprice = (totalprice * 80) / 100;
        $(".Effectiveprice").text(Effectiveprice);
    });
   
    $('.tst').on('DOMSubtreeModified', function () {
        var x = $("#tst").text();
        $("#serviceHrs").val(x);
    });
    $('.totalprice').on('DOMSubtreeModified', function () {
        var x = $("#totalprice").text();
        $("#subtotal").val(x);
    });
    $('.totalprice1').on('DOMSubtreeModified', function () {
        var x = $("#totalprice1").text();
        $("#totalcost").val(x);
    });
    $("#txtServiceStartDate").change(function () {
        var x = $("#txtServiceStartDate").val();
        $("#servicedate").val(x);
    });
    $("#timeselect").change(function () {
        var x = $("#timeselect :selected").val();
        $("#servicetime").val(x);
      
    });
    $("#exampleFormControlTextarea1").change(function () {
        var x = $("#exampleFormControlTextarea1").val();
        $("#sevicecomments").val(x);
    });
    $("#servicehaspets1").change(function () {
        if ($("#servicehaspets1").is(":checked")) {
            $("#servicehaspats").val(true);
            
        }
        else {
            $("#servicehaspats").val(false);
        }
    });
    $("#exampleFormControlTextarea1").change(function () {
        var x = $("#exampleFormControlTextarea1").val();
        $("#sevicecomments").val(x);
    });
    
});
var today = new Date();
document.getElementById('dateinsummary').innerHTML = (today.getDate() + '-' + (today.getMonth() + 1) + '-' + today.getFullYear());
$("#servicedate").val((today.getDate() + '-' + (today.getMonth() + 1) + '-' + today.getFullYear()));
function saveAddress() {
    jQuery(document).ready(function () {
        var street = $("#streetname").val();
        var house = $("#housenumber").val();
        var phone = $("#phone").val();
        var city = $("#city").val();
        var ps = $("#postalcode").val();
        if (street!="" && house !="") {
            $("#userAddress").append("<li class='ng - untouched ng - pristine ng - valid'><label><input formcontrolname='address' checked class='ng - untouched ng - pristine ng - valid' name='address' type='radio' id='109' ><span class='address-block'><b >Address:</b>" + street + "," + house + "</span><span><b>Phone number:</b>" + phone + "</span><span class='radio-pointer'></span></label></li>");
            $('#address-form').hide();
            $('#bttn').show();
        }
        else if (phone=="" && street != "" && house != ""){
            $("#userAddress").append("<li class='ng - untouched ng - pristine ng - valid'><label><input formcontrolname='address' checked class='ng - untouched ng - pristine ng - valid' name='address' type='radio' id='109' ><span class='address-block'><b >Address:</b>" + street + "," + house + "</span><span class='radio-pointer'></span></label></li>");
            $('#address-form').hide();
            $('#bttn').show();
        }
        $("#newad1").val(street);
        $("#newad2").val(house);
        $("#newpc").val(ps);
        $("#newcity").val(city);
        $("#newmobile").val(phone);
        
    });
}
