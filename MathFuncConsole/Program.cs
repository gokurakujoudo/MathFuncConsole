using System;
using MathFuncConsole.MathObjects;
using MathFuncConsole.MathObjects.Applications;

namespace MathFuncConsole {
    class Program {
        static void Main(string[] args) {
            Console.Title = "Quantitative Finance Pricing Console";

            Func<double> inner = () => 0.3D;
            Func<double> outer = () => inner();

            var b1 = new Bond("bond1", face: 100, ytm: outer, T: 1, t0: 0);
            Console.WriteLine(b1.Ytm()); // 0.30

            inner = () => 0.2D;
            Console.WriteLine(b1.Ytm()); // 0.20


            var y = new Yield("yield2", ytm: 0.3D);

            var b2 = new Bond("bond2", face: 100, ytm: y.Ytm, T: 1, t0: 0);
            Console.WriteLine(b2.Ytm()); // 0.30

            y.Ytm = 0.2D.Wrap();
            Console.WriteLine(b2.Ytm()); // 0.20



            Console.WriteLine(b2.ToString());


            inner = 1.Wrap();

            Console.WriteLine(b2.ToString());


            var y1 = new Yield("yield1", 0.2D); // valid
            var y2 = new Yield("yield2", 1); // valid, int can implicitly cast to double
            var y3 = new Yield("yield3"); // valid, use default value
            var y4 = new Yield("yield4", y1.Ytm); // valid, use a reference of another variable
            var y5 =
                new Yield("yield5", y1.Ytm()); // valid, use a static value of another variable, won't update with y1
            //var y6 = new Yield("yield6", y1); // invalid, y1 is neither a double or a Func<double> recommended supported


            var bond2 = new Bond("bond2", face: 100);
            bond2.P = 90.Wrap();
            bond2.Dm = 1.7.Wrap();
            bond2.Cov = 60.Wrap();

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

            var go1 = new GenericOption("go1", 100, 120, 1, 0.2);
            var go2 = new GenericOption("go2", go1.Pv1, go1.Pv2, go1.Maturity, price: go1.Price);
            Console.WriteLine(go1);
            Console.WriteLine(go2);

            go1.Pv2 = 100.Wrap();
            Console.WriteLine(go1);
            Console.WriteLine(go2);

            Console.Read();
        }
    }
}
