using System;
using MathFuncConsole.MathObjects.Helper;

namespace MathFuncConsole.MathObjects.Applications {
    class HullWhiteModel : MathObject {
        private Func<double> _alpha;
        private Func<double> _sigma;

        private readonly double[] _marketP, _marketF, _marketT;

        public HullWhiteModel(string name, object a, object sigma, double[] marketT, double[] marketP,
                              double[] marketF) : base(name) {
            _marketP = marketP;
            _marketF = marketF;
            _marketT = marketT;
            this.A = Input(a);
            this.Sigma = Input(sigma);
        }







        public (double at0t1, double bt0t1) HW_AB(double t0, double t1) {
            // 3.39
            var pmT0 = this.MarketP(t0);
            var pmT1 = this.MarketP(t1);
            var fmT0 = this.MarketF(t0);
            var bT0T1 = (1 - Math.Exp(-this.A() * (t1 - t0))) / this.A();
            var aT0T1 = pmT1 / pmT0 * Math.Exp(bT0T1 * fmT0 -
                                               Math.Pow(this.Sigma(), 2) / (4 * this.A()) *
                                               (1 - Math.Exp(-2 * this.A() * t0)) * Math.Pow(bT0T1, 2));
            return (aT0T1, bT0T1);
        }

        public double HW_P(double t0, double t1, double rT0) {
            var(aT0T1, bT0T1) = this.HW_AB(t0, t1);
            return aT0T1 * Math.Exp(-bT0T1 * rT0);
        }

        public double HW_ZBP(double t0, double t1, double t2, double x, double rT0) {
            var(_, bts) = this.HW_AB(t0, t2);
            var pT0T1 = this.HW_P(t0, t1, rT0);
            var pT0T2 = this.HW_P(t0, t2, rT0);
            var a = this.A();
            var sigmaP = this.Sigma() * Math.Sqrt((1 - Math.Exp(-2 * a * (t1 - t0))) / (2 * a)) * bts;
            var h = Math.Log(pT0T2 / (pT0T1 * x)) / sigmaP + sigmaP / 2;
            var price = -pT0T2 * NormalDist.NormDist(-h) + x * pT0T1 * NormalDist.NormDist(-h + sigmaP);
            return price;
        }

        public double HW_Caplets(double t0, double t1, double t2, double x, double n, double rT0) {
            var nPrime = n * (1 + x * (t2 - t1));
            var xPrime = 1 / (1 + x * (t2 - t1));
            var cpl = nPrime * this.HW_ZBP(t0, t1, t2, xPrime, rT0);
            return cpl;
        }

        public double HW_Caps(double t0, double tAlpha, double[] taoArray, double x, double n, double rT0) {
            var term = taoArray.Length;
            var caps = 0D;
            var t1 = tAlpha;
            var t2 = tAlpha + taoArray[0];
            for (var i = 0; i < term; i++) {
                if (i > 0)
                    t1 = t2;
                t2 += taoArray[i];
                caps = caps + this.HW_Caplets(t0, t1, t2, x, n, rT0);
            }
            return caps;
        }

        private double MarketF(double t) =>
            Interpolation.Linear(t, _marketT, _marketF);


        private double MarketP(double t) =>
            Interpolation.Linear(t, _marketT, _marketP);


        /// <summary>
        /// Alpha of HW model
        /// </summary>
        [Name("a")]
        public Func<double> A {
            get { return () => _alpha(); }
            set => _alpha = value;
        }

        /// <summary>
        /// Sigma of HW model
        /// </summary>
        [Name("σ")]
        public Func<double> Sigma {
            get { return () => _sigma(); }
            set => _sigma = value;
        }
    }
}
