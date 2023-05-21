using AgileLabs;
using AgileLabs.WebApp.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Niusys.Docs.Core.Configuration;
using Niusys.Docs.Core.Projects;

namespace Niusys.Docs.Core.DataStores
{
    public class DatabaseRegister : IServiceRegister
    {
        public int Order => 0;

        public void ConfigureServices(IServiceCollection services, AppBuildContext buildContext)
        {
            services.TryAddSingleton(sp =>
            {
                var connectionString = buildContext.Configuration.GetConnectionString("Default");
                if (connectionString == null)
                    throw new Exception("未配置数据库连接字符串");
                var dbParms = connectionString.Split(';');

                var newDbParms = new List<string>();
                foreach (var item in dbParms)
                {
                    var itemParms = item.Split('=');
                    switch (itemParms[0].ToLower())
                    {
                        case "filename":
                            newDbParms.Add(string.Join('=', itemParms[0], Path.Combine(ConfigurationDefaults.AppDataPath, itemParms[1])));
                            break;
                        default:
                            newDbParms.Add(item);
                            break;
                    }
                }

                return new MkdocsDatabase(string.Join(';', newDbParms.ToArray()));
            });
        }
    }
}
