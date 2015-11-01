using System.IO;
using System.Net;

namespace Utilities
{
    public static class HttpWebResponseExtensions
    {
        public static string GetContent(this HttpWebResponse response)
        {
            string content = null;

            // should check for encoding etc but it's only for example purposes

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                content = reader.ReadToEnd();
            }

            return content;
        }
    }
}