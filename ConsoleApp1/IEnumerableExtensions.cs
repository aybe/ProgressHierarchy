using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace ConsoleApp1
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class IEnumerableExtensions
    {
        /// <summary>
        ///     Flattens a hierarchy of elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<T> Flatten<T>([NotNull] this T source, [NotNull] Func<T, IEnumerable<T>> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            yield return source;

            foreach (var node in selector(source).SelectMany(s => Flatten(s, selector)))
            {
                yield return node;
            }
        }
    }
}