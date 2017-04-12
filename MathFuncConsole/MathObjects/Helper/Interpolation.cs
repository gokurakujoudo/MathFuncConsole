namespace MathFuncConsole.MathObjects.Helper {
    static class Interpolation {

        public static double Linear(double x0, double[] xs, double[] ys) {
            var k = 0;
            for (; k < xs.Length - 1; k++) if (xs[k + 1] > x0) break;
            if (k == xs.Length - 1) k--;
            return ys[k] + (x0 - xs[k]) / (xs[k + 1] - xs[k]) * (ys[k + 1] - ys[k]);
        }
    }
}
