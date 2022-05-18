using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ProxLocator.Engine.Utils
{
    public static class PrimitiveUtils
    {
        public static void ThrowIfNull(this string text)
        {
            if (text == null)
                throw new ArgumentNullException();
        }

        public static string ToEndpointString(this IPEndPoint endPoint)
        {
            char[] separators = { '|', ':' };
            var result = endPoint
                .Address
                .ToString()
                .Split(separators)[0];

            return result;
        }

        // Ex: collection.TakeLast(5);
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        private static Random rng = new Random();

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
