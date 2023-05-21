using AgileLabs;
using Niusys.Docs.Core.ProjectHttpClients.Abstract;
using Niusys.Docs.Core.Projects;

namespace Niusys.Docs.Core.ProjectHttpClients
{
    public class NullDocProjectHttpClient : DocProjectHttpClient, ITransient
    {
        public NullDocProjectHttpClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public override async Task<Tuple<Stream, string>> GetAttachment(DocProject project, string viewName, string relativePath)
        {
            await Task.CompletedTask;
            return new Tuple<Stream, string>(null, string.Empty);
        }

        public override async Task<List<string>> GetBranchListAsync(DocProject project, int pageIndex, int pageSize)
        {
            await Task.CompletedTask;
            return new List<string>();
        }

        public override async Task<string> GetMarkdownFile(DocProject project, string viewName, string relativePath)
        {
            await Task.CompletedTask;
            return string.Empty;
        }

        public override async Task<List<string>> GetTagListAsync(DocProject project, int pageIndex, int pageSize)
        {
            await Task.CompletedTask;
            return new List<string>();
        }
    }
}
