function cargarTabla() {
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false },
        { "data": "Boton", "orderable": false },
        { "data": "CONSECUTIVO_RUTA" },
        { "data": "ORIGEN" },
        { "data": "DESTINO" },
        { "data": "HORA_INICIA" },
        { "data": "HORA_LLEGA" },
        { "data": "DSC_NOVEDAD" },
        { "data": "OBSERVACIONES" }
    ];

    AplicarPluginTablaAjax("tabla", urlTabla + "?idConsecutivo=" + idConsecutivo + "&numeEvento=" + numeEvento, parametrosAjax);
}

$("#HORA_INICIA").dblclick(function () {
    $("#alertaJs").empty();

    var origen = $("#ORIGEN").val();
    var destino = $("#DESTINO").val();

    if (origen && destino) {
        TraerHora("HORA_INICIA");
    } else {
        GestionarAlerta("warning", "Debe asignar Origen y Destino..");
    }
});

$("#HORA_LLEGA").dblclick(function () {
    $("#alertaJs").empty();

    var origen = $("#ORIGEN").val();
    var destino = $("#DESTINO").val();

    if (origen && destino) {
        TraerHora("HORA_LLEGA");
    } else {
        GestionarAlerta("warning", "Debe asignar Origen y Destino..");
    }
});

function TraerHora(idCampo) {
    $.ajax({
        type: 'POST',
        url: urlHora,
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

function EditarRegistro(urlEditar, idRuta) {
    var ruta = urlEditar + "?id=" + idRuta + "&idFuerza=" + idFuerza + "&idSitio=" + idSitio + "&numeLlamada=" + numeLlamada + "&idConsecutivo=" + idConsecutivo + "&numeEvento=" + numeEvento + "";
    window.location.href = ruta;
}

function EliminarRegistro(urlEliminar, idRuta) {
    var ruta = urlEliminar + "?id=" + idRuta + "&idFuerza=" + idFuerza + "&idSitio=" + idSitio + "&numeLlamada=" + numeLlamada + "&idConsecutivo=" + idConsecutivo + "&numeEvento=" + numeEvento + "";
    EliminarRegistroGeneral(ruta);
}