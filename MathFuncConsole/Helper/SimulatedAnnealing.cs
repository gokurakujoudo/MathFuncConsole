
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MathFuncConsole.MathObjects;
using MathFuncConsole.MathObjects.Applications;

namespace MathFuncConsole.Helper {
    /// <summary>
    /// Implementation of multi-thread Simulated Annealing Algorithm (SAA). Designed for searching numeric solution 
    /// or optimizing target function of <see cref="MathObject"/>. 
    /// </summary>
    /// <typeparam name="T">Type of <see cref="MathObject"/> you want to play with</typeparam>
    public class SimulatedAnnealing<T> where T : MathObject {
        private readonly ConcurrentBag<SimulatedAnnealingWorker> _workers;

        /// <summary>
        /// Initial instance of <see cref="SimulatedAnnealing{T}"/>
        /// </summary>
        /// <param name="dummies">A set of identical dummy-<see cref="MathObject"/>s that will work as a calculator 
        /// in its own <see cref="Task"/>. SAA will be slower but more accurate if you have many dummies</param>
        /// <param name="xNames">Names of property from dummy as independent variables</param>
        /// <param name="ranges">Lower and upper bounds for each independent variables </param>
        /// <param name="objectiveFunc">Function that maps from a <see langword="T"/> to objection value. 
        /// For solving an equation, function should be set to error from target value. For optimizing, 
        /// function should be set for minimizing</param>
        /// <param name="temperature">Initial temperature for SAA. Model will converge slower with higher temperature</param>
        /// <param name="iters">Total iterations for each dummy. SAA will be slower but more accurate if you have more iterations</param>
        /// <param name="cooliter">Cooling interval for SAA</param>
        /// <param name="debug">If you want to see console print-off of each 1000 turns</param>
        public SimulatedAnnealing(IEnumerable<T> dummies, IReadOnlyCollection<string> xNames,
                                  IReadOnlyCollection<(double d, double u)> ranges, Func<T, double> objectiveFunc,
                                  double temperature = 10000, long iters = 200000, int cooliter = 100,
                                  bool debug = false) {
            var setter = new List<Action<double>[]>();
            var obj = new List<Func<double>>();
            foreach (var dummy in dummies) {
                setter.Add(xNames.Select(dummy.RemoteSetter).ToArray());
                obj.Add(() => objectiveFunc(dummy));
            }
            var dim = ranges.Count;
            if (dim != xNames.Count)
                throw new ArgumentException("Dimensions of xName and range are not the same");
            var lower = ranges.Select(r => r.d).ToArray();
            var upper = ranges.Select(r => r.u).ToArray();
            if (lower.Where((t, i) => t >= upper[i]).Any())
                throw new ArgumentException("Lower bound must be smaller than upper bound");
            _workers = new ConcurrentBag<SimulatedAnnealingWorker>(
                setter.Select((setters, i) => new SimulatedAnnealingWorker(
                                  i, dim, setters, lower, upper, obj[i], temperature, iters, cooliter, debug)));
        }

        /// <summary>
        /// Run SAA in the model
        /// </summary>
        /// <returns>Global minimum point and its value</returns>
        public (double[] x, double y) Run() {
            var sw = new Stopwatch();
            sw.Start();
            Parallel.ForEach(_workers, worker => worker.Run());
            sw.Stop();
            var results = _workers.Select(w => w.Answer).ToList();
            var bestObj = results.Min(t => t.y);
            var best = results.FirstOrDefault(t => t.y == bestObj);
            Console.WriteLine(
                $"SAA finished: {sw.Elapsed} => x = {best.x.ToStr()}, y = {best.y:E2}");
            return best;
        }

        private class SimulatedAnnealingWorker {
            private readonly int _dim, _id, _cooliter;
            private readonly Action<double>[] _setters;
            private readonly double[] _upper, _lower;
            private readonly Func<double> _objective;
            private double _temp;
            private readonly long _end;
            private readonly Random _r;
            private readonly bool _debug;
            private long _itr;
            private double[] _x;
            private double _y;

            public SimulatedAnnealingWorker(int id, int dim, Action<double>[] setters, double[] lower, double[] upper,
                                            Func<double> objective, double temp, long end, int cooliter,
                                            bool debug = false) {
                _dim = dim;
                _id = id;
                _cooliter = cooliter;
                _setters = setters;
                _lower = lower;
                _upper = upper;
                _objective = objective;
                _temp = temp;
                _end = end;
                _r = new Random();
                _debug = debug;
                _x = new double[_dim];
                _y = double.MaxValue;
            }

            public void Run() {
                var sw = new Stopwatch();
                sw.Start();
                var range = new double[_dim];
                var sigma = new double[_dim];
                for (var i = 0; i < _dim; i++) {
                    range[i] = _upper[i] - _lower[i];
                    sigma[i] = range[i] * 0.05;
                    _x[i] = _lower[i] + range[i] * _r.NextDouble();
                    _setters[i](_x[i]);
                    _y = _objective();
                }
                for (_itr = 0; _itr < _end; _itr++) {
                    if (_itr % _cooliter == 0) _temp *= .99;
                    var newX = new double[_dim];
                    for (var i = 0; i < _dim; i++) {
                        var dx = this.Norm(sigma[i]);
                        while (_x[i] + dx <= _lower[i] || _x[i] + dx >= _upper[i])
                            dx = NormalDist.NextSample(0, sigma[i]);
                        newX[i] = _x[i] + dx;
                        _setters[i](newX[i]);
                    }
                    var newY = _objective();
                    var dY = newY - _y;
                    if (_debug && _itr % 1000 == 0)
                        Console.WriteLine(
                            $"{this}\r\n\t{_id:D4}: dy = {dY:F6}, t = {_temp:F4} => prob = {Math.Exp(-dY / _temp):F6}");
                    if (!(dY <= 0) && !(_r.NextDouble() <= Math.Exp(-dY / _temp))) continue;
                    _x = newX;
                    _y = newY;
                }
                sw.Stop();
                Console.WriteLine($"{_id:D4} finished: {sw.Elapsed} min = {_y:F6}");
                this.Answer = (_x, _y);
            }

            public (double[]x,double y) Answer { get; private set; }

            public override string ToString() =>
                $"{_id:D4}-{_itr:D6}: {_x.ToStr()} => {_y:F4}";

            private const double TWO_PI = 2 * Math.PI;

            private double Norm(double sigma) {
                var u1 = 1 - _r.NextDouble();
                var u2 = _r.NextDouble();
                var r = Math.Sqrt(-2 * Math.Log(u1));
                var t = u2 * TWO_PI;
                var s = r * Math.Cos(t); //r*sin(t)
                return s * sigma;
            }
        }
    }
}
