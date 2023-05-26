using Niusys.Docs.Core.Configuration;

namespace Niusys.Docs.Core.Projects
{
    public class DocProject
    {
        public long Id { get; set; }

        /// <summary>
        /// 展示名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string RequestPath { get; set; }

        /// <summary>
        /// Git地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Host类型
        /// </summary>
        public HostType HostType { get; set; }

        /// <summary>
        /// Git项目地址
        /// </summary>
        public string ProjectPath { get; set; }

        /// <summary>
        /// Docs路径
        /// </summary>
        public string WikiFolder { get; set; }

        /// <summary>
        /// 默认首页
        /// </summary>
        public string IndexFile { get; set; }

        /// <summary>
        /// 默认分支或者tag
        /// </summary>
        public string DefaultView { get; set; }

        /// <summary>
        /// 是否使用导航菜单
        /// </summary>
        public bool IsUseNavMenu { get; set; }

        /// <summary>
        /// 是否使用版本视图
        /// </summary>
        public bool IsUseVersionViews { get; set; }

        /// <summary>
        /// 可见模式
        /// </summary>
        public VisiableMode VisiableMode { get; set; }

        /// <summary>
        /// 请求Git仓库时的头信息
        /// </summary>
        public List<KeyValuePair<string, string>> RequestHeaders { get; set; }

        public DocProject()
        {
            RequestHeaders = new List<KeyValuePair<string, string>>();
        }
    }
}
