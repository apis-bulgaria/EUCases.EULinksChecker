using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EucasesLinkingService.PdfManipulation.Startup))]
namespace EucasesLinkingService.PdfManipulation
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
