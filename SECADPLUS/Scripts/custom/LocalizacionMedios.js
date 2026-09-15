var parametrosAjax = [
   { "data": "Dummy", "orderable": false, "width": "0.5%" },
   { "data": "DESC_PATRULLA" },
   { "data": "FECHA" },
   { "data": "DIRE_UBICACION" },
   { "data": "MUNICIPIO" },
   { "data": "X_COORD_UBICA" },
   { "data": "Y_COORD_UBICA" },
   { "data": "VIGENTE" },
];

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

var parametrosAjaxCiudad = [
      { "data": "Dummy", "orderable": false },
      { "data": "Boton", "orderable": false },
      { "data": "CODIGO" },
      { "data": "DESCRIPCION" },
];

$(document).ready(function () {
    AplicarPluginTablaAjax("_tblLocalizacionMedios", urlTabla, parametrosAjax);
});

function CargarMedio(codigo, descripcion) {
    console.log(codigo);
    $("#txt_desPatrulla").empty();
    $("#txt_desPatrulla").val(descripcion);

    $("#CADMEDISP_CONSECUTIVO").empty();
    $("#CADMEDISP_CONSECUTIVO").val(codigo);

    $("#ModalMedios").modal("toggle");
}

function CargaMedios() {
    if ($.fn.DataTable.isDataTable("#_tblMedios")) {
        $("#_tblMedios").DataTable().destroy();
        $("#_tblMedios tbody").empty();
    }

    AplicarPluginTablaAjax("_tblMedios", urlMedios, parametrosAjaxMedios);
    $("#ModalMedios").modal();
}

//Metodo que carga ciudades
function CargaCiudades() {
    $("#tablaCiudades").DataTable().destroy();
    $("#tablaCiudades tbody").empty();

    AplicarPluginTablaAjax("tablaCiudades", urlCiudades, parametrosAjaxCiudad);
    $("#ModalCiudades").modal();
}

//Metodo que carga una ciudad
function CargarCiudad(codigo, descripcion) {
    $("#DESCR_LUGE").empty();
    $("#DESCR_LUGE").val(descripcion);

    $("#ModalCiudades").modal("toggle");
}

function BuscarLocalizacionMedios() {
    $("#_tblLocalizacionMedios").DataTable().destroy();
    $("#_tblLocalizacionMedios tbody").empty();

    AplicarPluginTablaAjax("_tblLocalizacionMedios", valorFiltros(), parametrosAjax);
}
function valorFiltros() {
    var filtros = (urlTabla + "?strMedios=" + $("#txt_desPatrulla").val() + "&strCiudad=" + $("#DESCR_LUGE").val());
    return filtros;
}