using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC2013.Startup))]
namespace MVC2013
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
