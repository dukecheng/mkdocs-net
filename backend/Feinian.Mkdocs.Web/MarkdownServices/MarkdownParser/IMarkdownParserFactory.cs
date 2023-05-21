namespace Niusys.Docs.Web.MarkdownServices.MarkdownParser
{
    public interface IMarkdownParserFactory
    {
        IMarkdownParser GetParser(bool usePragmaLines = false, bool forceLoad = false);
    }
}
