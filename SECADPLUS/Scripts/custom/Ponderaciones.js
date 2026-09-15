$(".chosen-select").chosen();

var _idTipo = $("#TIPO").val();
var _nomTipo = $("#TIPO option:selected").text();

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

function IrCrearPonderar(idTipo, nomTipo, idTitu, nomTitu, idCond, nomCond) {
    if (idCond != 0) {
        window.location.href = urlCrearPonderada + "?idTipo=" + idTipo + "&nomTipo=" + nomTipo + "&idTitu=" + idTitu + "&nomTitu=" + nomTitu + "&idCond=" + idCond + "&nomCond=" + nomCond;
    }
}

function IrCrearDetallePonderar(idTipo, nomTipo, idTitu, nomTitu, idCond, nomCond, idPond) {
    if (idPond != 0) {
        window.location.href = urlCrearDetalle + "?idTipo=" + idTipo + "&nomTipo=" + nomTipo + "&idTitu=" + idTitu + "&nomTitu=" + nomTitu + "&idCond=" + idCond + "&nomCond=" + nomCond + "&idPond=" + idPond;
    }
}

function VerConductas(idTitu, nomTitu) {
    _idTitu = idTitu;
    _nomTitu = nomTitu;

    //$("#COD_CONDUCTA").val(idTitu);
    $("#NOM_TITULO").val(nomTitu);

    $.ajax({
        type: 'POST',
        //url: 'GetConductas',
        url: urlConductas,
        dataType: 'html',
        data: { idTitu: _idTitu, nomTitu: _nomTitu }
    }).done(function (clases) {
        if ($.fn.DataTable.isDataTable("#tablaConductas")) {
            $("#tablaConductas").DataTable().destroy();
        }
        $("#tablaConductas tbody").empty();
        $("#tablaConductas tbody").html(clases);
        AplicarPluginTabla("tablaConductas");
    });
};

function VerPonderar(idCond, nomCond) {
    _idCond = idCond;
    _nomCond = nomCond;

    //$("#COD_CONDUCTA").val(idCond);
    $("#NOM_CONDUCTA").val(nomCond);

    $.ajax({
        type: 'POST',
        //url: 'GetPonderar',
        url: urlPonderar,
        dataType: 'html',
        data: { idTipo: _idTipo, nomTipo: _nomTipo, idTitu: _idTitu, nomTitu: _nomTitu, idCond: _idCond, nomCond: _nomCond }
    }).done(function (ponderar) {
        if ($.fn.DataTable.isDataTable("#tablaPonderar")) {
            $("#tablaPonderar").DataTable().destroy();
        }
        $("#tablaPonderar tbody").empty();
        $("#tablaPonderar tbody").html(ponderar);
        AplicarPluginTabla("tablaPonderar");
    });
};

function VerDetallePonderar(idPond, nomPond) {
    _idPond = idPond;
    _nomPond = nomPond;

    $("#COD_PONDERAR").val(idPond);
    $("#NOM_PONDERAR").val(idPond);

    $.ajax({
        type: 'POST',
        //url: 'GetDetallePonderar',
        url: urlDetalle,
        dataType: 'html',
        data: { idTipo: _idTipo, nomTipo: _nomTipo, idTitu: _idTitu, nomTitu: _nomTitu, idCond: _idCond, nomCond: _nomCond, idPond: _idPond }
    }).done(function (detallePonderar) {
        if ($.fn.DataTable.isDataTable("#tablaDetallePonderar")) {
            $("#tablaDetallePonderar").DataTable().destroy();
        }
        $("#tablaDetallePonderar tbody").empty();
        $("#tablaDetallePonderar tbody").html(detallePonderar);
        AplicarPluginTabla("tablaDetallePonderar");
    });
};

$(document).ready(function () {
    $("#CLBI_TIBI_CODIGO").change(function () {
        $("#CLBI_CODIGO").empty();
        $.ajax({
            type: 'POST',
            //url: '@Url.Action("GetClasesBien")',
            //url: '/Ponderaciones/GetClasesBien',
            url: urlClases,
            dataType: 'json',
            data: { id: $("#CLBI_TIBI_CODIGO").val() },
            success: function (clasesBien) {
                $.each(clasesBien, function (i, clase) {
                    $("#CLBI_CODIGO").append($('<option></option>').val(clase.CODIGO).html(clase.DESCRIPCION));
                });
                $("#CLBI_CODIGO").chosen("destroy");
                $("#CLBI_CODIGO").chosen();
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });
        return false;
    })
});