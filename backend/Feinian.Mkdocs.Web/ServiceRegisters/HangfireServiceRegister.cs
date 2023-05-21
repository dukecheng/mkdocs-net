using AgileLabs;
using AgileLabs.AppRegisters;
using AgileLabs.WebApp.Hosting;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;

namespace Niusys.Docs.Web.ServiceRegisters
{
    public class HangfireServiceRegister : IServiceRegister, IRequestPiplineRegister
    {
        public int Order => 0;

        public RequestPiplineCollection Configure(RequestPiplineCollection piplineActions, AppBuildContext buildContext)
        {
            piplineActions.Register("hangfire", RequestPiplineStage.BeforeEndpointConfig, app =>
            {
                app.UseHangfireDashboard("/jobs", new DashboardOptions()
                {
                    DashboardTitle = "MKDocs's Jobs",
                    Authorization = new[] { new HangfireAuthorizationFilter() }
                });
            });
            return piplineActions;
        }

        public void ConfigureServices(IServiceCollection services, AppBuildContext buildContext)
        {
            services.AddSingleton<HangfireJobActivator>();
            services.AddSingleton<JobActivator>(serficeProvider => serficeProvider.GetRequiredService<HangfireJobActivator>());

            services.AddHangfire(configuration => configuration
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseColouredConsoleLogProvider()
                    .UseMemoryStorage());

            // Init JobStore.Current
            var serviceBuilder = services.BuildServiceProvider();
            _ = serviceBuilder.GetRequiredService<JobStorage>();
            JobActivator.Current = serviceBuilder.GetRequiredService<JobActivator>();

            services.AddHangfireServer();
            //if (!buildContext.HostEnvironment.IsDevelopment())
            //{

            //}           
        }

        private class HangfireJobActivator : JobActivator
        {
            private readonly IServiceProvider _serviceProvider;

            public HangfireJobActivator(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            }

            public override JobActivatorScope BeginScope(JobActivatorContext context)
            {
                var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
                var workContextScope = AgileLabContexts.Context.CreateScopeWithWorkContext(false);
                return new ServiceScope(workContextScope);
            }

            private class ServiceScope : JobActivatorScope
            {
                private readonly IWorkContextScope _scope;

                public ServiceScope(IWorkContextScope scope)
                {
                    _scope = scope ?? throw new ArgumentNullException(nameof(scope));
                    //scope.ServiceProvider.CreateScopeWithWorkContext(false);
                }

                public override object Resolve(Type type)
                {
                    return _scope.WorkContext.ServiceProvider.GetService(type);
                }

                public override void DisposeScope()
                {
                    _scope?.Dispose();
                }
            }
        }

        public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                var httpContext = context.GetHttpContext();

                // Allow all authenticated users to see the Dashboard (potentially dangerous).
                return true;
            }
        }
    }
}
