using System;
using System.Collections.Generic;

namespace WrapAndCast
{
    public static class IEnumerableExtender
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var i in list)
            {
                T item = i;
                action(item);
            }
        }
    }
}
