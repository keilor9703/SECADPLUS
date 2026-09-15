$(document).ready(function () {
    CargarCuadrantes();

});

function CargarCuadrantes() {
    var urlFinal = "";
    urlFinal = UrlConsultarCuadrantes.replace(/amp;/g, "");
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Sitio", "orderable": true},
                { "data": "Consecutivo", "orderable": true },
                { "data": "FuerzaUnidad", "orderable": true },
                { "data": "CodigoUnidad", "orderable": true },
                { "data": "NroCuadrante", "orderable": true },
                { "data": "CapaCuadrante", "orderable": true },
                { "data": "Ambito", "orderable": true },
                { "data": "Vigente", "orderable": true },
                { "data": "FechaDesde", "orderable": true },
                { "data": "FechaHasta", "orderable": true },
                { "data": "Descripcion", "orderable": true },
                { "data": "UsuarioTrans", "orderable": true },
                { "data": "NivelPeligrosidad", "orderable": true },
                { "data": "CentroideX", "orderable": true },
                { "data": "CentroideY", "orderable": true },
                { "data": "CAD1CUADSITGR_DIJIN", "orderable": true },
                { "data": "CAD1CUADCONS_DIJIN", "orderable": true },
                { "data": "CAD1CUADSITGR_DIPOL", "orderable": true },
                { "data": "CAD1CUADCONS_DIPOL", "orderable": true },
                { "data": "SiglaDpto", "orderable": true },
                { "data": "MotivoCierre", "orderable": true }
    ];
    AplicarPluginTablaAjax("tblCuadrantes", urlFinal, parametrosAjax);
}