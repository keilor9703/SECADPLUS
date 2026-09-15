$(document).ready(function () {
    // AplicarDateRangePicker("#RangoFecha");

    $("#_btnunidad").click(function () {
        CargarTablaUnidades();
    });

    AsignarFechas();
});

function CargarTablaUnidades() {
    var parametros = [
                        { "data": "Dummy", "orderable": false },
                        { "data": "Boton", "orderable": false },
                        { "data": "CODIGO", "orderable": true },
                        { "data": "DESCRIPCION" },
                        { "data": "ExtNombrePadre" },
                        { "data": "SIGLA" },
                        { "data": "OPERATIVA" },
                        { "data": "VIGENTE" },
                        { "data": "ExtLugar" }
    ];
    AplicarPluginTablaAjax("_TablaUnidades", urlTablaUnidades, parametros);
    console.log("accede");
    $("#ModalUnidades").modal();
}

function cargarUnidad(codigo, descripcion) {
    $("#hf_CodigoPadre").val(codigo);
    $("#txt_Unidad").val(descripcion);
    $("#ModalUnidades").modal("toggle");
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
        showClear: true,
    });
}

function AsignarFechas() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!

    var yyyy = today.getFullYear();
    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }
    var today = dd + '/' + mm + '/' + yyyy + ' 00:00:00';
    var todayFinal = dd + '/' + mm + '/' + yyyy + ' 23:59:59';

    $('#txtFechaInicial').val(today);
    $('#txtFechaFinal').val(todayFinal);
}