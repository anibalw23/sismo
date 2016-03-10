using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Monitoreo.Startup))]
namespace Monitoreo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
