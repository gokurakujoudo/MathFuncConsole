using System;
using System.Linq;
using MathFuncConsole.MathObjects.Applications;
using MathFuncConsole.MathObjects.Helper;

namespace MathFuncConsole {
    internal static class Program {
        private static void Main(string[] args) {
            Console.Title = "Quantitative Finance Pricing Console";

            //Demo_DynamicOptionsPricing();
            //Demo_ImpliedVolitity();
            //Demo_SaaImpliedVolitity();
            //Demo_EquationSetSaaSolver();

            Demo_HW_SaaSolver();

            Console.WriteLine();

            Console.Read();
        }

        private static void Demo_HW_SaaSolver() {
            var marketT = new[] {
                0, 0.255555556, 0.511111111, 0.761111111, 1.013888889, 1.269444444, 1.530555556, 1.775, 2.027777778,
                2.291666667, 2.541666667, 2.797222222, 3.047222222, 3.302777778, 3.555555556, 3.805555556, 4.058333333,
                4.313888889, 4.569444444, 4.819444444
            };

            var marketP = new[] {
                1, 0.993664, 0.989978, 0.98618, 0.982123, 0.977784, 0.973149, 0.968688, 0.963922, 0.958764, 0.95371,
                0.948381, 0.943253, 0.937892, 0.932477, 0.927016, 0.921614, 0.916074, 0.91046, 0.904898
            };

            var marketF = new[] {
                .0115761, 0.0134633, 0.0145677, 0.015404, 0.016344, 0.0173619, 0.0182408, 0.0188542, 0.0195574,
                0.0203891,
                0.0212078, 0.0219844, 0.0217523, 0.0223657, 0.0229775, 0.0235635, 0.0231886, 0.0236636, 0.0241312,
                0.0245827
            };

            var t0 = 0.2;
            var tAlpha = .5;
            var notional = 1E6;
            var samplesCount = 100;
            var strikeAndTaos = new[] {
                (.01, new[] {.25, .25, .25, .25}),
                (.02, new[] {.25, .25, .25, .25, .25, .25}),
                (.03, new[] {.25, .25, .25, .25, .25, .25, .25, .25}),
                (.04, new[] {.25, .25, .25, .25, .25, .25, .25, .25, .25, .25}),
            };

            var hwTarget = new HullWhiteModel("hw1", .5, .05, marketT, marketP, marketF);
            var capsTargets = strikeAndTaos
                .Select(sat => hwTarget.HW_CAP_CM(t0, tAlpha, sat.Item2, sat.Item1, notional, samplesCount))
                .ToArray();

            var temperature = 500;
            var iters = 20000;
            var cooliter = 40;
            var ranges = new[] {(0D, 5D), (0D, 0.5D)};
            var threads = 5;

            var saaSolver = HullWhiteModel.HW_SaaSolver(marketT, marketP, marketF, t0, tAlpha, strikeAndTaos, capsTargets, notional,
                                                        ranges, samplesCount, threads, temperature, iters, cooliter,
                                                        debug: true);

            saaSolver.Run();
        }



        private static void Demo_DynamicOptionsPricing() {
            var s1 = new Stock("s1", 100, 0.2, divd: 0.03);
            var s2 = new Stock("s2", 120, 0.3, divd: 0.02);
            var eo = new ExchangeOption("eo", s1, s2, 0.5, 1);
            var deo = new DeferredExchangeOption("deo", s1, s2, 0.5, 1, 2);
            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(eo);
            Console.WriteLine(deo);
            Console.WriteLine();

            s1.Price = 120.Wrap();
            s2.Divd = 0.01.Wrap();
            eo.Rho = 0.7.Wrap();
            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(eo);
            Console.WriteLine(deo);
            Console.WriteLine();
        }

        private static void Demo_ImpliedVolitity() {
            var go1 = new GenericOption("go1", 100, 120, 1, 0.2);
            var go2 = new GenericOption("go2", go1.Pv1, go1.Pv2, go1.Maturity, price: go1.Price);
            Console.WriteLine(go1);
            Console.WriteLine(go2);

            go1.Sigma = 0.3.Wrap();
            go1.Pv2 = 100.Wrap();
            Console.WriteLine(go1);
            Console.WriteLine(go2);
            Console.WriteLine();
        }

        private static void Demo_SaaImpliedVolitity() {
            var target = new GenericOption("go1", 100, 120, 1, 0.2);

            var paras = new object[] {string.Empty, target.Pv1, target.Pv2, target.Maturity, 0, null};
            var xNames = new[] { nameof(GenericOption.Sigma) };
            var range = new[] { (0D, 1D) };
            Func<GenericOption, double> objectiveFunc = go => Math.Abs(go.Price() - target.Price());

            var sa = new SimulatedAnnealing<GenericOption>(paras, xNames, range, objectiveFunc);

            var saResult = sa.Run();
            Console.WriteLine($"x -> {saResult.x.ToStr()}, y -> {saResult.y:E6}");
        }



        private static void Demo_EquationSetSaaSolver() {
            var vars = new[] {"a", "b", "c"};
            var ranges = new[] {(0D, 100D), (0D, 100D), (0D, 100D)};
            var equations = new Func<EquationSet, Func<double>>[] {
                eq => () => Math.Pow(eq["a"](), 2) + Math.Pow(eq["b"](), 2) - Math.Pow(eq["c"](), 2),
                eq => () => eq["a"]() + eq["b"]() + eq["c"]() - 12,
                eq => () => eq["a"]() + 2 * eq["b"]() - 11
            };
            var sa = EquationSet.SaaSolver(vars, equations, ranges);

            var (xStar, error) = sa.Run();
            Console.WriteLine($"Solutions: {xStar.ToStr()}, with error {error:F6}");
        }
    }
}

