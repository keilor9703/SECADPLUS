using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using SECADPLUS.Models;

[assembly: OwinStartupAttribute(typeof(SECADPLUS.Startup))]
namespace SECADPLUS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }        
    }
}
