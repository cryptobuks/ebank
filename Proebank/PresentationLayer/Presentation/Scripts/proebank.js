$(window).load(function () {
    $(".datepicker").datepicker({
        dateFormat: 'dd-mm-yy'
    });

    $('input, select').addClass('form-control');
});