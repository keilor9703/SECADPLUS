toastr.options = {
    "closeButton": true,
    "debug": false,
    "progressBar": true,
    "preventDuplicates": false,
    "positionClass": "toast-top-center",
    "onclick": null,
    "showDuration": "400",
    "hideDuration": "1000",
    "timeOut": "7000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut",
}
function MensajeError(Titulo, Mensaje) {
    toastr.error(Mensaje, Titulo);
}

function MensajeExitoso(Titulo, Mensaje) {
    toastr.success(Mensaje, Titulo);
}

function MensajeExitosoArchivos(Titulo, Mensaje) {
    toastr.success(Mensaje, Titulo);
    
}

function MensajeInformacion(Titulo, Mensaje) {
    toastr.info(Mensaje, Titulo);
}

function MensajeWarning(Titulo, Mensaje) {
    toastr.warning(Mensaje, Titulo);
    //window.location.reload();
}

function Confirmacion(opciones) {
    swal({
        title: opciones.Titulo,
        text: opciones.Mensaje,
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: opciones.TextoBotonAceptar,
        cancelButtonText: 'No',
        closeOnConfirm: true
    },
        function () {
            opciones.onConfirmacion()
        }
    );
}

var OpcionesConfirmacion = {
    onConfirmacion: function () { },
    Titulo: "",
    Mensaje: "",
    TextoBotonAceptar: "Aceptar",
    TextoBotonCancelar: "Cancelar",
};