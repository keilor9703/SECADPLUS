var urlImprimir = "../ReportesWeb/Reportes.aspx?REPORTE=EXCEPCIONES&RF=_RF";

function GenerarReporte() {
    var UsuarioOid = $("#hdIdOid").val();
    var RangoFecha = $("#RangoFecha").val();

    var Ip = $("#txt_Ip").val();
    var Id = $("#txt_Id").val();
    var url = urlImprimir;
    //lee los parámetros ingresados por el usuario
    if (RangoFecha != "" && RangoFecha != null) {
        url = url.replace("_RF", RangoFecha);
    }
    if (UsuarioOid != "" && UsuarioOid != null) {
        url += "&US=" + UsuarioOid;
    }
    if (Ip != "" && Ip != null) {
        url += "&IP=" + Ip;
    }
    if (Id != "" && Id != null) {
        url += "&ID=" + Id;
    }
    //console.log(url);
    window.open(url, '_blank');
}
function LimpiarUsuario() {
    $("#txt_Usuario").val("");
    $("#hdIdOid").val("");
}
//Permite asignar como parametro un usuario seleccionado
function CargarUsuario(Codigo, Nombre) {
    $("#txt_Usuario").val(Nombre);
    $("#hdIdOid").val(Codigo);
    //Evento al presionar un checkbox on switch
    $('input[name="cbxVigente"]').on('switchChange.bootstrapSwitch', function (event, state) {
        if (state) {
            $("#hdVigente").val(1);
        } else {
            $("#hdVigente").val(0);
        }
    });
}

$(function () {
    AplicarDateRangePicker("#RangoFecha");
    $("#btnGenerar").click(function () {
        GenerarReporte();
    });
})
//AplicarControlFecha("#fecha_desde", null);
//AplicarControlFecha("#fecha_hasta", null);
//Permite configurar el control de la tabla de usuarios OID
function CargarTablaUsuarios() {
    var parametrosAjax = [
                { "data": "Dummy", "orderable": false, "width": "0.5%" },
                { "data": "Boton", "orderable": false },
                { "data": "UserName", "orderable": true },
                { "data": "UsuarioSecad", "orderable": true },
                { "data": "Vigente", "orderable": true }

    ];
    AplicarPluginTablaAjax("tablaUsuarios", urlObtenerUsuarios, parametrosAjax);
}