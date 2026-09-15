$(function () {
    var idSitio = -1;
    var nombreSitio = "";
    $(".chosen-select").chosen();
    $("#BARRIOS").on("change", function () {
        var valor = $(this).val();
        var datos = valor.split("-");
        
        $("#BARR_CODIGO").val(datos[0]);
        $("#BARR_LUGE_CODIGO").val(datos[1]);

    });
    $("#LUGE_CODIGO").on("change", function () {
        //console.log($(this).value());
        seleccionado = $("#LUGE_CODIGO").val(); 
        var entityData = {
            "id": seleccionado
        };

        $.ajax({
            type: "POST",
            url: urlActionBuscaBarrios,
            content: "application/json; charset=utf-8",
            cache: false,
            data: entityData,
            dataType: "json",
            beforeSend: function () {
                $(".loader_decad").removeClass("hidden");
                $(".loader_back").removeClass("hidden");
            },
            success: function (d) {
                $("#BARRIOS").empty();
                $("#CODI_BARRIO").empty();
                $("#CODI_BARRIO").append("<option value='" + "" + "'>" + "" + "</option>");
                $("#BARRIOS").append("<option value='" + "" + "'>" + "" + "</option>");
                window.datos = d;
                $.each(d, function (i, item) {
                    $("#CODI_BARRIO").append("<option value='" + item.codigo + "'>" + item.descripcion + "</option>");
                    $("#BARRIOS").append("<option value='" + item.codigo + "'>" + item.descripcion + "</option>");
                })
                $(".chosen-select").chosen("destroy");
                $(".chosen-select").chosen();
                $(".loader_decad").addClass("hidden");
                $(".loader_back").addClass("hidden");
            },
            error: function (xhr, textStatus, errorThrown) {
                // TODO: Show error
            }
        });
    });
});

//function colapsarOMostrar(namePanel){
//    nombrePanel="#" + namePanel;
//    clase = "panel-collapsed";
//    debugger
//    $(nombrePanel).hasClass(clase) == false ? $(nombrePanel).addClass(clase) : $(nombrePanel).removeClass(clase);

//}

//carga sitiosImportantes para un sitio de grabación
function CargaSitiosImportantes(id, nombre) {
    nombreSitio = nombre;

    idTabla = "tblSitiosImportantes";
    $(".loader_decad").removeClass("hidden");
    $(".loader_back").removeClass("hidden");
    $.get(urlActionBuscaSitios + "/?idConsecutivo=" + id, function (data) {
        if (data[0] != null) {
            $('#sitiosImportantes').html(data);
        }
    }).done(function () {
        AplicarPluginTabla(idTabla);
    });
    $(".loader_decad").addClass("hidden");
    $(".loader_back").addClass("hidden");
}

function ValorSitioGrabacion(idConsecutivo) {
    var valorSitioGrabacion = (urlActionBuscaParqueaderos + "?idConsecutivo=" + idConsecutivo);
    return valorSitioGrabacion;
}

//carga parqueaderos para un sitio de grabación
function CargaParqueaderos(idConsecutivo, nombreSitio) {
    $("#botonParqueaderos").attr("onclick", "CargaParqueaderos(" + idConsecutivo + ")");

    $("#titleParqueaderos").html(nombreSitio);
    if ($.fn.DataTable.isDataTable("#tblParqueaderos")) {
        //  console.log("destruye");
        $("#tblParqueaderos").DataTable().destroy();
        $("#tblParqueaderos tbody").empty();
    }
    //carga databale por ajax y serverside procesing
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false },
                { "data": "Boton", "orderable": false },
                { "data": "CODIGO" },
                { "data": "DESCRIPCION" },
                { "data": "NOMBRECIUDAD", "orderable": false },
                { "data": "NOMBREBARRIO", "orderable": false },
                { "data": "DIRECCION", "orderable": false },
                { "data": "TELEFONO", "orderable": false },
                { "data": "HORARIO", "orderable": false },
                { "data": "NOMBRESITIOGRABACION", "orderable": false }

    ];

    AplicarPluginTablaAjax("tblParqueaderos", ValorSitioGrabacion(idConsecutivo), parametrosAjax);
}