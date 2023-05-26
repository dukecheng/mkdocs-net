namespace Niusys.Docs.Core.Projects
{
    public enum VisiableMode
    {
        /// <summary>
        /// 完全公开
        /// </summary>
        Public = 1,

        /// <summary>
        /// 私有(仅登录用户访问)
        /// </summary>
        Private = 2,

        /// <summary>
        /// 内部(组织内任意用户登录之后可以访问)
        /// </summary>
        Internal = 3,
    }
}
