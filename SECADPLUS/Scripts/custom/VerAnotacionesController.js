$(function () {
    CargarTablaAnotaciones();
});

function CargarTablaAnotaciones() {

    if ($.fn.DataTable.isDataTable("#tablaAnotacionesGenerales")) {
        $("#tablaAnotacionesGenerales").DataTable().destroy();
        $("#tablaAnotacionesGenerales tbody").empty();
    }
   
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
    { "data": "ANOTACION" },
    { "data": "DSP_FUERZA" },
    { "data": "DSP_CANAL" },
    { "data": "CADUSUA_GRABA" },
    { "data": "FECHA_GRABACION" },
    { "data": "CONSECUTIVO" }
    ];
    AplicarPluginTablaAjax("tablaAnotacionesGenerales", urlTablaAnotaciones + "?idSitio=" + glSitio + "&numeLlamada=" + globalLlamada, parametrosAjax);

}