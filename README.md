# MathFuncConsole
Building wheels for quantitative finance courses. Hoping to make it easier to use and elegant in logic. Welcome to fork and contribute. Please refer to this md file to get to know how I think and how I implement those features.

## Intro
This project is written in C# 7 in Visual Studio 2017 without any 3rd-party package. It is a wheel project for practicing and learning C# and its quantitative finance applications. 

### Features 
- Implementations of financial concepts
- Implementations of pricing and properties using financial theorems
- Binding variables between math objects
- Reflection of property list
- Custom attributes 
- More coming soon

### How it works

In order to calculate a value, we need to feed one or more parameters into function. While after calculation, relationships between the result and inputs will disappear (i.e. if we change a parameter and then quote to the result, you will not get any undated result). This is not convenient especially when you have a complex relationships inside your model (i.e. in an environment with an option based on a yield bond that priced by another yield model, we want to see Greeks of the option to change by changing yield). So that we have to use some types of data structures and architectures to save those relationships and update related values when an independent variable change.

I come up with the idea of wrapping every variables (including independent variables and dependent variables) by a Func<double> instead of double so that I can update its value by calling them. Here is a simple demo:


```C#
    Func<double> inner = () => 0.3D; // always return 0.3 => an independent variable with value of 0.3
    Func<double> outer = () => inner(); // this is a reference of the varible

    var b1 = new Bond("bond1", face: 100, ytm: outer, T: 1, t0: 0); // ignore what is it for a second, just look at the quote of outer
    Console.WriteLine(b1.Ytm()); // 0.30 => reflect the value of outer which is to seek for value of inner 

    inner = () => 0.2D; // if we update inner with a new value,
    Console.WriteLine(b1.Ytm()); // 0.20 => calling outer to get the new quote of inner 
```

This demo can provide us a dynamic relationship between a yield and a bond and we will use this type of binding almost everywhere in the project. But there is a potential risk here: if we modified inner or outer in a wrong way, then we may have unpredictable results. To fix this, I wrap everything into an abstract class called MathObject (Bond is a sub-class of MathObject). If we wrap yield into another MathObject, we will have this:

```C#
    var y = new Yield("yield2", ytm: 0.3D); // .ctor will do all wrapping for us

    var b2 = new Bond("bond2", face: 100, ytm: y.Ytm, T: 1, t0: 0); // quoting on ytm in yield
    Console.WriteLine(b2.Ytm()); // 0.30

    y.Ytm = 0.2D.Wrap(); // changing value of ytm, I will talk about Wrap() in a second
    Console.WriteLine(b2.Ytm()); // 0.20
```

This demo looks better but what is the mechanism behind? Let's have a look into the MathObject class:

```C#
    public abstract class MathObject {
        public string Name { get; } 
        public MathObject(string name) {}
        public override string ToString() {}
        protected static Func<double> Input(object obj, double? def = null) {}
    }
    
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
```
Property “Ytm” is the how I wrap a variable safely: I save its real value into _ytm and update _ytm when I set new value into Ytm and return a new reference of _ytm (works as outer in the first demo) to avoid mistaken modification. Note that Ytm is a property (that’s why its name start with a capital letter) and not a method. Quoting Ytm() is different from quoting Ytm.

```C#
    var y = new Yield("yield", ytm: 0.3D);
    
    Func<double> yRef = y.Ytm;   // returns the quote on Ytm, can be used to build up new connections
    double       yVal = y.Ytm(); // returns the latest numerical value of Ytm
```
Also note that in the .ctor of Yield class, all parameters are nullable object type except for its name. I use nullable object to create more flexibility and convenience for users in the following two way: first, you can pass in either a Func<double> or a double into the function, if a double is feed in, MathObject.Input() is eager to wrap it automatically; second, you can pass in null to let .ctor initialize its property by default value (also in MathObject.Input()). As a tradeoff, it abandons static type validation so that you should be aware of the type of your inputs. Here are some examples of valid and invalid inputs:

```C#
    var y1 = new Yield("yield1", 0.2D); // valid
    var y2 = new Yield("yield2", 1); // valid, int can implicitly cast to double
    var y3 = new Yield("yield3"); // valid, use default value
    var y4 = new Yield("yield4", y1.Ytm); // valid, use a reference of another variable
    var y5 = new Yield("yield5", y1.Ytm()); // valid, use a static value of another variable, won't update with y1
    var y6 = new Yield("yield6", y1); // invalid, y1 is neither a double or a Func<double>
```

If you want to create your implementations of other objects, please use Yield and Bond class as templates where I have wrote a lot of notations and comments. In default .ctor of Bond class, price and duration are automatically calculated from input variables, but because of the flexibility I made, you can create a bond with certain price and duration without giving it a yield. Note that if you do that, the Ytm of this bond will be meaningless. I will write a helper class later on to make Ytm meaningful again by calculating IRR from price and cash flows, so for now, just ignore it because you won’t need it in most cases. Here is an example:

```C#
    var bond1 = new Bond("bond1", face: 100) {P = 90.Wrap(), Dm = 1.7.Wrap(), Cov = 60.Wrap()};
    
    var bond2 = new Bond("bond2", face: 100);
    bond2.P = 90.Wrap();
    bond2.Dm = 1.7.Wrap();
    bond2.Cov = 60.Wrap();
```

Using above information is enough to do your duration-convexity hedge problems. bond1 and bond2 are identical, you can use either syntex. Note that if you want to change any of the properties outside the .ctor, you cannot pass in anything besides Func<double>. Using extend method .Wrap() for double and int is a shortcut to wrap a static number for you to use in object initializer.

A good tool for you to have a clear view of a MathObject is to use its ToString() method. I override it to print out all property that the MathObject has now and their current numerical value. I made it by dynamic reflection, so that you don't need to create unique ToString() for your sub-classes, but to make it works better, you are strongly recommended to add NameAttribute to your properties as what I did for Bond and Yield class. The Name attribute is only for print out, so no need to take care of its syntex, in general, it can be one or more letters/greeks, UTF-8 are supported by VS.

```C#
    [Name("β")] // add Name attrubite is strongly recommended, UTF-8 is supported
    public Func<double> Beta {
        get { return () => _beta(); }
        set { _beta = value; }
    }
```
