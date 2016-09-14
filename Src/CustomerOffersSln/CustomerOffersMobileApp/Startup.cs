using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(CustomerOffersMobileApp.Startup))]

namespace CustomerOffersMobileApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}