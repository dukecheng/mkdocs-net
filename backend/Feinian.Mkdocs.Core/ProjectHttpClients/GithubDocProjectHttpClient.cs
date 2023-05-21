using AgileLabs;
using Niusys.Docs.Core.ProjectHttpClients.Abstract;
using Niusys.Docs.Core.Projects;

namespace Niusys.Docs.Core.ProjectHttpClients
{
    /// <summary>
    /// https://raw.githubusercontent.com/dukecheng/mkdocs-net/master/docs/README.md
    /// </summary>
    public class GithubDocProjectHttpClient : DocProjectHttpClient, ITransient
    {
        public GithubDocProjectHttpClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public override async Task<string> GetMarkdownFile(DocProject project, string viewName, string relativePath)
        {
            var remotePath = $"{project.ProjectPath}/{viewName}/{project.WikiFolder}/{relativePath}";
            var result = await GetMarkdownContentAsync($"{project.Host}/{remotePath}", project.RequestHeaders);
            return result;
        }

        public override async Task<Tuple<Stream, string>> GetAttachment(DocProject project, string viewName, string relativePath)
        {
            var remotePath = $"{project.ProjectPath}/{viewName}/{project.WikiFolder}/{relativePath}";
            return await GetStreamContentAsync($"{project.Host}/{remotePath}", project.RequestHeaders);
        }

        public override async Task<List<string>> GetBranchListAsync(DocProject project, int pageIndex, int pageSize)
        {
            await Task.CompletedTask;
            return new List<string>();
        }

        public override async Task<List<string>> GetTagListAsync(DocProject project, int pageIndex, int pageSize)
        {
            await Task.CompletedTask;
            return new List<string>();
        }

        private class Branch
        {
            public string name { get; set; }
        }
    }
}
