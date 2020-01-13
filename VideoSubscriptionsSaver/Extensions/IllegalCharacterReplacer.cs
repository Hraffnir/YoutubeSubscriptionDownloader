using System.IO;
using System.Text.RegularExpressions;

namespace VideoSubscriptionsSaver.Extensions
{
    public static class IllegalCharacterReplacer
    {
        public static string Replace(string illegal)
        {
            var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(illegal, "");
        }
    }
}
