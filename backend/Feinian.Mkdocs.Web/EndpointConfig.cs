using AgileLabs.AppRegisters;
using AgileLabs.WebApp.Hosting;

namespace Niusys.Docs.Web
{
    public class EndpointConfig : IEndpointConfig
    {
        public void ConfigureEndpoints(IEndpointRouteBuilder options, AppBuildContext buildContext)
        {   
            options.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}
