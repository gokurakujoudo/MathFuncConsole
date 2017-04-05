using System;
using MathFuncConsole.Helper;

namespace MathFuncConsole.MathObjects.Applications {
    /// <summary>
    /// A generic implementation of <see cref="OptionBase"/>. Using generalized Black-Scholes Formula.
    /// </summary>
    public class GenericOption : OptionBase {
        #region vars

        private Func<double> _pv1;
        private Func<double> _pv2;
        private Func<double> _sigma;

        #endregion

        /// <summary>
        /// Initial instance of <see cref="GenericOption"/> 
        /// </summary>
        /// <param name="name">Name of the option</param>
        /// <param name="pv1">Present value of asset to be received</param>
        /// <param name="pv2">Present value of asset to be delivered</param>
        /// <param name="maturity">Time to maturity</param>
        /// <param name="sigma">Volatility of the option (Pricing mode)</param>
        /// <param name="price">Price of the option (Imp_vol mode)</param>
        public GenericOption(string name, object pv1, object pv2, object maturity,
                             object sigma = null, object price = null) : base(name, maturity) {
            this.Pv1 = Input(pv1);
            this.Pv2 = Input(pv2);
            if (sigma != null) {
                this.Sigma = Input(sigma);
                this.Price = PriceFunc(this.Pv1, this.Pv2, this.Sigma, this.Maturity);
            }
            else if (price != null) {
                this.Price = Input(price);
                this.Sigma = () => {
                    var tempOption = new GenericOption(string.Empty, this.Pv1, this.Pv2, this.Maturity, 0);
                    var setter = tempOption.RemoteSetter(nameof(tempOption.Sigma));
                    return Bisection.Search(tempOption.Price, setter, this.Price(), (0, 1));
                };
            }
            else throw new ArgumentNullException(nameof(sigma), "sigma and price can't be both null");
        }

        /// <summary>
        /// Pricing an genetic option by generalized Black-Scholes Formula.
        /// </summary>
        /// <param name="pv1">Present value of asset to be received</param>
        /// <param name="pv2">Present value of asset to be delivered</param>
        /// <param name="sigma">Volatility of the assets combined</param>
        /// <param name="maturity">Time to maturity</param>
        /// <returns></returns>
        public static Func<double> PriceFunc(Func<double> pv1, Func<double> pv2,
                                             Func<double> sigma, Func<double> maturity) => () => {
            var d1 = (Math.Log(pv1() / pv2()) + 0.5 * Math.Pow(sigma(), 2) * maturity()) /
                     (sigma() * Math.Sqrt(maturity()));
            var d2 = d1 - sigma() * Math.Sqrt(maturity());
            return pv1() * NormalDist.NormDist(d1) - pv2() * NormalDist.NormDist(d2);
        };

        #region property
        /// <summary>
        /// Present value of asset to be received
        /// </summary>
        [Name("p1")]
        public Func<double> Pv1 {
            get { return () => _pv1(); }
            set => _pv1 = value;
        }

        /// <summary>
        /// Present value of asset to be delivered
        /// </summary>
        [Name("p2")]
        public Func<double> Pv2 {
            get { return () => _pv2(); }
            set => _pv2 = value;
        }

        /// <summary>
        /// Volatility of the assets combined
        /// </summary>
        [Name("sigma")]
        public Func<double> Sigma {
            get { return () => _sigma(); }
            set => _sigma = value;
        }


        #endregion
    }
}
