function cargarTabla() {
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false },
        { "data": "Boton", "orderable": false },
        { "data": "CONSECUTIVO" },
        { "data": "DSP_SITIO_GRABA" },
        { "data": "FECHA" },
        { "data": "DSP_CADSITI_CODIGO" },
        { "data": "DSP_CADPATR_CODIGO" },
        { "data": "OBSERVACIONES" }
    ];

    AplicarPluginTablaAjax("tabla", urlTabla + "?idSitio=" + globalSitio + "&idFuerza=" + globalFuerza, parametrosAjax);
}

$("#FECHA").dblclick(function () {
    TraerFecha("FECHA");
});

function TraerFecha(idCampo) {
    $.ajax({
        type: 'POST',
        url: urlFecha,
        dataType: 'json',
        success: function (resultado) {
            if (resultado.result) {
                $("#" + idCampo).val(resultado.datos);
            } else {
                GestionarAlerta("warning", resultado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

function cargarSitios() {
    var parametrosAjax = [
    { "data": "Dummy", "orderable": false },
    { "data": "Boton", "orderable": false },
    //{ "data": "CONSECUTIVO" },
    { "data": "DESCRIPCION" }
    ];

    $("#tablaSitios").DataTable().destroy();
    $("#tablaSitios tbody").empty();
    AplicarPluginTablaAjax("tablaSitios", urlTablaSitios + "?idSitio=" + globalSitio, parametrosAjax);
    $("#ModalSitios").modal();
}

function cargarValorSitio(id, descripcion) {
    $("#CADSITI_CODIGO").val(id);
    $("#DSP_CADSITI_CODIGO").val(descripcion);

    $("#ModalSitios").modal("toggle");
}

function cargarPatrullas() {
    var parametrosAjax = [
    { "data": "Dummy", "orderable": false },
    { "data": "Boton", "orderable": false },
    //{ "data": "CONSECUTIVO" },
    { "data": "DESC_PATRULLA" }
    ];

    var ruta = urlTablaPatrullas + "?idFuerza=" + globalFuerza + "&idCanal=" + globalCanal + "&idSitio=" + globalSitio + "&turnoS=" + globalTurnoS + "&turnoA=" + globalTurnoA + "";

    $("#tablaPatrullas").DataTable().destroy();
    $("#tablaPatrullas tbody").empty();
    AplicarPluginTablaAjax("tablaPatrullas", ruta, parametrosAjax);
    $("#ModalPatrullas").modal();
}

function cargarValorPatrulla(idUnidadF, idUnidadC, codigo, descripcion) {
    $("#CADPATR_FUERZA").val(idUnidadF);
    $("#CADPATR_UNDE").val(idUnidadC);
    $("#CADPATR_CODIGO").val(codigo);
    $("#DSP_CADPATR_CODIGO").val(descripcion);

    $("#ModalPatrullas").modal("toggle");
}

//function EditarRegistro(urlEditar, idRuta) {
//    var ruta = urlEditar + "?id="+idRuta+"&idFuerza=" + idFuerza + "&idSitio=" + idSitio + "&numeLlamada=" + numeLlamada + "&idConsecutivo=" + idConsecutivo + "&numeEvento=" + numeEvento + "";
//    window.location.href = ruta;
//}

//function EliminarRegistro(urlEliminar, idRuta) {
//    var ruta = urlEliminar + "?id=" + idRuta + "&idFuerza=" + idFuerza + "&idSitio=" + idSitio + "&numeLlamada=" + numeLlamada + "&idConsecutivo=" + idConsecutivo + "&numeEvento=" + numeEvento + "";
//    EliminarRegistroGeneral(ruta);
//}