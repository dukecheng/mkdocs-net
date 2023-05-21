namespace Niusys.Docs.Web.MarkdownServices
{
    /// <summary>
    /// Internally held references that made accessible to the static Markdown functions
    /// </summary>
    internal static class MarkdownComponentState
    {
        internal static IServiceProvider ServiceProvider { get; set; }
        internal static MarkdownConfiguration Configuration { get; set; } = new MarkdownConfiguration();
    }
}
