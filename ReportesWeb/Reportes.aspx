<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reportes.aspx.cs" Inherits="SECADPLUS.Temporal.Page" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Reportes</title>
    <link rel="icon" href="../images/favicon.ico" type="image/x-icon">

    <link id="Link7" runat="server" rel="stylesheet" href="../Content/icons/icomoon/styles.css" type="text/css" />
    <link id="Link8" runat="server" rel="stylesheet" href="../Content/personalizado.css" type="text/css" />
    <link id="Link9" runat="server" rel="stylesheet" href="../Content/bootstrap.css" type="text/css" />
    <link id="Link10" runat="server" rel="stylesheet" href="../Content/core.css" type="text/css" />
    <link id="Link11" runat="server" rel="stylesheet" href="../Content/components.css" type="text/css" />
    <link id="Link12" runat="server" rel="stylesheet" href="../Content/colors.css" type="text/css" />
    <link id="Link13" runat="server" rel="stylesheet" href="../Content/extras/animate.min.css" type="text/css" />

    <!-- Archivos JS generales -->
    <script src="../Scripts/plugins/loaders/pace.min.js" type="text/javascript"></script>
    <script src="../Scripts/core/libraries/jquery.min.js" type="text/javascript"></script>
    <script src="../Scripts/core/libraries/bootstrap.min.js" type="text/javascript"></script>
    <script src="../Scripts/plugins/loaders/blockui.min.js" type="text/javascript"></script>

    <script src="../Scripts/core/app.js" type="text/javascript"></script>
    <script src="../Scripts/plugins/forms/validation/validate.min.js" type="text/javascript"></script>
    <script src="../Scripts/plugins/forms/styling/uniform.min.js" type="text/javascript"></script>
    <script src="../Scripts/custom/gotopbutton.js" type="text/javascript"></script>

    <!-- /archivos JS generales -->

    <!-- Archivos JS plantilla -->

    <!-- /archivos JS plantilla -->
</head>

<body>

    <!-- Header navbar -->

    <div class="navbar navbar-inverse color_secad">
        <div class="navbar-header">
            <!-- Logo policia -->
            <a class="navbar-brand" href="../Home/Index">
                <img src="../Images/escudo_policia.png" alt=""></a>
            <p class="titulo">
                Ministerio de Defensa Nacional<br>
                <b>Policia Nacional de Colombia</b>
            </p>

            <!-- Icono del menu -->
            <ul class="nav navbar-nav visible-xs-block">
                <li><a data-toggle="collapse" data-target="#navbar-mobile"><i class="icon-chevron-down"></i></a></li>
                <li><a class="sidebar-mobile-main-toggle"><i class="icon-paragraph-justify3"></i></a></li>
            </ul>
        </div>
        <div class="navbar-collapse collapse" id="navbar-mobile">
            <ul class="nav navbar-nav">
            </ul>
            <!-- Título central -->
            <div class="brand-secad">
                <img src="../Images/secad_w.png" alt="logo">
            </div>
            <!-- /Título central -->
            <!-- Sección derecha header -->
            <div class="navbar-right">
                <ul class="nav navbar-nav">
                    <!-- Botón  de notificaciones -->

                    <!-- /desplegable notificaciones -->
                </ul>
            </div>
        </div>
    </div>
    <!-- /header navbar -->
    

    <div class="content">
        <asp:Panel ID="panelContenedor" runat="server">
            <div class="row text-center">
                <div class="alert alert-warning alert-styled-left alert-arrow-left alert-bordered">
                    <button type="button" class="close" data-dismiss="alert"><span>×</span><span class="sr-only">Close</span></button>
                    <span class="text-semibold">
                        <asp:Label runat="server" ID="lbMensaje"></asp:Label></span>
                </div>
            </div>
        </asp:Panel>
        <div class="row">
            <div class="col-xs-12 col-md-12 col-lg-12">
                <div class="panel panel-flat">
                    <div class="panel-heading">
                        <h5 class="panel-title">Reporte SECAD:
							<a class="heading-elements-toggle"><i class="icon-more"></i></a></h5>

                        <div class="heading-elements">
                            <ul class="icons-list decad">
                                <li></li>
                                <li><a data-action="collapse"></a></li>
                            </ul>
                        </div>
                    </div>
                    <!-- Inicio de tabla -->
                    <div class="row">
                        <div class="col-md-12 col-xs-12">
                            <form id="form2" runat="server">
                                <asp:ScriptManager ID="ScriptManager2" runat="server">
                                </asp:ScriptManager>
                                <rsweb:ReportViewer ID="Reporte" runat="server" Width="100%" Height="1000px">
                                </rsweb:ReportViewer>
                            </form>
                        </div>
                    </div>
                    <!-- /inicio de tabla -->
                </div>
            </div>
        </div>
    </div>

    <!-- Footer -->
    <div class="footer-Secad text-center color_secad">
        <div class="row">
            <div class="col-md-4">
                <%--<img src="@Url.Content("~/Images/estrella_policia.png")" alt="nuevo país">--%>
            </div>
            <div class="col-md-4" style="font-family: montserrat bold; font-size: 14px;  color: white;">
                <p>
                    <a href="https://www.policia.gov.co/" target="_blank">Policía Nacional de Colombia</a>
                    Dirección General - Cra. 59 No. 26 - 21<br>
                    Centro Administrativo Nacional (CAN) Bogotá D.C.
                </p>
            </div>
            <div class="col-md-4">
                <%--<img src="@Url.Content("~/Images/ofite.png")" alt="nuevo país">--%>
            </div>
        </div>
    </div>
    <!-- /footer --> 
</body>
</html>