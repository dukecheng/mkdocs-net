using System.Net;
using System.Web;
using AgileLabs;
using Newtonsoft.Json;
using Niusys.Docs.Core.ProjectHttpClients.Abstract;
using Niusys.Docs.Core.Projects;

namespace Niusys.Docs.Core.ProjectHttpClients
{
    public class GitlabDocProjectHttpClient : DocProjectHttpClient, ITransient
    {
        public GitlabDocProjectHttpClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public override async Task<string> GetMarkdownFile(DocProject project, string viewName, string relativePath)
        {
            //var remotePath = $"{project.ProjectPath}/-/raw/{viewName}/{project.WikiFolder}/{relativePath}";
            var remotePath = $"api/v4/projects/{project.ProjectPath}/repository/files/{HttpUtility.UrlEncode($"{project.WikiFolder}/{relativePath}")}/raw?ref={viewName}";
            var result = await GetMarkdownContentAsync($"{project.Host}/{remotePath}", project.RequestHeaders);
            return result;
        }

        public override async Task<Tuple<Stream, string>> GetAttachment(DocProject project, string viewName, string relativePath)
        {
            //http://192.168.100.155/quantum-scm/framework/framework.netcore/-/raw/dev/docs/navmenu.md
            // http://192.168.100.155/api/v4/projects/325/repository/files/docs%2Fnavmenu.md/raw?ref=dev
            //var remotePath = $"{project.ProjectPath}/-/raw/{viewName}/{project.WikiFolder}/{relativePath}";
            var remotePath = $"api/v4/projects/{project.ProjectPath}/repository/files/{HttpUtility.UrlEncode($"{project.WikiFolder}/{relativePath}")}/raw?ref={viewName}";
            return await GetStreamContentAsync($"{project.Host}/{remotePath}", project.RequestHeaders);
        }

        public override async Task<List<string>> GetBranchListAsync(DocProject project, int pageIndex, int pageSize)
        {
            var url = $"{project.Host}/api/v4/projects/{WebUtility.UrlEncode(project.ProjectPath)}/repository/branches?order_by=name&per_page={pageSize}&page={pageIndex}";
            var jsonContent = await GetMarkdownContentAsync(url, project.RequestHeaders);
            var branchList = JsonConvert.DeserializeObject<List<Branch>>(jsonContent);
            return branchList?.Select(x => x.name).ToList();
        }

        public override async Task<List<string>> GetTagListAsync(DocProject project, int pageIndex, int pageSize)
        {
            var url = $"{project.Host}/api/v4/projects/{WebUtility.UrlEncode(project.ProjectPath)}/repository/tags?order_by=name&per_page={pageSize}&page={pageIndex}";
            var jsonContent = await GetMarkdownContentAsync(url, project.RequestHeaders);
            var branchList = JsonConvert.DeserializeObject<List<Branch>>(jsonContent);
            return branchList?.Select(x => x.name).ToList();
        }

        private class Branch
        {
            public string name { get; set; }
        }
    }
}
