using AgileLabs;
using AgileLabs.WebApp;
using AgileLabs.WebApp.Hosting;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Niusys.Docs.Core;
using Niusys.Docs.Core.Configuration;
using Niusys.Docs.Core.DataStores;
using Niusys.Docs.Core.Projects;

AgileLabApplication.StartApplication<AppBuildOptions>();
public class AppBuildOptions : DefaultMvcApplicationOptions
{
    public AppBuildOptions()
    {
        base.MvcBuilderCreateFunc = (IServiceCollection serviceCollection, Action<MvcOptions> action) => serviceCollection.AddControllersWithViews(action);
        ConfigureWebApplicationBuilder += (builder, buildContext) =>
        {
            builder.Configuration.AddJsonFile(ConfigurationDefaults.AppSettingsFilePath, true, true);
            if (!string.IsNullOrEmpty(builder.Environment?.EnvironmentName))
            {
                var path = string.Format(ConfigurationDefaults.AppSettingsEnvironmentFilePath, builder.Environment.EnvironmentName);
                builder.Configuration.AddJsonFile(path, true, true);
            }
        };

        ConfigureStartupInitService += (sp) =>
        {
            using (var scope = sp.CreateScope())
            {
                var rep = scope.ServiceProvider.GetRequiredService<MkdocsDatabase>();

                rep.StartupInspect(db =>
                {
                    db.GetCollection<DocProject>().EnsureIndex(x => x.Name, true);
                    db.GetCollection<User>().EnsureIndex(x => x.Email, true);
                });
            }
        };
    }

    public class HostedServiceRegister : IServiceRegister
    {
        public int Order => 100;

        public void ConfigureServices(IServiceCollection services, AppBuildContext buildContext)
        {
            services.AddHostedService<StartupHostedService>();
        }
    }

    public class StartupHostedService : IHostedService
    {
        private readonly ILogger<StartupHostedService> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public StartupHostedService(ILogger<StartupHostedService> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(nameof(StartAsync));
            RecurringJob.AddOrUpdate<DocProjectService>("Update Branch", (service) => service.BranchListSyncTrigger(null, default), "* */20 * * * *");
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(nameof(StopAsync));
            await Task.CompletedTask;
        }
    }
}

