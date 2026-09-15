var localBoton = "<button class='btn bg-teal btn-xs' type='button' data-popup='tooltip'" +                    
                               "onclick =\"location.href=' " + glUrlContestarMensaje + " '\" title='' data-placement ='bottom'>" +
                    " Contestar </button>";
var localConsecutivo = 0;
var localControlNuevos = false;
$(function () {
    //Control para evitar que la ventana emergente se despliegue de nuevo cuando no hay nuevos mensajes
    $("[name='nEmergentesCerrar']").click(function (){        
        $("[name='notificacionesEmergentes']").addClass("hidden");
        if (typeof sessionStorage.EmergerteCerrado === "undefined") {
            sessionStorage.EmergerteCerrado = "SI";
        }
        sessionStorage.EmergerteCerrado = "SI";
    });
    console.log("usuario = " + glUsuarioIdHub);

    //Generación de conexión sin el proxy autogenerado
    var ruta = glUrlHost + (menuUrl.replace("Home/Menu",""));
    var conexion = $.hubConnection(ruta+"/signalr", { useDefaultPath: false });
    //habilita loging de la conexion
    conexion.logging = false;
    //especifica parametros a enviar al servidor
    conexion.qs = {"ESTADONOTIFICA":"TRUE", "USUARIO":glUsuarioIdHub};
    var proxyHub = conexion.createHubProxy("NotificadorHub");
    //Registra handlers
    proxyHub.on('notificar', function (mensajes, emergente, noEmergente, cantidad) {
        //logica para presentar las notificaciones
        //Se almacena en la sesión de la pestaña activa del navegador el consecutivo del último mensaje notificado
        if (typeof sessionStorage.Consecutivo === "undefined") {
            sessionStorage.Consecutivo = 0;
        }
        //console.log("valor sessionstorage Inicial" + sessionStorage.Consecutivo);
        if (sessionStorage.Consecutivo < mensajes[0].Consecutivo) {
            console.log("nuevo mensaje");
            localControlNuevos = true;
            document.getElementById('SonidoAlerta').play();
            sessionStorage.Consecutivo = mensajes[0].Consecutivo;
        } else {
            sessionStorage.Consecutivo = mensajes[0].Consecutivo;
            localControlNuevos = false;
        }
        //console.log("valor sessionstorage" + sessionStorage.Consecutivo);
        
        //Verifica si deben presentarce mensajes de globo
        if (noEmergente) {
            CargarGlobo(mensajes, cantidad);
        } else {
            LimpiarNotificaciones(1);
        }
        //verifica si deben presentarse mensajes emergentes
        if(emergente){
            CargarEmergentes(mensajes);
        } else {
            LimpiarNotificaciones(2);
        }
    });
    proxyHub.on('limpiar', function () {
        //Logica para limpiar los mensajes
        LimpiarNotificaciones(1);
        LimpiarNotificaciones(2);
    });
    proxyHub.on('nuevo', function () {
        //Logica para limpiar los mensajes
        VerificarMensajeNuevo();
    });
    //Genera la conexión
    conexion.start().done(function () {
        //Eejcuta el inicio del notificador
        proxyHub.invoke('IniciarNotificador');
        console.log('Now connected, connection ID=' + conexion.id);
        console.log("Connected, transport = " + conexion.transport.name);
    })
    .fail(function () { console.log('Could not connect'); });
});

function CargarGlobo(mensajes, cantidad) {
    var contenidoHtml = "";
    if(mensajes.length===0){
        LimpiarNotificaciones(1);
    }
       
    //Construye el html de los mensajes
    $(mensajes).each(function () {        
        contenidoHtml += "<li class='media marginTopMediaCustom'>"
                                    + "<div class='media-left customSizeNoty'>"
                                    + "<button id='notifica_globo_"+this.Consecutivo+"' class='btn border-success  btn-flat text-success btn-rounded btn-icon btn-xs'  onclick='notificar(" + this.Consecutivo + ");'><i class='icon-checkmark4'></i></button>"
                                    + " </div>"
                                    + "<div class='media-body customSizeNotyLine'>"
                                    + "<a href = '#' class='media-heading'>"
                                    + "<span class='text-success'>Mensaje</span>"
                                    + "<span class='media-annotation pull-right'></span>"
                                    + "</a>"
                                    + "<span class='text-muted'>" + this.Mensaje + "</span>"
                                    + "<a href = '#' class='media-heading'>"
                                    + "<span class='text-success'></span>"
                                    + "<span class='media-annotation pull-right'></span>"
                                    + "</a>"
                                    + "<span class='text-muted'>" + localBoton + "</span>"
                                    + "</div>"
                                    + "</li>";
    });
    $("#contenedor").html(contenidoHtml);
    $("#contador").html(cantidad);

}

function CargarEmergentes(mensajes) {
    
    var htmlEmergente = "";
    //console.log((typeof sessionStorage.EmergerteCerrado === "undefined"));
    
    if (typeof sessionStorage.EmergerteCerrado ==="undefined") {
        sessionStorage.EmergerteCerrado = "NO";
    }
    //console.log("storage"+sessionStorage.EmergerteCerrado);
    //console.log("controlNuevos" + localControlNuevos);
    //console.log((sessionStorage.EmergerteCerrado === "SI"));
    if ((sessionStorage.EmergerteCerrado === "SI") && !localControlNuevos) {        
        return false;
    } else {
        //Construye el html de los mensajes
        $(mensajes).each(function () {
            htmlEmergente += "<div class='notify_message'>"
                                         + "<div class='row form-compact'>"
                                         + "<label class='col-xs-5 col-md-5'><button type='button' class='btn border-success btn-flat text-success btn-rounded btn-icon btn-xs' id='notifica_" + this.Consecutivo + "'  onclick='notificar(" + this.Consecutivo + ")'><i class='icon-checkmark4'></i></button> Mensaje:</label>"
                                         + "<span class='col-xs-7 col-md-7 label label-CustomNoty'>" + localBoton + "</span>"
                                         + "</div>"
                                         + "<div class='row form-compact'>"
                                         + "<span class='col-xs-10 col-md-12 label label-CustomNoty'>" + this.Mensaje + "</span>"
                                         + "</div><hr></div>";
        });
        $("[name='DragConetendorEmer']").html(htmlEmergente);
        $("[name='notificacionesEmergentes']").removeClass("hidden");
    }    
}

function LimpiarNotificaciones(opcion){
    if (opcion === 1) {
        $("#contador").html(0);
        $("#contenedor").empty();
    } else {
        $("[name='DragConetendorEmer']").empty();
        $("[name='notificacionesEmergentes']").addClass("hidden");
    }
}
function VerificarMensajeNuevo() {
    if (localControlNuevos) {
        desplegarNotificacionExterna();
        localControlNuevos = false;
    }

}
function desplegarNotificacionExterna(mensaje) {
    mensaje = typeof a !== 'undefined' ? a : "Tiene un nuevo mensaje";
    if (("Notification" in window) && Notification.permission === "granted") {
        // If it's okay let's create a notification
        var options = {
            body: "Revise SECADPLUS por nuevos mensajes.",
            icon: uri_icono_policia,
            requireInteraction: true
        }
        var notification = new Notification(mensaje, options);
    }
}