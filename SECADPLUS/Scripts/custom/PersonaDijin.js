var parametrosAjaxPatrullas = [
            { "data": "Dummy", "orderable": false, "width": "0.5%" },
            { "data": "DESC_PATRULLA" },
            { "data": "Canal", "visible": false },
            { "data": "MD_SITIO_GRABA", "visible": false },
            { "data": "MD_CONSECUTIVO", "visible": false },

];
var parametrosAjaxPerDisponible = [
            { "data": "Dummy", "orderable": false, "width": "0.5%" },
            { "data": "CADPOLI_CEDU_EMPLEADO" },
            { "data": "DES_POLICIA" },

];

var parametrosAjaxrRegistraduriaPer = [
            { "data": "Dummy", "orderable": false, "width": "0.5%" },
            { "data": "Boton", "orderable": false },
            { "data": "TIPO_IDENTIFICACION", "orderable": false },
            { "data": "IDENTIFICACION", "orderable": false },
            { "data": "NOMBRES", "orderable": false },
            { "data": "SEXO", "orderable": false },
            { "data": "FECHA_NACIMIENTO", "orderable": false },
            { "data": "FECHA_EXPEDICION", "orderable": false },
            { "data": "CIUDAD", "orderable": false },
            { "data": "DEPARTAMENTO", "orderable": false },
            { "data": "ESTADO_PERSONA", "orderable": false },

];

var parametrosAjaxOrdenesCaptura = [
            { "data": "Dummy", "orderable": false, "width": "0.5%", "Color": "red" },
            { "data": "NRO_ORDEN", "orderable": false },
            { "data": "NRO_FISCALIA", "orderable": false },
            { "data": "NOMBRE", "visible": false },
            { "data": "PERSONA_REGISTRADURIA", "visible": false },
            { "data": "FECHA", "visible": false },
            { "data": "CIUDAD", "visible": false },
            { "data": "DELITO", "visible": false },
            { "data": "DELITO", "visible": false },

];

$(function () {
    AplicarPluginTablaRegistraduriaResponse("_tblRegistraduriaPer");
    $(".styled, .multiselect-container input").uniform({
        radioClass: 'choice'
    });

    // $('#_tblOrdenCap').DataTable();
    if ($("#cnPropio").is(":checked")) {
        CargarPatrullas();
    }
    AplicarPluginTablaAjax("_tblPolicias", urlPerDisponible, parametrosAjaxPerDisponible);

    AplicarPluginTablaOrdenCaptura("_tblOrdenCap", urlOrdenes, parametrosAjaxOrdenesCaptura);

    $('#_tblPolicias tbody').on('click', 'tr', function () {
        var tablePolicias = $('#_tblPolicias').DataTable();
        var data = tablePolicias.row(this).data();

        $('#DES_POLICIA_SELECT').val(data['DES_POLICIA']);
        $('#DESC_POLICIA').val(data['DES_POLICIA'] + " CC: " + data['CADPOLI_CEDU_EMPLEADO']);

        //  alert('You clicked on ' + data['DES_POLICIA'] + '\'s row');
    });

    $('#_tblPatrullas tbody').on('click', 'tr', function () {
        var tablePatrullas = $('#_tblPatrullas').DataTable();
        var data = tablePatrullas.row(this).data();
        // $(this).toggleClass('selected');
        $('#DES_PATRULLA_SELECT').val(data['DESC_PATRULLA']);
        $('#CADUNID_FUERZA').val(data["CADUNID_FUERZA"]);
        $('#CADUNID_CODIGO').val(data["CADUNID_CODIGO"]);
        $('#CODIGO_patrulla').val(data["CODIGO"]);
        //  alert('You clicked on ' + data['DESC_PATRULLA'] + '\'s row');
        $('#DES_POLICIA_SELECT').val("");
        $('#DESC_POLICIA').val("");
    });
    $("#txtIdentificacionRegistraduria").keypress(function (e) {
        
        if (e.which == 13) {
            $("#ejecutarConsulta").focus();
        }
    });
})

//carga patrullas
function CargarPatrullas() {
    //console.log("Entro");

    if ($.fn.DataTable.isDataTable("#_tblPatrullas")) {
        //console.log("destroo");
        $("#_tblPatrullas").DataTable().destroy();
        $("#_tblPatrullas tbody").empty();
    }

    AplicarPluginTablaPatrullas("_tblPatrullas", valorFiltrosPatrulla(), parametrosAjaxPatrullas);
}
function CargarRegistraduriaPer() {
    $('#ModalRegistraduriaPer').modal();
    $("#_tblRegistraduriaPer").DataTable().destroy();
    $("#_tblRegistraduriaPer tbody").empty();

    AplicarPluginTablaRegistraduriaResponse("_tblRegistraduriaPer");
    //var t = $('#_tblRegistraduriaPer').DataTable();

    // t.columns.adjust().responsive.recalc();

    $('#txtIdentificacionRegConsul').val('');
    $('#txtNombreRegCosnul').val('');
}
//carga filtros patrulla
function valorFiltrosPatrulla() {
    var todos = null;

    if ($("#cnPropio").prop("checked")) {
        todos = "cp";
    } else {
        todos = "td";
    }

    var filtros = (urlPatrullas + "?strCnTodos=" + todos);
    return filtros;
}

//carga el primer dato de patrullas
function CargarPriemraFilaCanal(tablaId) {
    //console.log("EntroPriemra");

    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    datosTabla = tabla.row(0).data();

    if (totalTabla.length == 0) {
        return;
    }
    // console.log("Entro");

    $('#DSP_CANAL').val(datosTabla["Canal"]);
    $('#DSP_UNIDADL').val(datosTabla["Unidad"]);

    if ($.fn.DataTable.isDataTable("#_tblPolicias")) {
        //console.log("destroo");
        $("#_tblPolicias").DataTable().destroy();
        $("#_tblPolicias tbody").empty();
    }
    AplicarPluginTablaAjax("_tblPolicias", (urlPerDisponible + "?stMedioDispSitioGraba=" + datosTabla["MD_SITIO_GRABA"] + "&stpMedioDispConsecutivo=" + datosTabla["MD_CONSECUTIVO"]), parametrosAjaxPerDisponible);
}
//carga datos unidad y canal
function CargarDatosUnidadCanal(tablaId, object) {
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    datosTabla = tabla.row(object).data();

    if (totalTabla.length == 0) {
        return;
    }

    $('#DSP_CANAL').val(datosTabla["Canal"]);
    $('#DSP_UNIDADL').val(datosTabla["Unidad"]);

    if ($.fn.DataTable.isDataTable("#_tblPolicias")) {
        //console.log("destroo");
        $("#_tblPolicias").DataTable().destroy();
        $("#_tblPolicias tbody").empty();
    }
    AplicarPluginTablaAjax("_tblPolicias", (urlPerDisponible + "?stMedioDispSitioGraba=" + datosTabla["MD_SITIO_GRABA"] + "&stpMedioDispConsecutivo=" + datosTabla["MD_CONSECUTIVO"]), parametrosAjaxPerDisponible);
}
function CargarDatos() {
    $(".loader_decad").removeClass("hidden");
    $(".loader_back").removeClass("hidden");
    var numIdentifica = $('#txtIdentificacionRegistraduria').val();
    //console.log(numIdentifica);
    if (numIdentifica != "") {
        var tipoIdentificacion = "";

        if ($("#ckCedula").is(":checked")) {
            tipoIdentificacion = "CC";
        } else if ($("#ckPasaporte").is(":checked")) {
            tipoIdentificacion = "PA";
        } else if ($("#ckCedExtranjeria").is(":checked")) {
            tipoIdentificacion = "CE";
        } else if ($("#ckTarjetaId").is(":checked")) {
            tipoIdentificacion = "TI";
        }
        LimpiarAntecedentesReg();

        var desc_patrullaPoli = $('#DES_PATRULLA_SELECT').val() + " : " + $('#DES_POLICIA_SELECT').val();
        var uniFuerza = $('#CADUNID_FUERZA').val();
        var uniCodigo = $('#CADUNID_CODIGO').val();
        var patrullaCodigo = $('#CODIGO_patrulla').val();
        var descPolicia = $('#DESC_POLICIA').val();

        var datosPatrulla = desc_patrullaPoli + ";" + uniFuerza + ";" + uniCodigo + ";" + patrullaCodigo + ";" + descPolicia;

        $.ajax({
            type: 'POST',
            url: urlDatosAntecedentes,
            dataType: 'json',
            data: { strTipoIdentificacion: tipoIdentificacion, strNumeroIdentificacion: numIdentifica, strDatosPatrulla: datosPatrulla },
            success: function (resultados) {
                console.log("Terminó consulta de antecedentes");
                if (resultados.mensajeP != "") {
                    $('#mensajePonal').html(resultados.mensajeP);
                    $('#ModalPonal').modal();
                }

                if (resultados.datosPersona["p_nombre"] != "null") {
                    $('#txtIdentificacionReg').val(numIdentifica);
                    $('#txtTipoIdenReg').val(tipoIdentificacion);
                    $('#txtNombreReg').val(resultados.datosPersona["p_nombre"]);

                    if (resultados.datosPersona["p_estadoCedula"] != "null") {
                        $("#ESTADO_PERSONAReg").val(resultados.datosPersona["p_estadoCedula"]);
                        $("#ESTADO_PERSONAReg").trigger("chosen:updated");
                    }
                    if (resultados.datosPersona["p_genero"] != "null") {
                        $("#GeneroReg").val(resultados.datosPersona["p_genero"]);
                        $("#GeneroReg").trigger("chosen:updated");
                    }
                    if (resultados.datosPersona["p_fechaNacimiento"] != "null") {
                        $('#txtFechaNacimientoReg').val(resultados.datosPersona["p_fechaNacimiento"]);
                    }
                    if (resultados.datosPersona["p_fechaExp"] != "null") {
                        $('#txtFechaExpReg').val(resultados.datosPersona["p_fechaExp"]);
                    }
                    if (resultados.datosPersona["p_ciudad"] != "null") {
                        $('#txtCiudadReg').val(resultados.datosPersona["p_ciudad"]);
                    }
                    if (resultados.datosPersona["p_ciudad"] != "null") {
                        $('#txtDepartamentoReg').val(resultados.datosPersona["p_departamento"]);
                    }
                }

                if (resultados.observacionesInpec != "") {
                    $('#txtInpec').val(resultados.observacionesInpec);
                }

                if (resultados.mensajeOrdenCap != "") {
                    $('#txtDijin').val(resultados.mensajeOrdenCap);

                    if (resultados.mensajeOrdenCap == "VERIFIQUE A ESTA PERSONA LE FIGURA ORDEN DE CAPTURA") {
                        $('#txtDijin').css("background-color", "#c68787");
                        $('#mensajeOrdenCap').html("V e r i f i q u e  -   o r d e n ( e s ) -    d e  -  c a p t u r a ...");
                        $('#ModalOrdenCap').modal();

                        if ($.fn.DataTable.isDataTable("#_tblOrdenCap")) {
                            console.log("entreregis");
                            $("#_tblOrdenCap").DataTable().destroy();
                            $("#_tblOrdenCap tbody").empty();
                        }
                        console.log("Terminó consulta de antecedentes");
                        AplicarPluginTablaOrdenCaptura("_tblOrdenCap", (urlOrdenes + "?strTipoIdentificacion=" + tipoIdentificacion + "&strIdentificacion=" + numIdentifica), parametrosAjaxOrdenesCaptura);
                    }
                }

                $(".loader_decad").addClass("hidden");
                $(".loader_back").addClass("hidden");
            },
            error: function (ex) {
                $(".loader_decad").addClass("hidden");
                $(".loader_back").addClass("hidden");
                //GestionarAlerta("warning", );
            }
        });
    } else {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
        GestionarAlerta("warning", "Debe ingresar un numero de identificación.");
    }
}
function LimpiarAntecedentesReg() {
    $('#txtIdentificacionReg').val("");
    $('#txtTipoIdenReg').val("");
    $('#txtNombreReg').val("");
    $("#ESTADO_PERSONAReg").val("");
    $("#ESTADO_PERSONAReg").trigger("chosen:updated");

    $("#GeneroReg").val("0");
    $("#GeneroReg").trigger("chosen:updated");

    $('#txtFechaNacimientoReg').val("");
    $('#txtFechaExpReg').val("");
    $('#txtCiudadReg').val("");
    $('#txtDepartamentoReg').val("");

    $('#txtDijin').css("background-color", "#dee022");
    $('#txtDijin').val("");

    $('#txtInpec').val("");

    $('#mensajeOrdenCap').html("");
    $('#mensajePonal').html("");
    $("#txtTipoIdentificacionOrden").val("");
    $("#txtTipoIdentificacionOrden").trigger("chosen:updated");
    $('#txtIdentificacionOrden').val("");

    if ($.fn.DataTable.isDataTable("#_tblOrdenCap")) {
        // console.log("destrooLimpiar");
        // $("#_tblOrdenCap").DataTable().destroy();
        $("#_tblOrdenCap tbody").empty();
    }

    LimpiarOrdenCaptura();
}
function PersonaAnterior() {
    $("#_tblRegistraduriaPer").DataTable().destroy();
    $("#_tblRegistraduriaPer tbody").empty();

    AplicarPluginTablaRegistraduriaResponse("_tblRegistraduriaPer");

    $('#txtIdentificacionRegConsul').val('');
    $('#txtNombreRegCosnul').val('');
}
function PersonaSiguiente() {
    $(".loader_decad").removeClass("hidden");
    $(".loader_back").removeClass("hidden");
    var tableperReg = $('#_tblRegistraduriaPer').DataTable();
    var totalTabla = tableperReg.rows().data();
    //console.log(totalTabla.length);

    if (totalTabla.length == 0) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
        return;
    }

    var data = tableperReg.row(totalTabla.length - 1).data();

    //console.log(data[4]);

    var nombrelike = data[4];
    var anterior = data[3];
    nombrelike = nombrelike.split(" ")[0];

    //console.log(nombrelike);

    var datosSeparados = "";

    if (nombrelike.length > 0) {
        var strAnterior = anterior;
        var strOperador = ">";
        var strEsconsulta = "0";
        var strIdentificacion = "";
        var strNombres = nombrelike;

        datosSeparados = strAnterior + ";" + strOperador + ";" + strEsconsulta + ";" + strIdentificacion + ";" + strNombres;
    }

    $.ajax({
        url: urlNuevosDatosReg,
        method: "POST",
        content: "application/json; charset=utf-8",
        dataType: 'json',
        data: { strDatosConsulta: datosSeparados },
        success: function (resultado) {
            if (resultado.result) {
                if (resultado.filas.length > 0) {
                    var t = $('#_tblRegistraduriaPer').DataTable();

                    t.row.add([
                               resultado.filas[0].Dummy,
                               resultado.filas[0].Boton,
                               resultado.filas[0].TIPO_IDENTIFICACION,
                               resultado.filas[0].IDENTIFICACION,
                               resultado.filas[0].NOMBRES,
                               resultado.filas[0].SEXO,
                               resultado.filas[0].FECHA_NACIMIENTO,
                               resultado.filas[0].FECHA_EXPEDICION,
                               resultado.filas[0].CIUDAD,
                               resultado.filas[0].DEPARTAMENTO,
                               resultado.filas[0].ESTADO_PERSONA

                    ]).draw(false);

                    t.columns.adjust().responsive.recalc();
                } else {
                    GestionarAlertaModal("warning", "No se encontro datos.", "ContenedorAlertarModalReg");
                }
                $(".loader_decad").addClass("hidden");
                $(".loader_back").addClass("hidden");
            } else {
                GestionarAlerta("warning", resultado.datos);
                GestionarAlertaModal("warning", resultado.datos, "ContenedorAlertarModalReg");
            }
        }
    });
}
function BuscarPersonaReg() {
    //  console.log("Entro");

    var identificacion = $('#txtIdentificacionRegConsul').val();
    var nombres = $('#txtNombreRegCosnul').val();

    if (identificacion == "" && nombres == "") {
        return;
    }
    var datosSeparados = "";

    if (identificacion != "" && nombres == "") {
        var strAnterior = "0";
        var strOperador = "=";
        var strEsconsulta = "1";
        var strIdentificacion = identificacion;
        var strNombres = '';

        datosSeparados = strAnterior + ";" + strOperador + ";" + strEsconsulta + ";" + strIdentificacion + ";" + strNombres;
        // console.log("Sin nombre");
    }

    if (identificacion == "" && nombres != "") {
        var strAnterior = "0";
        var strOperador = ">";
        var strEsconsulta = "1";
        var strIdentificacion = "0";
        var strNombres = nombres;

        datosSeparados = strAnterior + ";" + strOperador + ";" + strEsconsulta + ";" + strIdentificacion + ";" + strNombres;
        console.log("Sin identificacion");
    }

    if (identificacion != "" && nombres != "") {
        var strAnterior = "0";
        var strOperador = "=";
        var strEsconsulta = "1";
        var strIdentificacion = identificacion;
        var strNombres = nombres;

        datosSeparados = strAnterior + ";" + strOperador + ";" + strEsconsulta + ";" + strIdentificacion + ";" + strNombres;
        console.log("Con los dos");
    }

    $.ajax({
        url: urlNuevosDatosReg,
        method: "POST",
        content: "application/json; charset=utf-8",
        dataType: 'json',
        data: { strDatosConsulta: datosSeparados },
        success: function (resultado) {
            if (resultado.result) {
                if (resultado.filas.length > 0) {
                    $("#_tblRegistraduriaPer").DataTable().destroy();
                    $("#_tblRegistraduriaPer tbody").empty();

                    AplicarPluginTablaRegistraduriaResponse("_tblRegistraduriaPer");
                    var t = $('#_tblRegistraduriaPer').DataTable();

                    t.row.add([
                               resultado.filas[0].Dummy,
                               resultado.filas[0].Boton,
                               resultado.filas[0].TIPO_IDENTIFICACION,
                               resultado.filas[0].IDENTIFICACION,
                               resultado.filas[0].NOMBRES,
                               resultado.filas[0].SEXO,
                               resultado.filas[0].FECHA_NACIMIENTO,
                               resultado.filas[0].FECHA_EXPEDICION,
                               resultado.filas[0].CIUDAD,
                               resultado.filas[0].DEPARTAMENTO,
                               resultado.filas[0].ESTADO_PERSONA

                    ]).draw(false);

                    //var otb = $('#' + tablaId).DataTable();
                    t.columns.adjust().responsive.recalc();
                } else {
                    GestionarAlertaModal("warning", "No se encontro datos.", "ContenedorAlertarModalReg");
                }
            } else {
                GestionarAlertaModal("warning", resultado.datos, "ContenedorAlertarModalReg");
            }
        }
    });
}
function CargarAntecedentesDatosReg(tpIdentificacion, identificacion) {
    //console.log(tpIdentificacion);
    //console.log(identificacion);

    $(".loader_decad").removeClass("hidden");
    $(".loader_back").removeClass("hidden");

    LimpiarAntecedentesReg();

    var desc_patrullaPoli = $('#DES_PATRULLA_SELECT').val() + " : " + $('#DES_POLICIA_SELECT').val();
    var uniFuerza = $('#CADUNID_FUERZA').val();
    var uniCodigo = $('#CADUNID_CODIGO').val();
    var patrullaCodigo = $('#CODIGO_patrulla').val();
    var descPolicia = $('#DESC_POLICIA').val();

    var datosPatrulla = desc_patrullaPoli + ";" + uniFuerza + ";" + uniCodigo + ";" + patrullaCodigo + ";" + descPolicia;
    //  console.log(datosPatrulla);

    $.ajax({
        type: 'POST',
        url: urlDatosAntecedentes,
        dataType: 'json',
        data: { strTipoIdentificacion: tpIdentificacion, strNumeroIdentificacion: identificacion, strDatosPatrulla: datosPatrulla },
        success: function (resultados) {
            if (resultados.mensajeP != "") {
                $('#mensajePonal').html(resultados.mensajeP);
                $('#ModalPonal').modal();
            }

            if (resultados.datosPersona["p_nombre"] != "null") {
                $('#txtIdentificacionReg').val(identificacion);
                $('#txtTipoIdenReg').val(tpIdentificacion);
                $('#txtNombreReg').val(resultados.datosPersona["p_nombre"]);

                if (resultados.datosPersona["p_estadoCedula"] != "null") {
                    $("#ESTADO_PERSONAReg").val(resultados.datosPersona["p_estadoCedula"]);
                    $("#ESTADO_PERSONAReg").trigger("chosen:updated");
                }
                if (resultados.datosPersona["p_genero"] != "null") {
                    $("#GeneroReg").val(resultados.datosPersona["p_genero"]);
                    $("#GeneroReg").trigger("chosen:updated");
                }
                if (resultados.datosPersona["p_fechaNacimiento"] != "null") {
                    $('#txtFechaNacimientoReg').val(resultados.datosPersona["p_fechaNacimiento"]);
                }
                if (resultados.datosPersona["p_fechaExp"] != "null") {
                    $('#txtFechaExpReg').val(resultados.datosPersona["p_fechaExp"]);
                }
                if (resultados.datosPersona["p_ciudad"] != "null") {
                    $('#txtCiudadReg').val(resultados.datosPersona["p_ciudad"]);
                }
                if (resultados.datosPersona["p_ciudad"] != "null") {
                    $('#txtDepartamentoReg').val(resultados.datosPersona["p_departamento"]);
                }
            }

            if (resultados.observacionesInpec != "") {
                $('#txtInpec').val(resultados.observacionesInpec);
            }

            if (resultados.mensajeOrdenCap != "") {
                $('#txtDijin').val(resultados.mensajeOrdenCap);

                if (resultados.mensajeOrdenCap == "VERIFIQUE A ESTA PERSONA LE FIGURA ORDEN DE CAPTURA") {
                    $('#txtDijin').css("background-color", "#c68787");
                    $('#mensajeOrdenCap').html("V e r i f i q u e  -   o r d e n ( e s ) -    d e  -  c a p t u r a ...");
                    $('#ModalOrdenCap').modal();

                    if ($.fn.DataTable.isDataTable("#_tblOrdenCap")) {
                        console.log("entreregis");
                        $("#_tblOrdenCap").DataTable().destroy();
                        $("#_tblOrdenCap tbody").empty();
                    }
                    AplicarPluginTablaOrdenCaptura("_tblOrdenCap", (urlOrdenes + "?strTipoIdentificacion=" + tpIdentificacion + "&strIdentificacion=" + identificacion), parametrosAjaxOrdenesCaptura);
                }
            }

            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
        }
    });

    $('#ModalRegistraduriaPer').modal("toggle");
}
//carga patrullas
function CargarPatrullasConsulta() {
    //console.log("Entro");

    var consulta = $('#txtConsultaPatrulla').val();

    //if (consulta != "") {
    if ($.fn.DataTable.isDataTable("#_tblPatrullas")) {
        // console.log("destroo");
        $("#_tblPatrullas").DataTable().destroy();
        $("#_tblPatrullas tbody").empty();
    }

    var todos = null;

    if ($("#cnPropio").prop("checked")) {
        todos = "cp";
    } else {
        todos = "td";
    }

    var filtros = (urlPatrullas + "?strCnTodos=" + todos + "&strConsulta=" + consulta);

    AplicarPluginTablaPatrullas("_tblPatrullas", filtros, parametrosAjaxPatrullas);
    // }
}
function OrdenesCapturaPer() {
    $(".loader_decad").removeClass("hidden");
    $(".loader_back").removeClass("hidden");
    var tipiIdentificacion = $('#txtTipoIdentificacionOrden').val();
    var Identificacion = $('#txtIdentificacionOrden').val();
    // console.log(tipiIdentificacion + " " + Identificacion);
    if (tipiIdentificacion != "" && Identificacion != "") {
        if ($.fn.DataTable.isDataTable("#_tblOrdenCap")) {
            // console.log("destroo");
            $("#_tblOrdenCap").DataTable().destroy();
            $("#_tblOrdenCap tbody").empty();
        }
        AplicarPluginTablaOrdenCaptura("_tblOrdenCap", (urlOrdenes + "?strTipoIdentificacion=" + tipiIdentificacion + "&strIdentificacion=" + Identificacion), parametrosAjaxOrdenesCaptura);
    } else {
        GestionarAlerta("warning", "Debe ingresar un numero de identificación.");
    }
}
function CargarCaposOrdenCap(tablaId) {
    LimpiarOrdenCaptura();

    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    datosTabla = tabla.row(0).data();

    if (totalTabla.length == 0) {
        return;
    }
    // console.log("Entro");

    $('#txtIdentificacionOrden').val(datosTabla["IDENTIFICACION"]);
    $('#txtTipoIdentificacionOrden').val(datosTabla["TIPO"]);

    $('#txtRegistraduriaOrden').val(datosTabla["PERSONA_REGISTRADURIA"]);
    $('#txtNombreOrden').val(datosTabla["NOMBRE"]);
    $('#txtFechaOrden').val(datosTabla["FECHA"]);
    $('#txtCiudadOrden').val(datosTabla["CIUDAD"]);
    $('#txtDelitoOrden').text(datosTabla["DELITO"]);
}
function OrdenesCapturaSelect(tablaId, object) {
    LimpiarOrdenCaptura();
    // console.log("entrelog");
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    if (totalTabla.length == 0) {
        return;
    }
    datosTabla = tabla.row(object).data();

    $('#txtIdentificacionOrden').val(datosTabla["IDENTIFICACION"]);
    $('#txtTipoIdentificacionOrden').val(datosTabla["TIPO"]);

    $('#txtRegistraduriaOrden').val(datosTabla["PERSONA_REGISTRADURIA"]);
    $('#txtNombreOrden').val(datosTabla["NOMBRE"]);
    $('#txtFechaOrden').val(datosTabla["FECHA"]);
    $('#txtCiudadOrden').val(datosTabla["CIUDAD"]);
    $('#txtDelitoOrden').text(datosTabla["DELITO"]);
}
function LimpiarOrdenCaptura() {
    $('#txtIdentificacionOrden').val();
    $('#txtTipoIdentificacionOrden').val();
    $('#txtRegistraduriaOrden').val("");
    $('#txtNombreOrden').val("");
    $('#txtFechaOrden').val("");
    $('#txtCiudadOrden').val("");
    $('#txtDelitoOrden').text("");
}
function LimpiarAntecedentesTotal() {
    LimpiarAntecedentesReg();
    $('#txtIdentificacionRegistraduria').val("");
    $('#ckCedula').trigger("click");
    $('#ckCedula').trigger("click");
}