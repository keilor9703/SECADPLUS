var parametrosAjaxVehiculos = [
            { "data": "Dummy", "orderable": false, "width": "0.2%" },
            { "data": "PLACA", "orderable": false },
            { "data": "ESTADO", "orderable": false },

            { "data": "MOTOR", "visible": false },
            { "data": "CHASIS", "visible": false },
            { "data": "SERIE", "visible": false },
            { "data": "MODELO", "visible": false },
            { "data": "MARCA", "visible": false },
            { "data": "LINEA", "visible": false },
            { "data": "COLOR", "visible": false },
            { "data": "TIPO", "visible": false },
            { "data": "AUTORIDAD", "visible": false },
];

$(function () {
    $("#txtDatoFiltro").keypress(function (e) {
        
        if (e.which === 13) {            
            $("#consultarVehiculos").focus();
        }
    });
    AplicarPluginTablaVehiculosDijin("_tblVehiculosDijin", urlVehiculos, parametrosAjaxVehiculos);
})

function ConsultarVehiculosAntecedentes() {
    $(".loader_decad").removeClass("hidden");
    $(".loader_back").removeClass("hidden");
    var filtroDato = $('#txtDatoFiltro').val();
    var tipoBusqueda = "";

    if ($("#ckPlaca").is(":checked")) {
        tipoBusqueda = "1";
    } else if ($("#ckMotor").is(":checked")) {
        tipoBusqueda = "2";
    } else if ($("#ckSerie").is(":checked")) {
        tipoBusqueda = "3";
    } else if ($("#ckChasis").is(":checked")) {
        tipoBusqueda = "4";
    }

    if (filtroDato != "") {
        if ($.fn.DataTable.isDataTable("#_tblVehiculosDijin")) {
            //console.log("destroo");
            $("#_tblVehiculosDijin").DataTable().destroy();
            $("#_tblVehiculosDijin tbody").empty();
        }
        AplicarPluginTablaVehiculosDijin("_tblVehiculosDijin", (urlVehiculos + "?strTipoBusqueda=" + tipoBusqueda + "&filtroBusqueda=" + filtroDato), parametrosAjaxVehiculos);
    } else {
        GestionarAlerta("warning", "Debe ingresar un dato de busqueda.");
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
    }
}

function CargarPriemraFilaVehiculo(tablaId) {
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    datosTabla = tabla.row(0).data();

    limpiarVehiculos();

    if (totalTabla.length == 0) {
        return;
    }

    var campoBusqueda = "";
    var filtroDato = $('#txtDatoFiltro').val();
    var dijin = "F";
    var desc_patrullaPoli = $('#DES_PATRULLA_SELECT').val() + " : " + $('#DES_POLICIA_SELECT').val();
    var uniFuerza = $('#CADUNID_FUERZA').val();
    var uniCodigo = $('#CADUNID_CODIGO').val();
    var patrullaCodigo = $('#CODIGO_patrulla').val();
    var descPolicia = $('#DESC_POLICIA').val();

    $('#txtMotor').val(datosTabla["MOTOR"]);
    $('#txtSerie').val(datosTabla["SERIE"]);
    $('#txtChasis').val(datosTabla["CHASIS"]);
    $('#txtModelo').val(datosTabla["MODELO"]);
    $('#txtMarca').val(datosTabla["MARCA"]);
    $('#txtLinea').val(datosTabla["LINEA"]);
    $('#txtColor').val(datosTabla["COLOR"]);
    $('#txtTipo').val(datosTabla["TIPO"]);
    $('#txtAutoridad').text(datosTabla["AUTORIDAD"]);

    likeEstado = datosTabla["ESTADO"].match(/SOLICITUD/);
    if (datosTabla["ESTADO"].toUpperCase() == "HURTADO" || likeEstado != null) {
        $('#mensajeVerifica').html('Verifique vehículo..');
        $('#ModalVehiculoVerifica').modal();
        dijin = "V";
    }

    if ($("#ckPlaca").is(":checked")) {
        campoBusqueda = "placa";
    } else if ($("#ckMotor").is(":checked")) {
        campoBusqueda = "motor";
    } else if ($("#ckSerie").is(":checked")) {
        campoBusqueda = "serie";
    } else if ($("#ckChasis").is(":checked")) {
        campoBusqueda = "chasis";
    }

    var datosAuditoria = datosTabla["PLACA"] + ";" + datosTabla["MOTOR"] + ";" + datosTabla["SERIE"] + ";" + datosTabla["CHASIS"] + ";" + datosTabla["ESTADO"] + ";" + campoBusqueda + ";" +
                         filtroDato + ";" + dijin + ";" + uniFuerza + ";" + uniCodigo + ";" + patrullaCodigo + ";" + desc_patrullaPoli + ";" + descPolicia;

    $.ajax({
        type: 'POST',
        url: urlVehiculosDijin,
        dataType: 'json',
        data: { strfiltroBusqueda: filtroDato, strDatosAuditoria: datosAuditoria },
        success: function (resultados) {
            if (resultados.result == true) {
                if (resultados.btnTem != "") {
                    $('#contenedorTem').html(resultados.btnTem);
                    $('#mensajeVerificaBienes').html("Verifique presenta anotación en la tabla de anotaciones de vehículos temporales");
                    $('#ModalBienes').modal();
                }

                if (resultados.msDijin != "") {
                    $('#mensajeVehiculoDijin').html(resultados.msDijin);
                    $('#ModalVehiculoDijin').modal();
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

function CargarDatosSelect(tablaId, object) {
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    datosTabla = tabla.row(object).data();
    limpiarVehiculos();
    if (totalTabla.length == 0) {
        return;
    }

    $('#txtMotor').val(datosTabla["MOTOR"]);
    $('#txtSerie').val(datosTabla["SERIE"]);
    $('#txtChasis').val(datosTabla["CHASIS"]);
    $('#txtModelo').val(datosTabla["MODELO"]);
    $('#txtMarca').val(datosTabla["MARCA"]);
    $('#txtLinea').val(datosTabla["LINEA"]);
    $('#txtColor').val(datosTabla["COLOR"]);
    $('#txtTipo').val(datosTabla["TIPO"]);
    $('#txtAutoridad').text(datosTabla["AUTORIDAD"]);

    //likeEstado = datosTabla["ESTADO"].match(/SOLICITUD/);
    //if (datosTabla["ESTADO"].toUpperCase() == "HURTADO" || likeEstado != null) {
    //    $('#mensajeVerifica').html('Verifique vehículo..');
    //    $('#ModalVehiculoVerifica').modal();

    //}

    var filtroDato = datosTabla["PLACA"];
    $.ajax({
        type: 'POST',
        url: urlVehiculosDijin,
        dataType: 'json',
        data: { strfiltroBusqueda: filtroDato },
        success: function (resultados) {
            if (resultados.result == true) {
                if (resultados.btnTem != "") {
                    $('#contenedorTem').html(resultados.btnTem);
                    $('#mensajeVerificaBienes').html("Verifique presenta anotación en la tabla de anotaciones de vehículos temporales");
                    $('#ModalBienes').modal();
                } else {
                    $('#contenedorTem').html("");
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

function recorrerTablaAplicaColor() {
    var tabla = $('#_tblVehiculosDijin').DataTable();
    var datosTabla = tabla.rows().data();
    var arrayTabla = [];
    var celda;
    var likeEstado = "";

    $.each(datosTabla, function (i, val) {
        likeEstado = datosTabla[i]["ESTADO"].match(/SOLICITUD/);

        if (datosTabla[i]["ESTADO"].toUpperCase() == "HURTADO" || likeEstado != null) {
            celda = tabla.cells(":contains('" + datosTabla[i]["ESTADO"] + "')").nodes();
            $(celda).addClass('bg-danger');
        } else {
            celda = tabla.cells(":contains('" + datosTabla[i]["ESTADO"] + "')").nodes();
            $(celda).addClass('secadBgW');
        }
    });
}

function limpiarVehiculos() {
    $('#txtMotor').val("");
    $('#txtSerie').val("");
    $('#txtChasis').val("");
    $('#txtModelo').val("");
    $('#txtMarca').val("");
    $('#txtLinea').val("");
    $('#txtColor').val("");
    $('#txtTipo').val("");
    $('#txtAutoridad').text("");
    $('#contenedorTem').html("");
}
function LimpiadoTotal() {
    $('#txtMotor').val("");
    $('#txtSerie').val("");
    $('#txtChasis').val("");
    $('#txtModelo').val("");
    $('#txtMarca').val("");
    $('#txtLinea').val("");
    $('#txtColor').val("");
    $('#txtTipo').val("");
    $('#txtAutoridad').text("");
    $('#contenedorTem').html("");
    $('#txtDatoFiltro').val("");
    $('#ckPlaca').trigger("click");
    $('#ckPlaca').trigger("click");

    if ($.fn.DataTable.isDataTable("#_tblVehiculosDijin")) {
        //  console.log("destroo");
        $("#_tblVehiculosDijin").DataTable().destroy();
        $("#_tblVehiculosDijin tbody").empty();
        AplicarPluginTablaVehiculosDijin("_tblVehiculosDijin", urlVehiculos, parametrosAjaxVehiculos);
    }
}