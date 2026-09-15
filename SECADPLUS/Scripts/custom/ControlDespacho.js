var tabla;
var totalTabla;
var datosTabla;
var cronometro;

$(document).ready(function () {
    CambiarVisibilidadMenu();
    CargaControlDespacho();
    cronometro = setInterval(CargaControlDespacho, _tiempoRefresco);
    //console.log("Comenzo");
    $("#iTimer").removeClass('icon-watch2 icon_size-md').addClass('icon-alarm-cancel icon_size-md');
    $("#btnDetener").removeClass('btn bg-teal btn-xs').addClass('btn bg-danger btn-xs');

    $('#tabla tbody').on('click', 'tr', function () {
        tabla = $('#tabla').DataTable();
        totalTabla = tabla.rows().data();

        datosTabla = tabla.row(this).data();

        if (totalTabla.length == 0) {
            return;
        }

        $('#SITIO_GRABA').val(datosTabla["SITIO_GRABA"]);
        $('#MIN_PEDIDO_PEND').val(datosTabla["MIN_PEDIDO_PEND"]);

        //if ($('#MIN_PEDIDO_PEND').val()) {
        //    $('#MIN_PEDIDO_PEND').addClass('bg-danger');

        //}else{
        //    $('#MIN_PEDIDO_PEND').removeClass('bg-danger');
        //}

        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            tabla.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });
});

function DeternerTimer() {
    if (cronometro == null) {
        $("#iTimer").removeClass('icon-watch2 icon_size-md').addClass('icon-alarm-cancel icon_size-md');
        $("#btnDetener").removeClass('btn bg-teal btn-xs').addClass('btn bg-danger btn-xs');
        cronometro = setInterval(CargaControlDespacho, _tiempoRefresco);
    } else {
        $("#iTimer").removeClass('icon-alarm-cancel icon_size-md').addClass('icon-watch2 icon_size-md');
        $("#btnDetener").removeClass('btn bg-danger btn-xs').addClass('btn bg-teal btn-xs');
        clearInterval(cronometro);
        cronometro = null;
    }
}

function CargaControlDespacho() {
    var parametrosAjax = [
            { "data": "Dummy", "orderable": false, "width": "0.1%" },
            { "data": "USUARIO", "orderable": false },
            { "data": "NOMBRE", "orderable": false },
            { "data": "LOGIN", "orderable": false },
            { "data": "EQUIPO", "orderable": false },
            { "data": "UBICACION", "orderable": false },
            { "data": "TURNO", "orderable": false },
            { "data": "CLASE_TURNO", "orderable": false },
            { "data": "GENERADOS", "orderable": false },
            { "data": "PENDIENTES", "orderable": false },
            { "data": "CNT_MEDIOS", "orderable": false },
            { "data": "FUERZA", "orderable": false },
            { "data": "CANAL", "orderable": false },
    ];

    $("#tabla").DataTable().destroy();
    $("#tabla tbody").empty();

    AplicarPluginTablaEncuestar("tabla", urlTabla, parametrosAjax);
}

function ConsultarPrimer() {
    var tabla = $('#tabla').DataTable();
    var fila = tabla.rows(0).data();

    if (fila[0] === undefined) {
        return;
    }

    $('#SITIO_GRABA').val(fila[0].SITIO_GRABA);
    $('#MIN_PEDIDO_PEND').val(fila[0].MIN_PEDIDO_PEND);
}

function recorrerTabla() {
    var tabla = $('#tabla').DataTable();
    var datosTabla = tabla.rows().data();
    var nodosTabla = tabla.rows().nodes();
    var arrayTabla = [];
    var celda;

    $.each(datosTabla, function (i, val) {
        //console.log(nodosTabla[i]);

        //# PintarColor PENDIENTES
        if (datosTabla[i]["PENDIENTES"] > 0)
            $('td', nodosTabla[i]).eq(9).addClass('bg-danger');//Rojo
        else
            $('td', nodosTabla[i]).eq(9).addClass('bg-success');//Verde

        //# PintarColor GENERADOS
        if (datosTabla[i]["CUENTA"] > 0) {
            $('td', nodosTabla[i]).eq(8).addClass('bg-success');//Verde
        } else
            $('td', nodosTabla[i]).eq(8).addClass('secadBgW');//Amarillo
    });
}

function IrAPedidos() {
    //recibir valor y enviar valor de MIN_PEDIDO_PEND
    //ir al formulario pedidos
}