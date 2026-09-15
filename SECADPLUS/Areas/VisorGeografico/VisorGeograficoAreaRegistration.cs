using System.Web.Mvc;

namespace SECADPLUS.Areas.VisorGeografico
{
    public class VisorGeograficoAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "VisorGeografico";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "VisorGeografico_default",
                "VisorGeografico/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}