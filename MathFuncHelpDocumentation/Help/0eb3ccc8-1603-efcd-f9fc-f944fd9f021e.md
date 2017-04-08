# DeferredExchangeOption Constructor 
 

Initial instance of <a href="038a4c75-6c25-a94c-6f1b-ae25c13c8b1f">DeferredExchangeOption</a> from two <a href="1df39166-cdbc-ea41-0f5d-56de5e09158b">Stock</a>s. Use Margrabe Formula to price.

**Namespace:**&nbsp;<a href="d9e4b2f9-9258-2f31-ca55-43e6b838bbc3">MathFuncConsole.MathObjects.Applications</a><br />**Assembly:**&nbsp;MathFuncConsole (in MathFuncConsole.exe) Version: 0.0.0.1 (0.0.0.1)

## Syntax

**C#**<br />
``` C#
public DeferredExchangeOption(
	string name,
	Stock s1,
	Stock s2,
	Object rho,
	Object optionMaturity,
	Object exchangeMaturity
)
```

**VB**<br />
``` VB
Public Sub New ( 
	name As String,
	s1 As Stock,
	s2 As Stock,
	rho As Object,
	optionMaturity As Object,
	exchangeMaturity As Object
)
```

**C++**<br />
``` C++
public:
DeferredExchangeOption(
	String^ name, 
	Stock^ s1, 
	Stock^ s2, 
	Object^ rho, 
	Object^ optionMaturity, 
	Object^ exchangeMaturity
)
```

**F#**<br />
``` F#
new : 
        name : string * 
        s1 : Stock * 
        s2 : Stock * 
        rho : Object * 
        optionMaturity : Object * 
        exchangeMaturity : Object -> DeferredExchangeOption
```

<br />

#### Parameters
&nbsp;<dl><dt>name</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/s1wwdcbf" target="_blank">System.String</a><br />Name of the deferred exchange option</dd><dt>s1</dt><dd>Type: <a href="1df39166-cdbc-ea41-0f5d-56de5e09158b">MathFuncConsole.MathObjects.Applications.Stock</a><br />Asset to be received</dd><dt>s2</dt><dd>Type: <a href="1df39166-cdbc-ea41-0f5d-56de5e09158b">MathFuncConsole.MathObjects.Applications.Stock</a><br />Asset to be delivered</dd><dt>rho</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">System.Object</a><br />Correlation between two assets</dd><dt>optionMaturity</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">System.Object</a><br />Time to maturity of option</dd><dt>exchangeMaturity</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/e5kfa45b" target="_blank">System.Object</a><br />Time until exchange >= TOption</dd></dl>

## See Also


#### Reference
<a href="038a4c75-6c25-a94c-6f1b-ae25c13c8b1f">DeferredExchangeOption Class</a><br /><a href="d9e4b2f9-9258-2f31-ca55-43e6b838bbc3">MathFuncConsole.MathObjects.Applications Namespace</a><br />