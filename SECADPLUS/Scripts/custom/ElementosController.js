$(function () {
    $(".icon-selector").click(function () {
        $("#text_icono").val($(this).text());
        //console.log($(this).text());
    });
});

function EliminarElemento(ruta) {
    EliminarRegistroGeneral(ruta);
}
//Permite configurar el control de la tabla de elementos
function CargarTablaElementos() {
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false },
                { "data": "ELEMENTO_NOMBRE", "orderable": true },
                { "data": "Padre", "orderable": true },
                { "data": "ELEMENTO_RUTA", "orderable": true },
                { "data": "ELEMENTO_POSICION", "orderable": true, "width": "2%" },
                { "data": "Tipo", "orderable": true },
                { "data": "ELEMENTO_ICONO", "orderable": true },
                { "data": "Estado", "orderable": true },
                { "data": "FechaCreacion", "orderable": true }
    ];
    AplicarPluginTablaAjax("tablaElementos", urlObtenerElementos, parametrosAjax);
}

function CargarTablaPermisos() {
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false },
                { "data": "Perfil", "orderable": true },
                { "data": "Elemento", "orderable": true },
                { "data": "Permiso", "orderable": true },
                { "data": "FechaCreacion", "orderable": true },
                { "data": "Estado", "orderable": true }
    ];
    AplicarPluginTablaAjax("tablaRolPerfil", ObtenerRegistros, parametrosAjax);
}