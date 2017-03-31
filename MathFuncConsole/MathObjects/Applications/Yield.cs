using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathFuncConsole.MathObjects.Applications {
    /// <summary>
    /// A wrap up for yield, also provide its multiple casting between types of yield.
    /// </summary>
    public class Yield : MathObject {

        private Func<double> _ytm;

        public Yield(string name, object ytm = null) : base(name) {
            this.Ytm = Input(ytm, .05D);
        }

        [Name("ytm")]
        public Func<double> Ytm {
            get { return () => _ytm(); }
            set { _ytm = value; }
        }
    }
}
