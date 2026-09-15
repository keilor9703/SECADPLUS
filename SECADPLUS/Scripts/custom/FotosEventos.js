var glLimite = 2097152;

var parametrosAjax = [
        { "data": "Dummy", "orderable": false, "width": "0.5%" },
        { "data": "Boton", "orderable": false },
        { "data": "CADEVEN_NUMEEVENTO" },
        { "data": "CONSECUTIVO" },
        { "data": "OBSERVACION" },
        { "data": "FOTO", "orderable": false },
];

$(document).ready(function () {

    $("#btnGuardar").click(function () {

        var actual = 0;
        if (document.getElementById('uploader').files.length > 0) {
            actual = document.getElementById('uploader').files[0].size;
        }
        if (actual >= glLimite) {
            GestionarAlerta("warning", "La imagen seleccionada excede el peso limite para cargue de imagenes de 2MB. Por favor reemplaze la imagen seleccionada.");
            return;
        }
        $("#frm_Fotos").submit();
    });

    //$("#_tblFotosEvento").DataTable().destroy();
    //$("#_tblFotosEvento tbody").empty();
    AplicarPluginTablaAjax("_tblFotosEvento", valorFiltros(), parametrosAjax);
});


function MuestraImagen(fileInput) {

    if (fileInput.files.length > 0) {
        actual = fileInput.files[0].size;
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

function EditaFotosEvento(sitioGraba, numeroEvento, consecutivo) {
    $.ajax({
        method: "POST",
        url: urlEditFoto,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { sitioGraba: sitioGraba, numeroEvento: numeroEvento, consecutivo: consecutivo },
        beforeSend: function () {
            //GestionarAlerta("info", "Consultando llamadas...");
        },
        success: function (resultados) {
            //carga la lista con elementos
            // console.log(data);
            if (resultados.result) {
                console.log(resultados.numeroEvento + "-" + resultados.consecutivo + "-" + resultados.observaciones + "-" + resultados.sitioGraba + "-" + resultados.usuario);
                //carga los daot spara la edición
                $("#CADEVEN_NUMEEVENTO").val(resultados.numeroEvento);
                $("#CONSECUTIVO").val(resultados.consecutivo);
                $("#OBSERVACION").val(resultados.observaciones);
                $("#CADEVEN_SITIOGRABA").val(resultados.sitioGraba);

                //$("#FECHA_GRABACION").val(resultados.fecha);
                $("#USUA_GRABA").val(resultados.usuario);

                //  $("#FOTO").val(resultados.foto);

                $("#thumbnil").removeAttr("src");
                $("#thumbnil").attr("src", "" + resultados.ruta);
                // MuestraImagen(resultados.ruta);
            } else {
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log("conexion caida, no se ha podido obtener la información del documento");
    });
}

//Permite realizar la eliminación de un registro especifíco
function EliminarFotoEvento(sitioGraba, numeroEvento, consecutivo) {
    var datos = { sitioGraba: sitioGraba, numeroEvento: numeroEvento, consecutivo: consecutivo };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Consecutivo: " + consecutivo,
        type: "warning",
        html: true,
        showCancelButton: true,
        confirmButtonColor: "#EF5350",
        cancelButtonColor: "#EF5385",
        confirmButtonText: "Eliminar",
        cancelButtonText: "Cancelar",
        closeOnConfirm: true,
        closeOnCancel: true
    },
    function (isConfirm) {
        if (isConfirm) {
            $.ajax({
                type: "POST",
                url: urlEliminarFoto,
                content: "application/json; charset=utf-8",
                dataType: "json",
                data: datos,
                beforeSend: function () {
                    $(".loader_decad").removeClass("hidden");
                    $(".loader_back").removeClass("hidden");
                },
                error: function (msn) {
                    GestionarAlerta("warning", msn);
                }
            }).done(function (resultado) {
                $(".loader_decad").addClass("hidden");
                $(".loader_back").addClass("hidden");
                if (resultado.result) {
                    GestionarAlerta("success", resultado.datos);
                    $('#CONSECUTIVO').val(resultado.consecutivo);

                    $("#_tblFotosEvento").DataTable().destroy();
                    $("#_tblFotosEvento tbody").empty();
                    AplicarPluginTablaAjax("_tblFotosEvento", valorFiltros(), parametrosAjax);
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}



function valorFiltros() {
    var strCadEvenNumeroEvento = $('#NumeroEvento').val();//"216861";
    var strSitioGraba = $('#SitioGraba').val();//"216861";

    $("#NUMERO_EVENTO").val(strCadEvenNumeroEvento);
    $("#CADEVEN_SITIOGRABA").val(strSitioGraba);
    //   console.log($("#NUMERO_EVENTO").val() + "-" + $("#CADEVEN_SITIOGRABA").val())
    var filtros = (urlTabla + "?numeroEvento=" + $("#NUMERO_EVENTO").val() + "&strSitioGraba=" + $("#CADEVEN_SITIOGRABA").val());

    return filtros;
}