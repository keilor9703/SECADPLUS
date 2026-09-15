$(function () {
    $(".chosen-select").chosen();
    $("#btnCambiarAcd").click(function () {
        CambiarAcd();
    });
    $(".owl-carousel").owlCarousel({
        autoPlay: 3000,
        stopOnHover: true,
        navigation: true,
        paginationSpeed: 1000,
        goToFirstSpeed: 2000,
        singleItem: true,
        //autoHeight : true,
        transitionStyle: "fade",
        items: 1
    });

    var owl = $('.owl-carousel');
    var items = $('.owl-item');
    var contador = 0;
    var opcion = true;
    var cuenta = parseInt(items.size().toString());
    setInterval(function () {
        if (cuenta == contador) opcion = false;
        if (opcion == true) {
            owl.trigger('next.owl.carousel');
            contador++;
        }
        else {
            owl.trigger('prev.owl.carousel');
            contador--;
            if (contador == 0) opcion = true;
        }
    }, 4000);
});
function CambiarAcd() {
    var acd = $("#listaAcd").val();
    if (acd === "") {
        GestionarAlerta("warning","Debe seleccionar un acd.");
        return false;
    }
    var datos = { intSitio: glSitio, strAcd: acd };
    $.ajax({
        type: "POST",
        url: urlCambiarAcd,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        error: function (msn) {
            GestionarAlerta("warning", msn);
        }
    }).done(function (resultado) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
        if (resultado.result) {
            GestionarAlerta("success", resultado.datos);
            //Recarga la página
            location.reload();
        } else {
            //implementar cuando existan problemas
            GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
        }
    });
}