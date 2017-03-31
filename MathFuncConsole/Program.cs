using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathFuncConsole.MathObjects;
using MathFuncConsole.MathObjects.Applications;

namespace MathFuncConsole
{
    class Program
    {
        static void Main(string[] args) {
            var b1 = new Bond("bond1", 100, 0.05, 1, 0);
            Console.WriteLine(b1.ToString());

            b1.F = () => 200;
            

            var b2 = new Bond("bond2", b1.F, b1.Y, b1.T, b1.Now);

            var t = b2;

            Console.WriteLine(b2.ToString());

            b1.Y = () => 0.1;


            Console.WriteLine(b1.ToString());

            Console.Read();
        }
    }
}
