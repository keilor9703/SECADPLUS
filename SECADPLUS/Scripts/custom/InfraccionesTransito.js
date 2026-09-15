AplicarPluginTabla("tablaCapitulos");
AplicarPluginTabla("tablaTitulos");

function IrCrearTitulos(idCap, nomCap) {
    if (idCap) {
        window.location.href = urlCrear + '?id=' + idCap + '&nom=' + nomCap;
    }
}

function VerTitulos(idCap, nomCap) {
    _idCap = idCap;
    _nomCap = nomCap;

    $("#COD_CAPITULO").val(idCap);
    $("#NOM_CAPITULO").val(nomCap);

    $.ajax({
        type: 'POST',
        //url: 'GetTitulos',
        url: urlTitulos,
        dataType: 'html',
        data: { id: idCap }
    }).done(function (titulos) {
        if ($.fn.DataTable.isDataTable("#tablaTitulos")) {
            $("#tablaTitulos").DataTable().destroy();
        }
        $("#tablaTitulos tbody").empty();
        $("#tablaTitulos tbody").html(titulos);
        AplicarPluginTabla("tablaTitulos");
    });
};