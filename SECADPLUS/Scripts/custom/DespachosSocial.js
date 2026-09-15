var glCasoText = "", glCasoId = "";

var parametrosFichaSocial = [
                       { "data": "Dummy", "orderable": false },
                       { "data": "Boton", "orderable": false },
                       { "data": "NRO_FICHA" },
                       { "data": "FICHA_REMITIDA" },
                       { "data": "DESP_MPIO_ATENCION" },
                       { "data": "DESP_BARRIO_ATENCION" },
                       { "data": "DIRECCION" },
                       { "data": "TELEFONO" },
                       { "data": "CODI_EVENTO" },
                       { "data": "OBSERVACION1" },
                       { "data": "CADUSUARIO" },
                       { "data": "FECHA_TRANSACCION" },
                       { "data": "BARRIO_CODIGO", "visible": false },
                       { "data": "BARRIO_LUGECODIGO", "visible": false },


];

var parametrosPersonaSocial = [
                       { "data": "Dummy", "orderable": false },
                       { "data": "Boton", "orderable": false },
                       { "data": "TIPO_IDENTIFICACION" },
                       { "data": "IDENTIFICACION" },
                       { "data": "NOMBRES" },
                       { "data": "APELLIDOS" },
                       { "data": "FECHA_NACE" },
                       { "data": "EDAD_PERSONA" },
                       { "data": "GENERO", "orderable": false },
                       { "data": "RAZA", "orderable": false },
                       { "data": "COND_LGTBI", "orderable": false },
                       { "data": "COND_DISCAPACIDAD", "orderable": false },
                       { "data": "COND_DESPLAZADO", "orderable": false },
                       { "data": "OBSERVACIONES", "orderable": false },


                       { "data": "CODI_GENERO", "visible": false },
                       { "data": "CODI_RAZA", "visible": false },
                       { "data": "CADUSUARIO", "visible": false },
                       { "data": "FECHA_TRANSACCION", "visible": false },
                       { "data": "NRO_FICHA", "visible": false },
                       { "data": "OBSERVACIONES", "visible": false },
                       { "data": "CONSECUTIVO", "visible": false },

];



var parametrosAjaxCiudad = [
       { "data": "Dummy", "orderable": false },
       { "data": "Boton", "orderable": false },
       { "data": "CODIGO" },
       { "data": "DESCRIPCION" },
];


var parametrosAjaxBarrioComuna = [
       { "data": "Dummy", "orderable": false },
       { "data": "Boton", "orderable": false },
       { "data": "DESCRIPCION" },
       { "data": "LUGE_CODIGO" },
       { "data": "CODIGO" },

];

var parametrosAjaxTS = [
       { "data": "Dummy", "orderable": false },
       { "data": "Boton", "orderable": false },
       { "data": "DESP_TIPO_SERVICIO" },
       { "data": "NRO_FICHA", "visible": false },
       { "data": "CADUSUARIO", "visible": false },
       { "data": "FECHA_TRANSACCION", "visible": false },
       { "data": "TIPO_SERVICIO", "visible": false },
];

var parametrosAjaxTR = [
       { "data": "Dummy", "orderable": false },
       { "data": "Boton", "orderable": false },
       { "data": "DESP_TIPO_REMISION" },
       { "data": "NRO_FICHA", "visible": false },
       { "data": "CADUSUARIO", "visible": false },
       { "data": "FECHA_TRANSACCION", "visible": false },
       { "data": "TIPO_REMISION", "visible": false },
];

$(document).ready(function () {
    CambiarVisibilidadMenu();

    AplicarPluginTablaAjaxFichaSocial("tablaFichaSocial", urlFichaSocial + "?strSitioGrabaEvento=" + SitioGrabaEventoGlobal + "&strEvento=" + EventoGlobal, parametrosFichaSocial);

    AplicarPluginTablaAjaxFichaSocialTipos("tablaTipoServisioSocial", urlCagarTipos + "?strSitioGrabaEvento=" + SitioGrabaEventoGlobal + "&strEvento=" + EventoGlobal + "&blTs=true" , parametrosAjaxTS, 1);
    AplicarPluginTablaAjaxFichaSocialTipos("tablaRemicionesSocial", urlCagarTipos + "?strSitioGrabaEvento=" + SitioGrabaEventoGlobal + "&strEvento=" + EventoGlobal + "&blTs=false" , parametrosAjaxTR, 0);
    AplicarPluginTablaAjaxFichaSocialPersonas("tablaPersonaSocial", urlCagarPersonasSocial + "?strSitioGrabaEvento=" + SitioGrabaEventoGlobal + "&strEvento=" + EventoGlobal , parametrosPersonaSocial);


    $("#IDENTIFICACION").keypress(function (e) {
        if (e.which == 13) {

            CargarDatosRegisEnter($('#TIPO_IDENTIFICACION').val(), $(this).val());
        }
    });

    $("#DESP_MPIO_ATENCION").keypress(function (e) {
        if (e.which == 13) {
        
            CargarCiudadesEnter($(this).val());
        }
    });


    $("#DESP_BARRIO_ATENCION").keypress(function (e) {
        if (e.which == 13) {
           
            CargarBarriosComunaEnter($(this).val());
        }
    });


    $("#txtCasoId").keypress(function (e) {
        if (e.which == 13) {
            glCasoText = "txtCaso";
            glCasoId = "txtCasoId";
            CompletarCaso($(this).val());
        }

    });


});



function CargarDatosRegisEnter(tipoIdentificacion, numeroIdentificacion) {
    console.log(tipoIdentificacion + "    " + numeroIdentificacion);

    if (numeroIdentificacion != '' && tipoIdentificacion != '' && tipoIdentificacion != '00' && tipoIdentificacion != 'SD') {

        $.ajax({
            type: 'POST',
            url: urlDatosRegistraduria,
            dataType: 'json',
            data: { strTipoIdentificacion: tipoIdentificacion, strIdentificacion: numeroIdentificacion },
            success: function (resultados) {
                if (resultados.result == true) {
                    $('#REGITRADURIA').val(resultados.datosReg);
                } else {
                    GestionarAlerta("warning", resultados.datos);
                }
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });

    }
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
        //beforeSend: function () {
        //    if (glPreguntasOpcion === 1) {
        //        $("#mensajeCaso").html("Identificando el caso ingresado...");
        //    } else {
        //        $("#mensajeCaso2").html("Identificando el caso ingresado...");
        //    }
        //},
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                $("#mensajeCaso").html("");
                $("#mensajeCaso2").html("");
                //if (data.datos === "") {
                //    if (glPreguntasOpcion === 1) {
                //        $("#mensajeCaso").html("No existe el caso ingresado...");
                //        $("#" + glCasoText).val("");
                //        $("#txtCasoId").focus();
                //    } else {
                //        $("#mensajeCaso2").html("No existe el caso ingresado...");
                //        $("#" + glCasoText).val("");
                //        $("#txtCasoId2").focus();
                //    }
                //} else {
                    SeleccionarCaso(codigo, data.datos);
                //}
            } else {
                $("#mensajeCaso").html("No existe el caso ingresado...");
                $("#txtCasoId").focus();
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
    //if (glPreguntasOpcion === 1) {
    //    $("#txtCasoId2").focus();
    //} else {
    //    $("#txtCiudad").focus();
    //}
    //$("#txtCiudad").focus();
}

function CargarCasos() {
   
    var urlFinal = "";
    urlFinal = urlConsultarCasos.replace(/amp;/g, "");
    console.log(urlFinal);
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CADCASO_CODIGO", "orderable": true },
                { "data": "DESCRIPCION", "orderable": true }
    ];


    if ($.fn.DataTable.isDataTable("#tblCasos")) {
        $("#tblCasos").DataTable().destroy();
        $("#tblCasos tbody").empty();
    }
    AplicarPluginTablaAjax("tblCasos", urlFinal, parametrosAjax);

    $("#ModalCasosReal").modal();


}

function crearFichaSocial()
{
   
    if ($('#MPIO_ATENCION').val() == "" && $('#DESP_MPIO_ATENCION').val() == "" && $('#DIRECCION').val() == ""
        && $('#txtCasoId').val() == "" && $('#OBSERVACION1').val() == "") {
        GestionarAlerta("warning", "Ingrese datos");
        return;
    }

    var datos = $("#frmSocial").serialize();
  

    $.ajax({
        method: "POST",
        url: urlCrearFichaSocial,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
       
        success: function (data) {
            
            if (data.result) {
                LimpiarCampos();
                GestionarAlerta("success", data.datos);
               
            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
    });

}

function crearPersonaSocial() {

    if ($('#NRO_FICHA').val() == "") {
        GestionarAlerta("warning", "Debe seleccionar o crear una ficha.");
        return;
    }

    if ($('#TIPO_IDENTIFICACION').val() == "" && $('#IDENTIFICACION').val() == "" && $('#NOMBRES').val() == ""
        && $('#APELLIDOS').val() == "" ) {
        GestionarAlerta("warning", "Ingrese datos requeridos");
        return;
    }

    var datos = $("#frmPersonaSocial").serialize();


    $.ajax({
        method: "POST",
        url: urlCrearPersonaSocial,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,

        success: function (data) {

            if (data.result) {
                limpiarCamposPersonaSocial();
                GestionarAlerta("success", data.datos);

            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
    });

}

function CargarCiudadesEnter(filtro) {
    if (filtro != "") {
        var localParams = { tab: 1 };
        $.ajax({
            type: 'POST',
            url: urlCiudadEnter,
            dataType: 'json',
            data: { strFiltro: filtro },
            success: function (resultados) {
                if (resultados.result == true) {

                    if (resultados.datosCiudad != "") {
                        $('#MPIO_ATENCION').val(resultados.codigoCiudad);
                        console.log(resultados.codigoCiudad);

                        $('#DESP_MPIO_ATENCION').val(resultados.datosCiudad);
                        $('#DESP_BARRIO_ATENCION').focus();

                        $('#BARRIO_CODIGO').val('');
                        $('#BARRIO_LUGECODIGO').val('');
                        $('#DESP_BARRIO_ATENCION').val('');


                    } else {                      
                        $('#MPIO_ATENCION').val('');
                        $('#BARRIO_CODIGO').val('');
                        $('#BARRIO_LUGECODIGO').val('');
                        $('#DESP_BARRIO_ATENCION').val('');
                      

                        if ($.fn.DataTable.isDataTable("#tablaCiudades")) {
                            $("#tablaCiudades").DataTable().destroy();
                            $("#tablaCiudades tbody").empty();
                        }
                        AplicarTablaConBusqueda("tablaCiudades", urlCiudades, parametrosAjaxCiudad, localParams, filtro);
                        $("#ModalCiudades").modal();
                    }


                } else {
                    GestionarAlerta("warning", resultados.datos);
                }
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });

    } else {
        $('#MPIO_ATENCION').val('');
        $('#BARRIO_CODIGO').val('');
        $('#BARRIO_LUGECODIGO').val('');
        $('#DESP_BARRIO_ATENCION').val('');
    }

}

function CargarBarriosComunaEnter(filtro) {
    var codLuge = $("#MPIO_ATENCION").val();
    console.log(codLuge);
    if (filtro != "" && codLuge != "") {
        var localParams = {intLugeCodigo: codLuge };
        $.ajax({
            type: 'POST',
            url: urlBarrioComunaEnter,
            dataType: 'json',
            data: { strFiltro: filtro, intLugeCodigo: codLuge },
            success: function (resultados) {
                if (resultados.result == true) {

                    if (resultados.datosBarrioComuna != "") {
                        $('#DESP_BARRIO_ATENCION').val(resultados.datosBarrioComuna);

                        $("#BARRIO_LUGECODIGO").empty();
                        $("#BARRIO_LUGECODIGO").val(resultados.lugueCodigo);

                        $("#BARRIO_CODIGO").empty();
                        $("#BARRIO_CODIGO").val(resultados.codigo);                

                        $("#DIRECCION").focus();


                    } else {
                       
                        $('#BARRIO_LUGECODIGO').val('');
                        $('#BARRIO_CODIGO').val('');

                        if ($.fn.DataTable.isDataTable("#tablaBarriosComuna")) {
                            $("#tablaBarriosComuna").DataTable().destroy();
                            $("#tablaBarriosComuna tbody").empty();
                        }
                        AplicarTablaConBusqueda("tablaBarriosComuna", urlBarrioComuna, parametrosAjaxBarrioComuna, localParams, filtro);
                        $("#ModalBarrioComuna").modal();
                    }


                } else {
                    GestionarAlerta("warning", resultados.datos);
                }
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });

    } else {
        $('#BARRIO_LUGECODIGO').val('');
        $('#BARRIO_CODIGO').val('');
    }
}

function CargaCiudades() {
    $("#tablaCiudades").DataTable().destroy();
    $("#tablaCiudades tbody").empty();

    AplicarPluginTablaAjax("tablaCiudades", (urlCiudades + "?tab=1"), parametrosAjaxCiudad);
    $("#ModalCiudades").modal();
}

function CargaBarrioComuna() {
    var codLuge = $("#MPIO_ATENCION").val();    

     if ($.fn.DataTable.isDataTable("#tablaBarriosComuna")) {
                            $("#tablaBarriosComuna").DataTable().destroy();
                            $("#tablaBarriosComuna tbody").empty();
                        }
     AplicarPluginTablaAjax("tablaBarriosComuna", (urlBarrioComuna + "?intLugeCodigo=" + codLuge), parametrosAjaxBarrioComuna);
                        $("#ModalBarrioComuna").modal();


}

function CargarCiudad(codigo, descripcion) {
    $("#MPIO_ATENCION").empty();
    $("#MPIO_ATENCION").val(codigo);
    $('#DESP_MPIO_ATENCION').val(descripcion);

    console.log(codigo);

    $("#ModalCiudades").modal("toggle");
}

function CargarBarriosComuna(luge, codigo, descripcion) {

    $("#BARRIO_LUGECODIGO").empty();
    $("#BARRIO_LUGECODIGO").val(luge);

    $("#BARRIO_CODIGO").empty();
    $("#BARRIO_CODIGO").val(codigo);


    $('#DESP_BARRIO_ATENCION').val(descripcion);

    console.log(codigo + "-------" + luge);

    $("#ModalBarrioComuna").modal("toggle");
}

function LimpiarCampos()
{
    LimpiarCamposSocial();
    LimpiarCamposTS();
    LimpiarCamposTR();
    limpiarCamposPersonaSocial();
}

function LimpiarCamposSocial()
{
    $('#MPIO_ATENCION').val('');
    $('#DESP_MPIO_ATENCION').val('');
    $('#BARRIO_CODIGO').val('');
    $('#BARRIO_LUGECODIGO').val('');
    $('#DESP_BARRIO_ATENCION').val('');

    $('#NRO_FICHA').val('');
    $('#DIRECCION').val('');
    $('#TELEFONO').val('');

    $('#txtCasoId').val('');   
    $('#FECHA_TRANSACCION').val('');
    $('#txtCaso').val('');
    $('#OBSERVACION1').val('');
    $('#OBSERVACION2').val('');

    $('#CADUSUARIO').val('');
    
    $("input[name='FICHA_REMITIDA']").prop("checked", false);

    if ($.fn.DataTable.isDataTable("#tablaFichaSocial")) {
        $("#tablaFichaSocial").DataTable().destroy();
        $("#tablaFichaSocial tbody").empty();
    }

    AplicarPluginTablaAjaxFichaSocial("tablaFichaSocial", urlFichaSocial + "?strSitioGrabaEvento=" + SitioGrabaEventoGlobal + "&strEvento=" + EventoGlobal, parametrosFichaSocial);

}

function LimpiarCamposTS()
{
    $('#TIPO_SERVICIO').val('');
    $('#TIPO_SERVICIO').chosen().trigger("chosen:updated");

    $('#spanUsuarioTS').html('    ');
    $('#spanFechaTransaccionTS').html('    ');

    if ($.fn.DataTable.isDataTable("#tablaTipoServisioSocial")) {
        $("#tablaTipoServisioSocial").DataTable().destroy();
        $("#tablaTipoServisioSocial tbody").empty();
    }

    AplicarPluginTablaAjaxFichaSocialTipos("tablaTipoServisioSocial", urlCagarTipos + "?strSitioGrabaEvento=" + SitioGrabaEventoGlobal + "&strEvento=" + EventoGlobal + "&blTs=true"  + "&strNroFicha=" + $('#NRO_FICHA').val(), parametrosAjaxTS, 1);

}

function LimpiarCamposTR() {
    $('#TIPO_REMISION').val('');
    $('#TIPO_REMISION').chosen().trigger("chosen:updated");

    $('#spanUsuarioTR').html('    ');
    $('#spanFechaTransaccionTR').html('    ');




    if ($.fn.DataTable.isDataTable("#tablaRemicionesSocial")) {
        $("#tablaRemicionesSocial").DataTable().destroy();
        $("#tablaRemicionesSocial tbody").empty();
    }

    AplicarPluginTablaAjaxFichaSocialTipos("tablaRemicionesSocial", urlCagarTipos + "?strSitioGrabaEvento=" + SitioGrabaEventoGlobal + "&strEvento=" + EventoGlobal + "&blTs=false" + "&strNroFicha=" + $('#NRO_FICHA').val(), parametrosAjaxTR, 0);

}

function limpiarCamposPersonaSocial()
{
    $('#CONSECUTIVO').val('');
    $('#REGITRADURIA').val('');

    

   // $('#NRO_FICHA_PERSONA').val('');
    $('#TIPO_IDENTIFICACION').val('');
    $('#TIPO_IDENTIFICACION').chosen().trigger("chosen:updated");

    $('#IDENTIFICACION').val('');
    $('#NOMBRES').val('');
    $('#APELLIDOS').val('');

    $('#FECHA_NACE').val('');
    $('#EDAD_PERSONA').val('');
    $('#CODI_GENERO').val('');
    $('#CODI_GENERO').chosen().trigger("chosen:updated");

    $('#CODI_RAZA').val('');
    $('#CODI_RAZA').chosen().trigger("chosen:updated");

    $("input[name='COND_LGTBI']").prop("checked", false);
    $("input[name='COND_DISCAPACIDAD']").prop("checked", false);
    $("input[name='COND_DESPLAZADO']").prop("checked", false);


    $('#OBSERVACIONES').val('');

    $('#cadusuario_Persona').val('');
    $('#fechaTransaccion_Persona').val('');


    if ($.fn.DataTable.isDataTable("#tablaPersonaSocial")) {
        $("#tablaPersonaSocial").DataTable().destroy();
        $("#tablaPersonaSocial tbody").empty();
    }

    AplicarPluginTablaAjaxFichaSocialPersonas("tablaPersonaSocial", urlCagarPersonasSocial + "?strSitioGrabaEvento=" + SitioGrabaEventoGlobal + "&strEvento=" + EventoGlobal + "&strNroFicha=" + $('#NRO_FICHA').val(), parametrosPersonaSocial);


}

function limpiarSelecionar() {

    $('#TIPO_SERVICIO').val('');
    $('#TIPO_SERVICIO').chosen().trigger("chosen:updated");

    $('#spanUsuarioTS').html('    ');
    $('#spanFechaTransaccionTS').html('    ');


    $('#TIPO_REMISION').val('');
    $('#TIPO_REMISION').chosen().trigger("chosen:updated");

    $('#spanUsuarioTR').html('    ');
    $('#spanFechaTransaccionTR').html('    ');

    $('#CONSECUTIVO').val('');
    $('#NRO_FICHA_PERSONA').val('');
    $('#TIPO_IDENTIFICACION').val('');
    $('#TIPO_IDENTIFICACION').chosen().trigger("chosen:updated");

    $('#IDENTIFICACION').val('');
    $('#NOMBRES').val('');
    $('#APELLIDOS').val('');

    $('#FECHA_NACE').val('');
    $('#EDAD_PERSONA').val('');
    $('#CODI_GENERO').val('');
    $('#CODI_GENERO').chosen().trigger("chosen:updated");

    $('#CODI_RAZA').val('');
    $('#CODI_RAZA').chosen().trigger("chosen:updated");

    $("input[name='COND_LGTBI']").prop("checked", false);
    $("input[name='COND_DISCAPACIDAD']").prop("checked", false);
    $("input[name='COND_DESPLAZADO']").prop("checked", false);


    $('#OBSERVACIONES').val('');

    $('#cadusuario_Persona').val('');
    $('#fechaTransaccion_Persona').val('');

}

function CargarDatosFicha(tablaId, object)
{
    limpiarSelecionar();

    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(object).data();

    $('#NRO_FICHA').val(datosTabla['NRO_FICHA']);
    $('#NRO_FICHA_PERSONA').val(datosTabla['NRO_FICHA']);


    
    $('#MPIO_ATENCION').val(datosTabla['MPIO_ATENCION']);
    $('#DESP_MPIO_ATENCION').val(datosTabla['DESP_MPIO_ATENCION']);

    $('#DESP_BARRIO_ATENCION').val(datosTabla['DESP_BARRIO_ATENCION']);
    $('#BARRIO_CODIGO').val(datosTabla['BARRIO_CODIGO']);
    $('#BARRIO_LUGECODIGO').val(datosTabla['BARRIO_LUGECODIGO']);
    console.log(datosTabla['BARRIO_CODIGO']);
    $('#CADUSUARIO').val(datosTabla['CADUSUARIO']);
    $('#DIRECCION').val(datosTabla['DIRECCION']);
    $('#TELEFONO').val(datosTabla['TELEFONO']);

    $('#txtCasoId').val(datosTabla['CODI_EVENTO']);

    if (datosTabla['CODI_EVENTO'] != "") {
        CompletarCaso(datosTabla['CODI_EVENTO']);    
    }

    $('#FECHA_TRANSACCION').val(datosTabla['FECHA_TRANSACCION']);
    $('#OBSERVACION1').val(datosTabla['OBSERVACION1']);
    $('#OBSERVACION2').val(datosTabla['OBSERVACION2']);



    var FICHA_REMITIDA = datosTabla["FICHA_REMITIDA"];


    if (FICHA_REMITIDA == 'SI') {
        $("input[name='FICHA_REMITIDA']").prop("checked", "checked");
    } else {
        $("input[name='FICHA_REMITIDA']").prop("checked", false);
    }

    if ($.fn.DataTable.isDataTable("#tablaTipoServisioSocial")) {
        $("#tablaTipoServisioSocial").DataTable().destroy();
        $("#tablaTipoServisioSocial tbody").empty();
    }


    AplicarPluginTablaAjaxFichaSocialTipos("tablaTipoServisioSocial", urlCagarTipos + "?strSitioGrabaEvento=" + SitioGrabaEventoGlobal + "&strEvento=" + EventoGlobal + "&blTs=true" + "&strNroFicha=" + datosTabla['NRO_FICHA'], parametrosAjaxTS, 1);

    if ($.fn.DataTable.isDataTable("#tablaRemicionesSocial")) {
        $("#tablaRemicionesSocial").DataTable().destroy();
        $("#tablaRemicionesSocial tbody").empty();
    }

    AplicarPluginTablaAjaxFichaSocialTipos("tablaRemicionesSocial", urlCagarTipos + "?strSitioGrabaEvento=" + SitioGrabaEventoGlobal + "&strEvento=" + EventoGlobal + "&blTs=false" + "&strNroFicha=" + datosTabla['NRO_FICHA'], parametrosAjaxTR, 0);



    if ($.fn.DataTable.isDataTable("#tablaPersonaSocial")) {
        $("#tablaPersonaSocial").DataTable().destroy();
        $("#tablaPersonaSocial tbody").empty();
    }

    AplicarPluginTablaAjaxFichaSocialPersonas("tablaPersonaSocial", urlCagarPersonasSocial + "?strSitioGrabaEvento=" + SitioGrabaEventoGlobal + "&strEvento=" + EventoGlobal + "&strNroFicha=" + datosTabla['NRO_FICHA'], parametrosPersonaSocial);

}

function CargarDatosFichaTS(tablaId, object)
{
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(object).data();

    $('#spanUsuarioTS').html(datosTabla["CADUSUARIO"]);
    $('#spanFechaTransaccionTS').html(datosTabla["FECHA_TRANSACCION"]);
}

function CargarDatosFichaTR(tablaId, object) {
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(object).data();

    $('#spanUsuarioTR').html(datosTabla["CADUSUARIO"]);
    $('#spanFechaTransaccionTR').html(datosTabla["FECHA_TRANSACCION"]);
}


function CargarDatosPersonaSocial(tablaId, object) {
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(object).data();

    $('#CONSECUTIVO').val(datosTabla["CONSECUTIVO"]);
    $('#TIPO_IDENTIFICACION').val(datosTabla["CODIGO_TIPO_IDENTIFICACION"]);    
    $('#TIPO_IDENTIFICACION').chosen().trigger("chosen:updated");


    $('#IDENTIFICACION').val(datosTabla["IDENTIFICACION"]);
    $('#NOMBRES').val(datosTabla["NOMBRES"]);

    $('#APELLIDOS').val(datosTabla["APELLIDOS"]);
    $('#FECHA_NACE').val(datosTabla["FECHA_NACE"]);
    $('#EDAD_PERSONA').val(datosTabla["EDAD_PERSONA"]);

    $('#CODI_GENERO').val(datosTabla["CODI_GENERO"]);
    $('#CODI_GENERO').chosen().trigger("chosen:updated");


    $('#CODI_RAZA').val(datosTabla["CODI_RAZA"]);
    $('#CODI_RAZA').chosen().trigger("chosen:updated");


    var COND_LGTBI = datosTabla["COND_LGTBI"];
    console.log(COND_LGTBI);
    if (COND_LGTBI == 'SI') {
        $("input[name='COND_LGTBI']").prop("checked", "checked");
    } else {
        $("input[name='COND_LGTBI']").prop("checked", false);
    }


    var COND_DISCAPACIDAD = datosTabla["COND_DISCAPACIDAD"];

    if (COND_DISCAPACIDAD == 'SI') {
        $("input[name='COND_DISCAPACIDAD']").prop("checked", "checked");
    } else {
        $("input[name='COND_DISCAPACIDAD']").prop("checked", false);
    }

    var COND_DESPLAZADO = datosTabla["COND_DESPLAZADO"];

    if (COND_DESPLAZADO == 'SI') {
        $("input[name='COND_DESPLAZADO']").prop("checked", "checked");
    } else {
        $("input[name='COND_DESPLAZADO']").prop("checked", false);
    }

  
    $('#OBSERVACIONES').val(datosTabla["OBSERVACIONES"]);
    $('#cadusuario_Persona').val(datosTabla["CADUSUARIO"]);
    $('#fechaTransaccion_Persona').val(datosTabla["FECHA_TRANSACCION"]);






}

function crearFichaSocialTS() {

    if ($('#NRO_FICHA').val() == "") {
        GestionarAlerta("warning", "Debe seleccionar o crear una ficha.");
        return;
    }

    if ($('#TIPO_SERVICIO').val() == "") {
        GestionarAlerta("warning", "Selecione un tipo servicio");
        return;
    }

    var datos = new Array();

    datos.push(SitioGrabaEventoGlobal);
    datos.push(EventoGlobal);
    datos.push($('#NRO_FICHA').val());
    datos.push($('#TIPO_SERVICIO').val());

    
    $.ajax({
        method: "POST",
        url: urlCrearFichaSocialTS,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strDatos:datos},

        success: function (data) {

            if (data.result) {
                LimpiarCamposTS();
                GestionarAlerta("success", data.datos);

            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
    });

}

function crearFichaSocialTR() {

    if ($('#NRO_FICHA').val() == "") {
        GestionarAlerta("warning", "Debe seleccionar o crear una ficha.");
        return;
    }

    if ($('#TIPO_REMISION').val() == "") {
        GestionarAlerta("warning", "Selecione un tipo remisión");
        return;
    }

    var datos = new Array();

    datos.push(SitioGrabaEventoGlobal);
    datos.push(EventoGlobal);
    datos.push($('#NRO_FICHA').val());
    datos.push($('#TIPO_REMISION').val());


    $.ajax({
        method: "POST",
        url: urlCrearFichaSocialTR,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strDatos: datos },

        success: function (data) {

            if (data.result) {
                LimpiarCamposTR();
                GestionarAlerta("success", data.datos);

            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
    });

}

function EliminarTipoServicio(sitioGraba, evento, nroFicha, tipoServicio, despTipoServicio) {
    console.log("entre");
    var datos = { intSitioGraba: sitioGraba, intNumeroEvento: evento, strNroFicha: nroFicha, strTipoServicio: tipoServicio };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Tipo Servicio: " + despTipoServicio,
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
                url: urlEliminarFichaSocialTS,
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
                    LimpiarCamposTS();
                    GestionarAlerta("success", resultado.datos);
                   
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}

function EliminarRemisiones(sitioGraba, evento, nroFicha, tipoRemision, despTipoRemision) {
    var datos = { intSitioGraba: sitioGraba, intNumeroEvento: evento, strNroFicha: nroFicha, strTipoRemision: tipoRemision };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Tipo Remision: " + despTipoRemision,
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
                url: urlEliminarFichaSocialTR,
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
                    LimpiarCamposTR();
                    GestionarAlerta("success", resultado.datos);

                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}

function EliminarPersonaSocial(sitioGraba, evento, nroFicha, consecutivo, identificacion) {
    console.log("entre");
    var datos = { intSitioGraba: sitioGraba, intNumeroEvento: evento, strNroFicha: nroFicha, intConsecutivo: consecutivo };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Identificación: " + identificacion,
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
                url: urlEliminarPersonaSocial,
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
                    limpiarCamposPersonaSocial();
                    GestionarAlerta("success", resultado.datos);

                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}


function InicializarElementos(opcion) {
    switch (opcion) {
        case "CREAR":
            //AplicarChosen();
            AplicarDateTimePlugin();
            break;
    }
}

function AplicarDateTimePlugin() {
    $(".FechaHora").datetimepicker({
        format: 'DD/MM/YYYY HH:mm:ss',
        showClear: true,
    });
}


function CargarPedido() {
    $.ajax({
        type: 'POST',
        url: urlDatosPedido,
        dataType: 'json',
        data: { strSitioGrabaEvento: SitioGrabaEventoGlobal, strNumeroEvento: EventoGlobal },
        success: function (resultados) {
            if (resultados.result == true) {
                if (resultados.datosHechos["p_MPIO_SOCIAL"] != 'null') {
                    $('#MPIO_ATENCION').val(resultados.datosHechos["p_MPIO_SOCIAL"]);
                }
                if (resultados.datosHechos["p_DSP_MPIOSOCIAL"] != 'null') {
                    $('#DESP_MPIO_ATENCION').val(resultados.datosHechos["p_DSP_MPIOSOCIAL"]);
                }
                if (resultados.datosHechos["p_BARRIO_CODIGO_SOCIAL"] != 'null') {
                    $('#BARRIO_CODIGO').val(resultados.datosHechos["p_BARRIO_CODIGO_SOCIAL"]);
                }

                if (resultados.datosHechos["p_BARRIO_LUGECODIGO_SOCIAL"] != 'null') {
                    $('#BARRIO_LUGECODIGO').val(resultados.datosHechos["p_BARRIO_LUGECODIGO_SOCIAL"]);
                }
                if (resultados.datosHechos["p_DSP_BARRIOSOCIAL"] != 'null') {
                    $('#DESP_BARRIO_ATENCION').val(resultados.datosHechos["p_DSP_BARRIOSOCIAL"]);
                }
                if (resultados.datosHechos["p_DIRECCION_SOCIAL"] != 'null') {
                    $('#DIRECCION').val(resultados.datosHechos["p_DIRECCION_SOCIAL"]);
                }

                if (resultados.datosHechos["p_TELEFONO_SOCIAL"] != 'null') {
                    $('#TELEFONO').val(resultados.datosHechos["p_TELEFONO_SOCIAL"]);
                }

                if (resultados.datosHechos["p_OBSERVACIONES_SOCIAL"] != 'null') {
                    $('#OBSERVACION1').val(resultados.datosHechos["p_OBSERVACIONES_SOCIAL"]);
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