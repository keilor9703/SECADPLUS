var parametrosAjaxEnvioMensajes = [
    { "data": "Dummy", "orderable": false },
    { "data": "Boton", "orderable": false },
    { "data": "DESP_SITIO_GRABA" },
    { "data": "CONSECUTIVO" },
    { "data": "FECHA" },
    { "data": "CADUSUA_INICIA" },
    { "data": "ESTADO" },
];


$(document).ready(function () {
    AplicarPluginTablaAjax("tablaCentro", urlMesajesEnviados, parametrosAjaxEnvioMensajes);
});