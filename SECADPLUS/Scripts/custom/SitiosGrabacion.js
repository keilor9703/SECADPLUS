$(".chosen-select").chosen();

var idDepto = $("#DEPARTAMENTO").val();
var idFuerza = $("#CADUNDE_FUERZA").val();

$(document).ready(function () {
    console.log("accedio");
    $("#CADUNDE_FUERZA").change(function () {
        $("#CADUNDE_CODIGO").val('');
        $("#UNIDAD").val('');
        idFuerza = $("#CADUNDE_FUERZA").val();
    })
    $("#DEPARTAMENTO").change(function () {
        $("#LUGE_CIUDAD").val('');
        $("#CIUDAD").val('');
        idDepto = $("#DEPARTAMENTO").val();
    })

    $("#CADUNDE_CODIGO").change(function () {
        $("#CONS_REGIONAL").empty();
        $.ajax({
            type: 'POST',
            //url: '@Url.Action("GetRegionalByUnidad")',
            //url: '/SitiosGrabacion/GetRegionalByUnidad',
            url: urlRegional,
            dataType: 'json',
            data: { idFuerza: $("#CADUNDE_FUERZA").val(), idCodigo: $("#CADUNDE_CODIGO").val() },
            success: function (regional) {
                $.each(regional, function (i, region) {
                    $("#CONS_REGIONAL").append($('<option></option>').val(region.CONSECUTIVO).html(region.DESCRIPCION));
                });
                //$("#CONS_REGIONAL").val(data[1]);
                $("#CONS_REGIONAL").chosen("destroy");
                $("#CONS_REGIONAL").chosen();
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });
        return false;
    })
    $("#Copiar").click(function (e) {
        e.preventDefault();
        $.ajax({
            type: "POST",
            //url: '@Url.Action("CopiarReferencias")',
            //url: '/SitiosGrabacion/CopiarReferencias',
            url: urlCopiar,
            data: { idDestino: $("#idDestino").val(), idOrigen: $("#SITIOORIGEN").val(), descCiudad: $("#descCiudad").val(), idCiudad: $("#idCiudad").val() },
            success: function (estado) {
                if (estado.result) {
                    GestionarAlerta("success", estado.datos);
                } else {
                    GestionarAlerta("warning", estado.datos);
                }
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });
    });
});

function cargarCiudades() {
    if (idDepto != 0) {
        var parametrosAjax = [
            { "data": "Dummy", "orderable": false },
            { "data": "Boton", "orderable": false },
            { "data": "CODIGO" },
            { "data": "DESCRIPCION" },
            { "data": "TIPO", "orderable": false },
            { "data": "DPTO", "orderable": false },
        ];

        $("#tablaCiudades").DataTable().destroy();
        $("#tablaCiudades tbody").empty();

        AplicarPluginTablaAjax("tablaCiudades", urlTablaCiudad + "?id=" + idDepto, parametrosAjax);
        $("#ModalCiudades").modal();
    }
}

function cargarUnidades() {
    if (idFuerza != 0) {
        var parametrosAjax = [
            { "data": "Dummy", "orderable": false },
            { "data": "Boton", "orderable": false },
            { "data": "CODIGO" },
            { "data": "DESCRIPCION" },
        ];

        $("#tablaUnidades").DataTable().destroy();
        $("#tablaUnidades tbody").empty();

        AplicarPluginTablaAjax("tablaUnidades", urlTablaUnidades + "?id=" + idFuerza, parametrosAjax);
        $("#ModalUnidades").modal();
    }
}

//Cierra modales al seleccionar un registro dentro de su contenido
function cargarValorCiudad(id, nom) {
    $("#LUGE_CIUDAD").val(id);
    $("#CIUDAD").val(nom);
    $("#ModalCiudades").modal("toggle");
}

function cargarValorUnidad(id, nom) {
    $("#CADUNDE_CODIGO").val(id);
    $("#UNIDAD").val(nom)
    $("#ModalUnidades").modal("toggle");

    $("#CONS_REGIONAL").empty();
    $.ajax({
        type: 'POST',
        url: urlRegional,
        dataType: 'json',
        data: { idFuerza: $("#CADUNDE_FUERZA").val(), idCodigo: $("#CADUNDE_CODIGO").val() },
        success: function (regional) {
            $.each(regional, function (i, region) {
                $("#CONS_REGIONAL").append($('<option></option>').val(region.CONSECUTIVO).html(region.DESCRIPCION));
            });
            //$("#CONS_REGIONAL").val(data[1]);
            $("#CONS_REGIONAL").chosen("destroy");
            $("#CONS_REGIONAL").chosen();
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}