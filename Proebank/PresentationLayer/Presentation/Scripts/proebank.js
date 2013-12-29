$(window).load(function () {
    if (jQuery.ui) {
        $(".datepicker").datepicker({
            dateFormat: 'yy-mm-dd'
        });
    }
    if (!Modernizr.inputtypes.date) {
        $('input[type=date]').datepicker({
            dateFormat: 'yy-mm-dd'
        });
    }
    $('input, select').addClass('form-control');
});