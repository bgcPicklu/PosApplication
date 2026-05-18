using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PosApplication.Startup))]
namespace PosApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
