using System;
using System.Linq;

namespace MathFuncConsole.MathObjects.Helper {
    /// <summary>
    /// Helper class for Monte-Carlo simulation methods
    /// </summary>
    public static class MonteCarloSimulation {

        /// <summary>
        /// Generate sample function for any given random variable x under Normal distribution and cast it into 
        /// another random variable y
        /// </summary>
        /// <param name="n">Number of samples</param>
        /// <param name="x0">Initial value of x</param>
        /// <param name="yFunc">A function maps x to y</param>
        /// <param name="mu0">Initial mean of x</param>
        /// <param name="sigma0">Initial sigma of x</param>
        /// <param name="iter">Stages in a path</param>
        /// <param name="t0">Initial time</param>
        /// <param name="dt">Delta time</param>
        /// <param name="shiftFunc">Distribution shift function</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Func<double[]> GenerateSamples(int n, Func<double> x0, Func<double, double, double> yFunc,
                                                Func<double> mu0, Func<double> sigma0, long iter = 1,
                                                Func<double> t0 = null, Func<double> dt = null,
                                                Func<double, double, (double, double)> shiftFunc = null) {
            if (iter <= 0) throw new ArgumentException(nameof(iter));
            if (iter == 1) {
                return () => {
                    var ys = new double[n];
                    var t = t0?.Invoke() ?? 0;
                    for (var i = 0; i < n; i++) {
                        var xi = x0() + NormalDist.NextSample(mu0(), sigma0());
                        ys[i] = yFunc(t, xi);
                    }
                    return ys.ToArray();
                };
            }

            return null;
            //TODO
        }

        private class McWorker {
            private readonly Random _r;
            private readonly double _dt;
            private double _mu, _sigma, _y, _x, _t;
            private readonly Func<double, double, double> _yFunc;
            private readonly Func<double, double, (double, double)> _shiftFunc;
            private readonly long _iter;

            public McWorker(double x0, Func<double, double, double> yFunc, double mu0, double sigma0, long iter,
                            double t0 = 0, double dt = 0, Func<double, double, (double, double)> shiftFunc = null) {
                _r = new Random();
                _t = t0;
                _x = x0;
                _mu = mu0;
                _iter = iter;
                _dt = dt;
                _sigma = sigma0;
                _yFunc = yFunc;
                _shiftFunc = shiftFunc;
            }

            public void Run() {
                for (var i = 0L; i < _iter; i++) {
                    _t += _dt;
                    if (_shiftFunc != null) {
                        var shift = _shiftFunc(_t, _x);
                        _mu += shift.Item1;
                        _sigma += shift.Item2;
                    }
                    _x += this.Norm(_mu, _sigma);
                }
                _y = _yFunc(_t, _x);
            }

            public (double x, double y) Result() => (_x, _y);

            private const double TWO_PI = 2 * Math.PI;

            private double Norm(double mu, double sigma) {
                var u1 = 1 - _r.NextDouble();
                var u2 = _r.NextDouble();
                var r = Math.Sqrt(-2 * Math.Log(u1));
                var t = u2 * TWO_PI;
                var s = r * Math.Cos(t); //r*sin(t)
                return s * sigma + mu;
            }
        }
    }
}
