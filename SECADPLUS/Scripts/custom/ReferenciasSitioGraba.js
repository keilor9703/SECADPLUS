var parametrosAjaxReferencias = [
    { "data": "Dummy", "orderable": false },
    { "data": "Boton", "orderable": false },
    { "data": "NOMBRE" },
    { "data": "CODIGO" },
    { "data": "DESCRIPCION" },
    { "data": "ABREVIATURA" },
];


$(document).ready(function () {
    AplicarPluginTablaAjax("tablaReferencias", (urlObtenerReferencias + "?strSitioGraba=" + SitioGrabaGlobal + "&descripcionSitio=" + descripcionSitio), parametrosAjaxReferencias);
});