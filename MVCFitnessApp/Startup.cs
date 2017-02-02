using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCFitnessApp.Startup))]
namespace MVCFitnessApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
