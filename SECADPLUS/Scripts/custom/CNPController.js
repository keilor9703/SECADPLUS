var glCreado = false;
$(function () {
    $(".chosen-select").chosen({ allow_single_deselect: false, width: "100%" });
    $(".styled, .multiselect-container input").uniform({
        radioClass: 'choice'
    });
    $("#slTipoDoc").change(function () {
        console.log($(this).val());
        if ($(this).val() !== "") {
            AsignarTipoDoc($(this).val());
        }
    });
    $("#txtCedula").keypress(function (e) {
        if (e.which == 13) {
            if ($("#txtCiudad").val() !== "") {
                ConsultarPersona($(this).val());
            }
        }
    });
    $("#txtComportamientoId").keypress(function (e) {
        if (e.which == 13) {
            if ($(this).val() !== "") {

                ComplementarComportamiento($(this).val());
            }
        }
    });
    $("#IDPERSONA").keypress(function (e) {
        if (e.which == 13) {
            if ($(this).val() !== "") {
                ConsultarRegistraduria($(this).val());
                ConsultarCantidadComparendos();
            }
        }
    });
    $("#txtComportamientoId").dblclick(function () {
        $("#ModalComportamientos").modal("toggle");
        ConsultarComportamientos();
    });
    $("#btnValidarComparendo").click(function () { ValidarComparendo(); });
    $("#btnGuardar").click(function () { GenerarCNP(); });
    $("#btnLimpiar").click(function () {
        $("#btnGuardar").show();
        $("#formCNP")[0].reset();
    });
    $("#btnRegistraduria").click(function () { ConsultarRegistraduria(); });
    $("#btnConsultarComparendos").click(function () { ConsultarComparendosAsociados(); });
    ConsultarComparendos();
});

function ConsultarPersona(cedula) {
    var urlFinal = urlConsultarPersona.replace(/amp;/g, "");
    if (!glCreado) {
        MostrarComparendoValidado(glCreado);
    } else {
        MostrarComparendoValidado(glCreado);
    }
    //Consulta la información solicitada
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 5000,
        data: { strCedula: cedula },
        beforeSend: function () {
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                $("#txtNombre").val(data.datos);
            } else {
                $("#txtCedula").focus();
                GestionarAlerta("warning", data.datos);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (textStatus === "timeout") {
                GestionarAlerta("warning", "Al parecer la consulta de los datos asociada a la cédula ingresada estan tardando demasiado. Proceso interrumpido.");
            }
        }
    }).done(function (data) {
    });
}
/////////////////////////////////////////////////////////////////////////
//Permite validar un comparendo contra la base de datos de secad/////////
/////////////////////////////////////////////////////////////////////////
function ValidarComparendo() {
    var cedula = $("#txtCedula").val();
    var comparendo = $("#txtComparendo").val();
    if (cedula === "" || comparendo === "") {
        GestionarAlerta("warning", "Debe ingresar un número de cédula y comparendo.");
        return false;
    }
    var urlFinal = urlValidarComparendo.replace(/amp;/g, "");
    //Consulta la información solicitada
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 5000,
        data: { strCedula: cedula, strComparendo: comparendo },
        beforeSend: function () {
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                if (data.datos === "S") {
                    MostrarComparendoValidado(true);
                } else {
                    MostrarComparendoValidado(false);
                    GestionarAlerta("warning", data.datos);
                }

            } else {
                MostrarComparendoValidado(false);
                GestionarAlerta("warning", data.datos);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (textStatus === "timeout") {
                GestionarAlerta("warning", "Al parecer la consulta de los datos asociada a la cédula ingresada estan tardando demasiado. Proceso interrumpido.");
            }
        }
    }).done(function (data) {
    });
}
/////////////////////////////////////////////////////////////////////////
//Permite presentar cuando un comparendo ha sido o no validado///////////
/////////////////////////////////////////////////////////////////////////
function MostrarComparendoValidado(opcion) {
    if (opcion) {
        $("#txtValidadoCNP").val("Validado con CNP");
        $("#checkCNP").prop("checked", true);
        $("#hdValidadoCNP").val("S");
    } else {
        $("#txtValidadoCNP").val("No validado con CNP");
        $("#checkCNP").prop("checked", false);
        $("#hdValidadoCNP").val("N");
    }
}
/////////////////////////////////////////////////////////////////////////
//Permite consultar la tabla de comparendos o complementar uno  /////////
/////////////////////////////////////////////////////////////////////////
function ConsultarComportamientos() {
    var urlFinal = "";
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "ID", "orderable": true },
                { "data": "Dominio", "orderable": true },
                { "data": "Descripcion", "orderable": true },
                { "data": "IdPadre", "orderable": false },
    ];
    $("#txtComportamientoId").val("");
    $("#txtComportamiento").val("");
    urlFinal = urlCommportamientos.replace(/amp;/g, "");
    AplicarPluginTablaAjax("tblComportamientos", urlFinal, parametrosAjax);
}
/////////////////////////////////////////////////////////////////////////
//Permite presentar la información de un comparendo consultado  /////////
/////////////////////////////////////////////////////////////////////////
function SeleccionarComportamiento(id, descripcion) {
    $("#ModalComportamientos").modal("hide");
    $("#txtComportamientoId").val(id);
    $("#txtComportamiento").val(descripcion);
}
/////////////////////////////////////////////////////////////////////////
//Permite consultar y presentar la descripción de un id de comortamiento/
/////////////////////////////////////////////////////////////////////////
function ComplementarComportamiento(id) {
    if (id === "") {
        $("#txtComportamiento").val("");
        return;
    }
    var urlFinal = urlCommportamientos.replace(/amp;/g, "");
    var datos = { strNumero: id };

    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {
            $("#mensajeComportamiento").html("Identificando el comportamiento ingresado...");
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                $("#mensajeComportamiento").html("");

                if (data.datos.CODIGO === "") {

                    $("#mensajeComportamiento").html("No existe el comportamiento ingresado...");
                    $("#txtComportamiento").val("");
                    $("#txtComportamientoId").focus();

                } else {
                    SeleccionarComportamiento(data.datos.CODIGO, data.datos.NOMBRE);
                }
            } else {
                $("#mensajeComportamiento").html("No existe el comportamiento ingresado...");
                $("#txtComportamiento").val("");
                $("#txtComportamientoId").focus();
            }
        }
    }).done(function (data) {
    });
}
/////////////////////////////////////////////////////////////////////////
//Permite asignar un tipo de documento/////////
/////////////////////////////////////////////////////////////////////////
function AsignarTipoDoc(tipo) {

    $("#hdTipoPersona").val(tipo);

}
/////////////////////////////////////////////////////////////////////////
//Permite consultar el nombre de una persona desde la base de datos /////
/////de la registraduría/////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////
function ConsultarRegistraduria() {
    //Valida que se ingresen los datos de tipo y cedula
    var tipo = $("#hdTipoPersona").val();
    var cedula = $("#IDPERSONA").val();
    if (tipo === "" || cedula === "") {
        GestionarAlerta("warning", "Hace falta seleccionar el tipo de documento o ingresarel número de la persona a consultar.");
        return false;
    }
    var urlFinal = urlConsultarRegistraduria.replace(/amp;/g, "");
    var datos = { strTipo: tipo, strDocumento: cedula };
    $("#PERSONA_REGISTRADURIA").val("Sin validación Registraduria");
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {
            $("#mensajePersona").html("Identificando el documento ingresado...");
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                $("#PERSONA_REGISTRADURIA").val(data.datos);
            } else {
                $("#PERSONA_REGISTRADURIA").val("");
                $("#IDPERSONA").focus();
                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {
        $("#mensajePersona").html("");
    });
}
////////////////////////////////////////////////////////////////////////////
//Permite consultar la cantidad de comparendos existentes para la persona///
////////////////////////////////////////////////////////////////////////////
function ConsultarCantidadComparendos() {

    //Valida que se ingresen los datos de tipo y cedula
    var tipo = $("#hdTipoPersona").val();
    var cedula = $("#IDPERSONA").val();
    var comparendo = $("#txtComparendo").val();
    if (comparendo === "") {
        GestionarAlerta("warning", "Hace falta ingresar el número de comparendo a consultar.");
        return false;
    }
    if (tipo === "" || cedula === "") {
        GestionarAlerta("warning", "Hace falta seleccionar el tipo de documento o ingresar el número de la persona a consultar.");
        return false;
    }
    var urlFinal = urlConsultarCantComparendos.replace(/amp;/g, "");
    var datos = { strComparendo: comparendo, strTipo: tipo, strDocumento: cedula };

    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: datos,
        beforeSend: function () {

        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                $("#txtCantidadComp").val(data.datos);
            } else {
                $("#txtCantidadComp").val("");

                GestionarAlerta("warning", data.datos);
            }
        }
    }).done(function (data) {

    });
}

function GenerarCNP() {
    var urlFinal = urlConsultarPersona.replace(/amp;/g, "");
    var urlFinalGuardar = $("#formCNP").attr("action");
    var datos = $("#formCNP").serialize();
    var cedula = $("#txtCedula").val();
    if (!glCreado) {
        MostrarComparendoValidado(glCreado);
    } else {
        MostrarComparendoValidado(glCreado);
    }
    //Consulta la información solicitada
    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        timeout: 5000,
        data: { strCedula: cedula },
        beforeSend: function () {
        },
        success: function (data) {
            //carga la lista con elementos
            if (data.result) {
                $("#txtNombre").val(data.datos);
                //Guarda el registro ´sólo si existe el policia
                $.ajax({
                    method: "POST",
                    url: urlFinalGuardar,
                    content: "application/json; charset=utf-8",
                    dataType: "json",
                    data: datos,
                    beforeSend: function () {
                        $(".loader_decad").removeClass("hidden");
                        $(".loader_back").removeClass("hidden");
                    },
                    success: function (data) {
                        //carga la lista con elementos
                        if (data.result) {
                            GestionarAlerta("success", data.datos);
                            $("#formCNP")[0].reset();
                        } else {

                            GestionarAlerta("warning", data.datos);
                        }
                    }
                }).done(function (data) {
                    $(".loader_decad").addClass("hidden");
                    $(".loader_back").addClass("hidden");

                });
            } else {
                $("#txtCedula").focus();
                GestionarAlerta("warning", data.datos);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (textStatus === "timeout") {
                GestionarAlerta("warning", "Al parecer la consulta de los datos asociada a la cédula ingresada estan tardando demasiado. Proceso interrumpido.");
            }
        }
    }).done(function (data) {
    });
    
    
}
/////////////////////////////////////////////////////////////////////////
//Permite consultar la tabla de comparendos existentes  /////////
/////////////////////////////////////////////////////////////////////////
function ConsultarComparendos() {
    var urlFinal = "";
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "Cedula", "orderable": true },
                { "data": "Policia", "orderable": true },
                { "data": "Comparendo", "orderable": true },
                { "data": "Fecha", "orderable": true },
                { "data": "Validado", "orderable": false },
                { "data": "Evento", "orderable": false },
    ];
    urlFinal = urlConsultarComparendos.replace(/amp;/g, "");
    AplicarPluginTablaAjax("tblComparendos", (urlFinal + "?dcNumEvento=" + dcNumEvento), parametrosAjax);
}
/////////////////////////////////////////////////////////////////////////
//Permite consultar la tabla de comparendos asociados  /////////
/////////////////////////////////////////////////////////////////////////
function ConsultarComparendosAsociados() {
    //Valida que se ingresen los datos de tipo y cedula
    var tipo = $("#hdTipoPersona").val();
    var cedula = $("#IDPERSONA").val();
    var comparendo = $("#txtComparendo").val();
    if (comparendo === "") {
        GestionarAlerta("warning", "Hace falta ingresar el número de comparendo a consultar.");
        return false;
    }
    if (tipo === "" || cedula === "") {
        GestionarAlerta("warning", "Hace falta seleccionar el tipo de documento o ingresar el número de la persona a consultar.");
        return false;
    }
    var urlFinal = "";
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Comparendo", "orderable": false },
                { "data": "IdComportamiento", "orderable": false },
                { "data": "Comportamiento", "orderable": true },
                { "data": "Fecha", "orderable": true },
                { "data": "Apellidos", "orderable": true },
                { "data": "Nombres", "orderable": false },

    ];
    $("#tabcomparendos").trigger("click");
    urlFinal = urlConsultarAsociados.replace(/amp;/g, "") + "?strComparendo=" + comparendo + "&strTipo=" + tipo + "&strCedula=" + cedula;
    AplicarPluginTablaAjax("tblAsociados", urlFinal, parametrosAjax);

}

function SeleccionarRegistro(cedula, policia, comparendo, valida, id, comportamiento, tipo, identificacion, nombres, apellidos, usuario, fecha, maquina, persona) {
    AsignarTipoDoc(tipo);
    $("#txtCedula").val(cedula);
    $("#txtNombre").val(policia);
    $("#txtComparendo").val(comparendo);
    $("#hdValidadoCNP").val(valida);
    $("#txtComportamientoId").val(id);
    $("#txtComportamiento").val(comportamiento);
    $("#slTipoDoc").val(tipo);
    $("#slTipoDoc").trigger("chosen:updated");
    $("#IDPERSONA").val(identificacion);
    $("#PERSONA_REGISTRADURIA").val(persona);
    $("#NOMBRE_PERSONA").val(nombres);
    $("#APELLIDOS_PERSONA").val(apellidos);
    $("#USUA_GRABA").val(usuario);
    $("#FECHA_GRABACION").val(fecha);
    $("#MAQUINA_GRABACION").val(maquina);
    $("#btnGuardar").hide();
    ConsultarCantidadComparendos();
}