var coordenadas_caso = Array();
coordenadas_caso["latitudes"] = Array();
coordenadas_caso["longitudes"] = Array();
var global_view;
var global_graphic;
var global_extent;
var global_dom;
var global_on;
var graficas_ultima_ruta = Array();
var color_estandar = "#8e93c4";
var g_color_prioridad_rutina = "#e68a00";
var g_color_prioridad_inmediata = "#009933";
var g_color_prioridad_flash = "#e60000";
var g_color_importancia_1 = "#e60000";
var g_color_importancia_2 = "#ff9900";
var g_color_importancia_3 = "#009933";
var g_color_alerta_tiempo = "#ffe6e6";
var gAreasAndLengthsParameters;
var gGeometryService;
var areasCasos = Array();
var globalMap;
var globalLayer;
var globalPopupTemplate;


function crearMapaPolicia(referencias_capas, casos_policia) {
    globalCreacionMapa = "N";
    require([
        "esri/Map", //Create a map
        "esri/views/SceneView", //view a 3D map
        "esri/widgets/BasemapToggle",
        "esri/layers/Layer",
        "esri/PopupTemplate",
        "esri/Graphic",
        "dojo/dom", // require dojo/dom for getting the DOM element
        "dojo/on",
        "esri/tasks/support/AreasAndLengthsParameters",
        "esri/tasks/GeometryService",
        "esri/geometry/Extent",
        "dojo/domReady!"//ensures DOM before executing code
    ], function (
        Map,
        SceneView,
        BasemapToggle,
        Layer,
        PopupTemplate,
        Graphic,
        dom,
        on,
        AreasAndLengthsParameters,
        GeometryService,
        Extent,
        domReady
    ) {
        global_graphic = Graphic;
        global_extent = Extent;
        gAreasAndLengthsParameters = AreasAndLengthsParameters;
        gGeometryService = GeometryService;
        global_dom = dom;
        global_on = on;
        //---MAP---
        var map = new Map({
            //basemap: "streets",
            basemap: globalMapaBase,
            ground: "world-elevation"
        });
        //---VIEW---
        global_view = new SceneView({
            container: "viewDiv", // Reference to the DOM node that will contain the view
            map: map, // References the map object created in step 3
            zoom: 8, // Sets the zoom level based on level of detail (LOD)
            center: [-74.063644, 4.624335]  // Sets the center point of view in lon/lat
        });
        //---TOGGLE---
        var basemapToggle = new BasemapToggle({
            view: global_view, // The view that provides access to the map's "streets" basemap
            //nextBasemap: "hybrid"  // Allows for toggling to the "hybrid" basemap
            nextBasemap: globalMapaBaseSecundario  // Allows for toggling to the "hybrid" basemap
        });
        basemapToggle.startup();
        // Add the BasemapToggle widget to the top right corner of the view
        global_view.ui.add(basemapToggle, "bottom-right");

        //add layers
        globalMap = map;
        globalLayer = Layer;
        globalPopTemplate = PopupTemplate;
        globalCreacionMapa = "S";
        // La variable 'globalVerificarCapas' se fija en 'S' al terminar de verificar las capas en iniciar_mapa.js método llenarCapas
        // Si al llegar a este punto la variable es diferente a 'S' la inclusión de las capas se realiza en dicho método
        if (globalVerificarCapas == "S") {
            incluirCapas(referencias_capas);
        }
        
        //add police cases
        incluirCasosPolicia(casos_policia);
    });
}

function incluirCapas(referencias_capas) {
    console.log("incluir capas");
    $("#map_layer_toggles").empty();
    $.each(referencias_capas, function (index, layer_reference) {
        var url_layer = layer_reference["url"];
        var nombre_layer = layer_reference["nombre"];
        var layer_dom_id = "layer" + index + "Toggle";
        var html_for_toggle = '<div class="checkbox checkbox-switchery checkbox-right switchery-xs">';
        html_for_toggle += '<label class="display-block">';
        html_for_toggle += '<input type="checkbox" class="switchery" id="' + layer_dom_id + '">';
        html_for_toggle += nombre_layer;
        html_for_toggle += '</label>';
        html_for_toggle += '</div>';
        $("#map_layer_toggles").append(html_for_toggle);

        var layer_id = "lyr_" + index;
        globalLayer.fromArcGISServerUrl({
            url: url_layer,
            properties: {
                // set any layer properties here
                popupTemplate: new globalPopTemplate(),
                id: layer_id,
                opacity: 0.7,
                visible: false
            }
        }).then(function (layer) {
            // add the layer to the map
            globalMap.add(layer);
            var layer_toggle = global_dom.byId(layer_dom_id);
            global_on(layer_toggle, "change", function () {
                layer.visible = layer_toggle.checked;
            });
        });
    });
}

function checkLoaded(layer) {
    setTimeout(function () {
        console.log("visible 3: " + layer.visible);
        checkLoaded(layer);
    }, 1000);
}

function funcion1() {
    console.log("funcion1")
}

function funcion2() {
    console.log("funcion2")
}

function incluirCasosPolicia(casos_policia) {
    console.log("Genera dato");
    $("#casos_policia").empty();
    $.each(casos_policia, function (index, caso_policia) {
        var color = color_estandar;
        if (caso_policia["color"] != null) {
            color = caso_policia["color"];
        }
        if (index === 0) {
            globalFecha = caso_policia["hora"];
            globalConsecutivo = caso_policia["id"];
        }
        //var caso_policia_link_nombre = "caso_policia_"+index;
        var caso_policia_link_nombre = "caso_policia_" + caso_policia["id"];
        //add cases in list
        var list_background_color = "#fff";
        if (caso_policia["mostrar_alerta_tiempo"] == "SI") {
            list_background_color = g_color_alerta_tiempo;
        }
        var html = '';
        html += '<li class="media" style="background-color:' + list_background_color + '">';
        html += '<a href="javascript:void(0);" class="media-link" id="' + caso_policia_link_nombre + '">';
        //html     +=         '<a href="javascript:void(0);" class="media-link" id="'+caso_policia["id"]+'">';
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
        html += '<div>';
        html += '<span class="text-muted" style="color=red">';
        html += ObtenerStringPrioridad(caso_policia["prioridad"]);
        html += '</span>';
        html += '<span class="media-annotation pull-right" style="color=red">';
        html += ObtenerStringImportancia(caso_policia["importancia"]);
        html += '</span>';
        html += '</div>';
        if (caso_policia["mostrar_alerta_tiempo"] == "SI") {
            html += '<div style="text-align: center; color:red">';
            html += 'ALERTA: La hora de asignación del evento supera el tiempo establecido por el sistema';
            html += '</div>';
        }
        html += '</div>';
        html += '</a>';
        html += '</li>';
        $("#casos_policia").append(html);
        //add events
        global_on(global_dom.byId(caso_policia_link_nombre), "click", function () {
            ConsultarPedidoMapa(caso_policia);
        });

        //Verifica si existe llamada de recepción
        if (glNumeLlamada > -1) {

            if (caso_policia["id"] == glNumeLlamada) {

                ConsultarPedidoMapa(caso_policia);
            }
        } else {
            if (index === 0) {
                $("#idFinal").val(caso_policia["nombre"]);
                ConsultarPedidoMapa(caso_policia);
            }
        }

    });
}

function ObtenerStringImportancia(importancia) {
    var string = '';
    switch (importancia) {
        case '01'://1
            string += '<span style="color: ' + g_color_importancia_1 + '">Importancia: 1</span>'
            break;
        case '02'://2
            string += '<span style="color: ' + g_color_importancia_2 + '">Importancia: 2</span>'
            break;
        case '03'://3
            string += '<span style="color: ' + g_color_importancia_3 + '">Importancia: 3</span>'
            break;
        default:
            string = ''
            break;
    }
    return string;
}

function ObtenerStringPrioridad(prioridad) {
    var string = '';
    switch (prioridad) {
        case '01'://Rutina
            string += '<span style="color: ' + g_color_prioridad_rutina + '">Prioridad: Rutina</span>'
            break;
        case '03'://Inmediata
            string += '<span style="color: ' + g_color_prioridad_inmediata + '">Prioridad: Inmediata</span>'
            break;
        case '04'://Flash
            string += '<span style="color: ' + g_color_prioridad_flash + '">Prioridad: Flash</span>'
            break;
        default:
            string = ''
            break;
    }
    return string;
}

function ConsultarPedidoMapa(caso_policia) {
    var numeLlamada = caso_policia["id"];
    $.ajax({
        type: 'POST',
        url: urlVerPedidoMapa,
        dataType: 'json',
        data: { idSitio: globalSitio, numeLlamada: numeLlamada },
        success: function (resultado) {
            if (resultado.result) {
                caso_policia = clasificarFigurasDeCaso(caso_policia, resultado.lista);
                dibujarFigurasDeCaso(caso_policia);
            } else {
                GestionarAlerta("warning", resultado.lista);
            }
        },
        error: function (ex) {
            GestionarAlerta("warning", ex);
        }
    });
}

function clasificarFigurasDeCaso(caso_policia, info_figura) {
    switch (info_figura[0]["SHAPE"]) {
        case ("CircularArea"):
            var circulos = Array();
            var circulo_uno = Array();
            circulo_uno["nombre"] = "Número de llamada: " + info_figura[0]["CADPEDI_NUMELLAMADA"];
            var area_span_id = "area_" + info_figura[0]["CADPEDI_NUMELLAMADA"];

            var html_content = "";
            html_content += "<div>";
            html_content += "Usuario: " + info_figura[0]["USUARIO_GRABA"] + "<br/>";
            html_content += "Fecha: " + info_figura[0]["TIME_"] + "<br/>";
            html_content += "Estado: " + info_figura[0]["DESC_ESTADO"] + "<br/>";
            html_content += "Número: " + info_figura[0]["MSID"] + "<br/>";
            html_content += "Operador: " + info_figura[0]["OPERADOR"] + "<br/>";
            html_content += "Tipo: " + info_figura[0]["SHAPE"] + "<br/>";
            html_content += "Lat, Lon: " + info_figura[0]["LATLON"] + "<br/>";
            html_content += "Comentario: " + info_figura[0]["COMENTARIO"] + "<br/>";
            html_content += "Dirección: " + info_figura[0]["DIRE_TELEFONO"] + "<br/>";
            html_content += "Barrio: " + info_figura[0]["BARRIO"] + "<br/>";
            html_content += "Nombre Caso: " + info_figura[0]["NOMBRECASO"] + "<br/>";
            html_content += "Área (Km<sup>2</sup>): <span id='" + area_span_id + "'>{Name:fijarArea}</span><br/>";
            html_content += "</div>";

            circulo_uno["info"] = html_content;
            circulo_uno["latitud"] = parseFloat(info_figura[0]["LATITUD_YDEC"]);
            circulo_uno["longitud"] = parseFloat(info_figura[0]["LONGITUD_XDEC"]);
            circulo_uno["radio"] = info_figura[0]["RADIUS"];

            circulos.push(circulo_uno);
            caso_policia["circulos"] = circulos;
            break;

        case ("CircularArcArea"):
            var semi_arcos = Array();
            var semi_arco_uno = Array();
            semi_arco_uno["nombre"] = "Número de llamada: " + info_figura[0]["CADPEDI_NUMELLAMADA"];
            var area_span_id = "area_" + info_figura[0]["CADPEDI_NUMELLAMADA"];

            var html_content = "";
            html_content += "<div>";
            html_content += "Usuario: " + info_figura[0]["USUARIO_GRABA"] + "<br/>";
            html_content += "Fecha: " + info_figura[0]["TIME_"] + "<br/>";
            html_content += "Estado: " + info_figura[0]["DESC_ESTADO"] + "<br/>";
            html_content += "Número: " + info_figura[0]["MSID"] + "<br/>";
            html_content += "Operador: " + info_figura[0]["OPERADOR"] + "<br/>";
            html_content += "Tipo: " + info_figura[0]["SHAPE"] + "<br/>";
            html_content += "Lat, Lon: " + info_figura[0]["LATLON"] + "<br/>";
            html_content += "Comentario: " + info_figura[0]["COMENTARIO"] + "<br/>";
            html_content += "Dirección: " + info_figura[0]["DIRE_TELEFONO"] + "<br/>";
            html_content += "Barrio: " + info_figura[0]["BARRIO"] + "<br/>";
            html_content += "Nombre Caso: " + info_figura[0]["NOMBRECASO"] + "<br/>";
            html_content += "Área (Km<sup>2</sup>): <span id='" + area_span_id + "'>{Name:fijarArea}</span><br/>";
            html_content += "</div>";

            semi_arco_uno["info"] = html_content;
            semi_arco_uno["latitud"] = parseFloat(info_figura[0]["LATITUD_YDEC"]);
            semi_arco_uno["longitud"] = parseFloat(info_figura[0]["LONGITUD_XDEC"]);
            semi_arco_uno["in_radius"] = info_figura[0]["INRADIUS"];
            semi_arco_uno["out_radius"] = info_figura[0]["OUTRADIUS"];
            semi_arco_uno["start_angle"] = info_figura[0]["STARTANGLE"];
            semi_arco_uno["stop_angle"] = info_figura[0]["STOPANGLE"];

            semi_arcos.push(semi_arco_uno);
            caso_policia["semi_arcos"] = semi_arcos;

            break;

        case ("EllipticalArea"):
            var ovalos = Array();
            var ovalo_uno = Array();
            ovalo_uno["nombre"] = "Número de llamada: " + info_figura[0]["CADPEDI_NUMELLAMADA"];
            var area_span_id = "area_" + info_figura[0]["CADPEDI_NUMELLAMADA"];

            var html_content = "";
            html_content += "<div>";
            html_content += "Usuario: " + info_figura[0]["USUARIO_GRABA"] + "<br/>";
            html_content += "Fecha: " + info_figura[0]["TIME_"] + "<br/>";
            html_content += "Estado: " + info_figura[0]["DESC_ESTADO"] + "<br/>";
            html_content += "Número: " + info_figura[0]["MSID"] + "<br/>";
            html_content += "Operador: " + info_figura[0]["OPERADOR"] + "<br/>";
            html_content += "Tipo: " + info_figura[0]["SHAPE"] + "<br/>";
            html_content += "Lat, Lon: " + info_figura[0]["LATLON"] + "<br/>";
            html_content += "Comentario: " + info_figura[0]["COMENTARIO"] + "<br/>";
            html_content += "Dirección: " + info_figura[0]["DIRE_TELEFONO"] + "<br/>";
            html_content += "Barrio: " + info_figura[0]["BARRIO"] + "<br/>";
            html_content += "Nombre Caso: " + info_figura[0]["NOMBRECASO"] + "<br/>";
            html_content += "Área (Km<sup>2</sup>): <span id='" + area_span_id + "'>{Name:fijarArea}</span><br/>";
            html_content += "</div>";

            ovalo_uno["info"] = html_content;
            ovalo_uno["latitud"] = parseFloat(info_figura[0]["LATITUD_YDEC"]);
            ovalo_uno["longitud"] = parseFloat(info_figura[0]["LONGITUD_XDEC"]);
            ovalo_uno["radius_minor"] = info_figura[0]["SEMIMINOR"];
            ovalo_uno["radius_major"] = info_figura[0]["SEMIMAJOR"];
            ovalo_uno["angle"] = info_figura[0]["ANGLE"];

            ovalos.push(ovalo_uno);
            caso_policia["ovalos"] = ovalos;

            break;

        case ("Polygon"):
            var poligonos = Array();
            var poligono_uno = Array();
            poligono_uno["nombre"] = "Número de llamada: " + info_figura[0]["CADPEDI_NUMELLAMADA"];
            var area_span_id = "area_" + info_figura[0]["CADPEDI_NUMELLAMADA"];

            var html_content = "";
            html_content += "<div>";
            html_content += "Usuario: " + info_figura[0]["USUARIO_GRABA"] + "<br/>";
            html_content += "Fecha: " + info_figura[0]["TIME_"] + "<br/>";
            html_content += "Estado: " + info_figura[0]["DESC_ESTADO"] + "<br/>";
            html_content += "Número: " + info_figura[0]["MSID"] + "<br/>";
            html_content += "Operador: " + info_figura[0]["OPERADOR"] + "<br/>";
            html_content += "Tipo: " + info_figura[0]["SHAPE"] + "<br/>";
            html_content += "Lat, Lon: " + info_figura[0]["LATLON"] + "<br/>";
            html_content += "Comentario: " + info_figura[0]["COMENTARIO"] + "<br/>";
            html_content += "Dirección: " + info_figura[0]["DIRE_TELEFONO"] + "<br/>";
            html_content += "Barrio: " + info_figura[0]["BARRIO"] + "<br/>";
            html_content += "Nombre Caso: " + info_figura[0]["NOMBRECASO"] + "<br/>";
            html_content += "Área (Km<sup>2</sup>): <span id='" + area_span_id + "'>{Name:fijarArea}</span><br/>";
            html_content += "</div>";

            poligono_uno["info"] = html_content;

            var array_temp = [];

            $.each(info_figura, function (i, val) {
                var latitud = parseFloat(info_figura[i]["P_LATITUD_YDEC"]);
                var longitud = parseFloat(info_figura[i]["P_LONGITUD_XDEC"]);

                array_temp.push([latitud, longitud])
            });
            array_temp.reverse();

            poligono_uno["points"] = array_temp;
            poligonos.push(poligono_uno);
            caso_policia["poligonos"] = poligonos;

            break;
    }
    //console.log(caso_policia);
    return caso_policia;
}

function dibujarFigurasDeCaso(caso_policia) {
    //console.log("dibujarFigurasDeCaso");
    //console.log(caso_policia);

    //Cuando el array caso_policia contiene la información de las figuras, las dibuja
    if (caso_policia instanceof Array) {
        coordenadas_caso["latitudes"] = Array();
        coordenadas_caso["longitudes"] = Array();

        global_view.graphics.removeAll();
        var datos_puntos = caso_policia["puntos"];
        var datos_polilineas = caso_policia["polilineas"];
        var datos_poligonos = caso_policia["poligonos"];
        var datos_circulos = caso_policia["circulos"];
        var datos_donas = caso_policia["donas"];
        var datos_semi_arcos = caso_policia["semi_arcos"];
        var datos_ellipses = caso_policia["ovalos"];

        var color = caso_policia["color"];
        if (datos_puntos instanceof Array) {
            $.each(datos_puntos, function (index, datos_punto) {
                datos_punto["color"] = color;
                dibujarPunto(datos_punto);
            });
        }
        if (datos_polilineas instanceof Array) {
            $.each(datos_polilineas, function (index, datos_polilinea) {
                datos_polilinea["color"] = color;
                dibujarPolilinea(datos_polilinea);
            });
        }
        if (datos_poligonos instanceof Array) {
            $.each(datos_poligonos, function (index, datos_poligono) {
                datos_poligono["color"] = color;
                dibujarPoligono(datos_poligono);
            });
        }
        if (datos_circulos instanceof Array) {
            $.each(datos_circulos, function (index, datos_circulo) {
                datos_circulo["color"] = color;
                dibujarCirculo(datos_circulo);
            });
        }
        if (datos_donas instanceof Array) {
            $.each(datos_donas, function (index, datos_dona) {
                datos_dona["color"] = color;
                dibujarDona(datos_dona);
            });
        }
        if (datos_semi_arcos instanceof Array) {
            $.each(datos_semi_arcos, function (index, datos_semi_arco) {
                datos_semi_arco["color"] = color;
                dibujarSemiArco(datos_semi_arco);
            });
        }
        if (datos_ellipses instanceof Array) {
            $.each(datos_ellipses, function (index, datos_ellipse) {
                datos_ellipse["color"] = color;
                dibujarElipse(datos_ellipse);
            });
        }
        if (
            (datos_puntos instanceof Array && datos_puntos.length > 0) ||
            (datos_polilineas instanceof Array && datos_polilineas.length > 0) ||
            (datos_poligonos instanceof Array && datos_poligonos.length > 0) ||
            (datos_circulos instanceof Array && datos_circulos.length > 0) ||
            (datos_donas instanceof Array && datos_donas.length > 0) ||
            (datos_semi_arcos instanceof Array && datos_semi_arcos.length > 0) ||
            (datos_ellipses instanceof Array && datos_ellipses.length > 0)
        ) {
            // Si hay alguna figura que se haya pintado
            //Centrar Mapa
            centrarMapaEnFiguras();
        }

        //Imprimir nombre
        if (caso_policia["nombre"] == null) {
            $("#nombre_caso_actual").html("No definido");
        }
        else {
            $("#nombre_caso_actual").html(caso_policia["nombre"]);
        }
    }
}

function dibujarPunto(datos_punto, es_ruta) {
    var punto_dibujado = null;
    //inicia definir zoom y centro del mapa
    coordenadas_caso["latitudes"].push(datos_punto["latitud"]);
    coordenadas_caso["longitudes"].push(datos_punto["longitud"]);
    centrarMapaEnFiguras();
    //finaliza definir zoom y centro del mapa

    require([
        "esri/geometry/Point",
        "esri/symbols/SimpleMarkerSymbol"
    ], function (
            Point,
            SimpleMarkerSymbol
            ) {
        //-----POINT-----
        var point = new Point({
            longitude: datos_punto["longitud"],
            latitude: datos_punto["latitud"]
        });
        // Create a symbol for drawing the point
        var markerSymbol = new SimpleMarkerSymbol({
            color: [226, 119, 40],
            outline: {// autocasts as new SimpleLineSymbol()
                color: [255, 255, 255],
                width: 2
            }
        });
        // Attributes to add in the polyline
        var lineAtt = {
            Name: datos_punto["nombre"], // The nombre of the pipeline
            Info: datos_punto["info"]
        };
        // Create a graphic and add the geometry and symbol to it
        var pointGraphic = new global_graphic({
            geometry: point,
            symbol: markerSymbol,
            attributes: lineAtt,
            popupTemplate:
            {
                title: "{Name}",
                content: [
                                {
                                    type: "fields",
                                    fieldInfos: [
                                        {
                                            fieldName: "Info"
                                        }
                                    ]
                                }
                ]
            }
        });
        // Add the graphics to the view's graphics layer
        global_view.graphics.addMany([pointGraphic]);
        if (es_ruta == true) {
            graficas_ultima_ruta.push(pointGraphic);
        }
        punto_dibujado = point;
    });
    return punto_dibujado;
}

function dibujarPolilinea(datos_polilinea, es_ruta) {
    //inicia definir zoom y centro del mapa
    $.each(datos_polilinea["points"], function (index, point) {
        coordenadas_caso["longitudes"].push(point[0]);
        coordenadas_caso["latitudes"].push(point[1]);
    });
    centrarMapaEnFiguras();
    //finaliza definir zoom y centro del mapa

    require([
        "esri/geometry/Polyline",
        "esri/symbols/SimpleLineSymbol"
    ], function (
            Polyline,
            SimpleLineSymbol
    ) {
        //---POLYLINE---
        // line geometry with the coordinates of the line
        var polyline = new Polyline({
            paths: datos_polilinea["points"]
        });
        // lineSymbol to render the polyline
        var lineSymbol = new SimpleLineSymbol({
            color: [226, 119, 40],
            width: 4
        });
        // Attributes to add in the polyline
        var lineAtt = {
            Name: datos_polilinea["nombre"],
            Info: datos_polilinea["info"],
        };
        // Create the graphic
        var polylineGraphic = new global_graphic(
        {
            geometry: polyline,
            symbol: lineSymbol,
            attributes: lineAtt,
            popupTemplate:
            {
                title: "{Name}",
                content: [
                                {
                                    type: "fields",
                                    fieldInfos: [
                                        {
                                            fieldName: "Info"
                                        }
                                    ]
                                }
                ]
            }
        });
        // Add the graphics to the view's graphics layer
        global_view.graphics.addMany([polylineGraphic]);
        if (es_ruta == true) {
            graficas_ultima_ruta.push(polylineGraphic);
        }
    });
}

function dibujarPoligono(datos_poligono) {
    //inicia definir zoom y centro del mapa
    $.each(datos_poligono["points"], function (index, point) {
        coordenadas_caso["longitudes"].push(point[0]);
        coordenadas_caso["latitudes"].push(point[1]);
    });
    centrarMapaEnFiguras();
    //finaliza definir zoom y centro del mapa

    require([
        "esri/geometry/Polygon", //draw polygon
        "esri/symbols/SimpleFillSymbol" //render polygon
    ], function (
        Polygon,
        SimpleFillSymbol
    ) {
        //---POLYGON---
        // Create a polygon geometry
        var polygon = new Polygon({
            rings: datos_poligono["points"]
        });

        // Create a symbol for rendering the graphic
        var color = color_estandar;
        if (datos_poligono["color"] != null) {
            color = datos_poligono["color"];
        }
        var fillSymbol = new SimpleFillSymbol({
            color: [hexToRgb(color).r, hexToRgb(color).g, hexToRgb(color).b, 0.8],
            outline: {// autocasts as new SimpleLineSymbol()
                color: [255, 255, 255],
                width: 1
            }
        });
        // Attributes to add in the graphic
        var shapeAttributes = {
            Name: datos_poligono["nombre"], // The nombre of the polyfon
            Info: datos_poligono["info"]
        };
        // Add the geometry and symbol to a new graphic
        var polygonGraphic = new global_graphic({
            geometry: polygon,
            symbol: fillSymbol,
            attributes: shapeAttributes,
            popupTemplate:
                    {
                        title: "{Name}",
                        content: datos_poligono["info"]
                    }
        });
        // Add the graphics to the view's graphics layer
        global_view.graphics.addMany([polygonGraphic]);
        obtenerAreaDeFigura(polygon, datos_poligono);
    });
}

function dibujarCirculo(datos_circulo) {
    //inicia definir zoom y centro del mapa
    var r_earth = 6378000;//radio aproximado de tierra en metros
    var pi = 3.1416;
    var latitud_adicional = (datos_circulo["radio"] / r_earth) * (180 / pi);
    coordenadas_caso["longitudes"].push(datos_circulo["longitud"]);
    coordenadas_caso["latitudes"].push(datos_circulo["latitud"] + latitud_adicional);
    coordenadas_caso["latitudes"].push(datos_circulo["latitud"]);
    coordenadas_caso["latitudes"].push(datos_circulo["latitud"] - latitud_adicional);

    centrarMapaEnFiguras();
    //finaliza definir zoom y centro del mapa

    require([
        "esri/geometry/Circle",
        "esri/geometry/Point",
        "esri/symbols/SimpleMarkerSymbol",
        "esri/symbols/SimpleFillSymbol"
    ], function (
            Circle,
            Point,
            SimpleMarkerSymbol,
            SimpleFillSymbol
            ) {
        //-----POINT-----
        var point = new Point({
            longitude: datos_circulo["longitud"],
            latitude: datos_circulo["latitud"]
        });
        // Create a symbol for drawing the point
        var markerSymbol = new SimpleMarkerSymbol({
            color: [226, 119, 40],
            outline: {// autocasts as new SimpleLineSymbol()
                color: [255, 255, 255],
                width: 2
            }
        });
        // Attributes to add in the graphic
        var shapeAttributes = {
            Name: datos_circulo["nombre"], // The nombre of the polyfon
            Info: datos_circulo["info"]
        };
        // Create a graphic and add the geometry and symbol to it
        var pointGraphic = new global_graphic({
            geometry: point,
            symbol: markerSymbol,
            attributes: shapeAttributes,
            popupTemplate:
            {
                title: "{Name}",
                content: datos_circulo["info"]
            }
        });
        //---CIRCLE---
        var circle = new Circle({
            center: point,
            radius: datos_circulo["radio"]
        });
        // Create a symbol for rendering the graphic
        var color = color_estandar;
        if (datos_circulo["color"] != null) {
            color = datos_circulo["color"];
        }
        var fillSymbol = new SimpleFillSymbol({
            color: [hexToRgb(color).r, hexToRgb(color).g, hexToRgb(color).b, 0.8],
            outline: {// autocasts as new SimpleLineSymbol()
                color: [255, 255, 255],
                width: 1
            }
        });
        // Add the geometry and symbol to a new graphic
        var circleGraphic = new global_graphic({
            geometry: circle,
            symbol: fillSymbol,
            attributes: shapeAttributes,
            popupTemplate:
            {
                title: "{Name}",
                content: datos_circulo["info"]
            }
        });
        // Add the graphics to the view's graphics layer
        global_view.graphics.addMany([pointGraphic, circleGraphic]);
        obtenerAreaDeFigura(circle, datos_circulo);
    });
}

function dibujarDona(datos_dona) {
    var punto = dibujarPunto(datos_dona, false);
    var dona;
    require([
        "esri/geometry/Circle",
        "esri/geometry/Polyline",
        "esri/geometry/geometryEngine",
        "esri/symbols/SimpleFillSymbol"
    ], function (
            Circle,
            Polyline,
            geometryEngine,
            SimpleFillSymbol
            ) {
        //círculo externo
        var circle_out = new Circle({
            center: punto,
            radius: datos_dona["out_radius"]
        });
        //círculo interno
        var circle_in = new Circle({
            center: punto,
            radius: datos_dona["in_radius"]
        });
        //Polilínea a partir de círculo
        var polyline = new Polyline();
        polyline.addPath(circle_in.rings[0]);
        //Dona
        var dona = geometryEngine.cut(circle_out, polyline);
        // Attributes to add in the graphic
        var shapeAttributes = {
            Name: datos_dona["nombre"], // The nombre of the polyfon
            Info: datos_dona["info"]
        };
        // Create a symbol for rendering the graphic
        var color = color_estandar;
        if (datos_dona["color"] != null) {
            color = datos_dona["color"];
        }
        var fillSymbol = new SimpleFillSymbol({
            color: [hexToRgb(color).r, hexToRgb(color).g, hexToRgb(color).b, 0.8],
            outline: {// autocasts as new SimpleLineSymbol()
                color: [255, 255, 255],
                width: 1
            }
        });
        // Add the geometry and symbol to a new graphic
        var donaGraphic = new global_graphic({
            geometry: dona[1],
            symbol: fillSymbol,
            attributes: shapeAttributes,
            popupTemplate:
            {
                title: "{Name}",
                content: datos_dona["info"]
            }
        });
        var dona = dona[1];
        // Add the graphics to the view's graphics layer
        global_view.graphics.addMany([donaGraphic]);
        obtenerAreaDeFigura(semi_dona[0], datos_dona);
    });
    return dona;
}

function dibujarSemiArco(datos_semidona) {
    //Teniendo en cuenta que en la base de datos, los ángulos se basan en el eje Y, es necesario hacer la conversión
    //para que el ángulo se base en el eje x con el ángulo de inicio
    datos_semidona["start_angle"] = 360 - (270 + datos_semidona["start_angle"]);
    //Teniendo en cuenta que en el sistema el ángulo final es la adición al ángulo inicial en sentido
    //de las manesillas del reloj, se adecua el valor
    datos_semidona["stop_angle"] = datos_semidona["start_angle"] - datos_semidona["stop_angle"];
    //Pasar ángulos a radianes para cálculos con Math de javascript
    datos_semidona["start_angle"] = (datos_semidona["start_angle"] * Math.PI / 180.0);
    datos_semidona["stop_angle"] = (datos_semidona["stop_angle"] * Math.PI / 180.0);

    require([
        "esri/geometry/Point",
        "esri/geometry/Circle",
        "esri/geometry/Polyline",
        "esri/geometry/geometryEngine",
        "esri/symbols/SimpleFillSymbol"
    ], function (
            Point,
            Circle,
            Polyline,
            geometryEngine,
            SimpleFillSymbol
        ) {
        //punto
        var punto = new Point({
            longitude: datos_semidona["longitud"],
            latitude: datos_semidona["latitud"]
        });
        //círculo externo
        var circle_out = new Circle({
            center: punto,
            radius: datos_semidona["out_radius"]
        });
        //círculo interno
        var circle_in = new Circle({
            center: punto,
            radius: datos_semidona["in_radius"]
        });
        //Dona
        var dona = geometryEngine.difference(circle_out, circle_in);

        //Polilínea con ángulos
        var r_earth = 6378000;//radio aproximado de tierra en metros
        var pi = 3.1416;

        //-----PUNTO 1 (ORIGEN)
        var lon1 = datos_semidona["longitud"];
        var lat1 = datos_semidona["latitud"];

        //PUNTO 2 (BASADO EN ÁNGULO INICIAL)
        //distancia en metros desde el punto de origen añadiendo el radio
        var distancia_x_inicial = (Math.cos(datos_semidona["start_angle"]) * datos_semidona["out_radius"]);//metros en x = cos(angulo)*radio
        var distancia_y_inicial = (Math.sin(datos_semidona["start_angle"]) * datos_semidona["out_radius"]);//metros en y = sin(angulo)*radio
        //calcular metros a ángulos
        var lat_inicial_adicional = (distancia_y_inicial / r_earth) * (180 / pi);
        var lon_inicial_adicional = (distancia_x_inicial / r_earth) * (180 / pi) / Math.cos(lat1 * pi / 180);
        //Adicionar ángulos a latitud y longitud
        var lon2 = lon1 + lon_inicial_adicional;
        var lat2 = lat1 + lat_inicial_adicional;
        //PUNTO 3 (BASADO EN ÁNGULO FINAL)
        //distancia en metros desde el punto de origen
        var distancia_x_final = (Math.cos(datos_semidona["stop_angle"]) * datos_semidona["out_radius"]);//metros en x = cos(angulo)*radio
        var distancia_y_final = (Math.sin(datos_semidona["stop_angle"]) * datos_semidona["out_radius"]);//metros en y = sin(angulo)*radio
        //calcular metros a ángulos
        var lat_final_adicional = (distancia_y_final / r_earth) * (180 / pi);
        var lon_final_adicional = (distancia_x_final / r_earth) * (180 / pi);
        //Adicionar ángulos a latitud y longitud
        var lon3 = lon1 + lon_final_adicional;
        var lat3 = lat1 + lat_final_adicional;

        var polyline = new Polyline({
            paths: [
                [lon2, lat2],
                [lon1, lat1],
                [lon3, lat3]
            ]
        });

        //inicia definir zoom y centro del mapa
        coordenadas_caso["longitudes"].push(lon1);
        coordenadas_caso["latitudes"].push(lat1);
        coordenadas_caso["longitudes"].push(lon2);
        coordenadas_caso["latitudes"].push(lat2);
        coordenadas_caso["longitudes"].push(lon3);
        coordenadas_caso["latitudes"].push(lat3);
        centrarMapaEnFiguras();
        //finaliza definir zoom y centro del mapa

        //Semi dona
        var semi_dona = geometryEngine.cut(dona, polyline);

        // Create a symbol for rendering the graphic
        var color = color_estandar;
        if (datos_semidona["color"] != null) {
            color = datos_semidona["color"];
        }
        var fillSymbol = new SimpleFillSymbol({
            color: [hexToRgb(color).r, hexToRgb(color).g, hexToRgb(color).b, 0.8],
            outline: {// autocasts as new SimpleLineSymbol()
                color: [255, 255, 255],
                width: 1
            }
        });
        //atributos para la gráfica
        var shapeAttributes = {
            Name: datos_semidona["nombre"], // The nombre of the polyfon
            Info: datos_semidona["info"]
        };
        // Create the graphic
        var semiDonaGraphic = new global_graphic(
        {
            geometry: semi_dona[0],
            symbol: fillSymbol,
            attributes: shapeAttributes,
            popupTemplate:
            {
                title: "{Name}",
                content: datos_semidona["info"]
            }
        });
        // Add the graphics to the view's graphics layer
        global_view.graphics.add(semiDonaGraphic);
        obtenerAreaDeFigura(semi_dona[0], datos_semidona);  
    });
}

function dibujarElipse(datos_elipse) {
    //inicia definir zoom y centro del mapa
    var r_earth = 6378000;//radio aproximado de tierra en metros
    var pi = 3.1416;
    var latitud_adicional = (datos_elipse["radius_major"] / r_earth) * (180 / pi);
    coordenadas_caso["longitudes"].push(datos_elipse["longitud"]);
    coordenadas_caso["latitudes"].push(datos_elipse["latitud"] + latitud_adicional);
    coordenadas_caso["latitudes"].push(datos_elipse["latitud"]);
    coordenadas_caso["latitudes"].push(datos_elipse["latitud"] - latitud_adicional);
    centrarMapaEnFiguras();
    //finaliza definir zoom y centro del mapa

    require([
        "esri/geometry/Point",
        "esri/geometry/Polygon",
        "esri/geometry/geometryEngine",
        "esri/symbols/SimpleFillSymbol",
        "esri/symbols/SimpleMarkerSymbol",
        "esri/Graphic"
    ], function (
        Point,
        Polygon,
        geometryEngine,
        SimpleFillSymbol,
        SimpleMarkerSymbol,
        Graphic
    ) {
        //-----MIDDLE POINT-----
        var point = new Point({
            longitude: datos_elipse["longitud"],
            latitude: datos_elipse["latitud"]
        });
        // Create a symbol for drawing the point
        var markerSymbol = new SimpleMarkerSymbol({
            color: [226, 119, 40],
            outline: {// autocasts as new SimpleLineSymbol()
                color: [255, 255, 255],
                width: 2
            }
        });
        // Attributes to add in the graphic
        var shapeAttributes = {
            Name: datos_elipse["nombre"], // The nombre of the polyfon
            Info: datos_elipse["info"]
        };
        // Create a graphic and add the geometry and symbol to it
        var pointGraphic = new global_graphic({
            geometry: point,
            symbol: markerSymbol,
            attributes: shapeAttributes,
            popupTemplate:
            {
                title: "{Name}",
                content: datos_elipse["info"]
            }
        });

        //Teniendo en cuenta que en la base de datos, los ángulos se basan en el eje Y, es necesario hacer la conversión
        //para que el ángulo se base en el eje x con el ángulo de inicio
        var rot = 360 - (270 + Number(datos_elipse["angle"]));
        var xCent = datos_elipse["longitud"];
        var yCent = datos_elipse["latitud"];
        var radiusX = datos_elipse["radius_major"] / 1000;
        var radiusY = datos_elipse["radius_minor"] / 1000;

        var radiusYDec = radiusY / 110.574;
        var radiusXDec = radiusX / (111.319 * Math.cos(radiusYDec * Math.PI / 180));
        var precision = 0.05; //afecta el número de nodos a crear en la ellipse

        var points = [];
        var pointIni = null;

        var xPos, yPos;

        for (var i = 0 * Math.PI; i < 2 * Math.PI; i += precision) {
            xPos = xCent - (radiusXDec * Math.sin(i));
            yPos = yCent + (radiusYDec * Math.cos(i));
            var p = new Point({
                longitude: xPos,
                latitude: yPos,
                spatialReference: { wkid: 4326 }
            });
            if (i == 0) {
                pointIni = new Point({
                    longitude: xPos,
                    latitude: yPos,
                    spatialReference: { wkid: 4326 }
                });
            }
            points.push(p);
        };
        points.push(pointIni);

        var aPolygon = new Polygon({
            spatialReference: { wkid: 4326 }
        });
        points.reverse();//invierte array para que incluya el color de fondo de la figura
        aPolygon.addRing(points);
        aPolygon = geometryEngine.rotate(aPolygon, rot);

        var color = color_estandar;
        if (datos_elipse["color"] != null) {
            color = datos_elipse["color"];
        }
        var fillSymbol = new SimpleFillSymbol({
            color: [hexToRgb(color).r, hexToRgb(color).g, hexToRgb(color).b, 0.8],
            outline: {// autocasts as new SimpleLineSymbol()
                color: [255, 255, 255],
                width: 1
            }
        });

        var aEllipseGraphic = new global_graphic({
            geometry: aPolygon,
            symbol: fillSymbol,
            attributes: shapeAttributes,
            popupTemplate:
            {
                title: "{Name}",
                content: datos_elipse["info"]
            }
        });
        global_view.graphics.addMany([pointGraphic, aEllipseGraphic]);
        obtenerAreaDeFigura(aPolygon, datos_elipse);
    });
}

function centrarMapaEnFiguras(coordenadas) {
    var xmin = 0;
    var ymin = 0;
    var xmax = 0;
    var ymax = 0;
    if (typeof coordenadas != 'undefined' &&
            coordenadas["xmin"] !== null &&
            coordenadas["ymin"] !== null &&
            coordenadas["xmax"] !== null &&
            coordenadas["ymax"] !== null
    ) {
        var xmin = coordenadas["xmin"];
        var ymin = coordenadas["ymin"];
        var xmax = coordenadas["xmax"];
        var ymax = coordenadas["ymax"];
    }
    else {
        $.each(coordenadas_caso["longitudes"], function (index, longitud) {
            //set xmin
            if (xmin === 0) {
                xmin = longitud;
            }
            else if (xmin > longitud) {
                xmin = longitud;
            }
            //set xmax
            if (xmax === 0) {
                xmax = longitud;
            }
            else if (xmax < longitud) {
                xmax = longitud;
            }
        });
        $.each(coordenadas_caso["latitudes"], function (index, latitud) {
            //set ymin
            if (ymin === 0) {
                ymin = latitud;
            }
            else if (ymin > latitud) {
                ymin = latitud;
            }
            //set ymax
            if (ymax === 0) {
                ymax = latitud;
            }
            else if (ymax < latitud) {
                ymax = latitud;
            }
        });

    }
    if (xmin !== 0 && ymin !== 0 && xmax !== 0 && ymax !== 0) {
        global_view.then(function () {
            //view is ready
            global_view.extent = new global_extent({
                xmin: xmin,
                ymin: ymin,
                xmax: xmax,
                ymax: ymax,
                spatialReference: 4326
            });
            global_view.zoom = (global_view.zoom - 1);
        })
        .otherwise(function (err) {
            console.error("SceneView rejected:", err);
        });
    }
}

function eliminarGraficas(array_graficas) {
    $.each(array_graficas, function (index, grafica) {
        global_view.graphics.remove(grafica);
    });
}

function eliminarGraficasUltimaRuta() {
    eliminarGraficas(graficas_ultima_ruta);
}

function hexToRgb(hex) {
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16)
    } : null;
}

//***************************************************************
//**********************Función que calcula el área de una figura
function obtenerAreaDeFigura(geometry_shape, datos_grafica) {
    if ((typeof areasCasos[datos_grafica["nombre"]] != 'undefined')) {
        console.log("valor de array: " + areasCasos[datos_grafica["nombre"]]);
        return;
    }
    var geometryService = new gGeometryService({
        url: "http://sampleserver6.arcgisonline.com/arcgis/rest/services/Utilities/Geometry/GeometryServer"
    });
    var areasAndLengthParams = new gAreasAndLengthsParameters({
        areaUnit: "square-kilometers",
        lengthUnit: "kilometers",
        polygons: [geometry_shape]
    });
    var area = geometryService.areasAndLengths(areasAndLengthParams);
    geometryService.areasAndLengths(areasAndLengthParams).then(function (results) {
        var areaCalc = results.areas[0];
        areasCasos[datos_grafica["nombre"]] = areaCalc;
        console.log("area calculada: ", areasCasos[datos_grafica["nombre"]]);
    });
}

fijarArea = function (value, key, data) {
    return areasCasos[value];
}