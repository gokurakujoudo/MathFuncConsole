using System;

namespace MathFuncConsole.MathObjects.Applications {
    /// <summary>
    /// Implementation of exchange options. Use Margrabe Formula to price.
    /// </summary>
    public class ExchangeOption : OptionBase {
        private Func<double> _rho;

        /// <summary>
        /// Initial instance of <see cref="ExchangeOption"/> from two <see cref="Stock"/>s. Use Margrabe Formula to price.
        /// </summary>
        /// <param name="name">Name of the deferred exchange option</param>
        /// <param name="s1">Asset to be received</param>
        /// <param name="s2">Asset to be delivered</param>
        /// <param name="rho">Correlation between two assets</param>
        /// <param name="maturity">Time to maturity of option</param>
        public ExchangeOption(string name, Stock s1, Stock s2, object rho, object maturity) : base(name, maturity) {
            this.Rho = Input(rho);
            this.Price = GenericOption.PriceFunc(() => Math.Exp(-s1.Divd() * this.Maturity()) * s1.Price(),
                                                 () => Math.Exp(-s2.Divd() * this.Maturity()) * s2.Price(),
                                                 () => Math.Sqrt(
                                                     Math.Pow(s1.Sigma(), 2) + Math.Pow(s2.Sigma(), 2) -
                                                     2 * this.Rho() * s1.Sigma() * s2.Sigma()), this.Maturity);
        }

        /// <summary>
        /// Correlation between two assets
        /// </summary>
        [Name("ρ")]
        public Func<double> Rho {
            get { return () => _rho(); }
            set => _rho = value;
        }
    }
}
