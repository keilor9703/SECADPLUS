var TipoLlamada="";
var CodigoGrupoD="";
var NombreGrupoD="";
var DevolviendoLlamada = false;

// adicionado para recepcion
var vSitioGraba = document.getElementById("CADPEDI_SITIOGRABA").value;
var vNumeLlamada = document.getElementById("CADPEDI_NUMELLAMADA").value;
var vUsuario = document.getElementById("USUA_MODIFICA").value;
var vEquipo = "";
var vAcd = "";
var vInfoGrabacion = "";
var vRecordId = "";
var vCallId = "";
var vTaskid = "";
var vCaller = "";
var vCalled = "";


function EnviarDatosGrabacionICP(pcadpedi_sitiograba, pcadpedi_numellamada, pacd, ptaskid, pcaller,precordid, pcalled, pcallid, pusersecad, pcadequipo) {
    console.log('Entró a EnviarDatosGrabacionICP ****');
    var Host = window.location.protocol + "//" + window.location.host;
    var Url = Host + '//Huawei//IncluirDatosGrabacionICP?p1=' + pcadpedi_sitiograba + "&p2=" + pcadpedi_numellamada + "&p3=" + pacd + "&p4=" + ptaskid + "&p5=" + pcaller + "&p6=" + precordid + "&p7=" + pcalled + "&p8=" + pcallid + "&p9=" + pusersecad + "&p10=" + pcadequipo;
    console.log('Envió  a base de datos los datos de grabacion ****' + Url);
    $.ajax({
        url: Url,
        method: "POST",
    }).done(function (Data) {
        console.log('Envió  a base de datos los datos de grabacion ****' + Data.Descripcion);
    }).fail(function (jqXHR, textStatus) {
        console.log(textStatus);
    });
}

function ObtenerMediosDisponiblesRadios(Codigo) {
    console.log('Entró a ConsPersMediosDispRadios ****');
    var Host = window.location.protocol + "//" + window.location.host;
    var SitioGrabacion = document.getElementById("CADPEDI_SITIOGRABA").value;
    var Url = Host + '//Huawei//ConsutarPersMediosDispRadios?p1sitio=' + SitioGrabacion + "&p2consMedio=" + Codigo;
    $.ajax({
        url: Url,
        method: "POST",
    }).done(function (Data) {
        if(Data.CodigoSalida == "00")
        {
            var Datos = Data.DatosSecad;
            var DatosTmp = JSON.parse(Datos);
            $('#DispositivosSeleccionados').empty();
            $.each(DatosTmp.V_DATOS,function(Indice,Valor){
                var Switch = '<input type="checkbox" %activo% data-elte="%elte%" data-on="Si" data-off="No" data-toggle="toggle" data-size="mini">';
            
                if(Valor.USUARIO_RADIO != null)
                {
                    Switch = Switch.replace('%activo%','');
                    Switch = Switch.replace('%elte%',Valor.USUARIO_RADIO);
                }
                else
                {
                    Switch = Switch.replace('%activo%','disabled');
                    Switch = Switch.replace('%elte%','');
                }

                var Linea = '<tr> \
                                <td>' + Switch + '</td> \
                                <td>' + Valor.NOMB_EMPLEADO + (Valor.USUARIO_RADIO == null ? '&nbsp; &nbsp;<i title="Este usuario no tiene una terminal eLTE asignada." class="icon-phone-slash text-danger"></i>':'') + '</td> \
                            </tr>'

                $('#DispositivosSeleccionados').append(Linea);        
            });

            var Checks = $('#DispositivosSeleccionados input:checkbox');
            $.each(Checks,function(Indice,Valor){
                $(Valor).bootstrapToggle({
                    on: 'Si',
                    off: 'No',
                    size: 'mini'
                  });
            });            
        }
    }).fail(function (jqXHR, textStatus) {
        console.log(textStatus);
    });
}

function CargarMediosDisponiblesRadios(Codigo) {
    var Resultado;
    var Host = window.location.protocol + "//" + window.location.host;
    var SitioGrabacion = document.getElementById("CADPEDI_SITIOGRABA").value;
    var Url = Host + '//Huawei//ConsutarPersMediosDispRadios?p1sitio=' + SitioGrabacion + "&p2consMedio=" + Codigo;
    $.ajax({
        url: Url,
        async:false,
        method: "POST",
    }).done(function (Data) {
        if(Data.CodigoSalida == "00")
        {
            var Datos = Data.DatosSecad;
            Resultado = JSON.parse(Datos);                
        }
    }).fail(function (jqXHR, textStatus) {
        console.log(textStatus);
    });

    return Resultado;
}

///cargar datos de grabaciones en recepcion
///function ObtenerDatosGrabaciones(SitioGr, NumeLlamada, FechaInicio, FechaFinal, Acd) {
function ObtenerDatosGrabaciones() {
    console.log('Entró a ObtenerDatosGrabaciones ****');
    var Host = window.location.protocol + "//" + window.location.host;

    var Url = Host + '//Huawei//ConsultarDatosGrabacion?p1Sitio=' + '4173' + "&p2Numellamada=" + '0' + "&p3FechaIni=" + "01/01/2019:00:00:00" + "&p4FechaFin=" + "23/07/2019:23:59:59" + "&p5Acd=" + "000";
    $.ajax({
        url: Url,
        method: "POST",
    }).done(function (Data) {
        if (Data.CodigoSalida == "00") {
            var Datos = Data.DatosSecad;
            var DatosTmp = JSON.parse(Datos);
            $('#ListaGrabacionesLlamadas').empty();
            $.each(DatosTmp.V_DATOS, function (Indice, Valor) {
                console.log('Pintando registros de grabaciones de llamadas')
                var Btn = '<button type="button" class="btn btn-success" id="btn-reproducir">'
                //var Linea = '<tr> <td>' + JSON.stringify(Valor) +'</td></tr>' con este se visualiuza cada registro con \ para cambiar de linea
                var Linea = '<tr><td></td><td>' + Valor.LLAMADA + '</td><td>' + Valor.FECHAGRABACION + '</td><td>' + Valor.ABONADO + '</td><td>' + Valor.MPIO + '</td><td>' + Valor.ACD + '</td><td>' + Valor.TASKID + '</td>' + '<td>' + Btn +'</td></tr>'
                $('#ListaGrabacionesLlamadas').append(Linea);
            });
 
        }
    }).fail(function (jqXHR, textStatus) {
        console.log(textStatus);
    });
}


//////




function IniciarSesionICP() {
    var Host = window.location.protocol + "//" + window.location.host;
    var Url = Host + '//Account//ObtenerConfiguracionICP';

    $.ajax({
        url: Url,
        method: "POST"
    }).done(function (Data)
    {
        var hubURL = 'http://localhost:8085/signalr';
        DeviceICP.Init(hubURL, function (e) {
            if (!e) {
                alert('No fue posible establecer la conexion con el servidor del Middleware');
            }
            else {
                sessionStorage.setItem('Extension', Data.Extension);
                DeviceICP.Login(Data.Usuario, Data.Contrasena, Data.Agente, Data.Extension, Data.URL);
            }
        }, CallBackServer);
    }).fail(function (jqXHR, textStatus)
    {
        console.log(textStatus);
    });
}

function FinalizarLlamadaELTE(Tabla)
{
    var Seleccionados = [];
    var Dispositivos = $("#" + Tabla).find('input:checkbox');
    $.each(Dispositivos,function(indice,valor){
        if($(valor).is(":checked"))
        {
            Seleccionados.push(valor);
        }
    }); 

    if(Seleccionados.length != 0)
    {
        if(Seleccionados.length > 1)
        {
            // $.each(Seleccionados,function(indice,valor){
            //     $(valor).attr("data-elte")
            // });  
            switch(TipoLlamada)
            {
                case "Voz":
                    {
                        DeviceELTE.DeleteDynamicGroup(CodigoGrupoD);
                        break;
                    }
                // case "Video":
                //     {
                //         DeviceELTE.HangUpVideoP2PCall($(Seleccionados[0]).attr("data-elte")); 
                //         break;
                //     }    
            }  
        }
        else
        {
            $('#ColgarELTE').hide();
            $('#VozELTE').prop('disabled',false);
            $('#VideoELTE').prop('disabled',false);
            $('#SolicitarELTE').prop('disabled',false);
            $('#CerrarELTE').prop('disabled',false);
            $("#eLTETexto").text("Finalizando llamada al " + $(Seleccionados[0]).attr("data-elte"));
            switch(TipoLlamada)
            {
                case "Voz":
                    {
                        DeviceELTE.HangUpP2PCall($(Seleccionados[0]).attr("data-elte"));
                        break;
                    }
                case "Video":
                    {
                        DeviceELTE.HangUpVideoP2PCall($(Seleccionados[0]).attr("data-elte")); 
                        break;
                    }    
            }
        }        
    }
}

function HacerLlamadaVozELTE(Tabla)
{
    TipoLlamada = "Voz";
    var Seleccionados = [];
    var Dispositivos = $("#" + Tabla).find('input:checkbox');
    $.each(Dispositivos,function(indice,valor){
        if($(valor).is(":checked"))
        {
            Seleccionados.push(valor);
        }
    }); 

    if(Seleccionados.length != 0)
    {
        if(Seleccionados.length > 1)
        {
            var Miembros = [];
            $.each(Seleccionados,function(indice,valor){
                console.log($(valor).attr("data-elte"));
                Miembros.push($(valor).attr("data-elte"));
            });   

            $('#ColgarELTE').show();
            $('#VozELTE').prop('disabled',true);
            $('#VideoELTE').prop('disabled',true);
            $('#SolicitarELTE').prop('disabled',true);
            $('#CerrarELTE').prop('disabled',true);
            $("#eLTETexto").text("Llamando al " + $(Seleccionados[0]).attr("data-elte"));            
            DeviceELTE.CreateDynamicGroup(Miembros);
        }
        else
        {
            $('#ColgarELTE').show();
            $('#VozELTE').prop('disabled',true);
            $('#VideoELTE').prop('disabled',true);
            $('#SolicitarELTE').prop('disabled',true);
            $('#CerrarELTE').prop('disabled',true);
            $("#eLTETexto").text("Llamando al " + $(Seleccionados[0]).attr("data-elte"));
            DeviceELTE.MakeP2PCall($(Seleccionados[0]).attr("data-elte"));
        }        
    }
    else
    {
        $("#eLTETexto").text("Seleccione al menos uno agente.");

        setTimeout(() => {
            $("#eLTETexto").text('')
        }, 3000);
    }
}

function EnviarSDSELTE(Tabla)
{
    var Destinos="";
    var Seleccionados = [];
    var Dispositivos = $("#" + Tabla).find('input:checkbox');
    var SDSMensaje = $("#SDSELTE").val();

    $.each(Dispositivos,function(indice,valor){
        if($(valor).is(":checked"))
        {
            Seleccionados.push(valor);
        }
    }); 

    if(Seleccionados.length != 0)
    {
        $.each(Seleccionados,function(indice,valor){
            Destinos += $(valor).attr("data-elte") + ";";
        }); 
           
        if(Destinos != "")
        {
            DeviceELTE.SendMessage(Destinos.substring(0, Destinos.length - 1),SDSMensaje);
        }        
    }
    else
    {
        $("#eLTETexto").text("Seleccione al menos uno agente.");
        
        setTimeout(() => {
            $("#eLTETexto").text('')
        }, 3000);
    }
}

function SolicitarVideoELTE(Tabla)
{
    TipoLlamada = "Video";
    var Destinos="";
    var Seleccionados = [];
    var Dispositivos = $("#" + Tabla).find('input:checkbox');
    var SDSMensaje = $("#SDSELTE").val();

    $.each(Dispositivos,function(indice,valor){
        if($(valor).is(":checked"))
        {
            Seleccionados.push(valor);
        }
    }); 

    if(Seleccionados.length > 1)
    {
        $("#eLTETexto").text("Solo se puede llamar a un agente.");
        
        setTimeout(() => {
            $("#eLTETexto").text('')
        }, 3000);   
    }
    else if(Seleccionados.length == 1)
    {
        $('#ColgarELTE').show();
        $('#VozELTE').prop('disabled',true);
        $('#VideoELTE').prop('disabled',true);
        $('#SolicitarELTE').prop('disabled',true);
        $('#CerrarELTE').prop('disabled',true);
        $("#eLTETexto").text("Llamando al " + $(Seleccionados[0]).attr("data-elte"));        
        DeviceELTE.RequestVideoP2PCall($(Seleccionados[0]).attr('data-elte'),false,false);
    }
    else{
        $("#eLTETexto").text("Seleccione al menos uno agente.");
        
        setTimeout(() => {
            $("#eLTETexto").text('')
        }, 3000);  
    }
}

function HacerLlamadaVideoELTE(Tabla)
{
    TipoLlamada = "Video";
    var Destinos="";
    var Seleccionados = [];
    var Dispositivos = $("#" + Tabla).find('input:checkbox');
    var SDSMensaje = $("#SDSELTE").val();

    $.each(Dispositivos,function(indice,valor){
        if($(valor).is(":checked"))
        {
            Seleccionados.push(valor);
        }
    }); 

    if(Seleccionados.length > 1)
    {
        $("#eLTETexto").text("Solo se puede llamar a un agente.");
        
        setTimeout(() => {
            $("#eLTETexto").text('')
        }, 3000);   
    }
    else if(Seleccionados.length == 1)
    {
        $('#ColgarELTE').show();
        $('#VozELTE').prop('disabled',true);
        $('#VideoELTE').prop('disabled',true);
        $('#SolicitarELTE').prop('disabled',true);
        $('#CerrarELTE').prop('disabled',true);
        $("#eLTETexto").text("Llamando al " + $(Seleccionados[0]).attr("data-elte"));        
        DeviceELTE.MakeVideoP2PCall($(Seleccionados[0]).attr('data-elte'),false);
    }
    else{
        $("#eLTETexto").text("Seleccione al menos uno agente.");
        
        setTimeout(() => {
            $("#eLTETexto").text('')
        }, 3000);  
    }
}

function HablarGrupo(AbrirMic)
{
    if(AbrirMic)
    {
        $("#eLTETexto").text("Hablando en el grupo " + NombreGrupoD + ".");
        $('#MicELTE').html("<i class='icon-mic-off2'></i>").removeClass('btn-info').addClass('btn-danger');;
        DeviceELTE.OpenMicToGroup(CodigoGrupoD);
    }
    else
    {
        $("#eLTETexto").text("Conectado al grupo " + NombreGrupoD + ".");
        $('#MicELTE').html("<i class='icon-mic2'></i>").removeClass('btn-danger').addClass('btn-info');;
        DeviceELTE.CloseMicToGroup(CodigoGrupoD);
    }
}

function InicializareLTE() {
    var hubURL = 'http://localhost:8085/signalr';

    DeviceELTE.Init(hubURL, function (e) {
      if (!e) {
      }
      else {
      }
    }, CallBackServer);
}

function FinalizarSesionICP() {
    DeviceICP.LogOut();
}

function ConectarMiddlewareICP() {
    var hubURL = 'http://localhost:8085/signalr';
    DeviceICP.Init(hubURL, function (e) {
        if (!e) {
            alert('No fue posible establecer la conexion con el servidor del Middleware');
        }
        else {
            DeviceICP.ChangeStatus(0);
            console.log('Inicializado.');
        }
    }, CallBackServer);
}

function IniciarGrabacionPantalla(numerotelefono)
{
    var Fecha = new Date();
    var FechaActual = Fecha.getDate()  + "_" + (Fecha.getMonth()+1) + "_" + Fecha.getFullYear() + "_" + Fecha.getHours() + "_" + Fecha.getMinutes();
    var Archivo = 'C:\\ScreenRecordings\\' + sessionStorage.getItem('Extension') + numerotelefono + FechaActual + '.avi';
    console.log(Archivo);
    ScreenRecord.StartRecording(Archivo, 11, 600000);
}

function DetenerGrabacionPantalla()
{
    ScreenRecord.StopRecording();
}

function GenerarStringAleatorio(tamano) {
    var resultado = "";
    var semilla = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
  
    for (var i = 0; i < tamano; i++)
      resultado += semilla.charAt(Math.floor(Math.random() * semilla.length));
  
    return resultado;
}

function ConectarMiddlewareSR() {
    var hubURL = 'http://localhost:8085/signalr';
    ScreenRecord.Init(hubURL, function (e) {
        if (!e) {
            alert('No fue posible establecer la conexion con el servidor del Middleware');
        }
        else {
            console.log('Inicializado.');
        }
    }, CallBackServer);
}

function TransferirLlamada()
{
    $('#PnlTransfer').modal( { show: true,
                            backdrop: 'static'})
}

function InsertarTelefono(ControlHijo)
{
    DeviceICP.ShowPhoneHTML(ControlHijo, 'TELEFONO');
    $('body').append($('#' + ControlHijo).find('#IncomingCall'));
    $('body').append($('#' + ControlHijo).find('#PnlTransfer'));   
    $('body').append($('#' + ControlHijo).find('#PnlGroup'));    
    $('#' + ControlHijo).find('#IncomingCall').remove();  
    $('#' + ControlHijo).find('#PnlTransfer').remove();
    $('#' + ControlHijo).find('#PnlGroup').remove();  
    $('#IncomingHang').hide();
}

function DesplegarTelefono(ControlModal) {
    $('#' + ControlModal).modal({ show: true,
                                    backdrop: 'static'});
}

function HacerLlamadaICP(telefono)
{
    DeviceICP.Call(telefono);
}

function ColgarLlamada()
{
    DeviceICP.HangUp();
}

function CallBackServer(eventType, message) {
    console.log(eventType + ': ' + JSON.stringify(message));
    switch (eventType) {
        case "Error":
            {
                console.log('Se presento un error: ' + message.ErrorCode);
                break;
            }
        case "ICPLogin":
            {
                if (message.ErrorCode == 0) {
                    DeviceICP.ChangeStatus(0);
                    console.log('logueado');
                }
                break;
            }
        case "ICPLogout":
            {
                if (message.ErrorCode == 0) {
                    console.log('deslogueado');
                }
                break;
            }
        case "ICPPhoneHangUp":
            {
                if (message.ErrorCode == 0) {
                    if(!DevolviendoLlamada)
                    {
                        $('#NUME_TELEFONO').val(message.Arguments.DialContent.PhoneNumber);
                        
                        var e = jQuery.Event("keypress");
                        e.which = 13; 
                        e.keyCode = 13;
                        $("#NUME_TELEFONO").trigger(e);    
    
                        RingCounter = 1;
                        $('#IncomingCall').modal('hide');  
                                           
                        //CerrarPedido('COLGARON');
    
                        DetenerGrabacionPantalla();                     
                    }
                    else
                    {
                        DevolviendoLlamada = false;
                        $('#btnColgarLlamada').hide();
                        $('#txtMensajeICPLlamada').text('');
                    }   
                    
                    setTimeout(function(){
                        DeviceICP.ChangeStatus(0);
                    },5000)   
                }
                break;
            }         
        case "ICPAnswerCall":
            {
            if (message.ErrorCode == 0) {
                EnLlamada = true;
                try{
                    CallerNumber = message.Arguments.Numbers[0];
                }
                catch(err){
                    CallerNumber = message.Arguments.CallerNumber
                }

                CallerNumber = message.Arguments.CallerNumber

                if(typeof CallerNumber === 'undefined')
                {
                    CallerNumber = message.Arguments.Numbers[0];
                }                
                
                $('#NUME_TELEFONO').val(CallerNumber);
                console.log('*******' + CallerNumber);
                // $('#NUME_TELEFONO').val(message.Arguments.CallerNumber);

                // console.log('*******' + message.Arguments.CallerNumber);

                var e = jQuery.Event("keypress");
                e.which = 13; 
                e.keyCode = 13;
                $("#NUME_TELEFONO").trigger(e);
    
                IniciarGrabacionPantalla(CallerNumber);

                // IniciarGrabacionPantalla(message.Arguments.CallerNumber);
            }
            break;
            }    
        case "ICPStartRecordingCall":
            {   
                if (message.ErrorCode == 0) {
                    console.log('ICPStartRecordingCall. Captura de los datos para EnviarDatosGrabacionICP  paso1');
                    vSitioGraba = document.getElementById("CADPEDI_SITIOGRABA").value;
                    vNumeLlamada = document.getElementById("CADPEDI_NUMELLAMADA").value;
                    vUsuario = document.getElementById("USUA_MODIFICA").value;
                    vEquipo = 'OFITE-AITEC7';
                    vAcd = sessionStorage.getItem('Extension');
                    vInfoGrabacion = message.Arguments;
                    console.log('ICPStartRecordingCall. Captura de los datos para EnviarDatosGrabacionICP paso2');
                    vRecordId = vInfoGrabacion.RecordId;
                    vCallId = vInfoGrabacion.CallId;
                    vTaskid = vInfoGrabacion.FileName;
                    console.log('ICPStartRecordingCall. Captura de los datos para EnviarDatosGrabacionICP paso3' );
                    vCaller = vInfoGrabacion.Numbers[0];
                    vCalled = vInfoGrabacion.Numbers[1];
                    console.log('ICPStartRecordingCall. Captura de los datos para EnviarDatosGrabacionICP paso4');
                    console.log("Parametros grabacion: " + vTaskid + " " + vCaller + " " + vRecordId + " " + vCalled + " " + vCallId);
                    //los datos se envian desde pedidoscontroler.js
                    //EnviarDatosGrabacionICP(vSitioGraba, vNumeLlamada, vAcd, vTaskid, vCaller, vRecordId, vCalled, vCallId, vUsuario, vEquipo);
                }
                break;
            }
        case "ICPChangeStatus":
            {
                if (message.ErrorCode == 0) {
                
                }
                break;
            }
        case "eLTEInit":
            {
                if (message.ErrorCode == 0) {
                    DeviceELTE.Login('2109', '2109', '172.27.54.186', '172.19.53.101', '5060');
                }
                break;
            }   
        case "eLTELogin":
            {
                if (message.ErrorCode == 0) {
                    // var statusReport = sessionStorage.getItem('statusReport');
                    // DeviceELTE.GetGroupList();
                    // if (typeof statusReport === null) {
                    //     DeviceELTE.GetUserList(1);
                    //     sessionStorage.setItem('statusReport', '1')
                    // }
                    // else {
                    //     DeviceELTE.GetUserList(0);
                    // }
                }
                break;
            }       
        case "P2P_IND_STATUS_RINGING":
            {
                if (message.ErrorCode == 0) {
                    $("#eLTETexto").text("Esperando respuesta...");
                }
                break;
            }
        case "P2P_IND_STATUS_REMOTE_NOANSWER":
            {
                $('#ColgarELTE').hide();
                $('#VozELTE').prop('disabled',false);
                $('#VideoELTE').prop('disabled',false);
                $('#SolicitarELTE').prop('disabled',false);
                $('#CerrarELTE').prop('disabled',false);
                $("#eLTETexto").text("Llamada no contestada por la otra parte.");

                setTimeout(() => {
                    $("#eLTETexto").text('')
                }, 3000); 
                break;                     
            }
        case "P2P_IND_STATUS_REMOTE_NOTCONNECTED":
            {
                $('#ColgarELTE').hide();
                $('#VozELTE').prop('disabled',false);
                $('#VideoELTE').prop('disabled',false);
                $('#SolicitarELTE').prop('disabled',false);
                $('#CerrarELTE').prop('disabled',false);
                $("#eLTETexto").text("Terminal eLTE apagada.");

                setTimeout(() => {
                    $("#eLTETexto").text('')
                }, 3000); 
                break;                    
            }
        case "P2P_IND_STATUS_ANSWERED":
            {
                $("#eLTETexto").text("En llamada con " + message.Arguments.Callee); 
                break;
            }
        case "P2P_IND_STATUS_HANGUPED":
            {
                $('#ColgarELTE').hide();
                $('#VozELTE').prop('disabled',false);
                $('#VideoELTE').prop('disabled',false);
                $('#SolicitarELTE').prop('disabled',false);
                $('#CerrarELTE').prop('disabled',false);
                $("#eLTETexto").text("Llamada finalizada por la otra parte.");

                setTimeout(() => {
                    $("#eLTETexto").text('')
                }, 3000); 
                break;
            }
        case "P2P_IND_STATUS_RELEASED_BUSY":
            {
                $('#ColgarELTE').hide();
                $('#VozELTE').prop('disabled',false);
                $('#VideoELTE').prop('disabled',false);
                $('#SolicitarELTE').prop('disabled',false);
                $('#CerrarELTE').prop('disabled',false);
                $("#eLTETexto").text("Llamada rechazada por la otra parte.");

                setTimeout(() => {
                    $("#eLTETexto").text('')
                }, 3000); 
                break;
            }  
        case "eLTESendMessageResult":
            {
                if (message.ErrorCode == 0) {
                    var Destino = message.Arguments.SdsReceiver;
                    $("#eLTETexto").text("Mensaje enviado con exito. " + Destino);
                    $('#UISDSELTE1').hide();
                    $('#UISDSELTE2').hide();

                    setTimeout(() => {
                        $("#eLTETexto").text('')
                    }, 3000); 
                }
                else
                {
                    var Destino = message.Arguments.SdsReceiver;
                    $("#eLTETexto").text("El mensaje no pudo ser enviado. " + Destino);
                    $('#UISDSELTE1').hide();
                    $('#UISDSELTE2').hide();

                    setTimeout(() => {
                        $("#eLTETexto").text('')
                    }, 3000);                     
                }
                break;
            }   
        case "P2Pvideocall_IND_STATUS_RINGING":
            {
                if (message.ErrorCode == 0) {
                    $("#eLTETexto").text("Esperando respuesta...");
                }
                break;                
            }
        case "P2Pvideocall_IND_STATUS_REMOTE_NOTCONNECTED":
            {
                $('#ColgarELTE').hide();
                $('#VozELTE').prop('disabled',false);
                $('#VideoELTE').prop('disabled',false);
                $('#SolicitarELTE').prop('disabled',false);
                $('#CerrarELTE').prop('disabled',false);
                $("#eLTETexto").text("Terminal eLTE apagada.");

                setTimeout(() => {
                    $("#eLTETexto").text('')
                }, 3000); 
                break;                   
            }
        case "P2Pvideocall_IND_STATUS_ANSWERED":
            {
                $("#eLTETexto").text("En video llamada con " + message.Arguments.Callee);
                break; 
            }   
        case "P2Pvideocall_IND_STATUS_REMOTE_NOANSWER":
            {
                $('#ColgarELTE').hide();
                $('#VozELTE').prop('disabled',false);
                $('#VideoELTE').prop('disabled',false);
                $('#SolicitarELTE').prop('disabled',false);
                $('#CerrarELTE').prop('disabled',false);
                $("#eLTETexto").text("Llamada no contestada por la otra parte.");

                setTimeout(() => {
                    $("#eLTETexto").text('')
                }, 3000); 
                break;                
            }
        case "P2Pvideocall_IND_STATUS_RELEASED_BUSY":
            {
                $('#ColgarELTE').hide();
                $('#VozELTE').prop('disabled',false);
                $('#VideoELTE').prop('disabled',false);
                $('#SolicitarELTE').prop('disabled',false);
                $('#CerrarELTE').prop('disabled',false);
                $("#eLTETexto").text("Llamada rechazada por la otra parte.");

                setTimeout(() => {
                    $("#eLTETexto").text('')
                }, 3000); 
                break;
            }   
        case "P2Pvideocall_IND_STATUS_HANGUPED":
        {
            $('#ColgarELTE').hide();
            $('#VozELTE').prop('disabled',false);
            $('#VideoELTE').prop('disabled',false);
            $('#SolicitarELTE').prop('disabled',false);
            $('#CerrarELTE').prop('disabled',false);
            $("#eLTETexto").text("Llamada finalizada por la otra parte.");

            setTimeout(() => {
                $("#eLTETexto").text('')
            }, 3000); 
            break;
        }   
        case "P2Pvideocall_IND_STATUS_HANGUPED_ACTIVE":
            {
                $('#ColgarELTE').hide();
                $('#VozELTE').prop('disabled',false);
                $('#VideoELTE').prop('disabled',false);
                $('#SolicitarELTE').prop('disabled',false);
                $('#CerrarELTE').prop('disabled',false);
                $("#eLTETexto").text("Llamada finalizada por la otra parte.");
    
                setTimeout(() => {
                    $("#eLTETexto").text('')
                }, 3000); 
                break;
            }     
        case "eLTECreateDynamicGroup":
            {
                if(message.ErrorCode == 0)
                {
                    CodigoGrupoD = message.Arguments.GroupID;
                    NombreGrupoD = message.Arguments.GroupName;

                    $('#MicELTE').show();
                    $("#eLTETexto").text("Conectado al grupo " + NombreGrupoD + ".");
                }
                break;
            } 
        case "eLTEDeleteDynamicGroup":
            {
                if(message.ErrorCode == 0)
                {
                    $('#MicELTE').hide();
                    $('#ColgarELTE').hide();
                    $('#VozELTE').prop('disabled',false);
                    $('#VideoELTE').prop('disabled',false);
                    $('#SolicitarELTE').prop('disabled',false);
                    $('#CerrarELTE').prop('disabled',false);
                    $("#eLTETexto").text("Se elimino el grupo " + NombreGrupoD + ".");

                    CodigoGrupoD = '';
                    NombreGrupoD = '';        

                    setTimeout(() => {
                        $("#eLTETexto").text('')
                    }, 3000); 
                }                
                break;
            }           
    }


    function IncluirDatosGrabacionICP(pSitio, pNumeroSecad, pAcd, pTaskId, pCaller, pRecordId, pCalled, pCallId, pUsuario, pEquipo) {
        var proceso = ClWsProcesosIntegracion()

        var datosGrabacion = proceso.ClGrabacionesICP.ClCadPedidosDatosICP();
        var OutServDatosGrabacion = proceso.ClGrabacionesICP.ClCadPedidosDatosICPOut();
        var servicioInsdatos = proceso.ClGrabacionesICP.MInsDatosGrabacion();

        datosGrabacion.cadpedi_sitiograba = pSitio;
        datosGrabacion.cadpedi_numellamada = pNumeroSecad;
        datosGrabacion.acd = pAcd;
        datosGrabacion.taskid = pTaskId;
        datosGrabacion.caller = pCaller;
        datosGrabacion.recordid = pRecordId;
        datosGrabacion.called = pCalled;
        datosGrabacion.callid = pCallId;
        datosGrabacion.usersecad = pUsuario;
        datosGrabacion.cadequipo = pEquipo;

        OutServDatosGrabacion = servicioInsdatos(datosGrabacion);
        console.log('Envío datos: ' + OutServDatosGrabacion.cadpedi_numellamada + ' ' + OutServDatosGrabacion.callid);
        return OutServDatosGrabacion;
    }


//

}