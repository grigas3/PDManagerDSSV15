using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDManager.Common.Extensions
{
    /// <summary>
    /// Linq Extensions
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Contains All
        /// Checks whether a list contains all values
        /// </summary>
        /// <typeparam name="T">List Generic Template</typeparam>
        /// <param name="source">Source IEnumerable</param>
        /// <param name="values">Values to be contained in the source collection</param>
        /// <returns>True if source contains all values, otherwise false</returns>
        public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> values)
        {
            return values.All(value => source.Contains(value));
        }

    }
}
