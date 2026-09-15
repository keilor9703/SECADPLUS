var parametrosAjax = [
        { "data": "Dummy", "orderable": false },
        { "data": "BOTON", "orderable": false },
        { "data": "CONSECUTIVO" },
        { "data": "USUARIO" },
        { "data": "SITIO_GRABA" },
        { "data": "NUMERO" },
        { "data": "FECHA" },
        { "data": "AÑO" },
        { "data": "ORIGEN" },
        { "data": "DESTINO" },
        { "data": "TEXTO" },
        { "data": "FIRMA" },
        { "data": "CARGO_FIRMA" },
        { "data": "TRANSMITE" },
        { "data": "RECIBE" }
];

AplicarPluginTablaAjax("tabla", urlTabla + "?idSitio=" + globalSitio + "&idFuerza=" + globalFuerza, parametrosAjax);