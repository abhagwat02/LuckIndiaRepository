
$(function () {
    
    $("ul.nav li.parent > a ").click(function () {
        $(this).find('em').toggleClass("fa-minus");
    });
    $(".sidebar span.icon").find('em:first').addClass("fa-plus");

    $(window).resize(function () {
        if ($(window).width() > 768) $('#sidebar-collapse').collapse('show')       
    });

    $(window).resize(function () {        
        if ($(window).width() <= 767) $('#sidebar-collapse').collapse('hide')
    });
});