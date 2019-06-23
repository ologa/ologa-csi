using EFDataAccess.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MAC.Startup))]
namespace MAC
{
    public partial class Startup
    {
        public async void Configuration(IAppBuilder app)
        {
            // SchedulerFactory sf = new SchedulerFactory();
            // await sf.InitializeAsync();
            ConfigureAuth(app);
        }
    }
}
