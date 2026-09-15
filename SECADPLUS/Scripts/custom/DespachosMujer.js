var parametrosPerRelacionadasMj = [
                       { "data": "Dummy", "orderable": false },
                       { "data": "Boton", "orderable": false },
                       { "data": "PERSATEN_CONSECUTIVO" },
                       { "data": "TIPO_IDENTIFICACION" },
                       { "data": "IDENTIFICACION" },
                       { "data": "NOMBRES" },
                       { "data": "APELLIDOS" },
                       { "data": "FECHA_NACE" },
                       { "data": "GENERO", "orderable": false },
                       { "data": "RAZA", "orderable": false },
                       { "data": "CODI_GENERO", "visible": false },
                       { "data": "CODI_RAZA", "visible": false },
                       { "data": "TIPO_INTERVINIENTE", "visible": false },
                       { "data": "CALIDAD_REPORTANTE", "visible": false },
                       { "data": "CADUSUARIO", "visible": false },
                       { "data": "FECHA_TRANSACCION", "visible": false },
                       { "data": "MEDIO_CONOCIMIENTO", "visible": false },
                       { "data": "NRO_FICHA", "visible": false },
                       { "data": "EDAD_PERSONA", "visible": false },
                       { "data": "TIPO_DISCAPACIDAD", "visible": false },
                       { "data": "TIPO_VICT_CONFLICTO", "visible": false },
                       { "data": "TIPO_ESPECTATIVA", "visible": false },
                       { "data": "COND_LGTBI", "visible": false },
                       { "data": "COND_DISCAPACIDAD", "visible": false },
                       { "data": "COND_DESPLAZADO", "visible": false },
                       { "data": "NIVEL_EDUCATIVO", "visible": false },
                       { "data": "ACTIVIDAD_ECONOMICA", "visible": false },
                       { "data": "PARTICIP_ORGANIZACION", "visible": false },
                       { "data": "NOMBRE_ORGANIZACION", "visible": false },
                       { "data": "REGIMEN_SALUD", "visible": false },
                       { "data": "MPIO_RESIDENCIA", "visible": false },

                       { "data": "COZO_LUGECODIGO_RESIDENCIA", "visible": false },
                       { "data": "COZO_CODIGO_RESIDENCIA", "visible": false },
                       { "data": "BARRIO_CODIGO_RESIDENCIA", "visible": false },
                       { "data": "BARRIO_LUGECODIGO_RESIDENCIA", "visible": false },

                       { "data": "DIRECCION_RESIDENCIA", "visible": false },
                       { "data": "DSP_CIUDAD_RESIDENCIA", "visible": false },
                       { "data": "dsp_comuna_RESIDENCIA", "visible": false },
                       { "data": "dsp_barrio_RESIDENCIA", "visible": false },
                       { "data": "TELEFONO1", "visible": false },
                       { "data": "TELEFONO2", "visible": false },
                       { "data": "TIPO_RELACION_AGRESOR", "visible": false },
                       { "data": "RELACION_FAMILIAR_AGRESOR", "visible": false },
                       { "data": "RELACION_PARTICULAR_AGRESOR", "visible": false },
                       { "data": "GRUPO_AGRESOR", "visible": false },
                       { "data": "MECANISMO_ARMA", "visible": false },
                       { "data": "DESCRIPCION_GRUPO", "visible": false },
                       { "data": "MPIO_HECHOS", "visible": false },

                       { "data": "COZO_LUGECODIGO_HECHOS", "visible": false },
                       { "data": "COZO_CODIGO_HECHOS", "visible": false },
                       { "data": "BARRIO_CODIGO_HECHOS", "visible": false },
                       { "data": "BARRIO_LUGECODIGO_HECHOS", "visible": false },

                       { "data": "DSP_MPIOHECOS", "visible": false },
                       { "data": "DSP_BARRIOHECHOS", "visible": false },
                       { "data": "DSP_COMUNAHECHOS", "visible": false },

                       { "data": "DIRECCION_HECHOS", "visible": false },
                       { "data": "LUGAR_OCURRENCIA", "visible": false },
                       { "data": "NOMBRE_SITIO", "visible": false },

                       { "data": "OBSERVACIONES", "visible": false },

];

var parametrosRAEMj = [
                       { "data": "Dummy", "orderable": false },
                       { "data": "Boton", "orderable": false },
                       { "data": "CONSECUTIVO_RAE" },
                       { "data": "DESC_TIPO_SERVICIO_RAE" },
                       { "data": "OBSERVACION_RAE", "visible": false },
];

var parametrosRMMj = [
                       { "data": "Dummy", "orderable": false },
                       { "data": "Boton", "orderable": false },
                       { "data": "CONSECUTIVO_RM" },
                       { "data": "DESC_TIPO_SERVICIO_RM" },
                       { "data": "OBSERVACION_RM", "visible": false },
];

var parametrosROIMj = [
                       { "data": "Dummy", "orderable": false },
                       { "data": "Boton", "orderable": false },
                       { "data": "CONSECUTIVO_ROI" },
                       { "data": "DESC_TIPO_SERVICIO_ROI" },
                       { "data": "OBSERVACION_ROI", "visible": false },
];

var parametrosAjaxCiudad = [
       { "data": "Dummy", "orderable": false },
       { "data": "Boton", "orderable": false },
       { "data": "CODIGO" },
       { "data": "DESCRIPCION" },
];
var parametrosAjaxComunas = [
       { "data": "Dummy", "orderable": false },
       { "data": "Boton", "orderable": false },
       { "data": "CODIGO" },
       { "data": "DESCRIPCION" },
];

var parametrosAjaxBarriosComunas = [
       { "data": "Dummy", "orderable": false },
       { "data": "Boton", "orderable": false },
       { "data": "CODIGO" },
       { "data": "DESCRIPCION" },
];

$(document).ready(function () {
    CambiarVisibilidadMenu();

    AplicarPluginTablaAjaxPerRelacionadosMj("tablaPerRelacionadas", urlPerRelacionadasMj + "?strSitioGrabaEvento=" + SitioGrabaEventoGlobal + "&strEvento=" + EventoGlobal, parametrosPerRelacionadasMj);

    AplicarPluginTablaAjaxRAEMujer("tablaRAE", urlRAEMujer, parametrosRAEMj);
    AplicarPluginTablaAjaxRMMujer("tablaRM", urlRMMujer, parametrosRMMj);
    AplicarPluginTablaAjaxROIMujer("tablaROI", urlROIMujer, parametrosROIMj);



    $("#IDENTIFICACION").keypress(function (e) {
        if (e.which == 13) {
       
            CargarDatosRegisEnter($('#TIPO_IDENTIFICACION').val(), $(this).val());
        }
    });
    //Residencia
    $("#MPIO_RESIDENCIA").keypress(function (e) {
        if (e.which == 13) {
            CargarCiudadesEnter($(this).val());
        }
    });

    $("#dsp_comuna_RESIDENCIA").keypress(function (e) {
        if (e.which == 13) {
            CargarComunaEnter($(this).val());
        }
    });


    $("#dsp_barrio_RESIDENCIA").keypress(function (e) {
        if (e.which == 13) {
            CargarBarriosEnter($(this).val());
        }
    });

    //hechos
    $("#MPIO_HECHOS").keypress(function (e) {
        if (e.which == 13) {
            CargarCiudadesHechosEnter($(this).val());
        }
    });

    $("#DSP_COMUNAHECHOS").keypress(function (e) {
        if (e.which == 13) {
            CargarComunaHechosEnter($(this).val());
        }
    });

    $("#DSP_BARRIOHECHOS").keypress(function (e) {
        if (e.which == 13) {
            CargarBarriosHechosEnter($(this).val());
        }
    });
});


//Residencia
function CargarCiudadesEnter(filtro)
{
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
                        $('#DSP_CIUDAD_RESIDENCIA').val(resultados.datosCiudad);
                        $('#dsp_comuna_RESIDENCIA').focus();

                        $('#dsp_comuna_RESIDENCIA').val('');
                        $('#COZO_CODIGO_RESIDENCIA').val('');
                        $('#COZO_LUGECODIGO_RESIDENCIA').val('');

                        $("#BARRIO_LUGECODIGO_RESIDENCIA").val('');
                        $("#BARRIO_CODIGO_RESIDENCIA").val('');
                        $("#dsp_barrio_RESIDENCIA").val('');

                    } else {
                        $('#DSP_CIUDAD_RESIDENCIA').val('');

                        $('#dsp_comuna_RESIDENCIA').val('');
                        $('#COZO_CODIGO_RESIDENCIA').val('');
                        $('#COZO_LUGECODIGO_RESIDENCIA').val('');

                        $("#BARRIO_LUGECODIGO_RESIDENCIA").val('');
                        $("#BARRIO_CODIGO_RESIDENCIA").val('');
                        $("#dsp_barrio_RESIDENCIA").val('');

                        if ($.fn.DataTable.isDataTable("#tablaCiudades")) {
                            $("#tablaCiudades").DataTable().destroy();
                            $("#tablaCiudades tbody").empty();
                        }
                        AplicarTablaConBusqueda("tablaCiudades", urlCargarCiudades, parametrosAjaxCiudad, localParams, filtro);
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
        $('#DSP_CIUDAD_RESIDENCIA').val('');

        $('#dsp_comuna_RESIDENCIA').val('');
        $('#COZO_CODIGO_RESIDENCIA').val('');
        $('#COZO_LUGECODIGO_RESIDENCIA').val('');

        $("#BARRIO_LUGECODIGO_RESIDENCIA").val('');
        $("#BARRIO_CODIGO_RESIDENCIA").val('');
        $("#dsp_barrio_RESIDENCIA").val('');
    }

}

function CargarComunaEnter(filtro)
{
    var codLuge = $("#MPIO_RESIDENCIA").val();
    if (filtro != "" && codLuge != "") {
        var localParams = { lugeCodigo: codLuge,tab:1 };
        $.ajax({
            type: 'POST',
            url: urlComunaEnter,
            dataType: 'json',
            data: { strFiltro: filtro, lugeCodigo: codLuge },
            success: function (resultados) {
                if (resultados.result == true) {

                    if (resultados.datosComuna != "") {
                        $('#dsp_comuna_RESIDENCIA').val(resultados.datosComuna);
                                                
                            $("#COZO_LUGECODIGO_RESIDENCIA").empty();
                            $("#COZO_LUGECODIGO_RESIDENCIA").val(resultados.lugueCodigo);

                            $("#COZO_CODIGO_RESIDENCIA").empty();
                            $("#COZO_CODIGO_RESIDENCIA").val(resultados.codigo);

                            $("#dsp_comuna_RESIDENCIA").empty();
                            $('#dsp_comuna_RESIDENCIA').val(resultados.datosComuna);                          

                            $("#BARRIO_LUGECODIGO_RESIDENCIA").val('');
                            $("#BARRIO_CODIGO_RESIDENCIA").val('');
                            $("#dsp_barrio_RESIDENCIA").val('');

                            $("#dsp_barrio_RESIDENCIA").focus();


                    } else {
                      //  $('#dsp_comuna_RESIDENCIA').val('');
                        $('#COZO_CODIGO_RESIDENCIA').val('');
                        $('#COZO_LUGECODIGO_RESIDENCIA').val('');
                    
                        $("#BARRIO_LUGECODIGO_RESIDENCIA").val('');
                        $("#BARRIO_CODIGO_RESIDENCIA").val('');
                        $("#dsp_barrio_RESIDENCIA").val('');

                        if ($.fn.DataTable.isDataTable("#tablaComunas")) {
                            $("#tablaComunas").DataTable().destroy();
                            $("#tablaComunas tbody").empty();
                        }
                        AplicarTablaConBusqueda("tablaComunas", urlCargaComunas, parametrosAjaxCiudad, localParams, filtro);
                        $("#ModalComunas").modal();
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
        $('#dsp_comuna_RESIDENCIA').val('');
        $('#COZO_CODIGO_RESIDENCIA').val('');
        $('#COZO_LUGECODIGO_RESIDENCIA').val('');

        $("#BARRIO_LUGECODIGO_RESIDENCIA").val('');
        $("#BARRIO_CODIGO_RESIDENCIA").val('');
        $("#dsp_barrio_RESIDENCIA").val('');
    }

}

function CargarBarriosEnter(filtro) {

    var codLuge = $("#COZO_LUGECODIGO_RESIDENCIA").val();
    var cod = $("#COZO_CODIGO_RESIDENCIA").val();

    if (filtro != "" && codLuge != "" && cod != "") {
        var localParams = {cozoLugeCodigo:codLuge,cozoCodigo:cod, tab: 1 };
        $.ajax({
            type: 'POST',
            url: urlBarriosEnter,
            dataType: 'json',
            data: { strFiltro: filtro, cozoLugeCodigo: codLuge, cozoCodigo: cod },
            success: function (resultados) {
                if (resultados.result == true) {

                    if (resultados.datosBarrio != "") {
                        $('#dsp_barrio_RESIDENCIA').val(resultados.datosBarrio);

                        $("#BARRIO_LUGECODIGO_RESIDENCIA").empty();
                        $("#BARRIO_LUGECODIGO_RESIDENCIA").val(resultados.lugueCodigo);

                        $("#BARRIO_CODIGO_RESIDENCIA").empty();
                        $("#BARRIO_CODIGO_RESIDENCIA").val(resultados.codigo);         

                        $("#DIRECCION_RESIDENCIA").focus();


                    } else {

                        $("#BARRIO_LUGECODIGO_RESIDENCIA").val('');
                        $("#BARRIO_CODIGO_RESIDENCIA").val('');

                     
                        if ($.fn.DataTable.isDataTable("#tablaBarriosComunas")) {
                            $("#tablaBarriosComunas").DataTable().destroy();
                            $("#tablaBarriosComunas tbody").empty();
                        }
                        AplicarTablaConBusqueda("tablaBarriosComunas", urlCargaBarriosComunas, parametrosAjaxBarriosComunas, localParams, filtro);
                        $("#ModalBarriosComunas").modal();
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
        $("#BARRIO_LUGECODIGO_RESIDENCIA").val('');
        $("#BARRIO_CODIGO_RESIDENCIA").val('');

    }

}
//hechos
function CargarCiudadesHechosEnter(filtro) {
    if (filtro != "") {
        var localParams = { tab: 2 };
        $.ajax({
            type: 'POST',
            url: urlCiudadEnter,
            dataType: 'json',
            data: { strFiltro: filtro },
            success: function (resultados) {
                if (resultados.result == true) {

                    if (resultados.datosCiudad != "") {                     

                        $("#DSP_MPIOHECOS").val(resultados.datosCiudad);
                        $('#DSP_COMUNAHECHOS').focus();

                        $("#COZO_LUGECODIGO_HECHOS").val('');
                        $("#COZO_CODIGO_HECHOS").val('');
                        $("#DSP_COMUNAHECHOS").val('');
                       

                        $("#BARRIO_LUGECODIGO_HECHOS").val('');
                        $("#BARRIO_CODIGO_HECHOS").val('');
                        $("#DSP_BARRIOHECHOS").val('');
                       

                    } else {
                        $('#DSP_MPIOHECOS').val('');
                        $("#COZO_LUGECODIGO_HECHOS").val('');
                        $("#COZO_CODIGO_HECHOS").val('');
                        $("#DSP_COMUNAHECHOS").val('');


                        $("#BARRIO_LUGECODIGO_HECHOS").val('');
                        $("#BARRIO_CODIGO_HECHOS").val('');
                        $("#DSP_BARRIOHECHOS").val('');
                       

                        if ($.fn.DataTable.isDataTable("#tablaCiudades")) {
                            $("#tablaCiudades").DataTable().destroy();
                            $("#tablaCiudades tbody").empty();
                        }
                        AplicarTablaConBusqueda("tablaCiudades", urlCargarCiudades, parametrosAjaxCiudad, localParams, filtro);
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
        $('#DSP_MPIOHECOS').val('');
        $("#COZO_LUGECODIGO_HECHOS").val('');
        $("#COZO_CODIGO_HECHOS").val('');
        $("#DSP_COMUNAHECHOS").val('');


        $("#BARRIO_LUGECODIGO_HECHOS").val('');
        $("#BARRIO_CODIGO_HECHOS").val('');
        $("#DSP_BARRIOHECHOS").val('');
    }

}

function CargarComunaHechosEnter(filtro) {
    var codLuge = $("#MPIO_HECHOS").val();
    if (filtro != "" && codLuge != "") {
        var localParams = { lugeCodigo: codLuge, tab: 2 };
        $.ajax({
            type: 'POST',
            url: urlComunaEnter,
            dataType: 'json',
            data: { strFiltro: filtro, lugeCodigo: codLuge },
            success: function (resultados) {
                if (resultados.result == true) {

                    if (resultados.datosComuna != "") {
                        $('#DSP_COMUNAHECHOS').val(resultados.datosComuna);
                        $("#COZO_LUGECODIGO_HECHOS").empty();                       
                        $("#COZO_LUGECODIGO_HECHOS").val(resultados.lugueCodigo);
                        $("#COZO_CODIGO_HECHOS").empty();
                        $("#COZO_CODIGO_HECHOS").val(resultados.codigo);                    
                        $("#DSP_BARRIOHECHOS").focus();
                        $("#BARRIO_LUGECODIGO_HECHOS").val('');
                        $("#BARRIO_CODIGO_HECHOS").val('');
                        $("#DSP_BARRIOHECHOS").val('');

                    } else {
                       
                        $('#COZO_LUGECODIGO_HECHOS').val('');
                        $('#COZO_CODIGO_HECHOS').val('');

                        $("#BARRIO_LUGECODIGO_HECHOS").val('');
                        $("#BARRIO_CODIGO_HECHOS").val('');
                        $("#DSP_BARRIOHECHOS").val('');

                        if ($.fn.DataTable.isDataTable("#tablaComunas")) {
                            $("#tablaComunas").DataTable().destroy();
                            $("#tablaComunas tbody").empty();
                        }
                        AplicarTablaConBusqueda("tablaComunas", urlCargaComunas, parametrosAjaxCiudad, localParams, filtro);
                        $("#ModalComunas").modal();
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
        $('#DSP_COMUNAHECHOS').val('');
        $('#COZO_LUGECODIGO_HECHOS').val('');
        $('#COZO_CODIGO_HECHOS').val('');

        $("#BARRIO_LUGECODIGO_HECHOS").val('');
        $("#BARRIO_CODIGO_HECHOS").val('');
        $("#DSP_BARRIOHECHOS").val('');
    }

}

function CargarBarriosHechosEnter(filtro) {
   

    var codLuge = $("#COZO_LUGECODIGO_HECHOS").val();
    var cod = $("#COZO_CODIGO_HECHOS").val();

    if (filtro != "" && codLuge != "" && cod != "") {
        var localParams = { cozoLugeCodigo: codLuge, cozoCodigo: cod, tab: 2 };
        $.ajax({
            type: 'POST',
            url: urlBarriosEnter,
            dataType: 'json',
            data: { strFiltro: filtro, cozoLugeCodigo: codLuge, cozoCodigo: cod },
            success: function (resultados) {
                if (resultados.result == true) {

                    if (resultados.datosBarrio != "") {
                        $('#DSP_BARRIOHECHOS').val(resultados.datosBarrio);



                        $("#BARRIO_LUGECODIGO_HECHOS").empty();
                        $("#BARRIO_LUGECODIGO_HECHOS").val(resultados.lugueCodigo);

                        $("#BARRIO_CODIGO_HECHOS").empty();
                        $("#BARRIO_CODIGO_HECHOS").val(resultados.codigo);

                        $("#DIRECCION_HECHOS").focus();


                    } else {

                        $("#BARRIO_LUGECODIGO_HECHOS").val('');
                        $("#BARRIO_CODIGO_HECHOS").val('');


                        if ($.fn.DataTable.isDataTable("#tablaBarriosComunas")) {
                            $("#tablaBarriosComunas").DataTable().destroy();
                            $("#tablaBarriosComunas tbody").empty();
                        }
                        AplicarTablaConBusqueda("tablaBarriosComunas", urlCargaBarriosComunas, parametrosAjaxBarriosComunas, localParams, filtro);
                        $("#ModalBarriosComunas").modal();
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
        $("#BARRIO_LUGECODIGO_HECHOS").val('');
        $("#BARRIO_CODIGO_HECHOS").val('');

    }

}


function CargarDatosRegisEnter(tipoIdentificacion,numeroIdentificacion) {
    console.log(tipoIdentificacion + "    " + numeroIdentificacion);

    if (numeroIdentificacion != '' && tipoIdentificacion != '' && tipoIdentificacion != '00' && tipoIdentificacion != 'SD') {

        $.ajax({
            type: 'POST',
            url: urlDatosRegis,
            dataType: 'json',
            data: { strTipoIdentificacion: tipoIdentificacion, strIdentificacion: numeroIdentificacion },
            success: function (resultados) {
                if (resultados.result == true) {                


                    if (resultados.datosAtendidos["p_PERSATEN_SITIOGRABACION"] != "null") {
                        $('#PERSATEN_SITIOGRABACION').val(resultados.datosAtendidos["p_PERSATEN_SITIOGRABACION"]);
                    }

                    //if (resultados.datosAtendidos["p_PERSATEN_CONSECUTIVO"] != "null") {
                    //    $('#PERSATEN_CONSECUTIVO').val(resultados.datosAtendidos["p_PERSATEN_CONSECUTIVO"]);
                    //}
                    
                    if (resultados.datosAtendidos["p_apellidos"] != "null") {
                        $('#APELLIDOS').val(resultados.datosAtendidos["p_apellidos"]);

                    }

                    if (resultados.datosAtendidos["p_nombres"] != "null") {
                        $('#NOMBRES').val(resultados.datosAtendidos["p_nombres"]);
                    }

                    if (resultados.datosAtendidos["p_fecha_nace"] != "null") {
                        $('#FECHA_NACE').val(resultados.datosAtendidos["p_fecha_nace"]);

                    }

                    if (resultados.datosAtendidos["p_codi_genero"] != "null") {
                        $('#CODI_GENERO').val(resultados.datosAtendidos["p_codi_genero"]);
                        $('#CODI_GENERO').chosen().trigger("chosen:updated");

                    }

                    if (resultados.datosAtendidos["p_codi_raza"] != "null") {
                        $('#CODI_RAZA').val(resultados.datosAtendidos["p_codi_raza"]);
                        $('#CODI_RAZA').chosen().trigger("chosen:updated");

                    }

                } else {
                    GestionarAlerta("warning", resultados.datos);
                }
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });

       
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

function CargarDatosPerRelacionaMj(tablaId, object) {
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(object).data();

    $('#REGITRADURIA').val('');
    if (datosTabla["CODIGO_TIPO_IDENTIFICACION"] != '00' && datosTabla["CODIGO_TIPO_IDENTIFICACION"] != 'SD') {
        $.ajax({
            type: 'POST',
            url: urlDatosRegistraduria,
            dataType: 'json',
            data: { strTipoIdentificacion: datosTabla["CODIGO_TIPO_IDENTIFICACION"], strIdentificacion: datosTabla["IDENTIFICACION"] },
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

    $('#PERSATEN_SITIOGRABACION').val(datosTabla["PERSATEN_SITIOGRABACION"]);
    $('#PERSATEN_CONSECUTIVO').val(datosTabla["PERSATEN_CONSECUTIVO"]);
    $('#EVEN_SITIOGRABACION').val(datosTabla["EVEN_SITIOGRABACION"]);
    $('#EVEN_NUMEEVENTO').val(datosTabla["EVEN_NUMEEVENTO"]);

    CargarRAEMujer(datosTabla["PERSATEN_SITIOGRABACION"], datosTabla["PERSATEN_CONSECUTIVO"], datosTabla["EVEN_SITIOGRABACION"], datosTabla["EVEN_NUMEEVENTO"]);
    CargarRMMujer(datosTabla["PERSATEN_SITIOGRABACION"], datosTabla["PERSATEN_CONSECUTIVO"], datosTabla["EVEN_SITIOGRABACION"], datosTabla["EVEN_NUMEEVENTO"]);
    CargarROIMujer(datosTabla["PERSATEN_SITIOGRABACION"], datosTabla["PERSATEN_CONSECUTIVO"], datosTabla["EVEN_SITIOGRABACION"], datosTabla["EVEN_NUMEEVENTO"]);

    $('#PERSATEN_CONSECUTIVO').val(datosTabla["PERSATEN_CONSECUTIVO"]);

    $('#TIPO_IDENTIFICACION').val(datosTabla["CODIGO_TIPO_IDENTIFICACION"]);
    $('#TIPO_IDENTIFICACION').chosen().trigger("chosen:updated");

    $('#IDENTIFICACION').val(datosTabla["IDENTIFICACION"]);
    $('#NOMBRES').val(datosTabla["NOMBRES"]);
    $('#APELLIDOS').val(datosTabla["APELLIDOS"]);
    $('#FECHA_NACE').val(datosTabla["FECHA_NACE"]);

    $('#CODI_GENERO').val(datosTabla["CODI_GENERO"]);
    $('#CODI_GENERO').chosen().trigger("chosen:updated");

    $('#CODI_RAZA').val(datosTabla["CODI_RAZA"]);
    $('#CODI_RAZA').chosen().trigger("chosen:updated");

    var TIPO_INTERVINIENTE = datosTabla["TIPO_INTERVINIENTE"];

    if (TIPO_INTERVINIENTE == "VP") {
        $('#ckVictima').trigger("click");
        $('#ckVictima').trigger("click");
    } else if (TIPO_INTERVINIENTE == "VS") {
        $('#ckVictimaSegunda').trigger("click");
        $('#ckVictimaSegunda').trigger("click");
    } else if (TIPO_INTERVINIENTE == "RP") {
        $('#ckReporte').trigger("click");
        $('#ckReporte').trigger("click");
    } else if (TIPO_INTERVINIENTE == "AG") {
        $('#ckAgresor').trigger("click");
        $('#ckAgresor').trigger("click");
    }

    var CALIDAD_REPORTANTE = datosTabla["CALIDAD_REPORTANTE"];

    if (CALIDAD_REPORTANTE == "VP") {
        $('#ckReportanteVP').trigger("click");
        $('#ckReportanteVP').trigger("click");
    } else if (CALIDAD_REPORTANTE == "VS") {
        $('#ckReportanteVS').trigger("click");
        $('#ckReportanteVS').trigger("click");
    } else if (CALIDAD_REPORTANTE == "FM") {
        $('#ckReportanteFM').trigger("click");
        $('#ckReportanteFM').trigger("click");
    } else if (CALIDAD_REPORTANTE == "CN") {
        $('#ckReportanteCN').trigger("click");
        $('#ckReportanteCN').trigger("click");
    } else if (CALIDAD_REPORTANTE == "DC") {
        $('#ckReportanteDC').trigger("click");
        $('#ckReportanteDC').trigger("click");
    }

    $('#spanUsuario').html(datosTabla["CADUSUARIO"]);

    var fechaTransacion = datosTabla["FECHA_TRANSACCION"].split(' ');
    $('#spanFechaTransacion').html(fechaTransacion[0]);

    $('#MEDIO_CONOCIMIENTO').val(datosTabla["MEDIO_CONOCIMIENTO"]);
    $('#MEDIO_CONOCIMIENTO').chosen().trigger("chosen:updated");

    $('#NRO_FICHA').val(datosTabla["NRO_FICHA"]);
    $('#EDAD_PERSONA').val(datosTabla["EDAD_PERSONA"]);

    $('#TIPO_DISCAPACIDAD').val(datosTabla["TIPO_DISCAPACIDAD"]);
    $('#TIPO_DISCAPACIDAD').chosen().trigger("chosen:updated");

    $('#TIPO_VICT_CONFLICTO').val(datosTabla["TIPO_VICT_CONFLICTO"]);
    $('#TIPO_VICT_CONFLICTO').chosen().trigger("chosen:updated");

    $('#TIPO_ESPECTATIVA').val(datosTabla["TIPO_ESPECTATIVA"]);
    $('#TIPO_ESPECTATIVA').chosen().trigger("chosen:updated");

    var COND_LGTBI = datosTabla["COND_LGTBI"];

    if (COND_LGTBI == 'S') {
        $("input[name='COND_LGTBI']").prop("checked", "checked");
    } else {
        $("input[name='COND_LGTBI']").prop("checked", false);
    }

    var COND_DISCAPACIDAD = datosTabla["COND_DISCAPACIDAD"];

    if (COND_DISCAPACIDAD == 'S') {
        $("input[name='COND_DISCAPACIDAD']").prop("checked", "checked");
    } else {
        $("input[name='COND_DISCAPACIDAD']").prop("checked", false);
    }

    var COND_DESPLAZADO = datosTabla["COND_DESPLAZADO"];

    if (COND_DESPLAZADO == 'S') {
        $("input[name='COND_DESPLAZADO']").prop("checked", "checked");
    } else {
        $("input[name='COND_DESPLAZADO']").prop("checked", false);
    }

    $('#NIVEL_EDUCATIVO').val(datosTabla["NIVEL_EDUCATIVO"]);
    $('#NIVEL_EDUCATIVO').chosen().trigger("chosen:updated");

    $('#ACTIVIDAD_ECONOMICA').val(datosTabla["ACTIVIDAD_ECONOMICA"]);
    $('#ACTIVIDAD_ECONOMICA').chosen().trigger("chosen:updated");

    var PARTICIP_ORGANIZACION = datosTabla["PARTICIP_ORGANIZACION"];

    if (PARTICIP_ORGANIZACION == 'S') {
        $("input[name='PARTICIP_ORGANIZACION']").prop("checked", "checked");
    } else {
        $("input[name='PARTICIP_ORGANIZACION']").prop("checked", false);
    }

    $('#NOMBRE_ORGANIZACION').val(datosTabla["NOMBRE_ORGANIZACION"]);

    $('#REGIMEN_SALUD').val(datosTabla["REGIMEN_SALUD"]);
    $('#REGIMEN_SALUD').chosen().trigger("chosen:updated");

    $('#NOMBRE_IPS').val(datosTabla["NOMBRE_IPS"]);

    $('#COZO_LUGECODIGO_RESIDENCIA').val(datosTabla["COZO_LUGECODIGO_RESIDENCIA"]);
    $('#COZO_CODIGO_RESIDENCIA').val(datosTabla["COZO_CODIGO_RESIDENCIA"]);

    $('#BARRIO_CODIGO_RESIDENCIA').val(datosTabla["BARRIO_CODIGO_RESIDENCIA"]);
    $('#BARRIO_LUGECODIGO_RESIDENCIA').val(datosTabla["BARRIO_LUGECODIGO_RESIDENCIA"]);

    $('#MPIO_RESIDENCIA').val(datosTabla["MPIO_RESIDENCIA"]);
    $('#DSP_CIUDAD_RESIDENCIA').val(datosTabla["DSP_CIUDAD_RESIDENCIA"]);
    $('#dsp_comuna_RESIDENCIA').val(datosTabla["dsp_comuna_RESIDENCIA"]);
    $('#dsp_barrio_RESIDENCIA').val(datosTabla["dsp_barrio_RESIDENCIA"]);

    $('#DIRECCION_RESIDENCIA').val(datosTabla["DIRECCION_RESIDENCIA"]);

    $('#TELEFONO1').val(datosTabla["TELEFONO1"]);
    $('#TELEFONO2').val(datosTabla["TELEFONO2"]);

    var TIPO_RELACION_AGRESOR = datosTabla["TIPO_RELACION_AGRESOR"]
    //console.log(TIPO_RELACION_AGRESOR);
    if (TIPO_RELACION_AGRESOR == "FM") {
        $('#ckAgresorFamilia').trigger("click");
        $('#ckAgresorFamilia').trigger("click");
    } else if (TIPO_RELACION_AGRESOR == "PA") {
        $('#ckAgresorParticulas').trigger("click");
        $('#ckAgresorParticulas').trigger("click");
    }

    $('#RELACION_FAMILIAR_AGRESOR').val(datosTabla["RELACION_FAMILIAR_AGRESOR"]);
    $('#RELACION_FAMILIAR_AGRESOR').chosen().trigger("chosen:updated");

    $('#RELACION_PARTICULAR_AGRESOR').val(datosTabla["RELACION_PARTICULAR_AGRESOR"]);
    $('#RELACION_PARTICULAR_AGRESOR').chosen().trigger("chosen:updated");

    $('#GRUPO_AGRESOR').val(datosTabla["GRUPO_AGRESOR"]);
    $('#GRUPO_AGRESOR').chosen().trigger("chosen:updated");

    $('#MECANISMO_ARMA').val(datosTabla["MECANISMO_ARMA"]);
    $('#MECANISMO_ARMA').chosen().trigger("chosen:updated");

    $('#DESCRIPCION_GRUPO').val(datosTabla["DESCRIPCION_GRUPO"]);

    $('#MPIO_HECHOS').val(datosTabla["MPIO_HECHOS"]);

    $('#COZO_LUGECODIGO_HECHOS').val(datosTabla["COZO_LUGECODIGO_HECHOS"]);
    $('#COZO_CODIGO_HECHOS').val(datosTabla["COZO_CODIGO_HECHOS"]);
    $('#BARRIO_CODIGO_HECHOS').val(datosTabla["BARRIO_CODIGO_HECHOS"]);
    $('#BARRIO_LUGECODIGO_HECHOS').val(datosTabla["BARRIO_LUGECODIGO_HECHOS"]);

    $('#DSP_MPIOHECOS').val(datosTabla["DSP_MPIOHECOS"]);
    $('#DSP_BARRIOHECHOS').val(datosTabla["DSP_BARRIOHECHOS"]);
    $('#DSP_COMUNAHECHOS').val(datosTabla["DSP_COMUNAHECHOS"]);

    $('#DIRECCION_HECHOS').val(datosTabla["DIRECCION_HECHOS"]);

    $('#LUGAR_OCURRENCIA').val(datosTabla["LUGAR_OCURRENCIA"]);
    $('#LUGAR_OCURRENCIA').chosen().trigger("chosen:updated");

    $('#NOMBRE_SITIO').val(datosTabla["NOMBRE_SITIO"]);

    $('#OBSERVACIONES_MJ').val(datosTabla["OBSERVACIONES"]);
}

function LimpiarCampos() {
    $('#REGITRADURIA').val('');

    $('#PERSATEN_CONSECUTIVO').val('');

    $('#TIPO_IDENTIFICACION').val('');
    $('#TIPO_IDENTIFICACION').chosen().trigger("chosen:updated");

    $('#IDENTIFICACION').val('');
    $('#NOMBRES').val('');
    $('#APELLIDOS').val('');
    $('#FECHA_NACE').val('');

    $('#CODI_GENERO').val('');
    $('#CODI_GENERO').chosen().trigger("chosen:updated");

    $('#CODI_RAZA').val('');
    $('#CODI_RAZA').chosen().trigger("chosen:updated");

    $('#ckVictima').trigger("click");
    $('#ckVictima').trigger("click");

    $('#ckReportanteVP').trigger("click");
    $('#ckReportanteVP').trigger("click");

    $('#spanUsuario').html('    ');
    $('#spanFechaTransacion').html('    ');

    $('#MEDIO_CONOCIMIENTO').val('');
    $('#MEDIO_CONOCIMIENTO').chosen().trigger("chosen:updated");

    $('#NRO_FICHA').val('');
    $('#EDAD_PERSONA').val('');

    $('#TIPO_VICT_CONFLICTO').val('');
    $('#TIPO_VICT_CONFLICTO').chosen().trigger("chosen:updated");

    $('#TIPO_ESPECTATIVA').val('');
    $('#TIPO_ESPECTATIVA').chosen().trigger("chosen:updated");

    $('#TIPO_DISCAPACIDAD').val('');
    $('#TIPO_DISCAPACIDAD').chosen().trigger("chosen:updated");

    $("input[name='COND_LGTBI']").prop("checked", false);
    $("input[name='COND_DISCAPACIDAD']").prop("checked", false);
    $("input[name='COND_DESPLAZADO']").prop("checked", false);

    $('#NIVEL_EDUCATIVO').val('');
    $('#NIVEL_EDUCATIVO').chosen().trigger("chosen:updated");

    $('#ACTIVIDAD_ECONOMICA').val('');
    $('#ACTIVIDAD_ECONOMICA').chosen().trigger("chosen:updated");

    $("input[name='PARTICIP_ORGANIZACION']").prop("checked", false);

    $('#NOMBRE_ORGANIZACION').val('');

    $('#REGIMEN_SALUD').val('');
    $('#REGIMEN_SALUD').chosen().trigger("chosen:updated");

    $('#NOMBRE_IPS').val('');

    $('#COZO_LUGECODIGO_RESIDENCIA').val('');
    $('#COZO_CODIGO_RESIDENCIA').val('');

    $('#BARRIO_CODIGO_RESIDENCIA').val('');
    $('#BARRIO_LUGECODIGO_RESIDENCIA').val('');

    $('#MPIO_RESIDENCIA').val('');
    $('#DSP_CIUDAD_RESIDENCIA').val('');
    $('#dsp_comuna_RESIDENCIA').val('');
    $('#dsp_barrio_RESIDENCIA').val('');

    $('#DIRECCION_RESIDENCIA').val('');

    $('#TELEFONO1').val('');
    $('#TELEFONO2').val('');

    $('#ckAgresorParticulas').trigger("click");
    $('#ckAgresorParticulas').trigger("click");

    $('#RELACION_FAMILIAR_AGRESOR').val('');
    $('#RELACION_FAMILIAR_AGRESOR').chosen().trigger("chosen:updated");

    $('#RELACION_PARTICULAR_AGRESOR').val('');
    $('#RELACION_PARTICULAR_AGRESOR').chosen().trigger("chosen:updated");

    $('#GRUPO_AGRESOR').val('');
    $('#GRUPO_AGRESOR').chosen().trigger("chosen:updated");

    $('#MECANISMO_ARMA').val('');
    $('#MECANISMO_ARMA').chosen().trigger("chosen:updated");

    $('#DESCRIPCION_GRUPO').val('');
    $('#MPIO_HECHOS').val('');

    $('#COZO_LUGECODIGO_HECHOS').val('');
    $('#COZO_CODIGO_HECHOS').val('');
    $('#BARRIO_CODIGO_HECHOS').val('');
    $('#BARRIO_LUGECODIGO_HECHOS').val('');

    $('#DSP_MPIOHECOS').val('');
    $('#DSP_BARRIOHECHOS').val('');
    $('#DSP_COMUNAHECHOS').val('');

    $('#DIRECCION_HECHOS').val('');

    $('#LUGAR_OCURRENCIA').val('');
    $('#LUGAR_OCURRENCIA').chosen().trigger("chosen:updated");

    $('#NOMBRE_SITIO').val('');

    $('#OBSERVACIONES_MJ').val('');

    $("#tablaRAE").DataTable().destroy();
    $("#tablaRAE tbody").empty();

    AplicarPluginTablaAjaxRAEMujer("tablaRAE", urlRAEMujer, parametrosRAEMj);
    $('#OBSERVACION_RAE').val('');
    $('#CONSECUTIVO_RAE').val('');
    $('#TIPO_SERV_RAE').val('');
    $('#TIPO_SERV_RAE').chosen().trigger("chosen:updated");

    $("#tablaRM").DataTable().destroy();
    $("#tablaRM tbody").empty();

    AplicarPluginTablaAjaxRMMujer("tablaRM", urlRMMujer, parametrosRMMj);
    $('#OBSERVACION_RM').val('');
    $('#CONSECUTIVO_RM').val('');
    $('#TIPO_SERV_RM').val('');
    $('#TIPO_SERV_RM').chosen().trigger("chosen:updated");

    $("#tablaROI").DataTable().destroy();
    $("#tablaROI tbody").empty();
    AplicarPluginTablaAjaxROIMujer("tablaROI", urlROIMujer, parametrosROIMj);
    $('#OBSERVACION_ROI').val('');
    $('#CONSECUTIVO_ROI').val('');
    $('#TIPO_SERV_ROI').val('');
    $('#TIPO_SERV_ROI').chosen().trigger("chosen:updated");

    $("#tablaPerRelacionadas").DataTable().destroy();
    $("#tablaPerRelacionadas tbody").empty();
    AplicarPluginTablaAjaxPerRelacionadosMj("tablaPerRelacionadas", urlPerRelacionadasMj + "?strSitioGrabaEvento=" + SitioGrabaEventoGlobal + "&strEvento=" + EventoGlobal, parametrosPerRelacionadasMj);
}

function CargarRAEMujer(persatenSitiograbacion, persatenConsecutivo, sitioGrabaEvento, evento) {
    $("#tablaRAE").DataTable().destroy();
    $("#tablaRAE tbody").empty();

    AplicarPluginTablaAjaxRAEMujer("tablaRAE", urlRAEMujer + "?strPersatenSitiograbacion=" + persatenSitiograbacion + "&strPersatenConsecutivo=" + persatenConsecutivo +
                                             "&strSitioGrabaEvento=" + sitioGrabaEvento + "&strEvento=" + evento, parametrosRAEMj);
}

function CargarRMMujer(persatenSitiograbacion, persatenConsecutivo, sitioGrabaEvento, evento) {
    $("#tablaRM").DataTable().destroy();
    $("#tablaRM tbody").empty();

    AplicarPluginTablaAjaxRMMujer("tablaRM", urlRMMujer + "?strPersatenSitiograbacion=" + persatenSitiograbacion + "&strPersatenConsecutivo=" + persatenConsecutivo +
                                             "&strSitioGrabaEvento=" + sitioGrabaEvento + "&strEvento=" + evento, parametrosRMMj);
}

function CargarROIMujer(persatenSitiograbacion, persatenConsecutivo, sitioGrabaEvento, evento) {
    $("#tablaROI").DataTable().destroy();
    $("#tablaROI tbody").empty();

    AplicarPluginTablaAjaxROIMujer("tablaROI", urlROIMujer + "?strPersatenSitiograbacion=" + persatenSitiograbacion + "&strPersatenConsecutivo=" + persatenConsecutivo +
                                             "&strSitioGrabaEvento=" + sitioGrabaEvento + "&strEvento=" + evento, parametrosROIMj);
}

function CargarPriemraFilaRAE(tablaId) {
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(0).data();

    $('#OBSERVACION_RAE').val(datosTabla["OBSERVACION_RAE"]);
}

function CargarPriemraFilaRM(tablaId) {
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(0).data();

    $('#OBSERVACION_RM').val(datosTabla["OBSERVACION_RM"]);
}

function CargarDatosSelectRAEMujer(tablaId, object) {
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(object).data();
    $('#OBSERVACION_RAE').val(datosTabla["OBSERVACION_RAE"]);
    $('#CONSECUTIVO_RAE').val(datosTabla["CONSECUTIVO_RAE"]);

    $('#TIPO_SERV_RAE').val(datosTabla["TIPO_SERV_RAE"]);
    $('#TIPO_SERV_RAE').chosen().trigger("chosen:updated");
}

function CargarDatosSelectRMMujer(tablaId, object) {
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(object).data();
    $('#OBSERVACION_RM').val(datosTabla["OBSERVACION_RM"]);

    $('#CONSECUTIVO_RM').val(datosTabla["CONSECUTIVO_RM"]);

    $('#TIPO_SERV_RM').val(datosTabla["TIPO_SERV_RM"]);
    $('#TIPO_SERV_RM').chosen().trigger("chosen:updated");
}

function CargarDatosSelectROIMujer(tablaId, object) {
  //  console.log("entre");
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }

    datosTabla = tabla.row(object).data();
    $('#OBSERVACION_ROI').val(datosTabla["OBSERVACION_ROI"]);

    $('#CONSECUTIVO_ROI').val(datosTabla["CONSECUTIVO_ROI"]);

    $('#TIPO_SERV_ROI').val(datosTabla["TIPO_SERV_ROI"]);
    $('#TIPO_SERV_ROI').chosen().trigger("chosen:updated");
}

function CargarHechosMj() {
    $.ajax({
        type: 'POST',
        url: urlDatosHechos,
        dataType: 'json',
        data: { strSitioGrabaEvento: SitioGrabaEventoGlobal, strNumeroEvento: EventoGlobal },
        success: function (resultados) {
            if (resultados.result == true) {
                if (resultados.datosHechos["p_MPIO_HECHOS"] != 'null') {
                    $('#MPIO_HECHOS').val(resultados.datosHechos["p_MPIO_HECHOS"]);
                }
                if (resultados.datosHechos["p_DSP_MPIOHECOS"] != 'null') {
                    $('#DSP_MPIOHECOS').val(resultados.datosHechos["p_DSP_MPIOHECOS"]);
                }
                if (resultados.datosHechos["p_BARRIO_CODIGO_HECHOS"] != 'null') {
                    $('#BARRIO_CODIGO_HECHOS').val(resultados.datosHechos["p_BARRIO_CODIGO_HECHOS"]);
                }
                if (resultados.datosHechos["p_BARRIO_LUGECODIGO_HECHOS"] != 'null') {
                    $('#BARRIO_LUGECODIGO_HECHOS').val(resultados.datosHechos["p_BARRIO_LUGECODIGO_HECHOS"]);
                }
                if (resultados.datosHechos["p_DSP_BARRIOHECHOS"] != 'null') {
                    $('#DSP_BARRIOHECHOS').val(resultados.datosHechos["p_DSP_BARRIOHECHOS"]);
                }

                if (resultados.datosHechos["p_DIRECCION_HECHOS"] != 'null') {
                    $('#DIRECCION_HECHOS').val(resultados.datosHechos["p_DIRECCION_HECHOS"]);
                }

                if (resultados.datosHechos["p_COZO_LUGECODIGO_HECHOS"] != 'null') {
                    $('#COZO_LUGECODIGO_HECHOS').val(resultados.datosHechos["p_COZO_LUGECODIGO_HECHOS"]);
                }

                if (resultados.datosHechos["p_COZO_CODIGO_HECHOS"] != 'null') {
                    $('#COZO_CODIGO_HECHOS').val(resultados.datosHechos["p_COZO_CODIGO_HECHOS"]);
                }

                if (resultados.datosHechos["p_DSP_COMUNAHECHOS"] != 'null') {
                    $('#DSP_COMUNAHECHOS').val(resultados.datosHechos["p_DSP_COMUNAHECHOS"]);
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

function CrearRAEMujer() {
    var PERSATEN_SITIOGRABACION = $('#PERSATEN_SITIOGRABACION').val();
    var PERSATEN_CONSECUTIVO = $('#PERSATEN_CONSECUTIVO').val();
    var EVEN_SITIOGRABACION = $('#EVEN_SITIOGRABACION').val();
    var EVEN_NUMEEVENTO = $('#EVEN_NUMEEVENTO').val();
    var CONSECUTIVO_RAE = $('#CONSECUTIVO_RAE').val();

    var recurso = $('#TIPO_SERV_RAE').val();
    var observacion = $('#OBSERVACION_RAE').val();

    // console.log(PERSATEN_SITIOGRABACION + "-" + PERSATEN_CONSECUTIVO + "-" + EVEN_SITIOGRABACION + "-" + EVEN_NUMEEVENTO)

    if (PERSATEN_SITIOGRABACION == "" || PERSATEN_CONSECUTIVO == "" || EVEN_SITIOGRABACION == "" || EVEN_NUMEEVENTO == "") {
        GestionarAlerta("warning", "Debe seleccionar o crear una persona relacionada primero.");
        return;
    }

    if (recurso != "") {
        var datosRAE = new Array();

        datosRAE.push(PERSATEN_SITIOGRABACION);
        datosRAE.push(PERSATEN_CONSECUTIVO);
        datosRAE.push(EVEN_SITIOGRABACION);
        datosRAE.push(EVEN_NUMEEVENTO);
        datosRAE.push(CONSECUTIVO_RAE);
        datosRAE.push(recurso);
        datosRAE.push(observacion);

        $.ajax({
            type: 'POST',
            url: urlCrearRAE,
            dataType: 'json',
            data: { strDatosNuevoRAE: datosRAE },
            success: function (resultados) {
                if (resultados.result == true) {
                    CargarRAEMujer(PERSATEN_SITIOGRABACION, PERSATEN_CONSECUTIVO, EVEN_SITIOGRABACION, EVEN_NUMEEVENTO);
                    GestionarAlerta("success", resultados.datos);
                    $('#OBSERVACION_RAE').val('');
                    $('#CONSECUTIVO_RAE').val('');
                    $('#TIPO_SERV_RAE').val('');
                    $('#TIPO_SERV_RAE').chosen().trigger("chosen:updated");
                } else {
                    GestionarAlerta("warning", resultados.datos);
                }
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });
    } else {
        GestionarAlerta("warning", "Debe seleccionar un recurso.");
    }
}

function CrearRMMujer() {
    var PERSATEN_SITIOGRABACION = $('#PERSATEN_SITIOGRABACION').val();
    var PERSATEN_CONSECUTIVO = $('#PERSATEN_CONSECUTIVO').val();
    var EVEN_SITIOGRABACION = $('#EVEN_SITIOGRABACION').val();
    var EVEN_NUMEEVENTO = $('#EVEN_NUMEEVENTO').val();
    var CONSECUTIVO_RM = $('#CONSECUTIVO_RM').val();

    var tipoAtencion = $('#TIPO_SERV_RM').val();
    var observacion = $('#OBSERVACION_RM').val();

    console.log(PERSATEN_SITIOGRABACION + "-" + PERSATEN_CONSECUTIVO + "-" + EVEN_SITIOGRABACION + "-" + EVEN_NUMEEVENTO)

    if (PERSATEN_SITIOGRABACION == "" || PERSATEN_CONSECUTIVO == "" || EVEN_SITIOGRABACION == "" || EVEN_NUMEEVENTO == "") {
        GestionarAlerta("warning", "Debe seleccionar o crear una persona relacionada primero.");
        return;
    }

    if (tipoAtencion != "") {
        var datosRM = new Array();

        datosRM.push(PERSATEN_SITIOGRABACION);
        datosRM.push(PERSATEN_CONSECUTIVO);
        datosRM.push(EVEN_SITIOGRABACION);
        datosRM.push(EVEN_NUMEEVENTO);
        datosRM.push(CONSECUTIVO_RM);
        datosRM.push(tipoAtencion);
        datosRM.push(observacion);

        $.ajax({
            type: 'POST',
            url: urlCrearRM,
            dataType: 'json',
            data: { strDatosNuevoRM: datosRM },
            success: function (resultados) {
                if (resultados.result == true) {
                    CargarRMMujer(PERSATEN_SITIOGRABACION, PERSATEN_CONSECUTIVO, EVEN_SITIOGRABACION, EVEN_NUMEEVENTO);
                    GestionarAlerta("success", resultados.datos);
                    $('#OBSERVACION_RM').val('');
                    $('#CONSECUTIVO_RM').val('');
                    $('#TIPO_SERV_RM').val('');
                    $('#TIPO_SERV_RM').chosen().trigger("chosen:updated");
                } else {
                    GestionarAlerta("warning", resultados.datos);
                }
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });
    } else {
        GestionarAlerta("warning", "Debe seleccionar un tipo atención.");
    }
}

function CrearROIMujer() {
    var PERSATEN_SITIOGRABACION = $('#PERSATEN_SITIOGRABACION').val();
    var PERSATEN_CONSECUTIVO = $('#PERSATEN_CONSECUTIVO').val();
    var EVEN_SITIOGRABACION = $('#EVEN_SITIOGRABACION').val();
    var EVEN_NUMEEVENTO = $('#EVEN_NUMEEVENTO').val();
    var CONSECUTIVO_ROI = $('#CONSECUTIVO_ROI').val();

    var oferta = $('#TIPO_SERV_ROI').val();
    var observacion = $('#OBSERVACION_ROI').val();

    // console.log(PERSATEN_SITIOGRABACION + "-" + PERSATEN_CONSECUTIVO + "-" + EVEN_SITIOGRABACION + "-" + EVEN_NUMEEVENTO)

    if (PERSATEN_SITIOGRABACION == "" || PERSATEN_CONSECUTIVO == "" || EVEN_SITIOGRABACION == "" || EVEN_NUMEEVENTO == "") {
        GestionarAlerta("warning", "Debe seleccionar o crear una persona relacionada primero.");
        return;
    }

    if (oferta != "") {
        var datosROI = new Array();

        datosROI.push(PERSATEN_SITIOGRABACION);
        datosROI.push(PERSATEN_CONSECUTIVO);
        datosROI.push(EVEN_SITIOGRABACION);
        datosROI.push(EVEN_NUMEEVENTO);
        datosROI.push(CONSECUTIVO_ROI);
        datosROI.push(oferta);
        datosROI.push(observacion);

        $.ajax({
            type: 'POST',
            url: urlCrearROI,
            dataType: 'json',
            data: { strDatosNuevoROI: datosROI },
            success: function (resultados) {
                if (resultados.result == true) {
                    CargarROIMujer(PERSATEN_SITIOGRABACION, PERSATEN_CONSECUTIVO, EVEN_SITIOGRABACION, EVEN_NUMEEVENTO);
                    GestionarAlerta("success", resultados.datos);
                    $('#OBSERVACION_ROI').val('');
                    $('#CONSECUTIVO_ROI').val('');
                    $('#TIPO_SERV_ROI').val('');
                    $('#TIPO_SERV_ROI').chosen().trigger("chosen:updated");
                } else {
                    GestionarAlerta("warning", resultados.datos);
                }
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });
    } else {
        GestionarAlerta("warning", "Debe seleccionar una oferta.");
    }
}

//Permite realizar la eliminación de un registro especifíco
function EliminarRMMujer(pasatenSitioGraba, pasatenConsecutivo, eventoSitioGraba, evento, consecutivoRM) {
    var datos = { strPasatenSitioGraba: pasatenSitioGraba, strPasatenConsecutivo: pasatenConsecutivo, strEventoSitioGraba: eventoSitioGraba, strEvento: evento, strConsecutivoRM: consecutivoRM };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Consecutivo: " + consecutivoRM,
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
                url: urlEliminarRM,
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
                    CargarRMMujer(pasatenSitioGraba, pasatenConsecutivo, eventoSitioGraba, evento);
                    GestionarAlerta("success", resultado.datos);
                    $('#OBSERVACION_RM').val('');
                    $('#CONSECUTIVO_RM').val('');
                    $('#TIPO_SERV_RM').val('');
                    $('#TIPO_SERV_RM').chosen().trigger("chosen:updated");
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}

//Permite realizar la eliminación de un registro especifíco
function EliminarRAEMujer(pasatenSitioGraba, pasatenConsecutivo, eventoSitioGraba, evento, consecutivoRAE) {
    var datos = { strPasatenSitioGraba: pasatenSitioGraba, strPasatenConsecutivo: pasatenConsecutivo, strEventoSitioGraba: eventoSitioGraba, strEvento: evento, strConsecutivoRAE: consecutivoRAE };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Consecutivo: " + consecutivoRAE,
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
                url: urlEliminarRAE,
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
                    CargarRAEMujer(pasatenSitioGraba, pasatenConsecutivo, eventoSitioGraba, evento);
                    GestionarAlerta("success", resultado.datos);
                    $('#OBSERVACION_RAE').val('');
                    $('#CONSECUTIVO_RAE').val('');
                    $('#TIPO_SERV_RAE').val('');
                    $('#TIPO_SERV_RAE').chosen().trigger("chosen:updated");
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}

//Permite realizar la eliminación de un registro especifíco
function EliminarROIMujer(pasatenSitioGraba, pasatenConsecutivo, eventoSitioGraba, evento, consecutivoROI) {
    var datos = { strPasatenSitioGraba: pasatenSitioGraba, strPasatenConsecutivo: pasatenConsecutivo, strEventoSitioGraba: eventoSitioGraba, strEvento: evento, strConsecutivoROI: consecutivoROI };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Consecutivo: " + consecutivoROI,
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
                url: urlEliminarROI,
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
                    CargarROIMujer(pasatenSitioGraba, pasatenConsecutivo, eventoSitioGraba, evento);
                    GestionarAlerta("success", resultado.datos);
                    $('#OBSERVACION_ROI').val('');
                    $('#CONSECUTIVO_ROI').val('');
                    $('#TIPO_SERV_ROI').val('');
                    $('#TIPO_SERV_ROI').chosen().trigger("chosen:updated");
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}

//Residencia
function CargaCiudades() {
    $("#tablaCiudades").DataTable().destroy();
    $("#tablaCiudades tbody").empty();

    AplicarPluginTablaAjax("tablaCiudades", (urlCargarCiudades + "?tab=1"), parametrosAjaxCiudad);
    $("#ModalCiudades").modal();
}

//Hechos
function CargarCiudadesHechos() {
    $("#tablaCiudades").DataTable().destroy();
    $("#tablaCiudades tbody").empty();

    AplicarPluginTablaAjax("tablaCiudades", (urlCargarCiudades + "?tab=0"), parametrosAjaxCiudad);
    $("#ModalCiudades").modal();
}

//Residencia
function CargarCiudad(codigo, descripcion) {
    $("#MPIO_RESIDENCIA").empty();
    $("#MPIO_RESIDENCIA").val(codigo);
    $("#DSP_CIUDAD_RESIDENCIA").empty();
    $("#DSP_CIUDAD_RESIDENCIA").val(descripcion);
    $("#ModalCiudades").modal("toggle");
}

//Hechos
function CargarCiudadHechos(codigo, descripcion) {
    $("#MPIO_HECHOS").empty();
    $("#MPIO_HECHOS").val(codigo);

    $("#DSP_MPIOHECOS").empty();
    $("#DSP_MPIOHECOS").val(descripcion);

    $("#ModalCiudades").modal("toggle");
}

//Residencia
function CargaComunasResidencia() {
    var codLuge = $("#MPIO_RESIDENCIA").val();
    if (codLuge != "") {
        $("#tablaComunas").DataTable().destroy();
        $("#tablaComunas tbody").empty();

        AplicarPluginTablaAjax("tablaComunas", (urlCargaComunas + "?lugeCodigo=" + codLuge + "&tab=1"), parametrosAjaxComunas);
    } else {
        $("#tablaComunas").DataTable().destroy();
        $("#tablaComunas tbody").empty();

        AplicarPluginTablaAjax("tablaComunas", urlCargaComunas, parametrosAjaxComunas);
    }

    $("#ModalComunas").modal();
}

//Hechos
function CargaComunasHechos() {
    var codLuge = $("#MPIO_HECHOS").val();
    if (codLuge != "") {
        $("#tablaComunas").DataTable().destroy();
        $("#tablaComunas tbody").empty();

        AplicarPluginTablaAjax("tablaComunas", (urlCargaComunas + "?lugeCodigo=" + codLuge + "&tab=0"), parametrosAjaxComunas);
    } else {
        $("#tablaComunas").DataTable().destroy();
        $("#tablaComunas tbody").empty();

        AplicarPluginTablaAjax("tablaComunas", urlCargaComunas, parametrosAjaxComunas);
    }

    $("#ModalComunas").modal();
}

//Residencia
function CargarComunaResidencia(luge, codigo, descripcion) {
    //console.log(luge + "---" + codigo);
    $("#COZO_LUGECODIGO_RESIDENCIA").empty();
    $("#COZO_LUGECODIGO_RESIDENCIA").val(luge);

    $("#COZO_CODIGO_RESIDENCIA").empty();
    $("#COZO_CODIGO_RESIDENCIA").val(codigo);

    $("#dsp_comuna_RESIDENCIA").empty();
    $("#dsp_comuna_RESIDENCIA").val(descripcion);

    $("#ModalComunas").modal("toggle");

    $("#BARRIO_LUGECODIGO_RESIDENCIA").val('');
    $("#BARRIO_CODIGO_RESIDENCIA").val('');
    $("#dsp_barrio_RESIDENCIA").val('');
}

//Hechos
function CargarComunaHecho(luge, codigo, descripcion) {
    //console.log(luge + "---" + codigo);
    $("#COZO_LUGECODIGO_HECHOS").empty();
    $("#COZO_LUGECODIGO_HECHOS").val(luge);

    $("#COZO_CODIGO_HECHOS").empty();
    $("#COZO_CODIGO_HECHOS").val(codigo);

    $("#DSP_COMUNAHECHOS").empty();
    $("#DSP_COMUNAHECHOS").val(descripcion);

    $("#ModalComunas").modal("toggle");

    $("#BARRIO_LUGECODIGO_HECHOS").val('');

    $("#BARRIO_CODIGO_HECHOS").val('');

    $("#DSP_BARRIOHECHOS").val('');
}

//Residencial
function CargaBarriosComunasResidencia() {
    var codLuge = $("#COZO_LUGECODIGO_RESIDENCIA").val();
    var cod = $("#COZO_CODIGO_RESIDENCIA").val();
    if (codLuge != "" && cod != null) {
        $("#tablaBarriosComunas").DataTable().destroy();
        $("#tablaBarriosComunas tbody").empty();

        AplicarPluginTablaAjax("tablaBarriosComunas", (urlCargaBarriosComunas + "?cozoLugeCodigo=" + codLuge + "&cozoCodigo=" + cod + "&tab=1"), parametrosAjaxBarriosComunas);
    } else {
        $("#tablaBarriosComunas").DataTable().destroy();
        $("#tablaBarriosComunas tbody").empty();

        AplicarPluginTablaAjax("tablaBarriosComunas", urlCargaBarriosComunas, parametrosAjaxBarriosComunas);
    }

    $("#ModalBarriosComunas").modal();
}

//Hechos
function CargaBarriosComunasHechos() {
    var codLuge = $("#COZO_LUGECODIGO_HECHOS").val();
    var cod = $("#COZO_CODIGO_HECHOS").val();
    if (codLuge != "" && cod != null) {
        $("#tablaBarriosComunas").DataTable().destroy();
        $("#tablaBarriosComunas tbody").empty();

        AplicarPluginTablaAjax("tablaBarriosComunas", (urlCargaBarriosComunas + "?cozoLugeCodigo=" + codLuge + "&cozoCodigo=" + cod + "&tab=0"), parametrosAjaxBarriosComunas);
    } else {
        $("#tablaBarriosComunas").DataTable().destroy();
        $("#tablaBarriosComunas tbody").empty();

        AplicarPluginTablaAjax("tablaBarriosComunas", urlCargaBarriosComunas, parametrosAjaxBarriosComunas);
    }

    $("#ModalBarriosComunas").modal();
}

//Hechos
function CargarBarriosComunaHecho(luge, codigo, descripcion) {
    //console.log(luge + "---" + codigo);
    $("#BARRIO_LUGECODIGO_HECHOS").empty();
    $("#BARRIO_LUGECODIGO_HECHOS").val(luge);

    $("#BARRIO_CODIGO_HECHOS").empty();
    $("#BARRIO_CODIGO_HECHOS").val(codigo);

    $("#DSP_BARRIOHECHOS").empty();
    $("#DSP_BARRIOHECHOS").val(descripcion);

    $("#ModalBarriosComunas").modal("toggle");
}

//Residencial
function CargarBarriosComunaResidencia(luge, codigo, descripcion) {
    // console.log(luge + "---" + codigo);
    $("#BARRIO_LUGECODIGO_RESIDENCIA").empty();
    $("#BARRIO_LUGECODIGO_RESIDENCIA").val(luge);

    $("#BARRIO_CODIGO_RESIDENCIA").empty();
    $("#BARRIO_CODIGO_RESIDENCIA").val(codigo);

    $("#dsp_barrio_RESIDENCIA").empty();
    $("#dsp_barrio_RESIDENCIA").val(descripcion);

    $("#ModalBarriosComunas").modal("toggle");
}

function GuardarPersonaRelacionadaMujer() {
    var TIPO_IDENTIFICACION = $('#TIPO_IDENTIFICACION').val();
    var IDENTIFICACION = $('#IDENTIFICACION').val();
    var NOMBRES = $('#NOMBRES').val();
    var APELLIDOS = $('#APELLIDOS').val();

    if (TIPO_IDENTIFICACION != "" && IDENTIFICACION != "" && NOMBRES != "" && APELLIDOS != "") {
        var datosPersonaRelacionada = new Array();

        datosPersonaRelacionada.push("PERSATEN_CONSECUTIVO=" + $('#PERSATEN_CONSECUTIVO').val());

        //Datos basicos
        datosPersonaRelacionada.push("TIPO_IDENTIFICACION=" + TIPO_IDENTIFICACION);
        datosPersonaRelacionada.push("IDENTIFICACION=" + IDENTIFICACION);
        datosPersonaRelacionada.push("NOMBRES=" + NOMBRES);
        datosPersonaRelacionada.push("APELLIDOS=" + APELLIDOS);

        datosPersonaRelacionada.push("FECHA_NACE=" + $('#FECHA_NACE').val());
        datosPersonaRelacionada.push("CODI_GENERO=" + $('#CODI_GENERO').val());
        datosPersonaRelacionada.push("CODI_RAZA=" + $('#CODI_RAZA').val());

        //Tipo registro

        var TIPO_INTERVINIENTE = "";

        if ($("#ckVictima").is(":checked")) {
            TIPO_INTERVINIENTE = "VP";
        } else if ($("#ckVictimaSegunda").is(":checked")) {
            TIPO_INTERVINIENTE = "VS";
        } else if ($("#ckReporte").is(":checked")) {
            TIPO_INTERVINIENTE = "RP";
        } else if ($("#ckAgresor").is(":checked")) {
            TIPO_INTERVINIENTE = "AG";
        }

        datosPersonaRelacionada.push("TIPO_INTERVINIENTE=" + TIPO_INTERVINIENTE);

        //Reportante

        var CALIDAD_REPORTANTE = "";

        if ($("#ckReportanteVP").is(":checked")) {
            CALIDAD_REPORTANTE = "VP";
        } else if ($("#ckReportanteVS").is(":checked")) {
            CALIDAD_REPORTANTE = "VS";
        } else if ($("#ckReportanteFM").is(":checked")) {
            CALIDAD_REPORTANTE = "FM";
        } else if ($("#ckReportanteCN").is(":checked")) {
            CALIDAD_REPORTANTE = "CN";
        } else if ($("#ckReportanteDC").is(":checked")) {
            CALIDAD_REPORTANTE = "DC";
        }

        datosPersonaRelacionada.push("CALIDAD_REPORTANTE=" + CALIDAD_REPORTANTE);

        //Info

        datosPersonaRelacionada.push("MEDIO_CONOCIMIENTO=" + $('#MEDIO_CONOCIMIENTO').val());
        datosPersonaRelacionada.push("NRO_FICHA=" + $('#NRO_FICHA').val());

        var COND_LGTBI = "";
        if ($("input[name='COND_LGTBI']").prop("checked")) {
            COND_LGTBI = "S";
        } else {
            COND_LGTBI = "N";
        }
        datosPersonaRelacionada.push("COND_LGTBI=" + COND_LGTBI);
        datosPersonaRelacionada.push("EDAD_PERSONA=" + $('#EDAD_PERSONA').val());

        var COND_DISCAPACIDAD = "";
        if ($("input[name='COND_DISCAPACIDAD']").prop("checked")) {
            COND_DISCAPACIDAD = "S";
        } else {
            COND_DISCAPACIDAD = "N";
        }

        datosPersonaRelacionada.push("COND_DISCAPACIDAD=" + COND_DISCAPACIDAD);
        datosPersonaRelacionada.push("TIPO_DISCAPACIDAD=" + $('#TIPO_DISCAPACIDAD').val());

        var COND_DESPLAZADO = "";
        if ($("input[name='COND_DESPLAZADO']").prop("checked")) {
            COND_DESPLAZADO = "S";
        } else {
            COND_DESPLAZADO = "N";
        }
        datosPersonaRelacionada.push("COND_DESPLAZADO=" + COND_DESPLAZADO);
        datosPersonaRelacionada.push("TIPO_VICT_CONFLICTO=" + $('#TIPO_VICT_CONFLICTO').val());

        datosPersonaRelacionada.push("NIVEL_EDUCATIVO=" + $('#NIVEL_EDUCATIVO').val());
        datosPersonaRelacionada.push("ACTIVIDAD_ECONOMICA=" + $('#ACTIVIDAD_ECONOMICA').val());

        var PARTICIP_ORGANIZACION = "";
        if ($("input[name='COND_DESPLAZADO']").prop("checked")) {
            PARTICIP_ORGANIZACION = "S";
        } else {
            PARTICIP_ORGANIZACION = "N";
        }
        datosPersonaRelacionada.push("PARTICIP_ORGANIZACION=" + PARTICIP_ORGANIZACION);
        datosPersonaRelacionada.push("NOMBRE_ORGANIZACION=" + $('#NOMBRE_ORGANIZACION').val());

        datosPersonaRelacionada.push("REGIMEN_SALUD=" + $('#REGIMEN_SALUD').val());
        datosPersonaRelacionada.push("NOMBRE_IPS=" + $('#NOMBRE_IPS').val());

        //Residencia

        datosPersonaRelacionada.push("MPIO_RESIDENCIA=" + $('#MPIO_RESIDENCIA').val());
        datosPersonaRelacionada.push("COZO_LUGECODIGO_RESIDENCIA=" + $('#COZO_LUGECODIGO_RESIDENCIA').val());
        datosPersonaRelacionada.push("COZO_CODIGO_RESIDENCIA=" + $('#COZO_CODIGO_RESIDENCIA').val());
        datosPersonaRelacionada.push("BARRIO_CODIGO_RESIDENCIA=" + $('#BARRIO_CODIGO_RESIDENCIA').val());
        datosPersonaRelacionada.push("BARRIO_LUGECODIGO_RESIDENCIA=" + $('#BARRIO_LUGECODIGO_RESIDENCIA').val());
        datosPersonaRelacionada.push("DIRECCION_RESIDENCIA=" + $('#DIRECCION_RESIDENCIA').val());
        datosPersonaRelacionada.push("TELEFONO1=" + $('#TELEFONO1').val());
        datosPersonaRelacionada.push("TELEFONO2=" + $('#TELEFONO2').val());

        //Info 2

        var TIPO_RELACION_AGRESOR = "";

        if ($("#ckAgresorFamilia").is(":checked")) {
            TIPO_RELACION_AGRESOR = "FM";
        } else if ($("#ckAgresorParticulas").is(":checked")) {
            TIPO_RELACION_AGRESOR = "PA";
        }

        datosPersonaRelacionada.push("TIPO_RELACION_AGRESOR=" + TIPO_RELACION_AGRESOR);
        datosPersonaRelacionada.push("RELACION_FAMILIAR_AGRESOR=" + $('#RELACION_FAMILIAR_AGRESOR').val());
        datosPersonaRelacionada.push("RELACION_PARTICULAR_AGRESOR=" + $('#RELACION_PARTICULAR_AGRESOR').val());
        datosPersonaRelacionada.push("GRUPO_AGRESOR=" + $('#GRUPO_AGRESOR').val());
        datosPersonaRelacionada.push("DESCRIPCION_GRUPO=" + $('#DESCRIPCION_GRUPO').val());
        datosPersonaRelacionada.push("MECANISMO_ARMA=" + $('#MECANISMO_ARMA').val());

        //Hechos
        datosPersonaRelacionada.push("TIPO_ESPECTATIVA=" + $('#TIPO_ESPECTATIVA').val());
        datosPersonaRelacionada.push("MPIO_HECHOS=" + $('#MPIO_HECHOS').val());

        datosPersonaRelacionada.push("COZO_LUGECODIGO_HECHOS=" + $('#COZO_LUGECODIGO_HECHOS').val());
        datosPersonaRelacionada.push("COZO_CODIGO_HECHOS=" + $('#COZO_CODIGO_HECHOS').val());
        datosPersonaRelacionada.push("BARRIO_CODIGO_HECHOS=" + $('#BARRIO_CODIGO_HECHOS').val());
        datosPersonaRelacionada.push("BARRIO_LUGECODIGO_HECHOS=" + $('#BARRIO_LUGECODIGO_HECHOS').val());

        datosPersonaRelacionada.push("DIRECCION_HECHOS=" + $('#DIRECCION_HECHOS').val());
        datosPersonaRelacionada.push("LUGAR_OCURRENCIA=" + $('#LUGAR_OCURRENCIA').val());
        datosPersonaRelacionada.push("NOMBRE_SITIO=" + $('#NOMBRE_SITIO').val());

        $.ajax({
            type: 'POST',
            url: urlCrearPersonaRelacionada,
            dataType: 'json',
            data: { strDatosPersonaRelacion: datosPersonaRelacionada, strEventoSitioGraba: SitioGrabaEventoGlobal, strEvento: EventoGlobal },
            success: function (resultados) {
                if (resultados.result == true) {
                    GestionarAlerta("success", resultados.datos);
                    LimpiarCampos();
                } else {
                    GestionarAlerta("warning", resultados.datos);
                }
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });
    } else {
        GestionarAlerta("warning", "Debe ingresar los campos requeridos.");
    }
}