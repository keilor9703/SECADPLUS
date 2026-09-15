var parametrosAjaxCanal = [
       { "data": "Dummy", "orderable": false, "width": "0.5%" },
       { "data": "Boton", "orderable": false },
       { "data": "DESCRIPCION" },
       { "data": "DISPONIBLE" },
       { "data": "CADFUERZ_ID", "visible": false },
       { "data": "CODIGO", "visible": false },

];

var parametrosAjaxPedidos = [
        { "data": "Dummy", "orderable": false, "width": "0.5%" },
        { "data": "NUME_LLAMADA" },
        { "data": "HORA" },
        { "data": "NUME_TELEFONO", },
        { "data": "ESTADO_LLAMADA" },
        { "data": "MPIO" },
        { "data": "DIRE_CASO" },
        { "data": "BARRIO" },
        { "data": "CODIGO1" },
        { "data": "PRIORIDAD" },
        { "data": "COMENTARIO", "visible": false },
        { "data": "RECEPTOR", "visible": false },
        { "data": "ENVIAR", "visible": false },
        { "data": "ESTADO", "visible": false },
        { "data": "HORA_CASO", "visible": false },
        { "data": "MINUTOS_RESPUESTA", "visible": false },

];

$(document).ready(function () {
    var parametrosAjax = [
            { "data": "Dummy", "orderable": false, "width": "0.5%" },
            { "data": "DESCRIPCION" },
            { "data": "ABREVIATURA" },
            { "data": "ESCUDO", "orderable": false },
            { "data": "ID", "visible": false },

    ];

    AplicarPluginTablaEntidades("_tblEntidades", urlTablaEntidad, parametrosAjax);
});

function CargarPrimerafilaCanales(tablaId) {
    LimpiarCampos();

    $("#_tblCanales").DataTable().destroy();
    $("#_tblCanales tbody").empty();
    $("#_tblPedidos").DataTable().destroy();
    $("#_tblPedidos tbody").empty();

    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(0).data();

    var idFuerza = datosTabla["ID"];
    

    AplicarPluginTablaCanales("_tblCanales", (urlTablaCanales + "?strFuerza=" + idFuerza), parametrosAjaxCanal);
}

function CargarDatosCanales(tablaId, object) {
    LimpiarCampos();

    $("#_tblCanales").DataTable().destroy();
    $("#_tblCanales tbody").empty();
    $("#_tblPedidos").DataTable().destroy();
    $("#_tblPedidos tbody").empty();

    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(object).data();

    var idFuerza = datosTabla["ID"];

    AplicarPluginTablaCanales("_tblCanales", (urlTablaCanales + "?strFuerza=" + idFuerza), parametrosAjaxCanal);
}

function CargarPrimerafilaPedidos(tablaId) {
    LimpiarCampos();
    $("#_tblPedidos").DataTable().destroy();
    $("#_tblPedidos tbody").empty();

    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(0).data();

    var canalFuerza = datosTabla["CADFUERZ_ID"];
    var canalCodigo = datosTabla["CODIGO"];

    // console.log("CADFUERZ_ID" + canalFuerza);

    AplicarPluginTablaPedidos("_tblPedidos", (urlTablaPedidos + "?strCanalFuerza=" + canalFuerza + "&strCanalCodigo=" + canalCodigo), parametrosAjaxPedidos);
}

function CargarDatosPedido(tablaId, object) {
    LimpiarCampos();
    $("#_tblPedidos").DataTable().destroy();
    $("#_tblPedidos tbody").empty();

    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }
    datosTabla = tabla.row(object).data();

    var canalFuerza = datosTabla["CADFUERZ_ID"];
    var canalCodigo = datosTabla["CODIGO"];

    AplicarPluginTablaPedidos("_tblPedidos", (urlTablaPedidos + "?strCanalFuerza=" + canalFuerza + "&strCanalCodigo=" + canalCodigo), parametrosAjaxPedidos);
}

function CargarPrimerafilaComentarios(tablaId) {
    LimpiarCampos();
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(0).data();
    $('#COMENTARIO').val(datosTabla["COMENTARIO"]);
    $('#RECEPTOR').val(datosTabla["RECEPTOR"]);
}

function CargarDatosComentarios(tablaId, object) {
    LimpiarCampos();
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(object).data();

    $('#COMENTARIO').val(datosTabla["COMENTARIO"]);
    $('#RECEPTOR').val(datosTabla["RECEPTOR"]);
}

function PintarCeldasTabla() {
    var tabla = $('#_tblPedidos').DataTable();
    var datosTabla = tabla.rows().data();
    var nodosTabla = tabla.rows().nodes();
    console.log("nodos" + nodosTabla.length);

    if (datosTabla.length == 0) {
        return;
    }

    try {
        $.each(datosTabla, function (i, val) {
            //console.log(nodosTabla[i]);

            //# PintarColor ESTADO LLAMADAS
            if (datosTabla[i]["ESTADO"] == 'S')
                $('td', nodosTabla[i]).eq(4).addClass('bg-danger');//Rojo
            else if (datosTabla[i]["ESTADO"] == 'P')
                $('td', nodosTabla[i]).eq(4).addClass('secadBgW');//AMARILLO
            else if (datosTabla[i]["ESTADO"] == 'E')
                $('td', nodosTabla[i]).eq(4).addClass('bg-success');//AMARILLO

            //pintar color prioridad
            if (datosTabla[i]["PRIORIDAD"] == 'RUTINA')
                $('td', nodosTabla[i]).eq(9).addClass('secadBgW');//AMARILLO
            else if (datosTabla[i]["PRIORIDAD"] == 'URGENTE')
                $('td', nodosTabla[i]).eq(9).addClass('bg-primary');//AZUL
            else if (datosTabla[i]["PRIORIDAD"] == 'INMEDIATO')
                $('td', nodosTabla[i]).eq(9).addClass('bg-success');//VERDE
            else if (datosTabla[i]["PRIORIDAD"] == 'FLASH')
                $('td', nodosTabla[i]).eq(9).addClass('bg-danger');//ROJO

            var minutos = datosTabla[i]["MINUTOS_RESPUESTA"];
            //  console.log(minutos);
            if (minutos >= 5) {
                $('td', nodosTabla[i]).eq(2).addClass('bg-danger');//ROJO
            } else if (minutos > 3 && minutos < 5) {
                $('td', nodosTabla[i]).eq(2).addClass('bg-primary');//AZUL
            } else if (minutos > 2 && minutos <= 3) {
                $('td', nodosTabla[i]).eq(2).addClass('bg-secadBgW');//AMARILLO
            } else if (minutos <= 2) {
                $('td', nodosTabla[i]).eq(2).addClass('bg-success');//AMARILLO
            }
        });
    } catch (e) {
        console.log("ErrorEn colores")
    }
}

function LimpiarCampos() {
    $('#COMENTARIO').val('');
    $('#RECEPTOR').val('');
}