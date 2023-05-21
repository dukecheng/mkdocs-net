using Markdig;
using Niusys.Docs.Web.MarkdownServices.MarkdownParser;

namespace Niusys.Docs.Web.MarkdownServices
{

    /// <summary>
    /// Holds configuration information about the MarkdownPageProcessor
    /// </summary>
    public class MarkdownConfiguration
    {
        public const string DefaultMarkdownViewTemplate = "~/Views/Shared/_MarkdownPageTemplate.cshtml";

        public IMarkdownParserFactory MarkdownParserFactory { get; set; } = new MarkdigMarkdownParserFactory();

        /// <summary>
        /// Optional global configuration for setting up the Markdig Pipeline
        /// </summary>
        public Action<MarkdownPipelineBuilder> ConfigureMarkdigPipeline { get; set; }

        /// <summary>
        /// Global HtmlTagBlackList when StripScriptTags is set for Markdown parsing
        /// </summary>
        public string HtmlTagBlackList { get; set; } = "script|iframe|object|embed|form";
    }
}
