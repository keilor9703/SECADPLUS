/*
 * Integración SECAD - VMS ISS SecurOS (caso de uso 1).
 * Muestra las cámaras cercanas al caso y reproduce su video en vivo dentro de SECAD.
 * El video llega por WebRTC (protocolo WHEP) desde el gateway de video; solo visualización.
 *
 * Requiere definidas en la vista: urlVmsCamarasCercanas, urlVmsMiniatura, urlVmsAbrirVideo.
 */
var IntegracionVms = (function () {
    var conexionActual = null;
    var sesionWhep = null;
    var contexto = { sitio: null, llamada: null };

    function escapar(texto) {
        return $('<div/>').text(texto == null ? '' : String(texto)).html();
    }

    function crearModal() {
        if ($('#modalVmsCamaras').length) {
            return;
        }
        var html =
            '<div class="modal fade" id="modalVmsCamaras" tabindex="-1" role="dialog">' +
            '  <div class="modal-dialog modal-lg" role="document">' +
            '    <div class="modal-content">' +
            '      <div class="modal-header bg-info">' +
            '        <button type="button" class="close" data-dismiss="modal">&times;</button>' +
            '        <h6 class="modal-title"><i class="icon-camera"></i> Cámaras cercanas al caso <span id="vmsTituloCaso"></span></h6>' +
            '      </div>' +
            '      <div class="modal-body">' +
            '        <div id="vmsMensaje" class="alert alert-warning" style="display:none"></div>' +
            '        <div class="row">' +
            '          <div class="col-md-5"><div id="vmsListaCamaras" style="max-height:480px;overflow-y:auto"></div></div>' +
            '          <div class="col-md-7">' +
            '            <div id="vmsNombreCamara" class="text-semibold" style="margin-bottom:6px">Seleccione una cámara para ver el video.</div>' +
            '            <video id="vmsVideo" autoplay muted playsinline controls style="width:100%;background:#000;min-height:240px"></video>' +
            '            <div id="vmsEstadoVideo" class="text-muted" style="margin-top:4px"></div>' +
            '          </div>' +
            '        </div>' +
            '      </div>' +
            '      <div class="modal-footer"><button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button></div>' +
            '    </div>' +
            '  </div>' +
            '</div>';
        $('body').append(html);
        $('#modalVmsCamaras').on('hidden.bs.modal', detenerVideo);
        $(window).on('beforeunload', detenerVideo);
    }

    function mostrarMensaje(texto) {
        if (texto) {
            $('#vmsMensaje').text(texto).show();
        } else {
            $('#vmsMensaje').hide().text('');
        }
    }

    function pintarCamaras(camaras) {
        var lista = $('#vmsListaCamaras').empty();
        if (!camaras.length) {
            mostrarMensaje('No hay cámaras dentro del radio de búsqueda de este caso.');
            return;
        }
        $.each(camaras, function (i, camara) {
            var estado = camara.enLinea
                ? '<span class="label label-success">En línea</span>'
                : '<span class="label label-default">Sin señal</span>';
            var miniatura = camara.enLinea
                ? '<img src="' + urlVmsMiniatura + '?idCamara=' + encodeURIComponent(camara.id) + '&t=' + new Date().getTime() +
                  '" style="width:120px;height:68px;object-fit:cover;background:#ddd" alt="" onerror="this.style.visibility=\'hidden\'"/>'
                : '<div style="width:120px;height:68px;background:#ddd"></div>';
            var fila = $(
                '<div class="media" style="cursor:pointer;padding:6px;border-bottom:1px solid #eee">' +
                '  <div class="media-left">' + miniatura + '</div>' +
                '  <div class="media-body">' +
                '    <div class="text-semibold">' + escapar(camara.nombre) + '</div>' +
                '    <div class="text-muted">' + escapar(camara.distancia) + ' m del caso</div>' + estado +
                '  </div>' +
                '</div>');
            fila.on('click', function () {
                lista.find('.media').css('background', '');
                fila.css('background', '#eef6fb');
                abrirVideo(camara);
            });
            lista.append(fila);
        });
    }

    function abrirCamarasCercanas(sitio, llamada) {
        if (!llamada) {
            alert('Seleccione primero un caso.');
            return;
        }
        contexto.sitio = sitio;
        contexto.llamada = llamada;
        crearModal();
        detenerVideo();
        mostrarMensaje(null);
        $('#vmsTituloCaso').text('N.° ' + llamada);
        $('#vmsNombreCamara').text('Seleccione una cámara para ver el video.');
        $('#vmsListaCamaras').html('<div class="text-muted">Consultando cámaras...</div>');
        $('#modalVmsCamaras').modal('show');
        $.ajax({
            url: urlVmsCamarasCercanas,
            type: 'POST',
            data: { intSitio: sitio, lgLlamada: llamada },
            success: function (respuesta) {
                if (!respuesta.result) {
                    $('#vmsListaCamaras').empty();
                    mostrarMensaje(respuesta.datos);
                    return;
                }
                pintarCamaras(respuesta.datos);
            },
            error: function () {
                $('#vmsListaCamaras').empty();
                mostrarMensaje('No fue posible consultar las cámaras cercanas.');
            }
        });
    }

    function abrirVideo(camara) {
        detenerVideo();
        $('#vmsNombreCamara').text(camara.nombre);
        if (!camara.enLinea) {
            $('#vmsEstadoVideo').text('La cámara no tiene señal en este momento.');
            return;
        }
        $('#vmsEstadoVideo').text('Conectando...');
        $.ajax({
            url: urlVmsAbrirVideo,
            type: 'POST',
            data: { idCamara: camara.id, intSitio: contexto.sitio, lgLlamada: contexto.llamada },
            success: function (respuesta) {
                if (!respuesta.result) {
                    $('#vmsEstadoVideo').text(respuesta.datos);
                    return;
                }
                reproducirWhep(respuesta.datos.urlWhep, respuesta.datos.token);
            },
            error: function () {
                $('#vmsEstadoVideo').text('No fue posible abrir el video.');
            }
        });
    }

    function esperarCandidatos(pc) {
        return new Promise(function (resolver) {
            if (pc.iceGatheringState === 'complete') {
                resolver();
                return;
            }
            var temporizador = setTimeout(resolver, 2000);
            pc.addEventListener('icegatheringstatechange', function () {
                if (pc.iceGatheringState === 'complete') {
                    clearTimeout(temporizador);
                    resolver();
                }
            });
        });
    }

    function reproducirWhep(urlWhep, token) {
        if (!window.RTCPeerConnection || !window.fetch) {
            $('#vmsEstadoVideo').text('Este navegador no soporta la reproducción de video (WebRTC). Actualice el navegador.');
            return;
        }
        var url = urlWhep + '?token=' + encodeURIComponent(token);
        var pc = new RTCPeerConnection();
        conexionActual = pc;
        pc.addTransceiver('video', { direction: 'recvonly' });
        pc.addTransceiver('audio', { direction: 'recvonly' });
        pc.ontrack = function (evento) {
            var video = document.getElementById('vmsVideo');
            if (video.srcObject !== evento.streams[0]) {
                video.srcObject = evento.streams[0];
            }
            $('#vmsEstadoVideo').text('En vivo');
        };
        pc.oniceconnectionstatechange = function () {
            if (pc === conexionActual && (pc.iceConnectionState === 'failed' || pc.iceConnectionState === 'disconnected')) {
                $('#vmsEstadoVideo').text('Se perdió la conexión con el video. Seleccione la cámara de nuevo.');
            }
        };
        pc.createOffer()
            .then(function (oferta) { return pc.setLocalDescription(oferta); })
            .then(function () { return esperarCandidatos(pc); })
            .then(function () {
                return fetch(url, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/sdp', 'Authorization': 'Bearer ' + token },
                    body: pc.localDescription.sdp
                });
            })
            .then(function (respuesta) {
                if (respuesta.status !== 201) {
                    throw new Error(respuesta.status === 401 ? 'El enlace de video venció o no es válido.' : 'El gateway de video respondió ' + respuesta.status + '.');
                }
                var ubicacion = respuesta.headers.get('Location');
                if (ubicacion) {
                    sesionWhep = new URL(ubicacion, urlWhep).toString();
                }
                return respuesta.text();
            })
            .then(function (respuestaSdp) {
                if (pc !== conexionActual) {
                    return;
                }
                return pc.setRemoteDescription({ type: 'answer', sdp: respuestaSdp });
            })
            .catch(function (error) {
                if (pc === conexionActual) {
                    $('#vmsEstadoVideo').text(error && error.message ? error.message : 'No fue posible reproducir el video.');
                    detenerVideo(true);
                }
            });
    }

    function detenerVideo(conservarMensaje) {
        if (sesionWhep && window.fetch) {
            // Libera la sesión en el gateway para que deje de leer la cámara.
            fetch(sesionWhep, { method: 'DELETE' }).catch(function () { });
        }
        sesionWhep = null;
        if (conexionActual) {
            conexionActual.close();
            conexionActual = null;
        }
        var video = document.getElementById('vmsVideo');
        if (video) {
            video.srcObject = null;
        }
        if (conservarMensaje !== true) {
            $('#vmsEstadoVideo').text('');
        }
    }

    return {
        abrirCamarasCercanas: abrirCamarasCercanas
    };
})();
