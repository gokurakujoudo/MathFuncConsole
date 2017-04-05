using System;

namespace MathFuncConsole.MathObjects.Applications {
    /// <summary>
    /// A stock with/without constant dividend yield.
    /// </summary>
    public class Stock : MathObject {
        #region vars

        private Func<double> _price;
        private Func<double> _divd;
        private Func<double> _miu;
        private Func<double> _sigma;

        #endregion


        /// <summary>
        /// Initial instance of <see cref="Stock"/>.
        /// </summary>
        /// <param name="name">Name of the stock</param>
        /// <param name="price">Price of the stock at time 0</param>
        /// <param name="sigma">Volatility of the stock return</param>
        /// <param name="miu">Drift of the stock return in real world</param>
        /// <param name="divd">Dividend yield of the stock</param>
        public Stock(string name, object price, object sigma, object miu = null, object divd = null) : base(name) {
            this.Price = Input(price);
            this.Sigma = Input(sigma);
            this.Miu = Input(miu, 0D);
            this.Divd = Input(divd, 0D);
        }


        #region property

        /// <summary>
        /// Price of the stock at time 0
        /// </summary>
        [Name("S0")]
        public Func<double> Price {
            get { return () => _price(); }
            set => _price = value;
        }

        /// <summary>
        /// Dividend yield of the stock
        /// </summary>
        [Name("q")]
        public Func<double> Divd {
            get { return () => _divd(); }
            set => _divd = value;
        }

        /// <summary>
        /// Drift of the stock return in real world
        /// </summary>
        [Name("μ")]
        public Func<double> Miu {
            get { return () => _miu(); }
            set => _miu = value;
        }

        /// <summary>
        /// Volatility of the stock return
        /// </summary>
        [Name("σ")]
        public Func<double> Sigma {
            get { return () => _sigma(); }
            set => _sigma = value;
        }

        #endregion
    }
}
