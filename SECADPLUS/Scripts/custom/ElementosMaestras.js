AplicarPluginTabla("tablaTipoBienes");
AplicarPluginTabla("tablaMarcas");
AplicarPluginTabla("tablaClasesBienes");
AplicarPluginTabla("tablaCaracteristicas");

function IrCrearMarcas(tibiCodigo) {
    window.location.href = urlCrearMarcas + '?tibiCodigo=' + tibiCodigo + '&nomTipoBien=' + nomTipoBien;
}

function IrCrearClasesBien(tibiCodigo) {
    window.location.href = urlCrearClases + '?tibiCodigo=' + tibiCodigo + '&nomTipoBien=' + nomTipoBien;
}

function IrCrearCaracteristicas(tibiCodigo) {
    window.location.href = urlCrearCaracteristicas + '?tibiCodigo=' + tibiCodigo + '&nomTipoBien=' + nomTipoBien;
}

function VerMarcas(tibiCodigo, tibiNom) {
    idTipoCodigo = tibiCodigo;
    nomTipoBien = tibiNom;

    $("#TIBI_CODIGO").val(tibiCodigo);
    $("#TIPO_BIEN1").val(nomTipoBien);
    $("#TIPO_BIEN2").val(nomTipoBien);
    $("#TIPO_BIEN3").val(nomTipoBien);

    $.ajax({
        type: 'POST',
        //url: 'GetMarcas',
        url: urlMarcas,
        dataType: 'html',
        data: { tibiCodigo: tibiCodigo }
    }).done(function (marcas) {
        if ($.fn.DataTable.isDataTable("#tablaMarcas")) {
            $("#tablaMarcas").DataTable().destroy();
        }
        $("#tablaMarcas tbody").empty();
        $("#tablaMarcas tbody").html(marcas);
        AplicarPluginTabla("tablaMarcas");
    });

    $.ajax({
        type: 'POST',
        //url: 'GetClasesBien',
        url: urlClases,
        dataType: 'html',
        data: { tibiCodigo: tibiCodigo }
    }).done(function (marcas) {
        if ($.fn.DataTable.isDataTable("#tablaClasesBienes")) {
            $("#tablaClasesBienes").DataTable().destroy();
        }
        $("#tablaClasesBienes tbody").empty();
        $("#tablaClasesBienes tbody").html(marcas);
        AplicarPluginTabla("tablaClasesBienes");
    });

    $.ajax({
        type: 'POST',
        //url: 'GetCaracteristicas',
        url: urlCaracteristicas,
        dataType: 'html',
        data: { tibiCodigo: tibiCodigo }
    }).done(function (marcas) {
        if ($.fn.DataTable.isDataTable("#tablaCaracteristicas")) {
            $("#tablaCaracteristicas").DataTable().destroy();
        }
        $("#tablaCaracteristicas tbody").empty();
        $("#tablaCaracteristicas tbody").html(marcas);
        AplicarPluginTabla("tablaCaracteristicas");
    });
};