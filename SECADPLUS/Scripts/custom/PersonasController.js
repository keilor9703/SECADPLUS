$(function () {
    $(".chosen-select").chosen();
    rowExample = $("#_tblPersonas tbody").html();

    var today = new Date();
    //carga databale por ajax y serverside procesing
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false },
                { "data": "Boton", "orderable": false },
                { "data": "CEDU_EMPLEADO" },
                { "data": "NOMB_EMPLEADO" },
                { "data": "NUME_PLACA", "orderable": false },
                { "data": "DIRE_RESIDENCIA", "orderable": false },
                { "data": "TELE_RESIDENCIA", "orderable": false },
                { "data": "fecha_Nacimiento", "orderable": false },
                { "data": "DESCRIPCIONSEXO", "orderable": false },
                { "data": "DESCRIPCIONGRUPOSANGUINEO", "orderable": false },
                { "data": "DESCRIPCIONESTADOCIVIL", "orderable": false },
                { "data": "DESCRIPCIONESPECIALIDAD", "orderable": false },
                { "data": "DESCRIPCIONTIPOLABORAL", "orderable": false },
                { "data": "DESCRIPCIONSECCION", "orderable": false },
                { "data": "DESCRIPCIONTIPOFUERZA", "orderable": false },
                { "data": "DESCRIPCIONUNIDADES", "orderable": false },
                { "data": "DESCRIPCIONGRADO", "orderable": false },
                { "data": "DESCRIPCIONFUERZA", "orderable": false }
    ];

    AplicarPluginTablaAjax("_tblPersonas", ValorGrado(), parametrosAjax);

    $("#GRAD_ALFABETICO").on("change", function () {
        //console.log(ValorFuerza());
        $("#_tblPersonas").DataTable().ajax.url(ValorGrado());
        $("#_tblPersonas").DataTable().ajax.reload();
    });
    $("#CADUNID_FUERZA").on("change", function () {
        $("#CADUNID_CODIGO").val("");
        $("#_UNIDAD").val("");
    });
});
function ValorGrado() {
    var gradoAlfabetico = (urlBuscarPersonas + "?gradoAlfabetico=" + $("#GRAD_ALFABETICO").val());
    //console.log(JSON.parse(fuerza));
    return gradoAlfabetico;
}

function cargarUnidades() {
    var idFuerza = $("#CADUNID_FUERZA").val();

    if (idFuerza != 0) {
        console.log(idFuerza);
        var parametrosAjax = [
            { "data": "Dummy", "orderable": false },
            { "data": "Boton", "orderable": false },
            { "data": "CODIGO" },
            { "data": "DESCRIPCION" },
        ];
        console.log(parametrosAjax);
        $("#tablaUnidades").DataTable().destroy();
        $("#tablaUnidades tbody").empty();

        AplicarPluginTablaAjax("tablaUnidades", urlTablaUnidades + "?id=" + idFuerza, parametrosAjax);
        console.log(urlTablaUnidades + "?id=" + idFuerza);
        $("#ModalUnidades").modal();
    }
}

function cargarValorUnidad(id, descripcion) {
    $("#CADUNID_CODIGO").val(id);
    $("#_UNIDAD").val(descripcion);
    $("#ModalUnidades").modal("toggle");
}

//Muestra un previo de las imagenes cargadas
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

//Carga Datos Ponal
function CargaDatosPonal() {
    if ($("#CEDU_EMPLEADO").val() != "")
        location.href = urlCargaDatos + "?strCedula=" + $("#CEDU_EMPLEADO").val();
    else {
        GestionarAlerta("warning", "No ha indicado un número de cédula");
    }
}