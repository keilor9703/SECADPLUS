AplicarPluginTabla("tablaTipos");
AplicarPluginTabla("tablaClases");

function IrCrearClase(idTip, nomTip) {
    if (idTip) {
        window.location.href = urlCrear + '?id=' + idTip + '&nom=' + nomTip;
    }
}

function VerClases(idTip, nomTip) {
    _idTip = idTip;
    _nomTip = nomTip;

    $("#COD_TIPO").val(idTip);
    $("#NOM_TIPO").val(nomTip);

    $.ajax({
        type: 'POST',
        //url: 'GetClases',
        url: urlClases,
        dataType: 'html',
        data: { id: idTip }
    }).done(function (clases) {
        if ($.fn.DataTable.isDataTable("#tablaClases")) {
            $("#tablaClases").DataTable().destroy();
        }
        $("#tablaClases tbody").empty();
        $("#tablaClases tbody").html(clases);
        AplicarPluginTabla("tablaClases");
    });
};