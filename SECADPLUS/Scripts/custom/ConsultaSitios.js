function CargaParqueaderos(idSitio, nomSitio) {
    $("#parqueaderos").trigger("click");

    $("#sitioGrabacion1").val(nomSitio);

    //ActivarLoader("panelParqueaderos");

    var parametrosAjax = [
        { "data": "Dummy", "orderable": false },
        { "data": "CODIGO" },
        { "data": "DESCRIPCION" },
        { "data": "NOMBRECIUDAD" },
        { "data": "NOMBREBARRIO" },
        { "data": "DIRECCION" },
        { "data": "TELEFONO" },
        { "data": "HORARIO" },
        { "data": "USUA_TRANSACCION" },
        { "data": "FECHA_TRANSACCION" },
    ];

    $("#tablaParqueaderos").DataTable().destroy();
    $("#tablaParqueaderos tbody").empty();

    AplicarPluginTablaAjax("tablaParqueaderos", urlTablaParqueaderos + "?id=" + idSitio, parametrosAjax);

    //InactivarLoader("panelParqueaderos");
}

function CargaSitiosImportantes(idSitio, nomSitio) {
    $("#importantes").trigger("click");

    $("#sitioGrabacion2").val(nomSitio);

    //ActivarLoader("panelImportantes");

    var parametrosAjax = [
        { "data": "Dummy", "orderable": false },
        { "data": "CONSECUTIVO" },
        { "data": "CLASESITIODESCRIPCION", "width": "100%" },
        { "data": "DESCRIPCION" },
        { "data": "NOMBRECIUDAD" },
        { "data": "NOMBREBARRIO" },
        { "data": "DIRECCION" },
        { "data": "LATITUD" },
        { "data": "LONGITUD" },
        { "data": "TELEFONO" },
        { "data": "COMENTARIO" },
    ];

    $("#tablaImportantes").DataTable().destroy();
    $("#tablaImportantes tbody").empty();

    AplicarPluginTablaAjax("tablaImportantes", urlTablaImportantes + "?id=" + idSitio, parametrosAjax);

    //InactivarLoader("panelImportantes");
}