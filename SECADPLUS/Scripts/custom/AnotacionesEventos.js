$(document).ready(function () {
    CargaAnotacionesEventos()
});

function CargaAnotacionesEventos() {
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.5%" },
        { "data": "ANOTACION" },
        { "data": "CONSECUTIVO" },
        { "data": "CADUSUA_GRABA" },
        { "data": "FECHA_GRABACION" },
    ];

    $("#tablaAnotacionesEvento").DataTable().destroy();
    $("#tablaAnotacionesEvento tbody").empty();

    AplicarPluginTablaAjax("tablaAnotacionesEvento", valorFiltros(), parametrosAjax);
    // $("#ModalAnotacionesEventos").modal();
}
function valorFiltros() {
    var strCadEvenNumeroEvento = $('#NumeroEvento').val();//"216861";
    var strSitioGraba = $('#SitioGraba').val();//"216861";

    var filtros = (urlTabla + "?strCadEvenNumeroEvento=" + strCadEvenNumeroEvento + "&strSitioGraba=" + strSitioGraba);
    $('#CadEvenNumeroEvento').val(strCadEvenNumeroEvento);
    return filtros;
}
function GuardarAnotacionEvento() {
    var strCadEvenNumeroEvento = $('#CadEvenNumeroEvento').val();
    if (strCadEvenNumeroEvento != '' && $('#txtAnotacion').val() != '') {
        $.ajax({
            url: urlDatos,
            method: "POST",
            content: "application/json; charset=utf-8",
            dataType: 'json',
            data: { strAnotacion: $('#txtAnotacion').val(), strNumeroEvento: strCadEvenNumeroEvento },
            success: function (resultado) {
                if (resultado.result) {
                    GestionarAlerta(resultado.tipos, resultado.mensajes);
                    strAnotacion: $('#txtAnotacion').val('');
                    //  $("#ModalAnotacionesEventos").modal("toggle");
                    CargaAnotacionesEventos();
                    GestionarAlerta("success", "Se creo exitosamente el registro");
                } else {
                    GestionarAlerta("warning", resultado.datos);
                }
            }
        });
    } else {
        GestionarAlerta("warning", "Digite una anotación");
    }
}
function AbrirModalAnotaciones() {
    $("#ModalAnotacionesEventos").modal();
}
function cerrarse() {
    window.close()
}
function CargarConexionesRemotas() {
    $.ajax({
        url: urlConexionRemota,
        method: "POST",
        content: "application/json; charset=utf-8",
        success: function (resultado) {
            if (resultado.result) {
                for (var i = 0; i < resultado.listaRemotos.length; i++) {
                    switch (i) {
                        case 0:
                            $('#txtConexion1').val(resultado.listaRemotos[i].DESCSITIO);
                            $('#DBLINK_REMOTOS_CON1').val(resultado.listaRemotos[i].DBLINK);
                            $('#FUERZA_REMOTOS_CON1').val(resultado.listaRemotos[i].FUERZA);
                            $('#SITIOGRABA_REMOTOS_CON1').val(resultado.listaRemotos[i].SITIOGRABA);
                            $('#CANAL_REMOTOS_CON1').val(resultado.listaRemotos[i].CANAL);
                            break;
                        case 1:
                            $('#txtConexion2').val(resultado.listaRemotos[i].DESCSITIO);
                            $('#DBLINK_REMOTOS_CON2').val(resultado.listaRemotos[i].DBLINK);
                            $('#FUERZA_REMOTOS_CON2').val(resultado.listaRemotos[i].FUERZA);
                            $('#SITIOGRABA_REMOTOS_CON2').val(resultado.listaRemotos[i].SITIOGRABA);
                            $('#CANAL_REMOTOS_CON2').val(resultado.listaRemotos[i].CANAL);
                            break;
                        case 2:
                            $('#txtConexion3').val(resultado.listaRemotos[i].DESCSITIO);
                            $('#DBLINK_REMOTOS_CON3').val(resultado.listaRemotos[i].DBLINK);
                            $('#FUERZA_REMOTOS_CON3').val(resultado.listaRemotos[i].FUERZA);
                            $('#SITIOGRABA_REMOTOS_CON3').val(resultado.listaRemotos[i].SITIOGRABA);
                            $('#CANAL_REMOTOS_CON3').val(resultado.listaRemotos[i].CANAL);
                            break;
                        case 3:
                            $('#txtConexion4').val(resultado.listaRemotos[i].DESCSITIO);
                            $('#DBLINK_REMOTOS_CON4').val(resultado.listaRemotos[i].DBLINK);
                            $('#FUERZA_REMOTOS_CON4').val(resultado.listaRemotos[i].FUERZA);
                            $('#SITIOGRABA_REMOTOS_CON4').val(resultado.listaRemotos[i].SITIOGRABA);
                            $('#CANAL_REMOTOS_CON4').val(resultado.listaRemotos[i].CANAL);
                            break;
                    }
                }
            } else {
                GestionarAlerta("warning", "No se encontró conexiones remotas.");
            }
        }
    });
}
function EnviarCasoRemoto(boton) {
    var idBtn = boton.id.split('_')[1];
    var descDBlink = $('#' + 'txtConexion' + idBtn).val();

    if (descDBlink != "") {
        var cadenaParametros = $('#' + 'DBLINK_REMOTOS_CON' + idBtn).val() + ';' +
                               $('#CadEvenNumeroEvento').val() + ';' +
                               $('#' + 'FUERZA_REMOTOS_CON' + idBtn).val() + ';' +
                               $('#' + 'CANAL_REMOTOS_CON' + idBtn).val() + ';' +
                               $('#' + 'SITIOGRABA_REMOTOS_CON' + idBtn).val();
        $.ajax({
            url: urlRemoto,
            method: "POST",
            content: "application/json; charset=utf-8",
            dataType: 'json',
            data: { strCadenaParametros: cadenaParametros },
            success: function (resultado) {
                if (resultado.result) {
                    GestionarAlerta(resultado.tipos, resultado.mensajes);
                    CargaAnotacionesEventos();
                } else {
                    GestionarAlerta("warning", resultado.datos);
                }
            }
        });
    } else {
        GestionarAlerta("warning", "Active acceso remoto");
    }
}