/// Botones 
var objLadda;
var mostrarMensajeSinResultados = true;
$(document).ready(function () {
    $(".icono-ordenamiento").hide();
    objLadda = $('.boton').ladda();
    objLadda.click(function () {
        objLadda.ladda('start');
    });
    $(".boton-tabla-paginada").click(function () {
        var identificadorTablaPaginada = $(this).attr("data-identificador-tabla-paginada");
        if (identificadorTablaPaginada == null || identificadorTablaPaginada == "") {
            identificadorTablaPaginada = "TablaResultadosBusqueda";
        }
        $(this).closest(".ibox").find("#_tp_descargar_pdf").val("");
        $(this).closest(".ibox").find("#_tp_descargar_excel").val("");
        DatosAjax({
            Area: $(this).attr("data-area"),
            Controlador: $(this).attr("data-controlador"),
            Accion: $(this).attr("data-accion"),
            DatosEnviar: $("#div_" + identificadorTablaPaginada).parent().find(":input").serialize(),
            onExitoso: function (datosRetornados) {
                console.log(mostrarMensajeSinResultados);
                if (datosRetornados.CantidadRegistros == 0 && mostrarMensajeSinResultados != false) {
                    MensajeInformacion("Resultado de la operación", "La consulta no arrojó resultados.");
                }
                TablaPaginada_Construir(identificadorTablaPaginada, datosRetornados);

            },
            onFallido: function (datosRetornados) { },
        })
    });
    // Click automático cuando
    if ($(".form-control").length == 0) {
        //$(".boton-tabla-paginada").click();
    }
    $(".tabla-paginada").find("th").click(function () {
        var boton = $(this).closest(".ibox").find(".ibox-title").find(".boton-tabla-paginada");
        if (!$(boton).prop("disabled")) {
            $("#_tp_campo_ordenamiento").val($(this).attr("id"));
            var sentido = $("#_tp_sentido_ordenamiento").val();
            if (sentido == null || sentido == "desc") {
                sentido = "asc";
                $(this).find(".icono-ordenamiento").removeClass("fa-chevron-up").addClass("fa-chevron-down");
            } else {
                sentido = "desc";
                $(this).find(".icono-ordenamiento").removeClass("fa-chevron-down").addClass("fa-chevron-up");
            }
            $("#_tp_sentido_ordenamiento").val(sentido);
            $(".icono-ordenamiento").hide();
            $(this).find(".icono-ordenamiento").show();
            $(boton).click();
        }
    });
    var menu = new BootstrapMenu('.tabla-paginada', {
        fetchElementData: function ($element) {
            return $element;
        },
        actions: [{
            name: 'Exportar a PDF',
            onClick: function (elemento) {
                TablaPaginada_ExportarPdf(elemento);
            }
        }, {
            name: 'Exportar a Word',
            onClick: function (elemento) {
                TablaPaginada_ExportarWord(elemento);
            }
        }, {
            name: 'Exportar a Excel',
            onClick: function (elemento) {
                TablaPaginada_ExportarExcel(elemento);
            }
        }]
    });
});
function TablaPaginada_Construir(IdentificadorTablaPaginada, Datos) {
    /// Primero se buscan las columnas y se indexan datos
    var columnas = [];
    var tabla = $("#" + IdentificadorTablaPaginada);
    $(tabla).find("tbody").find("tr").remove();
    tabla.find("th").each(function (indiceColumna, columna) {
        var campo = String($(columna).attr("id")).replace("_", "");
        if (campo != "" && campo != "undefined") {
            columnas.push({ NombreCampo: campo })
        }
    });
    var enlacesConstruidos = [];
    $(tabla).find(".enlace_tabla_paginada").each(function (indice, enlace) {
        var urlEnlace = $(enlace).attr("enlace-url") + "{id}";
        if ($(enlace).attr("enlace-javascript") != null) {
            urlEnlace = "javascript:" + $(enlace).attr("enlace-javascript");
        }
        var datosEnlace = {
            Url: urlEnlace,
            Icono: $(enlace).attr("enlace-icono"),
            Tooltip: $(enlace).attr("enlace-tooltip"),
            Area: $(enlace).attr("enlace-area"),
            Controlador: $(enlace).attr("enlace-controlador"),
            Accion: $(enlace).attr("enlace-accion"),
            AbrirModal: $(enlace).attr("enlace-abrir-modal"),
            TamanoModal: $(enlace).attr("enlace-tamano-modal"),
            MetodoJavascript: $(enlace).attr("enlace-javascript"),
            AbrirVentana: $(enlace).attr("enlace-abrir-ventana"),
        }
        enlacesConstruidos.push(datosEnlace);
    });

    $.each(Datos.DatosPagina, function (index, item) {
        var fila = $("<tr/>");
        $.each(columnas, function (indice, columna) {
            if (columna.NombreCampo != null) {
                if (item.Value[columna.NombreCampo] != undefined && item.Value[columna.NombreCampo] != "") {
                    fila.append($("<td/>").html(item.Value[columna.NombreCampo]));
                } else {
                    fila.append($("<td/>").text(" "));
                }
            }
        });
        // Ahora los links
        var columnaEnlaces = $("<td/>");
        $.each(enlacesConstruidos, function (indice, columnaEnlace) {
            var a = $("<a>");
            if (columnaEnlace.AbrirModal == "true") {
                a.attr("href", "javascript:AbrirFormularioModal('" + columnaEnlace.Area + "','" + columnaEnlace.Controlador + "','" + columnaEnlace.Accion + "','?q=" + item.Key + "','" + columnaEnlace.TamanoModal + "')");
            } else {
                if (columnaEnlace.Icono != "log-in") {
                    if (columnaEnlace.MetodoJavascript != null && columnaEnlace.MetodoJavascript != "") {
                        a.attr("href", columnaEnlace.Url.replace("{id}", "?q=" + item.Key));
                    } else {
                        a.attr("href", "/" + RutaDeploy + columnaEnlace.Url.replace("{id}", "?q=" + item.Key));
                    }
                } else {
                    a.attr("href", "/" + RutaDeploy + columnaEnlace.Url.replace("{id}", item.Value.Id).replace("{Etiqueta}", item.Value.Etiqueta));
                }
                if (columnaEnlace.AbrirVentana) {
                    a.attr("target", "_blank");
                }
            }
            a.attr("title", columnaEnlace.Tooltip);
            a.append("<span>").addClass("glyphicon").addClass("glyphicon-" + columnaEnlace.Icono).addClass("icono-tabla-paginada");
            columnaEnlaces.append(a);
        });
        fila.append(columnaEnlaces);
        tabla.append(fila);
    });
    // Finalmente el paginador
    var ul = $("<ul class=\"pagination\" />");

    if (Datos.PaginaActual >= 10) {
        var pagina = $("<li/>");
        var enlacePagina = $("<a/>");
        enlacePagina.attr("href", "javascript:TablaPaginada_CambiarPagina('" + IdentificadorTablaPaginada + "',0)");
        enlacePagina.text("<<")
        pagina.append(enlacePagina);
        ul.append(pagina);
        var paginaAnterior = Datos.PaginaActual - (Datos.PaginaActual % 10) - 1;
        var pagina = $("<li/>");
        var enlacePagina = $("<a/>");
        enlacePagina.attr("href", "javascript:TablaPaginada_CambiarPagina('" + IdentificadorTablaPaginada + "'," + Number(paginaAnterior - 9) + ")");
        enlacePagina.text("<")
        pagina.append(enlacePagina);
        ul.append(pagina);
    }
    var inicioPagina = Datos.PaginaActual - (Datos.PaginaActual % 10);
    if (Datos.PaginaActual + 10 > Datos.CantidadPaginas) {
        finPagina = inicioPagina + Datos.CantidadPaginas % 10;
    } else {
        finPagina = inicioPagina + 10;
    }
    for (var indicePagina = inicioPagina; indicePagina < finPagina; indicePagina++) {
        var pagina = $("<li/>");
        if (indicePagina == Datos.PaginaActual) {
            pagina.addClass("active");
        }
        var enlacePagina = $("<a/>");
        enlacePagina.attr("href", "javascript:TablaPaginada_CambiarPagina('" + IdentificadorTablaPaginada + "'," + indicePagina + ")");
        enlacePagina.text(indicePagina + 1)
        pagina.append(enlacePagina);
        ul.append(pagina);
    }
    if (Datos.PaginaActual < Datos.CantidadPaginas - 10) {
        var paginaSiguiente = Datos.PaginaActual - (Datos.PaginaActual % 10) + 10;
        var pagina = $("<li/>");
        var enlacePagina = $("<a/>");
        enlacePagina.attr("href", "javascript:TablaPaginada_CambiarPagina('" + IdentificadorTablaPaginada + "'," + paginaSiguiente + ")");
        enlacePagina.text(">")
        pagina.append(enlacePagina);
        ul.append(pagina);
        var pagina = $("<li/>");
        var enlacePagina = $("<a/>");
        enlacePagina.attr("href", "javascript:TablaPaginada_CambiarPagina('" + IdentificadorTablaPaginada + "'," + Number(Datos.CantidadPaginas - 1 ) + ")");
        enlacePagina.text(">>")
        pagina.append(enlacePagina);
        ul.append(pagina);
    }
    $("#" + IdentificadorTablaPaginada + "_Paginas").find("ul").remove();
    $("#" + IdentificadorTablaPaginada + "_Paginas").find("label").remove();
    $("#" + IdentificadorTablaPaginada + "_Paginas").append(ul);
    $("#" + IdentificadorTablaPaginada + "_Paginas").append("<label><br/>" + Datos.CantidadRegistros + " Registros en total</label>");
}
function TablaPaginada_CambiarPagina(IdentificadorTablaPaginada, Pagina) {
    $("#div_" + IdentificadorTablaPaginada).find("#_tp_pagina_actual").val(Pagina);
    $(".boton-tabla-paginada[data-identificador-tabla-paginada='" + IdentificadorTablaPaginada + "']").click();
}

function TablaPaginada_ExportarPdf(elemento) {
    var boton = $(elemento).closest(".ibox").find(".ibox-title").find(".boton-tabla-paginada");
    if (!$(boton).prop("disabled")) {
        $(elemento).closest(".ibox").find("#_tp_descargar_pdf").val("true");
        $(elemento).closest(".ibox").find("#_tp_descargar_word").val("");
        $(elemento).closest(".ibox").find("#_tp_descargar_excel").val("");
        var identificadorTablaPaginada = $(boton).attr("data-identificador-tabla-paginada");
        if (identificadorTablaPaginada == null || identificadorTablaPaginada == "") {
            identificadorTablaPaginada = "TablaResultadosBusqueda";
        }
        $("#form").attr("action", GenerarUrl($(boton).attr("data-area"), $(boton).attr("data-controlador"), $(boton).attr("data-accion"), null));
        $("#form").attr("target", "_blank");
        $("#form")[0].submit();
        $("#form").attr("action", "");
        $("#form").attr("target", "");
    }
}
function TablaPaginada_ExportarWord(elemento) {
    var boton = $(elemento).closest(".ibox").find(".ibox-title").find(".boton-tabla-paginada");
    if (!$(boton).prop("disabled")) {
        $(elemento).closest(".ibox").find("#_tp_descargar_pdf").val("");
        $(elemento).closest(".ibox").find("#_tp_descargar_word").val("true");
        $(elemento).closest(".ibox").find("#_tp_descargar_excel").val("");
        var identificadorTablaPaginada = $(boton).attr("data-identificador-tabla-paginada");
        if (identificadorTablaPaginada == null || identificadorTablaPaginada == "") {
            identificadorTablaPaginada = "TablaResultadosBusqueda";
        }
        $("#form").attr("action", GenerarUrl($(boton).attr("data-area"), $(boton).attr("data-controlador"), $(boton).attr("data-accion"), null));
        $("#form").attr("target", "_blank");
        $("#form")[0].submit();
        $("#form").attr("action", "");
        $("#form").attr("target", "");
    }
}
function TablaPaginada_ExportarExcel(elemento) {
    var boton = $(elemento).closest(".ibox").find(".ibox-title").find(".boton-tabla-paginada");
    if (!$(boton).prop("disabled")) {
        $(elemento).closest(".ibox").find("#_tp_descargar_pdf").val("");
        $(elemento).closest(".ibox").find("#_tp_descargar_word").val("");
        $(elemento).closest(".ibox").find("#_tp_descargar_excel").val("true");
        var identificadorTablaPaginada = $(boton).attr("data-identificador-tabla-paginada");
        if (identificadorTablaPaginada == null || identificadorTablaPaginada == "") {
            identificadorTablaPaginada = "TablaResultadosBusqueda";
        }
        $("#form").attr("action", GenerarUrl($(boton).attr("data-area"), $(boton).attr("data-controlador"), $(boton).attr("data-accion"), null));
        $("#form").attr("target", "_blank");
        $("#form")[0].submit();
        $("#form").attr("action", "");
        $("#form").attr("target", "");
    }
}