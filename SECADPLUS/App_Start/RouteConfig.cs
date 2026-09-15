using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace SECADPLUS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            string strNombreServidor = ConfigurationManager.AppSettings["ServerName"].ToString() + "/";
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Account", action = "Login" }
            );            
        }
    }
}