using System.Collections.Generic;
using System.Linq;
using MathFuncConsole.MathObjects.Helper;

namespace MathFuncConsole.MathObjects {
    internal class Curve {
        private readonly (double x, double y)[] _ordered;

        public Curve(IEnumerable<(double x, double y)> ordered) { _ordered = ordered.OrderBy(tu => tu.x).ToArray(); }

        public Curve(IEnumerable<double> x, IEnumerable<double> y) {
            var zip = x.Zip(y, (xi, yi) => (x:xi, y:yi)).ToArray();
            _ordered = zip.OrderBy(tu => tu.x).ToArray();
        }

        public Curve LinearExpand(int k = 1000)
        {
            var maxx = _ordered.Max(tu => tu.x);
            var minx = _ordered.Min(tu => tu.x);
            var newx = Enumerable.Range(0, k).Select(x => minx + x * (maxx - minx) / k).ToArray();
            var newy = newx.Select(x => Interpolation.LinearFit(x, _ordered));
            return new Curve(newx, newy);
        }

        public Curve CubicSplineExpand(int k = 1000) {
            var maxx = _ordered.Max(tu => tu.x);
            var minx = _ordered.Min(tu => tu.x);
            var newx = Enumerable.Range(0, k).Select(x => minx + x * (maxx - minx) / k).ToArray();
            var newy = Interpolation.CubicSpline(_ordered, newx);
            return new Curve(newx, newy);
        }

        public Curve LeastSquaresExpand(int n, int k = 1000) {
            var maxx = _ordered.Max(tu => tu.x);
            var minx = _ordered.Min(tu => tu.x);
            var newx = Enumerable.Range(0, k).Select(x => minx + x * (maxx - minx) / k).ToArray();
            var coff = LeastSquares.Generate(X(), Y(), n);
            var newy = newx.Select(x => LeastSquares.Fit(x, coff));
            return new Curve(newx, newy);
        }



        public double this[double x] => Interpolation.LinearFit(x, _ordered);

        public double[] X() => _ordered.Select(tu => tu.x).ToArray();
        public double[] Y() => _ordered.Select(tu => tu.y).ToArray();
        public (double x, double y)[] Points() => _ordered;

        public string ToString(string join) => string.Join(join, _ordered.Select(tu => tu.ToStr()));
    }
}