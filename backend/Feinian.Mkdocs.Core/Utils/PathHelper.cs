namespace Niusys.Docs.Core.Utils
{
    public static class PathHelper
    {

        /// <summary>
        /// Normalizes a file path to the operating system default
        /// slashes.
        /// </summary>
        /// <param name="path"></param>
        public static string NormalizeUrlPath(string path)
        {
            //return Path.GetFullPath(path); // this always turns into a full OS path

            if (string.IsNullOrEmpty(path))
                return path;

            var slash = Path.DirectorySeparatorChar;
            path = path.Replace('/', slash).Replace('\\', slash);
            return path.Replace(slash.ToString() + slash.ToString(), slash.ToString());
        }

        public static string NormalizePath(string path)
        {
            //return Path.GetFullPath(path); // this always turns into a full OS path

            if (string.IsNullOrEmpty(path))
                return path;

            var slash = '/';
            path = path.Replace('\\', slash).Replace("//", slash.ToString());
            return path.Replace(slash.ToString() + slash.ToString(), slash.ToString());
        }
    }
}
