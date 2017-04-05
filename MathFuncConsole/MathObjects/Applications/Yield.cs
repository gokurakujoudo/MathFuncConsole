using System;

namespace MathFuncConsole.MathObjects.Applications {
    /// <summary>
    /// A wrap up for yield, also provide its multiple casting between types of yield.
    /// </summary>
    public class Yield : MathObject {

        private Func<double> _ytm;

        /// <summary>
        /// Initial instance of <see cref="Yield"/> from yield to maturity
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ytm"></param>
        public Yield(string name, object ytm = null) : base(name) {
            this.Ytm = Input(ytm, .05D);
        }

        /// <summary>
        /// Yield in terms of YTM, not annualized
        /// </summary>
        [Name("ytm")]
        public Func<double> Ytm {
            get { return () => _ytm(); }
            set => _ytm = value;
        }
    }
}
