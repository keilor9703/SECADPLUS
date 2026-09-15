var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.5%" },
        { "data": "Boton", "orderable": false, "width": "20%" },
        { "data": "USUARIO" },
        { "data": "CONSECUTIVO" },
        { "data": "SITIO_GRABA" },
        { "data": "FECHA_GRABACION" },
        { "data": "ANOTACION" },
        { "data": "MAQUINA", "width": "0.5%" },

];

console.log("Entro");

AplicarPluginTablaAjax("tablaAnotaciones", urlTabla, parametrosAjax);