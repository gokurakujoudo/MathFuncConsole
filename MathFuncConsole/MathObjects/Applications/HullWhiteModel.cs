using System;
using System.Collections.Generic;
using System.Linq;
using MathFuncConsole.MathObjects.Helper;
using static System.Math;

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

        private (double at0t1, double bt0t1) AB(double t0, double t1) {
            // 3.39
            var pmT0 = MarketP(t0);
            var pmT1 = MarketP(t1);
            var fmT0 = MarketF(t0);
            var a = this.A();
            var bT0T1 = (1 - Exp(-a * (t1 - t0))) / a;
            var aT0T1 = pmT1 / pmT0 * Exp(bT0T1 * fmT0 -
                                               Pow(this.Sigma(), 2) / (4 * a) * (1 - Exp(-2 * a * t0)) *
                                               Pow(bT0T1, 2));
            return (aT0T1, bT0T1);
        }

        public double HW_ZBPrice_CF(double t0, double t1, double rT0) {
            // 3.39
            var(aT0T1, bT0T1) = AB(t0, t1);
            return aT0T1 * Exp(-bT0T1 * rT0);
        }

        internal double Alpha(double t) {
            var a = this.A();
            var sigma = this.Sigma();
            return MarketF(t) + sigma.Sq() / (2 * a.Sq()) * (1 - Exp(-a * t)).Sq();
        }

        internal double V(double tx, double ty) {
            var a = this.A();
            var sigma = this.Sigma();
            return sigma.Sq() / a.Sq() * (ty - tx + 2 / a * Exp(-a * (ty - tx)) -
                                          1 / (2 * a) * Exp(-2 * a * (ty - tx)) - 3 / (2 * a));
        }

        public double HW_ZBPrice_SM(double t0, double t1, double rT0, int n = 500000) {
            var bT0T1 = AB(t0, t1).bt0t1;
            var miu = bT0T1 * (rT0 - Alpha(t0)) + Log(MarketP(t0) / MarketP(t1)) + (V(0, t1) - V(0, t0)) / 2;
            var var = V(t0, t1);
            var samples = NormalDist.NextSamples(miu, Sqrt(var), n);
            var expectation = samples.Average(sample => Exp(-sample));
            return expectation;
        }

        public double HW_ZBPut_CF(double t0, double t1, double t2, double x, double rT0) {
            // 3.41
            var(_, bts) = AB(t1, t2);
            var pT0T1 = HW_ZBPrice_CF(t0, t1, rT0);
            var pT0T2 = HW_ZBPrice_CF(t0, t2, rT0);
            var a = this.A();
            var sigmaP = this.Sigma() * Sqrt((1 - Exp(-2 * a * (t1 - t0))) / (2 * a)) * bts;
            var h = Log(pT0T2 / (pT0T1 * x)) / sigmaP + sigmaP / 2;
            var price = -pT0T2 * NormalDist.NormDist(-h) + x * pT0T1 * NormalDist.NormDist(-h + sigmaP);
            return price;
        }

        public double HW_ZBPut_SM_Q(double t0, double t1, double t2, double x, double rT0, int m = 100, int n = 500) {
            var a = this.A();
            var sigma = this.Sigma();

            var dt = (t1 - t0) / m;
            var alpha0 = MarketF(0);

            var samples = new double[n];
            var ps = new double[n];
            var rt = new double[n][];
            for (var i = 0; i < n; i++) {
                rt[i] = new double[m];
                var laji = 0D;
                var t = t0;
                for (var j = 0; j < m; j++) {
                    t = t + dt;
                    laji += Exp(a * t) * NormalDist.NextSample(0, Sqrt(dt));
                    rt[i][j] = rT0 * Exp(-a * t) + Alpha(t) - alpha0 * Exp(-a * t) + sigma * Exp(-a * t) * laji;
                }
                var discount = Exp(-rt[i].Sum() * dt);
                var rt1 = rt[i][m - 1];
                var pT1T2 = HW_ZBPrice_CF(t1, t2, rt1);
                ps[i] = pT1T2;
                samples[i] = discount * Max(x - pT1T2, 0);
            }
            return samples.Average();
        }

        internal double Mt(double s, double t, double T) {
            var a = this.A();
            var sigma = this.Sigma();
            return sigma.Sq() / a.Sq() * (1 - Exp(-a * (t - s))) -
                   sigma.Sq() / (2 * a.Sq()) * (Exp(-a * (T - t)) - Exp(-a * (T + t - 2 * s)));

        }

        public double HW_ZBPut_SM_T(double t0, double t1, double t2, double x, double rT0, int n = 500) {
            // 3.37
            var a = this.A();
            var sigma = this.Sigma();

            var miu = rT0 * Exp(-a * (t1 - t0)) + Alpha(t1) - Alpha(t0) * Exp(-a * (t1 - t0));
            //var miu = xs * Exp(-a * (t1 - t0)) - Mt(t0, t1, t2) + Alpha(t1);
            var var = sigma.Sq() / (2 * a) * (1 - Exp(-2 * a * (t1 - t0)));
            var rt1 = NormalDist.NextSamples(miu, Sqrt(var), n);

            var pT0T1 = MarketP(t1);//HW_ZBPrice_CF( t0, t1, rT0);

            var samples = new double[n];
            for (var i = 0; i < n; i++) 
                samples[i] = HW_ZBPrice_CF(t1, t2, rt1[i]);

            var expectation = samples.Average(sample => Max(x - sample, 0));
            return pT0T1 * expectation;
        }


        private double HW_Caplets(double t0, double t1, double t2, double x, double n, double rT0) {
            // 2.27
            var nPrime = n * (1 + x * (t2 - t1));
            var xPrime = 1 / (1 + x * (t2 - t1));
            var cpl = nPrime * HW_ZBPut_CF(t0, t1, t2, xPrime, rT0);
            return cpl;
        }

        private double HW_Caps(double t0, double tAlpha, double[] taoArray, double x, double n, double rT0) {
            // 3.42
            var term = taoArray.Length;
            var caps = 0D;
            var t1 = tAlpha;
            var t2 = tAlpha;
            for (var i = 0; i < term; i++) {
                if (i > 0)
                    t1 = t2;
                t2 += taoArray[i];
                caps = caps + HW_Caplets(t0, t1, t2, x, n, rT0);
            }
            return caps;
        }

        private double[] HW_R_CM(double s, double rs, double t0, int n) {
            // 3.37
            if (t0 == s) return new[] {rs};
            var sigma = this.Sigma();
            var a = this.A();

            double AlphaFunc(double t) => MarketF(t) +
                                          Pow(sigma, 2) / (2 * Pow(a, 2)) * Pow(1 - Exp(-a * t), 2);

            var mu = rs * Exp(-a * (t0 - s)) + AlphaFunc(t0) -
                     AlphaFunc(s) * Exp(-a * (t0 - s));
            var stdvar = Pow(sigma, 2) / (2 * Pow(a, 2)) *
                         Pow(1 - Exp(-2 * a * (t0 - s)), 2);
            var sampleFunc = MonteCarloSimulation.GenerateSamples(
                n, 0.Wrap(), (t, x) => x, mu.Wrap(), stdvar.Wrap());
            var samples = sampleFunc();
            return samples;
        }

        public double HW_CAP_CM(double t0, double tAlpha, double[] taoArray, double x, double n, int num) {
            // 3.37
            Func<double, double> alphaFunc = t => MarketF(t) +
                                                    Pow(this.Sigma(), 2) / (2 * Pow(this.A(), 2)) *
                                                    Pow(1 - Exp(-this.A() * t), 2);
            var mu = MarketF(0) * Exp(-this.A() * t0) + alphaFunc(t0) -
                     alphaFunc(0) * Exp(-this.A() * t0);
            var sigma = Pow(this.Sigma(), 2) / (2 * Pow(this.A(), 2)) *
                        Pow(1 - Exp(-2 * this.A() * t0), 2);
            var sampleFunc = MonteCarloSimulation.GenerateSamples(
                num, 0.Wrap(), (t, rt) => HW_Caps(t0, tAlpha, taoArray, x, n, rt), mu.Wrap(), sigma.Wrap());
            var samples = sampleFunc();
            return samples.Average();
        }


        private readonly Dictionary<double, double> _mf = new Dictionary<double, double>();
        private readonly Dictionary<double, double> _mp = new Dictionary<double, double>();

        public double MarketF(double t) {
            if (_mf.ContainsKey(t))
                return _mf[t];
            var marketF = Interpolation.Linear(t, _marketT, _marketF);
            _mf[t] = marketF;
            return marketF;
        }


        public double MarketP(double t) {
            if (_mp.ContainsKey(t))
                return _mp[t];
            var marketP = Interpolation.Linear(t, _marketT, _marketP);
            _mp[t] = marketP;
            return marketP;
        }


        public static SimulatedAnnealing<HullWhiteModel> HW_SaaSolver(
            double[] marketT, double[] marketP, double[] marketF, double t0, double tAlpha,
            IReadOnlyCollection<(double, double[])> strikeAndTaos, double[] capsTargets, double notional,
            IReadOnlyCollection<(double, double)> ranges, int samplesCount = 100, int threads = 6,
            int temperature = 400, int iters = 20000, int cooliter = 40, bool debug = false) {
            var m = strikeAndTaos.Count;
            var paras = new object[] {string.Empty, 1, .1, marketT, marketP, marketF};
            Func<HullWhiteModel, double> obj = hwx => {
                var capFuncs = strikeAndTaos
                    .Select<(double, double[]), Func<double[], double>>(
                        sat => rts => rts.Average(r => hwx.HW_Caps(t0, tAlpha, sat.Item2, sat.Item1, notional, r)))
                    .ToArray();
                var totalDiff = 0D;
                var rs = hwx.HW_R_CM(0, marketF[0], t0, samplesCount);
                for (var i = 0; i < m; i++) {
                    var cp = capFuncs[i](rs);
                    var diff = (cp - capsTargets[i]) / (capsTargets[i]);
                    totalDiff += Pow(diff, 2);
                }
                return totalDiff;
            };
            var saaSolver =
                new SimulatedAnnealing<HullWhiteModel>(
                    paras, new[] {"A", "Sigma"}, ranges, obj, threads, temperature, iters, cooliter, debug);
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
