var OpcionesCargarArchivos = {
    onExitoso: function () { },
    onFallido: function () { },
};
function CargarArchivos(opciones) {
    var url = GenerarUrl("Seguridad", "ArchivosUpload", "ProcesarArchivo");
    $('input:file').each(function (indice, elemento) {
        $(this).fileupload({
            url: url + $(this).attr("tipo_documento_upload_id"),
            dataType: 'json',
            done: function (e, data) {
                opciones.onExitoso(data.result);
            },
            progressall: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $('#progress .progress-bar').css(
                    'width',
                    progress + '%'
                );
            }
        }).prop('disabled', !$.support.fileInput)
        .parent().addClass($.support.fileInput ? undefined : 'disabled');
    });
}
