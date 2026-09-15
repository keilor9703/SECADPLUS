$(function () {
    $("#PEP_PERFIL_ID").change(function () {
        console.log("accede");
        console.log($(this).val());
        cargarElementos($(this).val());
    });
});

function cargarElementos(perfilId) {
    if(perfilId ===""){
        $("#PEP_ELEMENTO_ID").empty();
        $("#PEP_ELEMENTO_ID").trigger("chosen:updated");
        return;
    }
    var urlFinal = urlObtenerElementos.replace(/amp;/g, "")+"?dcPerfilId="+perfilId;
    var opcion = "<option value=\"VALOR\">NOMBRE</option>";
    var opciones = "";
    $("#PEP_ELEMENTO_ID").empty();
    $("#PEP_ELEMENTO_ID").trigger("chosen:updated");
    $.ajax({
        type: "POST",
        url: urlFinal,
        content: "application/json; charset=utf-8",
        dataType: "json",
        success: function (resultados) {
            if (resultados.result) {
                for (i = resultados.datos.length - 1; i >= 0; i--) {
                    opciones += opcion.replace("VALOR", resultados.datos[i].CODIGO).replace("NOMBRE", resultados.datos[i].NOMBRE);
                    //$("#fuerzaImprimir").append(OptionValue.replace("VALOR", dataFuerza[i].FUERZA).replace("NOMBRE", dataFuerza[i].DESCRIPCION));
                    //$("#MTDOFUERZA").append(OptionValue.replace("VALOR", dataFuerza[i].FUERZA).replace("NOMBRE", dataFuerza[i].DESCRIPCION));
                }                
                $("#PEP_ELEMENTO_ID").html(opciones);
                $("#PEP_ELEMENTO_ID").trigger("chosen:updated");
            } else {
                GestionarAlerta("warning", resultados.datos);             
            }
        },
        error: function (ex) {
            GestionarAlerta("Warning", 'No existe información de elementos disponibles.' + ex);
        }
    });
}
