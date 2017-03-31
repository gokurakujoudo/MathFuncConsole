using System;
using System.Linq;
using MathFuncConsole.MathObjects.Applications;

namespace MathFuncConsole.MathObjects {

    /// <summary>
    /// The base class for all math objects. It provides multiple suppository method to deal with input/cast/print stuff.
    /// You shouldn't have any instance of <see cref="MathObject"/> in your code in most cases. 
    /// You are recommended to inherit from it and develop your own sub-class. Check <seealso cref="Bond"/> as a template for both
    /// writing your class and XML document to help others.
    /// </summary>
    public abstract class MathObject {
        /// <summary>
        /// Name of this <see cref="MathObject"/>, only used in print-out.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initial new instance of <see cref="MathObject"/>. You shouldn't call this method directly but to inherit it.
        /// </summary>
        /// <param name="name">Name of the <see cref="MathObject"/> for print-out</param>
        public MathObject(string name) { this.Name = name; }

        /// <inheritdoc />
        public sealed override string ToString() {
            var funcs = this.GetType().GetProperties().Where(p => p.Name != "Name").Select(p => {
                var att = p.GetCustomAttributes(typeof(NameAttribute), false).FirstOrDefault();
                var name = ((NameAttribute) att)?.Name ?? p.Name;
                return (name, p.GetValue(this).To<Func<double>>()());
            });
            return $"{this.Name}: {string.Join("|", funcs.Select(p => $"{p.Item1} = {p.Item2:F2}"))}";
        }

        /// <summary>
        /// Wrap a <see cref="double"/> to a <see cref="Func{TResult}"/>. 
        /// You should use <see cref="Input"/> to cast input instead of use this function explicitly.
        /// </summary>
        /// <param name="num"></param>
        /// <returns>a <see cref="Func{TResult}"/>, whose generic type argument is <see cref="double"/></returns>
        private static Func<double> Wrap(double num) => num.Wrap();

        /// <summary>
        /// To cast an input to what is should be (a <see cref="Func{TResult}"/>, whose generic type argument is <see cref="double"/>).
        /// You should use this function in your .ctor for every parameters input to enlarge flexibility. Only types that can cast to 
        /// a <see cref="double"/> or a <see cref="Func{TResult}"/> that returns a <see cref="double"/> or null are accepted.
        /// </summary>
        /// <exception cref="ArgumentException">If you pass in a input with wrong type</exception>
        /// <exception cref="ArgumentNullException">If you pass in a null and provide no default result</exception>
        /// <param name="obj">Input parameter</param>
        /// <param name="def">Default result when input is null, can be null itself, but then you have to be careful with possible input.</param>
        /// <returns>a <see cref="Func{TResult}"/>, whose generic type argument is <see cref="double"/></returns>
        protected static Func<double> Input(object obj, double? def = null) {
            if (obj == null)
                if (def.HasValue)
                    return Wrap(def.Value);
                else throw new ArgumentNullException(nameof(obj));
            if (obj is Func<double> f) return f;
            if (obj is int i) return Wrap(i);
            if (obj is double d) return Wrap(d);
            throw new ArgumentException("not a double or Func<out double>", nameof(obj));
        }
    }

    /// <summary>
    /// Helper class to implement extend methods for original data types. 
    /// Please feel free to add more useful non-type-specific methods here.
    /// </summary>
    static class MathClassHelper {

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
        /// <returns></returns>
        public static T To<T>(this object obj) => (T) obj;


    }


    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class NameAttribute : Attribute {
        public NameAttribute(string name) {
            this.Name = name;
        }
        public string Name { get; }
    }
}
