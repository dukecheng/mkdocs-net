using AgileLabs;
using AgileLabs.Sessions;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Niusys.Docs.Core.Models;
using Niusys.Docs.Core.ProjectHttpClients;
using Niusys.Docs.Web.Controllers;
using Niusys.Docs.Web.MarkdownServices.Utilities;
using Niusys.Docs.Web.Models;
using System.Text;

namespace Niusys.Docs.Web.Components;

[ViewComponent(Name = "NavMenu")]
public class NavMenuViewComponent : ViewComponent
{
    private readonly IRequestSession _requestSession;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<NavMenuViewComponent> _logger;
    private readonly DocProjectHttpClientFactory _docProjectHttpClientFactory;
    private readonly IWorkContextCore _workContext;

    public NavMenuViewComponent(IRequestSession requestSession,
                                IMemoryCache memoryCache,
                                ILogger<NavMenuViewComponent> logger,
                                DocProjectHttpClientFactory docProjectHttpClientFactory,
                                IWorkContextCore workContext)
    {
        _requestSession = requestSession;
        _memoryCache = memoryCache;
        _logger = logger;
        _docProjectHttpClientFactory = docProjectHttpClientFactory;
        _workContext = workContext;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // Model saved in middleware processing
        var model = HttpContext.Items["MarkdownProcessor_Model"] as MarkdownModel;
        if (model == null)
            throw new InvalidOperationException(
                "This controller is not accessible directly unless the Markdown Model is set");

        var relativePath = "navmenu.md";

        var inhirtQuery = new Dictionary<string, string>();
        if (Request.Query.ContainsKey("view"))
        {
            inhirtQuery.Add("view", Request.Query["view"]);
        }

        var rawMarkdown = await _memoryCache.GetOrCreateAsync($"mem:navmenu:{model.DocProject.Name}:{model.ViewName}", async options =>
        {
            string content = null;
            try
            {
                options.ApplyDefaultCachePolicy();
                var httpClient = _docProjectHttpClientFactory.CreateHttpClient(model.DocProject.HostType, _workContext);
                content = await httpClient.GetMarkdownFile(model.DocProject, model.ViewName, relativePath);

                var baseUrl = $"{_requestSession.HostWithScheme}/{model.DocProject.RequestPath}";
                content = MarkdownUtilities.FixupMarkdownRelativePaths(content, baseUrl, inhirtQuery);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DocProject [#{model.DocProject.Name}] navmenu��ȡʧ��, {ex.FullMessage()}");
            }

            if (content.IsNotNullOrWhitespace())
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            }
            else
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
            }
            options.SetSize(1);
            return content;
        });

        if (rawMarkdown.IsNullOrEmpty())
        {
            return View("Default", string.Empty);
        }

        //var markdown = Markdown.Parse(rawMarkdown, false, false, false);
        var doc = Markdig.Markdown.Parse(rawMarkdown);

        var rootList = new UlList();
        ListProcess(doc, rootList);
        var navmenuContent = new StringBuilder();

        var navmenuList = rootList.Items.Count == 1 ? rootList.Items.First().UlList : rootList;
        BuildNavMenuContent(navmenuList, navmenuContent);
        return View("Default", navmenuContent.ToString());
    }

    private static void BuildNavMenuContent(UlList rootList, StringBuilder navmenuContent)
    {
        if (!rootList.Items.Any())
        {
            return;
        }
        foreach (var item in rootList.Items)
        {

            if (item.UlList == null || !item.UlList.Items.Any())
            {
                navmenuContent.AppendLine($"<li data-value='{item.Text}'>");
                navmenuContent.AppendLine($"<a href=\"{item.Url}\">{item.Text}</a>");
                navmenuContent.AppendLine("</li>");
            }
            else
            {
                navmenuContent.AppendLine($"<li data-value='{item.Text}'>");
                navmenuContent.AppendLine($"<a>{item.Text}</a>");
                navmenuContent.AppendLine($"<ul>");
                BuildNavMenuContent(item.UlList, navmenuContent);
                navmenuContent.AppendLine($"</ul>");
                navmenuContent.AppendLine("</li>");
            }

        }
    }

    public void ListProcess(IMarkdownObject block, UlList root = null, int containerLevel = 0, int headingLevel = 0)
    {
        if (block is ContainerBlock)
        {
            //WrteLog($"{GetLevelSpace(containerLevel)}This is a container {block.GetType().Name}");
            var container = block as ContainerBlock;
            if (container.Count > 0)
            {
                foreach (var item in container)
                {
                    ListProcess(item, root, containerLevel + 1);
                }
            }
        }
        else if (block is LeafBlock)
        {
            var blockType = block.GetType().Name;
            switch (blockType)
            {
                case "HeadingBlock":
                    var heading = block as HeadingBlock;

                    //WrteLog($"{GetLevelSpace(containerLevel)}{GetLevelSpace(heading.Level)}This is a LeafBlock {block.GetType().Name} - {heading.Level}");
                    var headingText = GetHeadingText(heading);
                    root.AddLevelItem(new LiItem() { Text = headingText, Level = heading.Level, UlList = new UlList() }, heading.Level);
                    var headingLink = GetLinks(heading);
                    if (headingLink != null)
                    {
                        bool hasExtension = !string.IsNullOrEmpty(Path.GetExtension(headingLink.Url));
                        if (hasExtension)
                        {
                            root.AddItem(new LiItem() { Text = $"Default", Url = headingLink.Url });
                        }
                    }

                    break;
                case "ParagraphBlock":

                    var paragraph = block as ParagraphBlock;
                    foreach (var item in paragraph.Inline)
                    {
                        var pLink = GetInlineLinks(item);
                        if (pLink == null)
                        {
                            continue;
                        }
                        root.AddItem(new LiItem() { Text = pLink.FirstChild.ToString(), Url = pLink.Url });
                    }

                    //if (paragraph.Inline is ContainerInline pcontainerInline)
                    //{
                    //    if (pcontainerInline.Count() > 0)
                    //    {
                    //        foreach (var itemInline in pcontainerInline)
                    //        {
                    //            ListProcess(itemInline, root, containerLevel + 1);
                    //        }
                    //    }
                    //}


                    break;
                default:
                    //WrteLog($"{GetLevelSpace(containerLevel)}This is a LeafBlock {block.GetType().Name}");
                    break;
            }
        }
        LinkInline GetInlineLinks(Inline leafBlock)
        {
            if (!(leafBlock is LinkInline))
                return null;

            var link = leafBlock as LinkInline;
            return link;
        }
        LinkInline GetLinks(LeafBlock leafBlock)
        {
            foreach (var inline in leafBlock?.Inline)
            {
                if (!(inline is LinkInline))
                    continue;

                var link = inline as LinkInline;
                return link;
            }
            return null;
        }
        string GetHeadingText(HeadingBlock heading)
        {
            foreach (var inlineItem in heading?.Inline)
            {
                switch (inlineItem.GetType().Name)
                {
                    case "LiteralInline":
                        var literal = inlineItem as LiteralInline;
                        return literal.Content.ToString();
                    case "LinkInline":
                        var link = inlineItem as LinkInline;
                        return link.FirstChild.ToString();
                    default:
                        return inlineItem.ToString();
                }
            }
            return heading.ToString();
        }
    }


}
