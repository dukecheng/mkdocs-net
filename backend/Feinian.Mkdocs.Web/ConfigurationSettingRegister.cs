using AgileLabs;
using AgileLabs.Infrastructure;
using AgileLabs.TypeFinders;
using AgileLabs.WebApp.Hosting;
using Niusys.Docs.Core.Configuration;
using Niusys.Docs.Core.FileProviders;

namespace Niusys.Docs.Web
{
    public class ConfigurationSettingRegister : IServiceRegister
    {
        public int Order => 0;

        public void ConfigureServices(IServiceCollection services, AppBuildContext buildContext)
        {
            var defaultFileProvider = new AppFileProvider(buildContext.HostEnvironment);
            Singleton<IAppFileProvider>.Instance = defaultFileProvider;

            services.AddSingleton<IAppFileProvider>(defaultFileProvider);

            var typeFinder = Singleton<ITypeFinder>.Instance;
            #region Settings
            //add configuration parameters
            var configurations = typeFinder
                .FindClassesOfType<IConfig>()
                .Select(configType => (IConfig)Activator.CreateInstance(configType))
                .ToList();

            foreach (var config in configurations)
            {
                buildContext.Configuration.GetSection(config.ConfigTypeName()).Bind(config, options => options.BindNonPublicProperties = true);
            }
            var appSettings = AppSettingsHelper.SaveAppSettingsAsync(configurations, defaultFileProvider, false);
            services.AddSingleton(appSettings);
            #endregion
        }
    }
}
