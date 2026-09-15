$(function () {
    $(".chosen-select").chosen();

    $("#CADUNDE_FUERZA").on("change", function () {
        seleccionado = $("#CADUNDE_FUERZA").val();
        var entityData = {
            "idFuerza": seleccionado
        };
        $.ajax({
            type: "POST",
            url: urlActionBuscaUnidades,
            content: "application/json; charset=utf-8",
            cache: false,
            data: entityData,
            dataType: "json",
            beforeSend: function () {
                $(".loader_decad").removeClass("hidden");
                $(".loader_back").removeClass("hidden");
            },
            success: function (d) {
                $("#CADUNDE_CODIGO_PPAL").empty();
                $("#CADUNDE_CODIGO").empty();
                $("#CADUNDE_CODIGO_PPAL").append("<option value='" + -1 + "'>" + "" + "</option>");
                $("#CADUNDE_CODIGO").append("<option value='" + -1 + "selected'>" + "" + "</option>");
                $.each(d, function (i, item) {
                    $("#CADUNDE_CODIGO_PPAL").append("<option value='" + item.codigo + "'>" + item.descripcion + "</option>");
                })
                $("#CADUNDE_CODIGO_PPAL").chosen("destroy");
                $("#CADUNDE_CODIGO_PPAL").chosen();
                $(".loader_decad").addClass("hidden");
                $(".loader_back").addClass("hidden");
            },
            error: function (xhr, textStatus, errorThrown) {
                // TODO: Show error
            }
        });
        $("#CADUNDE_CODIGO").val("");
        $("#_UNIDAD").val("");
    });

    $("#CADUNDE_CODIGO_PPAL").on("change", function () {
        $("#CADUNDE_CODIGO").val("");
        $("#_UNIDAD").val("");
    });
    $(document).ready(function () {
        if ($("#resultadosActualizacion").val() != "")
            $("#mostrarmodal").modal("show");
    });
});

function cargarUnidades() {
    if ($("#CADUNDE_CODIGO_PPAL").val() != "") {
        var idUnidadPpal = $("#CADUNDE_CODIGO_PPAL").val();
        var idFuerza = $("#CADUNDE_FUERZA").val();
        if (idFuerza != 0) {
            console.log(idUnidadPpal);
            var parametrosAjax = [
                { "data": "Dummy", "orderable": false },
                { "data": "Boton", "orderable": false },
                { "data": "CODIGO" },
                { "data": "DESCRIPCION" },
            ];
            console.log(parametrosAjax);
            $("#tablaUnidades").DataTable().destroy();
            $("#tablaUnidades tbody").empty();

            AplicarPluginTablaAjax("tablaUnidades", urlTablaUnidades + "?id=" + idFuerza + "&intIdUnidadPrincipal=" + idUnidadPpal, parametrosAjax);
            //console.log(urlTablaUnidades + "?id=" + idFuerza);
            $("#ModalUnidades").modal();
        }
    } else {
        GestionarAlerta("warning", "Debe seleccionar una Unidad Principal");
    }
}

function cargarValorUnidad(id, descripcion) {
    $("#_UNIDAD").val(descripcion);
    $("#CADUNDE_CODIGO").val(id);
    //$("#CADUNDE_CODIGO").empty();
    //$("#CADUNDE_CODIGO").append($('<option></option>').val(id).html(descripcion));
    $("#ModalUnidades").modal("toggle");
}

//Mustra un previo de las imagenes cargadas
function MuestraImagen(fileInput) {
    if (isIE() === 9) {
        GestionarAlerta("warning", "Para Internet Explorer 9 no esta habilitada la vista previa de la imagen cargada.");
    } else {
        var files = fileInput.files;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var imageType = /image.*/;
            if (!file.type.match(imageType)) {
                continue;
            }
            var img = document.getElementById("thumbnil");
            img.file = file;
            var reader = new FileReader();
            reader.onload = (function (aImg) {
                return function (e) {
                    aImg.src = e.target.result;
                };
            })(img);
            reader.readAsDataURL(file);
        }
    }
}

//Verifica si el navegador es internet explorer e indica la versión
function isIE() {
    var myNav = navigator.userAgent.toLowerCase();
    return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : false;
}