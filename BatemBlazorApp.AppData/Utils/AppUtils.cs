using System.Reflection;

namespace BatemBlazorApp.AppData
{
    public static class AppUtils
    {
        public static string GetFileContent(Type type, string path)
        {
            string content = string.Empty;
            using (var stream = GetResourceStream(type, path))
            {
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream))
                        content = sr.ReadToEnd();
                }
            }
            return content;
        }
        static Stream GetResourceStream(Type type, string path)
        {
            return Assembly.GetAssembly(type).GetManifestResourceStream(path);
        }
    }
}
