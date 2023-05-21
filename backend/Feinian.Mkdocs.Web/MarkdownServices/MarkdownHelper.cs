using Niusys.Docs.Web.Models;
using Westwind.Utilities;

namespace Niusys.Docs.Web.MarkdownServices
{
    public class MarkdownHelper
    {
        public static MarkdownModel ParseMarkdownToModel(string markdown, string rawMarkdown, MarkdownModel model = null)
        {
            if (model == null)
                model = new MarkdownModel();

            string yaml = null;
            string firstLinesText = null;
            var firstLines = markdown.GetLines(50).ToList();

            if (markdown.StartsWith("---"))
            {
                firstLinesText = string.Join("\n", firstLines);
                yaml = firstLinesText.ExtractString("---", "---", returnDelimiters: true);
            }

            if (yaml != null && yaml.Contains("basePath: "))
                model.BasePath = yaml.ExtractString("basePath: ", "\n")?.Trim();
            //if (string.IsNullOrEmpty(model.BasePath))
            //    model.BasePath = model.FolderConfiguration.BasePath;

            //if (model.FolderConfiguration.ExtractTitle)
            //{

            //}

            if (yaml != null)
            {
                model.Title = yaml.ExtractString("title: ", "\n");
                model.YamlHeader = yaml.Replace("---", "").Trim();
            }

            // if we don't have Yaml headers the header has to be closer to the top
            firstLines = firstLines.Take(10).ToList();

            if (string.IsNullOrEmpty(model.Title))
            {
                foreach (var line in firstLines)
                {
                    if (line.TrimStart().StartsWith("# "))
                    {
                        model.Title = line.TrimStart(' ', '\t', '#');
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(model.Title))
            {
                for (var index = 0; index < firstLines.Count; index++)
                {
                    var line = firstLines[index];
                    if (line.TrimStart().StartsWith("===") && index > 0)
                    {
                        // grab the previous line
                        model.Title = firstLines[index - 1].Trim();
                        break;
                    }
                }
            }

            //var result = Markdown.ParseFromUrlAsync("https://github.com/RickStrahl/Westwind.AspNetCore.Markdown/raw/master/readme.md").Result;
            model.RawMarkdown = rawMarkdown;
            model.RenderedMarkdown = Markdown.ParseHtmlString(rawMarkdown, sanitizeHtml: false);
            return model;
        }
    }
}
