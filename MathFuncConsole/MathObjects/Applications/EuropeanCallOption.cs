
using System;
using MathFuncConsole.MathObjects.Helper;

namespace MathFuncConsole.MathObjects.Applications {
    /// <summary>
    /// Abstraction of European call options
    /// </summary>
    public class EuropeanCallOption : Option {
        #region var

        private Func<double> _stockPrice;
        private Func<double> _stockDivd;
        private Func<double> _strike;
        private Func<double> _sigma;
        private Func<double> _riskFree;

        #endregion

        /// <summary>
        /// Initial instance of <see cref="EuropeanCallOption"/>
        /// </summary>
        /// <param name="name">Name of the option</param>
        /// <param name="stock">Underlying stock</param>
        /// <param name="strike">Strike price</param>
        /// <param name="riskFree">Risk-free rate</param>
        /// <param name="maturity">Maturity of the option</param>
        public EuropeanCallOption(string name, Stock stock, object strike, object riskFree, object maturity) :
            this(name, stock.Price, strike, riskFree, maturity, stock.Sigma) { }

        /// <summary>
        /// Initial instance of <see cref="EuropeanCallOption"/>
        /// </summary>
        /// <param name="name">Name of the option</param>
        /// <param name="stockP">Current stock price</param>
        /// <param name="strike">Strike price</param>
        /// <param name="riskFree">Risk-free rate</param>
        /// <param name="maturity">Maturity of the option</param>
        /// <param name="sigma">Sigma of the stock, pass <see langword="null"/> for implied volatility</param>
        /// <param name="price">Price of the option, pass <see langword="null"/> for pricing from B/S</param>
        public EuropeanCallOption(string name, object stockP, object strike, object riskFree, object maturity,
                                  object sigma = null, object price = null) : base(name, maturity) {
            this.StockPrice = Input(stockP);
            this.Strike = Input(strike);
            this.RiskFree = Input(riskFree);
            if (sigma != null) {
                this.Sigma = Input(sigma);
                this.Price = GenericOption.PriceFunc(
                    () => Math.Exp(-this.StockDivd() * this.Maturity()) * this.StockPrice(),
                    () => Math.Exp(-this.RiskFree() * this.Maturity()) * this.Strike(),
                    this.Sigma, this.Maturity);
            }
            else if (price != null) {
                this.Price = Input(price);
                this.Sigma = () => {
                    var t = new EuropeanCallOption(string.Empty, this.StockPrice, this.Strike,
                                                   this.RiskFree, this.Maturity, sigma: 0);
                    return Bisection.Search(t.RemoteLink(nameof(t.Sigma), nameof(t.Price)), this.Price(), (0, 1));
                };
            }
            else throw new ArgumentNullException(nameof(sigma), "sigma and price can't be both null");
        }

        #region property

        /// <summary>
        /// Current stock price
        /// </summary>
        [Name("S0")]
        public Func<double> StockPrice {
            get { return () => _stockPrice(); }
            set => _stockPrice = value;
        }

        /// <summary>
        /// Dividend yield of the underlying stock
        /// </summary>
        [Name("q")]
        public Func<double> StockDivd {
            get { return () => _stockDivd(); }
            set => _stockDivd = value;
        }

        /// <summary>
        /// Strike price of this option
        /// </summary>
        [Name("K")]
        public Func<double> Strike {
            get { return () => _strike(); }
            set => _strike = value;
        }

        /// <summary>
        /// Volatility of the stock
        /// </summary>
        [Name("σ")]
        public Func<double> Sigma {
            get { return () => _sigma(); }
            set => _sigma = value;
        }

        /// <summary>
        /// Risk free rate
        /// </summary>
        [Name("r")]
        public Func<double> RiskFree {
            get { return () => _riskFree(); }
            set => _riskFree = value;
        }

        #endregion
    }
}
