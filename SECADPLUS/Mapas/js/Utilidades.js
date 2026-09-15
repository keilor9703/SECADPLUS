//////////////////// UTILIDADES DE GESTIÓN DE LA PÁGINA Y NAVEGACIÓN ///////////////////////////
function MaysPrimera(string) {
    //return string.toUpperCase();
    return string.charAt(0).toUpperCase() + string.slice(1).toLowerCase();
}

function RecargarPagina(timeout) {
    if (timeout == null) {
        timeout = 3000;
    }
    setTimeout(function () {
        window.location.reload();
    }, timeout);
}

function RedireccionarPagina(Area, Controlador, Accion, Id, Timeout) {
    if (Timeout == null) {
        Timeout = 3000;
    }
    setTimeout(function () {
        var url = GenerarUrl(Area, Controlador, Accion, "?q=" + Id);
        window.location = url;
    }, Timeout);
}

function EnviarPost(Area, Controlador, Accion) {
    var url = GenerarUrl(Area, Controlador, Accion);
    $("#form").attr("action", url);
    $("#form").submit();
}

function GenerarUrl(Area, Controlador, Accion, Id) {
    var url = $("#form").attr("action");
    var a = document.createElement('a');
    a.href = url;
    var puerto = "";
    if (a.port != 80) {
        puerto = ":" + a.port;
    }

    var retorno = a.protocol + "//" + a.hostname + puerto + "/" + RutaDeploy + "/";
    if (Area == null || Area == "") {
        Area = $("#Vista_Area").val();
    }
    if (Controlador == null || Controlador == "") {
        Controlador = $("#Vista_Controlador").val();
    }
    if (Accion == null || Accion == "") {
        Accion = $("#Vista_Accion").val();
    }
    retorno += Area + "/";
    retorno += Controlador + "/";
    retorno += Accion + "/";
    if (Id != null && Id != "") {
        retorno += Id;
    }
    console.log(retorno);
    return retorno;
}


function MostrarContenidoJson(elemento) {
    var contenido = $(elemento).text();
    var obj = JSON.parse(contenido);
    var textoMejorado = JSON.stringify(obj, undefined, 4);
    $(elemento).text(textoMejorado);
}
$(document).ready(function () {
    $("pre").each(function (indice, elemento) {
        MostrarContenidoJson(elemento);
    });
});
