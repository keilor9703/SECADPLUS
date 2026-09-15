var parametrosAjax = [
        { "data": "Dummy", "orderable": false },
        { "data": "Boton", "orderable": false },
        { "data": "CODIGO" },
        { "data": "DESCRIPCION" },
        { "data": "PRIORIDAD" },
        { "data": "IMPORTANCIA" },
        { "data": "TIPO_USO" },
];

AplicarPluginTablaAjax("tablaCasos", urlTabla, parametrosAjax);

//AplicarPluginTabla("tablaCasos");
AplicarPluginTabla("tablaPreguntas");

function IrAsociarPreguntas(idCaso, nomCaso) {
    if (idCaso) {
        window.location.href = urlCrear + '?id=' + idCaso + '&nom=' + nomCaso;
    }
}

function VerPreguntas(idCaso, nomCaso) {
    _idCaso = idCaso;
    _nomCaso = nomCaso;

    $("#COD_CASO").val(idCaso);
    $("#NOM_CASO").val(nomCaso);

    $.ajax({
        type: 'POST',
        //url: 'GetPreguntas',
        url: urlPreguntas,
        dataType: 'html',
        data: { id: idCaso }
    }).done(function (preguntas) {
        if ($.fn.DataTable.isDataTable("#tablaPreguntas")) {
            $("#tablaPreguntas").DataTable().destroy();
        }
        $("#tablaPreguntas tbody").empty();
        $("#tablaPreguntas tbody").html(preguntas);
        AplicarPluginTabla("tablaPreguntas");
    });
};

//function ActualizarVigente(idCaso, idPreg, element) {
//    //alert("Entra " + idCaso + " " + idPreg + " " + element.checked)
//    $.ajax({
//        type: 'POST',
//        url: '@Url.Action("EditarVigente")',
//        dataType: 'json',
//        data: { idCaso: idCaso, idPreg: idPreg, checkVigente: element.checked },
//        success: function (estado) {
//            //alert('Actualizado');
//        },
//        error: function (ex) {
//            alert('Fallo Actualización.' + ex);
//        }
//    });
//};