var globalControlMunicipios = 0;//variable que controla el tipo de nivel a aplicar en la consulta de municipios
$(function () {
    $(".styled, .multiselect-container input").uniform({
        radioClass: 'choice'
    });    
    $("input[name='nivel']").change(function () {
        globalControlMunicipios = $(this).val();
    });
    var parametrosAjax = [
            { "data": "Dummy", "orderable": false, "width": "0.5%" },
            { "data": "NUME_TELEFONO" },
            { "data": "NOMB_PROPIETARIO" },
            { "data": "NIT" },
            { "data": "DIRE_PROPIETA" },
            { "data": "BARRIO" },
            { "data": "CADLUGE_CODIGO" },
            { "data": "CADLUGE" },
            { "data": "BARR_CODIGO" },
            { "data": "BARRIO_ASIGNADO" },
    ];
    AplicarPluginTablaAjax("tablaDirectorio", valorFiltros(), parametrosAjax);
    $("#btnConsultar").click(function () {
        $("#tablaDirectorio").DataTable().ajax.url(valorFiltros());
        $("#tablaDirectorio").DataTable().ajax.reload();
    });
});

function valorFiltros() {
    var cadenaFiltros = "telefono=" + $("#telefono").val() + ';' + "propietario=" + $("#propietario").val() + ';' +
                        "nit=" + $("#nit").val() + ';' + "direccion=" + $("#direccion").val() + ';' +
                         "barrio=" + $("#barrio").val() + ';' + "codigoMunicipio=" + $("#codigoMunicipio").val() + ';' +
                         "municipio=" + $("#municipio").val() + ';' + "codigoBarrAntencion=" + $("#codigoBarrAntencion").val() + ';' +
                         "barrAtencion=" + $("#barrAtencion").val();

    var filtros = (urlTabla + "?cadenaFiltros=" + cadenaFiltros);

    return filtros;
}

function CargaCiudades() {
    var urlFinal = urlActionCargaCiudades.replace(/amp;/g, "") + "?intLugar=" + globalControlMunicipios;
    var parametrosAjaxCiudad = [
        { "data": "Dummy", "orderable": false },
        { "data": "Boton", "orderable": false },
        { "data": "CODIGO" },
        { "data": "DESCRIPCION" },
    ];

    $("#tablaCiudades").DataTable().destroy();
    $("#tablaCiudades tbody").empty();

    AplicarPluginTablaAjax("tablaCiudades", urlFinal, parametrosAjaxCiudad);
    $("#ModalCiudades").modal();
}

function CargarCiudad(codigo, descripcion) {
    $("#BARR_LUGE_CODIGO").empty();
    $("#BARR_LUGE_CODIGO").val(codigo);

    $("#codigoMunicipio").empty();
    $("#codigoMunicipio").val(codigo);

    $("#municipio").val(descripcion);
    CargaBarrios();
    $("#ModalCiudades").modal("toggle");
}

function CargarBarrio(codigo, descripcion) {
    $("#codigoBarrAntencion").empty();
    $("#codigoBarrAntencion").val(codigo);

    $("#barrAtencion").empty();
    $("#barrAtencion").val(descripcion);

    $("#ModalBarrios").modal("toggle");
}

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
}

function ValorCiudad() {
    var urlCiudad = (urlsCargaBarrios + "?lugecodigo=" + $("#BARR_LUGE_CODIGO").val());

    return urlCiudad;
}

function MostrarBarrios() {
    $("#ModalBarrios").modal();
}