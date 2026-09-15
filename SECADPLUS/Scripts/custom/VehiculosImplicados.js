function VerCasos(idSitio, idBien) {
    _idSitio = idSitio;
    _idBien = idBien;

    $("#CADBIEN_SITIOGRABA").val(idSitio);
    $("#CADBIEN_CONSECUTIVO").val(idBien);

    $.ajax({
        type: 'POST',
        //url: 'GetCasos',
        url: urlCasos,
        dataType: 'html',
        data: { idSitio: idSitio, idBien: idBien }
    }).done(function (casos) {
        if ($.fn.DataTable.isDataTable("#tablaCasos")) {
            $("#tablaCasos").DataTable().destroy();
        }
        $("#tablaCasos tbody").empty();
        $("#tablaCasos tbody").html(casos);
        AplicarPluginTabla("tablaCasos");
    });
};

function IrCrearCaso(idSitio, idBien) {
    if (idBien != 0) {
        window.location.href = urlCrearCasos + "?idSitio=" + idSitio + "&idBien=" + idBien;
    }
}

$(document).ready(function () {
    $("#MARCA_CODIGO").change(function () {
        $("#LINEA_CODIGO").empty();
        $.ajax({
            type: 'POST',
            //url: '@Url.Action("GetLineasByMarca")',
            //url: '/VehiculosImplicadosDatos/GetLineasByMarca',
            url: urlLineas,
            dataType: 'json',
            data: { id: $("#MARCA_CODIGO").val() },
            success: function (lineasMarcas) {
                $.each(lineasMarcas, function (i, lineas) {
                    $("#LINEA_CODIGO").append($('<option></option>').val(lineas.CODIGO).html(lineas.DESCRIPCION));
                });
                $("#LINEA_CODIGO").chosen("destroy");
                $("#LINEA_CODIGO").chosen();
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });
        return false;
    })
});

$(document).ready(function () {
    $("#LUGE_CODIGO").change(function () {
        $("#LUGE_BARRIO").empty();
        $.ajax({
            type: 'POST',
            //url: '@Url.Action("GetBarriosByMunicipio")',
            //url: '/VehiculosImplicadosCasos/GetBarriosByMunicipio',
            url: urlBarrios,
            dataType: 'json',
            data: { id: $("#LUGE_CODIGO").val() },
            success: function (barrios) {
                $.each(barrios, function (i, barrio) {
                    $("#LUGE_BARRIO").append($('<option></option>').val(barrio.CODIGO).html(barrio.DESCRIPCION));
                });
                $("#LUGE_BARRIO").chosen("destroy");
                $("#LUGE_BARRIO").chosen();
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });
        return false;
    })
});

function ConsultarVehiculos(sitio) {    
    if ($.fn.DataTable.isDataTable("#tablaDatos")) {
        $("#tablaDatos").DataTable().destroy();
        $("#tablaDatos tbody").empty();
    }
    var urlFinal = urlConsultarVehiculos.replace(/amp;/g, "") + "?intSitio=" + sitio;
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CLASE", "orderable": true },
                { "data": "CONSECUTIVO", "orderable": true },
                { "data": "MARCA", "orderable": true },
                { "data": "LINEA", "orderable": true },
                { "data": "COLOR", "orderable": true },
                { "data": "PLACA", "orderable": true },
                { "data": "CHASIS", "orderable": true },
                { "data": "MOTOR", "orderable": true },
                { "data": "SERIE", "orderable": true },
                { "data": "OBSERVACIONES", "orderable": true },
                { "data": "USUA_TRANSACCION", "orderable": true },
                { "data": "FECHA_TRANSACCION", "orderable": true },
    ];
    AplicarPluginTablaAjax("tablaDatos", urlFinal, parametrosAjax);
}