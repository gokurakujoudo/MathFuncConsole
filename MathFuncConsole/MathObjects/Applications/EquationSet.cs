using System;
using System.Collections.Generic;
using System.Linq;
using MathFuncConsole.Helper;

namespace MathFuncConsole.MathObjects.Applications {
    /// <summary>
    /// A group of equations and their variables.
    /// </summary>
    public class EquationSet : MathObject {
        private readonly Dictionary<string, Func<double>> _vars;
        private readonly HashSet<Func<double>> _equations;

        /// <summary>
        /// Initial instance of <see cref="EquationSet"/>
        /// </summary>
        /// <param name="name">Name of this equation set</param>
        /// <param name="vars">A list of variable names</param>
        public EquationSet(string name, IList<string> vars = null) : base(name) {
            if (vars != null && vars.Any())
                _vars = vars.Where(s => !string.IsNullOrWhiteSpace(s)).ToDictionary(v => v, v => 0.Wrap());
            _equations = new HashSet<Func<double>>();
        }

        /// <summary>
        /// Initial instance of <see cref="EquationSet"/>
        /// </summary>
        /// <param name="name">Name of this equation set</param>
        /// <param name="vars">A dictionary of variable names and their value/reference</param>
        /// <param name="equations">A group of equations</param>
        public EquationSet(string name, Dictionary<string, Func<double>> vars,
                           HashSet<Func<double>> equations = null) : base(name) {
            _vars = vars;
            _equations = new HashSet<Func<double>>();
            if (_equations != null)
                _equations = equations;
        }

        /// <summary>
        /// Add a equation into equation set
        /// </summary>
        /// <param name="eq">New equation</param>
        public void AddEquation(Func<double> eq) => _equations.Add(eq);

        /// <summary>
        /// Generate objective function of this equation set for optimization algorithms
        /// </summary>
        /// <returns>Objective function</returns>
        public Func<double> ObjectiveFunc() => () => _equations.Sum(f => Math.Abs(f()));

        /// <summary>
        /// Universal setter method without reference to the instance. To set value, input must be <see cref="double"/>.
        /// Remote setter is designed for generic computation methods.
        /// </summary>
        /// <param name="varName">Name of the target variable</param>
        /// <returns>A setter that you can assign new value to target variable</returns>
        public override Action<double> RemoteSetter(string varName) {
            if (!_vars.ContainsKey(varName)) throw new ArgumentException($"Can't find in {this.Name}", nameof(varName));
            return (f) => _vars[varName] = f.Wrap();
        }

        /// <summary>
        /// Universal getter method without reference to the instance. 
        /// Remote setter is designed for generic computation methods.
        /// </summary>
        /// <param name="varName">Name of the target variable</param>
        /// <returns>A getter that you can get new value from target variable</returns>
        public override Func<double> RemoteGetter(string varName) {
            if (!_vars.ContainsKey(varName)) throw new ArgumentException($"Can't find in {this.Name}", nameof(varName));
            return () => _vars[varName]();
        }

        /// <summary>
        /// Get or set value for variables in this equation set
        /// </summary>
        /// <param name="varName">Name of the target variable</param>
        /// <returns>Value of the variable</returns>
        public Func<double> this[string varName] {
            get => this.RemoteGetter(varName);
            set {
                if (!_vars.ContainsKey(varName))
                    _vars.Add(varName, value);
                else
                    _vars[varName] = value;
            }
        }

        /// <summary>
        /// Add equations into this equation set
        /// </summary>
        /// <param name="equations">A set of equations</param>
        public void AddEquations(ICollection<Func<EquationSet, Func<double>>> equations) {
            if (equations == null) throw new ArgumentNullException(nameof(equations));
            if (!equations.Any()) return;
            foreach (var eq in equations) {
                this.AddEquation(eq(this));
            }
        }

        /// <summary>
        /// Generate a multi-thread Simulated Annealing Algorithm (SAA) solver for an equation set
        /// </summary>
        /// <param name="vars">Name of variables</param>
        /// <param name="equations">Equations (functions that need to reach zero value)</param>
        /// <param name="range">Lower and Upper bound for each variables</param>
        /// <param name="n">Number of threads in SAA</param>
        /// <param name="temperature">Initial temperature in SAA</param>
        /// <param name="iters">Total iterations in SAA</param>
        /// <param name="cooliter">Cooling cycle in SAA</param>
        /// <param name="debug">Debug output in SAA</param>
        /// <returns>A SAA solver of the equation set</returns>
        public static SimulatedAnnealing<EquationSet> SaaSolver(
            string[] vars, Func<EquationSet, Func<double>>[] equations,
            IReadOnlyCollection<(double lower, double upper)> range,
            int n = 21, double temperature = 200_00, int iters = 500_000, int cooliter = 200, bool debug = false) {
            var dummys = new EquationSet[n];
            for (var i = 0; i < n; i++) {
                var eqs = new EquationSet("eqs1", vars);
                eqs.AddEquations(equations);
                dummys[i] = eqs;
            }
            Func<EquationSet, double> objFunc = (eqs) => eqs.ObjectiveFunc()();
            return new SimulatedAnnealing<EquationSet>(dummys, vars, range, objFunc, temperature,
                                                       iters, cooliter, debug);
        }
    }
}