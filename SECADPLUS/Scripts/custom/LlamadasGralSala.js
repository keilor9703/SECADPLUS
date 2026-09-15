///////////////////VARIABLES DE CONTROL PARA EL TIMER//////////////////
var timerLlamadasGral = 0;
var glTiempo = 15000;//Intervalo en milisegundo para la ejecución del timer
$(function () {
    GestionarAlerta("success", "Esta ventana se actualiza de manera automática cada 15s");
    CargarTablaGralSala();
    IniciarTimer();
});
///////////////TIMER PARA OBTENER NUEVA LLAMADA/////////////////////
function IniciarTimer() {
    //console.log("--------------Timer recepcion iniciado");
    //timerRecepcionId = setTimeout(ConsultarLlamada(), glTiempo);//Tiempo en milisegundos
    timerLlamadasGral = setInterval(function () { CargarTablaGralSala(); }, glTiempo);//Tiempo en milisegundos
    // var id = setInterval(function () { //console.log("intervalo"); },1000);
}
function CargarTablaGralSala() {
    if ($.fn.DataTable.isDataTable("#tablaGral")) {
        $("#tablaGral").DataTable().destroy();
        $("#tablaGral tbody").empty();
    }
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false },
        { "data": "NUME_LLAMADA" },
        { "data": "NUME_TELEFONO" },
        { "data": "HORA" },
        { "data": "CODIGO1" },
        { "data": "MPIO" },
        { "data": "DIRE_CASO" },
        { "data": "BARRIO" },
        { "data": "COMENTARIO" },
        { "data": "TIPO_PEDIDO" },
        { "data": "CALI_PEDIDO" },
        { "data": "ESTADO_LLAMADA" },
        { "data": "TIPO_GENERO" },
        { "data": "RECEPTOR" }
    ];

    AplicarPluginTablaAjax("tablaGral", urlTablaGralSala + "?idSitio=" + globalSitio, parametrosAjax);
}