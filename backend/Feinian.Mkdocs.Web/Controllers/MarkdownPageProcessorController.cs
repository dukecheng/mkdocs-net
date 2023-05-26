using AgileLabs;
using AgileLabs.AspNet.Sessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Niusys.Docs.Core.ProjectHttpClients;
using Niusys.Docs.Web.MarkdownServices;
using Niusys.Docs.Web.MarkdownServices.Utilities;
using Niusys.Docs.Web.Models;

namespace Niusys.Docs.Web.Controllers
{

    /// <summary>
    /// A generic controller implementation for processing Markdown
    /// files directly as HTML content
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MarkdownPageProcessorController : Controller
    {
        public MarkdownConfiguration MarkdownProcessorConfig { get; }

        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMemoryCache _memoryCache;
        private readonly IWorkContext _workContext;

        public MarkdownPageProcessorController(
            IWebHostEnvironment hostingEnvironment,
            MarkdownConfiguration config,
            IMemoryCache memoryCache, IWorkContext workContext)
        {
            MarkdownProcessorConfig = config;
            _memoryCache = memoryCache;
            _workContext = workContext;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Route("markdownprocessor/attachment")]
        public async Task<IActionResult> Attachment([FromServices] IRequestSession requestSession)
        {
            var model = HttpContext.Items["MarkdownProcessor_Model"] as MarkdownModel;
            if (model == null)
                throw new InvalidOperationException(
                    "This controller is not accessible directly unless the Markdown Model is set");

            var relativePath = model.RelativePath;
            if (relativePath == null)
                return NotFound();

            var streamResult = await _memoryCache.GetOrCreateAsync($"mem:attachment:{model.DocProject.Name}:{model.ViewName}:{relativePath}", async options =>
            {
                var httpClient = AgileLabContexts.Context.RootServiceProvider.GetRequiredService<DocProjectHttpClientFactory>()
                    .CreateHttpClient(model.DocProject.HostType, _workContext);
                
                try
                {
                    options.ApplyDefaultCachePolicy();
                    var result = await httpClient.GetAttachment(model.DocProject, model.ViewName, relativePath);                
                    return result;
                }
                catch (Exception ex)
                {
                    return new Tuple<Stream, string>(null, ex.FullMessage());
                }
            });

            if (streamResult.Item1 == null)
            {
                Response.StatusCode = 404;
                return Content(streamResult.Item2, "text/plain");
            }
            else
            {
                streamResult.Item1.SafeSeekToBegin();

                var outputStream = new MemoryStream();
                await streamResult.Item1.CopyToAsync(outputStream);
                outputStream.SafeSeekToBegin();
                return File(outputStream, streamResult.Item2);
            }

        }

        [HttpGet]
        [Route("markdownprocessor/markdownpage")]
        public async Task<IActionResult> MarkdownPage([FromServices] IRequestSession requestSession)
        {
            // Model saved in middleware processing
            var model = HttpContext.Items["MarkdownProcessor_Model"] as MarkdownModel;
            if (model == null)
                throw new InvalidOperationException(
                    "This controller is not accessible directly unless the Markdown Model is set");

            var relativePath = model.RelativePath;
            if (relativePath == null)
                return NotFound();
            var inhirtQuery = new Dictionary<string, string>();
            if (Request.Query.ContainsKey("view"))
            {
                inhirtQuery.Add("view", Request.Query["view"]);
            }
            var remotePath = $"{model.DocProject.ProjectPath}/raw/{model.ViewName}/{model.DocProject.WikiFolder}/{relativePath}";


            var rawMarkdown = await _memoryCache.GetOrCreateAsync($"mem:mkpage:{model.DocProject.Name}:{model.ViewName}{relativePath}", async options =>
            {
               
                var httpClient = AgileLabContexts.Context.RootServiceProvider.GetRequiredService<DocProjectHttpClientFactory>()
                        .CreateHttpClient(model.DocProject.HostType, _workContext);
                try
                {
                    var content = await httpClient.GetMarkdownFile(model.DocProject, model.ViewName, relativePath);
                    var baseUrl = $"{requestSession.HostWithScheme}/{model.RequestBasePath}/{model.RelativePath}";

                    content = MarkdownUtilities.FixupMarkdownRelativePaths(content, baseUrl, inhirtQuery);

                    options.ApplyDefaultCachePolicy();

                    return content;
                }
                catch (Exception ex)
                {
                    return ex.FullMessage();
                }
            });

            var markdown = Markdown.Parse(rawMarkdown, false, false, false);

            // set title, raw markdown, yamlheader and rendered markdown
            MarkdownHelper.ParseMarkdownToModel(markdown, rawMarkdown, model);
            return View(MarkdownConfiguration.DefaultMarkdownViewTemplate, model);
        }
    }

    public static class MemcacheEntryExtensions
    {
        public static void ApplyDefaultCachePolicy(this ICacheEntry options)
        {
            options.Size = 1;
            options.SlidingExpiration = TimeSpan.FromSeconds(15);
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
        }
    }
}
