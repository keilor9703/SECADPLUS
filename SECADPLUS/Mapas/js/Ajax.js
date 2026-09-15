/// *************** CONSTANTES DE CONFIGURACIÓN ******************
var RutaDeploy = "Sivicc2.Web.Administracion";

var OpcionesAjax = {
    onExitoso: function () { },
    onFallido: function () { },
    Area: null,
    Controlador: null,
    Accion: null,
    Id: null,
    DatosEnviar: null,
    EsPartialView: false,
    EsOperacion: false
};
var intervalWaitingFor;
function DatosAjax(opciones) {
    var url = GenerarUrl(opciones.Area, opciones.Controlador, opciones.Accion, opciones.Id);
    clearTimeout(intervalWaitingFor);
    waitingDialog.hide();
    var tipoDatos = "json";
    if (opciones.EsPartialView) {
        tipoDatos = "html";
    }
    $.ajax({
        type: "POST",
        dataType: tipoDatos,
        url: url,
        data: opciones.DatosEnviar,
        beforeSend: function () {
            $("#form :input").prop("disabled", true);
            intervalWaitingFor = setTimeout(function () {
                waitingDialog.hide();
                waitingDialog.show('Procesando...');
            }, 3000)
        },
        success: function (resultadosOperacion) {
            if (opciones.EsOperacion){
                ProcesarResultadosOperacion(resultadosOperacion);
            }
            opciones.onExitoso(resultadosOperacion);
            clearTimeout(intervalWaitingFor);
            waitingDialog.hide();
        },
        error: function (resultadosOperacion) {
            opciones.onFallido(resultadosOperacion);
            clearTimeout(intervalWaitingFor);
            waitingDialog.hide();
        },
        complete: function () {
            $("#form :input").prop("disabled", false);
            objLadda.ladda('stop');
            clearTimeout(intervalWaitingFor);
            waitingDialog.hide();
        }
    })
}

function ProcesarResultadosOperacion(resultadosOperacion) {
    if (resultadosOperacion.ResultadoExitoso) {
        if (resultadosOperacion.Warning) {
            MostrarWarningOperacion(resultadosOperacion);
        } else {
            MensajeExitoso("Resultado de la Operación", resultadosOperacion.MensajeResultadoFinal);
            if ($("#Vista_Redireccionar").val() == "true" && resultadosOperacion.RegistroIdEncriptado != null) {
                var area = "";
                if ($("#Vista_RedireccionArea").length > 0 && $("#Vista_RedireccionArea").val().length > 0) {
                    area = $("#Vista_RedireccionArea").val();
                } else {
                    area = $("#Vista_Area").val();
                }
                var controlador = "";
                if ($("#Vista_RedireccionControlador").length > 0 && $("#Vista_RedireccionControlador").val().length > 0) {
                    controlador = $("#Vista_RedireccionControlador").val();
                } else {
                    controlador = $("#Vista_Controlador").val();
                }
                var accion = "";
                if ($("#Vista_RedireccionAccion").length > 0 && $("#Vista_RedireccionAccion").val().length > 0) {
                    accion = $("#Vista_RedireccionAccion").val();
                } else {
                    accion = $("#Vista_Accion").val();
                }
                if ($("#Vista_RedireccionLimpiar").length > 0 && $("#Vista_RedireccionLimpiar").val().length > 0 && $("#Vista_RedireccionLimpiar").val() == "true") {
                    resultadosOperacion.RegistroIdEncriptado = null;
                }
                RedireccionarPagina(area, controlador, accion, resultadosOperacion.RegistroIdEncriptado, $("#Vista_RedireccionTiempoMilisegundos").val());
            }
        }
    } else {
        MostrarErrorOperacion(resultadosOperacion);
    }
}


function MostrarErrorOperacion(resultadosOperacion) {
    var mensaje = resultadosOperacion.MensajeResultadoFinal + "<br/><ul>";
    $.each(resultadosOperacion.ErroresValidacion, function (indice, errorValidacion) {
        mensaje += "<li>" + errorValidacion.Mensaje + "</li>";
    });
    if (resultadosOperacion.Warning) {
        $.each(resultadosOperacion.WarningValidacion, function (indice, errorValidacion) {
            mensaje += "<li>" + errorValidacion.Mensaje + "</li>";
        });

    }
    mensaje += "</ul>";
    MensajeError("Error en la operación", mensaje);
}

function MostrarWarningOperacion(resultadosOperacion) {
    var mensaje = resultadosOperacion.MensajeResultadoFinal + "<br/><ul>";
    $.each(resultadosOperacion.WarningValidacion, function (indice, errorValidacion) {
        mensaje += "<li>" + errorValidacion.Mensaje + "</li>";
    });
    mensaje += "</ul>";
    MensajeWarning("Advertencias en la operación", mensaje);
}

$(document).ready(function () {
    $(document).ajaxError(function(event, jqxhr, settings, thrownError) {
        if (jqxhr.getResponseHeader('REDIRECCION_SESION') === '1') {
            Alerta("Su sesión se ha cerrado, debe iniciar de nuevo sesión");
            setTimeout(function () {
                window.location = GenerarUrl("Seguridad", "Sesion", "");
            }, 3000);
        } else {
            var errorJson = $.parseJSON(jqxhr.responseText);
            var html = "<h3>Se ha generado un error en la consulta.</h3><table class=\"table\">";
            html += "<tr><td>ID:</td><td>" + errorJson.LogExcepcionAplicacionId + "</td></tr>";
            html += "<tr><td>Tipo de error:</td><td>" + errorJson.TipoExcepcion + "</td></tr>";
            html += "<tr><td>MensajeExcepcion:</td><td>" + errorJson.MensajeExcepcion + "</td></tr>";
            html += "</table>";
            Alerta(html);
        }
    });
    $(document).ajaxSuccess(function (event, request, settings) {
        if (request.getResponseHeader('REDIRECCION_SESION') === '1') {
            window.location = GenerarUrl("Seguridad", "Sesion", "");
        }
    });
});