var map;
var strAddress;
var locator;
var latDelito = 0;
var lonDelito = 0;
var numCuadrante, numCuadrante1, ent_administrativa, barrio, cod_estacion, cod_cai, cua_rural, nom_municipio;
var scaleBar;
var strInformacion;
var mapPoint;
var address;
var infoWindow;
var markColor = "0a9242";
var code;
var CentrarMapaPorMunicipio;
var mpioLayer;
var polygonHighlightSymbol;
var municipiosExtent;






console.log("Iniciando la carga de modulos...");


require([

    "esri/map",
    "esri/arcgis/utils",
    "esri/layers/FeatureLayer",
    "esri/graphicsUtils",
    "esri/symbols/SimpleLineSymbol",
    "esri/graphic",
    "esri/geometry/webMercatorUtils",
    "esri/symbols/SimpleFillSymbol",
    "esri/Color",
    "esri/dijit/BasemapGallery",
    "esri/dijit/LayerList",
    "esri/dijit/Scalebar",
    "esri/dijit/Search",
    "esri/dijit/LocateButton",
    "esri/dijit/Geocoder",
    "dojo/query",
    "dojo/on",
    "esri/tasks/query",
    "dojo/dom",
    "dojo/promise/all",
    "dojo/ready",
    "dojo/_base/array",
    "dojo/_base/lang",
    "dojo/parser",
    "dijit/registry",
    "dojo/dom-construct",
    "dijit/layout/BorderContainer",
    "dijit/layout/ContentPane",
    "dijit/form/ComboBox",
    "dojo/domReady!"
], function (

    Map,
    arcgisUtils,
    FeatureLayer,
    graphicsUtils,
    SimpleLineSymbol,
    Graphic,
    webMercatorUtils,
    SimpleFillSymbol,
    Color,
    BasemapGallery,
    LayerList,
    Scalebar,
    Search,
    LocateButton,
    Geocoder,
    query,
    on,
    Query,
    dom,
    all,
    ready,
    arrayUtils,
    lang,
    parser,
    registry,
    domConstruct) {



    console.log("Todos los módulos se cargaron correctamente.");


    parser.parse();

    /**
     * Crea un objeto mapa a partir de un objeto Web Map de ArcGIS Online.
     * Todas las capas (operacionales y mapas base), simbologia, escalas de visualizacion son tomadas del Web Map.
     * Si el Mapa no es publico, la aplicacion solicitara credeniales.
     */

    //Si se quiere acceder a un webmap de Portal se debe quitar el comentario siguiente y  reemplazar el id del webmap segun se requiera.
    //arcgisUtils.arcgisUrl = "https://srvportalgis.policia.gov.co/arcgis/sharing/rest/content/items";



    arcgisUtils.createMap("d453139dc4524a48ae9b2bb9b3d2a0dc", "map").then(function (response) {

        console.log("Mapa Cargado");

        //be78e4960cce4e0eb6ee21c099b6b284 --> Servicio Georreferenciacion ADENUNCIAR
        //Actualiza el titulo de la pagina web de acuerdo a titulo del webmap
        document.title = 'Visor SECAD';

        map = response.map;
        window.map = response.map;

        /**
         * Agrega  el widget Galeria de Mapas Base a la ventana de Mapa
         * @showArcGISBasemaps {Boolean}          Determina si se cargan o no los mapas Base de ArcGIS y Bing a la Galeria.
         * @map: Objeto Mapa creado previamente.
         */

        var basemapGallery = new BasemapGallery({
            showArcGISBasemaps: true,
            map: response.map
        }, "basemapGallery");
        basemapGallery.startup();

        basemapGallery.on("error", function (msg) {
            console.log("Ha ocurrido un error cargando la Galeria de Mapas Base:  ", msg);
        });

        /**
         * Agrega  el widget Scalebar a la ventana de Mapa que despliega una barra de escala en la parte inferior izquierda de la pantalla.
         * @scalebarUnit {String}. Los valores permitidos son  "english" o "metric" o "dual", si se selecciona dual, la barra de escala mostrara unidades en el sistema metrico y en el sistema ingles.
         * @map: Objeto Mapa creado previamente.
         */

        scaleBar = new Scalebar({
            map: response.map,
            scalebarUnit: "metric"
        });

        /**
         * Agrega  el widget LayerList a la ventana de Mapa el cual permite administrar la visibilidad y transparencia de las capas operacionales agregadas al mapa. Este widget respeta los niveles de
         * visualizacion de las capas configurados en el webmap
         * @map: Objeto Mapa creado previamente.
         * @showLegend: {Boolean} indica si se despliega o no la leyenda de las capas operacionales
         * @showSubLayers: {Boolean} indica si se despliegan o no las subcapas asociadas a las capas operacionales
         * @showOpacitySlider:  {Boolean} Indica si se debe desplegar el slider de transparencia
         * @layers: {array} lista de capas que seran cargadas en el control, por defecto se cargaran todas las capas operacionales del webmap indicado previamente.
         */

        var listLayers = new LayerList({
            map: response.map,
            showLegend: true,
            showSubLayers: true,
            showOpacitySlider: true,
            layers: arcgisUtils.getLayerList(response)
        }, "layerList");
        listLayers.startup();

        /**
         * Agrega  el widget Search a la ventana de Mapa el cual permite realizar la busqueda por direccion
         * @map: Objeto Mapa creado previamente.
         * @countryCode: {String} Restingue las busquedas al pais indicado en a traves del Codigo corto, 'COL' o 'CO' sirven para Colombia
         */

        var search = new Search({
            map: response.map,
            countryCode: 'COL,CO'
        }, "search");
        search.startup();

        /**
         * Agrega  el widget LocateButton a la ventana de Mapa el cual permite localizar y hacer zoom a la posicion (localizacion) del usuario.
         * Solo se activa si la pagina se carga como https (de forma segura).
         * @map: Objeto Mapa creado previamente.
         */

        geoLocate = new LocateButton({
            map: response.map
        }, "LocateButton");
        geoLocate.startup();

        /**
         * Agrega  el widget personalizado enviarInformacionLink el cual reescribe el comportamiento de la ventana infoWindow del mapa y agrega un tag html de tipo "a" a la seccion actionList de la misma.
         * @map: Objeto Mapa creado previamente.
         */

        enviarInformacionLink = domConstruct.create("a", {
            "class": "action",
            "id": "infoLink",
            "innerHTML": "Enviar Informaci\xf3n",
            "href": "javascript:void(0);"
        }, query(".actionList", window.map.infoWindow.domNode)[0]);

        on(enviarInformacionLink, "click", procesoInformacion);

        mapReady();

        // Registra los eventos click y mouse-move en el mapa y les agrega funciones especificas a cada uno
        map.on("click", executeIdentifyTask);
        map.on("mouse-move", showCoordinates);

        //Registra la URL del servicio de geocodificacion a usar para buscar por direcciones.
        locator = new esri.tasks.Locator("https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer");

        //Agrega un comportamiento adicional al widget Buscar, una vez el usuario selecciona una de las direcciones sugeridas el control se encarga de realizar la consulta de informacion de localizacion que se asociara al
        //delito (Codigo SIEDCO, Cod DANE, Direccion, Cuadrante, etc.)

        //Genera busqueda sobre la opcion seleccionada 
        on(search, 'select-result', function (response) {
            //map.infoWindow.hide();
            map.centerAndZoom(response.result.feature.geometry, 15);

        });

    });




    mpioLayer = new FeatureLayer("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/3", {
        mode: FeatureLayer.MODE_ONDEMAND,
        outFields: ["*"]
    });

    polygonHighlightSymbol = new SimpleFillSymbol(
        SimpleFillSymbol.STYLE_SOLID,
        new SimpleLineSymbol(
            SimpleLineSymbol.STYLE_SOLID,
            new Color([255, 0, 0]), 1),
        new Color([125, 125, 125, 0.35])
    );

    municipiosExtent = function (code) {
        if (code) {
            map.graphics.clear();
            var query = new Query();
            query.where = "CODIGO='" + code + "'";
            console.info(query.where);
            query.returnGeometry = true;
            mpioLayer.queryFeatures(query, function (featureSet) {
                var Geom = featureSet.features[0].geometry;
                var gra = new Graphic(Geom);
                var extent = webMercatorUtils.geographicToWebMercator(gra.geometry);
                gra.setSymbol(polygonHighlightSymbol);
                map.graphics.add(gra);
                map.setExtent(extent.getExtent());
            })
        }
    };



    function mapReady() {
        code = codigoDaneInicial;
        municipiosExtent(code);
        console.log("Codigo dane inicial " + code);
    }

    CentrarMapaPorMunicipio = function (_codDane) {
        code = _codDane;
        municipiosExtent(code);
        console.log("Seleccionó otro municipio:" + code);
    }

    //Function to show coordinates on map
    function showCoordinates(event) {
        mapCoor = esri.geometry.webMercatorToGeographic(event.mapPoint);
        dojo.byId("spnCoordinates").innerHTML = "Lat: " + converCoor(mapCoor.y) + " , Lon: " + converCoor(mapCoor.x);
    }

    //Function to convert decimal to degree
    function converCoor(coor) {
        var gg, mm, ss;
        if (coor < 0) {
            var absCoor = Math.abs(coor);
            gg = parseInt(absCoor, 10);
            mm = parseInt(((absCoor - gg) * 60), 10);
            ss = ((((absCoor - gg) * 60) - mm) * 60).toFixed(2);
            gg *= -1;
        } else {
            gg = parseInt(coor, 10);
            mm = parseInt(((coor - gg) * 60), 10);
            ss = ((((coor - gg) * 60) - mm) * 60).toFixed(2);
        }
        return (gg.toString() + '\xBA ' + mm.toString() + "\' " + ss.toString() + "\"");
    }

    function procesoInformacion() {
        console.log('Salida');
        onButtonOKClick();
        //dialog.show();
        //strAddress = "La Direcci&oacute; excede 10 mts de distancia a la intersecci&oacute;n vial m&aacute;s cercana."
        //locator.locationToAddress(mapPoint, 10, direccionObtenida, errorDireccionObtenida);
    }

    function direccionObtenida(evt) {
        if (evt.address) {
            strAddress = evt.address.Address + " (+/- 10 mts)";
        }
        return strAddress;
    }

    function errorDireccionObtenida(evt) {
        //onButtonOKClick();
        return strAddress;
    }

    function numberWithCommas(x) {
        return x.toFixed(6);
    }


    //Funtion to obtain parametres from URL
    function getUrlVars() {
        vars = {};
        var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi,
            function (m, key, value) {
                vars[key] = value;
            });
        var s = '';
        console.log(parts);
        for (var propertyName in vars) {
            // propertyName is what you want
            // you can get the value like this: myObject[propertyName]    	
            s += propertyName + "=" + vars[propertyName] + "&";
        }
        return s;
    }

    //Function to send the coordenates to new page
    function onButtonOKClick() {

        var va = getUrlVars();

        //var loc = "/Recepcion/ObtenerCordenadas?"; // Cambiar Loc - Metodo para capturar las coordenadas y enviarselas al metodo ObtenerUbicacionCriminalidad /// OJO!!!!! ///
        //console.log(loc);

        //var loc = "!pag_hechos.html?";
        if (vars.redirect == null && vars.funct == null) {
            window.location = loc + strInformacion + "&" + va;
        }
        if (vars.redirect != null && vars.funct == null) {
            loc = vars.redirect;
            window.location = loc + strInformacion + "&" + va;
        }
        if (vars.redirect == null && vars.funct != null) {
            window.opener[vars.funct](strInformacion + "&" + va);
            window.close();
        }
        if (vars.redirect != null && vars.funct != null) {
            alert('Debe especificar Nombre o URL, no ambas');
        }


        ValorLat("Hola");

        $.ajax({
            url: '/Recepcion/ObtenerCordenadas' + "/" + strInformacion,
            type: "GET",
            dataType: "json",
            success: function (response) {
                window.map.infoWindow.hide();


                //Mitst();
            },
            error: function (xhr, status) {

            }
        });


    }

    //Function to obtain the coordenates from points
    function makeMarker(lon, lat) {

        //code to disegner 
        var markPath = "M21.021,16.349c-0.611-1.104-1.359-1.998-2.109-2.623c-0.875,0.641-1.941,1.031-3.103,1.031c-1.164,0-2.231-" +
            "0.391-3.105-1.031c-0.75,0.625-1.498,1.519-2.111,2.623c-1.422,2.563-1.578,5.192-0.35,5.874c0.55,0.307,1.127,0.078,1.723-" +
            "0.496c-0.105,0.582-0.166,1.213-0.166,1.873c0,2.932,1.139,5.307,2.543,5.307c0.846,0,1.265-" +
            "0.865,1.466-2.189c0.201,1.324,0.62,2.189,1.463,2.189c1.406,0,2.545-2.375,2.545-5.307c0-0.66-0.061-1.291-0.168-" +
            "1.873c0.598,0.574,1.174,0.803,1.725,0.496C22.602,21.541,22.443,18.912,21.021,16.349zM15.808,13.757c2.362,0,4.278-1.916,4.278-" +
            "4.279s-1.916-4.279-4.278-4.279c-2.363,0-4.28,1.916-4.28,4.279S13.445,13.757,15.808,13.757z";

        var point = new esri.geometry.Point({
            longitude: lon,
            latitude: lat
        });
        var mark = new esri.Graphic(point, createSymbol(markPath, markColor));
        map.graphics.add(mark);
        markColor = "0a9242";
    }

    //Function to create symbol on click
    function createSymbol(path, color) {
        map.graphics.clear();
        var markerSymbol = new esri.symbol.SimpleMarkerSymbol();
        markerSymbol.setPath(path);
        markerSymbol.setColor(new dojo.Color(color));
        markerSymbol.setSize(23);
        markerSymbol.setOutline(null);
        return markerSymbol;
    }

    function formatearTexto(t) {
        if (t == undefined || t == ' (+/- 10 mts)') {
            t = 'Sin informacion';
        }
        return t;
    }




    //function agregarPuntoMapa(latitud, longitud) {
    //    if (!latitud || !longitud) return;

    //    var punto = new Point(longitud, latitud);
    //    var symbol = new SimpleMarkerSymbol()
    //        .setStyle(SimpleMarkerSymbol.STYLE_CIRCLE)
    //        .setColor(new Color([255, 0, 0, 0.5]));

    //    var pointGraphic = new Graphic(punto, symbol);
    //    map.graphics.clear();
    //    map.graphics.add(pointGraphic);
    //    map.centerAt(punto);
    //}

    //// Detectar cambios en las cajas de texto y actualizar el mapa
    //function detectarCambioCoordenadas() {
    //    var latitud = parseFloat($("#LATITUD_CASO").val());
    //    var longitud = parseFloat($("#LONGITUD_CASO").val());
    //    if (!isNaN(latitud) && !isNaN(longitud)) {
    //        agregarPuntoMapa(latitud, longitud);
    //    }
    //}

    //$("#LATITUD_CASO, #LONGITUD_CASO").on("change", detectarCambioCoordenadas);



    //function buscarDireccion() {
    //    var direccion = document.getElementById("direccionmapa").value;
    //    if (!direccion) return;

    //    var url = `https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/findAddressCandidates?SingleLine=${encodeURIComponent(direccion)}&f=json&outFields=Match_addr,Addr_type,StAddr,City`;

    //    fetch(url)
    //        .then(response => response.json())
    //        .then(data => {
    //            if (data.candidates.length > 0) {
    //                var mejorCoincidencia = data.candidates[0];
    //                var latitud = mejorCoincidencia.location.y;
    //                var longitud = mejorCoincidencia.location.x;
    //                $("#LATITUD_CASO").val(latitud).trigger("change");
    //                $("#LONGITUD_CASO").val(longitud).trigger("change");
    //                agregarPuntoMapa(latitud, longitud);
    //            } else {
    //                alert("No se encontró la dirección.");
    //            }
    //        })
    //        .catch(error => console.error("Error en la búsqueda de dirección:", error));
    //}



    function detectarCambioCoordenadasCampo() {
        var lat = parseFloat($("#LATITUD_CASO").val());
        var lon = parseFloat($("#LONGITUD_CASO").val());
        if (!isNaN(lat) && !isNaN(lon) && !(lat === 0 && lon === 0)) {
            makeMarker(lon, lat);
            map.centerAt(new esri.geometry.Point({ longitude: lon, latitude: lat }));
        }
    }

    $("#LATITUD_CASO, #LONGITUD_CASO").on("change", detectarCambioCoordenadasCampo);




    function executeIdentifyTask(evt) {
        mapPoint = evt.mapPoint;
        latDelito = evt.mapPoint.getLatitude();
        lonDelito = evt.mapPoint.getLongitude();

        var qtMpio = new esri.tasks.QueryTask("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/3");
        var qMpio = new esri.tasks.Query();

        qMpio.returnGeometry = false;

        qMpio.outFields = ['CODIGO'];

        var qGeom = new esri.geometry.Point({
            longitude: lonDelito,
            latitude: latDelito
        });

        qMpio.geometry = qGeom;

        var mpio_ = qtMpio.execute(qMpio);

        all([mpio_]).then(function (results) {
            if (results[0].features[0].attributes.CODIGO == code) {
                //Direccion
                locator.locationToAddress(mapPoint, 10, direccionObtenida, errorDireccionObtenida);

                //Cuadrantes AGOL
                var qtCuadrantes = new esri.tasks.QueryTask("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/11");
                var qCuadrantes = new esri.tasks.Query();

                /* Definimos las tareas de consulta para cada una de las capas dell servicio en AGOL */

                //Layer Municipios
                var qtMunicipios = new esri.tasks.QueryTask("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/3");
                var qMunicipios = new esri.tasks.Query();

                //Layer Jurisdiccion_Estaciones
                var qtJEstaciones = new esri.tasks.QueryTask("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/9");
                var qJEstaciones = new esri.tasks.Query();

                //Layer Barrios
                var qtBarrios = new esri.tasks.QueryTask("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/4");
                var qBarrios = new esri.tasks.Query();

                //Layer Cuadrantes Rurales
                var qtCRurales = new esri.tasks.QueryTask("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/12");
                var qCRurales = new esri.tasks.Query();

                //Layer Metropolitantas y Departamentos
                var qtMetros = new esri.tasks.QueryTask("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/7");
                var qMetros = new esri.tasks.Query();

                //qEstaciones.returnGeometry = false;
                qCuadrantes.returnGeometry = false;
                qMunicipios.returnGeometry = false;
                qJEstaciones.returnGeometry = false;
                qBarrios.returnGeometry = false;
                qCRurales.returnGeometry = false;
                qMetros.returnGeometry = false;


                qCuadrantes.outFields = ['CODIGO_SIEDCO', 'NRO_CUADRANTE']; //['CODIGO_SIEDCO'];
                qJEstaciones.outFields = ['CODIGO_SIEDCO'];
                qMunicipios.outFields = ['CODIGO', 'NOMBRE'];
                //qJEstaciones.outFields = ['CODIGO_SIEDCO'];
                qBarrios.outFields = ['NOMBRE'];
                qCRurales.outFields = ['SIEDCO'];
                qMetros.outFields = ['NOMBRE'];

                var qGeom = new esri.geometry.Point({
                    longitude: lonDelito,
                    latitude: latDelito
                });

                qMunicipios.geometry = qGeom;
                qJEstaciones.geometry = qGeom;
                qBarrios.geometry = qGeom;
                qCRurales.geometry = qGeom;
                qCuadrantes.geometry = qGeom;
                qMetros.geometry = qGeom;

                var municipio_ = qtMunicipios.execute(qMunicipios);
                var estacion_ = qtJEstaciones.execute(qJEstaciones);
                var barrio_ = qtBarrios.execute(qBarrios);
                var crural_ = qtCRurales.execute(qCRurales);
                var cuadrante_ = qtCuadrantes.execute(qCuadrantes);
                var metros_ = qtMetros.execute(qMetros);

                all([estacion_, cuadrante_, municipio_, barrio_, crural_, metros_]).then(function (results) {
                    makeMarker(lonDelito, latDelito);
                    //display a message so user knows something is happening
                    //domAttr.set(dom.byId("enviarInformacionLink"), "innerHTML", "Buscando informaci\xf3n...");
                    var codigoSiedco = "";
                    if (results[0].hasOwnProperty("features")) {
                        if (results[0].features.length > 0) {
                            codigoSiedco = "Cod SIEDCO Estacion: " + results[0].features[0].attributes.CODIGO_SIEDCO;
                        } else if (results[1].hasOwnProperty("features")) {
                            if (results[1].features.length > 0) {
                                codigoSiedco = "Cod SIEDCO Cuadrante: " + results[1].features[0].attributes.CODIGO_SIEDCO;
                            }
                        }
                    }
                    //estacion_
                    if (results[0].hasOwnProperty("features")) {
                        if (results[0].features.length > 0) {
                            cod_estacion = results[0].features[0].attributes.CODIGO_SIEDCO;
                        }
                    }
                    //cuadrante_
                    if (results[1].hasOwnProperty("features")) {
                        if (results[1].features.length > 0) {
                            numCuadrante = results[1].features[0].attributes.CODIGO_SIEDCO;
                            numCuadrante1 = results[1].features[0].attributes.NRO_CUADRANTE;
                        }
                    }

                    //municipio_		
                    if (results[2].hasOwnProperty("features")) {
                        if (results[2].features.length > 0) {
                            ent_administrativa = results[2].features[0].attributes.CODIGO;
                            nom_municipio = results[2].features[0].attributes.NOMBRE;
                        }
                    }

                    //barrio_		
                    if (results[3].hasOwnProperty("features")) {
                        if (results[3].features.length > 0) {
                            barrio = results[3].features[0].attributes.NOMBRE;
                        }
                    }

                    //crural_
                    if (results[4].hasOwnProperty("features")) {
                        if (results[4].features.length > 0) {
                            cua_rural = results[4].features[0].attributes.SIEDCO;
                        }
                    }

                    //metros_
                    if (results[5].hasOwnProperty("features")) {
                        if (results[5].features.length > 0) {
                            nom_metro = results[5].features[0].attributes.NOMBRE;
                        }
                    }

                    strInformacion = "";
                    if (numCuadrante1 != undefined)
                        strInformacion += "_nroCuadrante=" + numCuadrante1;
                    if (ent_administrativa != undefined)
                        strInformacion += "&_codDane=" + ent_administrativa;
                    if (cod_estacion != undefined)
                        strInformacion += "&_codEstacion=" + cod_estacion;
                    if (barrio != undefined)
                        strInformacion += "&_barrio=" + barrio;
                    if (numCuadrante != undefined)
                        strInformacion += "&_cuadrante=" + numCuadrante;
                    if (latDelito != undefined)
                        $("#LATITUD_CASO").val(latDelito);
                    if (lonDelito != undefined)
                        $("#LONGITUD_CASO").val(lonDelito);
                    if (strAddress != undefined)
                        //$("#DIRE_LLAMANTE").val(strAddress)
                        //if (cua_rural != undefined)
                        //    strInformacion += "&SiedcoCua_Rural=" + cua_rural;
                        if (nom_municipio != undefined)
                            strInformacion += "&_nomMunicipio=" + nom_municipio;


                });
            } else if (results[0].features[0].attributes.CODIGO != code) {
                alert("El punto no se encuentra en el municipio seleccionado");
            };
        });
    };
}, function (error) {
    // Esto se ejecutará si hay un error en la carga de algún módulo
    console.log("Error al cargar los módulos:", error);
});

function iniciarMapa() {
    if (window.map) {
        setTimeout(function () {
            map.resize();
            map.reposition();
            if (typeof code !== "undefined") {
                municipiosExtent(code);
            }
        }, 200);
    } else {
        console.warn("El mapa aún no se ha inicializado.");
    }
}