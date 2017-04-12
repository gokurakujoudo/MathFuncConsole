using System;
using System.Collections.Generic;
using System.Linq;

namespace MathFuncConsole.MathObjects.Helper {
    /// <summary>
    /// Helper class to implement extend methods for original data types. 
    /// Please feel free to add more useful non-type-specific methods here.
    /// </summary>
    public static class MathClassHelper {

        /// <summary>
        /// Wrapper function for <see cref="double"/>
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Func<double> Wrap(this double num) => () => num;

        /// <summary>
        /// Wrapper function for <see cref="int"/>
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Func<double> Wrap(this int num) => () => num;

        /// <summary>
        /// Shortcut to cast a obj from one type to another. Be careful of if these types can cast directly.
        /// </summary>
        /// <typeparam name="T">To type</typeparam>
        /// <param name="obj"></param>
        /// <returns><see longword="obj"/> as type of T</returns>
        public static T To<T>(this object obj) => (T) obj;

        /// <summary>
        /// Check if this value is close to target.
        /// </summary>
        /// <param name="x">Value</param>
        /// <param name="target">Target value</param>
        /// <param name="eps">Absolute tolerance</param>
        /// <returns></returns>
        public static bool WithinTolerance(this double x, double target, double eps = 1E-6) => Math.Abs(target - x) <=
                                                                                               eps;

        /// <summary>
        /// Return string of a list of double
        /// </summary>
        /// <param name="list">List of double</param>
        /// <param name="join">Separator between numbers</param>
        /// <returns></returns>
        public static string ToStr(this IEnumerable<double> list, string join = ", ") =>
            $"({string.Join(join, list.Select(s => $"{s:F6}"))})";

        /// <summary>
        /// Return string of a list of xs and y
        /// </summary>
        /// <param name="trace">List of xs and y</param>
        /// <returns></returns>
        public static string ToStr(this IEnumerable<(double[] x, double y)> trace) => string.Join(
            "\r\n", trace.Select(t => $"{t.x.ToStr()}, {t.y:F6}"));
    }
}