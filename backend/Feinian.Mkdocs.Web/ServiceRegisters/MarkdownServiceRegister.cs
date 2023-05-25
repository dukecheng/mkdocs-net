using AgileLabs;
using AgileLabs.WebApp.Hosting;
using Markdig;
using AgileLabs.AppRegisters;
using Niusys.Docs.Web.MarkdownServices;

namespace Niusys.Docs.Web.ServiceRegisters
{
    public class MarkdownServiceRegister : IServiceRegister, IRequestPiplineRegister
    {
        public int Order => 100;

        public RequestPiplineCollection Configure(RequestPiplineCollection piplineActions, AppBuildContext buildContext)
        {
            piplineActions.Register("UseMarkdown", RequestPiplineStage.BeforeRouting, app =>
            {
                app.UseMarkdown();
            });
            return piplineActions;
        }

        public void ConfigureServices(IServiceCollection services, AppBuildContext buildContext)
        {
            services.AddMarkdown(config =>
            {
                // optional custom MarkdigPipeline (using MarkDig; for extension methods)
                config.ConfigureMarkdigPipeline = builder =>
                {
                    builder.UseAdvancedExtensions()
                    .UseEmojiAndSmiley();
                    //builder.UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default);
                    //.UseDiagrams();
                    //    .UsePipeTables()
                    //    .UseGridTables()
                    //    .UseAutoIdentifiers(AutoIdentifierOptions.AutoLink) // Headers get id="name" 
                    //    .UseAutoLinks() // URLs are parsed into anchors
                    //    .UseAbbreviations()
                    //    .UseYamlFrontMatter()
                    //    .UseEmojiAndSmiley(true)
                    //    .UseListExtras()
                    //    .UseFigures()
                    //    .UseTaskLists()
                    //    .UseCustomContainers()
                    //    .UseGenericAttributes()
                    //    .UseMathematics();
                };
            });
        }
    }
}
