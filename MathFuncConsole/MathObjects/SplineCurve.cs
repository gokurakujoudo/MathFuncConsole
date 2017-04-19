using System.Collections.Generic;
using System.Linq;
using MathFuncConsole.MathObjects.Helper;

namespace MathFuncConsole.MathObjects {
    internal struct SplineCurve {
        private readonly (double x, double y)[] _ordered;

        public SplineCurve(IEnumerable<(double x, double y)> ordered) { _ordered = ordered.OrderBy(tu => tu.x).ToArray(); }

        public SplineCurve(IEnumerable<double> x, IEnumerable<double> y) {
            var zip = x.Zip(y, (xi, yi) => (x:xi, y:yi)).ToArray();
            _ordered = zip.OrderBy(tu => tu.x).ToArray();
        }

        public SplineCurve CubicSplineExpand(int k = 1000) {
            var maxx = _ordered[_ordered.Length - 1].x;
            var newx = Enumerable.Range(0, k).Select(x => x * maxx / k).ToArray();
            var newy = Interpolation.CubicSpline(Points(), newx);
            return new SplineCurve(newx, newy);
        }

        public double this[double x] => Interpolation.Linear(x, _ordered);

        public double[] X() => _ordered.Select(tu => tu.x).ToArray();
        public double[] Y() => _ordered.Select(tu => tu.y).ToArray();
        public (double x, double y)[] Points() => _ordered;
    }
}