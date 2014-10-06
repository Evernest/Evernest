using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EvernestWeb.Startup))]
namespace EvernestWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
