Although Jade supports arbitrary host-language (JavaScript) code in templates, Jadeite is only going to allow more primitive operations using a host-independent language.

Stuff that Jadeite code can do:

- local variable creation
- read and assign local variables
- read fields and properties of "model" (no assignment and no method calls)
- basic operators like arithmetic and string concatenation
- loops (for, foreach, while)
- if/else
- switch/case
- create and call helper functions (supports multiple return values)
- call a limited set of built-in functions (i.e. math functions)
- no exceptions can be explicitly thrown or caught

### Local Variables

Local variables can be created for any primitive type (boolean, numeric, string, character). For convenience, local variable can also be created to hold non-primitive types, but only existing instances of these types (contained within `model`) may be assigned. New instances of non-primitives or arrays cannot be created.

Use the `var` keyword to create local variables.

```csharp
var x = 0
```

```csharp
var y = model.SomeProperty
```

### Operators

Here are the operators supported by Jadeite code, grouped by order of precedence.

### Primary

`x.y` Member access.
`x[y]` Array or list access.
`x++` Postfix increment. Returns the value of x, then increments by 1.
`x--` Postfix decrement. Returns the value of x, then decrements by 1.

#### Logical Operators

//

### Numeric Operators

The typical numeric operators are available.

Operator | Meaning
---------|--------
+        | Add
-        | Subract
*        | Multiply
/        | Divide


### String Operators

//

### Model

//

### Loops

//

### Conditionals

//

### Switch

//

### Helper Functions

//

### Built-in Helpers

//
