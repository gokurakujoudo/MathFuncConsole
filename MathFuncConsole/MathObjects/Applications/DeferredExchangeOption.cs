using System;

namespace MathFuncConsole.MathObjects.Applications
{
    /// <summary>
    /// Implementation of deferred exchange options. Use Margrabe Formula to price.
    /// </summary>
    public class DeferredExchangeOption: ExchangeOption
    {
        private Func<double> _exchangeMaturity;

        /// <summary>
        /// Initial instance of <see cref="DeferredExchangeOption"/> from two <see cref="Stock"/>s. Use Margrabe Formula to price.
        /// </summary>
        /// <param name="name">Name of the deferred exchange option</param>
        /// <param name="s1">Asset to be received</param>
        /// <param name="s2">Asset to be delivered</param>
        /// <param name="rho">Correlation between two assets</param>
        /// <param name="optionMaturity">Time to maturity of option</param>
        /// <param name="exchangeMaturity">Time until exchange >= TOption</param>
        public DeferredExchangeOption(string name, Stock s1, Stock s2, object rho, object optionMaturity,
                                      object exchangeMaturity) : base(name, s1, s2, rho, optionMaturity) {

            this.ExchangeMaturity = Input(exchangeMaturity);
            this.Price = GenericOption.PriceFunc(() => Math.Exp(-s1.Divd() * this.ExchangeMaturity()) * s1.Price(),
                                                 () => Math.Exp(-s2.Divd() * this.ExchangeMaturity()) * s2.Price(),
                                                 () => Math.Sqrt(
                                                     Math.Pow(s1.Sigma(), 2) + Math.Pow(s2.Sigma(), 2) -
                                                     2 * this.Rho() * s1.Sigma() * s2.Sigma()), this.Maturity);
        }


        /// <summary>
        /// Maturity of the option
        /// </summary>
        [Name("T'")]
        public Func<double> ExchangeMaturity {
            get { return () => _exchangeMaturity(); }
            set => _exchangeMaturity = value;
        }
    }
}
