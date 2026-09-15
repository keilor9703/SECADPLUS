var intervalDespachosId;
var globalCapas = new Array();
var globalCasos = Array();
var globalTimer = true;//True habilita la actualización de casos
var globalControlCasos = 0;
var globalVerificarCapas = "N";
var globalCreacionMapa = "N";
////////////////////////////////////////////////////////////////////
/////control de intervalos cuando se encutre la ventana activa//////
////////////////////////////////////////////////////////////////////
//$(window).on("blur focus", function (e) {
//    var prevType = $(this).data("prevType");

//    if (prevType != e.type) {   //  reduce double fire issues
//        switch (e.type) {
//            case "blur":
//                // detiene timer si la ventana no se encuentra activa
//                //console.log("Detiene Timer mapa");
//                clearInterval(intervalDespachosId);

//                break;
//            case "focus":
//                // habilita timer si no existe abonado
//                //console.log("Inicia Timer mapa");
//                IniciarFuncionalidad();
//                break;
//        }
//    }

//    $(this).data("prevType", e.type);
//})
////////////////////////////////////////////////////////////////////
/////control de intervalos cuando se encutre la ventana activa//////
////////////////////////////////////////////////////////////////////
$(function () {
    $(".drag_panel").draggable();
});

$(function () {
    $("#panel-casos, #panel-capas").hide();

    $("#panel-capas .panel-close").click(function () {
        $("#panel-capas").hide("fast");
    });

    $("#panel-casos .panel-close").click(function () {
        $("#panel-casos").hide("fast");
    });

    $("#casos").click(function () {
        $("#panel-casos").show(1000);
    });
    /*Script para aplicar estilos a los checkbox del panel de capas*/
    $("#capas").click(function () {
        $("#panel-capas").show(1000);
        $("input[type='checkbox']").each(function (key, item) {
            new Switchery($(item)[0], {
                color: 'teal'
            });
        });
    });
});
function HabilitarRecepcion() {
    if (glNumeLlamada > -1) {
        $("#USUARIO").val(glUsuarioSeleccion);
        $("#USUARIO").trigger("chosen:updated");
        globalTimer = false;
        IniciarFuncionalidad();
    }
}
$(document).ready(function () {
    CambiarVisibilidadMenu();
    //Habilita controles para recepcion
    HabilitarRecepcion();
    $("#USUARIO").change(function () {
        IniciarFuncionalidad();
    });

    //var casos = obtenerCasos();
});

function IniciarFuncionalidad() {
    $("#casos_policia").empty();
    _strUsuario = $("#USUARIO").val();
    _nomTipo = $("#TIPO option:selected").text();

    $("#COD_CONDUCTA").val(null);
    $("#NOM_CONDUCTA").val(null);

    ConsultarCapasMapa(_strUsuario);
    ConsultarCasosMapa(_strUsuario);
    intervalDespachosId = setInterval(function () { ConsultarCasosMapaNuevos(_strUsuario); }, tiempoRecarda);
}

function DeternerTimer() {
    if (intervalDespachosId == null) {
        intervalDespachosId = setInterval(ConsultarCasosMapa, tiempoRecarda);
    } else {
        clearInterval(intervalDespachosId);
        intervalDespachosId = null;
    }
}



function LlenarCapas(lista) {
    globalCapas = [];

    var contador = 0;
    var cantidad_capas = lista.length;
    //console.log("Cantidad capas: " + cantidad_capas);

    $.each(lista, function (i, val) {

        //Agregamos las capas siempre que no haya error o timeout
        var urlFinal = lista[i].URL.replace(/amp;/g, "");
        //console.log('Buscar capa: ' + lista[i].NOMBRE);
        $.ajax({
            url: urlFinal,
            //timeout: 5000,
            success: function () {
                contador++;
                //console.log('Capa encontrada: ' + lista[i].NOMBRE);
                //carga las posiciones encontradas de la dirección
                var capa = new Array();
                capa["url"] = lista[i].URL;
                capa["nombre"] = lista[i].NOMBRE;
                globalCapas.push(capa);
                if (contador == cantidad_capas) {
                    globalVerificarCapas = "S";
                    // La variable globalCreacionMapa es fijada en 'S' al terminar de crear el mapa en construccion_figura.js método crearMapaPolicia
                    // Si la variable es diferente de 'S' la inclusión de las capas se realiza en dicho método
                    if (globalCreacionMapa == "S") {
                        incluirCapas(globalCapas);
                    }
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                //console.log('Error en capa: ' + lista[i].NOMBRE);
                contador++;
                if (textStatus === "timeout") {
                    GestionarAlerta("warning", "Algunas capas no pudieron ser cargadas.");
                } else {
                    //console.log('Status: ' + textStatus);
                    //console.log('Error: ' + errorThrown);
                    GestionarAlerta("warning", "Se han presentado errores al consultar las capas");
                }
                if (contador == cantidad_capas) {
                    globalVerificarCapas = "S";
                    if (globalCreacionMapa == "S") {
                        incluirCapas(globalCapas);
                    }
                }
            }
        });
    });
}

function ConsultarCasosMapa(_strUsuario) {

    var datos = { idSitio: globalSitio, tiempoBusqueda: tiempoBusqueda, strFecha: globalFecha, strConsecutivo: globalConsecutivo, strUsuario: _strUsuario, strOrigen : glOrigen }
    $.ajax({
        type: 'POST',
        url: urlVerCasosMapa,
        dataType: 'json',
        data: datos,
        success: function (resultado) {
            if (resultado.result) {
                Llenar(resultado.lista);
            } else {
                GestionarAlerta("warning", resultado.lista);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}
function AgregarCasos(lista) {
    globalCasos = new Array();
    ////console.log(globalCasos.length);
    var cantidadInicial = globalCasos.length;
    ////console.log(globalConsecutivo);
    globalControlCasos = globalConsecutivo;

    $.each(lista, function (i, val) {
        ////console.log("Entro: " + i);
        ////console.log(lista[i].CADPEDI_NUMELLAMADA + " - " + lista[i].MSID);
        //Carga el valor del consecutivo

        //if (lista[i].CADPEDI_NUMELLAMADA <= globalConsecutivo && cantidadInicial >0) {
        //    return true;
        //}
        if (i === 0) {
            globalFecha = lista[i].TIME_;
            globalConsecutivo = lista[i].CADPEDI_NUMELLAMADA;
        }
        var caso = Array();

        caso["id"] = lista[i].CADPEDI_NUMELLAMADA;
        caso["nombre"] = "Llamada: " + lista[i].CADPEDI_NUMELLAMADA + " Consec.: " + lista[i].REG_CONSECUTIVO;
        caso["descripcion"] = lista[i].DESC_ESTADO;
        caso["hora"] = lista[i].TIME_;
        caso["usuario"] = lista[i].USUARIO_GRABA;
        caso["consecutivo"] = lista[i].REG_CONSECUTIVO;
        
        if (lista[i].ESTADO == 'S') {
            caso["color_icono"] = "orange";
            caso["color"] = "#a84e1d";//hexadecimal
        }

        if (lista[i].ESTADO == 'P') {
            caso["color_icono"] = "red";
            caso["color"] = "#a81d31";//hexadecimal
        }

        if (lista[i].ESTADO == 'E' || lista[i].ESTADO == 'C') {
            caso["color_icono"] = "green";
            caso["color"] = "#1da894";//hexadecimal
        }

        caso["importancia"] = lista[i].IMPORTANCIA;
        caso["prioridad"] = lista[i].PRIORIDAD;
        caso["mostrar_alerta_tiempo"] = lista[i].TIEMPO_ASIGNA;

        /*
        //Ejemplo importancia prioridad y alerta:
        caso["importancia"] = '01';
        caso["prioridad"] = '01';
        caso["mostrar_alerta_tiempo"] = 'NO';
        */
        globalCasos.push(caso);
    });
    //console.log("adicion de los casos finalizada");
    ////console.log(globalCasos.length);
}
function Llenar(lista) {

    //AgregarCasos(lista);
    //var capas = obtenerCapas();
    crearMapaPolicia(globalCapas, globalCasos);
}

function ConsultarCapasMapa(_strUsuario) {
    globalVerificarCapas = "N";
    if (globalCapas.length > 0) {
        return true;
    }
    $.ajax({
        type: 'POST',
        url: urlVerCapasMapa,
        dataType: 'json',
        //data: { strUsuario: globalUsuario },
        data: { strUsuario: _strUsuario },
        async: false,
        success: function (resultado) {
            //console.log("capas consultadas");
            //console.log(resultado.Lista);
            if (resultado.result) {
                LlenarCapas(resultado.lista);
            } else {
                GestionarAlerta("warning", resultado.lista);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    }).fail(function () {
        GestionarAlerta("warning", "No se han podido cargar las capas del mapa.");
    });
}

function ConsultarCasosMapaNuevos(_strUsuario) {

    ////console.log("Interval Id:" + intervalDespachosId);
    ////console.log("Tiempo Recarga :" + tiempoRecarda);
    if (globalFecha === "") {
        return;
    }
    ////console.log(globalSitio + " " + tiempoBusqueda + " " + globalFecha + " " + globalConsecutivo + " " + _strUsuario);
    var datos = { idSitio: globalSitio, tiempoBusqueda: tiempoBusqueda, strFecha: globalFecha, strConsecutivo: globalConsecutivo, strUsuario: _strUsuario, strOrigen : glOrigen }
    $.ajax({
        type: 'POST',
        url: urlVerCasosMapa,
        dataType: 'json',
        data: datos,
        success: function (resultado) {
            ////console.log(resultado);
            ////console.log(resultado.lista.length);
            if (resultado.result) {
                if (resultado.lista.length > 0) {

                    LlenarNuevos(resultado.lista);
                }
            } else {
                GestionarAlerta("warning", resultado.lista);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    }).fail(function () {
        GestionarAlerta("warning", "Falló la carga de casos nuevos, timer detenido.");
        clearInterval(intervalDespachosId);
    });
}

function LlenarNuevos(lista) {
    AgregarCasos(lista);

    ////console.log(globalCasos);
    actualizarCasosPolicia(globalCasos.reverse())
    //var capas = obtenerCapas();
    //crearMapaPolicia(globalCapas, globalCasos);
}

function actualizarCasosPolicia(casos_policia) {

    $.each(casos_policia, function (index, caso_policia) {
        var color = color_estandar;
        if (caso_policia["color"] != null) {
            color = caso_policia["color"];
        }


        if (caso_policia["id"] <= globalControlCasos) {

            //actualiza los estados
            $("#casoColor_" + caso_policia["id"]).attr("style", "background-color: " + color);
            $("#casoEstado_" + caso_policia["id"]).html(caso_policia["descripcion"]);
            return true;
        } else {

        }
        globalConsecutivo = caso_policia["id"];
        //var caso_policia_link_nombre = "caso_policia_"+index;
        var caso_policia_link_nombre = "caso_policia_" + caso_policia["id"];
        //add cases in list
        var html = '<li class="media">';
        html += '<a href="javascript:void(0);" class="media-link" id="' + caso_policia_link_nombre + '">';
        html += '<div class="media-left">';
        html += '<div class="icono_caso" style="background-color: ' + color + '" id="casoColor_' + caso_policia["id"] + '"></div>';
        html += '</div>';
        html += '<div class="media-body">';
        html += '<div class="media-heading">';
        html += '<span class="text-semibold">';
        html += caso_policia["nombre"];
        html += '</span>';
        html += '<span class="media-annotation pull-right">';
        html += caso_policia["hora"];
        html += '</span>';
        html += '</div>';
        html += '<span class="text-muted" id="casoEstado_' + caso_policia["id"] + '">';
        html += caso_policia["descripcion"];
        html += '</span>';
        html += '<span class="media-annotation pull-right">';
        html += caso_policia["usuario"];
        html += '</span>';
        html += '</div>';
        html += '</a>';
        html += '</li>';
        $("#casos_policia").prepend(html);
        //add events
        global_on(global_dom.byId(caso_policia_link_nombre), "click", function () {
            ConsultarPedidoMapa(caso_policia);
        });
    });
}

//function obtenerCasos(){
//    //EN ESTA FUNCIÓN SE DEBE HACER LLAMADO AJAX PARA OBTENER LOS CASOS
//    //Cada elemento del array, debe ser un caso con los siguientes índices: id, name, description, time, icon_color.
//    var casos = Array();
//    casos = obtenerCasosEjemplo();
//    return casos;
//}
