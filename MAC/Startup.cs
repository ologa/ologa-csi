using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MAC.Startup))]
namespace MAC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
