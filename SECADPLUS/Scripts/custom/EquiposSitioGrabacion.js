$(".chosen-select").chosen();
//AplicarPluginTabla("tablaEquipos");
AplicarPluginTabla("tablaConexiones");

var parametrosAjax = [
        { "data": "Dummy", "orderable": false },
        { "data": "Boton", "orderable": false },
        { "data": "SITIO_GRABA" },
        { "data": "IDENTIFICACION" },
        { "data": "DESCRIPCION" },
        { "data": "ACD" },
        { "data": "TNSNAMES60" },
        { "data": "UBICACION" },
        { "data": "SERIE" }
];

AplicarPluginTablaAjax("tablaEquipos", urlTabla, parametrosAjax);

function VerConexiones(codSitio, id) {
    $.ajax({
        type: 'POST',
        //url: 'GetConexionesEquipos',
        //url: '/EquiposSitioGrabacion/GetConexionesEquipos',
        url: urlConexiones,
        dataType: 'html',
        data: { codSitio: codSitio, id: id }
    }).done(function (conexiones) {
        if ($.fn.DataTable.isDataTable("#tablaConexiones")) {
            $("#tablaConexiones").DataTable().destroy();
        }
        console.log("entro");
        $("#tablaConexiones tbody").empty();
        $("#tablaConexiones tbody").html(conexiones);
        AplicarPluginTabla("tablaConexiones");
    });

    //$("#tablaConexiones").load("GetConexionesEquipos", { codSitio: codSitio, id: id });
};

function ActualizarVigente(codSitio, id, element) {
    $.ajax({
        type: 'POST',
        //url: '@Url.Action("EditaConexionVigente")',
        //url: '/EquiposSitioGrabacion/EditaConexionVigente',
        url: urlVigente,
        dataType: 'json',
        data: { codSitio: codSitio, id: id, checkVigente: element.checked },
        success: function (estado) {
            if (estado.result) {
                GestionarAlerta("success", estado.datos);
            } else {
                GestionarAlerta("warning", estado.datos);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
};