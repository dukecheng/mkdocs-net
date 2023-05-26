using Niusys.Docs.Core.Configuration;

namespace Niusys.Docs.Core.Projects
{
    public class DocProject : IConfig
    {
        public long Id { get; set; }

        /// <summary>
        /// չʾ����
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// �����ַ
        /// </summary>
        public string RequestPath { get; set; }

        /// <summary>
        /// Git��ַ
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Host����
        /// </summary>
        public HostType HostType { get; set; } = HostType.Gogs;

        /// <summary>
        /// Git��Ŀ��ַ
        /// </summary>
        public string ProjectPath { get; set; }

        /// <summary>
        /// Docs·��
        /// </summary>
        public string WikiFolder { get; set; }

        /// <summary>
        /// Ĭ����ҳ
        /// </summary>
        public string IndexFile { get; set; }

        /// <summary>
        /// Ĭ�Ϸ�֧����tag
        /// </summary>
        public string DefaultView { get; set; }

        /// <summary>
        /// ����Git�ֿ�ʱ��ͷ��Ϣ
        /// </summary>
        public List<KeyValuePair<string, string>> RequestHeaders { get; set; }

        public DocProject()
        {
            RequestHeaders = new List<KeyValuePair<string, string>>();
        }
    }
}
