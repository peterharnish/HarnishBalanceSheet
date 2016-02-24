using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HarnishBalanceSheet.Startup))]
namespace HarnishBalanceSheet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
