using Niusys.Docs.Core.Configuration;

namespace Niusys.Docs.Core.Projects
{
    public class DocProject
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
        public HostType HostType { get; set; }

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
        /// �Ƿ�ʹ�õ����˵�
        /// </summary>
        public bool IsUseNavMenu { get; set; }

        /// <summary>
        /// �Ƿ�ʹ�ð汾��ͼ
        /// </summary>
        public bool IsUseVersionViews { get; set; }

        /// <summary>
        /// �ɼ�ģʽ
        /// </summary>
        public VisiableMode VisiableMode { get; set; }

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
