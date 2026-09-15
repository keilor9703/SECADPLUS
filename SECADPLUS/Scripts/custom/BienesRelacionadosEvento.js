$(document).ready(function () {
    var parametrosAjax = [
            { "data": "Dummy", "orderable": false, "width": "0.5%" },
            { "data": "Boton", "orderable": false },
            { "data": "CONSECUTIVO" },
            { "data": "USUA_TRANSACCION" },
            { "data": "FECHA_TRANSACCION" },
            { "data": "PLACA" },
            { "data": "CHASIS" },
            { "data": "MOTOR" },
            { "data": "SERIE" },
            { "data": "TIPO_VEHICULO", "orderable": false },
            { "data": "CLASE_BIEN", "orderable": false },
            { "data": "COLOR", "orderable": false },
            { "data": "MARCA", "orderable": false },
            { "data": "LINEA", "orderable": false },
            { "data": "OBSERVACIONES" },

    ];

    AplicarPluginTablaAjax("_tblBienesRelacionados", urlTabla, parametrosAjax);
});