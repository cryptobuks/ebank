using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Presentation.Internal.Startup))]
namespace Presentation.Internal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
