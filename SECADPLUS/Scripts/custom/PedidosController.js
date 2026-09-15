///////////////////VARIABLES DE CONTROL PARA EL TIMER//////////////////
var glEnUso = false;
var timerRecepcionId = 0;
var glTiempo = 2000;//Intervalo en milisegundo para la ejecución del timer
var timerLlamadas24HId = 0;
var glTiempoLlamada24H = 15000;//Intervalo en milisegundo para la ejecución del timer

//////////////////////////////////////////////////////////////////////
var timeoutArcGis = 5000;
var btnDesasignar = "<button class='btn bg-teal btn-xs' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para ver'>" +
    "<i class='icon-arrow-down132'></i></button>";
var glLugeCodigo = 0;
var glCasoText = "";
var glCasoId = "";
var glTipo = "01";
var glCalidad = "";
var glPreguntasCaso = "";
var glPreguntasCaso2 = "";
var glPreguntasOpcion = 0;
var glEstado = "S";
var glCreado = false;
var glCerrar = false;
var glAsociada = false;
var glControlRevision = false;
var glEnviarPedido = false; //Permite controlar cuando el usuario selecciona el envio del pedido
var prioridad = "";
var numellamadarecargar;

//$("#prioridad3").prop("checked", "checked");

//MarcarPrioridad("02");
////////////////////////////////////////////////////////////////////
/////control de intervalos cuando se encutre la ventana activa//////
////////////////////////////////////////////////////////////////////
//$(window).on("blur focus", function (e) {
//    var prevType = $(this).data("prevType");

//    if (prevType != e.type) {   //  reduce double fire issues
//        switch (e.type) {
//            case "blur":
//                // detiene timer si la ventana no se encuentra activa
//                DetenerTimer();
//                break;
//            case "focus":
//                // habilita timer si no existe abonado
//                //console.log("abonado " + $("#NUME_LLAMADA").val());
//                if ($("#NUME_LLAMADA").val() === "") {
//                    IniciarTimer();
//                }
//                break;
//        }
//    }

//    $(this).data("prevType", e.type);
//})
////////////////////////////////////////////////////////////////////
////////CARGUE DE FUNCIONALIDADES///////////////////////////////////
////////////////////////////////////////////////////////////////////
function AplicarPluginFiltroFecha() {

    $(".FechaHora").datetimepicker({
        format: 'DD/MM/YYYY',
        //minDate : new Date(),
        showClear: true,
        useCurrent: false,
        locale: 'es'
    });
}

$(function () {
    $("#btnLimpiarFiltro").click(function () {
        var urlFinal = "";
        $("input[name='filtroConsulta']").val("");
        urlFinal = urlConsultaPedido.replace(/amp;/g, "") + "?strSitio=" + glSitio + "&strUsuario=" + glUsuario + "&strOpcion=" + glModo;
        $("#tblPedidos").DataTable().ajax.url(urlFinal);
        $("#tblPedidos").DataTable().ajax.reload();

    });
    $("#txtFecha").on("dp.change", function (e) {
        var urlFinal = "";
        urlFinal = urlConsultaPedido.replace(/amp;/g, "") + "?strSitio=" + glSitio + "&strUsuario=" + glUsuario + "&strOpcion=" + glModo + "&strCasoPedido=" + $("#txtCodigoCaso").val() + "&strAbonado=" + $("#txtAbonado").val() + "&strFecha=" + $("#txtFecha").val() + "&strDesc=" + $("#txtDescripcion").val();
        $("#tblPedidos").DataTable().ajax.url(urlFinal);
        $("#tblPedidos").DataTable().ajax.reload();
    });

    $("input[name='filtroConsulta']").keypress(function (e) {
        if (e.which == 13) {
            var urlFinal = "";
            urlFinal = urlConsultaPedido.replace(/amp;/g, "") + "?strSitio=" + glSitio + "&strUsuario=" + glUsuario + "&strOpcion=" + glModo + "&strCasoPedido=" + $("#txtCodigoCaso").val() + "&strAbonado=" + $("#txtAbonado").val() + "&strFecha=" + $("#txtFecha").val() + "&strDesc=" + $("#txtDescripcion").val();
            $("#tblPedidos").DataTable().ajax.url(urlFinal);
            $("#tblPedidos").DataTable().ajax.reload();

        }
    });
    $('[data-toggle="tooltip"]').tooltip();
    ///Eventos de cierre de modales///
    $("#ModalCiudades").on("hidden.bs.modal", function () {
        //$("#txtBarrio").focus();
        $("#DIRE_CASO").focus();
    });
    $("#ModalBarrios").on("hidden.bs.modal", function () {
        // $("#DIRE_CASO").focus();
        $("#NOMB_LLAMANTE").focus();
    });
    $("#ModalAsociadas").on("hidden.bs.modal", function () {
        $("#CADPEDI_NUME_LLAMADA").focus();
    });
    ///Eventos de cierre de modales///
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab

        if (target === "#asociadas")
            ConsultarLlamadaAsociada();
    });


    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href") // activated tab

        if (target === "#asociadas")
            ConsultarLlamadaAsociada();

        if (target === "#XGeoinv")
            iniciarMapa();
    });



    $("#ddlMedios").click(function (e) {
        e.preventDefault();
        $("#PROP_TELEFONO").focus();
    });
    //ActivarLoader("panel2");
    //InactivarLoader("panel2");
    //eventos change
    $("#txtCasoId").keypress(function (e) {
        if (e.which == 13) {
            glCasoText = "txtCaso";
            glCasoId = "txtCasoId";
            glPreguntasOpcion = 1;
            CompletarCaso($(this).val());
        }
    });
    //$("#txtCasoId").blur(function () {
    //    glCasoText = "txtCaso";
    //    glCasoId = "txtCasoId";
    //    glPreguntasOpcion = 1;
    //    CompletarCaso($(this).val());
    //});
    $("#txtCasoId2").keypress(function (e) {
        if (e.which == 13) {
            glCasoText = "txtCaso2";
            glCasoId = "txtCasoId2";
            glPreguntasOpcion = 2;
            if ($(this).val() === "") {
                $("#txtCiudad").focus();
            } else {
                CompletarCaso($(this).val());
            }
        }
    });
    //$("#txtCasoId2").blur(function () {
    //    glCasoText = "txtCaso2";
    //    glCasoId = "txtCasoId2";
    //    glPreguntasOpcion = 2;
    //    CompletarCaso($(this).val());
    //});
    $("#txtCiudad").keypress(function (e) {
        if (e.which == 13) {
            if ($("#txtCiudad").val() != "") {
                $("#ModalCiudades").modal("toggle");
                CargarCiudades($("#txtCiudad").val());
            }
        }
    });

   
    //$("#txtCiudad").blur(function () {
    //    if ($("#txtCiudad").val() != "") {
    //        $("#ModalCiudades").modal("toggle");
    //        CargarCiudades($("#txtCiudad").val());
    //    }

    //   // CompletarCiudad($(this).val());
    //});
    $("#ESTADO").click(function (e) {
        //console.log(glControlRevision);
        //console.log((glControlRevision === false));
        if (glControlRevision === false) {
            e.preventDefault();
            $("#PROP_TELEFONO").focus();
        }
    });
    //Eventos click
    $("#usu24").click(function () {
        DetenerTimer();
        CargarTablaLlamadas24h();
        IniciarTimerLlamadas24H();
    });

    $("#btnGuardarComentario").click(function () {
        GuardarComentarios();
    });
    $("#tabgral").click(function (event) {
        window.open(urlLlamadasGral, '_blank');
        //CargarTablaGralSala();

    });
    $("#tabPrincipal").click(function () {
        DetenerTimerLlamadas24H();
        IniciarTimer();
    });
    $("#tabMensajes").click(function () {
        window.open(urlEnvioMensajes, '_blank');
    });
    //Eventos doble click
    $("#NOMB_LLAMANTE").dblclick(function () {
        $(this).val($("#PROP_TELEFONO").val());
    });
    $("#DIRE_LLAMANTE").dblclick(function () {
        $(this).val($("#DIRE_CASO").val().substr(0, 59));
    });
    $("#DIRE_CASO").dblclick(function () {
        ConsultarSitios();
    });

    //Eventos programados sobre los radios check del formulario
   // $("input[name='prioridad']").change(function () {
        //$("#PRIORIDAD").val($(this).val());
   // });
    $("input[name='importancia']").change(function () {
        $("#IMPORTANCIA").val($(this).val());
    });

    $("input[name='todosCanales']").change(function () {
        if ($(this).prop("checked")) {
            $("#TOTAL_CANALES").val("S");
        } else {
            $("#TOTAL_CANALES").val("N");
        }
    });
    $("input[name='mujeres']").change(function () {
        if ($(this).prop("checked")) {
            $("#TIPO_GENERO").val("VG");
        } else {
            $("#TIPO_GENERO").val("CG");
        }
    });
    //asigna evento de doble click barrios
    //$("#txtBarrio").blur(function () {
    //    if ($("#txtBarrio").val() != "") {
    //        $("#ModalBarrios").modal("toggle");
    //        CargarBarrios($("#txtBarrio").val());
    //    }

    //});
    $("#txtBarrio").keypress(function (e) {
        if (e.which == 13) {
            if ($("#txtBarrio").val() != "") {

                CargarBarrios($("#txtBarrio").val());
            }
        }
    });
    //asigna evento de doble click asociadas
    $("#CADPEDI_NUME_LLAMADA").dblclick(function () {
        $("#ModalAsociadas").modal("toggle");
        CargarLlamadasAsociadas("");
    });
    //Evento al presionar un checkbox on switch
    $(".styled, .multiselect-container input").uniform({
        radioClass: 'choice'
    });
    //////console.log("Pedidos");
    CambiarVisibilidadMenu();
    $("#ESTADO").val(glEstado);
    $("#ESTADO").trigger("chosen:updated");
    $("#NUME_TELEFONO").keypress(function (e) {
        if (e.which == 13) {
            ConsultaAbonado();
        }
    });
    //rmr 22-11-2020
    $("#NUME_LLAMADA").keypress(function (e) {
        if (e.which == 13) {
            //ConsultaAbonado();
        }
    });
    //fin rmr 22-11-2020
    $("#DIRE_CASO").keypress(function (e) {
        if (e.which == 13) {
            $("#txtBarrio").focus();
        }
    });
    //$("#NUME_TELEFONO").blur(function () {
    //    ConsultaAbonado();
    //});

    //asigna evento de doble click ciudades
    $("#txtCiudad").dblclick(function () {
        $("#ModalCiudades").modal("toggle");
        CargarCiudades($("#txtCiudad").val());
    });
    //asigna evento de doble click casos
    $("#txtCasoId").dblclick(function () {
        $("#ModalCasos").modal("toggle");
        glCasoText = "txtCaso";
        glCasoId = "txtCasoId";
        glPreguntasOpcion = 1;
        CargarCasos();
    });
    //asigna evento de doble click casos
    $("#txtCasoId2").dblclick(function () {
        $("#ModalCasos").modal("toggle");
        glCasoText = "txtCaso2";
        glCasoId = "txtCasoId2";
        glPreguntasOpcion = 2;
        CargarCasos();
    });
    //asigna evento de doble click barrios
    $("#txtBarrio").dblclick(function () {
        //$("#ModalBarrios").modal("toggle");
        PresentarTablaBarrios($("#txtBarrio").val());
    });
    //
    //$("#tblCanalesAsignados").click(function () {
    //});
});

function ConsultaAbonado() {
    $(".alert").hide();
    if ($("#NUME_TELEFONO").val() === "") {
        glEnUso = false;
        //IniciarTimer();
        return;
    }
    glEnUso = true;
    ConsultarNumeLlamada();
    ConsultarDatosLlamada($("#NUME_TELEFONO").val());
    ConsultarMedioCom($("#NUME_TELEFONO").val());
    //rmr 20-11-2020
    //ConsultarXcrime($("#NUME_TELEFONO").val());
    //rmr 20-11-2020
    $("#mensajeModo").html("Registrando llamada ..");
    $("#txtCasoId").focus();
}
function AplicarChosen() {
    //  $(".chosen-select").chosen({ allow_single_deselect: true, width: "100%" });
    $(".chosen-select").chosen({ allow_single_deselect: false, width: "100%" });
    $("#ddlMedios").click(
        function (e) {
            e.preventDefault();
            return;
        }
    );
    //Activa solo lectura sobre los controles seleccionados
    //$('.lectura option').each(function () {
    //    $(this).attr("disabled", "disabled");
    //});
    //   $(".lectura").trigger("chosen:updated");
}



function ConsultarDatosLlamada(numero) {
    //alert("entro1");
    var urlFinal = urlConsultaDatosLlamada.replace(/amp;/g, "");
    var llamada = $("#NUME_LLAMADA").val();
    var direccion = "Reporte Coord:";
    var dirAux = "";
    //El codigo del barrio será el seleccionado
    $("#LUGE_CODIGO").val($("#txtCiudadId").val());

    //Consulta la información solicitada
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 5000,
        data: { strConexionDijin: glConexionDijin, strTelefono: numero, strNumeracion: glNumeracion, intLugeCad: glLugeCad, strUsuario: globalUsuario },
        beforeSend: function () {
            $("#mensajeAbonado").html("Consultando datos de llamada...");
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                dirAux = data.datos.p_direccion;
                $("#PROP_TELEFONO").val(data.datos.p_propietario.substring(0, 59));
                $("#DIRE_TELEFONO").val(direccion);
                $("#spCiudad").val(data.datos.p_desciudabonado);
                $("#BARR_TELEFONO").val(data.datos.p_barrio);
                $("#LUGE_TELEFONO").val(data.datos.p_codciudad);
                //$("#LUGE_CODIGO").val(data.datos.p_codciudad);
                if ($("#FechaCaso").val() === "") {
                    $("#FechaCaso").val(data.datos.FECHA_HORA);
                }
                //Presenta el mapa
                //DesplegarCasoMapa(data.datos.CONSECUTIVO);
                //glLugeCodigo = data.datos.p_codciudad;
                //if (data.datos.PCORDX === "null") {
                //    $("#LONGITUD_CASO").val(0);
                //} else {
                //    //$("#LONGITUD_CASO").val(data.datos.P_latitud);
                //    $("#LONGITUD_CASO").val(data.datos.PCORDX).trigger("change");
                //}
                //if (data.datos.PCORDY === "null") {
                //    $("#LATITUD_CASO").val(0);
                //} else {
                //    $("#LATITUD_CASO").val(data.datos.PCORDY).trigger("change");
                //}
                //Implementar la consulta de dirección por coordenadas
                $("#TIPO_PEDIDO").val(glTipo);
                $("#TIPO_PEDIDO").trigger("chosen:updated");
                $("#CALI_PEDIDO").val(glCalidad);
                $("#CALI_PEDIDO").trigger("chosen:updated");
            } else {
                GestionarAlerta("warning", data.datos);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (textStatus === "timeout") {
                GestionarAlerta("warning", "Al parecer la consulta de los datos asociados al abonado estan tardando demasiado. Evalúe la posibilidad de inhabilitar las conexiones a DIJIN o TELEPOL.");
                //Deja sin definir los valores
                $("#PROP_TELEFONO").val("No Reportado");
                $("#DIRE_TELEFONO").val("No Reportado");
                $("#BARR_TELEFONO").val("No Reportado");
                $("#LONGITUD_CASO").val(0).trigger("change");
                $("#LATITUD_CASO").val(0).trigger("change");
                $("#spCiudad").val("No Reportado");
            }
        }
    }).done(function (data) {
        $("#mensajeAbonado").html("");

        console.log("Ejecutando ConsultarUbicacion con número:", numero, "y dirección auxiliar:", dirAux);

        ConsultarUbicacion(numero, dirAux);
    });
}

//rmr 20-11-2020 


function ConsultarXcrime(numero, p_numerosecad_txt) {

    //alert("entroxcrime abonado=" + numero + " No de llamada: " + p_numerosecad_txt );
    //VARIABLES DE PRUEBA
    //var p_abonado = "3115918343"; 
    //var p_numerosecad = "48954149";
    //PRODUCCION
    var p_abonado = numero;  //numero;
    var p_numerosecad = p_numerosecad_txt;
    var prioridadaux = "";



    var urlFinal = urlConsultaDatosXcrime.replace(/amp;/g, "");
   
    $.ajax({
        type: "POST",
        url: urlFinal,
        data: { strpabonado: p_abonado, strpnumerosecad: p_numerosecad },
        dataType: 'json',
        cache: false,
        success: function (respuesta) {
            if (respuesta.success) {
                $("#P_IDREGXCRIME").val(respuesta.dataM[0].IdRegxCrime);
                $("#P_FECHALLAMADA").val(respuesta.dataM[0].FechaLlamada);
                $("#txtTipoRegi").val(respuesta.dataM[0].TipoRegistro);
                $("#txtPrioridad").val(respuesta.dataM[0].Prioridad);
                prioridadaux = respuesta.dataM[0].Prioridad;
                if (prioridadaux == "1") {
                   // $("#prioridad3").prop("checked", "checked");

                    document.getElementById("prioridad3").checked = true; 
                    //document.getElementById("red").checked = true;
                }
                if (prioridadaux == "2") {
                  $("#prioridad2").prop("checked", "checked");
                    document.getElementById('prioridad2').checked = true;
                    //$("#prioridad2").trigger("chosen:updated");
                }
                if (prioridadaux == "3") {
                  //  $("#prioridad1").prop("checked", "checked");
                    //$("#prioridad1").trigger("chosen:updated");
                    document.getElementById('prioridad1').checked = true;
                }

                
                $("#txtCategoria").val(respuesta.dataM[0].DescXcrime);
                $("#P_TIPOXCRIME").val(respuesta.dataM[0].TipoXcrime);
                $("#txtDescripcion").val(respuesta.dataM[0].Observacion);
                $("#txtDescriHides").val(respuesta.dataM[0].Transcripcion);
                $("#txtFechaIngBD").val(respuesta.dataM[0].FechaIngbd);

                var Descripcion = document.getElementById("txtDescriHides").value;
                document.getElementById("txtTranscripcion").innerHTML = Descripcion;
                document.getElementById("taPreguntas").innerHTML = Descripcion;

                CargarTablaJson(respuesta.dataM[0].SitioGrabacion, respuesta.dataM[0].Idreg);
                

            } else {
                //GestionarAlerta("warning", "No existe informarcion en Transcripción, revise");
            }
        },
        error: function () {
           
        }
    });
    
}


function CargarTablaJson(V_SitioGraba, V_IdReg) {
    if ($.fn.dataTable.isDataTable("#tblCodigoXCrime")) {
        $("#tblCodigoXCrime").DataTable().destroy();
    }
    $("#tblCodigoXCrime").DataTable({
        "ajax": {
            url: urlConsultaDatosXcrimeDatos,
            data: { SitioGrabacion: V_SitioGraba, Idreg: V_IdReg },
            type: "POST",
            datatype: "json"
        },
        "initComplete": function (settings, json) {
            if (json.success) {
               
            }
            else {
               
            }
        },
        language: glOpcionesIdioma,
        "columns": [
            { "data": "CodiMotivo", "name": "CodiMotivo"},
            { "data": "DescripcionCodigo", "name": "DescripcionCodigo" }
                ],
        lengthMenu: [
            [10, 25, 50, -1],
            ['10 registros', '25 registros', '50 registros', 'Todos']
        ]
            });
        }


function CargarCodigosXCrime(V_SitioGraba, V_IdReg) {

    var urlFinal = urlConsultaDatosXcrimeDatos.replace(/amp;/g, "");
    
    $.ajax({
        type: "POST",
        url: urlFinal,
        data: { SitioGrabacion: V_SitioGraba, Idreg: V_IdReg },
        dataType: 'json',
        cache: false,
        success: function (respuesta) {
            if (respuesta.success) {

                $("#txtFechaIngBD").val(respuesta.DataD[0].CodiMotivo);

                //var o = JSON.parse(respuesta.DataD);//A la variable le asigno el json decodificado
                //$('#tblCodigoXCrime').dataTable({
                //    data: o,
                //    columns: [
                //        { "data": "o.CodiMotivo" },
                //        { "data": "o.DescripcionCodigo" }
                //    ],
                //});

            } else {
                alert("Error");
            }
        },
        error: function () {
            alert("Error2");
        }
    });

}


//function ConsultarXcrime(numero) {
//    alert("entroxcrime");
//    var p_abonado = "3115918343";  //numero;
//    //var p_fecha = $("#HORA_CASO").val();
//    var p_numerosecad = "48954149";
//    var urlFinal = urlConsultaDatosXcrime.replace(/amp;/g, "");
//    //Consulta la información solicitada
//    $.ajax({
//        method: "POST",
//        url: urlFinal,
//        content: "application/json; charset=utf-8",
//        dataType: "json",
//        timeout: 5000,
//        data: { strpabonado: p_abonado, strpnumerosecad: p_numerosecad },
//        beforeSend: function () {
//            $("#mensajeAbonado").html("Consultando datos de llamada...");
//        },
//        success: function (data) {
//            //carga la lista con elementos
//            if (data.result) {
//                //dirAux = data.datos.p_direccion;
//                //$("#PROP_TELEFONO").val(data.datos.p_propietario.substring(0, 59));
//                //$("#DIRE_TELEFONO").val(direccion);
//                //$("#spCiudad").val(data.datos.p_desciudabonado);
//                //$("#BARR_TELEFONO").val(data.datos.p_barrio);
//                //$("#LUGE_TELEFONO").val(data.datos.p_codciudad);
//                ////$("#LUGE_CODIGO").val(data.datos.p_codciudad);
//                //if ($("#FechaCaso").val() === "") {
//                //    $("#FechaCaso").val(data.datos.FECHA_HORA);
//                //}
//                ////Presenta el mapa
//                ////DesplegarCasoMapa(data.datos.CONSECUTIVO);
//                //glLugeCodigo = data.datos.p_codciudad;
//                //if (data.datos.PCORDX === "null") {
//                //    $("#LONGITUD_CASO").val(0);
//                //} else {
//                //    //$("#LONGITUD_CASO").val(data.datos.P_latitud);
//                //    $("#LONGITUD_CASO").val(data.datos.PCORDX);
//                //}
//                //if (data.datos.PCORDY === "null") {
//                //    $("#LATITUD_CASO").val(0);
//                //} else {
//                //    $("#LATITUD_CASO").val(data.datos.PCORDY);
//                //}
//                ////Implementar la consulta de dirección por coordenadas
//                //$("#TIPO_PEDIDO").val(glTipo);
//                //$("#TIPO_PEDIDO").trigger("chosen:updated");
//                //$("#CALI_PEDIDO").val(glCalidad);
//                //$("#CALI_PEDIDO").trigger("chosen:updated");
//            } else {
//                GestionarAlerta("warning", data.datos);
//            }
//        },
//        error: function (jqXHR, textStatus, errorThrown) {
//            if (textStatus === "timeout") {
//                GestionarAlerta("warning", "Al parecer la consulta de los datos asociados al abonado estan tardando demasiado. Evalúe la posibilidad de inhabilitar las conexiones a DIJIN o TELEPOL.");
//                //Deja sin definir los valores
//                $("#PROP_TELEFONO").val("No Reportado");
//                $("#DIRE_TELEFONO").val("No Reportado");
//                $("#BARR_TELEFONO").val("No Reportado");
//                $("#LONGITUD_CASO").val(0);
//                $("#LATITUD_CASO").val(0);
//                $("#spCiudad").val("No Reportado");
//            }
//        }
//    }).done(function (data) {
//        $("#mensajeAbonado").html("");
//        ConsultarUbicacion(numero, dirAux);
//    });
//}
//fin rmr 20-11-2020

function ConsultarNumeLlamada() {
    /*alert("prioridad"+prioridad);
    if (prioridad == 1) {
        $("#prioridad1").prop("checked", "checked");
    }
    if (prioridad == 2) {
        $("#prioridad2").prop("checked", "checked");
    }
    if (prioridad == 3) {
        $("#prioridad3").prop("checked", "checked");
    }*/
    var urlFinal = urlConsultarNumeLlamada.replace(/amp;/g, "");
    var llamada = $("#NUME_LLAMADA").val();
    console.log("esta cosultando");
    //Consulta la información solicitada
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strLlamada: llamada },
        beforeSend: function () {
            $("#mensajeAbonado").html("Consultando consecutivo...");
        },
        success: function (data) {
            //carga la lista con elementos

            if (data.result) {

                $("#NUME_LLAMADA").val(data.llamada);
                $("#NumeLlamada").val(data.llamada);
                //incluir aqui la funcion
                //rmr funcion 22-11-2020
                ConsultarXcrime($("#NUME_TELEFONO").val(), data.llamada);

                

                
                //fin rmr funcion 22-11-2020
            } else {
                GestionarAlerta("warning", data.datos);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (textStatus === "timeout") {
                //GestionarAlerta("warning", "Al parecer la consulta de los datos asociados al abonado estan tardando demasiado. Evalúe la posibilidad de inhabilitar las conexiones a DIJIN o TELEPOL.");
                //Deja sin definir los valores
            }
        }
    }).done(function (data) {
        $("#mensajeAbonado").html("");
        //Presenta el mapa
        DesplegarCasoMapa(data.llamada);
        // CargarCanalesDisponibles();
        });
    //$('#xcrime a:last').tab('show')
}

function ConsultarUbicacion(numero, direccion) {
    var urlFinal = urlUbicacion.replace(/amp;/g, "");
    var llamada = $("#NUME_LLAMADA").val();
    var direccion = "Reporte Coord:";
    //verifica si existe conexion habilitada
    //console.log("telepol"+glConexionTelepol);
    if (glConexionTelepol === "NO") {
       
        return false;
    }
    //Consulta la información solicitada
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 5000,
        data: { strTelefono: numero, strSitio: globalSitio, strLlamada: llamada, strUsuario: globalUsuario },
        beforeSend: function () {
            $("#mensajeAbonado").html("Consultando ubicación...");
        },
        success: function (data) {
            //carga la lista con elementos

            if (data.result) {
                ////console.log("Ubicacion");
                console.log(data.datos);
                direccion = direccion + " " + ((data.datos.POPERADOR === "null") ? "" : data.datos.POPERADOR) + ". " + data.datos.PFECHAGMLC + ". " + direccion;
                // $("#NUME_LLAMADA").val(data.datos.CONSECUTIVO);
                //$("#NumeLlamada").val(data.datos.CONSECUTIVO);
                glLugeCodigo = data.datos.p_codciudad;
                if (data.datos.PCORDX === "null") {
                    $("#LONGITUD_CASO").val(0).trigger("change");
                } else {
                    //$("#LONGITUD_CASO").val(data.datos.P_latitud);
                    $("#LONGITUD_CASO").val(data.datos.PCORDX).trigger("change");
                }
                if (data.datos.PCORDY === "null") {
                    $("#LATITUD_CASO").val(0).trigger("change");
                } else {
                    $("#LATITUD_CASO").val(data.datos.PCORDY).trigger("change");
                }
            } else {
                GestionarAlerta("warning", data.datos);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (textStatus === "timeout") {
                //GestionarAlerta("warning", "Al parecer la consulta de los datos asociados al abonado estan tardando demasiado. Evalúe la posibilidad de inhabilitar las conexiones a DIJIN o TELEPOL.");
                //Deja sin definir los valores
            }
        }
    }).done(function (data) {
        $("#mensajeAbonado").html("");
        //Presenta el mapa
        DesplegarCasoMapa(llamada);
        // CargarCanalesDisponibles();
    });
}
function ConsultarMedioCom(numero) {
    var urlFinal = urlConsultarMedioCom.replace(/amp;/g, "");
    //Consulta la información solicitada
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strNumero: numero, strNumeracion: glNumeracion },
        beforeSend: function () {
            $("#mensajeAbonado").html("Identificando medio com...");
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                if (data.datos != "") {
                    $("#ddlMedios").val(data.datos);
                    $("#ddlMedios").trigger("chosen:updated");
                }
            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
        $("#mensajeAbonado").html("");
    });
}
function CargarCiudades(valor) {
    var ureCiudades = urlConsultarCiudades;
    var localParams = { dcDptoId: glLugarDpto };

    if (glLugarDpto === "") { return; }
    //Carga tabla de ciudades
    if ($.fn.DataTable.isDataTable("#tblCiudades")) {
        $("#tblCiudades").DataTable().destroy();
        $("#tblCiudades tbody").empty();
    }
    ureCiudades = ureCiudades.replace(/amp;/g, "");

    //////console.log(urlRadios);
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.5%" },
        { "data": "Boton", "orderable": false, "width": "2%" },
        { "data": "CODIGO", "orderable": true },
        { "data": "DESCRIPCION", "orderable": true },
        { "data": "TIPO", "orderable": true },
    ];
    AplicarTablaConBusqueda("tblCiudades", ureCiudades, parametrosAjax, localParams, valor);
}
function SeleccionarLugar(codigo, nombre, opcion, codigoDane) {
    switch (opcion) {
        case "CIUDAD":
            $("#txtCiudadId").val(codigo);
            $("#LUGE_CODIGO").val(codigo);
            $("#txtCiudad").val(nombre);
            $("#txtBarrioId").val("");
            $("#txtBarrio").val("");
            $("#ModalCiudades").modal("hide");
            $("#txtBarrio").focus();
            CentrarMapaPorMunicipio(codigoDane); // 81794
            break;
    }
}
function CargarCasos() {
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
function SeleccionarCaso(codigo, descripcion) {
    var subCodigo = "";
    $("#" + glCasoId).val(codigo);
    $("#" + glCasoText).val(descripcion);
    $("#ModalCasos").modal("hide");
    //Consulta las preguntas asociadas al caso
    ConsultarPreguntas(codigo);
    ValidarCaso(codigo);
    if (glPreguntasOpcion === 1) {
        ValidarFuerza
    }
    if (glPreguntasOpcion === 1) {
        $("#txtCasoId2").focus();
    } else {
        $("#txtCiudad").focus();
    }
    //$("#txtCiudad").focus();
}
function ValidarCaso(caso) {
    //Verifica el numero de caso
    subCodigo = caso.substring(0, 3);
    if (subCodigo != "900") {
        glCalidad = "01";
        glEstado = "S";
        $("#CALI_PEDIDO").val(glCalidad);
        $("#CALI_PEDIDO").trigger("chosen:updated");
        $("#ESTADO").val(glEstado);
        $("#ESTADO").trigger("chosen:updated");
    }
}
function ValidarFuerza() {
    if (glFuerza === 10500006 || glFuerza === 11526006) {
        glTipo = "10";
        $("#TIPO_PEDIDO").val(glTipo);
        $("#TIPO_PEDIDO").trigger("chosen:updated");
    }
}
function CargarBarrios(valor) {
    var urlFinal = urlConsultaBarrioNombre.replace(/amp;/g, "");
    var ciudadId = $("#txtCiudadId").val();
    var barrio = $("#txtBarrio").val();
    // ////console.log(ciudadId);
    if (ciudadId === "") { return; }
    if (barrio === "") {
        PresentarTablaBarrios(valor);
    } else {
        //consulta barrio por nombre
        //Consulta la información solicitada
        $.ajax({
            method: "POST",
            url: urlFinal,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: { lugeCodigo: ciudadId, strNombre: barrio },
            beforeSend: function () {
                $("#mensajeBarrio").html("Consultando barrios...");
            },
            success: function (data) {
                //carga la lista con elementos
                if (data.result) {
                    //console.log("cantidad " + data.cantidad);
                    switch (data.cantidad) {
                        case 1:
                            CargarBarrio(data.datos.codigo, data.datos.descripcion, data.datos.comuna);
                            break;
                        case 2:
                            PresentarTablaBarrios(valor);
                            break;
                        case 0:
                            PresentarTablaBarrios(valor);
                            break;
                    }

                } else {
                    GestionarAlerta("warning", data.datos);
                }
            }
        }).done(function (data) {

            $("#mensajeBarrio").html("");

        });
    }

}
function PresentarTablaBarrios(valor) {
    var urlFinal = "";
    var ciudadId = $("#txtCiudadId").val();
    $("#ModalBarrios").modal("show");
    // ////console.log(ciudadId);
    if (ciudadId === "") { return; }
    var localParams = { lugeCodigo: ciudadId };
    //Carga tabla de barrios
    if ($.fn.DataTable.isDataTable("#_tblBarrios")) {
        $("#_tblBarrios").DataTable().destroy();
        $("#_tblBarrios tbody").empty();
    }
    urlFinal = urlConsultarBarrios.replace(/amp;/g, "");

    //////console.log(urlRadios);
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.5%" },
        { "data": "Boton", "orderable": false, "width": "2%" },
        { "data": "CODIGOBARRIO", "orderable": true, "width": "5%" },
        { "data": "DESCRIPCIONBARRIO", "orderable": true },
        { "data": "COMUNA", "orderable": true }
    ];
    AplicarTablaConBusqueda("_tblBarrios", urlFinal, parametrosAjax, localParams, valor);
}
function CargarBarrio(codigo, descripcion, comuna) {
    $("#txtComuna").val(comuna);
    $("#txtBarrioId").val(codigo);
    $("#txtBarrio").val(descripcion);
    $("#ModalBarrios").modal("hide");
    $("#NOMB_LLAMANTE").focus();
}
function ConsultarPreguntas(caso) {
    var urlFinal = urlConsultarPreguntas.replace(/amp;/g, "");

    //Consulta la información solicitada
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strCaso: caso },
        beforeSend: function () {
            if (glPreguntasOpcion === 1) {
                $("#mensajeCaso").html("Cargando preguntas asociadas...");
            } else {
                $("#mensajeCaso2").html("Cargando preguntas asociadas...");
            }
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                if (glPreguntasOpcion === 1) {
                    glPreguntasCaso = data.datos;
                } else {
                    glPreguntasCaso2 = data.datos;
                }
                $("#taPreguntas").val($("#taPreguntas").val() + glPreguntasCaso + "" + glPreguntasCaso2);
            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
        if (glPreguntasOpcion === 1) {
            $("#mensajeCaso").html("");
        } else {
            $("#mensajeCaso2").html("");
        }
    });
}

function CargarCanales(opcion) {
    if (glCerrar) {
        return;
    }
    var visible = true;
    var urlFinal = "";
    var strTabla = "tblCanales";
    var localParams = { strOpcion: opcion, strNumeroLlamada: $("#NUME_LLAMADA").val(), strSitio: $("#SITIO_GRABA").val() };
    if (opcion === "PONAL") {
        strTabla = "tblCanalesPonal";
    }
    if (opcion === "ASIGNADOS" || opcion === "CONSULTA") {
        strTabla = "tblCanalesAsignados";
    }
    //Verifica existencia de tabla
    if ($.fn.DataTable.isDataTable("#" + strTabla)) {
        $("#" + strTabla).DataTable().destroy();
        $("#" + strTabla + " tbody").empty();
    }
    urlFinal = urlConsultarCanales.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales

    var parametrosAjax = [
        { "data": "Entidad", "orderable": true },
        { "data": "DESCRIPCION", "orderable": true, "width": "2%" },
        { "data": "Boton", "orderable": false, "width": "2%" }
    ];
    if (opcion === "ASIGNADOS" || opcion === "CONSULTA") {
        if (glModo) {
            //  visible = false;
        }
        parametrosAjax = [
            { "data": "Entidad", "orderable": true },
            { "data": "Boton", "orderable": false, "width": "2%", "visible": visible },
            { "data": "DESCRIPCION", "orderable": true },
            { "data": "Estado", "orderable": true },
            { "data": "Evento", "orderable": true }
        ];
    }
    AplicarPluginTablaParams(strTabla, urlFinal, parametrosAjax, localParams);
}

function SeleccionarCanal() {
    var tabla = $('#tblCanalesAsignados').DataTable();
    tabla.row.add(["valor", "valor", "valor", "valor", btnDesasignar]).draw(false);
}

function ValidarPedidoCanales() {
    var caso = $("#txtCasoId").val();
    var direccion = $("#DIRE_CASO").val();
    var descripcion = $("#taPreguntas").val();
    var calidad = $("#CALI_PEDIDO").val();
    var barrio = $("#txtBarrio").val();
    var barrioId = $("#txtBarrioId").val();
    var puntoCARDINAL = $("#ddlCoord").val();
    //console.log("valor punto " + puntoCARDINAL);
    if (puntoCARDINAL === "" || typeof (puntoCARDINAL) === "undefined") {
        $("#ddlCoord").val(1);
        $("#ddlCoord").trigger("chosen:updated");
        //console.log("valor punto actualizado" + $("#PUCA_CODIGO").val());
    }
    if ($("#NUME_TELEFONO").val() === "") {
        GestionarAlerta("warning", "Revise el nro telefonico ya que esta en blanco..");
        return false;
    }
    if ($("#NUME_LLAMADA").val() === "") {
        GestionarAlerta("warning", "No ha generado un número de llamada para el caso actual..");
        return false;
    }
    //rmr 05-12-2020

    if (glConexionMapa == "SI") {
        if (($("#LONGITUD_CASO").val() === "0") || ($("#LONGITUD_CASO").val() === "")) {
            GestionarAlerta("warning", "Seleccione pestaña Mapa GeoInversa y ubique el punto de ubicación geografica de la llamada ..");
            return false;
        }
        if (($("#LONGITUD_CASO").val() === "0") || ($("#LONGITUD_CASO").val() === "")) {
            GestionarAlerta("warning", "Seleccione pestaña Mapa GeoInversa y ubique el punto de ubicación geografica de la llamada ..");
            return false;
        }
    }
    //fin rmr 05-12-2020
    if (!glCerrar) {
        if ($("#txtCasoId").val() === "") {
            GestionarAlerta("warning", "Revise el codigo de pedido ya que esta en blanco..");
            return false;
        }
        if (caso === "900") {
            GestionarAlerta("danger", "Si el caso es negativo no debe enviar canales de despacho..");
            return false;
        }
        if (direccion === "" || direccion === null) {
            GestionarAlerta("danger", "Debe Diligenciar el campo Direccion caso..");
            return false;
        }
        if (descripcion === "" || descripcion === null) {
            GestionarAlerta("danger", "Digite la descripcion del caso.");
            return false;
        }
        if (calidad != "01") {
            if (calidad != "06") {
                GestionarAlerta("danger", "Si el pedido no es efectivo no debe Enviar Canales de despacho");
                return false;
            }
        }
        if (barrio !== "" && barrioId === "") {
            GestionarAlerta("danger", "El barrio ingresado no es válido, por favor revise el barrio ingresado.");
            return false;
        }
    }

    return true;

}
function GuardarPedido() {
    //console.log("va a cerrar" + glCerrar);
    //Reliza la validación previa de campos
    if (!ValidarPreCreacion()) {
        //console.log("NO VALIDO");
        return;
    }
    //Verifica si el pedido ya ha sido creado
    if (glCreado && !glCerrar) {
        CargarCanales("ASIGNADOS");
        return;
    }
    if (glCerrar) {
        //console.log("va a cerrar" + glCerrar);
        GestionarAlerta("info", "Cerrando el pedido (caso) Por favor espere...");
        GenerarPedido();
        return;
    }
    //GenerarPedido();
    var fechaCaso = $("#FECH_CASO").val();
    var codigoCaso = $("#txtCasoId").val();
    var direCaso = $("#DIRE_CASO").val();
    var lugeCodigo = $("#txtCiudadId").val();
    var calidad = $("#CALI_PEDIDO").val();
    //console.log(fechaCaso);
    $.ajax({
        method: "POST",
        url: urlValidarRepetido,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strFechaCaso: fechaCaso, strCodigoCaso: codigoCaso, strDireCaso: direCaso, strLugeCodigo: lugeCodigo },
        beforeSend: function () {
            GestionarAlerta("info", "Guardando el pedido (caso) y cargando canales...");
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                //Presenta pantalla de verificación
                swal({
                    title: "Caso repedito",
                    text: "Este caso ya fue reportado. Generar Nuevo Evento o Asociar al evento ya registrado",
                    type: "warning",
                    html: true,
                    showCancelButton: true,
                    confirmButtonColor: "#EF5350",
                    cancelButtonColor: "#EF5385",
                    confirmButtonText: "Asociar",
                    cancelButtonText: "Generar",
                    closeOnConfirm: true,
                    closeOnCancel: true
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            $("#CADPEDI_SITIO_GRABA").val($("#SITIO_GRABA"));
                            $("#CADPEDI_NUME_LLAMADA").val(data.datos.LLAMADA);
                            $("#CALI_PEDIDO").val("06");
                            $("#CALI_PEDIDO").trigger("chosen:updated");
                            $("#ESTADO").val("C");
                            $("#ESTADO").trigger("chosen:updated");
                        }
                    }
                );
            } else {
                if (data.datos != "NUEVO") {
                    GestionarAlerta("warning", data.datos);
                    return;
                }
            }
            //Verifica la calidad del pedido
            if (calidad === "06" && $("#CADPEDI_NUME_LLAMADA").val() === "") {
                GestionarAlerta("warning", "Si es una llamada redundante debe haber una llamada asociada..");
                return;
            }
            if (codigoCaso === "913" && (glFuerza != 10500006 && glFuerza != 11526006)) {
                $("#TOTAL_CANALES").val("S");
            }
            GenerarPedido();
        }
    });
}
function GenerarPedido() {
    var urlFinal = urlCrearPedido.replace(/amp;/g, "");
    var datos = $("#formPedido").serialize();

    //Realiza la creación del pedido
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {
            GestionarAlerta("info", "Guardando el pedido (caso) y cargando canales...");
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                glCreado = true;
                //Asigna la gecha de grabacion
                $("#FECHA_GRABACION").val(data.fechaGraba);
                CargarCanales("ASIGNADOS");
                CargarCanales("PONAL");
                CargarCanales("NORMAL");
                GestionarAlerta("success", "Pedido (caso) generado de manera exitosa...");
                //El pedido es eviado si se esta cerrando
                if (glCerrar) {
                    //console.log("Envia pedido");
                    EnviarPedido();
                }
            } else {
                //console.log("Envia pedido problema");
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
    });
}
function PasarDireccion() {
    $("#DIRE_CASO").val($("#DIRE_TELEFONO").val());
}

function GestionarCanal(entidadId, canalId, opcion) {
    $("#CADCANA_FUERZ_ID").val(entidadId);
    $("#CADCANA_CODIGO").val(canalId);
    var sitio = $("#SITIO_GRABA").val();
    var numero = $("#NUME_LLAMADA").val();
    $("#CADPEDI_SITIOGRABA").val(sitio);
    $("#CADPEDI_NUMELLAMADA").val(numero);
    if (!glCreado) {
        GestionarAlerta("warning", "Aún no se ha guardado el pedido.");
        return;
    }
    if (opcion === "CREAR") {
        var urlFinal = urlCrearCanal.replace(/amp;/g, "");
        var datos = $("#formPedido").serialize();
        //Consulta la información solicitada
        $.ajax({
            method: "POST",
            url: urlFinal,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: datos,
            beforeSend: function () {
                // GestionarAlerta("info", "Asignando el canal...");
            },
            success: function (data) {
                //carga la lista con elementos
                if (data.result) {
                    glCreado = true;
                    //GestionarAlerta("success", "Canal asignado de manera exitosa...");
                    CargarCanales("ASIGNADOS");
                    CargarCanales("PONAL");
                    CargarCanales("NORMAL");
                } else {
                    GestionarAlerta("warning", data.datos);
                }
            }
        }).done(function (data) {
            $("#alertaJs").empty();
        });
    } else {
        var urlFinal = urlRemoverCanal.replace(/amp;/g, "");
        var datos = { intSitio: sitio, lgNumero: numero, lgEntidad: entidadId, lgCanal: canalId };
        //Consulta la información solicitada
        $.ajax({
            method: "POST",
            url: urlFinal,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: datos,
            beforeSend: function () {
                //GestionarAlerta("info", "Removiendo el canal...");
            },
            success: function (data) {
                //carga la lista con elementos
                if (data.result) {
                    glCreado = true;
                    //GestionarAlerta("success", "Canal Removido de manera exitosa...");
                    CargarCanales("ASIGNADOS");
                    CargarCanales("PONAL");
                    CargarCanales("NORMAL");
                } else {
                    GestionarAlerta("warning", data.datos);
                }
            }
        }).done(function (data) {
            $("#alertaJs").empty();

        });
    }
}

function IniciarPedido() {
    if (ValidarPedidoCanales()) {
        GuardarPedido();
    }
}
function Actualizar() {
    //Reliza la validación previa de campos
    if (!ValidarPreCreacion()) {
        return;
    }
    var urlFinal = urlActualizarPedido.replace(/amp;/g, "");
    var datos = $("#formPedido").serialize();
    //Consulta la información solicitada
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {
            GestionarAlerta("info", "Guardando el pedido (caso) y cargando canales...");
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                glCreado = true;
                //si el pedido va a ser enviado entonces no carga las tablas de canales
                if (!glEnviarPedido) {
                    CargarCanales("ASIGNADOS");
                    CargarCanales("PONAL");
                    CargarCanales("NORMAL");
                }

            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
        GestionarAlerta("success", "Pedido (caso) guardado de manera exitosa...");
    });
}
function ActualizarPedido() {
    if (ValidarPedidoCanales()) {
        if (glCreado) {
            Actualizar();
        } else {
            GuardarPedido();
        }
    }
}



function EnviarPedido() {
    glEnviarPedido = true;
    var reja = 0;
    if ((ValidarPedidoCanales()) || glCerrar) {
        //actualiza el pedido antes de enviarlo
        if (glCreado) {
            Actualizar();
        } else {
            GuardarPedido();
        }
        //Valida que exisa a lo menos un canal de atención asignado
        var table = $('#tblCanalesAsignados').DataTable();

        if (!table.data().count() && ($("#txtCasoId").val() !== "900")) {
            //rmr 05-12-2020
            if (glConexionMapa == "SI")
            {
                if (($("#LONGITUD_CASO").val() === "0") || ($("#LONGITUD_CASO").val() === "")) {
                    reja = 1;
                    GestionarAlerta("danger", "Seleccione pestaña Mapa GeoInversa y ubique el punto de ubicación geografica de la llamada ..");
                    return false;
                }
                if (($("#LONGITUD_CASO").val() === "0") || ($("#LONGITUD_CASO").val() === "")) {
                    reja = 1;
                    GestionarAlerta("danger", "Seleccione pestaña Mapa GeoInversa y ubique el punto de ubicación geografica de la llamada ..");
                    return false;
                    }
            }
            //fin rmr 05-12-2020
            if (reja == 0)
            {
            GestionarAlertaModal("danger", "Debe Asignar un canal de atención antes de enviar el pedido.", "mnsCanales");
            $("#alertaJs").empty();
                return;
            }
        }
        //  ////console.log("Validado de manera correcta");
        // 04-12-2019 respuesta wsdl
        //var wsdl = $("#RESPUESTA_WS_NEW").val();
        //var wsdl = '@Session["wsdl"]';
        var wsdl = sessionStorage.getItem("wsdl");
        // 04-12-2019 respuesta wsdl
        var sitio = $("#SITIO_GRABA").val();
        var numeroLlamada = $("#NUME_LLAMADA").val();
        var telefono = $("#NUME_TELEFONO").val();
        var pedido = $("#txtCasoId").val();
        var urlFinal = urlEnviarPedido.replace(/amp;/g, "");
        var datos = { strSitio: sitio, strNumeroLlamada: numeroLlamada, strTelefono: telefono, strPedido: pedido };
        //Consulta la información solicitada
        $.ajax({
            method: "POST",
            url: urlFinal,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: datos,
            beforeSend: function () {
                //GestionarAlerta("info", "Asignando el canal...");
            },
            success: function (data) {
                //carga la lista con elementos
                if (data.result) {
                    window.location.href = urlEnviarRecepcion + "?strMensaje=" + "Pedido (" + numeroLlamada + ") Enviado de manera exitosa " + data.datowsdl;
                    ////console.log("Se envio");
                } else {
                    GestionarAlerta("warning", data.datos);
                }
            }
        }).done(function (data) {
            // GestionarAlerta("success", "Canal asignado de manera exitosa...");
        });
    }
}
function ValidarPreCreacion() {
    var codigoCaso = $("#txtCasoId").val();
    var calidad = $("#CALI_PEDIDO").val();

    if ($("#NUME_TELEFONO").val() === "") {
        GestionarAlerta("warning", "Revise el nro telefonico ya que esta en blanco..");
        return false;
    }
    if ($("#txtCasoId").val() === "") {
        GestionarAlerta("warning", "Revise el codigo de pedido ya que esta en blanco..");
        return false;
    }
    if ($("#PUCA_CODIGO").val() === "") {
        $("#PUCA_CODIGO").val(1);
        $("#PUCA_CODIGO").trigget("chosen:updated");
    }
    if ($("#DIRE_LLAMANTE").val() === "") {
        $("#DIRE_LLAMANTE").val("NO REPORTA");
    }
    if (codigoCaso === "900") {
        $("#ESTADO").val("C");
        $("#ESTADO").trigger("chosen:updated");
        if ($("#DIRE_CASO").val() === "") {
            $("#DIRE_CASO").val("NO REPORTA");
        }
        if ($("#DIRE_LLAMANTE").val() === "") {
            $("#DIRE_LLAMANTE").val("NO REPORTA");
        }
    } else {
        if ($("#DIRE_CASO").val() === "") {
            GestionarAlerta("warning", "Revise la dirección del caso ya que esta en blanco..");
            return false;
        }
        //Verifica la calidad del pedido
        if (calidad != "06") {
            $("#CALI_PEDIDO").val("01");
            $("#CALI_PEDIDO").trigger("chosen:updated");
        } else {
            $("#ESTADO").val("S");
            $("#ESTADO").trigger("chosen:updated");
        }
    }
    if (calidad != "01") {
        $("#ESTADO").val("C");
        $("#ESTADO").trigger("chosen:updated");
    }
    if ($("#taPreguntas").val() === "" || $("#taPreguntas").val() === null) {
        GestionarAlerta("danger", "Digite la descripcion del caso.");
        return false;
    }
    if ($("#DIRE_LLAMANTE").val() === "") {
        GestionarAlerta("warning", "Revise la dirección del llamante..");
        return false;
    }
    return true;
}
function CerrarPedido(opcion) {

    var codigoPedido = $("#txtCasoId").val().replace(/amp;/g, "");
    var telefono = $("#NUME_TELEFONO").val();
    if (telefono === "") {
        GestionarAlerta("danger", "No se ha ingresado un número teléfonico/celular.");
        return;
    }
    if (codigoPedido !== "") {
        GestionarAlerta("danger", "Ya hay una emergencia indicada.. Si la desea cambiar por favor borre los códigos.");
        return;
    }
    ConsultarDatosLlamada(telefono);
    $("#TIPO_PEDIDO").val("01");

    $("#txtCasoId").val("900");
    $("#IMPORTANCIA").val("01");
    //$("#PRIORIDAD").val("02");

    $("#ESTADO").val("C");
    $("#ENVIAR").val("N");
    $("#LUGE_CODIGO").val(glLugeCad);
    glCerrar = true;
    switch (opcion) {
        case "INFORMACION":
            $("#CALI_PEDIDO").val("05");
            $("#taPreguntas").val("Solicitan Información.");
            break;
        case "PERSONAMOLESTANDO":
            $("#CALI_PEDIDO").val("02");
            $("#taPreguntas").val("Persona Molestando.");
            break;
        case "NINOMOLESTADO":
            $("#CALI_PEDIDO").val("09");
            $("#taPreguntas").val("Niños Molestando.");
            break;
        case "LLAMADAINSULTANTE":
            $("#CALI_PEDIDO").val("10");
            $("#taPreguntas").val("Llamada Insultante.");
            break;
        case "COLGARON":
            $("#CALI_PEDIDO").val("13");
            $("#taPreguntas").val("No se escucha nada.");
            break;
    }
    //Almacena y envia el pedido
    GuardarPedido();
}

function CargarTablaLlamadas24h() {
    if ($.fn.DataTable.isDataTable("#tablaUsu")) {
        $("#tablaUsu").DataTable().destroy();
        $("#tablaUsu tbody").empty();
    }
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false },
        { "data": "NUME_LLAMADA" },
        { "data": "HORA_CASO" },
        { "data": "NUME_TELEFONO" },
        { "data": "TIPO_PEDIDO" },
        { "data": "NOMB_LLAMANTE" },
        { "data": "LUGE_CODIGO" },
        { "data": "DIRE_CASO" },
        { "data": "COMENTARIO" },
        { "data": "CODI_PEDIDO" },
        { "data": "ESTADO" },
        { "data": "CADUSUA_USUARIO" }
    ];

    AplicarPluginTablaAjax("tablaUsu", urlTabla + "?usuario=" + globalUsuario, parametrosAjax);
}

function AbrirModalComentarios() {
    var caliPedido = $("#CALI_PEDIDO").val();
    var estado = $("#ESTADO").val();
    if (caliPedido != "01") {
        GestionarAlerta("warning", "Si el caso no es efectivo no debe adicionar comentarios..");
        return;
    }
    if (estado === "C") {
        GestionarAlerta("warning", "Si el caso esta cerrado no puede adicionar comentarios.. ");
        return;
    }
    if (!glCreado) {
        GestionarAlerta("warning", "El caso no ha sido generado.. ");
        return;
    }
    $("#ModalComentarios").modal("toggle");
    CargarTablaComentarios();
}

function GuardarComentarios() {
    var anotacion = $("#ANOTACION").val();

    if (anotacion === "") {
        GestionarAlertaModal("warning", "Debe completar el comentario.", "alertasComentarios");
        return;
    }
    urlGuardarComentarios
    //Genera el comentario
    var urlFinal = urlGuardarComentarios.replace(/amp;/g, "");
    var datos = $("#formComentarios").serialize();
    //Realiza la creación del pedido
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {
            //GestionarAlerta("info", "Guardando el pedido (caso) y cargando canales...");
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                CargarTablaComentarios();
                GestionarAlertaModal("success", "Se ha generado el comentario de manera exitosa.", "alertasComentarios");
                $("#ANOTACION").val("");
            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
        $("#CONSECUTIVO").val(0);
    });
}
function EditarComentario(sitio, llamada, consecutivo, anotacion) {
    $("#ANOTACION").val(anotacion);
    $("#CONSECUTIVO").val(consecutivo);
}
function CargarTablaComentarios() {
    var urlFinal = "";
    var sitio = $("#SITIO_GRABA").val();
    var llamada = $("#NUME_LLAMADA").val();
    ////console.log(llamada);
    ////console.log(sitio);
    var localParams = { strSitio: sitio, strNumeroLlamada: llamada };
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblComentarios")) {
        $("#tblComentarios").DataTable().destroy();
        $("#tblComentarios tbody").empty();
    }
    urlFinal = urlConsultarComentarios.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    //////console.log(urlRadios);
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.5%" },
        { "data": "Boton", "orderable": false, "width": "2%" },
        { "data": "ANOTACION", "orderable": true },
        { "data": "USUA_USUARIO", "orderable": true },
        { "data": "Fecha", "orderable": false }
    ];
    AplicarPluginTablaParams("tblComentarios", urlFinal, parametrosAjax, localParams);
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
            if (glPreguntasOpcion === 1) {
                $("#mensajeCaso").html("Identificando el caso ingresado...");
            } else {
                $("#mensajeCaso2").html("Identificando el caso ingresado...");
            }
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                $("#mensajeCaso").html("");
                $("#mensajeCaso2").html("");
                if (data.datos === "") {
                    if (glPreguntasOpcion === 1) {
                        $("#mensajeCaso").html("No existe el caso ingresado...");
                        $("#" + glCasoText).val("");
                        $("#txtCasoId").focus();
                    } else {
                        $("#mensajeCaso2").html("No existe el caso ingresado...");
                        $("#" + glCasoText).val("");
                        $("#txtCasoId2").focus();
                    }
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

function CompletarCiudad(codigo) {
    if (codigo === "") {
        $("#txtCiudadId").val("");
        return;
    }
    var urlFinal = urlConsultarCiudades.replace(/amp;/g, "");
    var datos = { dcDptoId: glLugarDpto, strCiudadId: codigo };

    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {
            $("#mensajeCiudad").html("Identificando la ciudad ingresada...");
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                $("#mensajeCiudad").html("");
                if (data.datos.CODIGO === "") {
                    $("#mensajeCiudad").html("No existe la ciudad ingresada...");
                    $("#txtCiudadId").val("");
                    $("#LUGE_CODIGO").val(0);
                    $("#txtCiudad").focus();
                } else {
                    SeleccionarLugar(data.datos.CODIGO, data.datos.NOMBRE, "CIUDAD");
                }
            } else {
                $("#mensajeCiudad").html("No existe la ciudad ingresada...");
                $("#txtCiudad").focus();
            }
        }
    }).done(function (data) {
    });
}

function CargarTablaPedidos() {

    var urlFinal = "";
    var localParams = { strSitio: glSitio, strUsuario: glUsuario, strOpcion: glModo };
    ////console.log(glModo);
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblPedidos")) {
        $("#tblPedidos").DataTable().destroy();
        $("#tblPedidos tbody").empty();
    }
    urlFinal = urlConsultaPedido.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    //////console.log(urlRadios);
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.5%" },
        { "data": "Boton", "orderable": false, "width": "2%" },
        { "data": "NUME_LLAMADA", "orderable": true },
        { "data": "NUME_TELEFONO", "orderable": true },
        { "data": "CODI_PEDIDO", "orderable": true },
        { "data": "CADPEDI_NUME_LLAMADA", "orderable": true },
        { "data": "SITIO_GRABA", "orderable": true },
        { "data": "PROP_TELEFONO", "orderable": true },
        { "data": "HORA_CASO", "orderable": true },
        { "data": "CADUSUA_USUARIO", "orderable": true },
        { "data": "COMENTARIO", "orderable": true }
    ];
    AplicarPluginTablaPedidosRecepcion("tblPedidos", urlFinal, parametrosAjax, localParams);

}
///////////////TIMER PARA OBTENER NUEVA LLAMADA/////////////////////
function IniciarTimer() {
    //console.log("--------------Timer recepcion iniciado");
    //timerRecepcionId = setTimeout(ConsultarLlamada(), glTiempo);//Tiempo en milisegundos
    timerRecepcionId = setInterval(function () { ConsultarLlamada(); }, glTiempo);//Tiempo en milisegundos
    // var id = setInterval(function () { ////console.log("intervalo"); },1000);
}

function ConsultarLlamada() {
    var numTelefono = 0;
    ////console.log("timer Id: " + timerRecepcionId);
    ////console.log(glEnUso);
    ////console.log("tiempo: " + glTiempo);
    if (glEnUso) {
        DetenerTimer();
        return;
    }
    //Consulta si existe una nueva llamada
    var urlFinal = urlConsultarLlamada;
    var datos = { intSitio: $("#SITIO_GRABA").val(), strACD: glACD };
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {
            //GestionarAlerta("info", "Consultando llamadas...");
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                //////console.log("Nunmero telefonico:"+numTelefono);
                numTelefono = data.datos.NUMERO;
                if (numTelefono <= 0) {
                    return;
                } else {
                    //Detiene el timer
                    glEnUso = true;
                    DetenerTimer();
                    //Carga y consulta el numero telefonico encontrado
                   

                    $("#NUME_TELEFONO").val(numTelefono);
                    $("#mensajeModo").html("Llamada Entrante ..");
                    ConsultarNumeLlamada();
                    ConsultarDatosLlamada($("#NUME_TELEFONO").val());
                    ConsultarMedioCom($("#NUME_TELEFONO").val());
                    ConsultarUbicacionCelular(numTelefono);   // <-- nueva línea
                    $("#mensajeModo").html("Registrando llamada ..");





                }
            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
        //$("#alertaJs").empty();

        //GestionarAlerta("success", "Pedido (caso) guardado de manera exitosa...");
    }).fail(function (jqXHR, textStatus, errorThrown) {
        ////console.log("conexion caida, timer detenido");
        DetenerTimer();
    });
    //clearInterval(timerRecepcionId);
    //timerRecepcionId = setTimeout(ConsultarLlamada(), glTiempo);//Tiempo en milisegundos
}
function test() {
    alert("ss");
}




function ConsultarUbicacionCelular(numTelefono) {
    var urlFinal = urlObtenerUbicacionCelular; // definir en la vista igual que urlConsultarLlamada
    var datos = { strNumeroTelefono: numTelefono };

    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {
            $("#mensajeInversa").html("Consultando ubicación...");
        },
        success: function (data) {
            if (data.result) {
                $("#LATITUD_CASO").val(data.latitud).trigger("change");
                $("#LONGITUD_CASO").val(data.longitud).trigger("change");
                $("#mensajeInversa").html("");
            } else {
                $("#LATITUD_CASO").val("");
                $("#LONGITUD_CASO").val("");
                $("#mensajeInversa").html(data.datos);
            }
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        $("#mensajeInversa").html("No fue posible consultar la ubicación.");
    });
}

///Limpia el formulario de recepción
function Limpiar() {
    //Presenta pantalla de verificación
    if (glModo === "") {
        swal({
            title: "Atención!",
            text: "¿Está seguro de cancelar la solicitud?. La información del caso no será guardada en la base de datos.",
            type: "warning",
            html: true,
            showCancelButton: true,
            confirmButtonColor: "#EF5350",
            cancelButtonColor: "#EF5385",
            confirmButtonText: "Aceptar",
            cancelButtonText: "Cancelar",
            closeOnConfirm: true,
            closeOnCancel: true
        },
            function (isConfirm) {
                if (isConfirm) {
                    window.location.href = urlLimpiar;
                }
            });
    } else {
        window.location.href = urlLimpiar;
    }

}
///////////////////////////////////////////////////////////////////////////////////////////////
////////////////funciones para la consulta y revisión de casos/////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////
function CargarLlamadasAsociadas(valor) {
    var urlFinal = "";
    var fechaHora = $('#FechaCaso').val();
    var llamada = $("#NUME_LLAMADA").val();
    if (llamada === "") {
        GestionarAlerta("warning", "No se ha asignado un número de llamada.");
        return;
    }
    var localParams = { strSitio: globalSitio, strHoraCaso: fechaHora, strLlamada: llamada };
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblAsociadas")) {
        $("#tblAsociadas").DataTable().destroy();
        $("#tblAsociadas tbody").empty();
    }
    urlFinal = urlAsociadas.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    //////console.log(urlRadios);
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.5%" },
        { "data": "Boton", "orderable": false, "width": "2%" },
        { "data": "Llamada", "orderable": true },
        { "data": "HoraCaso", "orderable": true },
        { "data": "NumeTelefono", "orderable": true },
        { "data": "Calidad", "orderable": false },
        { "data": "Ciudad", "orderable": false },
        { "data": "Llamante", "orderable": false },
        { "data": "Dire_Caso", "orderable": false },
        { "data": "Estado", "orderable": false },
        { "data": "Sitio", "orderable": false },

    ];
    AplicarTablaConBusqueda("tblAsociadas", urlFinal, parametrosAjax, localParams, valor);
}
function SeleccionarAsociada(llamada, sitio) {
    //////console.log(sitio);
    $("#CADPEDI_NUME_LLAMADA").val(llamada);
    $("#CADPEDI_SITIO_GRABA").val(sitio);
    $("#ModalAsociadas").modal("hide");
}
function CargarCanalesConsulta() {
    CargarCanales("ASIGNADOS");
    if (glModo === "CONSULTA") {
        CargarCanales("PONAL");
        CargarCanales("NORMAL");
    }
}
function MarcarImportantia(valor) {
    switch (valor) {
        case "01":
            $("#baja").prop("checked", "checked");
            break;
        case "02":
            $("#media").prop("checked", "checked");
            break;
        case "03":
            $("#alta").prop("checked", "checked");
            break;
    }
}

function MarcarPrioridad(valor) {
    switch (valor) {
        case "01":
            $("#rutina").prop("checked", "checked");
            break;
        case "03":
            $("#inmd").prop("checked", "checked");
            break;
        case "04":
            $("#flash").prop("checked", "checked");
            break;
    }
}
function MarcarCanales(valor) {
    if (valor === "S") {
        $("#todosCanales").prop("checked", "checked");
    }
}
function MarcarMujer(valor) {
    if (valor === "VG") {
        $("#mujeres").prop("checked", "checked");
    }
}
function ActivarModoConsulta() {
    glCreado = true;
    //console.log(glModo);
    //Verifica si debe ocultar elementos
    if (glModo === "REVISION") {
        $("#botonesControl").hide();
        $("#asociadas").hide();
        $("#tabMensajes").hide();
        $("#usu24").hide();
        $("#tabgral").hide();
        $("#btnValidarGuaradar").hide();
        $("#btnRedesSociales").hide();
        $("#btnComentarios").hide();
        $("#tablasParaAsignar").hide();
        $("#resumenCaso").removeClass("hidden");
        $("#validarDir").removeClass("hidden");
        $("#NOMB_LLAMANTE").prop("readonly", "readonly");
        $("#DIRE_LLAMANTE").prop("readonly", "readonly");
        $("#DIRE_CASO").prop("readonly", "readonly");
        $("#taPreguntas").prop("readonly", "readonly");
        glControlRevision = true;
        return false;
    }
    if (glModo === "CONSULTA") {
        $
        $("#btnValidarGuaradar").hide();
        $("#btnRedesSociales").hide();
        $("#btnComentarios").hide();
        $("#btnRedesSociales").hide();
        //$("#tablasParaAsignar").hide();

        //$("#resumenCaso").removeClass("hidden");
        //$("#validarDir").removeClass("hidden");
        $("#NOMB_LLAMANTE").prop("readonly", "readonly");
        $("#DIRE_LLAMANTE").prop("readonly", "readonly");
        $("#DIRE_CASO").prop("readonly", "readonly");
        $("#taPreguntas").prop("readonly", "readonly");
        //Oculta opciones para realizar cambios sobre el registro del pedido
        $("#btnValidarGuaradar").hide();
        //$("#btnEnviarPedido").hide();
        $("#btnGuardarPedido").hide();
        glControlRevision = false;
    }

}
function ConsultarLlamadaAsociada() {
    var sitio = $("#CADPEDI_SITIO_GRABA").val();
    var llamada = $("#CADPEDI_NUME_LLAMADA").val();
    ////console.log(sitio);
    ////console.log(llamada);
    if (sitio === "" || llamada === "") {
        // $("#tabIngreso").trigger("click");
        $('#tabIngreso a:last').tab('show')
        GestionarAlerta("warning", "No existe una llamada asociada.");
        return;
    }
    $("#formPedido").attr('action', urlConsultarAsociada);
    $("#formPedido").submit();
}

function CargarCanalesDisponibles() {
    CargarCanales("PONAL");
    CargarCanales("NORMAL");
}

function ActivarEstado(estado) {
    glEstado = estado;
    ////console.log(estado);
    ////console.log($("#ESTADO").val());
    //$("#ESTADO").val(estado);
    //$("#ESTADO option[value='"+estado+"]").prop('selected', true);
    //////console.log($("#ESTADO").val());
    //$("#ESTADO").trigger("change");
}


/***********RMR 17-11-2020*/
//$(document).ready(function () {
//    activaTab('XGeoinv');
//});

//function activaTab(tab) {
//    //alert("url mapa");
//    $('.nav-tabs a[href="#' + tab + '"]').tab('show');
//};
///*
//function DesplegarCasoMapaRMR() {
//    $("#frameMapa").attr("src", urlMapa + "?strNumeLlamada=" + numeLlamada);
//    alert("url mapa");
//}*/
///***********RMR 17-11-2020*/


function DesplegarCasoMapa(numeLlamada) { 
    numellamadarecargar = numeLlamada; 
    $("#frameMapa").attr("src", urlMapa + "?strNumeLlamada=" + numeLlamada);
}
function AbrirMapa() {
    if ($("#NUME_LLAMADA").val() === "") {
        return;
    }
    var win = window.open(urlMapaGeneral + "?strNumeLlamada=" + $("#NUME_LLAMADA").val(), '_blank');
    win.focus();
}

function AbrirDocumentos() {
    if (!glCreado) {
        GestionarAlerta("warning", "Debe primero generar un caso.");
        return;
    }
    var llamada = $("#NUME_LLAMADA").val();
    $("#frameArchivos").attr("src", "");
    $("#frameArchivos").attr("src", urlCrearDocumentos + "?intSitio=" + globalSitio + "&lgNumeLlamada=" + llamada + "&strUsuario=" + globalUsuario);
    $("#ModalArchivos").modal("show");
}
///////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////GEOREFENCIACIÓN DE ARGICS///////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////
function ConsultarGeoInversa() {
    //Verifica que existan valores para las coordenadas X y Y
    var lat = $("#LATITUD_CASO").val();
    var long = $("#LONGITUD_CASO").val();
    if (lat === "0" || long === "0") {
        GestionarAlerta("warning", "No hay valores válidos de coordenadas para completar la acción solicitada.");
        return;
    }
    var urlFinal = urlGeoInversa.replace(/amp;/g, "") + "{x:" + long + ",y:" + lat + "}&distance=&langCode=&outSR=&returnIntersection=false&f=pjson";

    $.ajax({
        method: "POST",
        url: urlFinal,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        dataType: "json",
        timeout: timeoutArcGis,
        beforeSend: function () {
            $("#mensajeInversa").html("Consultando datos de georef. inversa...");
        },
        success: function (data) {
            if (data.error) {
                ////console.log("error ajax");
                GestionarAlerta("danger", "El servicio de georeferencia ha presentado problemas y no ha sido posible completar la consulta de georeferencia inversa.");
                return;
            }
            if (data.address) {
                $("#DIRE_CASO").val(data.address.Match_addr.substr(0, 79));
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {

            if (textStatus === "timeout") {
                GestionarAlerta("warning", "Al parecer la consulta está tardando demasiado. El servicio de ArcGis no responde.");
                $("#mensajeInversa").empty();
                return;
            } else {
                GestionarAlerta("warning", "Parece que el servicio tiene problemas. Intente acceder al siguiente  <a target='_blank' href='" + urlFinal + "'>vinculo</a>")
            }
        }
    }).done(function (data) {
        $("#mensajeInversa").empty();
    }).fail(function (jqXHR, textStatus, errorThrown) {
        $("#mensajeInversa").empty();
    });
}

function ConsultarGeoDirecta() {
    //Verifica que existan valores para las coordenadas X y Y
    var direccion = $("#DIRE_CASO").val();
    var medio = $("#ddlMedios").val();
    //if(medio!="01"){
    //    GestionarAlerta("warning", "El medio de comunicación debe ser Telefono Fijo para poder completar esta acción.");
    //    return;
    //}
    if (direccion === "") {
        GestionarAlerta("warning", "No hay valores válidos de dirección para completar la acción solicitada.");
        return;
    }
    //var urlFinal = urlGeoDirecta.replace(/amp;/g, "") + direccion + "&f=pjson";
    var urlFinal = urlGeoDirecta.replace(/amp;/g, "");
    var datos = { Address: direccion, f: "pjson" }
    $.ajax({
        method: "POST",
        url: urlFinal,
        data: datos,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        dataType: "json",
        timeout: timeoutArcGis,
        beforeSend: function () {
            $("#mensajeDirecta").html("Consultando datos de georef. Directa...");
        },
        success: function (data) {
            ////console.log(data);
            if (data.error) {
                ////console.log("error ajax");
                GestionarAlerta("danger", "El servicio de georeferencia ha presentado problemas y no ha sido posible completar la consulta de georeferencia inversa.");
                return;
            }
            //carga las posiciones encontradas de la dirección
            if (data.candidates) {
                if (data.candidates.length <= 0) {
                    GestionarAlerta("warning", "El servicio de georeferencia no ha encontrado coordenadas para la dirección ingresada.");
                    return;
                }
                $(data.candidates).each(function () {
                    if (this.location) {
                        $("#LATITUD_CASO").val(this.location.y);
                        $("#LONGITUD_CASO").val(this.location.x);
                        return false;
                    }
                });
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (textStatus === "timeout") {
                GestionarAlerta("warning", "Al parecer la consulta está tardando demasiado. El servicio de ArcGis no responde.");
                $("#mensajeDirecta").empty();
                return;
            } else {
                GestionarAlerta("warning", "Parece que el servicio tiene problemas. Intente acceder al siguiente <a target='_blank' href='" + urlFinal + "'>vinculo</a>")
            }
        }
    }).done(function (data) {
        $("#mensajeDirecta").empty();
    }).fail(function (jqXHR, textStatus, errorThrown) {
        $("#mensajeDirecta").empty();
    });
}

function AbriReporteSeguimientoEvento() {
    var llamada = $("#NUME_LLAMADA").val();
    var fechaCaso = $("#FechaCaso").val();
    if (!glCreado) {
        GestionarAlerta("warning", "Debe primero generar un caso.");
        return;
    }
    var win = window.open(urlReporteSeguimientoEvento + "?strNumeroLLamada=" + llamada + "&strFechaInicial=" + fechaCaso, '_blank');
    win.focus();
}
//////////////////////////////////////////////////////////////////////////////
//////////////////FUNCIONES REVISIÓN DE CASOS/////////////////////////////////
function ConsultarResumen() {
    var urlFinal = urlResumenCaso.replace(/amp;/g, "");
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            if (data.result) {
                $("#taResumenCaso").val(data.datos);
            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
    }).fail(function (jqXHR, textStatus, errorThrown) {
        ////console.log("conexion caida, timer detenido");
        DetenerTimer();
    });
}
/**
Permite consultar la información al verificar una dirección
**/
function VerificarDireccion() {
    var urlFinal = urlVerificarDir.replace(/amp;/g, "");
    ////console.log(urlFinal);
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            if (data.result) {
                //Carga los datos encontrados
                $("#txtDirValidada").val(data.datos.PDIRE_CASO);
                $("#txtComunaModal").val(data.datos.PVALUENOMCOMUNA);
                $("#txtComunaCod").val(data.datos.PCODCOMUNA);
                $("#txtBarrioModal").val(data.datos.PVALUENOMBARRIO);
                $("#txtBarrioCod").val(data.datos.PCODBARRIO);
                $("#txtLatitud").val(data.datos.PVALUECOORDX);
                $("#txtLongitud").val(data.datos.PVALUECOORDY);
                $("#txtEstado").val(data.datos.PESTADO);
                $("#txtMensaje").val(data.datos.PSALIDA);

                $("#ModalDireccion").modal("show");
                
            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
    }).fail(function (jqXHR, textStatus, errorThrown) {
    });
}




//Permite Asignar
function AsignarDireccionValidada() {
    if ($("#txtBarrioModal").val() === "") {
        $("#CODI_BARRIO").val("");
        $("#txtBarrio").val("");
    } else {
        $("#CODI_BARRIO").val($("#txtBarrioCod").val());
        $("#txtBarrio").val($("#txtBarrioCod").val());
    }
    $("#txtComuna").val($("#txtComunaModal").val());
    $("#DIRE_CASO").val($("#txtDirValidada").val());
    $("#LATITUD_CASO").val($("#txtLatitud").val());
    $("#LONGITUD_CASO").val($("#txtLongitud").val());
    $("#ModalDireccion").modal("hide");
}
function DetenerTimer() {
    //console.log("--------------Timer recepcion detenido");
    clearInterval(timerRecepcionId);
}

function IniciarTimerLlamadas24H() {
    //console.log("--------------Timer Llamadas 24H iniciado");
    //timerRecepcionId = setTimeout(ConsultarLlamada(), glTiempo);//Tiempo en milisegundos
    timerLlamadas24HId = setInterval(function () { CargarTablaLlamadas24h(); }, glTiempoLlamada24H);//Tiempo en milisegundos
    // var id = setInterval(function () { ////console.log("intervalo"); },1000);
}

function DetenerTimerLlamadas24H() {
    //console.log("--------------Timer Llamadas 24H detenido");
    clearInterval(timerLlamadas24HId);
}

function ConsultarSitios() {
    var lugarCodigo = $("#txtCiudadId").val();
    var valor = $("#DIRE_CASO").val();
    var urlFinal = urlConsultaSitios.replace(/amp;/g, "");

    var datos = { intSitio: globalSitio, lgLugeCodigo: lugarCodigo }
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.5%" },
        { "data": "Boton", "orderable": false, "width": "2%" },
        { "data": "Sitio", "orderable": true },
        { "data": "Direccion", "orderable": true },
        { "data": "Barrio", "orderable": true },
        { "data": "BarrCodigo", "orderable": true },
        { "data": "BarrioLugarCodigo", "orderable": true },
    ];
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblSitios")) {
        $("#tblSitios").DataTable().destroy();
        $("#tblSitios tbody").empty();
    }
    //AplicarPluginTablaAjax("tblSitios", urlFinal, parametrosAjax);
    AplicarTablaConBusqueda("tblSitios", urlFinal, parametrosAjax, datos, valor);
    $("#ModalSitios").modal("toggle");
}


function SeleccionarDireccion(V_ValorDireccion) {
    $("#DIRE_CASO").val(V_ValorDireccion);
}