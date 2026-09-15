$(document).ready(function () {
    $("#MARCA_CODIGO").change(function () {
        $("#LINEA_CODIGO").empty();
        $.ajax({
            type: 'POST',
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
    $("#TIIN_CODIGO").change(function () {
        $("#CLIN_CODIGO").empty();
        $.ajax({
            type: 'POST',
            url: urlClaseNovedad,
            dataType: 'json',
            data: { id: $("#TIIN_CODIGO").val() },
            success: function (clasesNovedad) {
                $.each(clasesNovedad, function (i, clase) {
                    $("#CLIN_CODIGO").append($('<option></option>').val(clase.CODIGO).html(clase.DESCRIPCION));
                });
                $("#CLIN_CODIGO").chosen("destroy");
                $("#CLIN_CODIGO").chosen();
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });
        return false;
    })
});

//Metodo que carga ciudades
function CargaCiudades() {
    var parametrosAjaxCiudad = [
        { "data": "Dummy", "orderable": false },
        { "data": "Boton", "orderable": false },
        { "data": "CODIGO" },
        { "data": "DESCRIPCION" },
    ];

    $("#tablaCiudades").DataTable().destroy();
    $("#tablaCiudades tbody").empty();

    AplicarPluginTablaAjax("tablaCiudades", (urlActionCargaCiudades), parametrosAjaxCiudad);
    $("#ModalCiudades").modal();
}
//Metodo que carga barrios
function CargaBarrios() {
    var parametrosAjaxBarrio = [
       { "data": "Dummy", "orderable": false },
       { "data": "Boton", "orderable": false },
       { "data": "CODIGOBARRIO" },
       { "data": "DESCRIPCIONBARRIO" },
    ];

    $("#_tblBarrios").DataTable().destroy();
    $("#_tblBarrios tbody").empty();
    AplicarPluginTablaAjax("_tblBarrios", ValorCiudad(), parametrosAjaxBarrio);
    //  $("#ModalBarrios").modal("toggle");
}
//Metodo que carga valores de ciudad
function ValorCiudad() {
    var urlCiudad = (urlsCargaBarrios + "?lugecodigo=" + $("#LUGE_CODIGO").val());
    return urlCiudad;
}
//Metodo que carga una ciudad
function CargarCiudad(codigo, descripcion) {
    $("#LUGE_CODIGO").empty();
    $("#LUGE_CODIGO").val(codigo);

    $("#LUGE_BARRIO").empty();
    $("#LUGE_BARRIO").val(codigo);

    $("#DESCR_LUGE").empty();
    $("#DESCR_LUGE").val(descripcion);

    //limpiar barrios
    $("#CODI_BARRIO").empty();
    $("#CODI_BARRIO").val('');

    $("#DESCR_BARRIO").empty();
    $("#DESCR_BARRIO").val('');

    $("#ModalCiudades").modal("toggle");
    CargaBarrios();
}
//Metodo que muestra barrios
function MostaraBarrio() {
    if ($("#LUGE_CODIGO").val() != "") {
        CargaBarrios();
    }

    $("#ModalBarrios").modal();
}
//Metodo que carga un barrio
function CargarBarrio(codigo, descripcion) {
    $("#CODI_BARRIO").empty();
    $("#CODI_BARRIO").val(codigo);

    $("#DESCR_BARRIO").empty();
    $("#DESCR_BARRIO").val(descripcion);

    $("#ModalBarrios").modal("toggle");
}
//Metodo que carga infracciones
function CargarInfracciones() {
    var parametrosAjaxInfraccion = [
        { "data": "Dummy", "orderable": false },
        { "data": "Boton", "orderable": false },
        { "data": "CODIGO" },
        { "data": "DESCRIPCION" },
        { "data": "INMOVILIZACION" },
        { "data": "NRO_SALARIOS" },
        { "data": "VIGENTE" },
    ];

    $("#_tblInfracciones").DataTable().destroy();
    $("#_tblInfracciones tbody").empty();

    AplicarPluginTablaAjax("_tblInfracciones", urlsInfracciones, parametrosAjaxInfraccion);
    $("#ModalInfracciones").modal();
}
//Metodo que carga una infraccion
function CargarInfraccion(codigo, descripcion) {
    $("#INFR_CODIGO").empty();
    $("#INFR_CODIGO").val(codigo);

    $("#INFR_DESCRIPCION").empty();
    $("#INFR_DESCRIPCION").val(descripcion);

    $("#ModalInfracciones").modal("toggle");
}
//Metodo que carga parqueaderos
function CargarParqueaderos() {
    var parametrosAjaxParqueaderos = [
        { "data": "Dummy", "orderable": false },
        { "data": "Boton", "orderable": false },
        { "data": "CODIGO" },
        { "data": "DESCRIPCION" },
        { "data": "SITIO_GRABA" },
        { "data": "NOMBRECIUDAD" },

    ];

    $("#_tblParqueaderos").DataTable().destroy();
    $("#_tblParqueaderos tbody").empty();

    AplicarPluginTablaAjax("_tblParqueaderos", urlsParqueaderos, parametrosAjaxParqueaderos);
    $("#ModalParqueaderos").modal();
}
//Metodo que carga un parqueadero
function CargarParqueadero(codigo, descripcion, sitioGraba) {
    $("#PARQ_CODIGO").empty();
    $("#PARQ_CODIGO").val(codigo);

    $("#DESCR_PARQUEADERO").empty();
    $("#DESCR_PARQUEADERO").val(descripcion);

    $("#PARQ_SITIOGRABA").empty();
    $("#PARQ_SITIOGRABA").val(sitioGraba);

    $("#ModalParqueaderos").modal("toggle");
}
//Metodo que carga unidades
function CargarUnidades() {
    var parametrosAjaxUnidades = [
        { "data": "Dummy", "orderable": false },
        { "data": "Boton", "orderable": false },
        { "data": "COD_UNIDAD" },
        { "data": "DESC_UNIDAD" },
        { "data": "FUERZA" },

    ];

    $("#_tblUnidades").DataTable().destroy();
    $("#_tblUnidades tbody").empty();

    AplicarPluginTablaAjax("_tblUnidades", urlsUnidades, parametrosAjaxUnidades);
    $("#ModalUnidades").modal();
}
//Metodo que carga una unidad
function CargarUnidad(codigo, descripcion, desUnidad) {
    $("#CADUNITUR_UNDEFUERZA").empty();
    $("#CADUNITUR_UNDEFUERZA").val(codigo);

    $("#CADUNITUR_UNDECODIGO").empty();
    $("#CADUNITUR_UNDECODIGO").val(descripcion);

    $("#DESCR_UNIDAD").empty();
    $("#DESCR_UNIDAD").val(desUnidad);

    //limpiar medios

    $("#MEDISP_SITIOGRABA").empty();
    $("#MEDISP_SITIOGRABA").val('');

    $("#MEDISP_CONSECUTIVO").empty();
    $("#MEDISP_CONSECUTIVO").val('');

    $("#DESCR_PATRULLA").empty();
    $("#DESCR_PATRULLA").val('');

    $("#ModalUnidades").modal("toggle");
}
//Metodo que carga medios
function CargarMedios() {
    if ($('#CADUNITUR_UNDEFUERZA').val() != "") {
        var parametrosAjaxMedios = [
       { "data": "Dummy", "orderable": false },
       { "data": "Boton", "orderable": false },
       { "data": "ESTADO" },
       { "data": "TIPO" },
       { "data": "DESC_PATRULLA" },
       { "data": "ESTADO_PATRULLA" },
       { "data": "CUENTA" },
       { "data": "personal" },
        ];

        $("#_tblMedios").DataTable().destroy();
        $("#_tblMedios tbody").empty();

        AplicarPluginTablaAjax("_tblMedios", (urlsMedios + "?dcmFuerzaUndeUnidad=" + $('#CADUNITUR_UNDEFUERZA').val() + "&dcmCodigoUndeUnidad=" + $('#CADUNITUR_UNDECODIGO').val()), parametrosAjaxMedios);
    }

    $("#ModalMedios").modal();
}
//Metodo que carga un medio
function CargarMedio(sitioGraba, consecutivo, descPatrulla) {
    $("#MEDISP_SITIOGRABA").empty();
    $("#MEDISP_SITIOGRABA").val(sitioGraba);

    $("#MEDISP_CONSECUTIVO").empty();
    $("#MEDISP_CONSECUTIVO").val(consecutivo);

    $("#DESCR_PATRULLA").empty();
    $("#DESCR_PATRULLA").val(descPatrulla);

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
        showClear: true,
    });
}

function CargarConsulta() {
    var placa = $('#PLACA').val();

    location.href = (urlsConsulta + "?strPlaca=" + placa);
}