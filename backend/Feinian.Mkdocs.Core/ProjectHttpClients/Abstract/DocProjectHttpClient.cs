using AgileLabs;
using System.Text;
using UtfUnknown;
using Niusys.Docs.Core.Projects;
using Niusys.Docs.Core.Utils;

namespace Niusys.Docs.Core.ProjectHttpClients.Abstract
{
    public abstract class DocProjectHttpClient
    {
        private readonly HttpClient _httpClient;

        public DocProjectHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public abstract Task<string> GetMarkdownFile(DocProject project, string viewName, string relativePath);

        public abstract Task<Tuple<Stream, string>> GetAttachment(DocProject project, string viewName, string relativePath);

        public virtual Task<string> GetNavMenu(DocProject project, string viewName)
        {
            return GetMarkdownFile(project, viewName, "navmenu.md");
        }

        public abstract Task<List<string>> GetBranchListAsync(DocProject project, int pageIndex, int pageSize);

        public abstract Task<List<string>> GetTagListAsync(DocProject project, int pageIndex, int pageSize);

        private async Task<HttpResponseMessage> SendRequestInternalAsync(string url, List<KeyValuePair<string, string>> headers = null)
        {
            var uri = new Uri(url);
            url = PathHelper.NormalizePath(uri.PathAndQuery).TrimStart('\\', '/').TrimEnd('\\', '/');
            uri = new Uri($"{uri.Scheme}://{uri.Host}/{url}");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            if (headers != null && headers.Any())
            {
                foreach (var item in headers)
                {
                    httpRequestMessage.Headers.TryAddWithoutValidation(item.Key, item.Value);
                }
            }
            var responseMessage = await _httpClient.SendAsync(httpRequestMessage);
            if (responseMessage.IsSuccessStatusCode)
            {
                return responseMessage;
            }
            throw new Exception($"{responseMessage.StatusCode} {url}");
        }

        public async Task<string> GetMarkdownContentAsync(string url, List<KeyValuePair<string, string>> headers = null)
        {
            var httpResponse = await SendRequestInternalAsync(url, headers);
            var contentStream = await httpResponse.Content.ReadAsStreamAsync();
            var detectionResult = CharsetDetector.DetectFromStream(contentStream);
            contentStream.SafeSeekToBegin();
            using (var reader = new StreamReader(contentStream, detectionResult?.Detected?.Encoding ?? Encoding.Default))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<Tuple<Stream, string>> GetStreamContentAsync(string url, List<KeyValuePair<string, string>> headers = null)
        {
            var httpResponse = await SendRequestInternalAsync(url, headers);
            var contentStream = await httpResponse.Content.ReadAsStreamAsync();
            return new Tuple<Stream, string>(contentStream, httpResponse.Content.Headers?.ContentType?.MediaType ?? string.Empty);
        }
    }
}
