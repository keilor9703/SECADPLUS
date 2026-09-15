var cronometro;

$(document).ready(function () {
    if (cronometro != null) {
        clearInterval(cronometro);
    }

    cronometro = setInterval(RefrescarConexiones, _tiempoRefresco);
    $("#iTimer").removeClass('icon-watch2 icon_size-md').addClass('icon-alarm-cancel icon_size-md');
    $("#btnDetener").removeClass('btn bg-teal btn-xs').addClass('btn bg-danger btn-xs');

    $('#listCondicion').append("<option selected='selected' select value='" + "" + "'>" + "Todos" + "</option>");
    $('#listCondicion').chosen().trigger("chosen:updated");
});

function CargarDatos(tablaId, object) {
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    datosTabla = tabla.row(object).data();

    if (totalTabla.length == 0) {
        return;
    }

    $('#idFuerza').val(datosTabla["CADCANA_FUERZA_ID"]);
    $('#idCanal').val(datosTabla["CADCANA_CODIGO"]);

    $.ajax({
        url: urlDatos,
        method: "POST",
        content: "application/json; charset=utf-8",
        dataType: 'json',
        data: { idfuerza: $('#idFuerza').val(), idCanal: $('#idCanal').val() },
        success: function (resultado) {
            $('#fuerza').val(resultado.fuerza);
            $('#canal').val(resultado.canal);
        }
    });
}
function RefrescarConexiones() {
    var parametrosAjax = [
            { "data": "Dummy", "orderable": false, "width": "0.5%" },
            { "data": "DESC_CONDICION" },
            { "data": "GRAD_ALFABETICO" },
            { "data": "NOMB_EMPLEADO" },
            { "data": "FECHA_INICIO" },
            { "data": "FECHA_HASTA" },
            { "data": "SESION_ID" },
            { "data": "MAQUINA" },
            { "data": "CADUSUA_USUARIO" },
            { "data": "CADCANA_FUERZA_ID", "visible": false },
            { "data": "CADCANA_CODIGO", "visible": false },

    ];

    $("#tablaConexiones").DataTable().destroy();
    $("#tablaConexiones tbody").empty();

    AplicarPluginTablaConexiones("tablaConexiones", valorFiltros(), parametrosAjax);

    limpiar();
}
function valorFiltros() {
    var filtros = (urlTabla + "?strTipoConexion=" + $("#selTipoConexion").val() + "&strCondicion=" + $("#listCondicion").val());
    return filtros;
}
function DeternerTimer() {
    if (cronometro == null) {
        $("#iTimer").removeClass('icon-watch2 icon_size-md').addClass('icon-alarm-cancel icon_size-md');
        $("#btnDetener").removeClass('btn bg-teal btn-xs').addClass('btn bg-danger btn-xs');
        $("#btnDetener").attr("data-original-title", "Clic detener timer");

        cronometro = setInterval(RefrescarConexiones, _tiempoRefresco);
    } else {
        $("#iTimer").removeClass('icon-alarm-cancel icon_size-md').addClass('icon-watch2 icon_size-md');
        $("#btnDetener").removeClass('btn bg-danger btn-xs').addClass('btn bg-teal btn-xs');
        $("#btnDetener").attr("data-original-title", "Clic iniciar timer");
        clearInterval(cronometro);
        cronometro = null;
    }
}
function CerrarSesiones() {
    $.ajax({
        url: urlCerrar,
        method: "POST",
        content: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (resultado) {
            console.log(resultado.cerradas);
            GestionarAlerta("success", "Registros Cerrados y Actualizados cantidad : " + resultado.cerradas);
        }
    });
}
function limpiar() {
    $('#idFuerza').val('');
    $('#idCanal').val('');
    $('#fuerza').val('');
    $('#canal').val('');
}
function CargarPriemraFila(tablaId) {
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    datosTabla = tabla.row(0).data();

    if (totalTabla.length == 0) {
        return;
    }

    $('#idFuerza').val(datosTabla["CADCANA_FUERZA_ID"]);
    $('#idCanal').val(datosTabla["CADCANA_CODIGO"]);

    $.ajax({
        url: urlDatos,
        method: "POST",
        content: "application/json; charset=utf-8",
        dataType: 'json',
        data: { idfuerza: $('#idFuerza').val(), idCanal: $('#idCanal').val() },
        success: function (resultado) {
            $('#fuerza').val(resultado.fuerza);
            $('#canal').val(resultado.canal);
        }
    });
}