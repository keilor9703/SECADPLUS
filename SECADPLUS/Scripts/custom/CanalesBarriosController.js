var parametrosAjax = [
            { "data": "Dummy", "orderable": false },
            { "data": "DESCRIPCIONCIUDAD", "orderable": false },
            { "data": "DESCRIPCIONBARRIO" },
            { "data": "Boton", "orderable": false }
];
$(function () {
    //CargaCiudades();
});

function CargaCiudades() {
    //console.log(idFuerza);
    var parametrosAjax = [
        { "data": "Dummy", "orderable": false },
        { "data": "Boton", "orderable": false },
        { "data": "CODIGO" },
        { "data": "DESCRIPCION" },
    ];

    $("#tablaCiudades").DataTable().destroy();
    $("#tablaCiudades tbody").empty();
    //console.log(urlActionCargaCiudades);
    AplicarPluginTablaAjax("tablaCiudades", urlActionCargaCiudades, parametrosAjax);
    $("#ModalCiudades").modal();
}

function CargarCiudad(codigo, descripcion) {
    $("#BARR_LUGE_CODIGO").empty();
    $("#BARR_LUGE_CODIGO").val(codigo);
    $("#_CIUDAD").val(descripcion);
    CargaBarrios();
    $("#ModalCiudades").modal("toggle");
}

//control sobre los barrios seleccionados que se modifiquen
window.estados = [];
function BarrioCambia(elId, lugeCodigo) {
    //elId = $(this).attr("id");
    elEstado = $("#" + elId).is(':checked');
    asignacionBarrios = {
        id: elId,
        estado: elEstado,
        canalId: $("#CODIGOCANAL").val(),
        LugeCodigo: lugeCodigo
    };
    var yaExiste = false;
    for (var i = 0; i < estados.length; i++) {
        if (estados[i].id == elId) {
            estados[i].estado = elEstado;
            //console.log("cambio: " + estados[i].id);
            yaExiste = true;
        }
    }
    if (!yaExiste)
        estados.push(asignacionBarrios);
};

function CargaBarrios() {
    AplicarPluginTablaAjax("_tblBarrios", ValorCiudad(), parametrosAjax);
    $("#_tblBarrios").DataTable().ajax.url(ValorCiudad());
    $("#_tblBarrios").DataTable().ajax.reload();
}
function ValorCiudad() {
    var urlCiudad = (urlsCargaBarrios + "?lugecodigo=" + $("#BARR_LUGE_CODIGO").val() + "&canal=" + $("#CODIGOCANAL").val() + "&fuerzaId=" + $("#CODIGOFUERZA").val());

    return urlCiudad;
}
function EnviaEstados() {
    codCanal = $("#CODIGOCANAL").val();
    fuerzaId = $("#CODIGOFUERZA").val();
    var entityData = {
        "estadosBarriosCanales": estados,
        "canalCodigo": codCanal,
        "fuerzaId": fuerzaId
    };
    $.ajax({
        type: "POST",
        url: urlsEnviaBarrios,
        content: "application/json; charset=utf-8",
        cache: false,
        dataType: "json",
        data: entityData,
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        success: function (d) {
            if (d.result) {
                debugger
                //window.location.reload();
                GestionarAlerta("success", "Registros Ingresados para el canal: " + d.intNuevos + ", Registros Rechazados por Duplicidad: " + d.intDuplicados + ", Registros Rechazados por Error: " + d.intRechazadosPorError);
            } else {
                //implementar cuando existan problemas
                GestionarAlerta("warning", d.dato);
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log("error");
            //implementar cuando existan problemas
            GestionarAlerta("warning", errorThrown);
        }
    });
    $(".loader_decad").addClass("hidden");
    $(".loader_back").addClass("hidden");
    window.estados = [];
}