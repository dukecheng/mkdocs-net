using Niusys.Docs.Core.DataStores;
using Niusys.Docs.Core.Projects;
using Niusys.Docs.Core.Utils;
using Niusys.Docs.Web.MarkdownServices;
using Niusys.Docs.Web.Models;

namespace Niusys.Docs.Web.Middlewares
{
    /// <summary>
    /// Middleware that allows you to serve static Markdown files from disk
    /// and merge them using a configurable View template.
    /// </summary>
    public class MarkdownPageProcessorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly MarkdownConfiguration _configuration;

        private readonly IWebHostEnvironment _env;
        private readonly MkdocsDatabase _mkdocsDatabase;

        public MarkdownPageProcessorMiddleware(RequestDelegate next,
            MarkdownConfiguration configuration,
            IWebHostEnvironment _env,
            MkdocsDatabase mkdocsDatabase
        )
        {
            _next = next;
            _configuration = configuration;
            this._env = _env;
            _mkdocsDatabase = mkdocsDatabase;
        }

        public Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (string.IsNullOrEmpty(path))
                return _next(context);

            var pathSegments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (!pathSegments.Any())
                return _next(context);

            var docRepName = pathSegments[0];
            var docRep = _mkdocsDatabase.Get<DocProject>(x => x.RequestPath == docRepName);
            if (docRep == null)
                return _next(context);

            path = string.Join('/', pathSegments.Skip(1).ToList());

            var viewName = context.Request.Query.ContainsKey("view") ? context.Request.Query["view"].ToString() : docRep.DefaultView;

            if (!path.Any())
            {
                path = docRep.IndexFile;
            }

            bool hasExtension = !string.IsNullOrEmpty(Path.GetExtension(path));
            if (!hasExtension)
            {
                path += $"/{docRep.IndexFile}";
                path = PathHelper.NormalizePath(path);
            }

            bool hasMdExtension = path.EndsWith(".md", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".markdown", StringComparison.OrdinalIgnoreCase);

            var model = new MarkdownModel
            {
                DocProject = docRep,                
                RequestBasePath = docRepName,
                ViewName = viewName,
                RelativePath = path
            };

            // push the model into the context for controller to pick up
            context.Items["MarkdownProcessor_Model"] = model;
            context.Request.Path = hasMdExtension ? (PathString)"/markdownprocessor/markdownpage" : (PathString)"/markdownprocessor/attachment";

            return _next(context);
        }
    }
}
