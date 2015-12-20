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

### Keywords

The following are reserved keywords, and may only be used as variable names if prefixed with `@`.

- `break`
- `case`
- `const`
- `continue`
- `default`
- `else`
- `false`
- `func`
- `if`
- `in`
- `loop`
- `model`
- `null`
- `return`
- `switch`
- `true`
- `var`

The following type keywords are available only for the purpose of numeric casting and as parameter type declarations:

- `bool` (parameter declaration only)
- `byte`
- `char`
- `double`
- `int`
- `long`
- `sbyte`
- `short`
- `string` (parameter declaration only)
- `uint`
- `ulong`
- `ushort`

Any line in a Jadeite template which begins with one of the above keywords is automatically interpreted as unbuffered code, and therefore does not need to start with a `-`.

### Local Variables

Local variables can be created for any primitive type (boolean, numeric, string, character). For convenience, local variable can also be created to hold non-primitive types, but only existing instances of these types (contained within `model`) may be assigned. New instances of non-primitives or arrays cannot be created.

Use the `var` keyword to create local variables.

```csharp
var x = 0
```

```csharp
var y = model.SomeProperty
```

A variable's type cannot be changed after it has been created.

```csharp
var x = 1
x = 1.2 // this is an error because x was declared as an integer
```

Local variables have lexical scoping.

### Primitive Types

//

### Operators

Here are the operators supported by Jadeite code, grouped by order of precedence.

### Primary

Operator | Types    | Meaning
---------|----------|--------
`x.y`    | objects  | Member access.
`x[y]`   | arrays   | Array or list access.
`x++`    | numerics | Postfix increment. Returns the value of x, then increments by 1.
`x--`    | numerics | Postfix decrement. Returns the value of x, then decrements by 1.

#### Unary

Operator | Types    | Meaning
---------|----------|--------
`+x`     | numerics | Returns value of x.
`-x`     | numerics | Numeric negation.
`!x`     | booleans | Logical negation.
`~x`     | integers | Bitwise complement (reverses each bit).
`++x`    | numerics | Prefix increment. Increments x by 1, and returns its new value.
`--x`    | numerics | Prefix decrement. Decrements x by 1, and returns its new value.
`(T)x`   | numerics | Casts x as type T. Only numeric casts are allowed (i.e. casting a double as an int).

#### Multiplicative

Operator | Types    | Meaning
---------|----------|--------
`x * y`  | numerics | Multiply x and y.
`x / y`  | numerics | Divide x by y.
`x % y`  | numerics | Modulus. Return remainder of x divided by y.

#### Additive

Operator | Types             | Meaning
---------|-------------------|--------
`x + y`  | numerics, strings | Add x and y. For strings, `+` is the concatenation operator.
`x - y`  | numerics          | Subtract y from x.

#### Shift

Operator | Types    | Meaning
---------|----------|--------
`x << y` | integers | Shift y bits left with zero-fill on right.
`x >> y` | integers | Shift y bits right. If x is a signed integer, the left fill will be the sign. Unsigned integers will be zero-filled on the left.

#### Relational and Type-testing

Operator | Types    | Meaning
---------|----------|--------
`x < y`  | numerics | True if x is less than y.
`x > y`  | numerics | True if x is greater than y.
`x <= y` | numerics | True is x is less than or equal to y.
`x >= y` | numerics | True if x is greater than or equal to y.

#### Equality

Operator | Types    | Meaning
---------|----------|--------
`x == y` | all      | True if the value of x is equal to the value of y. For object types, reference equality is used.
`x != y` | all      | True if the value of x is not equal to the value of y. For object types, reference equality is used.

#### Logical/Bitwise AND

Operator | Types              | Meaning
---------|--------------------|--------
`x & y`  | integers, booleans | Bitwise AND for integers. Logical AND for booleans.

#### Logical/Bitwise XOR

Operator | Types              | Meaning
---------|--------------------|--------
`x ^ y`  | integers, booleans | Bitwise XOR for integers. Logical XOR for booleans.

#### Logical/Bitwise OR

Operator | Types              | Meaning
---------|--------------------|--------
`x | y`  | integers, booleans | Bitwise OR for integers. Logical OR for booleans.

#### Conditional AND

Operator | Types    | Meaning
---------|----------|--------
`x && y` | booleans | Logical AND. If x evaluates to false, y will not be evaluated.

#### Conditional OR

Operator | Types    | Meaning
---------|----------|--------
`x || y` | booleans | Logical OR. If x evaluates to true, y will not be evaluated.

### Null-coalescing

Operator | Types    | Meaning
---------|----------|--------
`x ?? y` | objects  | Returns x if it is non-null, otherwise y.

#### Conditional Expression (Ternary)

Operator    | Types                   | Meaning
------------|-------------------------|--------
`x ? y : z` | all (x must be boolean) | Returns y when x is true, otherwise returns z.

#### Assignment

Operator  | Types              | Meaning
----------|--------------------|--------
`x = y`   | all                | Assignment
`x += y`  | numerics           | Increment x by the value of y.
`x -= y`  | numerics           | Subtract the value of y from x and store the result in x.
`x *= y`  | numerics           | Multiply x and y and store the result in x.
`x /= y`  | numerics           | Divide x by y and store the result in x.
`x %= y`  | numerics           | Divide x by y and store the remainder in x.
`x &= y`  | integers, booleans | AND x and y and store the result in x. Logical for booleans, bitwise for integers.
`x |= y`  | integers, booleans | OR x and y and store the result in x. Logical for booleans, bitwise for integers.
`x ^= y`  | integers, booleans | XOR x and y and store the result in x. Logical for booleans, bitwise for integers.
`x <<= y` | integers           | Shift x by y bits left and store the result in x (using same rules as shift opertors).
`x >>= y` | integers           | Shift x by y bits right and store the result in x (using same rules as shift opertors).

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

func (int, bool) SomeHelper(int i)
	return i, true

var x, var y = SomeHelper(1)