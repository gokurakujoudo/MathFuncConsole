using System;

namespace MathFuncConsole.MathObjects.Applications {
    /// <summary>
    /// This is a demo for an arbitrary bond with/without coupons. Basic property of the bond (price, duration, convexity) are calculated automatically.
    /// </summary>
    public class Bond : MathObject {
        #region vars

        private Func<double> _yieldToMaturity;
        private Func<double> _faceValue;
        private Func<double> _maturity;
        private Func<double> _couponRate;
        private Func<double> _payTimesPerYear;
        private Func<double> _now;
        private Func<double> _couponPayment;
        private Func<double> _price;
        private Func<double> _duration;
        private Func<double> _modifyDuration;
        private Func<double> _convexity;

        #endregion

        /// <summary>
        /// Initial a new <see cref="Bond"/> with following parameters, could be either static numbers or reference from other variables. 
        /// I use <see cref="object"/> as input to allow you to pass in a number(<see cref="double"/>/<see cref="int"/>) or 
        /// a <see cref="Func{TResult}"/>(whose generic type argument is <see cref="double"/>, as you will get from any other MathObject) 
        /// or null(use default value). Types other than these three will raise an <see cref="ArgumentException"/>. You can change any of
        /// these property later on as you do in any other place of C# but be sure to wrap any static number.
        /// <para>To help you with making your class/.ctor correctly, please read and understand this .ctor</para>
        /// </summary>
        /// <param name="name">This is only a name for the bond, no real meaning</param>
        /// <param name="face">Face value of the bond, by default it is 100</param>
        /// <param name="ytm">Yield To Maturity of this bond, not annualized, by default is 0</param>
        /// <param name="T">Maturity in terms of number of years, by default is 1</param>
        /// <param name="now">Time point of pricing, in term of years, by default is 0(now)</param>
        /// <param name="coupon">Coupon Rate per year, by default is 0(discount bond)</param>
        /// <param name="paytimes">Coupon frequency in terms of times per year, by default is 1</param>
        public Bond(string name, object face = null, object ytm = null, object T = null, object now = null,
                    object coupon = null,
                    object paytimes = null) {

            this.Name = name;

            this.F = Input(face, 100);
            this.Y = Input(ytm, 0);
            this.T = Input(T, 1);
            this.Now = Input(now, 0);
            this.C = Input(coupon, 0);
            this.M = Input(paytimes, 1);

            this.Cp = () => this.C() * this.F() / this.M();
            this.P = () => {
                var times = (this.T() - this.Now()) * this.M();
                var pv = 0D;
                if (this.Cp() > 0)
                    for (var i = 1; i <= times; i++)
                        pv += this.Cp() / Math.Pow(1 + this.Y(), i);
                pv += this.F() / Math.Pow(1 + this.Y(), times);
                return pv;
            };
            this.D = () => {
                var d = 0D;
                var times = (this.T() - this.Now()) * this.M();
                if (this.Cp() > 0)
                    for (var i = 1; i <= times; i++)
                        d += this.Cp() / Math.Pow(1 + this.Y(), i) * i / this.M();
                d += this.F() / Math.Pow(1 + this.Y(), times) * (this.T() - this.Now());
                return d / this.P();
            };
            this.Dm = () => this.D() / (1 + this.Y());
            this.Cov = () => {
                var cov = 0D;
                var times = (this.T() - this.Now()) *
                            this.M();
                if (this.Cp() > 0)
                    for (var i = 1; i <= times; i++)
                        cov += this.Cp() * i * (i + 1) / Math.Pow(1 + this.Y(), i);
                cov += this.F() * times * (times + 1) /
                       Math.Pow(1 + this.Y(), times);
                return cov / (this.P() * Math.Pow(1 + this.Y(), 2));
            };
        }


        #region property
        /// <summary>
        /// >Face value of the bond
        /// </summary>
        public Func<double> F {
            get { return () => _faceValue(); }
            set { _faceValue = value; }
        }
        /// <summary>
        /// Maturity in terms of number of years
        /// </summary>
        public Func<double> T {
            get { return () => _maturity(); }
            set { _maturity = value; }
        }
        /// <summary>
        /// Yield To Maturity of this bond, not annualized
        /// </summary>
        public Func<double> Y {
            get { return () => _yieldToMaturity(); }
            set { _yieldToMaturity = value; }
        }
        /// <summary>
        /// Coupon Rate per year
        /// </summary>
        public Func<double> C {
            get { return () => _couponRate(); }
            set { _couponRate = value; }
        }
        /// <summary>
        /// Coupon frequency in terms of times per year
        /// </summary>
        public Func<double> M {
            get { return () => _payTimesPerYear(); }
            set { _payTimesPerYear = value; }
        }
        /// <summary>
        /// Time point of pricing, in term of years
        /// </summary>
        public Func<double> Now {
            get { return () => _now(); }
            set { _now = value; }
        }

        /// <summary>
        /// Coupon payment each term
        /// </summary>
        public Func<double> Cp {
            get { return () => _couponPayment(); }
            set { _couponPayment = value; }
        }
        /// <summary>
        /// Bond price at the time of <see cref="Now"/>
        /// </summary>
        public Func<double> P {
            get { return () => _price(); }
            set { _price = value; }
        }
        /// <summary>
        /// Duration of the Bond
        /// </summary>
        public Func<double> D {
            get { return () => _duration(); }
            set { _duration = value; }
        }
        /// <summary>
        /// Modified duration of the bond
        /// </summary>
        public Func<double> Dm {
            get { return () => _modifyDuration(); }
            set { _modifyDuration = value; }
        }
        /// <summary>
        /// Convexity of the bond
        /// </summary>
        public Func<double> Cov {
            get { return () => _convexity(); }
            set { _convexity = value; }
        }

        #endregion
    }
}
