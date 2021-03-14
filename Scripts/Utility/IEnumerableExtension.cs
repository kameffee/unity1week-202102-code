using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            return collection.OrderBy(i => Guid.NewGuid());
        }

        public static T Random<T>(this IEnumerable<T> collection)
        {
            return collection.ElementAtOrDefault(UnityEngine.Random.Range(0, collection.Count()));
        }
    }
}