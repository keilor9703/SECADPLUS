var opcionGeneral = "<option value=VALOR>DESC</option>";
var editar = false;
var glEditarUsuario = false;
var glOpcion = "CONSULTA";
var glUsuarioOid = "";
var htmlRolAsociado = "<div class='col-xs-6 col-md-6 col-sm-4 icon-s|elector text-left boderroles bg-asociado' onClick=\"AsoaciarRol('_ID','_NOMBRE',_ASOCIADO, this)\" ><i class='icon-checkmark4'></i>_NOMBRE</div>";
var htmlRol = "<div class='col-xs-6 col-md-6 col-sm-4 icon-s|elector text-left boderroles'  onClick=\"AsoaciarRol('_ID','_NOMBRE', _ASOCIADO, this)\">_NOMBRE</div>";
var glCanal = 0;
$(function () {
    //Oculta campos del formulario
    if ($("#hdOpcion").val() !== "") {
        $("#usuarioOid").prop("readonly", true);
        $("#btnRegresar").hide();
        $(".ocultar").addClass("hidden");
    }
    // Initialize
    var validator = $(".form-validate-jquery").validate({
        ignore: 'input[type=hidden], .select2-search__field', // ignore hidden fields
        errorClass: 'validation-error-label',
        successClass: 'validation-valid-label',
        highlight: function (element, errorClass) {
            $(element).removeClass(errorClass);
        },
        unhighlight: function (element, errorClass) {
            $(element).removeClass(errorClass);
        },
        // Different components require proper error label placement
        errorPlacement: function (error, element) {
            // Input with icons and Select2
            if (element.parents('div').hasClass('has-feedback') || element.hasClass('select2-hidden-accessible')) {
                error.appendTo(element.parent());
            }
            else {
                error.insertAfter(element);
            }
        },
        validClass: "validation-valid-label",
        success: function (label) {
            label.addClass("validation-valid-label").text("Correcto.")
        },
        required: "Este campo es obligatorio",
        rules: {
            password: {
                minlength: 5
            },
            repeat_password: {
                equalTo: "#password"
            },
            numbers: {
                number: true
            }
        },
        messages: {
            password: {
                required: "Debe ingresar una contraseña",
                minlength: "La contraseña debe tener al menos 5 caracteres",
            },
            repeat_password: {
                required: "Debe repetir la contraseña ingresada",
                equalTo: "Las contraseñas no coinciden",
            }
        }
    });
    //Oculta el boton de guardar
    if (!editar) {
        $("#btnGuardarCrear").hide();
        HabilitarGuardar();
    }
    //Asigna evento cuando se aplique doble click sobre el dpto
    $("#txtDptoId").dblclick(function () {
        $("#ModalDeptos").modal("toggle");
    });
    $("#txtUnidadId").dblclick(function () {
        $("#ModalUnidades").modal("toggle");
    });
    $("#txtCiudadId").dblclick(function () {
        $("#ModalCiudades").modal("toggle");
        CargarCiudades();
    });

    //Evento al presionar un checkbox on switch
    $('input[name="cbxVigente"]').on('switchChange.bootstrapSwitch', function (event, state) {
        if (state) {
            $("#hdVigente").val(true);
        } else {
            $("#hdVigente").val(false);
        }
    });
    //Evento al presionar un checkbox on switch
    $('input[name="cbxOid"]').on('switchChange.bootstrapSwitch', function (event, state) {
        if (state) {
            $("#hdOid").val(true);
        } else {
            $("#hdOid").val(false);
        }
    });
    $("#txtCedula").change(function () {
        //consulta los datos de policía
        ConsultarPolicia($(this).val());
        //Consulta si existe usuario empresarial
        ConsultarUsuEmpresarial($(this).val());
    });
    $("#btnGuardar").click(function () {
        var usuarioId = $("#hdUsuarioId").val();
        var sitio = $("#listSitio").val();
        var condicion = $("#listCondicion").val();
        var entidad = $("#listEntidad").val();
        var canal = $("#listCanal").val();
        //Verifica el contenido de la data antes de ser enviada

        if (sitio === null || sitio === "") sitio = 0;
        if (condicion === null || condicion === "") condicion = "";
        if (entidad === null || entidad === "") entidad = 0;
        if (canal === null || canal === "") canal = 0;
        if (canal === 0 && entidad === 0 && entidad === 0 && condicion === "" && sitio === 0) {
            GestionarAlerta("warning", "No se realizaron cambios al registro seleccionado.");
            $("#ModalCambioCanal").modal("toggle");
            return;
        }
        $.ajax({
            type: "POST",
            url: urlCambiarCanal,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: { strUsuario: usuarioId, intSitio: sitio, intEntidad: entidad, intCanal: canal, strCondicion: condicion },
            beforeSend: function () {
                $(".loader_decad").removeClass("hidden");
                $(".loader_back").removeClass("hidden");
            }
        }).done(function (resultado) {
            $(".loader_decad").addClass("hidden");
            $(".loader_back").addClass("hidden");
            if (resultado) {
                $("#ModalCambioCanal").modal("toggle");
                GestionarAlerta("success", "Usuario Actualizado de manera exitosa. La pantalla será recargada en 5 segundos...");
                setTimeout(function () { location.reload(); }, 5000);
            } else {
                //implementar cuando existan problemas
                GestionarAlerta("warning", "Ocurrio un problema.");
            }
        });
    });
})
//Permite presentar el modal con el formulario para el cambio de canal
function AbrirCambioCanal(IdUsuario, Sitio, Entidad, Canal, Condicion) {
    glCanal = Canal;
    $("#hdSitio").val(Sitio);
    $("#hdEntidad").val(Entidad);
    $("#hdCanal").val(Canal);
    $("#hdCondicion").val(Condicion);
    $("#hdUsuarioId").val(IdUsuario);
    $("#ModalCambioCanal").modal("show");
    CargarDatosAsociados();
}
//Permite cargar la información asociada a un usuario seleccionado
function CargarDatosAsociados() {
    $("#listCanal").empty();
    $("#listEntidad").empty();
    var sitio = $("#hdSitio").val();
    var entidad = $("#hdEntidad").val();
    var canal = $("#hdCanal").val();
    var condicion = $("#hdCondicion").val();
    var usuario = $("#hdUsuarioId").val();
    if (sitio > 0) {
        $("#listSitio").val(sitio);
        $("#listSitio").trigger("change");
        editar = true;
    }

    if (condicion !== "") {
        $("#listCondicion").val(condicion);
        $("#listCondicion").trigger("change");
    }
}
//Permite consultar las entidades asociadas a un sitio de grabación
function ObtenerEntidades() {
    //Consulta las entidades asociadas al sitio de grabacion
    //genera la url
    var sitioId = $("#listSitio").val();
    var entidad = $("#hdEntidad").val();
    //limpia variables
    $("#hdEntidad").val("");
    
    $("#hdCanal").val("");
    
    if (sitioId === null || sitioId === "") {
        $("#listEntidad").empty();
        $("#listCanal").empty();
        return;
    }
    $.ajax({
        type: "POST",
        url: urlObtenerEntidades,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { intSitio: sitioId },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        }
    }).done(function (datos) {
        $("#listEntidad").empty();
        $("#listCanal").empty();

        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
        $("#listEntidad").append(opcionGeneral.replace("VALOR", "").replace("DESC", ""));
        $.each(datos, function (i, item) {
            $("#listEntidad").append(opcionGeneral.replace("VALOR", datos[i].codigo).replace("DESC", datos[i].descripcion));
        });
        //Verifica si se edita y existe entidad asociada
        if (editar && entidad > 0) {
            $("#listEntidad").val(entidad);
            $("#listEntidad").trigger("change");
            editar = true;
        } else {
            editar = false;
        }
        $("#listEntidad").trigger("chosen:updated");
        $("#listCanal").trigger("chosen:updated");
    });
}
//Permite consultar los canales asociados a una entidad seleccionada
function ObtenerCanales() {
    //Consulta las entidades asociadas al sitio de grabacion
    //genera la url
    var entidadId = $("#listEntidad").val();

    var canal = glCanal;
    $("#hdCanal").val("");
    
    if (entidadId === null || entidadId === "") return;
    $("#hdEntidad").val(entidadId);
    $.ajax({
        type: "POST",
        url: urlObtenerCanales,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { intEntidad: entidadId },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        }
    }).done(function (datos) {
        $("#listCanal").empty();
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
        $("#listCanal").append(opcionGeneral.replace("VALOR", "").replace("DESC", ""));
        $.each(datos, function (i, item) {
            $("#listCanal").append(opcionGeneral.replace("VALOR", datos[i].codigo).replace("DESC", datos[i].descripcion));            
            if(canal == datos[i].codigo){
                $("#hdCanal").val(canal);
            }
        });
        if (editar && canal > 0) {
            $("#listCanal").val(canal);
            
        } else {
            editar = false;
        }
        $("#listCanal").trigger("chosen:updated");
    });
}
function CargarUsuarios() {
    var localParams = { strOpcion: glOpcion };
    var parametros = [
                    { "data": "Dummy", "orderable": false },
                    { "data": "Boton", "orderable": false },
                    { "data": "USUARIO", "orderable": true },
                    { "data": "IDENTIFICACION" },
                    { "data": "NombreEmpleado" },
                    { "data": "Grado" },
                    { "data": "Departamento" },
                    { "data": "Ciudad" },
                    { "data": "FechaDesde" },
                    { "data": "FechaHasta" },
                    { "data": "CondicionDesc" },
                    { "data": "Sitio" },
                    { "data": "Fuerza" },
                    { "data": "CanalDes" },
                    { "data": "UsuarioOid" }
    ];

    AplicarPluginTablaParams("tablaUsuarios", urlConsultaUsuarios, parametros, localParams);
}

function InicializarElementos(opcion) {
    switch (opcion) {
        case "CONSULTA":
            $.ajax({
                type: "POST",
                url: urlCambioCanal,
                content: "application/json; charset=utf-8",
                dataType: "html"
            }).done(function (contenido) {
                $("#contenedorCambioCanal").html(contenido);
                // Allow clear selection
                $('.select-clear').select2({
                    placeholder: "Seleccione una opción",
                    allowClear: true,
                    "language": {
                        "noResults": function () {
                            return "No se encontraron resultados.";
                        }
                    },
                });
            });
            CargarUsuarios();
            break;
        case "CREAR":
            $(".chosen-clear").chosen({ allow_single_deselect: true });
            break;
        default:
            $(".chosen-clear").chosen({ allow_single_deselect: true });
            glOpcion = opcion;
            CargarUsuarios();
            break;
    }
}
function InicializarCreacion() {
    console.log(editar);
    if (editar) {
        if ($("#hdOid").val() === "True") {
            $('#cbxOid').bootstrapSwitch('state', true)
        } else {
            $("#hdOid").val(false);
            $('#cbxOid').bootstrapSwitch('state', false)
        }
        if ($("#hdVigente").val() === "True") {
            $('#cbxVigente').bootstrapSwitch('state', true)
        } else {
            $("#hdVigente").val(false);
            $('#cbxVigente').bootstrapSwitch('state', false)
        }
    } else {
        if ($('#cbxVigente').bootstrapSwitch('state')) {
            $("#hdVigente").val(true);
        } else {
            $("#hdVigente").val(false);
        }
        if ($('#cbxOid').bootstrapSwitch('state')) {
            $("#hdOid").val(true);
        } else {
            $("#hdOid").val(false);
        }
    }

    if ($("#txtUsuario").val() !== "" || $("#txtUsuario").val() !== null) {
        $("#listSitio").trigger("change");
        if (editar) {
            InicializarEdicion();
        }
    }
}
function CargarDeptos() {
    var urlFinalDeptos = urlConsultarDptos.replace(/amp;/g, "");
    var parametros = [
                    { "data": "Dummy", "orderable": false, "width": "0.2%" },
                    { "data": "Boton", "orderable": false },
                    { "data": "CODIGO", "orderable": true },
                    { "data": "DESCRIPCION" },
                    { "data": "TIPO" }
    ];
    AplicarPluginTablaAjax("tblDeptos", urlFinalDeptos, parametros);
}
function CargarCiudades() {
    var dptoId = $("#txtDptoId").val();
    var ureCiudades = urlConsultarCiudades;
    var localParams = { dcDptoId: dptoId };

    if (dptoId === "") { return; }
    //Carga tabla de patrullas y canales disponibles
    if ($.fn.DataTable.isDataTable("#tblCiudades")) {
        $("#tblCiudades").DataTable().destroy();
        $("#tblCiudades tbody").empty();
    }
    ureCiudades = ureCiudades.replace(/amp;/g, "");
    //Agrega parametros de la consulta de canales
    //console.log(urlRadios);
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false, "width": "2%" },
                { "data": "CODIGO", "orderable": true },
                { "data": "DESCRIPCION", "orderable": true }
    ];
    AplicarPluginTablaParams("tblCiudades", ureCiudades, parametrosAjax, localParams);
}

function SeleccionarLugar(codigo, nombre, opcion) {
    switch (opcion) {
        case "DPTO":
            $("#txtDptoId").val(codigo);
            $("#txtDpto").val(nombre);
            $("#ModalDeptos").modal("toggle");
            $("#txtCiudadId").val("");
            $("#txtCiudad").val("");
            break;
        case "CIUDAD":
            $("#txtCiudadId").val(codigo);
            $("#txtCiudad").val(nombre);
            $("#ModalCiudades").modal("toggle");
            break;
    }
}
function CargarUnidades() {
    var urlFinalUnidades = urlConsultarUnidades.replace(/amp;/g, "");
    var parametros = [
                    { "data": "Dummy", "orderable": false, "width": "0.2%" },
                    { "data": "Boton", "orderable": false },
                    { "data": "CODIGO", "orderable": true },
                    { "data": "DESCRIPCION", "orderable": true },
                    { "data": "SIGLA", "orderable": true },
                    { "data": "FUERZA", "orderable": true }
    ];
    AplicarPluginTablaAjax("tblUnidades", urlFinalUnidades, parametros);
}

function SeleccionarUnidad(codigo, nombre, fuerza) {
    $("#txtUnidadId").val(codigo);
    $("#txtUnidad").val(nombre);
    $("#ModalUnidades").modal("toggle");
    $("#hdFuerza").val(fuerza);
}
function SeleccionarCanal(objeto) {
    $("#hdCanal").val($(objeto).val());
}
function ConsultarPolicia(cedula) {
    //Limpia nombre y grado
    $("#txtNombre").val("");
    $("#txtGrado").val("");
    $.ajax({
        type: "POST",
        url: urlConsultarPolicia,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strCedula: cedula },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        }
    }).done(function (resultado) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
        if (resultado.result) {
            $("#txtNombre").val(resultado.datos[0]);
            $("#txtGrado").val(resultado.datos[1]);
            HabilitarGuardar();
        } else {
            //implementar cuando existan problemas
            GestionarAlerta("warning", resultado.datos);
            $("#txtCedula").focus();
        }
    });
}
function ConsultarUsuEmpresarial(cedula) {
    $("#txtUsuarioOid").val("");
    $.ajax({
        type: "POST",
        url: urlConsultarUsuEmpresarial,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strCedula: cedula },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        }
    }).done(function (resultado) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
        if (resultado.result) {
            $("#usuarioOid").val(resultado.datos);
        } else {
            //implementar cuando existan problemas
            GestionarAlertaModal("warning", resultado.datos, "segundoContenedor");
        }
    });
}
function HabilitarGuardar() {
    //Oculta el boton de guardar

    if ($("#txtNombre").val() != "") {
        $("#btnGuardarCrear").show();
    }
}
function InicializarEdicion() {
    glEditarUsuario = true;
    //Desabilita la olbigatoriedad de la contraseña
    $("#password").removeAttr("required");
    $("#repeat_password").removeAttr("required");

    $("#txtUsuario").attr("readonly", "true");
    $("#form_usuarios").attr("action", urlEditarUsuario);
    HabilitarGuardar();
}

function AbrirCambioRol(usuario_oid_id) {
    if (usuario_oid_id === "") {
        GestionarAlerta("warning", "El usuario seleccionado no tiene asociado ningún usuario empresarial asociado.");
        return;
    }
    $("#contenedorRoles").empty();
    ConsultarRolesAsociados(usuario_oid_id);
    $("#ModalRoles").modal("toggle");
    glUsuarioOid = usuario_oid_id;
}
function AsoaciarRol(id, nombre, asocia, objeto) {
    $.ajax({
        type: "POST",
        url: urlAsociarRol,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { struUsuarioEmpresarial: glUsuarioOid, strRolId: id, asociado: asocia },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        }
    }).done(function (resultado) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
        if (resultado.result) {
            ConsultarRolesAsociados(glUsuarioOid);
            GestionarAlertaModal("success", resultado.datos, "modalAlerta");
        } else {
            //implementar cuando existan problemas
            GestionarAlertaModal("danger", resultado.datos, "modalAlerta");
        }
    });
}
//Consulta los roles asociados a un usuario
function ConsultarRolesAsociados(usuario_oid_id) {
    $("#contenedorRoles").empty();
    //Consulta los roles asociados
    $.ajax({
        type: "POST",
        url: urlConsultarRoles,
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: { strUsuarioEmpresarial: usuario_oid_id },
        beforeSend: function () {
            $(".loader_decad").removeClass("hidden");
            $(".loader_back").removeClass("hidden");
        }
    }).done(function (resultado) {
        $(".loader_decad").addClass("hidden");
        $(".loader_back").addClass("hidden");
        if (resultado.result) {
            $(resultado.datos).each(
                function (indice) {
                    if (this.Asociado === 1) {
                        $("#contenedorRoles").append(htmlRolAsociado.replace("_ID", this.Id).replace(/_NOMBRE/g, this.Name).replace("_ASOCIADO", this.Asociado));
                    } else {
                        $("#contenedorRoles").append(htmlRol.replace("_ID", this.Id).replace(/_NOMBRE/g, this.Name).replace("_ASOCIADO", this.Asociado));
                    }
                }
                );
        } else {
            //implementar cuando existan problemas
            GestionarAlertaModal("danger", resultado.datos, "segundoContenedor");
        }
    });
}