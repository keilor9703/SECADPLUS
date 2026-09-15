require([
    "dojo/parser",
    "esri/dijit/BasemapGallery",
    "esri/dijit/LayerList",
    "esri/dijit/LocateButton",
    "dojo/ready",
    "dojo/on",
    "esri/dijit/Search",
    "dijit/layout/BorderContainer",
    "dijit/layout/ContentPane",
    "dojo/dom",
    "esri/map",
    "esri/urlUtils",
    "esri/arcgis/utils",
    "esri/dijit/Legend",
    "esri/dijit/Scalebar",
    "esri/geometry/Point",
    "esri/graphic",
    "esri/symbols/SimpleMarkerSymbol",
    "dojo/domReady!"
], function (
    parser,
    BasemapGallery,
    LayerList,
    LocateButton,
    ready,
    on,
    Search,
    BorderContainer,
    ContentPane,
    dom,
    Map,
    urlUtils,
    arcgisUtils,
    Legend,
    Scalebar,
    Point,
    Graphic,
    SimpleMarkerSymbol
) {
        ready(function () {

            parser.parse();

            //if accessing webmap from a portal outside of ArcGIS Online, uncomment and replace path with portal URL
            //arcgisUtils.arcgisUrl = "https://pathto/portal/sharing/content/items";
            arcgisUtils.createMap("da89bd5e69024aca92bc36e5908b3233", "map").then(function (response) {

                var map = response.map;

                //Función para mostrar coordenadas en el mapa - Función para convertir coordenadas decimales a grados
                function showCoordinates(event) {
                    mapCoor = esri.geometry.webMercatorToGeographic(event.mapPoint);
                    dojo.byId("spnCoordinates").innerHTML = "Lat: " + converCoor(mapCoor.y) + " , Lon: " + converCoor(mapCoor.x);
                }

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

                map.on("mouse-move", showCoordinates);


                // Habilitación de opción para buscar direcciones
                var search = new Search({
                    map: response.map,
                    countryCode: 'COL,CO'

                }, "search");
                search.startup();

                //Agregar la escala
                var scalebar = new Scalebar({
                    map: map,
                    scalebarUnit: "english"
                });

                //Lista base de mapas
                var basemapGallery = new BasemapGallery({
                    showArcGISBasemaps: true,
                    map: map
                }, "basemapGallery");
                basemapGallery.startup();

                basemapGallery.on("error", function (msg) {
                    console.log("Ha ocurrido un error cargando la Galeria de Mapas Base:  ", msg);
                });

                //Lista de las capas de cuadrante, cuadrante rural y malla vial
                var listLayers = new LayerList({
                    map: response.map,
                    showLegend: true,
                    showSubLayers: true,
                    showOpacitySlider: true,
                    layers: arcgisUtils.getLayerList(response)
                }, "layerList");
                listLayers.startup();

                //Opción para obtener la ubicación actual
                geoLocate = new LocateButton({
                    map: response.map
                }, "LocateButton");
                geoLocate.startup();


                //Parametros longitud y latitud que recibe del botón "Ver"
                var longitud = parseFloat(document.getElementById('idlong').value);
                var latitud = parseFloat(document.getElementById('idlat').value);

                // Simbolo Muñeco
                var markPath = "M21.021,16.349c-0.611-1.104-1.359-1.998-2.109-2.623c-0.875,0.641-1.941,1.031-3.103,1.031c-1.164,0-2.231-" +
                    "0.391-3.105-1.031c-0.75,0.625-1.498,1.519-2.111,2.623c-1.422,2.563-1.578,5.192-0.35,5.874c0.55,0.307,1.127,0.078,1.723-" +
                    "0.496c-0.105,0.582-0.166,1.213-0.166,1.873c0,2.932,1.139,5.307,2.543,5.307c0.846,0,1.265-" +
                    "0.865,1.466-2.189c0.201,1.324,0.62,2.189,1.463,2.189c1.406,0,2.545-2.375,2.545-5.307c0-0.66-0.061-1.291-0.168-" +
                    "1.873c0.598,0.574,1.174,0.803,1.725,0.496C22.602,21.541,22.443,18.912,21.021,16.349zM15.808,13.757c2.362,0,4.278-1.916,4.278-" +
                    "4.279s-1.916-4.279-4.278-4.279c-2.363,0-4.28,1.916-4.28,4.279S13.445,13.757,15.808,13.757z";

                // Color Simbolo Muñeco
                var markColor = "0a9242";

                // Creación de simbolo
                function createSymbol(path, color) {
                    var markerSymbol = new esri.symbol.SimpleMarkerSymbol();
                    markerSymbol.setPath(path);
                    markerSymbol.setColor(new dojo.Color(color));
                    markerSymbol.setSize(23);
                    markerSymbol.setOutline(null);
                    return markerSymbol;
                }

                // Punto basado en las coordenadas suministradas
                var point = new esri.geometry.Point({
                    longitude: longitud,
                    latitude: latitud
                });

                // Instancia Graphic
                var mark = new esri.Graphic(point, createSymbol(markPath, markColor));

                //Grafica el simbolo y centra la ubicación, respecto a las coordenadas suministradas
                map.graphics.add(mark);
                map.centerAndZoom(point, 14);
            });

        });

    });