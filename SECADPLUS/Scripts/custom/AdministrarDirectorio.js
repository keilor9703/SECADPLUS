$(document).ready(function () {
    var estructura = '<table class="col-md-10 col-lg-12"><tr><td> Posición </td><td> Longitud </td><td> Descripción <td><tr>' +
                        '<tr><td>001</td><td>12</td><td>Nro Telefonico</td></tr>' +
                        '<tr><td>013</td><td>60</td><td>Nombre Propietario</td></tr>' +
                        '<tr><td>073</td><td>60</td><td>Direccion</td></tr>' +
                        '<tr><td>133</td><td>60</td><td>Barrio descripcion</td></tr>' +
                        '<tr><td>193</td><td>15</td><td>Nit</td></tr><table>   <br /> <br />';

    $('#txtEstructuraArchivo').html(estructura);
});

$(function () {
    $('button.file-download').click(function () {
        var actual = 0;
        console.log(document.getElementById('uploader').files.length);
        if (document.getElementById('uploader').files.length > 0) {
            actual = document.getElementById('uploader').files[0].size;
        }
        console.log(actual);
        if (actual >= longMaxArchivo) {
            GestionarAlerta("warning", "El archivo seleccionado excede el tamaño limite para cargue de archivos. Por favor reemplace el archivo seleccionado.");
            return;
        }

        if ($('#uploader').val() != "" && $('#codigoLugar').val() != "") {
            GestionarAlerta("success", "Se proceso exitosamente el archivo. ");
        }

        $("#frm_directorio").submit();
    });
});

function ProcesoAutomatico() {
    if ($('#codigoLugar').val() != "") {
        $.ajax({
            url: urlActionProcesoAutomatico,
            method: "POST",
            content: "application/json; charset=utf-8",
            data: { strCodigoLugar: $('#codigoLugar').val() },
            dataType: 'json',
            success: function (resultado) {
                console.log(resultado.proceso);
                GestionarAlerta("success", resultado.proceso);
            }
        });
    } else {
        GestionarAlerta("warning", "Debe seleccionar una ciudad o municipio");
    }
}

function CargaCiudades() {
    var parametrosAjaxCiudad = [
        { "data": "Dummy", "orderable": false },
        { "data": "Boton", "orderable": false },
        { "data": "CODIGO" },
        { "data": "DESCRIPCION" },
    ];

    $("#tablaCiudades").DataTable().destroy();
    $("#tablaCiudades tbody").empty();

    AplicarPluginTablaAjax("tablaCiudades", (urlActionCargaCiudades + "?tab=1"), parametrosAjaxCiudad);
    $("#ModalCiudades").modal();
}

function CargarCiudad(codigo, descripcion) {
    $("#codigoLugar").empty();
    $("#codigoLugar").val(codigo + "-" + descripcion);

    $("#ModalCiudades").modal("toggle");
}

function CargarDirrectorio() {
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.5%" },
        { "data": "NUME_TELEFONO" },
        { "data": "NOMB_PROPIETARIO" },
        { "data": "NIT" },
        { "data": "DIRE_PROPIETA" },
        { "data": "BARRIO" },
        { "data": "CADLUGE_CODIGO" },
        { "data": "CADLUGE" },
        { "data": "BARR_CODIGO" },
        { "data": "BARRIO_ASIGNADO" },
    ];

    $("#tablaAdministrarDirectorio").DataTable().destroy();
    $("#tablaAdministrarDirectorio tbody").empty();

    AplicarPluginTablaAjaxDirectorioTel("tablaAdministrarDirectorio", (urlTabla + "?cadenaFiltros=" + ""), parametrosAjax);
}

function CargarDirrectorioBarrios() {
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.5%" },
        { "data": "NUME_TELEFONO" },
        { "data": "NOMB_PROPIETARIO" },
        { "data": "NIT" },
        { "data": "DIRE_PROPIETA" },
        { "data": "BARRIO" },
        { "data": "CADLUGE_CODIGO" },
        { "data": "CADLUGE" },
        { "data": "BARR_CODIGO" },
        { "data": "BARRIO_ASIGNADO" },
    ];

    $("#tablaAdministrarDirectorio").DataTable().destroy();
    $("#tablaAdministrarDirectorio tbody").empty();

    AplicarPluginTablaAjaxDirectorioTel("tablaAdministrarDirectorio", (urlTabla + "?cadenaFiltros=" + "barriosNulls"), parametrosAjax);
}

function CargaCiudadesDirectorio() {
    var parametrosAjaxCiudad = [
        { "data": "Dummy", "orderable": false },
        { "data": "Boton", "orderable": false },
        { "data": "CODIGO" },
        { "data": "DESCRIPCION" },
    ];

    $("#tablaCiudades").DataTable().destroy();
    $("#tablaCiudades tbody").empty();

    AplicarPluginTablaAjax("tablaCiudades", (urlActionCargaCiudades + "?tab=2"), parametrosAjaxCiudad);
    $("#ModalCiudades").modal();
}

function CargarCiudadDirectorio(codigo, descripcion) {
    $("#BARR_LUGE_CODIGO").empty();
    $("#BARR_LUGE_CODIGO").val(codigo);

    $("#codigoMunicipio").empty();
    $("#codigoMunicipio").val(codigo);

    $("#municipio").empty();
    $("#municipio").val(descripcion);
    CargaBarrios();
    $("#ModalCiudades").modal("toggle");
}

function CargarBarrio(codigo, descripcion) {
    $("#codigoBarrAntencion").empty();
    $("#codigoBarrAntencion").val(codigo);

    $("#barrAtencion").empty();
    $("#barrAtencion").val(descripcion);

    $("#ModalBarrios").modal("toggle");
}

function CargaBarrios() {
    var parametrosAjaxBarrio = [
       { "data": "Dummy", "orderable": false },
       { "data": "Boton", "orderable": false },
       { "data": "CODIGOBARRIO" },
       { "data": "DESCRIPCIONBARRIO" },
    ];

    $("#_tblBarrios").DataTable().destroy();
    $("#_tblBarrios tbody").empty();
    AplicarPluginTablaAjax("_tblBarrios", ValorCiudad(), parametrosAjaxBarrio);
}

function ValorCiudad() {
    var urlCiudad = (urlsCargaBarriosDir + "?lugecodigo=" + $("#BARR_LUGE_CODIGO").val());
    return urlCiudad;
}

function MostaraBarrio() {
    var CodMunicipio = $('#codigoMunicipio').val();
    console.log(CodMunicipio);
    if (CodMunicipio != "") {
        var parametrosAjaxBarrio = [
             { "data": "Dummy", "orderable": false },
             { "data": "Boton", "orderable": false },
             { "data": "CODIGOBARRIO" },
             { "data": "DESCRIPCIONBARRIO" },
        ];

        $("#_tblBarrios").DataTable().destroy();
        $("#_tblBarrios tbody").empty();
        AplicarPluginTablaAjax("_tblBarrios", (urlsCargaBarriosDir + "?lugecodigo=" + CodMunicipio), parametrosAjaxBarrio);
    } else {
        $("#_tblBarrios").DataTable().destroy();
        $("#_tblBarrios tbody").empty();
    }

    $("#ModalBarrios").modal();
}

function datosNuevos() {
    if ($('#telefono').val() == "" || $('#propietario').val() == ""
        || $('#direccion').val() == "" || $('#barrio').val() == "") {
        GestionarAlerta("warning", "Debe seleccionar los campos requeridos");
        return;
    }

    var arrDatos = new Array($('#telefono').val(), $('#propietario').val(),
        $('#nit').val(), $('#direccion').val(), $('#barrio').val(),
        $('#codigoMunicipio').val(), $('#codigoBarrAntencion').val(),
        $('#barrAtencion').val());

    $.ajax({
        type: "POST",
        url: urlsDatosUsuarioDir,
        data: "{'datos':" + JSON.stringify(arrDatos) + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (resultado) {
            if (resultado.result) {
                GestionarAlerta(resultado.tipos, resultado.mensajes);
                limpiarCampos();
            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
    });
}

function limpiarCampos() {
    $('#telefono').val("");
    $('#propietario').val("");
    $('#nit').val("");
    $('#direccion').val("");
    $('#barrio').val("");
    $('#codigoMunicipio').val("");
    $('#codigoBarrAntencion').val("");
    $('#barrAtencion').val("");
    $('#municipio').val("");
}

function CargarDatosDirecotio(tablaId, object) {
    tabla = $("#" + tablaId).DataTable();
    totalTabla = tabla.rows().data();

    datosTabla = tabla.row(object).data();

    if (totalTabla.length == 0) {
        return;
    }
    datosTabla = tabla.row(object).data();

    $('#telefono').val(datosTabla["NUME_TELEFONO"]);
    $('#propietario').val(datosTabla["NOMB_PROPIETARIO"]);
    $('#nit').val(datosTabla["NIT"]);
    $('#direccion').val(datosTabla["DIRE_PROPIETA"]);
    $('#barrio').val(datosTabla["BARRIO"]);
    $('#codigoMunicipio').val(datosTabla["CADLUGE_CODIGO"]);
    $('#municipio').val(datosTabla["CADLUGE"]);
    $('#codigoBarrAntencion').val(datosTabla["BARR_CODIGO"]);
    $('#barrAtencion').val(datosTabla["BARRIO_ASIGNADO"]);
}

function CambiarBarriosManual() {
    var barrio = $('#barrio').val();
    var CodMunicipio = $('#codigoMunicipio').val();
    var CodBarrio = $('#codigoBarrAntencion').val();

    if (barrio != "" && CodMunicipio != "" && CodBarrio != "") {
        $.ajax({
            type: 'POST',
            url: urlCambioBarrioManual,
            dataType: 'json',
            data: { strBarrio: barrio, strCodLuge: CodMunicipio, strCodBarrNuevo: CodBarrio },
            success: function (resultados) {
                if (resultados.result == true) {
                    GestionarAlerta("success", resultados.mensaje);
                    limpiarCampos();

                    $("#tablaAdministrarDirectorio").DataTable().destroy();
                    $("#tablaAdministrarDirectorio tbody").empty();
                } else {
                    GestionarAlerta("warning", resultados.datos);
                }
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });
    } else {
        GestionarAlerta("warning", "Debe seleccionar un registro y buscar el nuevo barrio que va a asignar.");
    }
}