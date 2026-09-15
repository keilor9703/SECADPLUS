var glLimite = 2097152;

$(function () {
    $("#btnGuardar").click(function () {
        var actual = 0;
        console.log(document.getElementById('uploader').files.Count);
        if (document.getElementById('uploader').files.Count > 0) {
            actual = document.getElementById('uploader').files[0].size;
        }

        if (actual >= glLimite) {
            GestionarAlerta("warning", "La imagen seleccionada excede el peso limite para cargue de imagenes de 2MB. Por favor reemplaze la imagen seleccionada.");
            return;
        }
        $("#frm_documentos").submit();
    });
});

function MuestraImagen(fileInput) {
    var actual = 0;
    if (document.getElementById('uploader').files.Count > 0) {
        actual = document.getElementById('uploader').files[0].size;
    }
    if (actual >= glLimite) {
        GestionarAlerta("warning", "La imagen seleccionada excede el peso limite para cargue de imagenes de 2MB. Por favor reemplaze la imagen seleccionada.");
    }
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
            var img = document.getElementById("imagen");
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
//Carga los documnentos asociados a un caso
function CargarTablaPedidos() {
    var urlFinal = "";
    var localParams = { strSitio: glSitio, strLlamada: glNumeroLlamada };
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblDocumentos")) {
        $("#tblDocumentos").DataTable().destroy();
        $("#tblDocumentos tbody").empty();
    }
    urlFinal = urlConsultarDocumentos.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    //console.log(urlRadios);
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CONSECUTIVO", "orderable": true },
                { "data": "FechaGrabacion", "orderable": true },
                { "data": "FechaModifica", "orderable": true },
                { "data": "USUA_MODIFICA", "orderable": true },
                { "data": "RUTA", "orderable": true },
                { "data": "NOMBRE_ARCHIVO", "orderable": true },
                { "data": "BotonImagen", "orderable": true }
    ];
    AplicarPluginTablaParams("tblDocumentos", urlFinal, parametrosAjax, localParams);
}

function EditarDocumento(consecutivo) {
    //consulta el registro
    //Consulta si existe una nueva llamada
    var urlFinal = urlConsultarDocumento.replace(/amp;/g, "");

    var datos = { intSitio: glSitio, lgNumeroLlamada: glNumeroLlamada, srConsecutivo: consecutivo };
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {
            //GestionarAlerta("info", "Consultando llamadas...");
        },
        success: function (data) {
            //carga la lista con elementos
            console.log(data);
            if (data.result) {
                //carga los daot spara la edición
                $("#CONSECUTIVO").val(consecutivo);
                $("#RUTA").val(data.datos.RUTA);
                $("#FECHA_GRABACION").val(data.datos.FECHA_GRABACION);
                $("#NOMBRE_ARCHIVO").val(data.datos.NOMBRE_ARCHIVO);
                $("#imagen").removeAttr("src");
                $("#imagen").attr("src", "" + data.imagen);
            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log("conexion caida, no se ha podido obtener la información del documento");
    });
}