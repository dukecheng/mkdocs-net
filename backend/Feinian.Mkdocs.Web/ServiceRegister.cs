using AgileLabs;
using AgileLabs.AspNet.ClientAppServices;
using AgileLabs.Securities;
using AgileLabs.WebApp.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.WebEncoders;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Niusys.Docs.Web
{
    public class ServiceRegister : IServiceRegister
    {
        public int Order => 0;

        public void ConfigureServices(IServiceCollection services, AppBuildContext buildContext)
        {
            services.AddNiusysSecurity(options =>
            {
                options.EncryptionKey = "2218EF6E-7D95-442F-B967-3979B00E9226";
            });
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });
            

            services.AddClientApp(options =>
            {
                options.RootPath = "ClientApp";

                var adminClientApp = ClientAppConfig.Create("/admin", "AdminWeb");
                if (adminClientApp.DefaultPageStaticFileOptions == null)
                    adminClientApp.DefaultPageStaticFileOptions = new StaticFileOptions();
                adminClientApp.DefaultPageStaticFileOptions.OnPrepareResponse = ctx =>
                {
                    // Cache static files for 12 hours
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=60, immutable");
                    ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddMinutes(1).ToString("R", CultureInfo.InvariantCulture));
                };

                options.ClientApps.Add(adminClientApp);
            });

            #region Mvc
            services.Configure<ApiBehaviorOptions>(options =>
            {
                // https://stackoverflow.com/questions/55289631/inconsistent-behaviour-with-modelstate-validation-asp-net-core-api
                options.SuppressModelStateInvalidFilter = true;
            });
            #endregion
        }
    }
}
