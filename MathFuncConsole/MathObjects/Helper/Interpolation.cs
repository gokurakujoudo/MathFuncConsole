using System;

namespace MathFuncConsole.MathObjects.Helper {
    static class Interpolation {

        public static double Linear(double x0, double[] xs, double[] ys) {
            var lower = 0;
            var upper = xs.Length - 1;
            if (x0 < xs[lower])
                upper = 1;
            else if (x0 > xs[upper])
                lower = upper - 1;
            else {
                var mid = (lower + upper) / 2;
                while (lower <= mid && mid <= upper && upper - lower > 1) {
                    var x = xs[mid];
                    if (x0 == x)
                        return ys[mid];
                    if (x0 > x)
                        lower = mid;
                    if (x0 < x)
                        upper = mid;
                    mid = (lower + upper) / 2;
                }
            }
            var (x1, y1) = (xs[lower], ys[lower]);
            var (x2, y2) = (xs[upper], ys[upper]);
            return x1 + (x0 - x1) / (x2 - x1) * (y2 - y1);
        }

        public static double Linear(double x0, (double x, double y)[] ordered) {
            var lower = 0;
            var upper = ordered.Length - 1;
            if (x0 < ordered[lower].x) 
                upper = 1; //smaller
            else if (x0 > ordered[upper].x) 
                lower = upper - 1; //larger
            else {
                var mid = (lower + upper) / 2;
                while (lower <= mid && mid <= upper && upper - lower > 1) {
                    var x = ordered[mid].x;
                    if (x0 == x)
                        return ordered[mid].y;
                    if (x0 > x)
                        lower = mid;
                    if (x0 < x)
                        upper = mid;
                    mid = (lower + upper) / 2;
                }
            }
            var (x1, y1) = ordered[lower];
            var (x2, y2) = ordered[upper];
            return x1 + (x0 - x1) / (x2 - x1) * (y2 - y1);
        }

        public static double[] CubicSpline((double x, double y)[] points, double[] xs)
        {
            var n = points.Length;
            var h = new double[n];
            var f = new double[n];
            var l = new double[n];
            var v = new double[n];
            var g = new double[n];

            for (var i = 0; i < n - 1; i++)
            {
                h[i] = points[i + 1].x - points[i].x;
                f[i] = (points[i + 1].y - points[i].y) / h[i];
            }

            for (var i = 1; i < n - 1; i++)
            {
                l[i] = h[i] / (h[i - 1] + h[i]);
                v[i] = h[i - 1] / (h[i - 1] + h[i]);
                g[i] = 3 * (l[i] * f[i - 1] + v[i] * f[i]);
            }

            var b = new double[n];
            var tem = new double[n];
            var m = new double[n];
            var fn = (points[n - 1].y - points[n - 2].y) / (points[n - 1].x - points[n - 2].x);

            b[1] = v[1] / 2;
            for (var i = 2; i < n - 2; i++)
                b[i] = v[i] / (2 - b[i - 1] * l[i]);
            tem[1] = g[1] / 2;
            for (var i = 2; i < n - 1; i++)
                tem[i] = (g[i] - l[i] * tem[i - 1]) / (2 - l[i] * b[i - 1]);
            m[n - 2] = tem[n - 2];
            for (var i = n - 3; i > 0; i--)
                m[i] = tem[i] - b[i] * m[i + 1];
            m[0] = 3 * f[0] / 2.0;
            m[n - 1] = fn;
            var xlength = xs.Length;
            var insertRes = new double[xlength];
            for (var i = 0; i < xlength; i++)
            {
                int j;
                for (j = 0; j < n; j++)
                    if (xs[i] < points[j].x)
                        break;
                j -= 1;
                if (j == -1 || j == points.Length - 1)
                {
                    if (j == -1)
                        throw new Exception("Out of upper bound");
                    if (j == points.Length - 1 && xs[i] == points[j].x)
                        insertRes[i] = points[j].y;
                    else
                        throw new Exception("Out of lower bound");
                }
                else
                {
                    var p1 = ((xs[i] - points[j + 1].x) / (points[j].x - points[j + 1].x)).Sq();
                    var p2 = (xs[i] - points[j].x) / (points[j + 1].x - points[j].x).Sq();
                    var p3 = p1 * (1 + 2 * (xs[i] - points[j].x) / (points[j + 1].x - points[j].x)) * points[j].y + p2 *
                             (1 + 2 * (xs[i] - points[j + 1].x) / (points[j].x - points[j + 1].x)) * points[j + 1].y;
                    var p4 = p1 * (xs[i] - points[j].x) * m[j] + p2 * (xs[i] - points[j + 1].x) * m[j + 1];
                    p4 += p3;
                    insertRes[i] = p4;
                }

            }
            return insertRes;
        }

    }
}
