# EuropeanCallOption Class
 


## Inheritance Hierarchy
<a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">System.Object</a><br />&nbsp;&nbsp;<a href="bce605e3-e729-258a-0e66-9bfb6e48c607">MathFuncConsole.MathObjects.MathObject</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="03a5ddd0-9c60-07f4-42e6-a2afd39c4365">MathFuncConsole.MathObjects.Applications.Option</a><br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MathFuncConsole.MathObjects.Applications.EuropeanCallOption<br />
**Namespace:**&nbsp;<a href="d9e4b2f9-9258-2f31-ca55-43e6b838bbc3">MathFuncConsole.MathObjects.Applications</a><br />**Assembly:**&nbsp;MathFuncConsole (in MathFuncConsole.exe) Version: 0.0.0.1 (0.0.0.1)

## Syntax

**C#**<br />
``` C#
internal class EuropeanCallOption : Option
```

**VB**<br />
``` VB
Friend Class EuropeanCallOption
	Inherits Option
```

**C++**<br />
``` C++
internal ref class EuropeanCallOption : public Option
```

**F#**<br />
``` F#
type EuropeanCallOption =  
    class
        inherit Option
    end
```

<br />
The EuropeanCallOption type exposes the following members.


## Constructors
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="27187f5e-f82b-186a-66d1-86b24905f555">EuropeanCallOption(String, Stock, Object, Object, Object)</a></td><td>
Initializes a new instance of the EuropeanCallOption class</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="67273824-7061-66a1-204f-6314e6fc6ba0">EuropeanCallOption(String, Object, Object, Object, Object, Object, Object)</a></td><td>
Initializes a new instance of the EuropeanCallOption class</td></tr></table>&nbsp;
<a href="#europeancalloption-class">Back to Top</a>

## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="df88782c-d691-8fbf-cc0e-b85e10a1a003">Maturity</a></td><td>
Maturity of the option
 (Inherited from <a href="03a5ddd0-9c60-07f4-42e6-a2afd39c4365">Option</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="7c6d74af-467f-3271-ca40-a41261eb9865">Name</a></td><td>
Name of this <a href="bce605e3-e729-258a-0e66-9bfb6e48c607">MathObject</a>, only used in print-out.
 (Inherited from <a href="bce605e3-e729-258a-0e66-9bfb6e48c607">MathObject</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="e192ad57-648b-2d2c-24e2-cfd4f717d85d">Price</a></td><td>
Price of the option. No default pricing in <a href="03a5ddd0-9c60-07f4-42e6-a2afd39c4365">Option</a>.
 (Inherited from <a href="03a5ddd0-9c60-07f4-42e6-a2afd39c4365">Option</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="5f4f434e-5257-936e-d08d-6ed5fe869a1b">RiskFree</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="51098b63-2721-4c52-59d6-de4693a24f0a">Sigma</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="d019d928-002b-7820-01a3-94eadf70f4fb">StockDivd</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="f9fd50ec-8868-737d-ea44-5d091d73fd91">StockPrice</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="3bc63f0f-622a-fadc-384a-0f8095554c15">Strike</a></td><td /></tr></table>&nbsp;
<a href="#europeancalloption-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="090965dd-a373-63b1-08c1-bc38738ea2ad">RemoteGetter</a></td><td>
Universal getter method without reference to the instance. Please use direct getter as much as possible. Remote setter is designed for generic computation methods.
 (Inherited from <a href="bce605e3-e729-258a-0e66-9bfb6e48c607">MathObject</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="f27fefd1-791f-1bb1-3e1e-c8d33f566812">RemoteLink</a></td><td>
Universal abstraction for inner relationship between two properties in this instance. This method build up a function that can help you get a new value of the dependent variable when you change the value of the independent variable. Input value should always be <a href="http://msdn2.microsoft.com/en-us/library/643eft0t" target="_blank">Double</a> and return types is also <a href="http://msdn2.microsoft.com/en-us/library/643eft0t" target="_blank">Double</a>. Remote link is designed for generic computation methods.
 (Inherited from <a href="bce605e3-e729-258a-0e66-9bfb6e48c607">MathObject</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="9549b6b3-3f98-bc2d-f2fe-10355f8d5464">RemoteSetter</a></td><td>
Universal setter method without reference to the instance. To set value, input must be <a href="http://msdn2.microsoft.com/en-us/library/643eft0t" target="_blank">Double</a>. Please use direct setters as much as possible. Remote setter is designed for generic computation methods.
 (Inherited from <a href="bce605e3-e729-258a-0e66-9bfb6e48c607">MathObject</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="da420a04-08e1-4ec7-b29f-b8446ae7263d">ToString</a></td><td>
Returns a string that represents the current object.
 (Inherited from <a href="bce605e3-e729-258a-0e66-9bfb6e48c607">MathObject</a>.)</td></tr></table>&nbsp;
<a href="#europeancalloption-class">Back to Top</a>

## Fields
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Private field](media/privfield.gif "Private field")</td><td><a href="0301d4d2-1cc6-0150-206d-11537f7cc5f4">_maturity</a></td><td> (Inherited from <a href="03a5ddd0-9c60-07f4-42e6-a2afd39c4365">Option</a>.)</td></tr><tr><td>![Private field](media/privfield.gif "Private field")</td><td><a href="1b45e884-d560-ce8b-cd48-4580a467c2a0">_price</a></td><td> (Inherited from <a href="03a5ddd0-9c60-07f4-42e6-a2afd39c4365">Option</a>.)</td></tr><tr><td>![Private field](media/privfield.gif "Private field")</td><td><a href="69f0501d-6636-b336-8bd2-fa0c542b7d50">_riskFree</a></td><td /></tr><tr><td>![Private field](media/privfield.gif "Private field")</td><td><a href="cf781bf9-d56c-6126-ee5a-2bc1f18dbd5a">_sigma</a></td><td /></tr><tr><td>![Private field](media/privfield.gif "Private field")</td><td><a href="d54a421c-ed2e-68d5-19ac-1fd5f85cf4dd">_stockDivd</a></td><td /></tr><tr><td>![Private field](media/privfield.gif "Private field")</td><td><a href="ef257c5d-a98e-6dff-30ea-95e778de76b1">_stockPrice</a></td><td /></tr><tr><td>![Private field](media/privfield.gif "Private field")</td><td><a href="94e19419-163f-edd3-73f8-62d37b25dbf4">_strike</a></td><td /></tr></table>&nbsp;
<a href="#europeancalloption-class">Back to Top</a>

## Extension Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="718ec2ab-e890-7d30-f161-f5a9ecf2f0b3">To(T)</a></td><td>
Shortcut to cast a obj from one type to another. Be careful of if these types can cast directly.
 (Defined by <a href="f8375fff-6215-8a0d-083f-b42a5658e465">MathClassHelper</a>.)</td></tr></table>&nbsp;
<a href="#europeancalloption-class">Back to Top</a>

## See Also


#### Reference
<a href="d9e4b2f9-9258-2f31-ca55-43e6b838bbc3">MathFuncConsole.MathObjects.Applications Namespace</a><br />