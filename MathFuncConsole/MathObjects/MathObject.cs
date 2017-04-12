using System;
using System.Linq;
using System.Reflection;
using MathFuncConsole.MathObjects.Applications;
using MathFuncConsole.MathObjects.Helper;

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
        public MathObject(string name) {
            this.Name = name;
        }

        /// <summary>
        /// Universal setter method without reference to the instance. To set value, input must be <see cref="double"/>.
        /// Please use direct setters as much as possible. Remote setter is designed for generic computation methods.
        /// </summary>
        /// <param name="propertyName">Name of the target property</param>
        /// <returns>A setter that you can assign new value to target property</returns>
        public virtual Action<double> RemoteSetter(string propertyName) {
            if (this.GetType().GetProperty(propertyName) == null)
                throw new ArgumentException($"{propertyName} property doesn't exist in {this.GetType().Name}");
            return newValue => this.GetType().GetProperty(propertyName)?.SetValue(this, newValue.Wrap());
        }

        /// <summary>
        /// Universal getter method without reference to the instance. 
        /// Please use direct getter as much as possible. Remote setter is designed for generic computation methods.
        /// </summary>
        /// <param name="propertyName">Name of the target property</param>
        /// <returns>A getter that you can get new value from target property</returns>
        public virtual Func<double> RemoteGetter(string propertyName) {
            if (this.GetType().GetProperty(propertyName) == null)
                throw new ArgumentException($"{propertyName} property doesn't exist in {this.GetType().Name}");
            return this.GetType().GetProperty(propertyName)?.GetValue(this)?.To<Func<double>>();
        }

        /// <summary>
        /// Universal abstraction for inner relationship between two properties in this instance. This method build up a function that 
        /// can help you get a new value of the dependent variable when you change the value of the independent variable. Input value should 
        /// always be <see cref="double"/> and return types is also <see cref="double"/>. Remote link is designed for generic computation methods.
        /// </summary>
        /// <param name="xName">Name of the target property that act as independent variable</param>
        /// <param name="yName">Name of the target property that act as dependent variable</param>
        /// <param name="defaultX">If set, after every calculations, x will be reset to this value.</param>
        /// <returns>A function of mapping independent variable to dependent variable</returns>
        public Func<double, double> RemoteLink(string xName, string yName, double? defaultX = null) => newX => {
            var xProperty = this.GetType().GetProperty(xName);
            if (xProperty == null) throw new TargetInvocationException($"property {xName} cannot be found", null);
            xProperty.SetValue(this, newX.Wrap());
            var yProperty = this.GetType().GetProperty(yName);
            if (yProperty == null) throw new TargetInvocationException($"property {yName} cannot be found", null);
            var newY = yProperty.GetValue(this).To<Func<double>>()();
            if (defaultX.HasValue)
                xProperty.SetValue(this, defaultX.Value.Wrap());
            return newY;
        };


        /// <inheritdoc />
        public sealed override string ToString() {
            var funcs = this.GetType()
                            .GetProperties()
                            .Where(p => p.Name != "Name")
                            .Select(p => {
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


    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class NameAttribute : Attribute {
        public NameAttribute(string name) { this.Name = name; }
        public string Name { get; }
    }
}
