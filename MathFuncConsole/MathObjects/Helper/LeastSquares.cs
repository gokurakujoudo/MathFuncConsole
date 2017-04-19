using System;
using System.Linq;

namespace MathFuncConsole.MathObjects.Helper {
    internal static class LeastSquares {

        ///<summary>
        ///Implementation least squares method
        ///</summary>
        ///<param name="xs">Known xs</param>
        ///<param name="ys">Known ys</param>
        ///<param name="dimension">Highest dimension of x</param>
        ///<returns>[a0,a1,...,a-dim]</returns>
        public static double[] Generate(double[] xs, double[] ys, int dimension) {
            if (xs == null || ys == null || xs.Length <= 1 || xs.Length != ys.Length
                || xs.Length < dimension || dimension < 2) {
                throw new ArgumentException("Invalid match of length");
            }
            var s = new double[(dimension - 1) * 2 + 1];
            for (var i = 0; i < s.Length; i++) {
                foreach (var t in xs)
                    s[i] += Math.Pow(t, i);
            }
            var b = new double[dimension];
            for (var i = 0; i < b.Length; i++) {
                for (var j = 0; j < xs.Length; j++)
                    b[i] += Math.Pow(xs[j], i) * ys[j];
            }
            var a = new double[dimension][];
            for (var i = 0; i < dimension; i++) {
                a[i] = new double[dimension];
                for (var j = 0; j < dimension; j++)
                    a[i][j] = s[i + j];
            }

            // Now we need to calculate each coefficients of augmented matrix  
            return CalcLinearEquation(a, b);
        }

        /* 
         * Calculate linear equation. 
         *  
         * The matrix equation is like this: Ax=B 
         *  
         * @param a two-dimensional array 
         *  
         * @param b one-dimensional array 
         *  
         * @return x, one-dimensional array 
         */
        private static double[] CalcLinearEquation(double[][] a, double[] b) {
            if (a == null || b == null || a.Length == 0 || a.Length != b.Length)
                return null;
            if (a.Any(x => x == null || x.Length != a.Length))
                return null;
            var len = a.Length - 1;
            var result = new double[a.Length];
            if (len == 0) {
                result[0] = b[0] / a[0][0];
                return result;
            }
            var aa = new double[len][];
            for (var i = 0; i < len; i++)
                aa[i] = new double[len];
            var bb = new double[len];
            int posx = -1, posy = -1;
            for (var i = 0; i <= len; i++) {
                for (var j = 0; j <= len; j++)
                    if (a[i][j] != 0.0d) {
                        posy = j;
                        break;
                    }
                if (posy == -1) continue;
                posx = i;
                break;
            }
            if (posx == -1)
                return null;
            var count = 0;
            for (var i = 0; i <= len; i++) {
                if (i == posx)
                    continue;
                bb[count] = b[i] * a[posx][posy] - b[posx] * a[i][posy];
                var count2 = 0;
                for (var j = 0; j <= len; j++) {
                    if (j == posy)
                        continue;

                    aa[count][count2] = a[i][j] * a[posx][posy] - a[posx][j] * a[i][posy];
                    count2++;
                }
                count++;
            }

            // Calculate sub linear equation  
            var result2 = CalcLinearEquation(aa, bb);

            // After sub linear calculation, calculate the current coefficient  
            var sum = b[posx];
            count = 0;
            for (var i = 0; i <= len; i++) {
                if (i == posy)
                    continue;
                sum -= a[posx][i] * result2[count];
                result[i] = result2[count];
                count++;
            }
            result[posy] = sum / a[posx][posy];
            return result;
        }

        public static double Fit(double x, double[] coefficient) =>
            coefficient?.Select((t, i) => Math.Pow(x, i) * t).Sum() ?? 0;

        //
    }
}
