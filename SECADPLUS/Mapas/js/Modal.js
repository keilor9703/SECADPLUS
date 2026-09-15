function AbrirFormularioModalPanel(elemento) {
    var datos = "?" + $(elemento).closest(".ibox").find(":input").serialize();
    AbrirFormularioModal($(elemento).attr("data-area"), $(elemento).attr("data-controlador"), $(elemento).attr("data-accion"), datos, $(elemento).attr("data-tamano-modal"));
}
function AbrirFormularioModal(Area, Controlador, Accion, DatosEnviar, TamanoModal) {
    abrirModal(TamanoModal);
    if (Area == undefined || Area == "") {
        Area = $(this).attr("data-area");
    }
    if (Controlador == undefined || Controlador == "") {
        Controlador = $(this).attr("data-controlador");
    }
    if (Accion == undefined || Accion == "") {
        Accion = $(this).attr("data-accion");
    }
    $("#iframe_modal").on("load", function () {
        $(".modal-header-container").hide();
        $(".modal-body").show();
    })
    $("#iframe_modal").attr("src", GenerarUrl(Area, Controlador, Accion, DatosEnviar));
}

function Alerta(Mensaje) {
    $(".model-body-alerta").html(Mensaje);
    $("#div_modal_alerta").removeClass("modal-oculto").addClass("modal-visible");
    $(".modal-backdrop").removeClass("modal-oculto").addClass("modal-visible");
}

function AlertaError(Mensaje) {
    $(".model-body-alerta").html(Mensaje);
    $("#div_modal_alerta").removeClass("modal-oculto").addClass("modal-visible").addClass("modal-grande");
    $(".modal-backdrop").removeClass("modal-oculto").addClass("modal-visible");
}
$(".modal-dialog").draggable({
    handle: ".modal-header"
});

function abrirModal(TamanoModal) {
    $(".modal-dialog").removeClass("modal-grande").removeClass("modal-normal").removeClass("modal-pequeno");
    if (TamanoModal.toLowerCase() == "pequeno") {
        $(".modal-dialog").addClass("modal-pequeno");
    }
    if (TamanoModal.toLowerCase() == "normal") {
        $(".modal-dialog").addClass("modal-normal");
    }
    if (TamanoModal.toLowerCase() == "grande") {
        $(".modal-dialog").addClass("modal-grande");
    }
    $(".modal-header-container").show();
    $(".modal-body").hide();
    $("#div_modal").removeClass("modal-oculto").addClass("modal-visible");
    $(".modal-backdrop").removeClass("modal-oculto").addClass("modal-visible");
}

function cerrarModal() {
    $("#div_modal").removeClass("modal-visible").addClass("modal-oculto");
    $("#div_modal_alerta").removeClass("modal-visible").addClass("modal-oculto");
    $(".modal-backdrop").removeClass("modal-visible").addClass("modal-oculto");
    //window.location.reload();
    objLadda.ladda('stop');    
}
