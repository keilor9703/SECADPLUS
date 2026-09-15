var RingCounter = 1;
var IntervalRings;
var ICPHub;
var IVSHub;
var ELTEHub;
var ScreenRecordHub;
var HtmlPhone = "\
<div id='IncomingCall' class='modal' tabindex='-1' role='dialog'> \
<div class='modal-dialog' role='document'> \
    <div class='modal-content'> \
        <div class='modal-header'> \
            <h5 class='modal-title'>Llamanda Entrante...</h5> \
        </div> \
        <div class='modal-body'> \
            <label id='IncomingPhoneNumber'></label> \
        </div> \
        <div class='modal-footer'> \
            <button type='button' id='IncomingHang' onclick='MakeClick(this);' class='btn btn-danger'> \
                <i class='icon-phone-slash'></i> \
            </button> \
            <button type='button' id='IncomingAnswer' onclick='MakeClick(this);' class='btn btn-success'> \
                <i class='icon-phone2'></i> \
            </button> \
        </div> \
    </div> \
</div> \
</div> \
<div id='PnlGroup' class='modal fade' id='exampleModalCenter' tabindex='-1' role='dialog' aria-labelledby='exampleModalCenterTitle' aria-hidden='true'> \
  <div class='modal-dialog modal-dialog-centered' role='document'> \
    <div class='modal-content'> \
      <div class='modal-header'> \
        <h6 class='modal-title' id='exampleModalLongTitle'>Conferencia</h6> \
        <button type='button' class='close' data-dismiss='modal' aria-label='Close'> \
          <span aria-hidden='true'>&times;</span> \
        </button> \
      </div> \
      <div class='modal-body'> \
            <div class='input-group mb-3'> \
                <input type='text' id='SPDestinosGrupo' class='form-control form-control-sm' placeholder='Escriba el telefono a agregar' aria-describedby='basic-addon2'> \
            <div class='input-group-btn'> \
                    <button id='SPGroupCall' onclick='MakeClick(this);' class='btn btn-outline-secondary btn-sm'  data-toggle='tooltip' data-placement='top' title='Click para crear la conferencia...' type='button'> \
                        <i class='icon-phone2 text-success'></i> \
                    </button> \
            </div> \
      </div> \
    </div> \
  </div> \
</div> \
    </div> \
<div id='PnlTransfer' class='modal fade' id='exampleModalCenter' tabindex='-1' role='dialog' aria-labelledby='exampleModalCenterTitle' aria-hidden='true'> \
  <div class='modal-dialog modal-dialog-centered' role='document'> \
    <div class='modal-content'> \
      <div class='modal-header'> \
        <h6 class='modal-title' id='exampleModalLongTitle'>Transferir Llamada</h6> \
        <button type='button' class='close' data-dismiss='modal' aria-label='Close'> \
          <span aria-hidden='true'>&times;</span> \
        </button> \
      </div> \
      <div class='modal-body'> \
            <div class='input-group mb-3'> \
                <input type='text' id='SPTelefonoDestino' class='form-control form-control-sm' placeholder='Telefono destino' aria-describedby='basic-addon2'> \
            <div class='input-group-btn'> \
                    <button id='SPCallTransfer' onclick='MakeClick(this);' class='btn btn-outline-secondary btn-sm'  data-toggle='tooltip' data-placement='top' title='Click para transferir la llamada...' type='button'> \
                        <i class='icon-phone2 text-success'></i> \
                    </button> \
            </div> \
      </div> \
    </div> \
  </div> \
</div> \
    </div> \
    <div class='card'> \
            <div id='SPTeclado' class='card-body'> \
            <div class='row mb-3'> \
            <div class='input-group mb-3'> \
            <div class='input-group-addon'> \
                <i id='SPStatus' class='icon-radio-checked2 text-success'></i> \
            </div> \
                <input type='text' id='SPTelefono' maxlength='11' class='form-control form-control-sm'> \
                <div class='input-group-btn'> \
                <button type='button' class='btn btn-default dropdown-toggle' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'><i class='icon-three-bars'></i><span class='caret'></span></button> \
                <ul class='dropdown-menu dropdown-menu-right'> \
                <li id='SPTransfer' onclick='MakeClick(this);' class='text-sm'><a href='#'><i class='icon-phone-outgoing'></i> &nbsp; Transferir</a></li> \
                <li id='SPGroup' onclick='MakeClick(this);' class='text-sm'><a href='#'><i class='icon-make-group'></i> &nbsp; Crear Conferencia</a></li> \
                <li role='separator' class='divider'></li> \
                <li id='SPBusy' onclick='MakeClick(this);' class='text-sm'><a href='#'><i class='icon-radio-checked2 text-danger'></i> &nbsp; Ocupado</a></li> \
                <li id='SPIdle' onclick='MakeClick(this);' class='text-sm'><a href='#'><i class='icon-radio-checked2 text-success'></i> &nbsp; Disponible</a></li> \
                <li role='separator' class='divider'></li> \
                <li id='SPHold' onclick='MakeClick(this);' class='text-sm'><a href='#'><i class='icon-pause2'></i> &nbsp; Llamada en Espera</a></li> \
                <li id='SPUnHold' onclick='MakeClick(this);' class='text-sm'><a href='#'><i class='icon-play3'></i> &nbsp; Continuar Llamada</a></li> \
                <li role='separator' class='divider'></li> \
                <li id='SPMute' onclick='MakeClick(this);' class='text-sm'><a href='#'><i class='icon-mic-off2 text-danger'></i> &nbsp; Desactivar Microfono</a></li> \
                <li id='SPUnMute' onclick='MakeClick(this);' class='text-sm'><a href='#'><i class='icon-mic2 text-success'></i> &nbsp; Activar Microfono</a></li> \
                <li role='separator' class='divider'></li> \
                <li id='SPStart' onclick='MakeClick(this);' class='text-sm'><a href='#'><i class='icon-floppy-disk'></i> &nbsp; Grabar Llamada</a></li> \
                <li id='SPStop' onclick='MakeClick(this);' class='text-sm'><a href='#'><i class='icon-stop2 text-danger'></i> &nbsp; Detener Grabacion</a></li> \
                </ul> \
                </div> \
            </div> \
            &nbsp; \
            </div> \
            <div class='row'> \
                <div class='col-md-4 text-center'> \
                    <button id='SP1' type='button' onclick='MakeClick(this);' class='btn btn-default btn-lg'>1</button> \
                </div> \
                <div class='col-md-4 text-center'> \
                    <button id='SP2' type='button' onclick='MakeClick(this);' class='btn btn-default btn-lg'>2</button> \
                </div> \
                <div class='col-md-4 text-center'> \
                    <button id='SP3' type='button' onclick='MakeClick(this);' class='btn btn-default btn-lg'>3</button> \
                </div> \
            </div> \
            <br /> \
            <div class='row'> \
                <div class='col-md-4 text-center'> \
                    <button id='SP4' type='button' onclick='MakeClick(this);' class='btn btn-default btn-lg'>4</button> \
                </div> \
                <div class='col-md-4 text-center'> \
                    <button id='SP5' type='button' onclick='MakeClick(this);' class='btn btn-default btn-lg'>5</button> \
                </div> \
                <div class='col-md-4 text-center'> \
                    <button id='SP5' type='button' onclick='MakeClick(this);' class='btn btn-default btn-lg'>6</button> \
                </div> \
            </div> \
            <br /> \
            <div class='row'> \
                <div class='col-md-4 text-center'> \
                    <button id='SP7' type='button' onclick='MakeClick(this);' class='btn btn-default btn-lg'>7</button> \
                </div> \
                <div class='col-md-4 text-center'> \
                    <button id='SP8' type='button' onclick='MakeClick(this);' class='btn btn-default btn-lg'>8</button> \
                </div> \
                <div class='col-md-4 text-center'> \
                    <button id='SP9' type='button' onclick='MakeClick(this);' class='btn btn-default btn-lg'>9</button> \
                </div> \
            </div> \
            <br /> \
            <div class='row'> \
                <div class='col-md-4 text-center'> \
                    <button id='SPC' onclick='MakeClick(this);' data-toggle='tooltip' data-placement='top' title='Click para realizar la llamada...' type='button' class='btn btn-success'> \
                        <i class='icon-phone2'></i> \
                    </button> \
                </div> \
                <div class='col-md-4 text-center'> \
                    <button id='SP0' onclick='MakeClick(this);' type='button' class='btn btn-default btn-lg'>0</button> \
                </div> \
                <div class='col-md-4 text-center'> \
                    <button id='SPH' onclick='MakeClick(this);' data-toggle='tooltip' data-placement='top' title='Click para finalizar la llamada...' type='button' class='btn btn-danger'> \
                        <i class='icon-phone-slash'></i> \
                    </button> \
                </div> \
            </div> \
        </div> \
    </div>";

function Validate(divName) {
    var Resultado = false;
    var Campos = $('#' + divName).find('input, textarea');
    var Total = Campos.length;
    var Validos = 0;

    $.each(Campos, function (key, value) {
        if ($(value).prop('required')) {
            if ($(value).val() == '') {
                if ($(value).parent().find('#requerido').val() == undefined) {
                    if ($('.login-wrap').length != 0) {
                        var largo = Number($('.login-wrap').css('min-height').replace('px', ''));
                        largo += 20;
                        $('.login-wrap').css('min-height', largo + 'px');
                    }

                    $(value).parent().append('<div id="requerido" class="input-group"><p class="text-danger"><small>Campo requerido</small></p></div>');
                }
            }
            else {
                Validos++
                if ($(value).parent().find('#requerido').val() != undefined) {
                    if ($('.login-wrap').length != 0) {
                        var largo = Number($('.login-wrap').css('min-height').replace('px', ''));
                        largo -= 20;
                        $('.login-wrap').css('min-height', largo + 'px');
                    }
                    $(value).parent().find('#requerido').remove();
                }
            }
        }
    });
    if (Validos == Total) {
        Resultado = true;
    }
    else {
        Resultado = false
    }
    return Resultado;
}

function MakeClick(button) {
    var idButton = $(button).attr('id');

    if (idButton === 'SP0' || idButton === 'SP1' || idButton === 'SP2' || idButton === 'SP3' || idButton === 'SP4' || idButton === 'SP5' || idButton === 'SP6' || idButton === 'SP7' || idButton === 'SP8' || idButton === 'SP9') {
        var Numero = $('#SPTelefono').val() + $(button).text();
        $('#SPTelefono').val(Numero);
    }

    if (idButton === 'IncomingHang') {
        RingCounter = 1;
        DeviceICP.HangUp();
        $('#IncomingCall').modal('hide');
    }

    if (idButton === 'IncomingAnswer') {
        RingCounter = 1;
        DeviceICP.AnswerCall();
        $('#IncomingCall').modal('hide');
    }

    if (idButton === 'SPC') {
        DeviceICP.Call($('#SPTelefono').val());
    }

    if (idButton === 'SPH') {
        DeviceICP.HangUp();
    }

    if (idButton === 'SPBusy') {
        DeviceICP.ChangeStatus(1);

        $('#SPStatus').removeClass('text-success');
        $('#SPStatus').addClass('text-danger');
    }

    if (idButton === 'SPIdle') {
        DeviceICP.ChangeStatus(0);

        $('#SPStatus').removeClass('text-danger');
        $('#SPStatus').addClass('text-success');
    }

    if (idButton === 'SPTransfer') {
        $('#PnlTransfer').modal('show');
    }

    if (idButton === 'SPGroup') {
        $('#PnlGroup').modal('show');
    }
    
    if (idButton === 'SPCallTransfer') {
        DeviceICP.CallTransfer($('#SPTelefonoDestino').val());
        $('#PnlTransfer').modal('hide');
    }

    if (idButton === 'SPGroupCall') {
        DeviceICP.MakeConference($('#SPDestinosGrupo').val());
        $('#PnlGroup').modal('hide');
    }
    
    if (idButton === 'SPMute') {
        DeviceICP.Mute(false);
    }

    if (idButton === 'SPUnMute') {
        DeviceICP.Mute(true);
    }

    if (idButton === 'SPHold') {
        DeviceICP.Hold(false);
    }

    if (idButton === 'SPUnHold') {
        DeviceICP.Hold(true);
    }
}

var DeviceICP = {
    IcpServerURL: '',
    Initializated: false,
    IncomingCall: false,
    RecordingId: '',
    IncomingCallPhoneNumber: 0,
    ShowPhoneHTML: function (containername, title) {
        HtmlPhone = HtmlPhone.replace('%title%', title);
        $('#' + containername).html(HtmlPhone);
        $('[data-toggle="tooltip"]').tooltip();
    },
    Init: function (MiddlewareUrl, callBack, callBackServer) {
        var connection = $.hubConnection(MiddlewareUrl);
        ICPHub = connection.createHubProxy('icpHub');

        ICPHub.on('sendMessage', function (eventType, message) {
            switch (eventType) {
                case 'ICPIncomingCall':
                    {
                        $('#IncomingPhoneNumber').text('Llamada entrante desde el numero: ' + message.Arguments.CallerNumber);
                        $('#IncomingCall').modal('show');
                        IntervalRings = setInterval(() => {
                            RingCounter++;
                            $('#IncomingPhoneNumber').text('Llamada entrante desde el numero: ' + message.Arguments.CallerNumber + ' Repiques: ' + RingCounter);
                        }, 3000);
                        break;
                    }
                case 'ICPLongNoAnswer':
                    {
                        break;
                    }
                case 'ICPPhoneHangUp':
                    {
                        clearInterval(IntervalRings);
                        $('#IncomingCall').modal('hide');
                        RingCounter = 1;
                        DeviceICP.ChangeStatus(0);
                        break;
                    }
            }

            callBackServer(eventType, message);
            console.log(eventType + ': ' + JSON.stringify(message));
        });

        connection.start()
            .done(function () { Initializated = true; callBack(true); })
            .fail(function () { Initializated = false; callBack(false); });
    },
    MiddlewareVersion: function () {
        ICPHub.invoke('icpGetMiddlewareVersion').done(function () { }).fail(function (error) {
            console.log('icpGetMiddlewareVersion Error: ' + error);
        });
    },
    Login: function (username, password, agentid, phonenumber, icpserverip) {
        ICPHub.invoke('icpLogin', username, password, agentid, phonenumber, icpserverip).done(function () { }).fail(function (error) {
            console.log('icpLogin Error: ' + error);
        });
    },
    Call: function (phonenumber) {
        ICPHub.invoke('icpCall', phonenumber).done(function () { }).fail(function (error) {
            console.log('icpCall Error: ' + error);
        });
    },
    GetPCName: function () {
        ICPHub.invoke('icpGetPCName').done(function () { }).fail(function (error) {
            console.log('icpGetPCName Error: ' + error);
        });
    },
    GetPCMonitors: function () {
        ICPHub.invoke('icpGetPCMonitors').done(function () { }).fail(function (error) {
            console.log('icpGetPCMonitors Error: ' + error);
        });
    },
    HangUp: function () {
        ICPHub.invoke('icpHangUp').done(function () { }).fail(function (error) {
            console.log('icpHangUp Error: ' + error);
        });
    },
    Mute: function (unmute) {
        ICPHub.invoke('icpMuteCall', unmute).done(function () { }).fail(function (error) {
            console.log('icpMuteCall Error: ' + error);
        });
    },
    Hold: function (unhold) {
        ICPHub.invoke('icpHoldCall', unhold).done(function () { }).fail(function (error) {
            console.log('icpHoldCall Error: ' + error);
        });
    },
    AddPhoneToBlackList: function (phonnenumber, locktime) {
        ICPHub.invoke('icpAddPhoneToBlackList', unhold).done(function () { }).fail(function (error) {
            console.log('icpAddPhoneToBlackList Error: ' + error);
        });
    },
    RemovePhoneFromBlackList: function (phonenumber) {
        ICPHub.invoke('icpRemovePhoneFromBlackList', unhold).done(function () { }).fail(function (error) {
            console.log('icp Error: ' + error);
        });
    },
    AddPhoneToRedList: function (phonenumber, priority) {
        ICPHub.invoke('icpAddPhoneToRedList', unhold).done(function () { }).fail(function (error) {
            console.log('icpAddPhoneToRedList Error: ' + error);
        });
    },
    RemovePhoneFromRedList: function (phonenumber) {
        ICPHub.invoke('icpRemovePhoneFromRedList', unhold).done(function () { }).fail(function (error) {
            console.log('icpRemovePhoneFromRedList Error: ' + error);
        });
    },
    CallTransfer: function (targetphonenumber) {
        ICPHub.invoke('icpCallTransfer', targetphonenumber).done(function () { }).fail(function (error) {
            console.log('icpCallTransfer Error: ' + error);
        });
    },
    ChangeStatus: function (status) {
        ICPHub.invoke('icpChangeStatus', status).done(function () { }).fail(function (error) {
            console.log('icpicpChangeStatusHoldCall Error: ' + error);
        });
    },
    AdminBarge: function (agentid) {
        ICPHub.invoke('icpAdminBarge', agentid).done(function () { }).fail(function (error) {
            console.log('icpAdminBarge Error: ' + error);
        });
    },
    AdminMonitor: function (agentid) {
        ICPHub.invoke('icpAdminMonitor', agentid).done(function () { }).fail(function (error) {
            console.log('icpAdminMonitor Error: ' + error);
        });
    },
    SendSMS: function (deviceid, phonenumber, text, ipaddress, ipport) {
        ICPHub.invoke('icpSendSMS', deviceid, phonenumber, text, ipaddress, ipport).done(function () { }).fail(function (error) {
            console.log('icpSendSMS Error: ' + error);
        });
    },
    AnswerCall: function () {
        ICPHub.invoke('icpAnswerCall').done(function () { }).fail(function (error) {
            console.log('icpAnswerCall Error: ' + error);
        });
    },
    MakeConference: function (targetphonenumber) {
        ICPHub.invoke('icpMakeConference', targetphonenumber).done(function () { }).fail(function (error) {
            console.log('icpMakeConference Error: ' + error);
        });
    },
    // PlayCallRecord: function (callrecordid) {
    //     CreateInstance();
    //     AXObject.PlayCallRecording(callrecordid);
    // },
    LogOut: function () {
        ICPHub.invoke('icpLogout').done(function () { }).fail(function (error) {
            console.log('icpLogout Error: ' + error);
        });
    },
};

var DeviceIVS = {
    Initializated: false,
    Init: function (MiddlewareUrl, callBack, callBackServer) {
        var connection = $.hubConnection(MiddlewareUrl);
        IVSHub = connection.createHubProxy('ivsHub');

        IVSHub.on('sendMessage', function (eventType, message) {
            console.log(eventType + ': ' + message);
            callBackServer(eventType, message);
        });

        connection.start()
            .done(function () { Initializated = true; callBack(true); })
            .fail(function () { Initializated = false; callBack(false); });
    },
    MiddlewareVersion: function () {
        IVSHub.invoke('ivsGetMiddlewareVersion').done(function () { }).fail(function (error) {
            console.log('ivsGetMiddlewareVersion Error: ' + error);
        });
    },
    Login: function (username, password, serverip, serverport) {
        IVSHub.invoke('ivsLogin', username, password, serverip, serverport).done(function () { }).fail(function (error) {
            console.log('ivsLogin Error: ' + error);
        });
    },
    ListCameras: function () {
        IVSHub.invoke('ivsListCameras').done(function () { }).fail(function (error) {
            console.log('ivsListCameras Error: ' + error);
        });
    },
    GetLiveFeed: function (cameracode, windowtitle, showcontrols) {
        IVSHub.invoke('ivsGetLiveFeed', cameracode, windowtitle, showcontrols).done(function () { }).fail(function (error) {
            console.log('ivsGetLiveFeed Error: ' + error);
        });
    },
    GetCameraConfiguration: function (cameracode) {
        IVSHub.invoke('ivsGetCameraConfiguration', cameracode).done(function () { }).fail(function (error) {
            console.log('ivsGetCameraConfiguration Error: ' + error);
        });
    },
    GetCameraRecordings: function (cameracode, startdate, enddate) {
        IVSHub.invoke('ivsGetCameraRecordings', cameracode, startdate, enddate).done(function () { }).fail(function (error) {
            console.log('ivsGetCameraRecordings Error: ' + error);
        });
    },
    PlayCameraRecording: function (cameracode, startdate, enddate, showcontrols) {
        IVSHub.invoke('ivsPlayCameraRecording', cameracode, startdate, enddate, showcontrols).done(function () { }).fail(function (error) {
            console.log('ivsPlayCameraRecording Error: ' + error);
        });
    },
    DownloadCameraRecording: function (cameracode, startdate, enddate, progressbar) {
        var code = 1 + Math.floor(Math.random() * 100);
        $(progressbar).attr('id', 'download_' + code);

        IVSHub.invoke('ivsDownloadCameraRecording', cameracode, startdate, enddate, code).done(function () { }).fail(function (error) {
            console.log('ivsDownloadCameraRecording Error: ' + error);
        });
    },
    PlayPauseCameraRecording: function (handle) {
        IVSHub.invoke('ivsPlayPauseCameraRecording', handle).done(function () { }).fail(function (error) {
            console.log('ivsPlayPauseCameraRecording Error: ' + error);
        });
    },
    PlayResumeCameraRecording: function (handle) {
        IVSHub.invoke('ivsPlayResumeCameraRecording', handle).done(function () { }).fail(function (error) {
            console.log('ivsPlayResumeCameraRecording Error: ' + error);
        });
    },
    PlayResumeCameraRecording: function (handle) {
        IVSHub.invoke('ivsPlayResumeCameraRecording', handle).done(function () { }).fail(function (error) {
            console.log('ivsPlayResumeCameraRecording Error: ' + error);
        });
    },
    PlayStopCameraRecording: function (handle) {
        IVSHub.invoke('ivsPlayStopCameraRecording', handle).done(function () { }).fail(function (error) {
            console.log('ivsPlayStopCameraRecording Error: ' + error);
        });
    },
    PlayCameraRecordingSpeed: function (handle, speed) {
        IVSHub.invoke('ivsPlayCameraRecordingSpeed', handle, speed).done(function () { }).fail(function (error) {
            console.log('ivsPlayCameraRecordingSpeed Error: ' + error);
        });
    },
    MoveCameraPositionUp: function (cameracode) {
        IVSHub.invoke('ivsMoveCameraPositionUp', cameracode).done(function () { }).fail(function (error) {
            console.log('ivsMoveCameraPositionUp Error: ' + error);
        });
    },
    MoveCameraPositionDown: function (cameracode) {
        IVSHub.invoke('ivsMoveCameraPositionDown', cameracode).done(function () { }).fail(function (error) {
            console.log('ivsMoveCameraPositionDown Error: ' + error);
        });
    },
    MoveCameraPositionLeft: function (cameracode) {
        IVSHub.invoke('ivsMoveCameraPositionLeft', cameracode).done(function () { }).fail(function (error) {
            console.log('ivsMoveCameraPositionLeft Error: ' + error);
        });
    },
    MoveCameraPositionRight: function (cameracode) {
        IVSHub.invoke('ivsMoveCameraPositionRight', cameracode).done(function () { }).fail(function (error) {
            console.log('ivsMoveCameraPositionRight Error: ' + error);
        });
    },
    ZoomCameraIn: function (cameracode) {
        IVSHub.invoke('ivsZoomCameraIn', cameracode).done(function () { }).fail(function (error) {
            console.log('ivsZoomCameraIn Error: ' + error);
        });
    },
    ZoomCameraOut: function (cameracode) {
        IVSHub.invoke('ivsZoomCameraOut', cameracode).done(function () { }).fail(function (error) {
            console.log('ivsZoomCameraOut Error: ' + error);
        });
    },
    StopCameraPosition: function (cameracode) {
        IVSHub.invoke('ivsStopCameraPosition', cameracode).done(function () { }).fail(function (error) {
            console.log('ivsStopCameraPosition Error: ' + error);
        });
    },
}

var DeviceELTE = {
    Initializated: false,
    Init: function (MiddlewareUrl, callBack, callBackServer) {
        var connection = $.hubConnection(MiddlewareUrl);
        ELTEHub = connection.createHubProxy('elteHub');

        ELTEHub.on('sendMessage', function (eventType, message) {
            console.log(eventType + ': ' + message);
            callBackServer(eventType, message);
        });

        connection.start()
            .done(function () {
                console.log(DeviceELTE.Initializated);
                if (!DeviceELTE.Initializated) {
                    ELTEHub.invoke('elteInit').done(function () { DeviceELTE.Initializated = true; }).fail(function (error) {
                        DeviceELTE.Initializated = true;
                    });
                }

                callBack(true);
            })
            .fail(function () { Initializated = false; callBack(false); });
    },
    MiddlewareVersion: function () {
        ELTEHub.invoke('elteGetMiddlewareVersion').done(function () { }).fail(function (error) {
            console.log('eLTEGetMiddlewareVersion Error: ' + error);
        });
    }, 
    Login: function (username, password, serverip, localip, serverport) {
        ELTEHub.invoke('elteLogin', username, password, serverip, localip, serverport).done(function () { }).fail(function (error) {
            console.log('eLTELogin Error: ' + error);
        });
    },
    Logout: function () {
        ELTEHub.invoke('elteLogout', username, password, serverip, serverport).done(function () { }).fail(function (error) {
            console.log('eLTELogout Error: ' + error);
        });
    },
    GetUserList: function (notifystatuschange) {
        ELTEHub.invoke('elteGetUserList', notifystatuschange).done(function () { }).fail(function (error) {
            console.log('eLTEGetUserList Error: ' + error);
        });
    },
    GetGroupList: function () {
        ELTEHub.invoke('elteGetGroupList').done(function () { }).fail(function (error) {
            console.log('eLTEGetGroupList Error: ' + error);
        });
    },
    MakeP2PCall: function (targetuserid) {
        ELTEHub.invoke('elteMakeP2PCall', targetuserid).done(function () { }).fail(function (error) {
            console.log('eLTEMakeP2PCall Error: ' + error);
        });
    },
    HangUpP2PCall: function (targetuserid) {
        ELTEHub.invoke('elteHangUpP2PCall', targetuserid).done(function () { }).fail(function (error) {
            console.log('eLTEHangUpP2PCall Error: ' + error);
        });
    },
    AnswerP2PCall: function (calleruserid) {
        ELTEHub.invoke('elteAnswerP2PCall', calleruserid).done(function () { }).fail(function (error) {
            console.log('eLTEAnswerP2PCall Error: ' + error);
        });
    },
    RejectP2PCall: function (calleruserid) {
        ELTEHub.invoke('elteRejectP2PCall', calleruserid).done(function () { }).fail(function (error) {
            console.log('eLTERejectP2PCall Error: ' + error);
        });
    },
    MakeVideoP2PCall: function (targetuserid,rearcamera) {
        ELTEHub.invoke('elteMakeP2PVideoCall', targetuserid,rearcamera).done(function () { }).fail(function (error) {
            console.log('eLTEMakeP2PVideoCall Error: ' + error);
        });
    },
    RequestVideoP2PCall: function (targetuserid,rearcamera,muted) {
        ELTEHub.invoke('elteRequestP2PVideo', targetuserid,rearcamera,muted).done(function () { }).fail(function (error) {
            console.log('eLTERequestP2PVideo Error: ' + error);
        });
    },
    HangUpVideoP2PCall: function (targetuserid) {
        ELTEHub.invoke('elteHangUpP2PVideoCall', targetuserid).done(function () { }).fail(function (error) {
            console.log('eLTEHangUpP2PVideoCall Error: ' + error);
        });
    },    
    SuscribeToGroup: function (groupid) {
        ELTEHub.invoke('elteSuscribeToGroup', groupid).done(function () { }).fail(function (error) {
            console.log('eLTESuscribeToGroup Error: ' + error);
        });
    },
    CreateDynamicGroup: function (useridlist) {
        ELTEHub.invoke('elteCreateDynamicGroup', useridlist).done(function () { }).fail(function (error) {
            console.log('eLTECreateDynamicGroup Error: ' + error);
        });
    },
    DeleteDynamicGroup: function (groupid) {
        ELTEHub.invoke('elteDeleteDynamicGroup', groupid).done(function () { }).fail(function (error) {
            console.log('eLTEDeleteDynamicGroup Error: ' + error);
        });
    },
    UnSuscribeFromGroup: function (groupid) {
        ELTEHub.invoke('elteUnSuscribeFromGroup', groupid).done(function () { }).fail(function (error) {
            console.log('eLTEUnSuscribeFromGroup Error: ' + error);
        });
    },
    OpenMicToGroup: function (groupid) {
        ELTEHub.invoke('elteOpenMicToGroup', groupid).done(function () { }).fail(function (error) {
            console.log('eLTEOpenMicToGroup Error: ' + error);
        });
    },
    CloseMicToGroup: function (groupid) {
        ELTEHub.invoke('elteCloseMicToGroup', groupid).done(function () { }).fail(function (error) {
            console.log('eLTECloseMicToGroup Error: ' + error);
        });
    },
    UnSuscribeFromGroup: function (groupid) {
        ELTEHub.invoke('elteUnSuscribeFromGroup', groupid).done(function () { }).fail(function (error) {
            console.log('eLTEUnSuscribeFromGroup Error: ' + error);
        });
    },
    SuscribeToGIS: function (targetuserid) {
        ELTEHub.invoke('elteSuscribeToGIS', targetuserid).done(function () { }).fail(function (error) {
            console.log('eLTESuscribeToGIS Error: ' + error);
        });
    },
    UnSuscribeFromGIS: function (targetuserid) {
        ELTEHub.invoke('elteUnSuscribeFromGIS', targetuserid).done(function () { }).fail(function (error) {
            console.log('eLTEUnSuscribeFromGIS Error: ' + error);
        });
    },
    SendMessage: function (targetuserid, message) {
        ELTEHub.invoke('elteSendMessage', targetuserid, message).done(function () { }).fail(function (error) {
            console.log('eLTESendMessage Error: ' + error);
        });
    },
    SendMessageWithAttachments: function (targetuserid, message, attachmentlist) {
        ELTEHub.invoke('elteSendMessageWithAttachments', targetuserid, message, attachmentlist).done(function () { }).fail(function (error) {
            console.log('eLTESendMessageWithAttachments Error: ' + error);
        });
    },
    Finalize: function () {
        ELTEHub.invoke('elteFinalize').done(function () { }).fail(function (error) {
            console.log('eLTEFinalize Error: ' + error);
        });
    },
}

var ScreenRecord = {
    Init: function (MiddlewareUrl, callBack, callBackServer) {
        var connection = $.hubConnection(MiddlewareUrl);
        ScreenRecordHub = connection.createHubProxy('screenRecordHub');

        ScreenRecordHub.on('sendMessage', function (eventType, message) {

            callBackServer(eventType, message);
            console.log(eventType + ': ' + JSON.stringify(message));
        });

        connection.start()
            .done(function () { callBack(true); })
            .fail(function () { callBack(false); });
    },
    StartRecording: function (filename, videocodec, bitrate) {
        ScreenRecordHub.invoke('startRecording', filename, videocodec, bitrate).done(function () { }).fail(function (error) {
            console.log('startRecording Error: ' + error);
        });
    },
    StopRecording: function () {
        ScreenRecordHub.invoke('stopRecording').done(function () { }).fail(function (error) {
            console.log('stopRecording Error: ' + error);
        });
    },
};
