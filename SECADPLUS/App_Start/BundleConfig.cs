using System.Web.Optimization;

namespace SECADPLUS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=1201862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new StyleBundle("~/bundle/LoginCss").Include(
                      "~/Content/icons/icomoon/styles.css",
                      "~/Content/personalizado.css",
                      "~/Content/bootstrap.css",
                      "~/Content/core.css",
                      "~/Content/components.css",
                      "~/Content/colors.css",
                      "~/Content/bootstrap-datetimepicker.min.css"
                      ));
            bundles.Add(new ScriptBundle("~/bundle/LoginJs").Include(
                    "~/Scripts/plugins/loaders/pace.min.js",
                    "~/Scripts/core/libraries/jquery.min.js",
                    "~/Scripts/core/libraries/bootstrap.min.js",
                    "~/Scripts/plugins/loaders/blockui.min.js",
                    "~/Scripts/plugins/forms/validation/validate.min.js",
                    "~/Scripts/plugins/forms/styling/uniform.min.js",
                    "~/Scripts/custom/LoginController.js",
                    "~/Scripts/pages/login_validation.js",
                    "~/Scripts/plugins/ui/ripple.min.js",
                    "~/Scripts/plugins/forms/selects/select2.min.js"));
            bundles.Add(new StyleBundle("~/bundle/PrincipalCss").Include(
                     "~/Content/icons/icomoon/styles.css",
                     "~/Content/bootstrap.css",
                     "~/Content/core.css",
                     "~/Content/components.css",
                     "~/Content/colors.css",
                     "~/Content/extras/animate.min.css",
                     "~/Content/chosenl/bootstrap-chosen.css",
                     "~/Content/personalizado.css"
                     ));
            bundles.Add(new StyleBundle("~/bundle/MapaArcGISCss").Include(
                     "~/Content/clases_mapa/arcgis_js_42/css/main.css",
                     "~/Content/clases_mapa/css/main.css",
                     "~/Content/clases_mapa/css/visor_mapa.css"
                     ));
            bundles.Add(new Bundle("~/bundle/MapaArcGISJs").Include(
                   "~/Scripts/clases_mapa/init.js",
                   "~/Scripts/clases_mapa/custom.js",
                   "~/Scripts/clases_mapa/calcular_ruta.js",
                   "~/Scripts/clases_mapa/construccion_figuras.js",
                   "~/Scripts/clases_mapa/iniciar_mapa.js"
                   )
               );
            bundles.Add(new ScriptBundle("~/bundle/PrincipalJs").Include(
                    "~/Scripts/plugins/loaders/pace.min.js",
                    "~/Scripts/core/libraries/jquery.min.js",
                   "~/Scripts/core/libraries/jquery_ui/interactions.min.js",
                    "~/Scripts/core/libraries/bootstrap.min.js",
                    "~/Scripts/plugins/loaders/blockui.min.js",
                    "~/Scripts/custom/PrincipalController.js",
                    "~/Scripts/core/app.js",
                    "~/Scripts/plugins/forms/validation/validate.min.js",
                    "~/Scripts/plugins/forms/styling/uniform.min.js",
                   "~/Scripts/jquery.signalR-2.2.2.min.js",
                    "~/Scripts/custom/gotopbutton.js",
                    "~/Scripts/Huawei/IntegracionHuawei.js",
                    "~/Scripts/Huawei/middlewareClient.js",
                    "~/Scripts/custom/jqredirect.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundle/HubsControllerJs").Include("~/Scripts/custom/SecadPlusHubsController.js"));
            bundles.Add(new ScriptBundle("~/bundle/MapaBaseJs").Include(
                    "~/Scripts/plugins/loaders/pace.min.js",
                    "~/Scripts/core/libraries/jquery.min.js",
                   "~/Scripts/core/libraries/jquery_ui/interactions.min.js",
                    "~/Scripts/core/libraries/bootstrap.min.js",
                    "~/Scripts/plugins/loaders/blockui.min.js",
                    "~/Scripts/core/app.js",
                    "~/Scripts/plugins/forms/validation/validate.min.js",
                    "~/Scripts/plugins/forms/styling/uniform.min.js",
                    "~/Scripts/custom/gotopbutton.js"
                    ));
            bundles.Add(new Bundle("~/bundle/TablasJs").Include(
                   "~/Scripts/plugins/tables/datatables/datatables.min.js",
                   "~/Scripts/plugins/tables/datatables/extensions/responsive.min.js",
                   "~/Scripts/custom/fnFilterOnReturn.js",
                   "~/Scripts/plugins/forms/selects/select2.min.js",
                   "~/Scripts/Custom/InicializadorTabla.js"
                   )
               );
            bundles.Add(new ScriptBundle("~/bundle/FormularioJs").Include(
                    "~/Scripts/pages/form_select2.js",
                    "~/Scripts/plugins/forms/selects/select2.min.js",
                    "~/Scripts/pages/form_layouts.js",
                    "~/Scripts/core/libraries/jquery_ui/interactions.min.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundle/InputsJs").Include(
                    "~/Scripts/plugins/forms/styling/switchery.min.js",
                    "~/Scripts/plugins/forms/inputs/touchspin.min.js",
                    "~/Scripts/pages/form_input_groups.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundle/CalendarioJs").Include(
                    "~/Scripts/plugins/notifications/jgrowl.min.js",
                    "~/Scripts/plugins/ui/moment/moment.min.js",
                    "~/Scripts/plugins/pickers/daterangepicker.js",
                    "~/Scripts/plugins/pickers/anytime.min.js",
                    "~/Scripts/plugins/pickers/pickadate/picker.js",
                    "~/Scripts/plugins/pickers/pickadate/picker.date.js",
                    "~/Scripts/plugins/pickers/pickadate/picker.time.js",
                    "~/Scripts/plugins/pickers/pickadate/legacy.js",
                    "~/Scripts/pages/picker_date.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundle/DatePickerJs").Include(
                    "~/Scripts/plugins/notifications/jgrowl.min.js",
                    "~/Scripts/plugins/ui/moment/moment.min.js",
                    "~/Scripts/plugins/pickers/pickadate/picker.js",
                    "~/Scripts/plugins/pickers/pickadate/picker.date.js",
                    "~/Scripts/plugins/pickers/pickadate/legacy.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundle/ElementosMaestras").Include(
                        "~/Scripts/Custom/ElementosMaestras.js"));
            //-----------------------
            bundles.Add(
                new ScriptBundle("~/bundle/Select2Js").Include(
                    "~/Scripts/core/libraries/jquery_ui/interactions.min.js",
                    "~/Scripts/plugins/forms/selects/select2.min.js",
                    "~/Scripts/pages/form_select2.js"
                    )
                );
            bundles.Add(new ScriptBundle("~/bundle/CheckSwitchJs").Include(
                "~/Scripts/plugins/forms/styling/switch.min.js",
                "~/Scripts/custom/CheckBoxSwitch.js",
                "~/Scripts/custom/ElementosController.js"
                )
                );
            bundles.Add(new ScriptBundle("~/bundle/ModalsJs").Include(
                "~/Scripts/plugins/notifications/bootbox.min.js",
                "~/Scripts/plugins/notifications/sweet_alert.min.js",
                "~/Scripts/custom/ElementosController.js"
                )
                );
            bundles.Add(new ScriptBundle("~/bundle/PnotifyJs").Include(
                "~/Scripts/plugins/notifications/pnotify.min.js",
                "~/Scripts/custom/GestorAlertas.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/NivelesJs").Include(
                    "~/Scripts/custom/UnidadesController.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundle/LugaresControllerJs").Include(
                    "~/Scripts/custom/LugaresController.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundle/CustomTablesJs").Include(
                   "~/Scripts/custom/datatables.js"
                   ));
            bundles.Add(new ScriptBundle("~/bundle/SpinnerJs").Include(
                   "~/Scripts/plugins/buttons/spin.min.js",
                   "~/Scripts/plugins/buttons/ladda.min.js"
                   ));
            bundles.Add(new ScriptBundle("~/bundle/ModalsAlertJs").Include(
                "~/Scripts/plugins/notifications/bootbox.min.js",
                "~/Scripts/plugins/notifications/sweet_alert.min.js"
                )
                );
            bundles.Add(new ScriptBundle("~/bundle/ChosenJs").Include(
                "~/Scripts/chosenl/chosen.jquery.js",
                "~/Scripts/chosen/prism.js"
                )
                );
            bundles.Add(new StyleBundle("~/bundle/CarouselCss").Include(
                     "~/Content/owl_carousel/owl.carousel.css"
                     ));
            bundles.Add(new ScriptBundle("~/bundle/CarouselJs").Include(
                "~/Scripts/owl_carousel/owl.carousel.js",
                "~/Scripts/custom/HomeController.js"
                )
                );
            bundles.Add(new ScriptBundle("~/bundle/UsuarioControllerJs").Include(
                "~/Scripts/custom/UsuarioController.js"
                ));
            //------------------------
            bundles.Add(new ScriptBundle("~/bundle/LugaresGeograficosControllerJs").Include(
                "~/Scripts/custom/LugaresGeograficosController.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/SitiosImportantesControllerJs").Include(
                "~/Scripts/custom/SitiosImportantesController.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/PersonasControllerJs").Include(
                "~/Scripts/custom/PersonasController.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/BannerControllerJs").Include(
                "~/Scripts/custom/BannerController.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/ConsultarControllerJs").Include(
                "~/Scripts/custom/ConsultarController.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/EntidadesControllerJs").Include(
                 "~/Scripts/custom/EntidadesController.js"
                 ));
            bundles.Add(new ScriptBundle("~/bundle/CheckBoxJs").Include(
                "~/Scripts/plugins/forms/styling/switchery.min.js",
                "~/Scripts/plugins/forms/styling/switch.min.js"
                )
                );
            bundles.Add(new ScriptBundle("~/bundle/MediosControllerJs").Include(
                 "~/Scripts/custom/MediosController.js"
                 ));
            bundles.Add(new ScriptBundle("~/bundle/CanalesBarriosControllerJs").Include(
     "~/Scripts/custom/CanalesBarriosController.js"
     ));
            bundles.Add(new ScriptBundle("~/bundle/EnvioMensajesJs").Include(
                "~/Scripts/custom/EnvioMensajes.js"
                ));

            bundles.Add(new ScriptBundle("~/bundle/IniciarEnvioMensajesJs").Include(
              "~/Scripts/custom/IniciarEnvioMensajes.js"
             ));
            bundles.Add(new ScriptBundle("~/bundle/ReferenciasSitioGrabaJs").Include(
                  "~/Scripts/custom/ReferenciasSitioGraba.js"
                 ));
            bundles.Add(new ScriptBundle("~/bundle/SitiosGrabacionCrearJs").Include(
                "~/Scripts/custom/SitiosGrabacion.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/EquiposSitioGrabacionJs").Include(
                "~/Scripts/custom/EquiposSitioGrabacion.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/CasosMaestrasJs").Include(
                "~/Scripts/custom/CasosMaestras.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/AnotacionesGeneralesJs").Include(
             "~/Scripts/custom/AnotacionesGenerales.js"
             ));
            bundles.Add(new ScriptBundle("~/bundle/DirectorioJs").Include(
          "~/Scripts/custom/Directorio.js"
          ));

            bundles.Add(new ScriptBundle("~/bundle/AdministrarDirectorioJs").Include(
       "~/Scripts/custom/AdministrarDirectorio.js"
       ));

            bundles.Add(new ScriptBundle("~/bundle/VerConexionesJs").Include(
           "~/Scripts/custom/VerConexiones.js"
           ));

            bundles.Add(new ScriptBundle("~/bundle/InfraccionesTransitoJs").Include(
                "~/Scripts/custom/InfraccionesTransito.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/TiposNovedadesJs").Include(
                "~/Scripts/custom/TiposNovedades.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/MaestraConductasJs").Include(
                "~/Scripts/custom/MaestraConductas.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/PonderacionesJs").Include(
                "~/Scripts/custom/Ponderaciones.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/VehiculosImplicadosJs").Include(
                "~/Scripts/custom/VehiculosImplicados.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/ReferenciasSECADJs").Include(
                "~/Scripts/custom/ReferenciasSECAD.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/PreguntasJs").Include(
                "~/Scripts/custom/Preguntas.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/AtencionCasosJs").Include(
                "~/Scripts/custom/AtencionCasos.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/ConsultaSitiosJs").Include(
                "~/Scripts/custom/ConsultaSitios.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/ConsultaSitiosJs").Include(
                "~/Scripts/custom/ConsultaSitios.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/PendientesDespachoJs").Include(
                "~/Scripts/custom/PendientesDespacho.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/IntegracionVmsJs").Include(
                "~/Scripts/custom/IntegracionVms.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/CerradosDespachoJs").Include(
                "~/Scripts/custom/CerradosDespacho.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/EventoMediosRutaJs").Include(
                "~/Scripts/custom/EventoMediosRuta.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/ControlDespachoJs").Include(
                "~/Scripts/custom/ControlDespacho.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/ControlRecepcionJs").Include(
                "~/Scripts/custom/ControlRecepcion.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/ComunicadosJs").Include(
                "~/Scripts/custom/Comunicados.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/LlamadaUsuario24hJs").Include(
                "~/Scripts/custom/LlamadaUsuario24h.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/LlamadasGralSalaJs").Include(
                "~/Scripts/custom/LlamadasGralSala.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/EncuestarJs").Include(
                "~/Scripts/custom/Encuestar.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/RevistasJs").Include(
                "~/Scripts/custom/Revistas.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/TurnosJs").Include(
                "~/Scripts/custom/TurnosController.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/ReporteExcepcionesJs").Include(
                "~/Scripts/custom/ReporteExcepciones.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/ReporteResumenSitioGrabaJs").Include(
               "~/Scripts/custom/ReporteResumenSitioGraba.js"
               ));

            bundles.Add(new ScriptBundle("~/bundle/ReporteAntecedentesUsuarioJs").Include(
              "~/Scripts/custom/ReporteAntecedentesUsuario.js"
              ));

            bundles.Add(new ScriptBundle("~/bundle/ReporteSeguimientoCasosJs").Include(
           "~/Scripts/custom/ReporteSeguimientoCasos.js"
           ));

            bundles.Add(new ScriptBundle("~/bundle/AnotacionesEventosJs").Include(
            "~/Scripts/custom/AnotacionesEventos.js"
            ));

            bundles.Add(new ScriptBundle("~/bundle/FotosEventosJs").Include(
               "~/Scripts/custom/FotosEventos.js"

               ));
            bundles.Add(new ScriptBundle("~/bundle/GestionarLlamadasPEntidadJs").Include(
           "~/Scripts/custom/GestionarLlamadasPEntidad.js"

           ));
            bundles.Add(new ScriptBundle("~/bundle/CrearFotosEventosJs").Include(
          "~/Scripts/custom/EventosFotosCrear.js"
          ));
            bundles.Add(new ScriptBundle("~/bundle/NovedadPatrullasEventoJs").Include(
           "~/Scripts/custom/NovedadPatrullasEvento.js"
           ));
            bundles.Add(new ScriptBundle("~/bundle/AccionesNovedadPatrullasJs").Include(
           "~/Scripts/custom/AccionesNovedadPatrulla.js"
           ));

            bundles.Add(new ScriptBundle("~/bundle/BienesRelacionadosEventoJs").Include(
               "~/Scripts/custom/BienesRelacionadosEvento.js"
               ));
            bundles.Add(new ScriptBundle("~/bundle/BienesRelacionadosCreateJs").Include(
             "~/Scripts/custom/BienesRelacionadosCreate.js"
             ));

            bundles.Add(new ScriptBundle("~/bundle/PersonaDijinJs").Include(
           "~/Scripts/custom/PersonaDijin.js"
           ));

            bundles.Add(new ScriptBundle("~/bundle/VehiculoDijinJs").Include(
              "~/Scripts/custom/VehiculoDijin.js"
              ));

            bundles.Add(new ScriptBundle("~/bundle/DespachosMujerJs").Include(
          "~/Scripts/custom/DespachosMujer.js"
          ));
            bundles.Add(new ScriptBundle("~/bundle/DespachosSocialJs").Include(
            "~/Scripts/custom/DespachosSocial.js"
            ));

            bundles.Add(new ScriptBundle("~/bundle/LocalizacionMediosJs").Include(
              "~/Scripts/custom/LocalizacionMedios.js"
              ));

            bundles.Add(new ScriptBundle("~/bundle/DateRangePickerJs").Include(
                    "~/Scripts/plugins/notifications/jgrowl.min.js",

                    "~/Scripts/plugins/ui/moment/moment.min.js",

                    "~/Scripts/plugins/pickers/daterangepick2.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundle/DateTimePickerJs").Include(
                //"~/Scripts/plugins/ui/moment/moment.min.js",
                "~/Scripts/plugins/ui/moment/moment_locales.min.js",
                 "~/Scripts/bootstrap-datetimepicker.js"
                 ));
            bundles.Add(new ScriptBundle("~/bundle/GenerarInformacionPlanosController").Include(
                "~/Scripts/custom/GenerarInformacionPlanosController.js"
                ));
            bundles.Add(new ScriptBundle("~/bundle/ServiciosPrestadosJs").Include(
           "~/Scripts/custom/ServiciosPrestadosController.js"
           ));
            bundles.Add(new ScriptBundle("~/bundle/PedidosJs").Include(
           "~/Scripts/custom/PedidosController.js"
           ));
            bundles.Add(new ScriptBundle("~/bundle/PedidosDocsJs").Include(
           "~/Scripts/custom/PedidosDocumentosController.js"
           ));
            bundles.Add(new ScriptBundle("~/bundle/SitiosGrabacionJs").Include(
           "~/Scripts/custom/SitiosGrabacionController.js"
           ));
            bundles.Add(new ScriptBundle("~/bundle/CNPJs").Include(
           "~/Scripts/custom/CNPController.js"
           ));
            bundles.Add(new ScriptBundle("~/bundle/CuadrantesJs").Include(
           "~/Scripts/custom/CuadrantesController.js"
           ));
            bundles.Add(new ScriptBundle("~/bundle/PerfilPermisoJs").Include(
           "~/Scripts/custom/PerfilPermisoController.js"
           ));
            bundles.Add(new ScriptBundle("~/bundle/CanalesControllerJs").Include(
           "~/Scripts/custom/CanalesController.js"
           ));
            bundles.Add(new ScriptBundle("~/bundle/IncidentesControllerJs").Include(
           "~/Scripts/custom/IncidentesController.js"
           ));
            bundles.Add(new ScriptBundle("~/bundle/VerAnotacionesControllerJs").Include(
           "~/Scripts/custom/VerAnotacionesController.js"
           ));
        }
    }
}