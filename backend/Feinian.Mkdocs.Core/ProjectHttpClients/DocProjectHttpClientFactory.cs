using Niusys.Docs.Core.Projects;
using AgileLabs;
using Microsoft.Extensions.DependencyInjection;
using Niusys.Docs.Core.ProjectHttpClients.Abstract;

namespace Niusys.Docs.Core.ProjectHttpClients
{
    public class DocProjectHttpClientFactory : ISingleton
    {
        public DocProjectHttpClient CreateHttpClient(HostType hostType, IWorkContext workContext)
        {
            Type type = typeof(NullDocProjectHttpClient);
            switch (hostType)
            {
                case HostType.Gogs:
                    type = typeof(GogosDocProjectHttpClient);
                    break;
                case HostType.Gitlab:
                    type = typeof(GitlabDocProjectHttpClient);
                    break;
                case HostType.Github:
                    break;
                default:
                    break;
            }

            if (type != null)
            {
                return workContext.ServiceProvider.GetRequiredService(type) as DocProjectHttpClient;
            }
            return null;
        }
    }
}
