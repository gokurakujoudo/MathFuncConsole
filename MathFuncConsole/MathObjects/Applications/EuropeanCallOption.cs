
using System;
using MathFuncConsole.Helper;

namespace MathFuncConsole.MathObjects.Applications {
    class EuropeanCallOption : Option {
        #region var

        private Func<double> _stockPrice;
        private Func<double> _stockDivd;
        private Func<double> _strike;
        private Func<double> _sigma;
        private Func<double> _riskFree;

        #endregion

        public EuropeanCallOption(string name, Stock stock, object strike, object riskFree, object maturity) :
            this(name, stock.Price, strike, riskFree, maturity, stock.Sigma) { }

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

        [Name("S0")]
        public Func<double> StockPrice {
            get { return () => _stockPrice(); }
            set => _stockPrice = value;
        }

        [Name("q")]
        public Func<double> StockDivd {
            get { return () => _stockDivd(); }
            set => _stockDivd = value;
        }

        [Name("K")]
        public Func<double> Strike {
            get { return () => _strike(); }
            set => _strike = value;
        }

        [Name("σ")]
        public Func<double> Sigma {
            get { return () => _sigma(); }
            set => _sigma = value;
        }

        [Name("r")]
        public Func<double> RiskFree {
            get { return () => _riskFree(); }
            set => _riskFree = value;
        }

        #endregion
    }
}
