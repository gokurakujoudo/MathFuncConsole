namespace MathFuncConsole.MathObjects.Helper {
    static class Interpolation {

        public static double Linear(double x0, double[] xs, double[] ys) {
            var length = xs.Length;
            double p;
            if (x0 > xs[length - 1])
                p = ys[length - 1] + (x0 - xs[length - 1]) / (xs[length - 1] - xs[length - 2]) *
                    (ys[length - 1] - ys[length - 2]);
            else if (x0 < xs[0])
                p = ys[0] - (xs[0] - x0) / (xs[1] - xs[0]) * (ys[1] - ys[0]);
            else {
                int min = 0, max = 0;
                for (var i = 0; i < length - 1; i++) {
                    if (xs[i] == x0)
                        return ys[i];
                    if (!(xs[i] < x0) || !(xs[i + 1] > x0)) continue;
                    min = i;
                    max = i + 1;
                    break;
                }
                p = ys[min] + (x0 - xs[min]) / (xs[max] - xs[min]) * (ys[max] - ys[min]);
            }
            return p;
        }
    }
}
