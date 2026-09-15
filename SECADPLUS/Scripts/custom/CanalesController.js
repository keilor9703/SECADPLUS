$(function () {
    ConsultarCanales();
});
//////////////////////////////////////////////////
//Permite consultar la tabla de canales  /////////
//////////////////////////////////////////////////
function ConsultarCanales() {
    var urlFinal = "";
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CODIGO", "orderable": true },
                { "data": "DESCRIPCION", "orderable": true },
                { "data": "CLASE_TURNO", "orderable": true },
                { "data": "ABREVIATURA", "orderable": true },
                { "data": "TIPO_CANAL", "orderable": true },
                { "data": "VIGENTE", "orderable": true },
                { "data": "FechaDesde", "orderable": true },
                { "data": "FechaHasta", "orderable": true },
                { "data": "DISPONIBLE", "orderable": true },
                { "data": "OBSERVACION", "orderable": true },
                { "data": "TIPO_ATENCION", "orderable": true },
    ];
    urlFinal = urlConsultarCanales.replace(/amp;/g, "");
    AplicarPluginTablaAjax("_tblCanales", (urlFinal + "?fuerzaId=" + fuerzaId), parametrosAjax);
}