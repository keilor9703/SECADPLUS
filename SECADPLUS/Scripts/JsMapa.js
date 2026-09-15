


require([
    "esri/map",
   
    "esri/geometry/Point",
    "esri/symbols/SimpleMarkerSymbol",
    "esri/graphic",
    "esri/Color",
    "esri/geometry/Extent",
    "esri/layers/FeatureLayer",
    "esri/tasks/query",
    "esri/symbols/SimpleFillSymbol",
    "esri/symbols/SimpleLineSymbol",
    
    "dojo/domReady!"
], function (
    Map, Point, SimpleMarkerSymbol, Graphic, Color, Extent, FeatureLayer, Query, SimpleFillSymbol, SimpleLineSymbol, 
) {

    var map = new Map("map", {
            center: [-74.0721, 4.7110], // Bogotá
            zoom: 0,
            basemap: "streets"
    });







    window.map = map;
    window.currentCodigoDane = null;
    window.mapaListo = false;   // <-- flag nuevo

        var mpioLayer = new FeatureLayer("https://services3.arcgis.com/8cBoM4o6pnuUb1z1/ArcGIS/rest/services/SIDENCO_SinMalla/FeatureServer/3", {
            mode: FeatureLayer.MODE_ONDEMAND,
            outFields: ["*"]
        });

        var polygonHighlightSymbol = new SimpleFillSymbol(
            SimpleFillSymbol.STYLE_SOLID,
            new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0]), 1),
            new Color([125, 125, 125, 0.35])
        );

        function actualizarMapa(extent) {
            if (extent) {
                map.setExtent(new Extent(extent));
            }
        }

        function municipiosExtent(code) {
            if (!code) {
                console.warn("Código de municipio inválido:", code);
                return;
            }

            console.log("Buscando municipio con código:", code);
            map.graphics.clear();
            var query = new Query();
            query.where = "CODIGO='" + code + "'";
            query.returnGeometry = true;

            mpioLayer.queryFeatures(query, function (featureSet) {
                if (!featureSet || featureSet.features.length === 0) {
                    console.warn("No se encontraron municipios con el código:", code);
                    return;
                }

                var geom = featureSet.features[0].geometry;
                var gra = new Graphic(geom, polygonHighlightSymbol);

                try {
                    var extent = geom.getExtent();
                    if (!extent) {
                        throw new Error("La geometría no tiene extensión válida.");
                    }

                    map.graphics.add(gra);
                    map.setExtent(extent.expand(1.5)); // Expandimos el área para mejor visualización
                } catch (error) {
                    console.error("Error al manejar la geometría:", error);
                }
            }, function (error) {
                console.error("Error en la consulta de municipios:", error);
            });
        }

        //function mapReady() {
        //    if (typeof codigoDaneInicial !== "undefined" && codigoDaneInicial) {
        //        console.log("Código DANE inicial:", codigoDaneInicial);
        //        municipiosExtent(codigoDaneInicial);
        //    } else {
        //        console.warn("codigoDaneInicial no está definido o es inválido.");
        //    }
        //}



        function mapReady() {
            if (typeof codigoDaneInicial !== "undefined" && codigoDaneInicial) {
                console.log("Código DANE inicial:", codigoDaneInicial);
                window.currentCodigoDane = codigoDaneInicial;
                municipiosExtent(codigoDaneInicial);
            } else {
                console.warn("codigoDaneInicial no está definido o es inválido.");
            }
        }

        // Función para centrar el mapa en un municipio
        window.CentrarMapaPorMunicipio = function (_codDane) {
            if (!_codDane) {
                console.warn("Código DANE inválido en CentrarMapaPorMunicipio.");
                return;
            }
            console.log("Centrando mapa en municipio con código:", _codDane);
            window.currentCodigoDane = _codDane;
            municipiosExtent(_codDane);
        };

    map.on("load", mapReady);




    // iniciarMapa AHORA VIVE AQUÍ ADENTRO
    window.iniciarMapa = function () {
        if (window.mapaListo && map && typeof map.resize === "function") {
            setTimeout(function () {
                map.resize();
                map.reposition();
                if (window.currentCodigoDane) {
                    municipiosExtent(window.currentCodigoDane);
                }
            }, 200);
        } else {
            console.warn("El mapa aún no se ha inicializado; reintenta en un momento.");
        }
    };



        map.on("click", function (event) {
            var latitud = event.mapPoint.getLatitude();
            var longitud = event.mapPoint.getLongitude();

            //$("#LATITUD_CASO").val(latitud);
            //$("#LONGITUD_CASO").val(longitud);
            $("#LATITUD_CASO").val(latitud).trigger("change");
            $("#LONGITUD_CASO").val(longitud).trigger("change");

            var symbol = new SimpleMarkerSymbol()
                .setStyle(SimpleMarkerSymbol.STYLE_CIRCLE)
                .setColor(new Color([255, 0, 0, 0.5]));

            var pointGraphic = new Graphic(new Point(event.mapPoint.x, event.mapPoint.y, map.spatialReference), symbol);

            map.graphics.clear();
            map.graphics.add(pointGraphic);
    });

        $("#direccionmapa").keypress(function (e) {
            if (e.which == 13) {
                buscarDireccion();
            }
        });

        
        function agregarPuntoMapa(latitud, longitud) {
            if (!latitud || !longitud) return;

            var punto = new Point(longitud, latitud);
            var symbol = new SimpleMarkerSymbol()
                .setStyle(SimpleMarkerSymbol.STYLE_CIRCLE)
                .setColor(new Color([255, 0, 0, 0.5]));

            var pointGraphic = new Graphic(punto, symbol);
            map.graphics.clear();
            map.graphics.add(pointGraphic);
            map.centerAt(punto);
        }

        // Detectar cambios en las cajas de texto y actualizar el mapa
        function detectarCambioCoordenadas() {
            var latitud = parseFloat($("#LATITUD_CASO").val());
            var longitud = parseFloat($("#LONGITUD_CASO").val());
            if (!isNaN(latitud) && !isNaN(longitud)) {
                agregarPuntoMapa(latitud, longitud);
            }
        }

        $("#LATITUD_CASO, #LONGITUD_CASO").on("change", detectarCambioCoordenadas);


        function buscarDireccion() {
            var direccion = document.getElementById("direccionmapa").value;
            if (!direccion) return;

            var url = `https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/findAddressCandidates?SingleLine=${encodeURIComponent(direccion)}&f=json&outFields=Match_addr,Addr_type,StAddr,City`;

            fetch(url)
                .then(response => response.json())
                .then(data => {
                    if (data.candidates.length > 0) {
                        var mejorCoincidencia = data.candidates[0];
                        var latitud = mejorCoincidencia.location.y;
                        var longitud = mejorCoincidencia.location.x;
                        $("#LATITUD_CASO").val(latitud).trigger("change");
                        $("#LONGITUD_CASO").val(longitud).trigger("change");
                        agregarPuntoMapa(latitud, longitud);
                    } else {
                        alert("No se encontró la dirección.");
                    }
                })
                .catch(error => console.error("Error en la búsqueda de dirección:", error));
        }

            

          



        map.addLayer(mpioLayer);
});
