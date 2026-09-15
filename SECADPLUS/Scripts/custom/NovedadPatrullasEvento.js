$(document).ready(function () {
    var parametrosAjax = [
            { "data": "Dummy", "orderable": false, "width": "0.5%" },
            { "data": "Boton", "orderable": false },
            { "data": "MEDIO", "orderable": false },
            { "data": "DESC_TIPO_NOVEDAD" },
            { "data": "HORA_INICIA" },
            { "data": "HORA_TERMINA" },
            { "data": "OBSERVACION" },

    ];

    AplicarPluginTablaAjax("tablaNovedadesPatrulla", urlTabla, parametrosAjax);
});