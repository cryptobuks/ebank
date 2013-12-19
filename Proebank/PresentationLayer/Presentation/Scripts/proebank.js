$(window).load(function () {
    if (jQuery.ui) {
        $(".datepicker").datepicker({
            dateFormat: 'dd-mm-yy'
        });
    }
    
    $('input, select').addClass('form-control');
});