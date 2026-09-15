var parametrosAjaxMedios = [
       { "data": "Dummy", "orderable": false },
       { "data": "Boton", "orderable": false },
       { "data": "DESC_PATRULLA", "orderable": false },
       { "data": "SITIO_GRABA", "orderable": false },
       { "data": "CONSECUTIVO", "orderable": false },
       { "data": "ESTADO_PATRULLA", "orderable": false },
       { "data": "TIPO", "orderable": false },
       { "data": "ESTADO", "orderable": false },
       { "data": "CUENTA", "orderable": false },
];

$(document).ready(function () {
    CargaMedios();
});

function CargarMedio(codigo, descripcion) {
    console.log(codigo);
    $("#txt_desPatrulla").empty();
    $("#txt_desPatrulla").val(descripcion);

    $("#CADMEDISP_CONSECUTIVO").empty();
    $("#CADMEDISP_CONSECUTIVO").val(codigo);

    $("#ModalMedios").modal("toggle");
}

function InicializarElementos(opcion) {
    switch (opcion) {
        case "CREAR":
            //AplicarChosen();
            AplicarDateTimePlugin();
            break;
    }
}

function AplicarDateTimePlugin() {
    $(".FechaHora").datetimepicker({
        format: 'DD/MM/YYYY HH:mm:ss',
        showClear: true
    });
}

function CargaMedios() {
    if ($.fn.DataTable.isDataTable("#_tblMedios")) {
        $("#_tblMedios").DataTable().destroy();
        $("#_tblMedios tbody").empty();
    }

    AplicarPluginTablaAjax("_tblMedios", urlMedios, parametrosAjaxMedios);
    // $("#ModalMedios").modal();
}

function AbrirModalMedios() {
    $("#ModalMedios").modal();
}