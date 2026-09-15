$(".chosen-select").chosen();

var _idTipo = $("#TIPO").val();
var _nomTipo = $("#TIPO option:selected").text();

AplicarPluginTabla("tablaTitulos");
AplicarPluginTabla("tablaConductas");

function IrCrearTitulos(idTipo, nomTipo) {
    if (idTipo) {
        window.location.href = urlCrearTitulo + "?idTipo=" + idTipo + "&nomTipo=" + nomTipo;
    }
}

function IrCrearConducta(idTipo, nomTipo, idTitu, nomTitu) {
    if (idTitu != 0) {
        window.location.href = urlCrearConducta + "?idTipo=" + idTipo + "&nomTipo=" + nomTipo + "&idTitu=" + idTitu + "&nomTitu=" + nomTitu;
    }
}

$(document).ready(function () {
    $("#TIPO").change(function () {
        _idTipo = $("#TIPO").val();
        _nomTipo = $("#TIPO option:selected").text();

        $("#COD_CONDUCTA").val(null);
        $("#NOM_CONDUCTA").val(null);

        $.ajax({
            type: 'POST',
            //url: 'GetTitulos',
            url: urlTitulos,
            dataType: 'html',
            data: { idTipo: $("#TIPO").val(), nomTipo: _nomTipo }
        }).done(function (clases) {
            if ($.fn.DataTable.isDataTable("#tablaTitulos")) {
                $("#tablaTitulos").DataTable().destroy();
            }
            $("#tablaTitulos tbody").empty();
            $("#tablaTitulos tbody").html(clases);
            AplicarPluginTabla("tablaTitulos");

            if ($.fn.DataTable.isDataTable("#tablaConductas")) {
                $("#tablaConductas").DataTable().destroy();
            }
            $("#tablaConductas tbody").empty();
            AplicarPluginTabla("tablaConductas");
        });
    })
});

function VerConductas(idTitu, nomTitu) {
    _idTitu = idTitu;
    _nomTitu = nomTitu;

    $("#COD_CONDUCTA").val(idTitu);
    $("#NOM_CONDUCTA").val(nomTitu);

    $.ajax({
        type: 'POST',
        //url: 'GetConductas',
        url: urlConductas,
        dataType: 'html',
        data: { idTitu: idTitu, nomTitu: _nomTitu }
    }).done(function (clases) {
        if ($.fn.DataTable.isDataTable("#tablaConductas")) {
            $("#tablaConductas").DataTable().destroy();
        }
        $("#tablaConductas tbody").empty();
        $("#tablaConductas tbody").html(clases);
        AplicarPluginTabla("tablaConductas");
    });
};