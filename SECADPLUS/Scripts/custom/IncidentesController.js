$(function () {
    CargarTablaIncidentes();
});
function CargarTablaIncidentes() {

    var urlFinal = "";
    var localParams = { strSitio: glSitio};
    ////console.log(glModo);
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tablaIncidentes")) {
        $("#tablaIncidentes").DataTable().destroy();
        $("#tablaIncidentes tbody").empty();
    }
    urlFinal = urlConsultarIncidentes.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    //////console.log(urlRadios);
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },                
                { "data": "TIPO", "orderable": true },
                { "data": "PERSONAS", "orderable": true },
                { "data": "VEHICULOS", "orderable": true },
                { "data": "SOLICITANTE", "orderable": true },
                { "data": "PATRULLA", "orderable": true },
                { "data": "MOTIVO", "orderable": true },
                { "data": "COMENTARIO", "orderable": true },
                { "data": "FECHA", "orderable": true },
                { "data": "USUARIO", "orderable": true }
    ];
    AplicarPluginTablaPedidosRecepcion("tablaIncidentes", urlFinal, parametrosAjax, localParams);

}