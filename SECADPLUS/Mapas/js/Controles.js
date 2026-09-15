/// Botones 
var objLadda;
$(document).ready(function () {
    $('input[type="text"]').keypress(function (evento) {
        if (evento.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });
    /// Boton de guardar operación
    $(".boton-guardar").click(function () {
        var panel = $(this).closest(".ibox").find(":input");
        var boton = $(this);
        panel.validate();
        if (panel.valid()) {
            DatosAjax({
                onExitoso: function (datosRetornados) {
                    console.log(boton);
                    console.log($(boton).attr("data-limpiar-formulario"));
                    if ($(boton).attr("data-limpiar-formulario") == true || $(boton).attr("data-limpiar-formulario") == "True") {
                        $(boton).closest("form")[0].reset();
                        $(".lista-select2").select2("val", "0");
                    }
                },
                onFallido: function (datosRetornados) { },
                Area: $(this).attr("data-area"),
                Controlador: $(this).attr("data-controlador"),
                Accion: $(this).attr("data-accion"),
                Id: $(this).attr("data-id"),
                DatosEnviar: $("#form").serialize(),
                EsPartialView: false,
                EsOperacion: true
            })
        } else {
            setTimeout(function () {
                objLadda.ladda('stop');
            }, 1000);
        }
    });
    $(".boton-link-pagina").click(function () {
        RedireccionarPagina($(this).attr("data-area"), $(this).attr("data-controlador"), $(this).attr("data-accion"), null, 0);
    });

    /// CONFIGURACIÓN DEL DATE PICKER
    /// Fechas Individuales
    $('.control-fecha').each(function (indice, elemento) {
        $(elemento).datepicker({
            forceParse: true,
            autoclose: true,
            format: "dd/mm/yyyy",
            language: "es",
            todayHighlight: true,
            startDate: $(elemento).attr("data-fecha-minima"),
            endDate: $(elemento).attr("data-fecha-maxima"),
        })
    });
    /// Ahora rangos de fechas
    $(".rango-fechas").closest("div").addClass("input-daterange")
    $('.input-daterange').datepicker({
        forceParse: true,
        autoclose: true,
        format: "dd/mm/yyyy",
        language: "es",
        todayHighlight: true,
    });
    $('.input-daterange').find(':text').change(function () {
        var id = $(this).attr("id");
        if (id.includes("_Desde")) { // Es Fecha Desde
            $("#" + id.replace('_Desde', '_Hasta')).datepicker('remove');
            $("#" + id.replace('_Desde', '_Hasta')).datepicker({
                forceParse: true,
                autoclose: true,
                format: "dd/mm/yyyy",
                language: "es",
                todayHighlight: true,
                startDate: $(this).val()
            });
            if (moment($(this).val().isAfter($("#" + id.replace('_Desde', '_Hasta'))))){
                $("#" + id.replace('_Desde', '_Hasta')).val("");
            }
        }
        if (id.includes("")) { // Es Fecha Desde
            $("#" + id.replace('_Hasta', '_Desde')).datepicker('remove');
            $("#" + id.replace('_Hasta', '_Desde')).datepicker({
                forceParse: true,
                autoclose: true,
                format: "dd/mm/yyyy",
                language: "es",
                todayHighlight: true,
                endDate: $(this).val()
            });
            if (moment($(this).val().isBefore($("#" + id.replace('_Hasta', '_Desde'))))){
                $("#" + id.replace('_Hasta', '_Desde')).val("");
            }
        }
    });
    /// Control de Hora
    $('.control-hora').each(function (indice, elemento) {
        $(elemento).datetimepicker({
            locale: "es",
            format: "hh:mm a",
            //minDate: $(elemento).attr("data-fecha-minima"),
            //maxDate: $(elemento).attr("data-fecha-maxima"),
            allowInputToggle: true,
        })
    });
    $('.control-fechahora').each(function (indice, elemento) {
        $(elemento).datetimepicker({
            locale: "es",
            format: "DD/MM/YYYY hh:mm a",
            minDate: $(elemento).attr("data-fecha-minima"),
            maxDate: $(elemento).attr("data-fecha-maxima"),
            allowInputToggle: true,
            sideBySide: true,
        })
    });


    $(":input").inputmask({
        'autoUnmask': true
    });

    ///////////////// Arbol
    $("#Arbol_Busqueda").change(function (event) {
        event.preventDefault();
        Arbol_Buscar($(this).val());
    });
    $("#Arbol_Busqueda").on("keydown", function (event) {
        if (event.which == 13 && $("#Arbol_Busqueda").val() != "") {
            event.preventDefault();
            Arbol_Buscar($(this).val());
        }
    });
    /// Hidden dentro de un control booleano

    $(".cmn-toggle-yes-no").change(function () {
        $($("#" + $(this).prop("id").replace("cb_",""))).val($(this).prop("checked"));
    });

    $(".cmn-toggle-hidden").bind("change", function () {
        console.log("Cambio");
        console.log($(this).prop("id"));
        $($("#cb_" + $(this).prop("id"))).prop("cheched", $(this).val())
    })
    ///Previene el submit al dar enter
    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            //event.preventDefault();
            //return false;
        }
    });
    $(".lista-select2").select2();
    $("#Arbol_Busqueda").focus();
});

function Arbol_Mostrar(elemento) {

    if ($(elemento).parent().attr("data-collapsed") == null
        || $(elemento).parent().attr("data-collapsed") == "false") {
        $(elemento).parent().next('ol').hide();
        $(elemento).parent().attr("data-collapsed", "true");
    } else {
        $(elemento).parent().next('ol').show();
        $(elemento).parent().attr("data-collapsed", "false");
    }
}

function Arbol_SeleccionarUnidad(FuerzaId, DependenciaId, Descripcion) {
    $('#' + $("#Control").val() + "_FuerzaId", window.parent.document).val(FuerzaId);
    $('#' + $("#Control").val() + "_DependenciaId", window.parent.document).val(DependenciaId);
    $('#' + $("#Control").val() + "_Descripcion", window.parent.document).html(Descripcion);

  window.parent.cerrarModal();
}

function Arbol_AbrirModal(Control, Filtro) {
    AbrirFormularioModal("Seguridad", "UnidadEmpleado", "ModalSeleccionarUnidad", "?Control=" + Control + "&FiltroEstaciones=" + Filtro, "Normal");
}

function Arbol_Buscar(Busqueda) {
    Busqueda = Busqueda.toUpperCase();
    $("a").removeClass("unidad-encontrada");
    $('ol').hide();
    $("a:contains('" + Busqueda + "')").each(function (indice, elemento) {
        $(this).addClass("unidad-encontrada");
        $(this).parents("ol").show();
        $(this).parents().attr("data-collapsed", "false");
    });
    $('html, body').animate({
        scrollTop: ($(".unidad-encontrada:eq(0)").offset().top - 5) + 'px'
    }, 'fast');
}
(function ($) {
    $.fn.ListaSelectEstablecerOpciones = function (opcionesLista) {
        var lista = this;
        var agregarEncabezado = false;
        if (lista.find("option[value='']").length > 0) {
            agregarEncabezado = true;
        }
        lista.empty();
        if (agregarEncabezado) {
            lista.append($("<option></option>").attr("value", "").text("[--Seleccione--]"));
        }
        if (opcionesLista != undefined) {
            $.each(opcionesLista, function (indice, opcion) {
                lista.append($("<option></option>").attr("value", indice).text(opcion));
            });
        }
        lista.change();
        lista.prop("disabled", false);
        return this;

    };
}(jQuery));

(function ($) {
    $.fn.ListaSelectCambiado = function (opciones) {
        var lista = this;
        this.change(function () {

            /// Construcción de la URL
            var url = GenerarUrl(opciones.Area, opciones.Controlador, opciones.Accion, "?q=" + opciones.Id);
            if (opciones.Controlador != null & opciones.Accion != null) {
                // url += "/" + opciones.Controlador + "/" + opciones.Accion;
                /// Variables a pasar
                var datosEnviar = {};
                datosEnviar[$(this).attr("id")] = $(this).val();
                //console.log("-----" + lista.attr("id"));
                if (opciones.DatosEnviar != null) {
                    $.each(opciones.DatosEnviar, function waitingDialog(elemento) {
                        //console.log(elemento + " = " + $("#" + elemento).val());
                        datosEnviar[elemento] = $("#" + elemento).val();
                    });
                }

                //console.log(datosEnviar);
                var listaRetornada = [];
                $.ajax({
                    type: "POST",
                    dataType: "json",
                    url: url,
                    data: datosEnviar,
                    beforeSend: function () {
                        // TODO: Hacer el efecto de spin en el control
                    },
                    success: function (datosRetornados) {
                        opciones.onExitoso(datosRetornados);
                    },
                    error: function () {
                        opciones.onExitoso(null);
                    },
                    complete: function () {
                        objLadda.ladda('stop');
                    }
                })
            } else {
                console.log("Error: No esta bien establecido el controlador o la acción");
            }
        });
        //this.change();
        return this;
    };

    var opcionesListaSelectCambiado = {
        onExitoso: function () { },
        onFallido: function () { },
        Area: "",
        Controlador: "",
        Accion: "",
        DatosEnviar: null
    };
}(jQuery));

(function ($) {
    $.fn.ocultar = function () {
        $(this).closest(".form-group").hide();
    }
}(jQuery));

(function ($) {
    $.fn.mostrar = function () {
        $(this).closest(".form-group").show();
    }
}(jQuery));

function AbrirMenuModal() {
    AbrirFormularioModal("", "Account", "Login", null, null);
}
function AbrirLinkMenu(url) {
    top.document.location = url;
}