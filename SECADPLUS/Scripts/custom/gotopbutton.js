$(document).ready(function () {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 50) {
            $('#back-to-top').fadeIn();
        } else {
            $('#back-to-top').fadeOut();
        }
    });
    // scroll body to 0px on click
    $('#back-to-top').click(function () {
        $('#back-to-top').tooltip('hide');
        $('body,html').animate({
            scrollTop: 0
        }, 800);
        return false;
    });

    $('#back-to-top').tooltip('show');
});

function EliminarRegistroGeneral(ruta) {
    url = ruta;
    $.ajax(
     {
         url: url,
         method: "POST",
         dataType: "html",
         success: function (data) {
             //  console.log(data);
             swal({
                 title: "",
                 text: data,
                 type: "warning",
                 html: true,
                 showCancelButton: true,
                 confirmButtonColor: "#EF5350",
                 cancelButtonColor: "#EF5385",
                 confirmButtonText: "Eliminar",
                 cancelButtonText: "Cancelar",
                 closeOnConfirm: false,
                 closeOnCancel: true
             },
             function (isConfirm) {
                 if (isConfirm) {
                     $("#form_eliminar").submit();
                 }
             }
             );
         },
         error: function (data) {
             console.log(data);
         }
     });
}