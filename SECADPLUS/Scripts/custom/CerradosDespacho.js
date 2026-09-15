var tabla;
var totalTabla;
var datosTabla;
var cronometro;
var globalLista;
var globalEvento;
var globalTurno;
var globalLlamada;
var globalEventoEstado;
var intentos = false;;
var tablaReload;
$(document).ready(function () {
    CambiarVisibilidadMenu();
    //GestionarAlerta("success", "Revise en cadreferenciassecad el valor para TIEMPO_PANT_SELECCION.. Se establecen por defecto 15 segundos..");

    CargaLlamadasCerradas(_idFuerza, _idCanal);

    cronometro = setInterval(
        function(){
            console.log("recarga tabla");
            tablaReload.ajax.reload();
        }
    , _tiempoRefresco);
    //console.log("Comenzo");
    $("#iTimer").removeClass('icon-watch2 icon_size-md').addClass('icon-alarm-cancel icon_size-md');
    $("#btnDetener").removeClass('btn bg-teal btn-xs').addClass('btn bg-danger btn-xs');

    $('#tablaCerradas tbody').on('click', 'tr', function () {
        tabla = $('#tablaCerradas').DataTable();
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

    $("#llamadas").click(function () {
        $("#alertaJs").empty();
        DeternerTimer();
        CargaLlamadasCerradas(_idFuerza, _idCanal);
    });

    $("#eventos").click(function () {
        intentos = true;
        $("#alertaJs").empty();
        DeternerTimer();
        CargaListadoEventos(false);
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

//-------------------------------
// Primer Tab Llamadas Cerradas

function CargaLlamadasCerradas(idFuerza, idCanal) {
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
    

    tablaReload = AplicarPluginTablaCerrados("tablaCerradas", urlTablaCerradas + "?idFuerza=" + idFuerza + "&idCanal=" + idCanal, parametrosAjax);
}

function RefrescarPanatalla() {
    CargaLlamadasCerradas(_idFuerza, _idCanal);
}

function RefrescarPanatallaTiempo() {
    CargaLlamadasCerradas(_idFuerza, _idCanal);
}

function DeternerTimer() {
    if (cronometro == null) {
        $("#iTimer").removeClass('icon-watch2 icon_size-md').addClass('icon-alarm-cancel icon_size-md');
        $("#btnDetener").removeClass('btn bg-teal btn-xs').addClass('btn bg-danger btn-xs');
        cronometro = setInterval(RefrescarPanatallaTiempo, _tiempoRefresco);
    } else {
        $("#iTimer").removeClass('icon-alarm-cancel icon_size-md').addClass('icon-watch2 icon_size-md');
        $("#btnDetener").removeClass('btn bg-danger btn-xs').addClass('btn bg-teal btn-xs');
        clearInterval(cronometro);
        cronometro = null;
    }
}

function recorrerTablaRecepcion() {
    var tabla = $('#tablaCerradas').DataTable();
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

function CargarEventos(NUME_LLAMADA) {
    if (!NUME_LLAMADA) {
        return;
    }

    $("#alertaJs").empty();
    DeternerTimer();

    var arrayParametros = new Array();

    arrayParametros.push(globalSitio);
    arrayParametros.push(NUME_LLAMADA);
    arrayParametros.push(globalFuerza);
    arrayParametros.push(globalCanal);

    $.ajax({
        type: 'POST',
        url: urlProcesarEvento,
        dataType: 'json',
        data: { parametros: arrayParametros },
        success: function (resultado) {
            //console.log("Entro");
            if (resultado.result) {
                globalNumeEvento = resultado.datos

                $("#eventos").trigger("click");
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
// Fin Primer Tab Llamadas Cerradas

//-------------------------------
// Segundo Tab Eventos

function CargaListadoEventos(bandera) {
    if (bandera) {
        globalNumeEvento = 0;
    }

    var parametrosAjax = [
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

        //if (intentos <= 2)
        CargarCamposPedido(_numeEvento, _numeLlamada);
        //CargaComentariosAdicionales(_numeLlamada);
        intentos = false;
    }
}

function CargarCamposPedido(numeEvento, numeroLlamada) {
    globalEvento = numeEvento;

    $.ajax({
        type: 'POST',
        url: urlPedidoEvento,
        dataType: 'json',
        //data: { idSitio: globalSitio, numeLlamada: numeroLlamada}, cambio rmr 08-10-2020 09:44 am
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
                LlenarCamposClaseEvento(resultado.lista3);
                CargaEventoMedios(numeEvento);
                CargaEventoMediosControl();
                CargarAnotacionesTabla();
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
    document.getElementById("textAsociarLlamada").innerHTML = lista[0].CADPEDI_NUME_LLAMADA;

    $('#textComentarios').val(lista[0].COMENTARIO);

    document.getElementById("textCaso1").innerHTML = lista[0].CODI_PEDIDO;
    document.getElementById("textDescCaso1").innerHTML = lista[0].DESC_CODI_PEDIDO;
    document.getElementById("textCaso2").innerHTML = lista[0].CODI_PEDIDO2;
    document.getElementById("textDescCaso2").innerHTML = lista[0].DESC_CODI_PEDIDO2;
    document.getElementById("textDireccion").innerHTML = lista[0].DIRE_CASO;
    document.getElementById("textBarrio").innerHTML = lista[0].DSP_BARRIO;
    document.getElementById("textCiudad").innerHTML = lista[0].DSP_CIUDAD;
    document.getElementById("textCasoReal").innerHTML = lista[0].CODI_PEDIDO_REAL;
    document.getElementById("textDescCasoReal").innerHTML = lista[0].DESC_CODI_PEDIDO_REAL;
    //rmr cambio 07-10-2020
    
    //fin rmr cambio 07-10-2020

    if (globalEventoEstado == "Cerrado") {
        /*
         * /*
         * CODI_PEDIDO_REAL = strCodigoReal;
            DESC_CODI_PEDIDO_REAL = strDescCodigoReal;
            ---CAMBIO RMR 05-10-2020
            $("#CADCODI_FINAL").val(lista[0].CODI_PEDIDO);
            $("#DSP_CODIFINAL").val(lista[0].DESC_CODI_PEDIDO);

            $("#CADCODI_FINAL").val(lista[0].CODI_PEDIDO_REAL);
            $("#DSP_CODIFINAL").val(lista[0].DESC_CODI_PEDIDO_REAL);
         */       
        $("#CADCODI_FINAL").val(lista[0].CODI_PEDIDO_REAL);
        $("#DSP_CODIFINAL").val(lista[0].DESC_CODI_PEDIDO_REAL);
    }
    else {
        $("#CADCODI_FINAL").val('');
        $("#DSP_CODIFINAL").val('');
        if (lista[0].CODI_PEDIDO_REAL) {
            $("#CADCODI_FINAL").val(lista[0].CODI_PEDIDO_REAL);
            $("#DSP_CODIFINAL").val(lista[0].DESC_CODI_PEDIDO_REAL);
        }
    }

    $('#CADPEDI_SITIOGRABA').val(globalSitio);
    $('#CADPEDI_NUMELLAMADA').val(lista[0].NUME_LLAMADA);
    $('#CODI_PREGUNTA').val(lista[0].CODI_PEDIDO);

    $('#LUGE_CODIGO').val(lista[0].LUGE_CODIGO);

    CargaComentariosAdicionales(lista[0].NUME_LLAMADA);
}

function LlenarCamposDatosAdicionales(lista2) {
    //document.getElementById("Latitud").innerHTML = lista2[0].X_COORD;
    //document.getElementById("Longitud").innerHTML = lista2[0].Y_COORD;
    var x = globalEvento;
    if ((lista2.length) > 0) {
        $('#Latitud').val(lista2[0].X_COORD);
        $('#Longitud').val(lista2[0].Y_COORD);
        $('#ModalidadId').val(lista2[0].MODALIDADCASO);
        $('#ConductaId').val(lista2[0].CONDUCTAESPECIAL);
        $('#TipoArmId').val(lista2[0].ARMAMEDIO);
        $('#TipoMovilizaId').val(lista2[0].TIPOMOVILIZAGRESOR);
    }
}

function LlenarCamposClaseEvento(lista3) {
    //document.getElementById("Latitud").innerHTML = lista2[0].X_COORD;
    //document.getElementById("Longitud").innerHTML = lista2[0].Y_COORD;
    if ((lista3.length) > 0) {
        $('#CADCLCI_CODIGO').val(lista3[0].CADCLCI_CODIGO);
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
    var codiCaso = $('#CODI_PREGUNTA').val();

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
            { "data": "CODIGO" },
            { "data": "DESCRIPCION" },
            { "data": "CIUDAD_BARRIO" },
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
                CargarCamposPedido(globalEvento, numeLlamada);
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
function AdicionarMedioCaso() {
    var arrayParametros = new Array();

    arrayParametros.push(globalSitio);
    arrayParametros.push(globalEvento);
    arrayParametros.push(globalSitio);
    arrayParametros.push($('#CONSECUTIVO').val());
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
        { "data": "HORA_TERMINA", "orderable": false },
        { "data": "NOVEDAD", "orderable": false }
    ];

    $("#tablaEventoMedios").DataTable().destroy();
    $("#tablaEventoMedios tbody").empty();

    AplicarPluginTablaAjax("tablaEventoMedios", urlTablaEventoMedios + "?idSitio=" + globalSitio + "&numeEvento=" + numeEvento, parametrosAjax);
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
    // console.log(likeRuta);

    if (likeRuta == null) {
        $('#HORA_RUTA').val(datosTabla["HORA_RUTA"]);
    } else {
        $('#HORA_RUTA').val('');
    }

    var likeLlegada = datosTabla["HORA_LLEGA"].match(/button/);
    //console.log(likeLlegada);
    if (likeLlegada == null) {
        $('#HORA_LLEGA').val(datosTabla["HORA_LLEGA"]);
    } else {
        $('#HORA_LLEGA').val('');
    }

    var likeTermina = datosTabla["HORA_TERMINA"].match(/button/);
    // console.log(likeTermina);

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

function EliminarEventoMedio(consecutivo) {
    console.log("Entro consecutivo: " + consecutivo);

    var arrayParametros = new Array();

    arrayParametros.push(globalSitio);
    arrayParametros.push(consecutivo);
    arrayParametros.push(globalSitio);
    arrayParametros.push(globalEvento);

    $.ajax({
        type: 'POST',
        url: urlValidarEventosMediosRuta,
        dataType: 'json',
        data: { parametros: arrayParametros },
        success: function (resultado) {
            if (resultado.result) {
                EliminarDatoEventoMedio(arrayParametros);
            } else {
                GestionarAlerta("warning", resultado.datos);
                //EliminarDatoEventoMedio(arrayParametros);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

function EliminarDatoEventoMedio(arrayParametros) {
    console.log(arrayParametros);

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

function cargarCasos(opcion) {
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

function CargarAnotacionesTabla() {
    var parametrosAjax = [
    { "data": "ANOTACION", "orderable": false },
    { "data": "CONSECUTIVO", "orderable": false },
    { "data": "CADUSUA_GRABA", "orderable": false },
    { "data": "FECHA_GRABACION", "orderable": false },
    ];

    $("#tablaAnotaciones").DataTable().destroy();
    $("#tablaAnotaciones tbody").empty();
    //AplicarPluginTablaAjax("tablaAnotaciones", urlTablaAnotaciones + "?idSitio=" + globalSitio + "&numeEvento=" + globalEvento, parametrosAjax);
    AplicarPluginTablaAjax("tablaAnotaciones", urlTablaAnotaciones + "?idSitio=" + globalSitio + "&numeLlamada=" + globalLlamada, parametrosAjax);
}

//------------Botones

function AbrirTurnosEvento() {
    var tabla = $('#tablaEventoMedios').DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length > 0) {
        var win = window.open(urlTurnosEvento + "?intSitioGrabacion=" + globalSitio + "&strNombre=" + globalDesSitioGraba + "&intEntidad=" + globalFuerza + "&strEntidad=" + globalDesFuerza + "&strConsulta=CONSULTA" + "&intTurno=" + globalTurno + "&intSitioGrabacionTurno=" + globalSitio, '_blank');
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

function AbrirResibirLlamadaEvento() {
    var win = window.open(urlResibirLlamadaEvento, '_blank');
    win.focus();
}

function AbrirDatosLlamadasEvento() {
    var win = window.open(urlDatosLlamadaEvento + "?intSitio=" + globalSitio + "&lgLlamada=" + globalLlamada, '_blank');
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
    /*
    if (codigoFinal) {
        GestionarAlerta("warning", "Digite el codigo de cierre del evento ...");
        return;
    }*/

    var arrayParametros = new Array();

    arrayParametros.push(codigoFinal);
    arrayParametros.push(globalSitio);
    arrayParametros.push(globalEvento);
    //arrayParametros.push("rigo");
    //arrayParametros.push("rigo");

    $.ajax({
        type: 'POST',
        url: urlProcesarCerrarEvento,
        dataType: 'json',
        data: { parametros: arrayParametros },
        success: function (resultado) {
            if (resultado.result) {
                $("#eventos").trigger("click");
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

function cargarAnotaciones(modal) {
    if (modal) {
        $("#ModalBtnAnotacionesGenerales").modal();
    }

    var parametrosAjax = [
    { "data": "ANOTACION" },
    { "data": "DSP_FUERZA" },
    { "data": "DSP_CANAL" },
    { "data": "CADUSUA_GRABA" },
    { "data": "FECHA_GRABACION" },
    { "data": "CONSECUTIVO" }
    ];

    $("#tablaAnotacionesGenerales").DataTable().destroy();
    $("#tablaAnotacionesGenerales tbody").empty();
    AplicarPluginTablaAjax("tablaAnotacionesGenerales", urlTablaAnotaciones + "?idSitio=" + globalSitio + "&numeEvento=" + globalEvento, parametrosAjax);
}

//function AbrirFotos() {
//    var win = window.open(urlFotos + "?strSitioGraba=" + globalSitio + "&strNumeroEvento=" + globalEvento, '_blank');
//    win.focus();
//}

function AbrirVehiculos() {
    var win = window.open(urlVehiculos + "?intNumeroLLamada=" + globalLlamada + "&intSitioGraba=" + globalSitio, '_blank');
    win.focus();
}

//function AbrirAnotacionesGenerales() {
//    var win = window.open(urlAnotacionesGenerales, '_blank');
//    win.focus();
//}

function ProcesarVerifXY() {
    //colocar codigo de proceso (Procedimiento Almacenado)
}

function AbrirMensajero() {
    var win = window.open(urlMensajero, '_blank');
    win.focus();
}

//------------Fin Botones

function CargaEventoMediosControl() {
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.1%" },
        { "data": "DESC_PATRULLA" },
        { "data": "Boton", "orderable": false },
        { "data": "CODI_PEDIDO" },
        { "data": "CODI_PEDIDO2" },
        { "data": "HORA_ASIGNA" },
        { "data": "HORA_RUTA" },
        { "data": "HORA_LLEGA" },
        { "data": "DIRE_CASO" }
    ];

    $("#tablaEventoMediosControl").DataTable().destroy();
    $("#tablaEventoMediosControl tbody").empty();

    AplicarPluginTablaMediosControl("tablaEventoMediosControl", urlTablaMediosControl + "?idFuerza=" + globalFuerza + "&idCanal=" + globalCanal, parametrosAjax);
}

function recorrerTablaMediosControl() {
    var tabla = $('#tablaEventoMediosControl').DataTable();
    var datosTabla = tabla.rows().data();
    var arrayTabla = [];
    var celda;

    $.each(datosTabla, function (i, val) {
        //# PintarColor HORA_LLEGA
        var horaLlega = datosTabla[i]["HORA_LLEGA"];
        var minutos = datosTabla[i]["MINUTOS"];

        if (minutos >= _tiempoLlega) {
            celda = tabla.cells(":contains('" + horaLlega + "')").nodes();
            $(celda).addClass('bg-danger');
        } else {
            celda = tabla.cells(":contains('" + horaLlega + "')").nodes();
            $(celda).addClass('bg-success');
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

function AsignarHorasTabla(idCampo, consecutivo) {
    ///Se nesesitan mas no se implemetan estas funciones ya que despachos ya las tiene implementadas
}

function AsignarHorasTablaPorEvento(idCampo, consecutivo, evento) {
    ///Se nesesitan mas no se implemetan estas funciones ya que despachos ya las tiene implementadas
}

///-----

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
}

function SalvarCambios() {
    CargaEventoMediosControl();
}