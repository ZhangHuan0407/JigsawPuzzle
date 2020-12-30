using System;
using System.Collections.Generic;
using System.Net.Http;

namespace JigsawPuzzle
{
    [ShareScript]
    public static class HttpContentConverter
    {
        /* field */
        internal static readonly Dictionary<string, Func<object, IEnumerable<HttpContent>>> ObjectToHttpContent = new Dictionary<string, Func<object, IEnumerable<HttpContent>>>()
        {
            { typeof(string).FullName, StringToHttpContent },
            { typeof(string[]).FullName, StringArrayToHttpContent },
            { typeof(byte[]).FullName, ByteArrayToHttpContent },
        };
        internal static readonly Dictionary<string, Func<HttpContent, object>> HttpContentToObject = new Dictionary<string, Func<HttpContent, object>>()
        {
            { typeof(string).FullName, HttpContentToString },
            { typeof(byte[]).FullName, HttpContentToByteArray },
        };

        /* func */
        public static IEnumerable<HttpContent> StringToHttpContent(object obj)
        {
            if (obj is string str)
                yield return new StringContent(str);
            else
                throw new ArgumentException($"Argument type is not equal, {nameof(obj)} :{obj?.GetType().FullName}");
        }
        public static object HttpContentToString(HttpContent httpContent)
        {
            if (httpContent is HttpContent content)
                return content.ReadAsStringAsync().Result;
            else
                throw new ArgumentException($"Argument type is not equal, {nameof(httpContent)} :{httpContent?.GetType().FullName}");
        }

        public static IEnumerable<HttpContent> StringArrayToHttpContent(object obj)
        {
            if (obj is string[] lines)
                foreach (string line in lines)
                    yield return new StringContent(line);
            else
                throw new ArgumentException($"Argument type is not equal, {nameof(obj)} :{obj?.GetType().FullName}");
        }

        public static IEnumerable<HttpContent> ByteArrayToHttpContent(object obj)
        {
            if (obj is byte[] content)
                yield return new ByteArrayContent(content);
            else
                throw new ArgumentException($"Argument type is not equal, {nameof(obj)} :{obj?.GetType().FullName}");
        }
        public static object HttpContentToByteArray(HttpContent httpContent)
        {
            if (httpContent is HttpContent content)
                return content.ReadAsByteArrayAsync().Result;
            else
                throw new ArgumentException($"Argument type is not equal, {nameof(httpContent)} :{httpContent?.GetType().FullName}");
        }
    }
}