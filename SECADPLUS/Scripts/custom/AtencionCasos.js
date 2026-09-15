$(".chosen-select").chosen();

AplicarPluginTabla("tablaCasos");

function cargarCasos(opcion) {
    var parametrosAjax = [
    { "data": "Dummy", "orderable": false },
    { "data": "Boton", "orderable": false },
    { "data": "CODIGO" },
    { "data": "DESCRIPCION" }
    ];

    $("#tablaCasosDH").DataTable().destroy();
    $("#tablaCasosDH tbody").empty();
    AplicarPluginTablaAjax("tablaCasosDH", urlTablaCasos + "?opcion=" + opcion, parametrosAjax);
    $("#ModalCasos").modal();
}

function cargarValorCaso(id, descripcion, opcion) {
    switch (opcion) {
        case "desde":
            $("#CADCASO_CODIGO_DESDE").val(id);
            break;
        case "hasta":
            $("#CADCASO_CODIGO_HASTA").val(id);
            break;
        case "nuevo":
            $("#CADCASO_CODIGO").val(id);
            $("#DESCRIPCION").val(descripcion);
            break;
        case "edita":
            $("#CADCASO_CODIGO").val(id);
            $("#DESCRIPCION").val(descripcion);
            break;
        default:
    }

    $("#ModalCasos").modal("toggle");
}

function Procesar() {
    var idCasoDesde = $("#CADCASO_CODIGO_DESDE").val()
    var idCasoHasta = $("#CADCASO_CODIGO_HASTA").val()

    if (idCasoDesde && idCasoHasta) {
        $.ajax({
            type: 'POST',
            //url: '@Url.Action("ProcesarCasosCanal", "AtencionCasos")',
            url: urlProcesar,
            dataType: 'json',
            data: { idFuerza: _idFuerza, idCanal: _idCanal, casoDesde: idCasoDesde, casoHasta: idCasoHasta },
            success: function (estado) {
                if (estado.result) {
                    //window.location.href = '@Url.Action("Index", "AtencionCasos")' + "?idFuerza=" + _idFuerza + "&idCanal=" + _idCanal;
                    window.location.href = urlIndex + "?idFuerza=" + _idFuerza + "&idCanal=" + _idCanal;
                    GestionarAlerta("success", estado.datos);
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", estado.datos);
                }
            },
            error: function (ex) {
                alert('Fallo Proceso.' + ex);
            }
        });
    }
    else {
        GestionarAlerta("warning", "Debe seleccionar un rango desde - hasta");
    }
};

function IrCrearManual(idFuerza, idCanal) {
    window.location.href = urlCrear + "?idFuerza=" + idFuerza + "&idCanal=" + idCanal;
}