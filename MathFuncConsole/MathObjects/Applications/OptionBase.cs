using System;

namespace MathFuncConsole.MathObjects.Applications {
    /// <summary>
    /// Base type of any options. You should not make any instances directly from <see cref="OptionBase"/>.
    /// Please refer to <seealso cref="GenericOption"/> for implementing an option
    /// </summary>
    public abstract class OptionBase : MathObject {
        private Func<double> _maturity;
        private Func<double> _price;

        /// <summary>
        /// Initial instance for <see cref="OptionBase"/>. You should not call this directly. 
        /// </summary>
        /// <param name="name">Name of the option</param>
        /// <param name="maturity">Maturity of the option</param>
        public OptionBase(string name, object maturity) : base(name) {
            this.Maturity = Input(maturity);
            this.Price = 0.Wrap();
        }

        /// <summary>
        /// Maturity of the option
        /// </summary>
        [Name("T")]
        public Func<double> Maturity {
            get { return () => _maturity(); }
            set => _maturity = value;
        }

        /// <summary>
        /// Price of the option. No default pricing in <see cref="OptionBase"/>.
        /// </summary>
        [Name("P0")]
        public Func<double> Price {
            get { return () => _price(); }
            set => _price = value;
        }
    }
}