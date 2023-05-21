using Microsoft.AspNetCore.Html;

namespace Niusys.Docs.Web.MarkdownServices
{
    /// <summary>
    /// Markdown Helper class that provides basic parsing features for 
    /// </summary>
    public static class Markdown
    {
        #region From String
        /// <summary>
        /// Renders raw markdown from string to HTML
        /// </summary>
        /// <param name="markdown">The markdown to parse into HTML</param>
        /// <param name="usePragmaLines">print line number ids into the document</param>
        /// <param name="forceReload">forces the markdown parser to be reloaded</param>
        /// <param name="sanitizeHtml">Remove script tags from HTML output</param>
        /// <returns></returns>
        public static string Parse(string markdown, bool usePragmaLines = false, bool forceReload = false, bool sanitizeHtml = false)
        {
            if (string.IsNullOrEmpty(markdown))
                return "";

            var parser = MarkdownComponentState.Configuration.MarkdownParserFactory.GetParser(usePragmaLines, forceReload);
            return parser.Parse(markdown, sanitizeHtml);
        }

        /// <summary>
        /// Renders raw Markdown from string to HTML.
        /// </summary>
        /// <param name="markdown">The markdown to parse into HTML</param>
        /// <param name="usePragmaLines">print line number ids into the document</param>
        /// <param name="forceReload">forces the markdown parser to be reloaded</param>
        /// <param name="sanitizeHtml">Remove script tags from HTML output</param>
        /// <returns></returns>
        public static HtmlString ParseHtmlString(string markdown, bool usePragmaLines = false, bool forceReload = false, bool sanitizeHtml = false)
        {
            return new HtmlString(Parse(markdown, usePragmaLines, forceReload, sanitizeHtml));
        }

        #endregion From String
    }
}
