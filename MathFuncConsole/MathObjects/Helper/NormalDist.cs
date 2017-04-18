using System;

namespace MathFuncConsole.MathObjects.Helper {
    /// <summary>
    /// Helper class for Normal Distribution
    /// </summary>
    public static class NormalDist {
        /// <summary>
        /// Work as the same as Excel function NORMDIST(...)
        /// </summary>
        /// <param name="x">Value of input</param>
        /// <param name="mean">Mean of the Normal Distribution</param>
        /// <param name="std">Standard deviation for the Normal Distribution</param>
        /// <param name="cumulative">Return CDF or PDF</param>
        /// <returns></returns>
        public static double NormDist(double x, double mean = 0, double std = 1, bool cumulative = true) {
            if (cumulative) {
                return Phi(x, mean, std);
            }
            var tmp = 1 / ((Math.Sqrt(2 * Math.PI) * std));
            return tmp * Math.Exp(-.5 * Math.Pow((x - mean) / std, 2));
        }

        private static double Erf(double z) {
            var t = 1.0 / (1.0 + 0.5 * Math.Abs(z));

            // use Horner's method
            var ans = 1 - t * Math.Exp(-z * z - 1.26551223 +
                                       t * (1.00002368 +
                                            t * (0.37409196 +
                                                 t * (0.09678418 +
                                                      t * (-0.18628806 +
                                                           t * (0.27886807 +
                                                                t * (-1.13520398 +
                                                                     t * (1.48851587 +
                                                                          t * (-0.82215223 +
                                                                               t * (0.17087277))))))))));
            if (z >= 0) return ans;
            return -ans;
        }

        // cumulative normal distribution
        private static double Phi(double z) => 0.5 * (1.0 + Erf(z / (Math.Sqrt(2.0))));

        // cumulative normal distribution with mean mu and std deviation sigma
        private static double Phi(double z, double mu, double sigma) => Phi((z - mu) / sigma);

        private static readonly Random Rnd = new Random();
        private const double TWO_PI = 2 * Math.PI;

        /// <summary>
        /// Generate samples from normal distribution
        /// </summary>
        /// <param name="mu">Mean of the normal distribution</param>
        /// <param name="sigma">Standard deviation of the normal distribution</param>
        /// <returns></returns>
        public static double NextSample(double mu = 0, double sigma = 1) {
            var u1 = 1 - Rnd.NextDouble();
            var u2 = Rnd.NextDouble();
            var r = Math.Sqrt(-2 * Math.Log(u1));
            var t = u2 * TWO_PI;
            var s = r * Math.Cos(t); //r*sin(t)
            return s * sigma + mu;
        }

        /// <summary>
        /// Generate samples from normal distribution
        /// </summary>
        /// <param name="mu">Mean of the normal distribution</param>
        /// <param name="sigma">Standard deviation of the normal distribution</param>
        /// <param name="n">Number of samples</param>
        /// <returns></returns>
        public static double[] NextSamples(double mu = 0, double sigma = 1, int n = 100) {
            var samples = new double[n];
            for (var i = 0; i < n; i += 2) {
                var u1 = 1 - Rnd.NextDouble();
                var u2 = Rnd.NextDouble();
                var r = Math.Sqrt(-2 * Math.Log(u1));
                var t = u2 * TWO_PI;
                samples[i] = r * Math.Cos(t);
                if (i + 1 < n)
                    samples[i + 1] = r * Math.Sin(t);
            }
            return samples;
        }
    }
}

