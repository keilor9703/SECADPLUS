$(function () {
    $(".chosen-select").chosen();
    AplicarPluginTabla("tblBanner");
});

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