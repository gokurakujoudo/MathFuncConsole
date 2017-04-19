using System;

namespace MathFuncConsole.MathObjects.Helper {
    internal static class LeastSquares {
        ///<summary>
        ///Implementation least squares method
        ///</summary>
        ///<param name="xs">Known xs</param>
        ///<param name="ys">Known ys</param>
        ///<param name="dimension">Highest dimension of x</param>
        ///<returns>[a0,a1,...,a-dim]</returns>
        public static double[] MultiLine(double[] xs, double[] ys, int dimension)
        {
            var length = xs.Length;
            var n = dimension + 1; 
            var guass = new double[n, n + 1]; 
            for (var i = 0; i < n; i++) {
                int j;
                for (j = 0; j < n; j++) 
                    guass[i, j] = SumArr(xs, j + i, length);
                guass[i, j] = SumArr(xs, i, ys, 1, length);
            }
            return ComputGauss(guass, n);
        }

        /// <summary>
        /// Sum of element to the power of n
        /// </summary>
        /// <param name="arr">Input array</param>
        /// <param name="n">Power</param>
        /// <param name="length">Length of the array</param>
        /// <returns></returns>
        private static double SumArr(double[] arr, int n, int length) 
        {
            var s = 0D;
            for (var i = 0; i < length; i++)
                s += (arr[i] != 0 || n != 0) ? Math.Pow(arr[i], n) : 1;
            return s;
        }

        /// <summary>
        /// Sum of pair of elements to the power of ns
        /// </summary>
        /// <param name="arr1">Input array1</param>
        /// <param name="n1">Power1</param>
        /// <param name="arr2">Input array2</param>
        /// <param name="n2"></param>
        /// <param name="length">Power2</param>
        /// <returns></returns>
        private static double SumArr(double[] arr1, int n1, double[] arr2, int n2, int length) {
            double s = 0;
            for (var i = 0; i < length; i++) 
                if ((arr1[i] != 0 || n1 != 0) && (arr2[i] != 0 || n2 != 0))
                    s += Math.Pow(arr1[i], n1) * Math.Pow(arr2[i], n2);
                else
                    s += 1;
            return s;
        }

        private static double[] ComputGauss(double[,] guass, int n) {
            var x = new double[n];
            for (var i = 0; i < n; i++) x[i] = 0D; 
            for (var j = 0; j < n; j++) {
                var  max = 0D;
                var k = j;
                for (var i = j; i < n; i++) 
                    if (Math.Abs(guass[i, j]) > max) {
                        max = guass[i, j];
                        k = i;
                    }
                if (k != j) 
                    for (var m = j; m < n + 1; m++) {
                        var temp = guass[j, m];
                        guass[j, m] = guass[k, m];
                        guass[k, m] = temp;
                    }                
                if (0 == max) 
                    return x;
                for (var i = j + 1; i < n; i++) {
                    var s = guass[i, j];
                    for (var m = j; m < n + 1; m++) {
                        guass[i, m] = guass[i, m] - guass[j, m] * s / (guass[j, j]);
                    }
                }
            } 
            for (var i = n - 1; i >= 0; i--) {
                var s = 0D;
                for (var j = i + 1; j < n; j++) {
                    s += guass[i, j] * x[j];
                }
                x[i] = (guass[i, n] - s) / guass[i, i];
            }
            return x;
        } 



        //
    }
}
