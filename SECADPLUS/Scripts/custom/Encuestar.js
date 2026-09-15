var globalLista;

$(".chosen-fixed").chosen({ width: "100%" });

$(document).ready(function () {
    CargaEventos();
});

function CargaEventos() {
    var parametrosAjax = [
            { "data": "BOTON", "orderable": false, "width": "0.1%" },
            { "data": "NUME_EVENTO", "orderable": false },
            { "data": "ESTADO_EVENTO", "orderable": false },
            { "data": "FECH_CREA", "orderable": false },
            { "data": "CADUSUA_EVEN_GENERA", "orderable": false },
    ];

    $("#tablaEventos").DataTable().destroy();
    $("#tablaEventos tbody").empty();

    AplicarPluginTablaEventos("tablaEventos", urlTablaEventos + "?idSitio=" + globalSitio, parametrosAjax);
}

function ConsultarTurno() {
    var tabla = $('#tablaEventos').DataTable();
    var fila = tabla.rows(0).data();

    if (fila[0] === undefined) {
        return;
    }

    var _numeLlamada = fila[0].CADPEDCA_NUMELLAMADA;
    var _numeEvento = fila[0].NUME_EVENTO;
    var _turno = fila[0].CADTURN_TURNO;

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

    CargarCamposPedido(_numeEvento, _numeLlamada);
    CargarEncuesta(_numeEvento, _numeLlamada);

    //LlenarCamposEncuestaM(_numeEvento);

    //CargaEventoMedios(_numeEvento);
    //VerEventoMedios(_numeEvento);
}

function CargarCamposPedido(numeEvento, numeroLlamada) {
    $.ajax({
        type: 'POST',
        url: urlPedidoEvento,
        dataType: 'json',
        data: { idSitio: globalSitio, numeLlamada: numeroLlamada },
        success: function (resultado) {
            if (resultado.result) {
                globalLista = resultado.lista;
                LlenarCamposPedido(resultado.lista);
                LlenarCamposPedidoEncuesta(resultado.lista);
                CargaEventoMedios(numeEvento);
            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
    CargarEncuesta(numeEvento, numeroLlamada);
}

function CargarEncuesta(numeEvento, numeroLlamada) {
    $('#NumeroLLamadaEncuestaPed').val(numeroLlamada);
    $.ajax({
        type: 'POST',
        url: urlEncuesta,
        dataType: 'json',
        data: { idSitio: globalSitio, numeLlamada: numeroLlamada },
        success: function (resultados) {
            //console.log("inicia");
            limpiarEncuesta();

            if (resultados.result) {
                if (resultados.exite) {
                    // console.log("CargarEncuesta");
                    LlenarCamposEncuestaM(resultados.encuesta);
                }
            } else {
                GestionarAlerta("warning", resultados.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

function CargaEventoMedios(numeEvento) {
    $.ajax({
        type: 'POST',
        //url: 'VerEventoMedios',
        url: urlTablaEventoMedios,
        dataType: 'html',
        data: { idSitio: globalSitio, numeEvento: numeEvento }
    }).done(function (medios) {
        if ($.fn.DataTable.isDataTable("#tablaEventoMedios")) {
            $("#tablaEventoMedios").DataTable().destroy();
        }
        $("#tablaEventoMedios tbody").empty();
        $("#tablaEventoMedios tbody").html(medios);
        AplicarPluginTabla("tablaEventoMedios");
    });
};

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
    document.getElementById("textTipoInfo").innerHTML = lista[0].TIPO_INFORMACION;
    document.getElementById("textCodigoCooperante").innerHTML = lista[0].CODIGO_COOPERANTE;
    document.getElementById("textAsociarLlamada").innerHTML = lista[0].CADPEDI_NUME_LLAMADA;
    $('#textComentarios').val(lista[0].COMENTARIO);
    document.getElementById("textCaso1").innerHTML = lista[0].CODI_PEDIDO;
    document.getElementById("textDescCaso1").innerHTML = lista[0].DESC_CODI_PEDIDO;
    document.getElementById("textCaso2").innerHTML = lista[0].CODI_PEDIDO2;
    document.getElementById("textDescCaso2").innerHTML = lista[0].DESC_CODI_PEDIDO2;
    document.getElementById("textDireccion").innerHTML = lista[0].DIRE_CASO;
    document.getElementById("textBarrio").innerHTML = lista[0].DSP_BARRIO;
    document.getElementById("textCiudad").innerHTML = lista[0].DSP_CIUDAD;
}

function LlenarCamposPedidoEncuesta(lista) {
    document.getElementById("textEncSitio").innerHTML = globalSitio;
    document.getElementById("textEncNoLlamada").innerHTML = lista[0].NUME_LLAMADA;
    document.getElementById("textEncTelefono").innerHTML = lista[0].NUME_TELEFONO;
    document.getElementById("textEncFechaCaso").innerHTML = lista[0].HORA_CASO;
    document.getElementById("textEncCodCiudad").innerHTML = lista[0].LUGE_CODIGO;
    document.getElementById("textEncCiudad").innerHTML = lista[0].DSP_CIUDAD;
    document.getElementById("textEncCodCaso").innerHTML = lista[0].CODI_PEDIDO;
    document.getElementById("textEncCaso").innerHTML = lista[0].DESC_CODI_PEDIDO;
    document.getElementById("textEncDireccion").innerHTML = lista[0].DIRE_CASO;
    document.getElementById("textEncLlamante").innerHTML = lista[0].NOMB_LLAMANTE;
    $('#textEncResumen').val(lista[0].DIRE_CASO + " " + lista[0].COMENTARIO);

    //--

    $('#CADSITIO_GRABACION').val(globalSitio);
    $('#CADNUME_PEDIDO').val(lista[0].NUME_LLAMADA);
    $('#NUME_TELEFONO').val(lista[0].NUME_TELEFONO);
    $('#LUGE_CASO').val(lista[0].LUGE_CODIGO);
    $('#FECHA_CASO').val(lista[0].HORA_CASO);
    $('#CODIGO_CASO').val(lista[0].CODI_PEDIDO);
    $('#RESUMEN_CASO').val(lista[0].DIRE_CASO + " " + lista[0].COMENTARIO);
    $('#LLAMANTE').val(lista[0].NOMB_LLAMANTE);
}

function LlenarCamposEncuestaM(encuesta) {
    //console.log("Encuesta  "  + encuesta[14]);

    if (encuesta[0] == "SI") {
        $('#ACEPTA_ENCUESTA_SI').trigger("click");
        $('#ACEPTA_ENCUESTA_SI').trigger("click");
    } else if (encuesta[0] == "NO") {
        $('#ACEPTA_ENCUESTA_NO').trigger("click");
        $('#ACEPTA_ENCUESTA_NO').trigger("click");
    }

    $('#NOVEDAD_ENCUESTA').val(encuesta[1]);
    $('#NOVEDAD_ENCUESTA').chosen().trigger("chosen:updated");

    $('#NOMBRE_ENCUESTADO').val(encuesta[2]);

    $('#NOVEDAD_ATENCION').val(encuesta[3]);
    $('#NOVEDAD_ATENCION').chosen().trigger("chosen:updated");

    if (encuesta[4] == "0") {
        $('#CALIF_OPORTUNIDAD_0').trigger("click");
        $('#CALIF_OPORTUNIDAD_0').trigger("click");
    } else if (encuesta[4] == "1") {
        $('#CALIF_OPORTUNIDAD_1').trigger("click");
        $('#CALIF_OPORTUNIDAD_1').trigger("click");
    } else if (encuesta[4] == "2") {
        $('#CALIF_OPORTUNIDAD_2').trigger("click");
        $('#CALIF_OPORTUNIDAD_2').trigger("click");
    } else if (encuesta[4] == "3") {
        $('#CALIF_OPORTUNIDAD_3').trigger("click");
        $('#CALIF_OPORTUNIDAD_3').trigger("click");
    } else if (encuesta[4] == "4") {
        $('#CALIF_OPORTUNIDAD_4').trigger("click");
        $('#CALIF_OPORTUNIDAD_4').trigger("click");
    }

    if (encuesta[5] == "0") {
        $('#CALIF_AMABILIDAD_0').trigger("click");
        $('#CALIF_AMABILIDAD_0').trigger("click");
    } else if (encuesta[5] == "1") {
        $('#CALIF_AMABILIDAD_1').trigger("click");
        $('#CALIF_AMABILIDAD_1').trigger("click");
    } else if (encuesta[5] == "2") {
        $('#CALIF_AMABILIDAD_2').trigger("click");
        $('#CALIF_AMABILIDAD_2').trigger("click");
    } else if (encuesta[5] == "3") {
        $('#CALIF_AMABILIDAD_3').trigger("click");
        $('#CALIF_AMABILIDAD_3').trigger("click");
    } else if (encuesta[5] == "4") {
        $('#CALIF_AMABILIDAD_4').trigger("click");
        $('#CALIF_AMABILIDAD_4').trigger("click");
    }

    if (encuesta[6] == "0") {
        $('#CALIF_EFECTIVIDAD_0').trigger("click");
        $('#CALIF_EFECTIVIDAD_0').trigger("click");
    } else if (encuesta[6] == "1") {
        $('#CALIF_EFECTIVIDAD_1').trigger("click");
        $('#CALIF_EFECTIVIDAD_1').trigger("click");
    } else if (encuesta[6] == "2") {
        $('#CALIF_EFECTIVIDAD_2').trigger("click");
        $('#CALIF_EFECTIVIDAD_2').trigger("click");
    } else if (encuesta[6] == "3") {
        $('#CALIF_EFECTIVIDAD_3').trigger("click");
        $('#CALIF_EFECTIVIDAD_3').trigger("click");
    } else if (encuesta[6] == "4") {
        $('#CALIF_EFECTIVIDAD_4').trigger("click");
        $('#CALIF_EFECTIVIDAD_4').trigger("click");
    }

    $('#OBSERV_OPORTUNIDAD').val(encuesta[7]);
    $('#OBSERV_AMABILIDAD').val(encuesta[8]);
    $('#OBSERV_EFECTIVIDAD').val(encuesta[9]);

    $('#OBSERVACION').val(encuesta[10]);
    $('#EMAIL_PERSONA').val(encuesta[11]);

    if (encuesta[12] == "A") {
        $('#ESTADO_ENCUESTA_A').trigger("click");
        $('#ESTADO_ENCUESTA_A').trigger("click");
    } else if (encuesta[12] == "C") {
        $('#ESTADO_ENCUESTA_C').trigger("click");
        $('#ESTADO_ENCUESTA_C').trigger("click");
    }

    $('#USUARIO_CREACION').val(encuesta[13]);
    $('#FECHA_CREACION').val(encuesta[14]);

    $('#USUARIO_MODIFICACION').val(encuesta[15]);
    $('#FECHA_MODIFICACION').val(encuesta[16]);
}

function limpiarEncuesta() {
    $('#ACEPTA_ENCUESTA_SI').trigger("click");
    $('#ACEPTA_ENCUESTA_SI').trigger("click");

    $('#NOVEDAD_ENCUESTA').val('');
    $('#NOVEDAD_ENCUESTA').chosen().trigger("chosen:updated");

    $('#NOMBRE_ENCUESTADO').val('');

    $('#NOVEDAD_ATENCION').val('');
    $('#NOVEDAD_ATENCION').chosen().trigger("chosen:updated");

    $('#CALIF_OPORTUNIDAD_0').trigger("click");
    $('#CALIF_OPORTUNIDAD_0').trigger("click");

    $('#CALIF_AMABILIDAD_0').trigger("click");
    $('#CALIF_AMABILIDAD_0').trigger("click");

    $('#CALIF_EFECTIVIDAD_0').trigger("click");
    $('#CALIF_EFECTIVIDAD_0').trigger("click");

    $('#OBSERV_OPORTUNIDAD').val('');
    $('#OBSERV_AMABILIDAD').val('');
    $('#OBSERV_EFECTIVIDAD').val('');

    $('#OBSERVACION').val('');
    $('#EMAIL_PERSONA').val('');

    $('#ESTADO_ENCUESTA_A').trigger("click");
    $('#ESTADO_ENCUESTA_A').trigger("click");

    $('#USUARIO_CREACION').val('');
    $('#FECHA_CREACION').val('');
    // $('#FECHA_CREACION').val(new Date.now);

    $('#USUARIO_MODIFICACION').val('');
    $('#FECHA_MODIFICACION').val('');
}

function guardarEncuesta() {
    var datosEncuesta = $("#CentroEncuesta").serialize();

    $.ajax({
        type: 'POST',
        url: rutaCreacion,
        dataType: 'json',
        data: datosEncuesta,
        success: function (resultado) {
            if (resultado.result) {
                GestionarAlerta("success", resultado.datos.MENSAJE);
                $('#FECHA_CREACION').val(resultado.datos.FECHA_CREACION);
            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

function AbrirDatosLlamadasEncuesta() {
    var win = window.open(urlDatosLlamadaEncuesta + "?intSitio=" + globalSitio + "&lgLlamada=" + $('#NumeroLLamadaEncuestaPed').val(), '_blank');
    win.focus();
}

//function CargaEventoMedios(numeEvento) {
//    var parametrosAjax = [
//            { "data": "DSP_DESC_UNIDAD" },
//            { "data": "DSP_MEDIODISPO" },
//            { "data": "TIPO_RESPONSABLE" },
//            { "data": "HORA_ASIGNA" },
//            { "data": "HORA_LLEGA" },
//            { "data": "HORA_TERMINA" },
//            { "data": "NOVEDAD" },
//    ];

//    $("#tablaEventoMedios").DataTable().destroy();
//    $("#tablaEventoMedios tbody").empty();

//    AplicarPluginTablaAjax("tablaEventoMedios", urlTablaEventoMedios + "?idSitio=" + globalSitio + "&numeEvento=" + numeEvento, parametrosAjax);
//}

//function DeternerTimer() {
//    if (cronometro == null) {
//        $("#iTimer").removeClass('icon-watch2 icon_size-md').addClass('icon-alarm-cancel icon_size-md');
//        $("#btnDetener").removeClass('btn bg-teal btn-xs').addClass('btn bg-danger btn-xs');
//        cronometro = setInterval(CargaControlDespacho, _tiempoRefresco);
//    } else {
//        $("#iTimer").removeClass('icon-alarm-cancel icon_size-md').addClass('icon-watch2 icon_size-md');
//        $("#btnDetener").removeClass('btn bg-danger btn-xs').addClass('btn bg-teal btn-xs');
//        clearInterval(cronometro);
//        cronometro = null;
//    }
//}