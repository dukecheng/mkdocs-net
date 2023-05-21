using System.Net;
using System.Web;
using AgileLabs;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Niusys.Docs.Web.MarkdownServices.Utilities
{
    public class MarkdownUtilities
    {
        /// <summary>
        /// Tries to fix up Markdown files for common doc and server
        /// platforms like Github, Gists, BitBucket and a few others
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ParseMarkdownUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var lurl = url.ToLower();

            string urlToOpen = url;


            if (lurl.Contains("gist.github.com"))
            {
                // Orig: https://gist.github.com/RickStrahl/2e485205f9a1a7f9c827c1da172e185b
                // Raw:  https://gist.github.com/RickStrahl/2e485205f9a1a7f9c827c1da172e185b/raw
                if (!lurl.Contains("/raw"))
                    urlToOpen = url + "/raw";
            }
            else if (lurl.Contains("docs.microsoft.com"))
            {
                // orig: https://docs.microsoft.com/en-us/dotnet/csharp/getting-started/
                if (!lurl.Contains(".md"))
                {
                    try
                    {
#pragma warning disable SYSLIB0014
                        var webClient = new WebClient();
#pragma warning restore SYSLIB0014
                        string content = webClient.DownloadString(url);

                        url = StringUtils.ExtractString(content, "data-original_content_git_url=\"", "\"");
                        if (!string.IsNullOrEmpty(url))
                            lurl = url.ToLower();
                    }
                    catch
                    {
                    }
                }
            }
            else if (lurl.Contains("/bitbucket.org/"))
            {
                // orig: https://bitbucket.org/RickStrahl/swfox_webbrowser/src/1fc23444c27cb691b47917663eabdf7ff9dec49e/Readme.md?at=master&fileviewer=file-view-default
                // raw: https://bitbucket.org/RickStrahl/swfox_webbrowser/raw/1fc23444c27cb691b47917663eabdf7ff9dec49e/Readme.md
                if (lurl.Contains("/src/"))
                    urlToOpen = url.Replace("/src/", "/raw/");
            }


            if (lurl.Contains("/github.com/"))
            {
                if (!lurl.Contains(".md"))
                {

                    if (!url.EndsWith("/"))
                        url += "/";
                    url = url + "blob/master/README.md";
                    lurl = url.ToLower();
                }

                // .md files that are not referenced as Raw documents
                if (lurl.Contains(".md") && lurl.Contains("/blob/"))
                {
                    // Norm:  https://github.com/RickStrahl/MarkdownMonster/blob/master/README.md
                    // Conv:  https://github.com/RickStrahl/MarkdownMonster/raw/master/README.md                
                    // Redir: https://raw.githubusercontent.com/RickStrahl/MarkdownMonster/master/README.md

                    // This what GitHub uses for their link on the actual repo
                    urlToOpen = url.Replace("/blob/", "/raw/");

                    // This is what Github redirects to
                    //urlToOpen = url.Replace("/blob/", "/").Replace("/github.com/", "/raw.githubusercontent.com/");
                }

            }

            return urlToOpen;
        }


        /// <summary>
        /// Fixes up relative paths in the generated Markdown based on a base URL
        /// passed in. Typically pass in the URL to the host document to fix up any
        /// relative links in relation to the base Url.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public static string FixupMarkdownRelativePaths(string markdown, string basePath, Dictionary<string, string> urlQuery)
        {
            var extension = Path.GetExtension(basePath);
            if (extension.IsNullOrEmpty() && !basePath.EndsWith('/'))
            {
                basePath += "/";
            }
            var doc = Markdig.Markdown.Parse(markdown);

            var uri = new Uri(basePath, UriKind.Absolute);

            markdown = LinkProcess(markdown, doc, uri, urlQuery);
            return markdown;
        }

        public static string LinkProcess(string markdown, Block block, Uri uri, Dictionary<string, string> urlQuery)
        {
            if (block is ContainerBlock)
            {
                var container = block as ContainerBlock;
                if (container.Count > 0)
                {
                    foreach (var item in container)
                    {
                        markdown = LinkProcess(markdown, item, uri, urlQuery);
                    }
                }
            }
            else if (block is LeafBlock)
            {
                var leaf = block as LeafBlock;
                if (leaf?.Inline != null)
                {
                    foreach (var inline in leaf?.Inline)
                    {
                        if (!(inline is LinkInline))
                            continue;

                        var link = inline as LinkInline;
                        if (link.Url.Contains("://"))
                            continue;

                        var urlBuilder = new UriBuilder(new Uri(uri, link.Url));
                        urlBuilder.Query = string.Join("&", urlQuery.Select(x => $"{HttpUtility.UrlEncode(x.Key)}={HttpUtility.UrlEncode(x.Value)}"));
                        markdown = markdown.Replace("](" + link.Url + ")", "](" + urlBuilder.ToString() + ")");
                    }
                }
            }
            return markdown;
        }

    }
}