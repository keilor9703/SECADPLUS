
function ConsultarSitios(opcion, consulta, sitio) {
    console.log("sitio: "+sitio);
    var sitioId = sitio;    
    if ($.fn.DataTable.isDataTable("#tblSitiosGrabacion")) {
        $("#tblSitiosGrabacion").DataTable().destroy();
        $("#tblSitiosGrabacion tbody").empty();
    }
    if (sitioId === null) {
        sitioId = 0;
    }
    console.log("sitio2: " + sitioId);
    var urlFinal = UrlConsultarSitiosGrabacion.replace(/amp;/g, "") + "?intSitio=" + sitioId + "&strOpcion=" + opcion + "&strConsulta=" + consulta;
    console.log("urlFinal: " + urlFinal);
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CONSECUTIVO", "orderable": true },
                { "data": "DESCRIPCION", "orderable": true },
                { "data": "Departamento", "orderable": true },
                { "data": "Ciudad", "orderable": true },
                { "data": "Fuerza", "orderable": true },
                { "data": "CADUNDE_CODIGO", "orderable": true },
                { "data": "Unidad", "orderable": true },
                { "data": "FechaInstalacion", "orderable": true },
                { "data": "OBSERVACIONES", "orderable": true },
    ];
    AplicarPluginTablaAjax("tblSitiosGrabacion", urlFinal, parametrosAjax);
}