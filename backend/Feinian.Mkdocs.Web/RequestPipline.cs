using AgileLabs;
using AgileLabs.AppRegisters;
using AgileLabs.AspNet.ClientAppServices;
using AgileLabs.WebApp.Hosting;

namespace Niusys.Docs.Web
{
    public class RequestPipline : IRequestPiplineRegister
    {
        public RequestPiplineCollection Configure(RequestPiplineCollection piplineActions, AppBuildContext buildContext)
        {
            piplineActions.Register("Default", RequestPiplineStage.BeforeRouting, app =>
            {
                if (!buildContext.HostEnvironment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                }
                app.UseStaticFiles();           
            });

            piplineActions.Register("UseClientApp", RequestPiplineStage.BeforeRouting, app =>
            {
                app.UseClientApp();
            });

            piplineActions.Register("Authentication", RequestPiplineStage.BeforeEndpointConfig, app =>
            {
                app.UseAuthorization();
            });
            return piplineActions;
        }
    }
}
