//var rutaCentro = "/EnvioMensajes/CreacionMensajesCentro";
//var rutaTexto = "/EnvioMensajes/CreacionMensajesTexto";
//var rutaUsuario = "/EnvioMensajes/CreacionMensajesTextoUsuario";




function limpiar() {
    $('#CADFUER_DESTINO').val('').trigger('chosen:updated');
    $('#CADCANA_DESTINO').val('').trigger('chosen:updated');
    $('#CADUSUA_DESTINO').val('').trigger('chosen:updated');
    $('#ESTADO').val('').trigger('chosen:updated');
}

//Agrega Registro Mensaje Texto Usuario
function agregarUsuario() {
    var listaEntidad = $("#CADFUER_DESTINO").val();
    var listaCanal = $("#CADCANA_DESTINO").val();
    var listaUsuario = $("#CADUSUA_DESTINO").val();
    var listaEstado = $("#ESTADO").val();

    if (listaEntidad === "" || listaEntidad === null) {
        GestionarAlerta("warning", "Por favor seleccione una entidad.");
        return;
    }
    if (listaCanal === "" || listaCanal === null) {
        GestionarAlerta("warning", "Por favor seleccione un canal.");
        return;
    }
    if (listaUsuario === "" || listaUsuario === null) {
        GestionarAlerta("warning", "Por favor seleccione un usuario.");
        return;
    }
    if (listaEstado === "" || listaEstado === null) {
        GestionarAlerta("warning", "Por favor seleccione un estado.");
        return;
    }

    var tabla = $("#tablaEnviarA").DataTable();

    var idFuerza = $("#CADFUER_DESTINO").val();
    var nomFuerza = $("#CADFUER_DESTINO option:selected").text();

    var idCanal = $("#CADCANA_DESTINO").val();
    var nomCanal = $("#CADCANA_DESTINO option:selected").text();

    var idUsuario = $("#CADUSUA_DESTINO").val();
    var nomUsuario = $("#CADUSUA_DESTINO option:selected").text();

    var idEstado = $("#ESTADO").val();
    var nomEstado = $("#ESTADO option:selected").text();

    var fechaNotificacion = $("#FECHA_NOTIFICACION").val();
    var fechaGrabacion = $("#FECHA_GRABACION").val();

    var boton = '<button class="btn btn-xs btn-default" type="button" data-popup="tooltip" ' +
                'title="" data-placement="bottom" data-original-title="Clic para eliminar"> ' +
                '<i class="icon-trash"></i> Eliminar</button>';

    tabla.row.add([null, boton, idFuerza, nomFuerza, idCanal, nomCanal, idUsuario, nomUsuario, idEstado, nomEstado, fechaNotificacion, fechaGrabacion]).draw(false);
    //tabla.row.add([null, "x", "1111111111111111111111", "22222222222222222222222", "33333333333333333333333333", "44444444444444444444444444", "555555555555555555", "66666666666666666666666666", "77777777777777777777", "8888888888888888888888888", "999999999999999999999999999999", "10000000000000000000000000000000"]).draw(false);

    tabla.columns.adjust().draw();

    $('#CADFUER_DESTINO').val('').trigger('chosen:updated');
    $('#CADCANA_DESTINO').val('').trigger('chosen:updated');
    $('#CADUSUA_DESTINO').val('').trigger('chosen:updated');
    $('#ESTADO').val('').trigger('chosen:updated');
}

function recorrerTablaUsuarios() {
    var tabla = $('#tablaEnviarA').DataTable();
    var datosTabla = tabla.rows().data();
    var arrayTabla = [];

    $.each(datosTabla, function (i, val) {
        //console.log(datosTabla[i]);
        //console.log(datosTabla[i][3]);

        filasTabla = {
            CADMENTEX_MENS_SITIOGRABA: "0",
            CADMENTEX_MENS_CONSECUTIVO: "0",
            CADMENTEX_CONSECUTIVO: "0",
            CADUSUA_DESTINO: datosTabla[i][6],
            ESTADO: datosTabla[i][8],
            FECHA_NOTIFICACION: datosTabla[i][10],
            FECHA_GRABACION: datosTabla[i][11],
            CADFUER_DESTINO: datosTabla[i][2],
            CADCANA_DESTINO: datosTabla[i][4]
        };

        arrayTabla.push(filasTabla);
    });

    $.ajax({
        type: 'POST',
        //url: '/EnvioMensajes/MensajesTexto',
        //url: '/EnvioMensajes/CreacionMensajesTextoUsuario',
        url: rutaUsuario,
        dataType: 'json',
        data: { usuariosTabla: arrayTabla },
        success: function (resultado) {
            //console.log("Entro");
            if (resultado.result) {
                GestionarAlerta("success", resultado.datos);
            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

//Eliminar Fila Mensaje Texto Usuario
$(document).ready(function () {
    var tabla = $('#tablaEnviarA').DataTable();

    $('#tablaEnviarA tbody').on('click', 'button', function () {
        var fila = $(this).parents('tr');

        swal({
            title: "¿Seguro desea eliminar registro?",
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
                     tabla
                     .row(fila)
                     .remove()
                     .draw();
                 }
             }
             );
    });
});

function guardarMensajes() {
    var listaSitios = $("#listaSitios").val();

    //var textoArea = $("#textAreaTexto").val();
    var textoArea = $("#TEXTO").val();

    //var listaEntidad = $("#listaEntidad").val();
    var listaEntidad = $("#CADFUER_DESTINO").val();

    //var listaCanal = $("#listaCanal").val();
    var listaCanal = $("#CADCANA_DESTINO").val();

    //var listaUsuario = $("#listaUsuario").val();
    var listaUsuario = $("#CADUSUA_DESTINO").val();

    //var listaEstado = $("#listaEstado").val();
    var listaEstado = $("#ESTADO").val();

    if (listaSitios === "" || listaSitios === null) {
        GestionarAlerta("warning", "Por favor seleccione un sitio de grabación.");
        return;
    }

    if (textoArea === "" || textoArea === null) {
        GestionarAlerta("warning", "Por favor escriba mensaje.");
        return;
    }

    var tabla = $('#tablaEnviarA').DataTable();

    if (tabla.data().length === 0) {
        if (listaEntidad === "" || listaEntidad === null) {
            GestionarAlerta("warning", "Por favor seleccione una entidad.");
            return;
        }
        if (listaCanal === "" || listaCanal === null) {
            GestionarAlerta("warning", "Por favor seleccione un canal.");
            return;
        }
        if (listaUsuario === "" || listaUsuario === null) {
            GestionarAlerta("warning", "Por favor seleccione un usuario.");
            return;
        }
        if (listaEstado === "" || listaEstado === null) {
            GestionarAlerta("warning", "Por favor seleccione un estado.");
            return;
        }

        GestionarAlerta("warning", "Por favor agregar usuario a la lista");
        return;
    }

    var datosCentro = $("#CentroMensajes").serialize();

    $.ajax({
        type: 'POST',
        url: rutaCentro,
        dataType: 'json',
        data: datosCentro,
        success: function (resultado) {
            if (resultado.result) {
                //Ajax Guardar Texto
                guardarTexto(resultado.datos.SITIO, resultado.datos.CONSECUTIVO)
                GestionarAlerta("success", resultado.datos.MENSAJE);
            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
};

function guardarTexto(idSitio, idCons) {
    $("#CADMENS_SITIOGRABA").val(idSitio);
    $("#CADMENS_CONSECUTIVO").val(idCons);
    $("#CONSECUTIVO").val(0);
    $("#idSitio").val(idSitio);
    $("#idCons").val(idCons);

    var datosTexto = $("#CentroTexto").serialize();

    $.ajax({
        type: 'POST',
        url: rutaTexto,
        dataType: 'json',
        data: datosTexto,
        success: function (resultado) {
            if (resultado.result) {
                //Ajax Guardar Texto
                guardarUsurio(resultado.datos.SITIO, resultado.datos.CONSECUTIVO, resultado.datos.MENCONSECUTIVO);
                GestionarAlerta("success", resultado.MENSAJE);
            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

function guardarUsurio(idSitio, idCons, idText) {
    $("#CADMENTEX_MENS_SITIOGRABA").val(idSitio);
    $("#CADMENTEX_MENS_CONSECUTIVO").val(idCons);
    $("#CADMENTEX_CONSECUTIVO").val(idText);
    $("#idSitio").val(idSitio);
    $("#idCons").val(idCons);
    $("#idText").val(idText);

    //-------------------------------

    var tabla = $('#tablaEnviarA').DataTable();
    var datosTabla = tabla.rows().data();
    var arrayTabla = [];

    $.each(datosTabla, function (i, val) {
        filasTabla = {
            CADMENTEX_MENS_SITIOGRABA: idSitio,
            CADMENTEX_MENS_CONSECUTIVO: idCons,
            CADMENTEX_CONSECUTIVO: idText,
            CADUSUA_DESTINO: datosTabla[i][6],
            ESTADO: datosTabla[i][8],
            FECHA_NOTIFICACION: datosTabla[i][10],
            FECHA_GRABACION: datosTabla[i][11],
            CADFUER_DESTINO: datosTabla[i][2],
            CADCANA_DESTINO: datosTabla[i][4]
        };

        arrayTabla.push(filasTabla);
    });

    $.ajax({
        type: 'POST',
        //url: '/EnvioMensajes/MensajesTexto',
        //url: '/EnvioMensajes/CreacionMensajesTextoUsuario',
        url: rutaUsuario,
        dataType: 'json',
        data: { usuariosTabla: arrayTabla },
        success: function (resultado) {
            //console.log("Entro");
            if (resultado.result) {
                //Ajax Guardar Texto
                Enviar(idSitio, idCons);
                limpiarFormularios();
                GestionarAlerta("success", resultado.datos);
            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

function VerMensajes(idSitio, idCons) {
    _idSitio = idSitio;
    _idCons = idCons;

    $("#CADMENS_SITIOGRABA").val(idSitio);
    $("#CADMENS_CONSECUTIVO").val(idCons);

    $.ajax({
        type: 'POST',
        //url: 'GetTextoMensajes',
        url: urlTextoMensajes,
        dataType: 'html',
        data: { idSitio: idSitio, idCons: idCons }
    }).done(function (textoMensajes) {
        if ($.fn.DataTable.isDataTable("#tablaTexto")) {
            $("#tablaTexto").DataTable().destroy();
        }
        $("#tablaTexto tbody").empty();
        $("#tablaTexto tbody").html(textoMensajes);
        AplicarPluginTabla("tablaTexto");
    });
};

function VerEnviarA(idSitio, idCons, idText) {
    _idSitio = idSitio;
    _idCons = idCons;
    _idText = idText;

    $("#CADMENS_SITIOGRABA").val(idSitio);
    $("#CADMENS_CONSECUTIVO").val(idCons);
    $("#CONSECUTIVO").val(idText);

    $.ajax({
        type: 'POST',
        //url: 'GetEnviarMensajesA',
        url: urlMensajesA,
        dataType: 'html',
        data: { idSitio: idSitio, idCons: idCons, idText: idText }
    }).done(function (enviarA) {
        if ($.fn.DataTable.isDataTable("#tablaEnviarA")) {
            $("#tablaEnviarA").DataTable().destroy();
        }
        $("#tablaEnviarA tbody").empty();
        $("#tablaEnviarA tbody").html(enviarA);
        AplicarPluginTabla("tablaEnviarA");
    });
};

function Enviar(idSitio, idCons) {
    //alert("Entra " + idSitio + " - " + idCons + " - ")
    $.ajax({
        type: 'POST',
        //url: '/EnvioMensajes/EnviarMensaje',
        url: urlEnviar,
        dataType: 'json',
        data: { idSitio: idSitio, idCons: idCons },
        success: function (resultado) {
            if (resultado.result) {
                //window.location.href = '@Url.Action("Index", "EnvioMensajes")';
                GestionarAlerta("success", resultado.datos);
            } else {
                //implementar cuando existan problemas
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
};

function limpiarFormularios() {
    $("#CentroMensajes")[0].reset();
    $("#CentroTexto")[0].reset();
    $("#CentroUsuarios")[0].reset();

    $('#listaSitios').val('').trigger('chosen:updated');
    $('#CADFUER_DESTINO').val('').trigger('chosen:updated');
    $('#CADCANA_DESTINO').val('').trigger('chosen:updated');
    $('#CADUSUA_DESTINO').val('').trigger('chosen:updated');
    $('#ESTADO').val('').trigger('chosen:updated');

    var tabla = $('#tablaEnviarA').DataTable();
    tabla.clear().draw();
}

$(document).ready(function () {
    $("#CADFUER_DESTINO").change(function () {
        $("#CADCANA_DESTINO").empty();
        $.ajax({
            type: 'POST',
            //url: '/EnvioMensajes/GetCanalesByEntidad',
            url: urlCanales,
            dataType: 'json',
            data: { id: $("#CADFUER_DESTINO").val() },
            success: function (canales) {
                $.each(canales, function (i, canal) {
                    $("#CADCANA_DESTINO").append($('<option></option>').val(canal.CODIGO).html(canal.DESCRIPCION));
                });
                $("#CADCANA_DESTINO").chosen("destroy");
                $("#CADCANA_DESTINO").chosen();
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });
        return false;
    })
});

$(document).ready(function () {
    $("#CADCANA_DESTINO").change(function () {
        $("#CADUSUA_DESTINO").empty();
        console.log($("#CADFUER_DESTINO").val() + " - " + $("#CADCANA_DESTINO").val());
        $.ajax({
            type: 'POST',
            //url: '/EnvioMensajes/GetUsuariosByEntidadCanal',
            url: urlUsuarios,
            dataType: 'json',
            data: { idFuerza: $("#CADFUER_DESTINO").val(), idCanal: $("#CADCANA_DESTINO").val() },
            success: function (usuarios) {
                $.each(usuarios, function (i, usuario) {
                    $("#CADUSUA_DESTINO").append($('<option></option>').val(usuario.CODIGO).html(usuario.DESCRIPCION));
                });
                $("#CADUSUA_DESTINO").chosen("destroy");
                $("#CADUSUA_DESTINO").chosen();
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });
        return false;
    })
});