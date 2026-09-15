var datosTabla;

$(function () {
    $(".chosen").chosen({ allow_single_deselect: true });
    //carga databale por ajax y serverside procesing
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false },
                { "data": "Boton", "orderable": false },
                { "data": "Codigo", "orderable": true },
                { "data": "Descripcion", "orderable": false }
    ];
    AplicarPluginTablaAjax("tablaAjax", urlTablaAjaxPro, parametrosAjax);
    //controles de acciones en el formulario
    $("#fuerza_id").val($("#fuerza_lista").val());
    $("#fuerza_lista").change(function () {
        $("#fuerza_id").val($(this).val());
    });
    //Permite obtener el valor de un control checkbox con switch
    $('input[name="cbxOperativa"]').on('switchChange.bootstrapSwitch', function (event, state) {
        if (state) {
            $("#hdOperativa").val("S");
        } else {
            $("#hdOperativa").val("N");
        }
    });
    //Evento al presionar un checkbox on switch
    $('input[name="cbxVigente"]').on('switchChange.bootstrapSwitch', function (event, state) {
        if (state) {
            $("#hdVigente").val("S");
        } else {
            $("#hdVigente").val("N");
        }
    });
    ///eventos Click
    $("#_btnlugares").click(function () {
        CargarTablaLugares();
    });
    $("#_btnsiath").click(function () {
        CargarTablaSiath();
    });
    $("#_btnunidad").click(function () {
        console.log("pressed");
        CargarTablaUnidades();
    });
    // Button with progress
    Ladda.bind('.btn-ladda-progress', {
        callback: function (instance) {
            var progress = 0;
            var interval = setInterval(function () {
                progress = Math.min(progress + Math.random() * 0.1, 1);
                instance.setProgress(progress);

                if (progress === 1) {
                    instance.stop();
                    clearInterval(interval);
                }
            }, 500);
        }
    });
});

function IniciarEdicion() {
    if ($("#hdOperativa").val() === "S") {
        $('#cbxOperativa').bootstrapSwitch('state', true)
    } else {
        $('#cbxOperativa').bootstrapSwitch('state', false)
    }
    if ($("#hdVigente").val() === "S") {
        $('#cbxVigente').bootstrapSwitch('state', true)
    } else {
        $('#cbxVigente').bootstrapSwitch('state', false)
    }
}
//Permite inicializar los elementos del formulario de creación
function IniciarElementos() {
    //Identifica el estado actual del valor vigente y operativo
    if ($('#cbxOperativa').bootstrapSwitch('state')) {
        $("#hdOperativa").val("S");
    } else {
        $("#hdOperativa").val("N");
    }
    if ($('#cbxVigente').bootstrapSwitch('state')) {
        $("#hdVigente").val("S");
    } else {
        $("#hdVigente").val("N");
    }
}
//Cierra modales al seleccionar un registro dentro de su contenido
function cargarValor(id, opcion) {
    switch (opcion) {
        case "LUGAR":
            $("#txtLugar").val(id);
            $("#ModalLugaresAjax").modal("toggle");
            break;
        case "SIATH":
            $("#hdSIATH").val(id);
            $("#ModalSiath").modal("toggle");
            break;
        default:
            return;
    }
}

function cargarLugaresAjax() {
    $("#ModalLugaresAjax").modal();
}
function CargarTablaUnidades() {
    var parametros = [
                        { "data": "Dummy", "orderable": false },
                        { "data": "Boton", "orderable": false },
                        { "data": "CODIGO", "orderable": true },
                        { "data": "DESCRIPCION" },
                        { "data": "ExtNombrePadre" },
                        { "data": "SIGLA" },
                        { "data": "OPERATIVA" },
                        { "data": "VIGENTE" },
                        { "data": "ExtLugar" }
    ];
    AplicarPluginTablaAjax("_TablaUnidades", urlTablaUnidades, parametros);
    console.log("accede");
    $("#ModalUnidades").modal();
}

function CargarTablaSiath() {
    var parametros = [
                        { "data": "Dummy", "orderable": false },
                        { "data": "Boton", "orderable": false },
                        { "data": "Codigo", "orderable": true },
                        { "data": "Descripcion", "orderable": true },
                        { "data": "Vigente", "orderable": true }
    ];
    AplicarPluginTablaAjax("_TablaSIATH", urlTablaSiath, parametros);
    console.log("accede");
    $("#ModalSiath").modal();
}

function CargarTabla(idTabla, idModal, ruta) {
    var filas = $("#" + idTabla + " tbody tr").length;

    if (filas <= 0) {
        $.ajax({
            method: "POST",
            url: ruta,
            content: "application/json; charset=utf-8",
            dataType: "html",
            beforeSend: function () {
                console.log("cargando2");
                //$(".loader_decad").fadeIn("slow");

                $(".loader_decad").removeClass("hidden");
                $(".loader_back").removeClass("hidden");
            },
            success: function (data) {
            },
            error: function () {
                console.log("Error");
            }
        }).done(function (data) {
            console.log("Exitoso");
            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
            $("#" + idTabla + " tbody").empty();
            $("#" + idTabla + " tbody").html(data);
            $("#" + idTabla).DataTable();
            $("#" + idModal).modal();
            console.log("Genero y desplego tabla");
        });
    } else {
        $("#" + idModal).modal();
    }
}
function cargarUnidad(codigo, descripcion) {
    console.log("carga unidad" + codigo + " " + descripcion);
    $("#hf_CodigoPadre").val(codigo);
    $("#txtUnidadPadre").val(descripcion);
    $("#ModalUnidades").modal("toggle");
}