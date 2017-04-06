using System;
using MathFuncConsole.MathObjects;
using MathFuncConsole.MathObjects.Applications;

namespace MathFuncConsole {
    internal static class Program {
        private static void Main(string[] args) {
            Console.Title = "Quantitative Finance Pricing Console";

            var s1 = new Stock("s1", price: 100, sigma: 0.2, divd: 0.03);
            var s2 = new Stock("s2", price: 120, sigma: 0.3, divd: 0.02);
            var eo = new ExchangeOption("eo", s1, s2, rho: 0.5, maturity: 1);
            var deo = new DeferredExchangeOption("deo", s1, s2, rho: 0.5, optionMaturity: 1, exchangeMaturity: 2);
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

            var rhoSetter = eo.RemoteSetter(nameof(eo.Rho));
            rhoSetter(0.1);
            Console.WriteLine(eo);
            Console.WriteLine();

            var go1 = new GenericOption("go1", pv1: 100, pv2: 120, maturity: 1, sigma: 0.2);
            var go2 = new GenericOption("go2", pv1: go1.Pv1, pv2: go1.Pv2, maturity: go1.Maturity, price: go1.Price);
            Console.WriteLine(go1);
            Console.WriteLine(go2);

            go1.Sigma = 0.3.Wrap();
            go1.Pv2 = 100.Wrap();
            Console.WriteLine(go1);
            Console.WriteLine(go2);



            Console.Read();
        }
    }
}
