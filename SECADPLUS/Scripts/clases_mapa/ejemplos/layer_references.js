//OBTIENE RUTAS CON NOMBRES Y URLS PARA ACCEDER A LOS LAYERS CON LOS CUALES DISPONE LA POLICÍA NACIONAL
function obtenerCapas(){
    var capas = new Array();
    
    //LAYER REFERENCE 1
    var capa_1 = new Array();
    capa_1["url"] = "https://gisponal.policia.gov.co/webadaptor/rest/services/CAPAS/CUADRANTES/MapServer";
    capa_1["nombre"] = "Jurisdicciones de cuadrante";
    capas.push(capa_1);
    
    //LAYER REFERENCE 2
    var capa_2 = new Array();
    capa_2["url"] = "https://gisponal.policia.gov.co/webadaptor/rest/services/CAPAS/CAI/MapServer";
    capa_2["nombre"] = "CAI";
    capas.push(capa_2);
    
    //LAYER REFERENCE 3
    var capa_3 = new Array();
    capa_3["url"] = "https://gisponal.policia.gov.co/webadaptor/rest/services/CAPAS/ESTACIONES/MapServer";
    capa_3["nombre"] = "Estación";
    capas.push(capa_3);
    
    //LAYER REFERENCE 4
    var capa_4 = new Array();
    capa_4["url"] = "https://gisponal.policia.gov.co/webadaptor/rest/services/CAPAS/DISTRITOS/MapServer";
    capa_4["nombre"] = "Distrito";
    capas.push(capa_4);
    
    //LAYER REFERENCE 5
    var capa_5 = new Array();
    capa_5["url"] = "https://gisponal.policia.gov.co/webadaptor/rest/services/CAPAS/METROS_Y_DPTOS/MapServer";
    capa_5["nombre"] = "Metros y Departamentos";
    capas.push(capa_5);
    
    //LAYER REFERENCE 6
    var capa_6 = new Array();
    capa_6["url"] = "https://gisponal.policia.gov.co/webadaptor/rest/services/CAPAS/REGIONALES/MapServer";
    capa_6["nombre"] = "Regionales";
    capas.push(capa_6);
    
    //LAYER REFERENCE 7
    var capa_7 = new Array();
    capa_7["url"] = "https://gisponal.policia.gov.co/webadaptor/rest/services/CAPAS/SECAD/MapServer";
    capa_7["nombre"] = "SECAD";
    capas.push(capa_7);
    
    return capas;
}
