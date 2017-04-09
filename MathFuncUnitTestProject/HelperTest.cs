using System;
using System.Diagnostics;
using System.Linq;
using MathFuncConsole.MathObjects.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathFuncUnitTestProject {
    [TestClass]
    public class HelperTest {
        [TestMethod]
        public void TestNormDistSample() {
            Console.WriteLine("Testing norm dist random samples");
            var num = 10000000;
            var nm = new double[num];
            for (var s = 0; s < num; s++) {
                nm[s] = NormalDist.NextSample();
            }
            var avg = nm.Average();
            var kurt = 0D;
            var skew = 0D;
            var vari = 0D;
            for (var s = 0; s < num; s++) {
                var e = (nm[s] - avg);
                vari += Math.Pow(e, 2);
                skew += Math.Pow(e, 3);
                kurt += Math.Pow(e, 4);
            }
            vari /= (num - 1);
            var std = Math.Sqrt(vari);
            skew /= (num - 1) * Math.Pow(std, 3);
            kurt /= (num - 1) * Math.Pow(std, 4);

            Console.WriteLine($"{avg:F5}|{vari:F5}|{skew:F5}|{kurt:F5}");

            Assert.IsTrue(avg.WithinTolerance(0, 1E-2));
            Assert.IsTrue(vari.WithinTolerance(1, 1E-2));
            Assert.IsTrue(skew.WithinTolerance(0, 1E-2));
            Assert.IsTrue(kurt.WithinTolerance(3, 1E-2));
        }
    }
}
