namespace Niusys.Docs.Core.Projects
{
    public class DocProjectTag
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string Name { get; set; }

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public int Revision { get; set; }
        public DateTime CreatedTime { get; set; }
        public DocProjectTag SetVersions(string x)
        {
            var len = x.IndexOf('-');
            if (len == -1)
            {
                len = x.IndexOf('_');
            }
            if (len == -1)
            {
                len = x.Length;
            }
            var result = x.AsSpan().Slice(0, len).ToString();
            if (Version.TryParse(result, out var version))
            {
                Major = version.Major;
                Minor = version.Minor;
                Build = version.Build;
                Revision = version.Revision;
            }
            return this;
        }
    }
}
