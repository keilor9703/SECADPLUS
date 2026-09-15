var Global_Menu = "";
var Global_DatosNotificacion = "";
var ItervalInicio = null;
var glMostrarEmergente = "False";
var glMostrarGlobo = "False";
var focoActivo= null;
function notificar(codigo) {
    if (codigo != "") {
        $.ajax({
            url: actualizarNotificacionUrl,
            method: "POST",
            content: "application/json; charset=utf-8",
            dataType: 'html',
            data: { strCodigosNotificacion: codigo },
            success: function (resultado) {
                //$("#codigosNotificaciones").val("");
                $("#notifica_" + codigo).parent().parent().parent().empty();
                $("#notifica_" + codigo).parent().parent().parent().remove();
                $("#notifica_globo_" + codigo).parent().parent().empty();
                $("#notifica_globo_" + codigo).parent().parent().remove();
                var valor = $("#contador").html();
                if(valor>0){
                    valor = valor - 1;                
                }                
                $("#contador").html((valor));                
            }
        });
    }
}

$(function () {    
    //Presenta por consola la versión de la aplicación
    SolicitarPermisosNotificaciones();
    //console.log("SECAD PLUS VERSION  2.6.7 FECHA:30/06/2017");
    console.log("SECAD SISEC");
    //PREVENIR inspector de elementos
    //$(document).keydown(function (e) {
    //    if (e.which === 123) {
    //        return false;
    //    }
    //});
    //$(document).on("contextmenu", function () {
    //    return false;
    //});
    //$('body').on('keydown', 'input, select, textarea', function (e) {
    //    var self = $(this)
    //      , form = self.parents('form:eq(0)')
    //      , focusable
    //      , next
    //    ;
    //    if (e.keyCode == 13) {
    //        focusable = form.find('input,a,select,button,textarea').filter(':visible');
    //        next = focusable.eq(focusable.index(this) + 1);
    //        if (next.length) {
    //            next.focus();
    //        } else {
    //            form.submit();
    //        }
    //        return false;
    //    }
    //});
    $(".collapse_menu_secad").click(function () {
        $(this).toggleClass("rotate-180")
          .animate({
              transition: "all ease-in-out 0.2s"
          });
    });
    //Carga el menú principal de la aplicación
    $.ajax(
          {
              url: menuUrl,
              method: "POST",
              dataType: "html",
              cache: true,
              success: function (data) {
                  Global_Menu = data;
                  $('#menuOpciones').append(data);
              }
          }).done(function () {
              
          });
});

function SolicitarPermisosNotificaciones() {
    //solicitar permisos para desplegar notificaciones.
    if (!("Notification" in window)) {
        //alert("Su navegador no permite activar notificaciones externas");
    } else {
        if (Notification.permission !== "denied") {
            Notification.requestPermission(function (permission) {
                if (permission === "granted") {
                    //var notification = new Notification("Las notificaciones se encuentran activadas");
                }
            });
        }
    }
}

//Permite gestionar mensajes de alertas en toda la aplicación para peticiones desde javascript
function GestionarAlerta(tipo, mensaje) {
    $("#alertaJs").empty();
    $("#alertaJs").html(alertaJs.replace("CUSTOM", tipo).replace("MENSAJE", mensaje));
    $("#back-to-top").trigger("click");
}
function GestionarAlertaModal(tipo, mensaje, contenedor) {
    $("#" + contenedor).empty();
    $("#" + contenedor).html(alertaJs.replace("CUSTOM", tipo).replace("MENSAJE", mensaje));
}
function EliminarRegistroGeneral(ruta) {
    url = ruta;
    $.ajax(
     {
         url: url,
         method: "POST",
         dataType: "html",
         success: function (data) {
             //  console.log(data);
             swal({
                 title: "",
                 text: data,
                 type: "warning",
                 html: true,
                 showCancelButton: true,
                 confirmButtonColor: "#EF5350",
                 cancelButtonColor: "#EF5385",
                 confirmButtonText: "Eliminar",
                 cancelButtonText: "Cancelar",
                 closeOnConfirm: false,
                 closeOnCancel: true
             },
             function (isConfirm) {
                 if (isConfirm) {
                     $("#form_eliminar").submit();
                 }
             }
             );
         },
         error: function (data) {
             console.log(data);
         }
     });
}
function EliminarRegistroGeneralAjax(ruta, datos, nombreElemento) {
    url = ruta;
    swal({
        title: "¿Esta seguro de eliminar el registro?",
        text: nombreElemento,
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
                url: ruta,
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
                } else {
                    //implementar cuando existan problemas
                    GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
                }
            });
        }
    }
    );
}
function EliminarRegistroAjax(ruta) {
    $.ajax({
        type: "POST",
        url: ruta,
        content: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        }
    }).done(function (resultado) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
        if (resultado.result) {
            GestionarAlerta("success", "Consulta exitosa");
        } else {
            //implementar cuando existan problemas
            GestionarAlerta("warning", "Ocurrio un problema." + resultado.datos);
        }
    });
}
//Permite desplegar la opción de eleminar a través de un modal
function EliminarRegistroModal(ruta) {
    bootbox.confirm("¿Esta seguro de eliminar este registro?", function (resultado) {
        if (resultado) {
            //realiza la eliminación
            $.ajax({
                type: "POST",
                url: ruta,
                content: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {

                    if (data.result) {
                        GestionarAlerta("success", "Registro Eliminado de manera exitosa.");
                        location.reload();
                    } else {
                        GestionarAlerta("warning", "Ocurrio un problema." + data.datos);
                    }
                }
            });
        }
    });
}
//Permite activar el loader sobre un panel especifíco
function ActivarLoader(id) {
    var block = $("#" + id);

    $(block).block({
        message: '<i class="icon-spinner2 spinner"></i>',
        overlayCSS: {
            backgroundColor: '#fff',
            opacity: 0.8,
            cursor: 'wait',
            'box-shadow': '0 0 0 1px #ddd'
        },
        css: {
            border: 0,
            padding: 0,
            backgroundColor: 'none'
        }
    });
}
function ActivarLoaderAvanzado(contenedor) {
    var block = $(contenedor);
    focoActivo = $(':focus');
    $(focoActivo).blur();
    $(block).block({
        message: '<i class="icon-spinner2 spinner"></i>',
        overlayCSS: {
            backgroundColor: '#fff',
            opacity: 0.8,
            cursor: 'wait',
            'box-shadow': '0 0 0 1px #ddd'
        },
        css: {
            border: 0,
            padding: 0,
            backgroundColor: 'none'
        }
    });
}
function InactivarLoader(id) {
    //console.log("destruye loader");
    var block = $("#" + id);
    $(block).unblock();
}
function InactivarLoaderAvanzado(contenedor) {
    //console.log("destruye loader");
    var block = $(contenedor);
    $(block).unblock();
    $(focoActivo).focus();
}
//Permite aplicar el control para seleccion de fecha a un elemento
function AplicarControlFecha(identificador, fecha) {
    //Configuración personalizada para los controles.
    var parametros = {
        selectYears: 20,
        selectMonths: true,
        format: 'dd/mm/yyyy',
        formatSubmit: 'dd/mm/yyyy',
        monthsFull: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
        weekdaysShort: ['Dom', 'Lun', 'Mar', 'Mier', 'Jue', 'Vien', 'Sab'],
        weekdaysFull: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sabádo'],
        labelMonthNext: 'Siguiente Mes',
        labelMonthPrev: 'Mes Anterior',
        labelMonthSelect: 'Seleccione un mes',
        labelYearSelect: 'Seleccione un año',
        today: 'Hoy',
        close: 'Cerrar',
        clear: 'Limpiar'
    };
    $(identificador).pickadate(parametros);
    if (fecha != null && fecha != "") {
        var control = $(identificador).pickadate();
        // Use the picker object directly.
        var selector = control.pickadate('picker');
        selector.set('select', fecha, { format: 'dd/mm/yyyy' })
    }
}
//Permite aplicar el control para seleccion de fecha a un elemento
function AplicarControlFechaLimiteMax(identificador, fecha) {
    //Configuración personalizada para los controles.
    var parametros = {
        selectYears: 20,
        selectMonths: true,
        format: 'dd mmm, yyyy',
        formatSubmit: 'dd/mm/yyyy',
        monthsFull: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
        weekdaysShort: ['Dom', 'Lun', 'Mar', 'Mier', 'Jue', 'Vien', 'Sab'],
        weekdaysFull: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sabádo'],
        labelMonthNext: 'Siguiente Mes',
        labelMonthPrev: 'Mes Anterior',
        labelMonthSelect: 'Seleccione un mes',
        labelYearSelect: 'Seleccione un año',
        today: 'Hoy',
        close: 'Cerrar',
        clear: 'Limpiar',
        max: fecha,
    };
    $(identificador).pickadate(parametros);
}
function AplicarDateRangePicker(identificador) {
    $(identificador).daterangepicker({
        "autoApply": true,
        "showDropdowns": true,
        "locale": {
            "format": "DD/MM/YYYY",
            "separator": "-",
            "applyLabel": "Aplicar",
            "cancelLabel": "Cancelar",
            "fromLabel": "Desde",
            "toLabel": "Hasta",
            "customRangeLabel": "Personalizado",
            "weekLabel": "W",
            "daysOfWeek": [
                "Do",
                "Lu",
                "Ma",
                "Mi",
                "Ju",
                "Vi",
                "Sa"
            ],
            "monthNames": [
                "Enero",
                "Febrero",
                "Marzo",
                "Abril",
                "Mayo",
                "Junio",
                "Julio",
                "Agosto",
                "Septiembre",
                "Octubre",
                "Noviembre",
                "Diciembre"
            ],
            "firstDay": 1
        },
    });
}
function CambiarVisibilidadMenu() {
    $("#botonMenu").trigger("click");
}

function AplicarFechaHoraPlugin(identificador) {
    $(identificador).datetimepicker({
        format: 'DD/MM/YYYY HH:mm:ss',
        locale: 'es',
        //minDate : new Date(),
        showClear: true
    });
}
function AplicarFechaHoraCustom(identificador, fecha) {
    $(identificador).datetimepicker({
        format: 'DD/MM/YYYY HH:mm:ss',
        locale: 'es',
        defaultDate: fecha,
        showClear: true
    });
}

function ConsultarHoraServidor() {
    var mensaje = "<i class='icon-alarm'></i> Última fecha y hora consultada de la BD: "
    var urlFinal = urlTiempoServidor.replace(/amp;/g, "");

    $.ajax({
        method: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {

        },
        success: function (data) {
            if (data.result) {
                $("#principalReloj").html(mensaje + data.datos);
            } else {
                $("#principalReloj").html("No se ha podido cargar la fecha y hora del servidor");
                GestionarAlertaModal("warning", data.datos);
            }
        },
        error: function () {
            //console.log("Error");
        }
    }).done(function (data) {

    });
}
/////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////Permite construir el breadcumb del sitio//////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////
function GenerarBreadCrumb() {
    var UrlInicial = "<li><a href='" + urlInicialBreadCrumb + "'><i class='icon-home2 position-left'></i>Inicio</a></li>"
    var UrlComplemento = "<li><a href='" + UrlComplementoRuta + "'>" + UrlNombreUbicacion + "</a></li>";
    $("[name=contenedorBreadCrumb]").html(UrlInicial + UrlComplemento);
    
}

function GenerarTutorial() {
    try {
        
        var urlLimpia = UrlComplementoRuta.split('?')[0];
        var urlVideos = "";
        var urlParticion = urlLimpia.split('/');
        if (urlParticion.length > 3) {        
        var tamañoUrl = urlParticion.length;
        console.log("particion" + urlParticion);
            urlVideos = "/" + urlParticion[tamañoUrl - 2] + "/" + urlParticion[tamañoUrl - 1] + "/" + "Tutorial.mp4";
        } else {
            urlVideos = urlLimpia + "/" + "Tutorial.mp4";
        }
        console.log("url " + urlComprobarVideos);
        console.log("video: " + urlVideos);
        $.ajax({
            type: 'POST',
            url: urlComprobarVideos,
            dataType: 'json',
            data: { strUrl: urlVideos },
            success: function (resultado) {

                if (resultado.result == true) {
                    var urlTutotial = (urlCargarVideos + "?strRutaVideo=" + urlVideos);
                    var UrlInicial = " / <li><a href='" + urlTutotial + "'  target='_blank' ><i class='icon-book position-right'> Ayuda</i></a></li>";
                    $("[name=contenedorTutoriales]").html(UrlInicial);
                }
            },
            error: function (ex) {
                GestionarAlerta("warning", ex);
            }
        });
    } catch (e) {
        console.log(e);
    }
}