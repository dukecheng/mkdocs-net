namespace Niusys.Docs.Core.Projects
{

    public class DocProjectBranch
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string Name { get; set; }
        public int Length { get; set; }
        public DateTime CreatedTime { get; set; }
        public DocProjectBranch SetProperty(string x)
        {
            Length = x.Length;
            return this;
        }
    }
}
