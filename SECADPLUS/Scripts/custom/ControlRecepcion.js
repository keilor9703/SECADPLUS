var tabla;
var totalTabla;
var datosTabla;
var cronometro;
var tablaReload;
$(document).ready(function () {
    CambiarVisibilidadMenu();
    CargaControlDespacho();

    //cronometro = setInterval(CargaControlDespacho, _tiempoRefresco);
    //cronometro = setInterval(CargaControlDespacho, 5000);
    //console.log("Comenzo");
    //$("#iTimer").removeClass('icon-watch2 icon_size-md').addClass('icon-alarm-cancel icon_size-md');
    //$("#btnDetener").removeClass('btn bg-teal btn-xs').addClass('btn bg-danger btn-xs');

    $('#tabla tbody').on('click', 'tr', function () {
        tabla = $('#tabla').DataTable();
        totalTabla = tabla.rows().data();

        datosTabla = tabla.row(this).data();

        if (totalTabla.length == 0) {
            return;
        }

        $('#SITIO_GRABA').val(datosTabla["SITIO_GRABA"]);

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
        //cronometro = setInterval(CargaControlDespacho, _tiempoRefresco);
        cronometro = setInterval(function () {
            console.log("recarga tabla");
            tablaReload.ajax.reload();
        }, _tiempoRefresco);
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
            { "data": "LOGIN", "orderable": false },
            { "data": "EQUIPO", "orderable": false },
            { "data": "UBICACION", "orderable": false },
            { "data": "RECIBIDAS", "orderable": false },
            { "data": "EFECTIVAS", "orderable": false },
            { "data": "ULTIMA_FECHA", "orderable": false },
            { "data": "PEDIDO_SIN_ENVIO", "orderable": false },
            { "data": "NOMBRE", "orderable": false },
            { "data": "TURNO", "orderable": false },
    ];
    
    
   // $("#tabla").DataTable().destroy();
   // $("#tabla tbody").empty();
    
    tablaReload = AplicarPluginTablaControlRecepcion("tabla", urlTabla, parametrosAjax);
    
}

function ConsultarPrimer() {
    var tabla = $('#tabla').DataTable();
    var fila = tabla.rows(0).data();

    if (fila[0] === undefined) {
        return;
    }

    $('#SITIO_GRABA').val(fila[0].SITIO_GRABA);
}

function recorrerTabla() {
    var tabla = $('#tabla').DataTable();
    var datosTabla = tabla.rows().data();
    var nodosTabla = tabla.rows().nodes();
    var arrayTabla = [];
    var celda;

    $.each(datosTabla, function (i, val) {
        //console.log(nodosTabla[i]);

        //# PintarColor RECIBIDAS
        if (datosTabla[i]["RECIBIDAS"] > 0)
            $('td', nodosTabla[i]).eq(5).addClass('bg-success');//Verde
        else
            $('td', nodosTabla[i]).eq(5).addClass('bg-danger');//Rojo

        //# PintarColor EFECTIVAS
        if (datosTabla[i]["EFECTIVAS"] > 0)
            $('td', nodosTabla[i]).eq(6).addClass('bg-success');//Verde
        else
            $('td', nodosTabla[i]).eq(6).addClass('secadBgW');//Amarillo

        //# PintarColor PEDIDO_SIN_ENVIO
        if (datosTabla[i]["PEDIDO_SIN_ENVIO"])
            $('td', nodosTabla[i]).eq(8).addClass('bg-danger');//Rojo
        else
            $('td', nodosTabla[i]).eq(8).addClass('bg-success');//Verde

        //# PintarColor ULTIMA_FECHA
        if (datosTabla[i]["TIEMPO_SEG"] < 300)
            $('td', nodosTabla[i]).eq(7).addClass('bg-success');//Verde
        else
            $('td', nodosTabla[i]).eq(7).addClass('secadBgW');//Amarillo

        if (datosTabla[i]["TIEMPO_SEG"] > 300 && datosTabla[i]["TIEMPO_SEG"] < 600)
            $('td', nodosTabla[i]).eq(7).addClass('secadBgW');//Amarillo

        if (datosTabla[i]["TIEMPO_SEG"] > 601 && datosTabla[i]["TIEMPO_SEG"] < 1200)
            $('td', nodosTabla[i]).eq(7).addClass('bg-primary');//Azul
        else
            $('td', nodosTabla[i]).eq(7).addClass('bg-danger');//Rojo

        if (datosTabla[i]["TIEMPO_SEG"] > 1200)
            $('td', nodosTabla[i]).eq(7).addClass('bg-danger');//Rojo

        if (datosTabla[i]["TIEMPO_SEG"] == '')
            $('td', nodosTabla[i]).eq(7).removeClass();
    });
}