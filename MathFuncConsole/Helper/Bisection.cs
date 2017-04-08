using System;
using MathFuncConsole.MathObjects;
using MathFuncConsole.MathObjects.Applications;

namespace MathFuncConsole.Helper {
    /// <summary>
    /// Helper class of bisection method for backing out implied variable from complex but monotonic function.
    /// </summary>
    public static class Bisection {
        /// <summary>
        /// General bisection method designed for <see cref="MathObject"/>. This function is designed for
        /// internal use inside sub-classes of <see cref="MathObject"/>. Check .ctor of <seealso cref="GenericOption"/> 
        /// for how to use this method to back out implied volatility from option price.
        /// </summary>
        /// <param name="updateFunc">Function that update dependent variable, should be a remote link between properties in <see cref="MathObject"/></param>
        /// <param name="target">Target value to be obtained by dependent variable by changing independent variable</param>
        /// <param name="range"><see cref="ValueTuple"/> that contains lower and upper bound of the searching scope</param>
        /// <param name="eps">Tolerance of the dependent variable and searching scope, by default is <see langword="1E-5"/></param>
        /// <returns></returns>
        public static double Search(Func<double,double> updateFunc, double target,
                                    (double lower, double upper) range, double eps = 1E-5) {

            var vLower = updateFunc(range.lower);
            if (MathClassHelper.WithinTolerance(target, vLower, eps)) return vLower;
            var vUpper = updateFunc(range.upper);
            if (MathClassHelper.WithinTolerance(target, vUpper, eps)) return vUpper;
            if (vLower * vUpper > 0) throw new ArgumentException("bad range choice");

            var (lower, upper) = range;

            while (!MathClassHelper.WithinTolerance(lower, upper, eps)) {
                var guess = (lower + upper) / 2;
                var vGuess = updateFunc(guess);
                if (MathClassHelper.WithinTolerance(target, vGuess, eps)) return guess;
                if ((vGuess - target) * (vUpper - target) < 0)
                    lower = guess;
                else {
                    upper = guess;
                    vUpper = vGuess;
                }
            }

            return (lower + upper) / 2;
        }
    }
}
