var opcion = $("input[name=opcionesRbEstado]:checked").val();
var tabla;
var totalTabla;
var datosTabla;
var cronometro;
var globalLista;
var globalEvento;
var globalTurno;
var globalLlamada;
var globalEventoEstado;
var intentos = false;
var glCasoText = "", glCasoId = "", glPreguntasOpcion = 0;

$(document).ready(function () {
   
   ////eventos para recepción SALUD
    //asigna evento de doble click casos
    $("#txtCasoId").dblclick(function () {
        $("#ModalCasosReal").modal("toggle");
        glCasoText = "txtCaso";
        glCasoId = "txtCasoId";
        glPreguntasOpcion = 1;
        CargarCasos();
    });
    $("#txtCasoId").blur(function (e) {

        console.log("press");
        glCasoText = "txtCaso";
        glCasoId = "txtCasoId";
        glPreguntasOpcion = 1;
        CompletarCaso($(this).val());

    });

    $("#CADCODI_FINAL").blur(function (e) {
        console.log("press");

        var filtro = $(this).val();

        if (filtro != "") {
            CompletarCasoEventoFinal(filtro);

        } else {
            $("#DSP_CODIFINAL").val('');
        }
    });

    $("#btnGAnotaPatrulla").click(function () {
        GuardarAnotacionPatrulla();
    });
    ////FIN SALUD
    CambiarVisibilidadMenu();
    //GestionarAlerta("success", "Revise en cadreferenciassecad el valor para TIEMPO_REFRESCO_EVENTOS.. Se establecen por defecto 15 segundos..");
    console.log(_tiempoRefresco);
    CargaLlamadasRecibidas(_idFuerza, _idCanal, opcion);
    CargaLlamadasRecepcion();

    cronometro = setInterval(RefrescarPantallaTiempo, _tiempoRefresco);
    //console.log("Comenzo");
    //$("#iTimer").removeClass('icon-watch2 icon_size-md').addClass('icon-alarm-cancel icon_size-md');
    //$("#btnDetener").removeClass('btn bg-teal btn-xs').addClass('btn bg-danger btn-xs');

    $("#iTimer").removeClass('icon-alarm-cancel icon_size-md').addClass('icon-watch2 icon_size-md');
    $("#btnDetener").removeClass('btn bg-danger btn-xs').addClass('btn bg-teal btn-xs');


    $("input[name=opcionesRbEstado]").click(function () {
        //alert("es: " + $(this).val());

        $('#textComentario').val('');
        document.getElementById("textReceptor").innerHTML = "Receptor";

        opcion = $(this).val();
        CargaLlamadasRecibidas(_idFuerza, _idCanal, opcion);
    });

    $('#tablaRecibidas tbody').on('click', 'tr', function () {
        tabla = $('#tablaRecibidas').DataTable();
        totalTabla = tabla.rows().data();

        datosTabla = tabla.row(this).data();

        if (totalTabla.length == 0) {
            return;
        }

        $('#textComentario').val(datosTabla["COMENTARIO"]);
        document.getElementById("textReceptor").innerHTML = datosTabla["RECEPTOR"];

        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            tabla.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });

    $("#tabPendientes").click(function () {
        console.log("pendiente");
        intentos = false;
        //$("#alertaJs").empty();
        DeternerTimer();
        $('#ModalBtnAnotacionesGenerales').addClass ('hidden');
    });

    $("#eventos").click(function () {
        intentos = true;
        $("#alertaJs").empty();
        DeternerTimer();
        CargaListadoEventos(false);
    });

    //Funcion para refrescar el Timer
    function ActivarTimer() {

        var evento = ""
        if ($('#EVENTO_NUMEEVENTO_CONTROL').val() != "") {
            evento = $('#EVENTO_NUMEEVENTO_CONTROL').val();
        } else {
            evento = globalEvento;
        }

        CargaEventoMedios(evento);
        CargaEventoMediosControl();
        GrabarIdUbicacion();

    }

    var tid = setInterval(ActivarTimer, 20000);


    $("#cerrados").click(function () {
        $("#alertaJs").empty();
        $("#eventos").trigger("click");
        var win = window.open(urlCerrados, '_blank');
        win.focus();
    });

    $('#tablaMediosDisponibles tbody').on('click', 'tr', function () {
        var tabla = $('#tablaMediosDisponibles').DataTable();
        var totalTabla = tabla.rows().data();

        if (totalTabla.length == 0) {
            return;
        }

        //console.log(this);

        var datosTabla = tabla.row(this).data();

        if (!datosTabla) {
            return;
        }

        $('#CADUNITUR_UNDEFUERZA').val(datosTabla["CADUNITUR_UNDEFUERZA"]);
        $('#CADUNITUR_UNDECODIGO').val(datosTabla["CADUNITUR_UNDECODIGO"]);
        $('#CADPATRU_CODIGO').val(datosTabla["CADPATRU_CODIGO"]);
        $('#CONSECUTIVO').val(datosTabla["CONSECUTIVO"]);
        $('#ID_RADIOUBICANEW').focus();
        CargarTablaAnotacionesPatrulla($('#CADUNITUR_UNDEFUERZA').val(), $('#CADUNITUR_UNDECODIGO').val(), $('#CADPATRU_CODIGO').val());
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            tabla.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
        $("#tablaMediosDisponibles tr").each(function () {
            $(this).removeClass("selected");
        });
        $(this).toggleClass("selected");
    });

    //Eventos Medios
    $("#HORA_RUTA").dblclick(function () {
        if (!$("#HORA_RUTA").val()) {
            TraerHora("HORA_RUTA");
            document.getElementById("HORA_RUTA").readOnly = true;
        }
    });

    $("#HORA_LLEGA").dblclick(function () {
        if (!$("#HORA_LLEGA").val()) {
            TraerHora("HORA_LLEGA");
            document.getElementById("HORA_LLEGA").readOnly = true;
        }
    });

    $("#HORA_TERMINA").dblclick(function () {
        if (!$("#HORA_TERMINA").val()) {
            TraerHora("HORA_TERMINA");
            document.getElementById("HORA_TERMINA").readOnly = true;
        }
    });

    //Eventos Medios Control
    $("#HORA_RUTA_CONTROL").dblclick(function () {
        if (!$("#HORA_RUTA_CONTROL").val()) {
            TraerHora("HORA_RUTA_CONTROL");
            document.getElementById("HORA_RUTA_CONTROL").readOnly = true;
        }
    });

    $("#HORA_LLEGA_CONTROL").dblclick(function () {
        if (!$("#HORA_LLEGA_CONTROL").val()) {
            TraerHora("HORA_LLEGA_CONTROL");
            document.getElementById("HORA_LLEGA_CONTROL").readOnly = true;
        }
    });
});

function AsignarHorasTabla(idCampo, consecutivo) {
    // console.log(idCampo + " otro " + consecutivo );
    //alert("1 AsignarHorasTabla");
    var campo = "";
    if (idCampo == 1) {
        campo = "HORA_RUTA";
    } else if (idCampo == 2) {
        campo = "HORA_LLEGA";
    } else if (idCampo == 3) {
        campo = "HORA_TERMINA";
    }

    var evento = ""
    if ($('#EVENTO_NUMEEVENTO_CONTROL').val() != "") {
        evento = $('#EVENTO_NUMEEVENTO_CONTROL').val();
    } else {
        evento = globalEvento;
    }

    var arrayParametros = new Array();

    arrayParametros.push(consecutivo);
    arrayParametros.push(evento);
    arrayParametros.push(campo);

    //console.log(arrayParametros);

    $.ajax({
        type: 'POST',
        url: urlProcesarHorasEventoMedios,
        dataType: 'json',
        data: { parametros: arrayParametros },
        success: function (resultado) {
            if (resultado.result) {
                CargaEventoMedios(evento);
                CargaEventoMediosControl();

            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

function TraerHora(idCampo) {
    var tiene = idCampo.indexOf("CONTROL");
    var medioConsecutivo;
    var evento;

    if (tiene > -1) {
        medioConsecutivo = $('#CADMEDISP_CONSECUTIVO_CONTROL').val();
        evento = $('#EVENTO_NUMEEVENTO_CONTROL').val();
    }
    else {
        medioConsecutivo = $('#CADMEDISP_CONSECUTIVO').val();
        evento = globalEvento;
    }

    var arrayParametros = new Array();

    arrayParametros.push(medioConsecutivo);
    arrayParametros.push(evento);
    arrayParametros.push(idCampo);

    //console.log(arrayParametros);

    $.ajax({
        type: 'POST',
        url: urlProcesarHorasEventoMedios,
        dataType: 'json',
        data: { parametros: arrayParametros },
        success: function (resultado) {
            if (resultado.result) {
                if (tiene > -1) {
                    CargaEventoMediosControl();
                } else {
                    CargaEventoMedios(globalEvento);
                }
                $("#" + idCampo).val(resultado.datos);
            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

function AsignarHorasTablaPorEvento(idCampo, consecutivo, evento) {
    //console.log(idCampo + " otro " + consecutivo );
    var campo = "";
    if (idCampo == 1) {
        campo = "HORA_RUTA";
    } else if (idCampo == 2) {
        campo = "HORA_LLEGA";
    } else if (idCampo == 3) {
        campo = "HORA_TERMINA";
    }

    var arrayParametros = new Array();

    arrayParametros.push(consecutivo);
    arrayParametros.push(evento);
    arrayParametros.push(campo);

    //console.log(arrayParametros);

    $.ajax({
        type: 'POST',
        url: urlProcesarHorasEventoMedios,
        dataType: 'json',
        data: { parametros: arrayParametros },
        success: function (resultado) {
            if (resultado.result) {
                CargaEventoMediosControl();
                CargaEventoMedios(globalEvento);
            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

//-------------------------------
// Primer Tab Llamadas Pendientes

function CargaLlamadasRecibidas(idFuerza, idCanal, opcion) {
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.1%" },
        { "data": "NUME_LLAMADA", "orderable": false },
        { "data": "BOTON", "orderable": false },
        { "data": "HORA", "orderable": false },
        { "data": "NUME_TELEFONO", "orderable": false },
        { "data": "D_ESTADO_CANAL", "orderable": false },
        { "data": "DSP_RECURSO", "orderable": false },
        { "data": "CNT_EVENTOS", "orderable": false },
        { "data": "CNT_RECURSOS", "orderable": false },
        { "data": "CODIGO1", "orderable": false },
        { "data": "MPIO", "orderable": false },
        { "data": "DIRE_CASO", "orderable": false },
        { "data": "BARRIO", "orderable": false },
        { "data": "PRIORIDAD", "orderable": false },
        { "data": "MINUTOS_DESPACHO", "visible": false },

    ];

    $("#tablaRecibidas").DataTable().destroy();
    $("#tablaRecibidas tbody").empty();

    //Cambiar valores
    AplicarPluginTablaDespachos("tablaRecibidas", urlTablaRecibidas + "?idFuerza=" + idFuerza + "&idCanal=" + idCanal + "&opcion=" + opcion, parametrosAjax);
}

function CargaLlamadasRecepcion() {
    var parametrosAjax = [
        //{ "data": "Dummy", "orderable": false },
        { "data": "NUME_LLAMADA", "orderable": false },
        { "data": "HORA_CASO", "orderable": false },
        { "data": "NUME_TELEFONO", "orderable": false },
        { "data": "MPIO", "orderable": false },
        { "data": "DIRE_CASO", "orderable": false },
        { "data": "CODIGO1", "orderable": false },
        { "data": "BARRIO", "orderable": false },
        { "data": "COMUNA", "orderable": false },
    ];

    $("#tablaRecepcion").DataTable().destroy();
    $("#tablaRecepcion tbody").empty();

    AplicarPluginTablaAjax("tablaRecepcion", urlTablaRecepcion, parametrosAjax);
}

function RefrescarPanatalla() {
    CargaLlamadasRecibidas(_idFuerza, _idCanal, opcion);
    CargaLlamadasRecepcion();
}

function RefrescarPantallaTiempo() {
    if (!controlRecepcion) {
        $("#iTimer").removeClass('icon-alarm-cancel icon_size-md').addClass('icon-watch2 icon_size-md');
        $("#btnDetener").removeClass('btn bg-danger btn-xs').addClass('btn bg-teal btn-xs');
        $("#btnDetener").attr("data-original-title", "Clic detener timer");

        clearInterval(cronometro);
        cronometro = null;
    }
    CargaLlamadasRecibidas(_idFuerza, _idCanal, opcion);
    CargaLlamadasRecepcion();
}

function DeternerTimer() {
    console.log("timer");

    if (cronometro == null) {

        $("#iTimer").removeClass('icon-alarm-cancel icon_size-md').addClass('icon-watch2 icon_size-md');
        $("#btnDetener").removeClass('btn bg-danger btn-xs').addClass('btn bg-teal btn-xs');
        $("#btnDetener").attr("data-original-title", "Clic detener timer");
        cronometro = setInterval(RefrescarPantallaTiempo, _tiempoRefresco);
    } else {

        $("#iTimer").removeClass('icon-watch2 icon_size-md').addClass('icon-alarm-cancel icon_size-md');
        $("#btnDetener").removeClass('btn bg-teal btn-xs').addClass('btn bg-danger btn-xs');
        $("#btnDetener").attr("data-original-title", "Clic iniciar timer");
        clearInterval(cronometro);
        cronometro = null;
    }
}

function recorrerTablaRecepcion() {
    var tabla = $('#tablaRecibidas').DataTable();
    var datosTabla = tabla.rows().data();
    var arrayTabla = [];
    var celda;

    $.each(datosTabla, function (i, val) {
        //console.log(datosTabla[i]["ESTADO_CANAL"]);

        //# PintarColorNumeLlamada
        //Rojo
        if (datosTabla[i]["CNT_EVENTOS"] == 0) {
            celda = tabla.cells(":contains('" + datosTabla[i]["NUME_LLAMADA"] + "')").nodes();
            $(celda).addClass('bg-danger');
        }

        //Amarillo
        if (datosTabla[i]["CNT_EVENTOS"] == 1 && datosTabla[i]["CNT_RECURSOS"] == 0) {
            celda = tabla.cells(":contains('" + datosTabla[i]["NUME_LLAMADA"] + "')").nodes();
            $(celda).addClass('secadBgW');
        }

        //Verde
        if (datosTabla[i]["CNT_EVENTOS"] == 1 && datosTabla[i]["CNT_RECURSOS"] > 0) {
            celda = tabla.cells(":contains('" + datosTabla[i]["NUME_LLAMADA"] + "')").nodes();
            $(celda).addClass('bg-success');
        }

        //# Fin PintarColorNumeLlamada

        //# PintarColorHora
        //Rojo

        var mm = parseFloat(datosTabla[i]["MINUTOS_DESPACHO"].replace(',', '.'));
        // console.log(mm);

        if (mm >= 5) {
            celda = tabla.cells(":contains('" + datosTabla[i]["HORA"] + "')").nodes();
            $(celda).addClass('bg-danger');
            //console.log("rojo=" + datosTabla[i]["TIEMPO_DESPACHO"]);
        }

        //Azul
        if (mm > 3 && mm < 5) {
            celda = tabla.cells(":contains('" + datosTabla[i]["HORA"] + "')").nodes();
            $(celda).addClass('bg-primary');
            //console.log("azul=" + datosTabla[i]["MINUTOS_DESPACHO"]);
        }

        //Amarillo
        if (mm > 2 && mm <= 3) {
            celda = tabla.cells(":contains('" + datosTabla[i]["HORA"] + "')").nodes();
            $(celda).addClass('secadBgW');
            //console.log("amarillo=" + datosTabla[i]["MINUTOS_DESPACHO"]);
        }

        //Verde
        if (mm <= 2) {
            celda = tabla.cells(":contains('" + datosTabla[i]["HORA"] + "')").nodes();
            $(celda).addClass('bg-success');
            // console.log("verde=" + datosTabla[i]["MINUTOS_DESPACHO"]);
        }

        //# Fin PintarColorHora

        //# PintarColorEstadoCanal
        //Amarillo
        if (datosTabla[i]["ESTADO_CANAL"] == "S") {// SIN ENVIAR
            celda = tabla.cells(":contains('" + datosTabla[i]["D_ESTADO_CANAL"] + "')").nodes();
            $(celda).addClass('secadBgW');
        }
        //Rojo
        if (datosTabla[i]["ESTADO_CANAL"] == "P") { // PENDIENTE DE DESPACHO
            celda = tabla.cells(":contains('" + datosTabla[i]["D_ESTADO_CANAL"] + "')").nodes();
            $(celda).addClass('bg-danger');
        }
        //Verde
        if (datosTabla[i]["ESTADO_CANAL"] == "E") { // EN PROCESO
            celda = tabla.cells(":contains('" + datosTabla[i]["D_ESTADO_CANAL"] + "')").nodes();
            $(celda).addClass('bg-success');
        }
        //Azul
        if (datosTabla[i]["ESTADO_CANAL"] == "T") { // PENDIENTE DE DESPACHO
            celda = tabla.cells(":contains('" + datosTabla[i]["D_ESTADO_CANAL"] + "')").nodes();
            $(celda).addClass('bg-primary');
        }
        //Claro
        if (datosTabla[i]["ESTADO_CANAL"] == "R") {  // PARA REVISION
            celda = tabla.cells(":contains('" + datosTabla[i]["D_ESTADO_CANAL"] + "')").nodes();
            $(celda).addClass('bg-info');
        }
        //Normal
        if (datosTabla[i]["ESTADO_CANAL"] == "I") { // EN INFORMACION
            //Sin Color
        }
        //# Fin PintarColorEstadoCanal

        //Amarillo
        if (datosTabla[i]["PRIORIDAD"] == "RUTINA") { // RUTINA
            celda = tabla.cells(":contains('" + datosTabla[i]["PRIORIDAD"] + "')").nodes();
            $(celda).addClass('secadBgW');
        }
        //Azul
        if (datosTabla[i]["PRIORIDAD"] == "URGENTE") { // URGENTE
            celda = tabla.cells(":contains('" + datosTabla[i]["PRIORIDAD"] + "')").nodes();
            $(celda).addClass('bg-primary');
        }
        //Verde
        if (datosTabla[i]["PRIORIDAD"] == "INMEDIATO") { // INMEDIATO
            celda = tabla.cells(":contains('" + datosTabla[i]["PRIORIDAD"] + "')").nodes();
            $(celda).addClass('bg-success');
        }
        //Rojo
        if (datosTabla[i]["PRIORIDAD"] == "FLASH") { // FLASH
            celda = tabla.cells(":contains('" + datosTabla[i]["PRIORIDAD"] + "')").nodes();
            $(celda).addClass('bg-danger');
        }
    });
}

function CargarEventos(NUME_LLAMADA, ESTADO_CANAL, CODI_PEDIDO) {
    //alert("1 CargarEventos");

    if (ESTADO_CANAL == "S") {
        GestionarAlerta("warning", "El evento se encuentra Aun en recepción..");
        return;
    }

    var arrayParametros = new Array();

    arrayParametros.push(globalSitio);
    arrayParametros.push(globalIdentificacion);
    arrayParametros.push(NUME_LLAMADA);
    arrayParametros.push(globalFuerza);
    arrayParametros.push(globalCanal);
    arrayParametros.push(CODI_PEDIDO);
    arrayParametros.push(globalClaseTurno);
    arrayParametros.push(globalUsuario);

    //console.log(arrayParametros);

    $.ajax({
        type: 'POST',
        url: urlProcesarEvento,
        dataType: 'json',
        data: { parametros: arrayParametros },
        success: function (resultado) {
            //console.log("Entro");
            if (resultado.result) {
                globalNumeEvento = resultado.datos;
                $("#eventos").trigger("click");
                // DeternerTimer();
                //ActivarLoader("panelEventos");
                //InactivarLoader("panelEventos");
            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

//-------------------------------
// Fin Primer Tab Llamadas Pendientes

//-------------------------------
// Segundo Tab Eventos

function CargaListadoEventos(bandera) {
    if (bandera) {
        globalNumeEvento = 0;
    }

    var parametrosAjax = [
            { "data": "Dummy", "orderable": false, "width": "0.1%" },
            { "data": "BOTON", "orderable": false, "width": "0.1%" },
            { "data": "NUME_EVENTO", "orderable": false },
            { "data": "ESTADO_EVENTO", "orderable": false },
            { "data": "FECH_CREA", "orderable": false },
            { "data": "CADUSUA_EVEN_GENERA", "orderable": false },
    ];

    $("#tablaEventos").DataTable().destroy();
    $("#tablaEventos tbody").empty();

    AplicarPluginTablaEventos("tablaEventos", urlTablaEventos + "?idSitio=" + globalSitio + "&numeEvento=" + globalNumeEvento + "&idFuerza=" + globalFuerza + "&idCanal=" + globalCanal, parametrosAjax);

   


}

function ConsultarTurno() {
    //alert("1");
    if (intentos) {
        var tabla = $('#tablaEventos').DataTable();
        var fila = tabla.rows(0).data();

        if (fila[0] === undefined) {
            return;
        }

        var _numeLlamada = fila[0].CADPEDCA_NUMELLAMADA;
        var _numeEvento = fila[0].NUME_EVENTO;
        var _turno = fila[0].CADTURN_TURNO;

        globalTurno = _turno;
        globalEvento = _numeEvento;
        globalLlamada = _numeLlamada;
        globalEventoEstado = fila[0].ESTADO_EVENTO;

        $('#CADCLCI_CODIGO').val(fila[0].CADCLCI_CODIGO);

        $("#CADCLCI_CODIGO").trigger("chosen:updated");

        $.ajax({
            type: 'POST',
            url: urlTurno,
            dataType: 'json',
            data: { idSitio: globalSitio, idTurno: _turno },
            success: function (resultado) {
                if (resultado.result) {
                    document.getElementById("textTurno").innerHTML = resultado.datos;
                } else {
                    GestionarAlerta("warning", resultado.datos);
                }
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });

        CargarCamposPedido(_numeEvento, _numeLlamada, globalEventoEstado);
        //CargaComentariosAdicionales(_numeLlamada);
        intentos = false;
    }
}
//Evento para cargar Tablas
function CargarCamposPedido(numeEvento, numeroLlamada, estadoEvento) {
    //alert("1 CargarCamposPedido");
    globalEvento = numeEvento;
    globalEventoEstado = estadoEvento;

    $.ajax({
        type: 'POST',
        url: urlPedidoEvento,
        dataType: 'json',
        //data: { idSitio: globalSitio, numeLlamada: numeroLlamada }, cambio rmr 08-10-2020 08:45 am
        data: { idSitio: globalSitio, numeLlamada: numeroLlamada, numeEvento: globalEvento },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (resultado) {
            if (resultado.result) {
                //globalLista = resultado.lista;
                LlenarCamposPedido(resultado.lista);
                LlenarCamposDatosAdicionales(resultado.lista2);
                CargaListadoMediosDisponibles(globalTurno);
                CargaEventoMedios(numeEvento);
                CargaEventoMediosControl();
            } else {
                GestionarAlerta("warning", resultado.datos);
            }

            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

function LlenarCamposPedido(lista) {
    $('#textSitiGraba').val(lista[0].SITIO_GRABA);
    document.getElementById("textHoraCaso").innerHTML = lista[0].HORA_CASO;
    document.getElementById("textLlamada").innerHTML = lista[0].NUME_LLAMADA;
    document.getElementById("textUsuario").innerHTML = lista[0].CADUSUA_USUARIO;
    document.getElementById("textAbonado").innerHTML = lista[0].NUME_TELEFONO;
    document.getElementById("textDispositivo").innerHTML = lista[0].DISP_TELEFONICO;
    document.getElementById("textPropNombre").innerHTML = lista[0].PROP_TELEFONO;
    document.getElementById("textPropDireccion").innerHTML = lista[0].DIRE_TELEFONO;
    document.getElementById("textCiudadTelefono").innerHTML = lista[0].LUGE_TELEFONO;
    document.getElementById("textBarrioTelefono").innerHTML = lista[0].BARR_TELEFONO;
    document.getElementById("textLlamante").innerHTML = lista[0].NOMB_LLAMANTE;
    document.getElementById("textDireccionLlamante").innerHTML = lista[0].DIRE_LLAMANTE;
    $('#textComentarios').val(lista[0].COMENTARIO);
    document.getElementById("textCaso1").innerHTML = lista[0].CODI_PEDIDO;
    document.getElementById("textDescCaso1").innerHTML = lista[0].DESC_CODI_PEDIDO;
    document.getElementById("textCaso2").innerHTML = lista[0].CODI_PEDIDO2;
    document.getElementById("textDescCaso2").innerHTML = lista[0].DESC_CODI_PEDIDO2;
    document.getElementById("textDireccion").innerHTML = lista[0].DIRE_CASO;
    document.getElementById("textBarrio").innerHTML = lista[0].DSP_BARRIO;
    document.getElementById("textCiudad").innerHTML = lista[0].DSP_CIUDAD;

    if (globalEventoEstado == "Cerrado") {
        /*
         * CODI_PEDIDO_REAL = strCodigoReal;
            DESC_CODI_PEDIDO_REAL = strDescCodigoReal;
            ---CAMBIO RMR 05-10-2020
            $("#CADCODI_FINAL").val(lista[0].CODI_PEDIDO);
            $("#DSP_CODIFINAL").val(lista[0].DESC_CODI_PEDIDO);
         */
        $("#CADCODI_FINAL").val(lista[0].CODI_PEDIDO_REAL);
        $("#DSP_CODIFINAL").val(lista[0].DESC_CODI_PEDIDO_REAL);
    }
    else {
        $("#CADCODI_FINAL").val('');
        $("#DSP_CODIFINAL").val('');
    }

    $('#CADPEDI_SITIOGRABA').val(globalSitio);
    $('#CADPEDI_NUMELLAMADA').val(lista[0].NUME_LLAMADA);
    $('#CODI_PEDIDO').val(lista[0].CODI_PEDIDO);

    $('#LUGE_CODIGO').val(lista[0].LUGE_CODIGO);

    CargaComentariosAdicionales(lista[0].NUME_LLAMADA);
}

function LlenarCamposDatosAdicionales(lista2) {
    if ((lista2.length) > 0)
    {
        $('#Latitud').val(lista2[0].X_COORD);
        $('#Longitud').val(lista2[0].Y_COORD);
        //document.getElementById("Latitud").innerHTML = lista2[0].X_COORD;
        //document.getElementById("Longitud").innerHTML = lista2[0].Y_COORD;
        $('#ModalidadId').val(lista2[0].MODALIDADCASO);
        $('#ConductaId').val(lista2[0].CONDUCTAESPECIAL);
        $('#TipoArmId').val(lista2[0].ARMAMEDIO);
        $('#TipoMovilizaId').val(lista2[0].TIPOMOVILIZAGRESOR);
    }
}

//Ingresar Registro Comentario Adicional
function IngresarComentarioAdicional() {
    var pregunta = $('#RTA_PREGUNTA').val();
    var anotacion = $('#ANOTACION').val();

    if (pregunta && anotacion) {
        var datosComentarios = $("#ComentariosAdicionales").serialize();

        $.ajax({
            type: 'POST',
            url: urlIngresarComentariosAdicionales,
            dataType: 'json',
            data: datosComentarios,
            success: function (resultado) {
                if (resultado.result) {
                    CargaComentariosAdicionales($('#CADPEDI_NUMELLAMADA').val());
                    //GestionarAlerta("success", resultado.datos.MENSAJE);
                } else {
                    GestionarAlerta("warning", resultado.datos);
                }
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });

        $('#RTA_PREGUNTA').val('');
        $('#ANOTACION').val('');
    }
}

function CargaComentariosAdicionales(numeLlamada) {
    $.ajax({
        type: 'POST',
        url: urlTablaComentariosAdicionales,
        dataType: 'html',
        data: { idSitio: globalSitio, numeLlamada: numeLlamada }
    }).done(function (Comentarios) {
        if ($.fn.DataTable.isDataTable("#tablaComentariosAdicionales")) {
            $("#tablaComentariosAdicionales").DataTable().destroy();
        }
        $("#tablaComentariosAdicionales tbody").empty();
        $("#tablaComentariosAdicionales tbody").html(Comentarios);
        AplicarPluginTabla("tablaComentariosAdicionales");
    });
};

function CargarPreguntasAdicionales() {
    var numeLlamada = $('#CADPEDI_NUMELLAMADA').val();
    var codiCaso = $('#CODI_PEDIDO').val();

    $.ajax({
        type: 'POST',
        url: urlPreguntasAdicionales,
        dataType: 'json',
        data: { idSitio: globalSitio, numeLlamada: numeLlamada, codiCaso: codiCaso, vigente: 'A', usuario: globalUsuario },
        success: function (resultado) {
            if (resultado.result) {
                CargaComentariosAdicionales(numeLlamada);
            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

function CargarBarriosComunas() {
    $('#DIRE_CASO').val('');
    $("#CODI_BARRIO").val('');
    $("#BARRIO").val('');
    $("#LUGE_BARRIO").val('');

    var lugeCodigo = $('#LUGE_CODIGO').val();

    if (lugeCodigo != 0) {
        var parametrosAjax = [
            { "data": "BOTON", "orderable": false },
            { "data": "CODIGO", "orderable": false },
            { "data": "DESCRIPCION", "orderable": false },
            { "data": "CIUDAD_BARRIO", "orderable": false },
        ];

        $("#tablaBarrios").DataTable().destroy();
        $("#tablaBarrios tbody").empty();

        AplicarPluginTablaAjax("tablaBarrios", urlBarriosComunas + "?id=" + lugeCodigo, parametrosAjax);
        $("#ModalActualizarDireccionBarrio").modal();
    }
}

function cargarValorBarrio(codigo, descripcion, ciudad) {
    $("#CODI_BARRIO").val(codigo);
    $("#BARRIO").val(descripcion);
    $("#LUGE_BARRIO").val(ciudad);
    //$("#ModalActualizarDireccionBarrio").modal("toggle");
}

function ActualizarDirBarrio() {
    //alert("1 ActualizarDirBarrio");
    var numeLlamada = $('#CADPEDI_NUMELLAMADA').val();
    var direccion = $('#DIRE_CASO').val();
    var barrio = $("#textBarrio").text();
    var lugeBarrio = $("#LUGE_BARRIO").val();
    var codiBarrio = $("#CODI_BARRIO").val();

    var arrayParametros = new Array();

    arrayParametros.push(globalSitio);
    arrayParametros.push(numeLlamada);
    arrayParametros.push(globalUsuario);
    arrayParametros.push(direccion);
    arrayParametros.push(barrio);
    arrayParametros.push(lugeBarrio);
    arrayParametros.push(codiBarrio);

    //console.log(arrayParametros);

    $.ajax({
        type: 'POST',
        url: urlProcesarBarriosComunas,
        dataType: 'json',
        data: { parametros: arrayParametros },
        success: function (resultado) {
            if (resultado.result) {
                CargarCamposPedido(globalEvento, numeLlamada, globalEventoEstado);
                $("#ModalActualizarDireccionBarrio").modal("toggle");
            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

//-------------------------------
// Fin Segundo Tab Eventos

//-------------------------------
// Tercer Tab Eventos
function CargaListadoMediosDisponibles(turno) {
    var parametrosAjax = [
        { "data": "BOTON", "orderable": false, "width": "0.1%" },
        { "data": "DESC_PATRULLA", "orderable": false, "width": "0.1%" },
        { "data": "ETA", "orderable": false, "width": "0.1%" },
        { "data": "CUENTA", "orderable": false, "width": "0.1%" },
        //rmr 08-06-2020
        {
            "data": null, "orderable": false, "width": "0.1%", "render": function (data, type, row)
            {
                //data.CADUNITUR_UNDECODIGO
                //data.CADPATRU_CODIGO
                //turno
                return '<span style="cursor:pointer" onclick="ConsultaCuadrantePorIdRadio(' + data.CADUNITUR_UNDECODIGO + ',' + data.CADPATRU_CODIGO + ',' + turno +')">' + data.ID_RADIO + '</span>'
            }
        },
        ////rmr 08-06-2020
        //{ "data": "ID_RADIO", "orderable": false, "width": "0.1%" }, comentado para implementar clic
        { "data": "DESC_UNIDAD", "orderable": false, "width": "0.1%" }
        
    ];

    $("#tablaMediosDisponibles").DataTable().destroy();
    $("#tablaMediosDisponibles tbody").empty();

    //AplicarPluginTablaAjax("tablaMediosDisponibles", urlTablaMediosDisponibles + "?idSitio=" + globalSitio + "&numeTurno=" + turno + "&idFuerza=" + globalFuerza + "&idCanal=" + globalCanal, parametrosAjax);
    if (isNaN(turno)) turno = 0;
    AplicarPluginTablaMediosDisponibles("tablaMediosDisponibles", urlTablaMediosDisponibles + "?idSitio=" + globalSitio + "&numeTurno=" + turno + "&idFuerza=" + globalFuerza + "&idCanal=" + globalCanal, parametrosAjax);
}

function ConsultaCuadrantePorIdRadio(CADUNITUR_UNDECODIGO, CADPATRU_CODIGO, turno) {

    $.ajax({
        type: 'POST',
        url: urlCuadrantePorIdRadio,
        dataType: 'json',
        data: { _CADUNITUR_UNDECODIGO: CADUNITUR_UNDECODIGO, _CADPATRU_CODIGO: CADPATRU_CODIGO, _turno: turno },
        success: function (resultado) {
            $("#CUADRANTE").val(resultado.datos.Cuadrante);
            $("#CUENTA").val(resultado.datos.Cuenta);
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });

}

function GrabarIdUbicacion() {
    var idUbicacion = $('#ID_RADIOUBICANEW').val();
    var fuerza = $('#CADUNITUR_UNDEFUERZA').val();
    var unidad = $('#CADUNITUR_UNDECODIGO').val();
    var patrulla = $('#CADPATRU_CODIGO').val();
    console.log(idUbicacion + "," + fuerza + "," + unidad + "," + patrulla);
    if (idUbicacion = ! "" && fuerza != "" && unidad != "" && patrulla != "") {
        console.log("EntroVali");
        var arrayParametros = new Array();

        arrayParametros.push(idUbicacion);
        arrayParametros.push(fuerza);
        arrayParametros.push(unidad);
        arrayParametros.push(patrulla);

        console.log(arrayParametros);

        $.ajax({
            type: 'POST',
            url: urlProcesarIdUbicacion,
            dataType: 'json',
            data: { parametros: arrayParametros },
            success: function (resultado) {
                if (resultado.result) {
                    CargaListadoMediosDisponibles(globalTurno);
                } else {
                    GestionarAlerta("warning", resultado.datos);
                }
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });

        $('#ID_RADIOUBICANEW').val('');
        $('#CADUNITUR_UNDEFUERZA').val('');
        $('#CADUNITUR_UNDECODIGO').val('');
        $('#CADPATRU_CODIGO').val('');
    }
    if (globalTurno != "") {
        CargaListadoMediosDisponibles(globalTurno);

    }



}

function recorrerTablaMediosDisponibles() {
    var tabla = $('#tablaMediosDisponibles').DataTable();
    var datosTabla = tabla.rows().data();
    var nodosTabla = tabla.rows().nodes();

    var arrayTabla = [];
    var celda;
    $.each(datosTabla, function (i, val) {
      //  console.log(datosTabla[i]["CUENTA"]);
        //# PintarColorDESC_UNIDAD
        //Verde
        if (datosTabla[i]["CUENTA"] == 0) {
            $('td', nodosTabla[i]).eq(1).addClass('bg-success');//verde

            //celda = tabla.cells(":contains('" + datosTabla[i]["DESC_UNIDAD"] + "')").nodes();
            //$(celda).addClass('bg-success');
        }

        //Amarillo
        if (datosTabla[i]["CUENTA"] == 1) {
            $('td', nodosTabla[i]).eq(1).addClass('secadBgW');//amarillo

            //celda = tabla.cells(":contains('" + datosTabla[i]["DESC_UNIDAD"] + "')").nodes();
            //$(celda).addClass('secadBgW');
        }

        //Rojo
        if (datosTabla[i]["CUENTA"] > 1) {
            $('td', nodosTabla[i]).eq(1).addClass('bg-danger');//Rojo

            //celda = tabla.cells(":contains('" + datosTabla[i]["DESC_UNIDAD"] + "')").nodes();
            //$(celda).addClass('bg-danger');
        }
        //# Fin PintarColorDESC_UNIDAD
    });
}

function AdicionarMedioCaso(consecutivo) {
    var arrayParametros = new Array();

    arrayParametros.push(globalSitio);
    arrayParametros.push(globalEvento);
    arrayParametros.push(globalSitio);
    arrayParametros.push(consecutivo);//$('#CONSECUTIVO').val()
    arrayParametros.push(globalCondicion);
    arrayParametros.push(globalFuerza);
    arrayParametros.push(globalCanal);
    arrayParametros.push(globalUsuario);

    //console.log(arrayParametros);

    $.ajax({
        type: 'POST',
        url: urlProcesarMedioCaso,
        dataType: 'json',
        data: { parametros: arrayParametros },
        success: function (resultado) {
            if (resultado.result) {
                CargaEventoMedios(globalEvento); //Cargar CADEVENTOMEDIOS
                CargaListadoMediosDisponibles(globalTurno);

                //Cargar V_EVENTO_MEDIOS_CONTROL
            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

//-------------------------------
// Fin Tercer Tab Eventos

function CargaEventoMedios(numeEvento) {
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.1%" },
        { "data": "BOTON", "orderable": false },
        { "data": "DSP_DESC_UNIDAD", "orderable": false },
        { "data": "DSP_MEDIODISPO", "orderable": false },
        { "data": "TIPO_RESPONSABLE", "orderable": false },
        { "data": "HORA_ASIGNA", "orderable": false },
        { "data": "HORA_RUTA", "orderable": false },
        { "data": "HORA_LLEGA", "orderable": false },
        { "data": "HORA_BCONTROL", "orderable": false, "visible": false },
        { "data": "HORA_REFERENCIA", "orderable": false, "visible": false },
        { "data": "HORA_REGULADO", "orderable": false, "visible": false },
        { "data": "HORA_IPS", "orderable": false, "visible": false },
        { "data": "HORA_RECIBIDO", "orderable": false, "visible": false },
        { "data": "HORA_TERMINA", "orderable": false },
        { "data": "NOVEDAD", "orderable": false },       
        { "data": "CADMEDISP_CONSECUTIVO", "orderable": false, "visible": false },
        {
            "data": null, className: "celdaCenter", "width": "4%", "render": function (data, type, row) {
                return '<div class="dropdown">' +
                    '<button class="btn btn-success" onclick="TraerCordenada(' + "'" + data.CADMEDISP_CONSECUTIVO + "'" + '); return false;" type="button" id="btnCoordenadas" ><span class="icon-earth"></span></button>'
                '</div>'
            }
        },
    ];

    if (glSalud === 1) {
        parametrosAjax = [
            { "data": "Dummy", "orderable": false, "width": "0.1%" },
            { "data": "BOTON", "orderable": false },
            { "data": "DSP_DESC_UNIDAD", "orderable": false },
            { "data": "DSP_MEDIODISPO", "orderable": false },
            { "data": "TIPO_RESPONSABLE", "orderable": false },
            { "data": "HORA_ASIGNA", "orderable": false },
            { "data": "HORA_RUTA", "orderable": false },
            { "data": "HORA_LLEGA", "orderable": false },
            { "data": "HORA_BCONTROL", "orderable": false },
            { "data": "HORA_REFERENCIA", "orderable": false },
            { "data": "HORA_REGULADO", "orderable": false },
            { "data": "HORA_IPS", "orderable": false },
            { "data": "HORA_RECIBIDO", "orderable": false },
            { "data": "HORA_TERMINA", "orderable": false },
            { "data": "NOVEDAD", "orderable": false },          
            { "data": "CADMEDISP_CONSECUTIVO", "orderable": false, "visible": false },
            {
                "data": null, className: "celdaCenter", "width": "4%", "render": function (data, type, row) {
                    return '<div class="dropdown">' +
                        '<button class="btn btn-success" onclick="TraerCordenada(' + "'" + data.CADMEDISP_CONSECUTIVO + "'" + '); return false;" type="button" id="btnCoordenadas" ><span class="icon-earth"></span></button>'
                    '</div>'
                }
            },
        ];
    }
    $("#tablaEventoMedios").DataTable().destroy();
    $("#tablaEventoMedios tbody").empty();
    if (isNaN(numeEvento)) numeEvento = 0;
    AplicarTablaPendientesAjax("tablaEventoMedios", urlTablaEventoMedios + "?idSitio=" + globalSitio + "&numeEvento=" + numeEvento, parametrosAjax);
};


//-------------------------------
// Cuarto Tab Eventos

function EventoMedioSeleccionado() {
    //var fila = $(objeto).parent().parent().parent().parent().parent().parent().parent().find('tr');

    var tabla = $('#tablaEventoMedios').DataTable();
    var fila = tabla.row('.selected');
    var datosTabla = tabla.row(fila).data();

    //var datosTabla = tabla.row(fila[0]).data();

    $("#CADMEDISP_CONSECUTIVO").val(datosTabla["CADMEDISP_CONSECUTIVO"]);
    $('#HORA_RUTA').val(datosTabla["HORA_RUTA"]);
    $('#HORA_LLEGA').val(datosTabla["HORA_LLEGA"]);
}

function ModalEventoMedio() {
    //var fila = $(objeto).parent().parent().parent().parent().parent().parent().parent().find('tr');

    var tabla = $('#tablaEventoMedios').DataTable();
    var fila = tabla.row('.selected');
    var datosTabla = tabla.row(fila).data();

    //var datosTabla = tabla.row(fila[0]).data();

    //var fila = $(objeto).parent().parent().parent().parent().parent().parent().find('td').each(function (indice) {
    //    console.log(indice);
    //    console.log($(this).html());
    //});

    $("#CADMEDISP_CONSECUTIVO").val(datosTabla["CADMEDISP_CONSECUTIVO"]);
    $("#DSP_DESC_UNIDAD").val(datosTabla["DSP_DESC_UNIDAD"]);
    $("#DSP_MEDIODISPO").val(datosTabla["DSP_MEDIODISPO"]);

    $('#TIPO_RESPONSABLE').val(datosTabla["TIPO_RESPONSABLE"]);

    //$('#TIPO_RESPONSABLE').val(datosTabla["CRESPONSABLE"]);
    //$("#TIPO_RESPONSABLE").trigger("chosen:updated");

    $('#HORA_ASIGNA').val(datosTabla["HORA_ASIGNA"]);
    var likeRuta = datosTabla["HORA_RUTA"].match(/button/);
    // console.log(datosTabla["HORA_RUTA"]);
    // console.log(likeRuta);

    if (likeRuta == null) {
        $('#HORA_RUTA').val(datosTabla["HORA_RUTA"]);
    } else {
        $('#HORA_RUTA').val('');
    }

    var likeLlegada = datosTabla["HORA_LLEGA"].match(/button/);
    //  console.log(likeLlegada);
    if (likeLlegada == null) {
        $('#HORA_LLEGA').val(datosTabla["HORA_LLEGA"]);
    } else {
        $('#HORA_LLEGA').val('');
    }

    var likeTermina = datosTabla["HORA_TERMINA"].match(/button/);
    //  console.log(likeTermina);

    if (likeTermina == null) {
        $('#HORA_TERMINA').val(datosTabla["HORA_TERMINA"]);
    } else {
        $('#HORA_TERMINA').val('');
    }

    if ($("#HORA_RUTA").val())
        document.getElementById("HORA_RUTA").readOnly = true;
    else
        document.getElementById("HORA_RUTA").readOnly = false;

    if ($("#HORA_LLEGA").val())
        document.getElementById("HORA_LLEGA").readOnly = true;
    else
        document.getElementById("HORA_LLEGA").readOnly = false;

    if ($("#HORA_TERMINA").val())
        document.getElementById("HORA_TERMINA").readOnly = true;
    else
        document.getElementById("HORA_TERMINA").readOnly = false;

    $('#NOVEDAD').val(datosTabla["CNOVEDAD"]);
    $("#NOVEDAD").trigger("chosen:updated");

    $("#ModalActualizarEventosMedios").modal();
}




function ActualizarEventoMedio() {
    var arrayParametros = new Array();

    arrayParametros.push($('#CADMEDISP_CONSECUTIVO').val());
    arrayParametros.push(globalEvento);
    arrayParametros.push($('#NOVEDAD').val());

    $.ajax({
        type: 'POST',
        url: urlActualizarNovedad,
        dataType: 'json',
        data: { parametros: arrayParametros },
        success: function (resultado) {
            if (resultado.result) {
                CargaEventoMedios(globalEvento);
                $("#ModalActualizarEventosMedios").modal("toggle");
            }
            else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

//function EliminarEventoMedio(consecutivo) {
//    var arrayParametros = new Array();

//    arrayParametros.push(globalSitio);
//    arrayParametros.push(consecutivo);
//    arrayParametros.push(globalSitio);
//    arrayParametros.push(globalEvento);

//    $.ajax({
//        type: 'POST',
//        url: urlValidarEventosMediosRuta,
//        dataType: 'json',
//        data: { parametros: arrayParametros },
//        success: function (resultado) {
//            if (resultado.result) {
//                EliminarDatoEventoMedio(arrayParametros);
//            } else {
//                GestionarAlerta("warning", resultado.datos);
//                //EliminarDatoEventoMedio(arrayParametros);
//            }
//        },
//        error: function (ex) {
//            GestionarAlerta("warning", ex);
//        }
//    });
//}

function EliminarEventoMedio(consecutivo, horaLlega, horaTermina) {
    //console.log(consecutivo + " - " + horaLlega + " - " + horaTermina);
    if (globalEventoEstado == "Cerrado") {
        GestionarAlerta("warning", "El caso ya esta cerrado y no debe ser modificado");
        return;
    }

    if (horaLlega != "VACIO" || horaTermina != "VACIO") {
        GestionarAlerta("warning", "La patrulla no puede ser borrada..");
        return;
    }

    var arrayParametros = new Array();

    arrayParametros.push(globalSitio);
    arrayParametros.push(consecutivo);
    arrayParametros.push(globalSitio);
    arrayParametros.push(globalEvento);

    swal({
        title: "¿Esta seguro de eliminar el registro?",
        type: "warning",
        html: true,
        showCancelButton: true,
        confirmButtonColor: "#EF5350",
        cancelButtonColor: "#EF5385",
        confirmButtonText: "Eliminar",
        cancelButtonText: "Cancelar",
        closeOnConfirm: true,
        closeOnCancel: true
    },

function (isConfirm) {
    if (isConfirm) {
        $.ajax({
            type: "POST",
            url: urlEliminarEventosMedios,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: { parametros: arrayParametros },
            error: function (msn) {
                GestionarAlerta("warning", msn);
            }
        }).done(function (resultado) {
            if (resultado.result) {
                CargaEventoMedios(globalEvento);
                GestionarAlerta("success", resultado.datos);
            } else {
                GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
            }
        });
    }
}
);
}

function EliminarDatoEventoMedio(arrayParametros) {
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        type: "warning",
        html: true,
        showCancelButton: true,
        confirmButtonColor: "#EF5350",
        cancelButtonColor: "#EF5385",
        confirmButtonText: "Eliminar",
        cancelButtonText: "Cancelar",
        closeOnConfirm: true,
        closeOnCancel: true
    },
    function (isConfirm) {
        if (isConfirm) {
            $.ajax({
                type: "POST",
                url: urlEliminarEventosMedios,
                content: "application/json; charset=utf-8",
                dataType: "json",
                data: { parametros: arrayParametros },
                error: function (msn) {
                    GestionarAlerta("warning", msn);
                }
            }).done(function (resultado) {
                if (resultado.result) {
                    CargaEventoMedios(globalEvento);
                    GestionarAlerta("success", resultado.datos);
                } else {
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}

//-------------------------------
// Fin Cuarto Tab Eventos

//-------------------------------
// Quinto Tab Eventos

function cargarAsociarLlamadas() {
    var arrayParametros = new Array();

    var idSitio = $('#textSitiGraba').val();
    var horaCaso = $('#textHoraCaso').text();
    var numeLlamada = $('#textLlamada').text();

    arrayParametros.push(idSitio);
    arrayParametros.push(globalSitio);
    arrayParametros.push(horaCaso);
    arrayParametros.push(numeLlamada);

    //console.log(arrayParametros);

    $.ajax({
        type: 'POST',
        //url: 'VerAsociarLlamadas',
        url: urlTablaAsociarLlamadas,
        dataType: 'html',
        data: { parametros: arrayParametros },
    }).done(function (asociar) {
        if ($.fn.DataTable.isDataTable("#tablaAsociarLlamadas")) {
            $("#tablaAsociarLlamadas").DataTable().destroy();
        }
        $("#tablaAsociarLlamadas tbody").empty();
        $("#tablaAsociarLlamadas tbody").html(asociar);
        AplicarPluginTablaModal("tablaAsociarLlamadas");
    });

    $("#ModalAsociarLlamadas").modal();
}

function cargarLlamada(numeLlamada) {
    $("#VL_CADPEDI_NUMELLAMADA").val(numeLlamada);
    $("#ModalAsociarLlamadas").modal("toggle");
}

function cargarCasos(opcion) {
    console.log("Vamos" + opcion);
    var parametrosAjax = [
    { "data": "Dummy", "orderable": false, "width": "0.1%" },
    { "data": "Boton", "orderable": false },
    { "data": "CODIGO" },
    { "data": "DESCRIPCION" }
    ];

    $("#tablaCasosDH").DataTable().destroy();
    $("#tablaCasosDH tbody").empty();
    AplicarPluginTablaAjax("tablaCasosDH", urlTablaCasos + "?opcion=" + opcion, parametrosAjax);
    $("#ModalCasos").modal();
}

function cargarValorCaso(id, descripcion, opcion) {
    $("#CADCODI_FINAL").val(id);
    $("#DSP_CODIFINAL").val(descripcion);

    $("#ModalCasos").modal("toggle");
}

//------------Botones

function AbrirTurnosEvento() {
    var tabla = $('#tablaEventoMedios').DataTable();
    totalTabla = tabla.rows().data();
    if (glEventoMedio ===0) {
        GestionarAlerta("warning", "Debe seleccionar una patrulla asignada.");
        return false;
    }
    if (totalTabla.length > 0) {
        var win = window.open(urlTurnosEvento + "?intSitioGrabacion=" + globalSitio + "&strNombre=" + globalDesSitioGraba + "&intEntidad=" + globalFuerza + "&strEntidad=" + globalDesFuerza + "&strConsulta=CONSULTA" + "&intTurno=" + globalTurno + "&intSitioGrabacionTurno=" + globalSitio+"&lgEvento="+globalNumeEvento+"&lgEventoMedio="+glEventoMedio, '_blank');
        win.focus();
    } else {
        GestionarAlerta("warning", "Debe tener activo un evento con una patrulla asignada..Revise..");
    }
}

function AbrirAnotacionesEvento() {
    var win = window.open(urlAnotacionesEvento + "?strSitioGraba=" + globalSitio + "&strNumeroEvento=" + globalEvento, '_blank');
    win.focus();
}

function AbrirVisorGeografico() {
    var win = window.open(urlVisorGeografico + "?strNumeLlamada=" + globalLlamada, '_blank');
    win.focus();
}

function AbriReposteSeguimientoEvento() {
    var win = window.open(urlReposteSeguimientoEvento + "?strNumeroLLamada=" + globalLlamada + "&strFechaInicial=" + $('#textHoraCaso').text(), '_blank');
    win.focus();
}

function AbrirMujerEvento() {
    var win = window.open(urlMujerEvento + "?strSitioGrabaEvento=" + globalSitio + "&strEvento=" + globalEvento, '_blank');
    win.focus();
}

function AbriSocialEvento() {
    var win = window.open(urlSocial + "?strSitioGrabaEvento=" + globalSitio + "&strEvento=" + globalEvento, '_blank');
    win.focus();
}

function AbrirResibirLlamadaEvento() {
    var win = window.open(urlResibirLlamadaEvento, '_blank');
    win.focus();
}

function AbrirDatosLlamadasEvento() {
    var numeLlamada = $("#textLlamada").text();
    var win = window.open(urlDatosLlamadaEvento + "?intSitio=" + globalSitio + "&lgLlamada=" + numeLlamada, '_blank');
    win.focus();
}

function AbrirLocalizaciones() {
    var win = window.open(urlLocalizaciones, '_blank');
    win.focus();
}

function AbrirRevistas() {
    var win = window.open(urlRevistas, '_blank');
    win.focus();
}

function AbrirResumenConductas() {
    //colocar codigo
}

function AbrirNovedad() {
    var win = window.open(urlNovedad, '_blank');
    win.focus();
}

function AbrirRutas() {
    var consecutivo = $('#CADMEDISP_CONSECUTIVO').val();
    var horaRuta = $('#HORA_RUTA').val();
    var horaLlega = $('#HORA_LLEGA').val();

    if (!horaRuta || !horaLlega) {
        GestionarAlerta("warning", "No tiene horas de inicio de desplazamiento o de llegada al sitio.. verifique....");
        return;
    }    
    if (consecutivo) {
        var win = window.open(urlRutas + "?idFuerza=" + globalFuerza + "&idSitio=" + globalSitio + "&numeLlamada=" + globalLlamada + "&idConsecutivo=" + consecutivo + "&numeEvento=" + globalEvento, '_blank');
        win.focus();
    } else {
        GestionarAlerta("warning", "No tiene seleccionado un medio de atención para iniciar rutas adicionales..");
        return;
    }
}

function ProcesarCasoSeguimiento() {
    var arrayParametros = new Array();

    arrayParametros.push(globalSitio);
    arrayParametros.push(globalEvento);
    arrayParametros.push("T");

    $.ajax({
        type: 'POST',
        url: urlActualizarSeguimiento,
        dataType: 'json',
        data: { parametros: arrayParametros },
        success: function (resultado) {
            if (resultado.result) {
                globalNumeEvento = globalEvento;
                CargaListadoEventos(false);
                GestionarAlerta("success", resultado.datos);
            }
            else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

function ProcesarCerrarEvento() {
    //colocar codigo de proceso (Procedimiento Almacenado)
    var codigoFinal = $("#CADCODI_FINAL").val();
    var cadclci_codigo = $("#CADCLCI_CODIGO").val();

    //if (!codigoFinal) {
    //    GestionarAlerta("warning", "Digite el codigo de cierre del evento ...");
    //    return;
    //}

    var arrayParametros = new Array();

    
    //if (!(V_Latitud) || !(V_Longitud)) {
    //    GestionarAlerta("warning", "Seleccione boton traer Coordenadas ...");
    //    return;
    //}
    var V_Latitud = $("#Latitud").val();
    var V_Longitud = $("#Longitud").val();
    var V_Modalidad = $("#ModalidadId").val();
    if (!(V_Modalidad)) {
        GestionarAlerta("warning", "Selecione Modalidad Caso ...");
        return;
    }
    var V_Conducta = $("#ConductaId").val();
    if (!(V_Conducta)) {
        GestionarAlerta("warning", "Selecione Conducta Especial ...");
        return;
    }
    var V_TipoArma = $("#TipoArmId").val();
    if (!(V_TipoArma)) {
        GestionarAlerta("warning", "Selecione Arma Medio ...");
        return;
    }
    var V_TipoMoviliza = $("#TipoMovilizaId").val();
    if (!(V_TipoMoviliza)) {
        GestionarAlerta("warning", "Selecione Tipo Moviliza Agresor ...");
        return;
    }

    arrayParametros.push(codigoFinal);
    arrayParametros.push(globalSitio);
    arrayParametros.push(globalEvento);
    arrayParametros.push(V_Latitud);
    arrayParametros.push(V_Longitud);
    arrayParametros.push(globalLlamada);
    arrayParametros.push(V_Modalidad);
    arrayParametros.push(V_Conducta);
    arrayParametros.push(V_TipoArma);
    arrayParametros.push(V_TipoMoviliza);
    arrayParametros.push(cadclci_codigo);

    $.ajax({
        type: 'POST',
        url: urlProcesarCerrarEvento,
        dataType: 'json',
        data: { parametros: arrayParametros },
        success: function (resultado) {
            if (resultado.result) {
                //  $("#eventos").trigger("click");
                console.log("Cerrar");
                var msm = "LLamada Cerrada.";

                location.href = (urlRecargaCerrar + "?strCerrado=" + msm);

                GestionarAlerta("success", resultado.datos);
            }
            else {
                CargaListadoEventos(false);
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });

    //$("#DSP_CODIFINAL").val('');
}

function cargarAnotaciones() {
    console.log(urlVerAnotaciones);
    var numeLlamada = $("#textLlamada").text();
    var win = window.open(urlVerAnotaciones + "?strSitio=" + globalSitio + "&strLlamada=" + numeLlamada, '_blank');
    //win.focus();
}


//function AbrirRapidSOS() {
//    var urlRapidSOS = "https://rapidsosintl.com/quick-access?t=VG-gpkmzxErTV2ppv7h5nNciYXMc5WqAcmeMOdzLqcT7Lyjcsx4SDJfl39Auh_Cw&action=new-incident";
//    window.open(urlRapidSOS, "_blank");
//}

function AbrirRapidSOS() {
    // Consumimos la variable global definida en la vista
    if (urlRapidSOS_Config) {
        window.open(urlRapidSOS_Config, "_blank");
    } else {
        console.error("La URL de RapidSOS no está configurada.");
    }
}

//function AbrirFotos() {
//    var win = window.open(urlFotos + "?strSitioGraba=" + globalSitio + "&strNumeroEvento=" + globalEvento, '_blank');
//    win.focus();
//}

function AbrirVehiculos() {
    var win = window.open(urlVehiculos + "?intNumeroLLamada=" + globalLlamada + "&intSitioGraba=" + globalSitio, '_blank');
    win.focus();
}

function AbrirAnotacionesGenerales() {
    var win = window.open(urlAnotacionesGenerales, '_blank');
    win.focus();
}

function ProcesarVerifXY() {
    //colocar codigo de proceso (Procedimiento Almacenado)
}

function SalvarCambios() {
    CargaEventoMediosControl();
    //Asigna el caso real
    if (glSalud === 1) {
        GuardarCasoReal();
    }
}

function AbrirMensajero() {
    var win = window.open(urlMensajero, '_blank');
    win.focus();
}

//------------Fin Botones

function CargaEventoMediosControl() {
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.1%" },
        { "data": "DESC_PATRULLA", "orderable": false },
        { "data": "Boton", "orderable": false },
        { "data": "CODI_PEDIDO", "orderable": false },
        { "data": "CODI_PEDIDO2", "orderable": false },
        { "data": "HORA_ASIGNA", "orderable": false },
        { "data": "HORA_RUTA", "orderable": false },
        { "data": "HORA_LLEGA", "orderable": false },
        { "data": "DIRE_CASO", "orderable": false }
    ];

    $("#tablaEventoMediosControl").DataTable().destroy();
    $("#tablaEventoMediosControl tbody").empty();

    AplicarPluginTablaMediosControl("tablaEventoMediosControl", urlTablaMediosControl + "?idFuerza=" + globalFuerza + "&idCanal=" + globalCanal, parametrosAjax);
}

function recorrerTablaMediosControl() {
    var tabla = $('#tablaEventoMediosControl').DataTable();
    var datosTabla = tabla.rows().data();
    var nodosTabla = tabla.rows().nodes();

    var arrayTabla = [];
    var celda;

    $.each(datosTabla, function (i, val) {
        //# PintarColor HORA_LLEGA
        var horaLlega = datosTabla[i]["HORA_LLEGA"];
        var minutos = datosTabla[i]["MINUTOS"];
        // console.log(horaLlega);
        var likeRuta = horaLlega.match(/button/);

        //  console.log(likeRuta);
        if (minutos >= _tiempoLlega && likeRuta != null) {
            //   celda = tabla.cells(":contains('" + horaLlega + "')").nodes();
            $('td', nodosTabla[i]).eq(7).addClass('bg-danger');//ROJO
            // $(celda).addClass('bg-danger');
        } else {
            //celda = tabla.cells(":contains('" + horaLlega + "')").nodes();
            //$(celda).addClass('bg-success');

            if (likeRuta != null) {
                $('td', nodosTabla[i]).eq(7).addClass('bg-success');//AMARILLO

            }

        }
        //# Fin PintarColor HORA_LLEGA
    });
}

function CargarTabEventos1(numeEvento) {
    globalEvento = numeEvento;
    CargaListadoEventos(false);
}

function CargarTabEventos2(numeEvento) {
    globalNumeEvento = numeEvento;
    intentos = true;
    CargaListadoEventos(false);
}

function ModalEventoMedioControl(objeto) {
    var fila = $(objeto).parent().parent().parent().parent().parent().parent().parent().find('tr');

    var tabla = $('#tablaEventoMediosControl').DataTable();
    var datosTabla = tabla.row(fila[0]).data();

    $("#CADMEDISP_CONSECUTIVO_CONTROL").val(datosTabla["CADMEDISP_CONSECUTIVO"]);
    $("#EVENTO_NUMEEVENTO_CONTROL").val(datosTabla["EVENTO_NUMEEVENTO"]);

    $('#HORA_ASIGNA_CONTROL').val(datosTabla["HORA_ASIGNA"]);
    $('#HORA_RUTA_CONTROL').val(datosTabla["HORA_RUTA"]);
    $('#HORA_LLEGA_CONTROL').val(datosTabla["HORA_LLEGA"]);

    if ($("#HORA_LLEGA_CONTROL").val() == "SIN HORA")
        $("#HORA_LLEGA_CONTROL").val('');

    if ($("#HORA_RUTA_CONTROL").val())
        document.getElementById("HORA_RUTA_CONTROL").readOnly = true;
    else
        document.getElementById("HORA_RUTA_CONTROL").readOnly = false;

    if ($("#HORA_LLEGA_CONTROL").val())
        document.getElementById("HORA_LLEGA_CONTROL").readOnly = true;
    else
        document.getElementById("HORA_LLEGA_CONTROL").readOnly = false;

    $("#ModalActualizarEventosMediosControl").modal();
}

//-------------------------------
// Fin Quinto Tab Eventos

//Daniel

function AbrirFotos() {
    //var llamada = $("#NUME_LLAMADA").val();
    $("#frameFotos").attr("src", urlFotos + "?strSitioGraba=" + globalSitio + "&strNumeroEvento=" + globalEvento);
    $("#ModalFotos").modal("show");
}

function AbrirAnotacionesEventos() {
    //var llamada = $("#NUME_LLAMADA").val();
    $("#frameAnotaciones").attr("src", urlAnotacionesEvento + "?strSitioGraba=" + globalSitio + "&strNumeroEvento=" + globalEvento);
    $("#ModalAnotaciones").modal("show");
    $("#txtAnotacion").focus();
}

function AbrirEventoMedioRuta() {

    var consecutivo = $('#CADMEDISP_CONSECUTIVO').val();
    var horaRuta = $('#HORA_RUTA').val();
    var horaLlega = $('#HORA_LLEGA').val();

    if (!horaRuta || !horaLlega) {
        GestionarAlerta("warning", "No tiene horas de inicio de desplazamiento o de llegada al sitio.. verifique....");
        return;
    }
    if (consecutivo) {
        $("#frameEventoMedioRuta").attr("src", urlRutas + "?idFuerza=" + globalFuerza + "&idSitio=" + globalSitio + "&numeLlamada=" + globalLlamada + "&idConsecutivo=" + consecutivo + "&numeEvento=" + globalEvento);
        $("#ModalEventoMediosRuta").modal("show");
    } else {
        GestionarAlerta("warning", "No tiene seleccionado un medio de atención para iniciar rutas adicionales..");
        return;
    }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////FUNCIONES NECESARIAS PARA EL FUNCIONAMIENTO DE EVENTOS CUANDO SE ENCUENTRE EN CANAL DE SALUD///////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function GuardarCasoReal() {
    globalLlamada
    globalSitio
    var codigo = $("#txtCasoId").val();
    var descCaso = $("#txtCaso").val();
    if (codigo === "") {
        return;
    }
    var urlFinal = urlAsignarCasoReal.replace(/amp;/g, "");
    var datos = { intSitio: globalSitio, lgLlamada: globalLlamada, strCasoReal: codigo };

    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {
            $("#mensajeCaso").html("Asignando el caso real...");
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                $("#mensajeCaso").empty();
                $("#textCaso1").html(codigo);
                $("#textDescCaso1").html(descCaso);
                $("#txtCasoId").val("");
                $("#txtCaso").val("");
            } else {
                $("#mensajeCaso").html("No se puedo asignar el caso real...");
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
    });
}
function CompletarCaso(codigo) {
    if (codigo === "") {
        $("#" + glCasoText).val("");
        return;
    }
    var urlFinal = urlConsultarCasos.replace(/amp;/g, "");
    var datos = { strCodigo: codigo };

    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {

        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                $("#mensajeCaso").html("");
                $("#mensajeCaso2").html("");
                if (data.datos === "") {

                    $("#" + glCasoText).val("");
                    $("#txtCasoId").focus();

                } else {
                    SeleccionarCaso(codigo, data.datos);
                }
            } else {
                $("#mensajeCaso").html("No existe el caso ingresado...");
                $("#txtCasoId").focus();
            }
        }
    }).done(function (data) {
    });
}


function CompletarCasoEventoFinal(codigo) {
    if (codigo === "") {      
        return;
    }
    var urlFinal = urlConsultarCasos.replace(/amp;/g, "");
    var datos = { strCodigo: codigo };

    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {

        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                console.log(data.datos);
                if (data.datos == "") {
                    console.log("1");
                    cargarCasos(1);

                } else {
                    console.log("2");
                    $('#DSP_CODIFINAL').val(data.datos);
                }
            } 
        }
    }).done(function (data) {
    });
}


function SeleccionarCaso(codigo, descripcion) {
    var subCodigo = "";
    $("#" + glCasoId).val(codigo);
    $("#" + glCasoText).val(descripcion);
    $("#ModalCasosReal").modal("hide");
    //Consulta las preguntas asociadas al caso
    if (glPreguntasOpcion === 1) {
        $("#txtCasoId2").focus();
    } else {
        $("#txtCiudad").focus();
    }
    //$("#txtCiudad").focus();
}
function CargarCasos() {
    console.log("EntroCaso");
    $("#" + glCasoId).val("");
    $("#" + glCasoText).val("");
    var urlFinal = "";
    urlFinal = urlConsultarCasos.replace(/amp;/g, "");
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CADCASO_CODIGO", "orderable": true },
                { "data": "DESCRIPCION", "orderable": true }
    ];
    AplicarPluginTablaAjax("tblCasos", urlFinal, parametrosAjax);
}
function CargarControlesSalud() {
    console.log("salud:" + glSalud);
    if (glSalud === 1) {
        $("#contenedorCasoReal").removeClass("hidden");
        $("#anotacionesPatrulla").removeClass("hidden");
        $("#btnLocalizacionUnidades").hide();
        $("#btnResumenConducta").hide();
        $("#btnVehiculosInv").hide();
        $("#lbEnviarPeticion").hide();
        $("#btnVerificarXY").hide();
    }
}

function GuardarAnotacionPatrulla() {
    var anotacion = $("#txtAnotacionPatrulla").val();
    var patrulla = $("#CADPATRU_CODIGO").val();
    var fuerza = $("#CADUNITUR_UNDEFUERZA").val();
    var unidad = $("#CADUNITUR_UNDECODIGO").val();
    if (anotacion === "") {
        GestionarAlerta("warning", "No se ha ingresado ninguna anotación.");
        return false;
    }
    if (patrulla === "") {
        GestionarAlerta("warning", "No se ha Seleccionado un medio de atención disponible.");
        return false;
    }
    var urlFinal = "";
    var datos = { ANOTACION: anotacion, CADPATRU_CODIGO: $('#CADPATRU_CODIGO').val(), CADPATRU_FUERZA: fuerza, CADPATRU_UNIDAD: unidad, CONSECUTIVO: $("#hdAnotaConsecutivo").val() };
    urlFinal = urlGuardarAnotaPatrulla.replace(/amp;/g, "");
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {

        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                $("#txtAnotacionPatrulla").val("");
                $("#hdAnotaConsecutivo").val(0)
                //GestionarAlerta("success", data.datos);
                CargarTablaAnotacionesPatrulla(fuerza, unidad, patrulla);
            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
    });
}

function CargarTablaAnotacionesPatrulla(fuerza, unidad, codigo) {
    var urlFinal = "";
    var strTabla = "tablaAnotacionesPatrulla";
    var localParams = { strFuerza: fuerza, strUnidad: unidad, strCodigo: codigo };

    //Verifica existencia de tabla
    if ($.fn.DataTable.isDataTable("#" + strTabla)) {
        $("#" + strTabla).DataTable().destroy();
        $("#" + strTabla + " tbody").empty();
    }
    urlFinal = urlConsultarAnotaciones.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales

    var parametrosAjax = [
            { "data": "Boton", "orderable": false },
            { "data": "Anotacion", "orderable": true }
    ];
    AplicarPluginTablaParams(strTabla, urlFinal, parametrosAjax, localParams);
}

function AsignarHorasTablaAdic(campo, consec, sitio, eventoSitio, eventoNumero) {
    var urlFinal = urlAsignarHAdic.replace(/amp;/g, "");
    var datos = { intSitio: sitio, intEventoSitio: eventoSitio, lgConsec: consec, lgEventoNum: eventoNumero, strCampo: campo };
    var evento = ""
    if ($('#EVENTO_NUMEEVENTO_CONTROL').val() != "") {
        evento = $('#EVENTO_NUMEEVENTO_CONTROL').val();
    } else {
        evento = globalEvento;
    }
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                GestionarAlerta("success", data.datos);
                CargaEventoMedios(evento);
            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
    });
}

function EditarAnotacion(consecutivo, anotacion) {
    $("#txtAnotacionPatrulla").val(anotacion);
    $("#hdAnotaConsecutivo").val(consecutivo);
}

function ElminarAnotacion(consecutivo, anotacion) {

    var patrulla = $("#CADPATRU_CODIGO").val();
    var fuerza = $("#CADUNITUR_UNDEFUERZA").val();
    var unidad = $("#CADUNITUR_UNDECODIGO").val();
    if (patrulla === "") {
        GestionarAlerta("warning", "No se ha Seleccionado un medio de atención disponible.");
        return false;
    }
    var urlFinal = "";
    var datos = { CADPATRU_CODIGO: patrulla, CADPATRU_FUERZA: fuerza, CADPATRU_UNIDAD: unidad, CONSECUTIVO: consecutivo };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Anotacion: " + anotacion,
        type: "warning",
        html: true,
        showCancelButton: true,
        confirmButtonColor: "#EF5350",
        cancelButtonColor: "#EF5385",
        confirmButtonText: "Eliminar",
        cancelButtonText: "Cancelar",
        closeOnConfirm: true,
        closeOnCancel: true
    },
    function (isConfirm) {
        if (isConfirm) {
            $.ajax({
                type: "POST",
                url: urlEliminarAnotaPatrulla,
                content: "application/json; charset=utf-8",
                dataType: "json",
                data: datos,
                beforeSend: function () {
                    $(".loader_decad").removeClass("hidden");
                    $(".loader_back").removeClass("hidden");
                },
                error: function (msn) {
                    GestionarAlerta("warning", msn);
                }
            }).done(function (resultado) {
                $(".loader_decad").addClass("hidden");
                $(".loader_back").addClass("hidden");
                if (resultado.result) {
                    //GestionarAlerta("success", resultado.datos);
                    CargarTablaAnotacionesPatrulla(fuerza, unidad, patrulla);
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}

function AbrirCNP() {
    console.log(typeof globalEvento);
    if (typeof globalEvento === "undefined" || globalEvento === null) {
        GestionarAlerta("warning", "No existe un numero de evento.");
        return false;
    }
    var urlFinal = urlCNP.replace(/amp;/g, "")+"?intSitio="+globalSitio+"&dcNumEvento="+globalEvento;
    window.open(urlFinal,"_blank");
}

//Guardas Coordenadas

function GuardarCoordenadas() {

    var Latitud = $("#Latitud").val();
    var Longitud = $("#Longitud").val();

    var arrayParametros = new Array();

    arrayParametros.push(Latitud);
    arrayParametros.push(Longitud);
    arrayParametros.push(globalSitio);
    arrayParametros.push(globalLlamada);
    $.ajax({
        type: 'POST',
        url: urlProcesarCerrarEvento,
        dataType: 'json',
        data: { parametros: arrayParametros },
        success: function (resultado) {
            if (resultado.result) {
                //  $("#eventos").trigger("click");
                console.log("Cerrar");
                var msm = "LLamada Cerrada.";

                location.href = (urlRecargaCerrar + "?strCerrado=" + msm);

                GestionarAlerta("success", resultado.datos);
            }
            else {
                CargaListadoEventos(false);
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });

    //$("#DSP_CODIFINAL").val('');
}
