using AgileLabs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Niusys.Docs.Core.DataStores;
using Niusys.Docs.Core.ProjectHttpClients;
using Niusys.Docs.Core.Projects;
using Niusys.Docs.Web.Models;

namespace Niusys.Docs.Web.Components;

[ViewComponent(Name = "ViewList")]
public class ViewListViewComponent : ViewComponent
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ViewListViewComponent> _logger;
    private readonly DocProjectHttpClientFactory _docProjectHttpClientFactory;
    private readonly IWorkContext _workContext;
    private readonly MkdocsDatabase _mkdocsDatabase;

    public ViewListViewComponent(IMemoryCache memoryCache, ILogger<ViewListViewComponent> logger,
                                DocProjectHttpClientFactory docProjectHttpClientFactory, IWorkContext workContext, MkdocsDatabase mkdocsDatabase)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _docProjectHttpClientFactory = docProjectHttpClientFactory;
        _workContext = workContext;
        _mkdocsDatabase = mkdocsDatabase;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = HttpContext.Items["MarkdownProcessor_Model"] as MarkdownModel;
        if (model == null)
            throw new InvalidOperationException(
                "This controller is not accessible directly unless the Markdown Model is set");
        var project = model.DocProject;
        var viewList = _mkdocsDatabase.Search<DocProjectView>(x => x.ProjectId == project.Id).OrderByDescending(x => x.CreatedTime);
        //var viewList = await _memoryCache.GetOrCreateAsync($"mem:viewlist:{model.DocProject.Name}:{viewMode}", async options =>
        //{
        //    List<string> result = new List<string>();
        //    try
        //    {
        //        var remoteResult = viewMode == ViewMode.Branch ? await GetBranches(model) : await GetTags(model);
        //        result.AddRange(remoteResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"ViewList({viewMode})��ȡʧ��, {ex.FullMessage()}");
        //    }

        //    options.Size = 1;
        //    //if (result.Any())
        //    //{
        //    //    options.SlidingExpiration = TimeSpan.FromMinutes(20);
        //    //    options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        //    //}
        //    //else
        //    //{

        //    //}
        //    options.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
        //    return result;
        //});
        await Task.CompletedTask;
        return View(viewList.Select(x => x.ViewName).ToList());
    }

    //public async Task<List<string>> GetTags(MarkdownModel model)
    //{
    //    await Task.CompletedTask;
    //    var project = model.DocProject;

    //    Func<IEnumerable<DocProjectTag>, IEnumerable<DocProjectTag>> queryOrderBy =
    //        filter => filter.OrderByDescending(x => x.CreatedTime).ThenByDescending(x => x.Minor).ThenByDescending(x => x.Revision);

    //    var top25 = _mkdocsDatabase.Search<DocProjectTag>(x => x.ProjectId == project.Id, 0, 25, queryOrderBy);
    //    return top25.Select(x => x.Name).ToList();
    //}
    //public async Task<List<string>> GetBranches(MarkdownModel model)
    //{
    //    await Task.CompletedTask;
    //    var project = model.DocProject;
    //    return _mkdocsDatabase.Search<DocProjectBranch>(x => x.ProjectId == project.Id, 0, 50, query =>
    //    {
    //        return query.OrderByDescending(x => x.CreatedTime).OrderBy(x => x.Length);
    //    }).Select(x => x.Name).ToList();
    //}
}
