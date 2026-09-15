$(document).ready(function () {
    $('#firma').append("<option selected='selected' select value='" + "CR. NOMBRE APELLIDO" + "'>" + "CR. NOMBRE APELLIDO" + "</option>");
    // $('#firma').chosen().trigger("chosen:updated");

    AsignarFechas();
});

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