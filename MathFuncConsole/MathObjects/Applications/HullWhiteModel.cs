using System;
using System.Collections.Generic;
using System.Linq;
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

        public double HW_ZBPrice_CF(double t0, double t1, double rT0) {
            // 3.39
            var(aT0T1, bT0T1) = this.HW_AB(t0, t1);
            return aT0T1 * Math.Exp(-bT0T1 * rT0);
        }

        public double HW_ZBPut_CF(double t0, double t1, double t2, double x, double rT0) {
            // 3.41
            var(_, bts) = this.HW_AB(t0, t2);
            var pT0T1 = this.HW_ZBPrice_CF(t0, t1, rT0);
            var pT0T2 = this.HW_ZBPrice_CF(t0, t2, rT0);
            var a = this.A();
            var sigmaP = this.Sigma() * Math.Sqrt((1 - Math.Exp(-2 * a * (t1 - t0))) / (2 * a)) * bts;
            var h = Math.Log(pT0T2 / (pT0T1 * x)) / sigmaP + sigmaP / 2;
            var price = -pT0T2 * NormalDist.NormDist(-h) + x * pT0T1 * NormalDist.NormDist(-h + sigmaP);
            return price;
        }

        public double HW_Caplets(double t0, double t1, double t2, double x, double n, double rT0) {
            // 2.27
            var nPrime = n * (1 + x * (t2 - t1));
            var xPrime = 1 / (1 + x * (t2 - t1));
            var cpl = nPrime * this.HW_ZBPut_CF(t0, t1, t2, xPrime, rT0);
            return cpl;
        }

        public double HW_Caps(double t0, double tAlpha, double[] taoArray, double x, double n, double rT0) {
            // 3.42
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

        public double[] HW_R_CM(double s, double rs, double t0, int n) {
            // 3.37
            if (t0 == s) return new[] {s};
            Func<double, double> alphaFunc = (t) => this.MarketF(t) +
                                                    Math.Pow(this.Sigma(), 2) / (2 * Math.Pow(this.A(), 2)) *
                                                    Math.Pow(1 - Math.Exp(-this.A() * t), 2);
            var mu = rs * Math.Exp(-this.A() * (t0 - s)) + alphaFunc(t0) -
                     alphaFunc(s) * Math.Exp(-this.A() * (t0 - s));
            var sigma = Math.Pow(this.Sigma(), 2) / (2 * Math.Pow(this.A(), 2)) *
                        Math.Pow(1 - Math.Exp(-2 * this.A() * (t0 - s)), 2);
            var sampleFunc = MonteCarloSimulation.GenerateSamples(
                n, 0.Wrap(), (t, x) => x, mu.Wrap(), sigma.Wrap());
            var samples = sampleFunc();
            return samples;
        }

        public double HW_CAP_CM(double t0, double tAlpha, double[] taoArray, double x, double n, int num) {
            // 3.37
            Func<double, double> alphaFunc = (t) => this.MarketF(t) +
                                                    Math.Pow(this.Sigma(), 2) / (2 * Math.Pow(this.A(), 2)) *
                                                    Math.Pow(1 - Math.Exp(-this.A() * t), 2);
            var mu = this.MarketF(0) * Math.Exp(-this.A() * t0) + alphaFunc(t0) -
                     alphaFunc(0) * Math.Exp(-this.A() * t0);
            var sigma = Math.Pow(this.Sigma(), 2) / (2 * Math.Pow(this.A(), 2)) *
                        Math.Pow(1 - Math.Exp(-2 * this.A() * t0), 2);
            var sampleFunc = MonteCarloSimulation.GenerateSamples(
                num, 0.Wrap(), (t, rt) => this.HW_Caps(t0, tAlpha, taoArray, x, n, rt), mu.Wrap(), sigma.Wrap());
            var samples = sampleFunc();
            return samples.Average();
        }

        private double MarketF(double t) =>
            Interpolation.Linear(t, _marketT, _marketF);


        private double MarketP(double t) =>
            Interpolation.Linear(t, _marketT, _marketP);


        public static SimulatedAnnealing<HullWhiteModel> HW_SaaSolver(
            double[] ts, double[] ps, double[] fs, double t0, double tAlpha,
            IReadOnlyCollection<(double, double[])> strikeAndTaos, double[] capsTargets, double notional,
            IReadOnlyCollection<(double, double)> ranges, int samplesCount = 100, int threads = 6,
            int temperature = 400, int iters = 20000, int cooliter = 40, bool debug = false)
        {
            var m = strikeAndTaos.Count;
            var paras = new object[] { string.Empty, 1, .1, ts, ps, fs };
            Func<HullWhiteModel, double> obj = (hwx) => {
                var capFuncs = strikeAndTaos
                    .Select<(double, double[]), Func<double[], double>>(
                        sat => (rts) => rts.Average(r => hwx.HW_Caps(t0, tAlpha, sat.Item2, sat.Item1, notional, r)))
                    .ToArray();
                var totalDiff = 0D;
                var rs = hwx.HW_R_CM(0, fs[0], t0, samplesCount);
                for (var i = 0; i < m; i++)
                {
                    var cp = capFuncs[i](rs);
                    var diff = (cp - capsTargets[i]) / (capsTargets[i]);
                    totalDiff += Math.Pow(diff, 2);
                }
                return totalDiff;
            };
            var saaSolver =
                new SimulatedAnnealing<HullWhiteModel>(
                    paras, new[] { "A", "Sigma" }, ranges, obj, threads, temperature, iters, cooliter, debug);
            return saaSolver;
        }


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
