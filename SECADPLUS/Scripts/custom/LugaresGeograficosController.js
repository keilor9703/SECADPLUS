var parametrosAjax = [
                { "data": "Dummy", "orderable": false },
                { "data": "Boton", "orderable": false },
                { "data": "CODIGO" },
                { "data": "DESCRIPCION" },
                { "data": "TIPO", "orderable": false },
                { "data": "LUGE_CODIGO", "orderable": false },
                { "data": "CODIGO_REGISTRADURIA", "orderable": false },
                { "data": "CODIGO_DANE", "orderable": false },
                { "data": "DPTO", "orderable": false },
                { "data": "NIVEL", "orderable": false },
                { "data": "CODIGO_SIATH", "orderable": false },
                { "data": "CODIGO_SIEDCO", "orderable": false },
                { "data": "VIGENTE", "orderable": false }
];

var parametrosAjaxComunas = [
                      { "data": "Dummy", "orderable": false },
                      { "data": "Boton", "orderable": false },
                      { "data": "CODIGO" },
                      { "data": "DESCRIPCION" },
                      { "data": "VIGENTE", "orderable": false },
                      { "data": "ORDEN", "orderable": false }
];

var parametrosAjaxBarrios = [
                  { "data": "Dummy", "orderable": false },
                  { "data": "Boton", "orderable": false },
                  { "data": "CODIGO" },
                  { "data": "DESCRIPCION" },
                  { "data": "LUGE_CODIGO", "orderable": false },
                  { "data": "COZO_CODIGO", "orderable": false },
                  { "data": "COZO_LUGE_CODIGO", "orderable": false }
];

var nivelesName = ["Continentes", "Paises", "Departamentos", "Cabeceras", "Veredas", "Comunas", "Barrios"];
$(function () {
    //Ajusta tamaño de tablas ocultas por tabs
    $("a[data-toggle=\"tab\"]").on("shown.bs.tab", function (e) {
        $($.fn.dataTable.tables(true)).DataTable().columns.adjust();
    });
    //controles de acciones en el formulario
    var config = {
        '.chosen-select': {},
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'No se encontraron resultados' },
        '.chosen-select-width': { width: "95%" }
    }

    //recorro los níveles borrando filas y aplicando el datatable

    for (i = 0 ; i < nivelesName.length; i++) {
        //carga databale por ajax y serverside procesing
        if (nivelesName[i] === "Comunas") {
            AplicarPluginTablaAjax("Comunas", UrlConValorComuna(-1, -1), parametrosAjaxComunas);
        } else
            if (nivelesName[i] === "Barrios") {
                AplicarPluginTablaAjax("Barrios", UrlConValorBarrio(-1, -1), parametrosAjaxBarrios);
            } else {
                AplicarPluginTablaAjax(nivelesName[i], UrlConValorPadre(-1, -1), parametrosAjax);
            }
    }
});

function UrlConValorPadre(idFather, nivel) {
    var urlConValorPadre = (urlAction1 + "?idFather=" + idFather + "&strNombre=" + nivelesName[nivel + 1] + "&intNivel=" + (nivel + 1));
    return urlConValorPadre;
}

function UrlConValorComuna(idFather, nivel) {
    var urlConValorComuna = (urlAction2 + "?idFather=" + idFather + "&strNombre=" + nivelesName[nivel + 1] + "&intNivel=" + (nivel + 1));
    return urlConValorComuna;
}

function UrlConValorBarrio(idCozoCodigo, idLugeCodigo) {
    var urlConValorBarrio = (urlAction3 + "?idCozoCodigo=" + idCozoCodigo + "&idLugeCodigo=" + idLugeCodigo);
    return urlConValorBarrio;
}

//funcion para reemplazar todo en un string
String.prototype.replaceAll = function (str1, str2, ignore) {
    return this.replace(new RegExp(str1.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"), (ignore ? "gi" : "g")), (typeof (str2) == "string") ? str2.replace(/\$/g, "$$$$") : str2);
}

//abre el contenido en una nueva ventana e indica elemento padre y nivel
function AbreEnNuevaVentana(url, father, level) {
    window.open(url);
    window.father = father;
    window.level = level;
}

//busca en niveles inferiores con el nivel del padre y pinta el resultado en el contenedor
function BuscaPorPadre(idPadre, nivel, contenedorBotonId, descripcion) {
    LimpiarTablas(nivel + 1);
    if (idPadre != "_FATHER") {
        //  $("#" + nivelesName[nivel]).DataTable().ajax.url(UrlConValorPadre(idPadre, nivel));
        if ($.fn.DataTable.isDataTable("#" + nivelesName[nivel])) {
            $("#" + nivelesName[nivel]).DataTable().destroy();
            $("#" + nivelesName[nivel] + " tbody").empty();
        }
        if (nivel < 5) {
            AplicarPluginTablaAjax(nivelesName[nivel], UrlConValorPadre(idPadre, nivel), parametrosAjax);
        }

        switch (nivel) {
            case 1: {
                $("#n2").trigger("click");
                $("#_titlePaises").html("Países de: " + descripcion);
                break;
            }
            case 2: {
                $("#_titleDepartamentos").html("Departamentos de: " + descripcion);
                break;
            }
            case 3:
                {
                    $("#n3").trigger("click");
                    $("#_titleCabeceras").html("Cabeceras de: " + descripcion)
                    break;
                }
            case 4: {
                $("#_titleVeredas").html("Veredas de: " + descripcion);
                $("#_titleComunas").html("Comunas de: " + descripcion);
                BuscaComunas(idPadre, nivel, "_botonComunas", descripcion, false)
                break;
            }
            default: { }
        }
        $("#" + contenedorBotonId).empty();

        var nuevoBoton = botonBase.replace("_FATHER", idPadre).replace("_LEVEL", nivel).replace("_CONTENEDOR", "\"" + contenedorBotonId + "\"").replace("_DESCRIPCION", "\"" + descripcion + "\"");

        $("#" + contenedorBotonId).html(nuevoBoton.replace("elboton", "btn" + nivel));
    }
}

//busca Comunas correspondientes al id del padre
function BuscaComunas(idPadre, nivel, contenedorBotonId, descripcion, deVereda) {
    LimpiarTablas(6);
    if (idPadre != "_FATHER") {
        if (nivel == 5) $("#n4").trigger("click");
    }
    $("#Comunas").DataTable().ajax.url(UrlConValorComuna(idPadre));
    $("#Comunas").DataTable().ajax.reload();
    $("#" + contenedorBotonId).empty();

    $("#_titleComunas").html("Comunas de: " + descripcion);

    var nuevoBoton = botonBase.replace("_FATHER", idPadre).replace("_LEVEL", "\"" + nivel + "\"").replace("BuscaPorPadre", "BuscaComunas").replace("_CONTENEDOR", "\"" + contenedorBotonId + "\"").replace("_DESCRIPCION", "\"" + descripcion + "\"");

    $("#" + contenedorBotonId).html(nuevoBoton.replace("elboton", "btn5"));
}

//busca Barrios correspondientes al id del padre
//var botonBase = "<button id='elboton' onclick='BuscaPorPadre(_FATHER, _LEVEL, _TEXTO_ID, _TEXTO);' class='btn btn-success btn-xs' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para Recargar el Nivel'>Recargar<b><i class='glyphicon glyphicon-plus-sign position-left'></i></b></button>";

function BuscaBarrios(idCozo, idLuge, nivel, contenedorBotonId, descripcion) {
    $("#Barrios").DataTable().ajax.url(UrlConValorBarrio(idCozo, idLuge));
    $("#Barrios").DataTable().ajax.reload();
    $("#_titleBarrios").html("Barrios de: " + descripcion);
    $("#" + contenedorBotonId).empty();
    var nuevoBoton = botonBase.replace("BuscaPorPadre(_FATHER, _LEVEL, _CONTENEDOR, _DESCRIPCION)", "BuscaBarrios(" + idCozo + "," + idLuge + ",\"" + nivel + "\",\"" + "_Barrios" + "\",\"" + descripcion + "\")").replace("_FATHER", idLuge).replace("_LEVEL", "\"" + nivel + "\"")
    $("#" + contenedorBotonId).html(nuevoBoton.replace("elboton", "btn" + nivel));
}

botonBaseVacio = "<button  id='elboton' class='btn btn-success btn-xs' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para Recargar el Nivel'>Recargar<b><i class='glyphicon glyphicon-refresh position-left'></i></b></button>";
botonBase = "<button  id='elboton' onclick='BuscaPorPadre(_FATHER, _LEVEL, _CONTENEDOR, _DESCRIPCION);' class='btn btn-success btn-xs' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para Recargar el Nivel'>Recargar<b><i class='glyphicon glyphicon-refresh position-left'></i></b></button>";
//aplicar plugin de tabla según id para lugaresgeograficos
AplicarPluginTabla("continentes");

//Busca Barrios por ciudad/mpio/comuna
function BuscaBarriosPorCiudad(idControlDestino) {
    seleccionado = $("#LUGE_CODIGO").val();
    var entityData = {
        "id": seleccionado
    };

    $.ajax({
        type: "POST",
        url: "/SitiosImportancia/BuscaBarrios",
        content: "application/json; charset=utf-8",
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        },
        cache: false,
        data: entityData,
        dataType: "json",
        success: function (d) {
            $("#CODI_BARRIO").empty();
            $.each(d, function (i, item) {
                $(idControlDestino).append("<option value='" + item.codigo + "'>" + item.descripcion + "</option>");
            })
            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
        },
        error: function (xhr, textStatus, errorThrown) {
            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
        }
    })
}

//Limpia las tablas que se requieran
function LimpiarTablas(idNivel) {
    ////nombres de los niveles

    //recorro los niveles borrando filas y aplicando el datatable
    for (i = parseInt(idNivel) ; i <= nivelesName.length; i++) {
        if ($.fn.DataTable.isDataTable("#" + nivelesName[i])) {
            $("#" + nivelesName[i]).DataTable().destroy();
            $("#" + nivelesName[i] + " tbody").empty();
            if (nivelesName[i] === "Comunas") {
                AplicarPluginTablaAjax("Comunas", UrlConValorComuna(-1, -1), parametrosAjaxComunas);
            } else if (nivelesName[i] === "Barrios") {
                AplicarPluginTablaAjax("Barrios", UrlConValorBarrio(-1, -1), parametrosAjaxBarrios);
            } else {
                AplicarPluginTablaAjax(nivelesName[i], UrlConValorPadre(-1, -1), parametrosAjax);
            }
        }
        $("#" + nivelesName[i] + " tbody").empty();
        $("#_title" + nivelesName[i]).html(nivelesName[i] + " de: ");
        $("#_boton" + nivelesName[i]).html(botonBaseVacio);
    }
}

//Permite realizar la eliminación de un registro especifico
function EliminarLugarGeografico(urlEliminar, IdCodigo, btnRecargar, descripcion) {
    btnRecargar--;
    var datos = { codigo: IdCodigo };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Nombre Lugar " + descripcion,
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
                url: urlEliminar,
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
                    //recarga el nivel actual

                    $("#btn" + btnRecargar).trigger("click");
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    })
};

//Permite realizar la eliminación de una comuna especifica
function EliminarComuna(urlEliminar, IdCodigo, idPadre, descripcion) {
    var datos = {
        id: IdCodigo,
        idPadre: idPadre
    };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Nombre Comuna " + descripcion,
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
                url: urlEliminar,
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
                    //recarga el nivel actual

                    $("#btn5").trigger("click");
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    })
};

//Permite realizar la eliminación de una comuna especifica
function EliminarBarrio(urlEliminar, IdCodigo, idPadre, descripcion) {
    var datos = {
        id: IdCodigo,
        idPadre: idPadre
    };
    //Presenta pantalla de verificación
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: "Nombre Barrio " + descripcion,
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
                url: urlEliminar,
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
                    //recarga el nivel actual

                    $("#btnbarrios").trigger("click");
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    })
};