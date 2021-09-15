# Metrology
Metrology is a .NET Library intended to provide functionality that allows a user to
associate units of measure with a quantity and perform mathematic operations and conversions
without having to manually track units along the way.

## Basics
The primary type that users will interact with is the `Quantity`. `Quantity` is an immutable class that 
represents the combination of units of measures (referred to as a _unit vector_, e.g. `m/s^2`) and a scalar
amount. Together, it represents a meaningful amount of a physical quantity and the units thereof.

### Creating a `Quantity`
```csharp
// In C#

// Using a string literal
Quantity lGravity = "9.81 m/s^2";

// Using a constructor
Quantity lGravity = new Quantity( 9.81m, "m/s^2" );
```

```vb
' In Visual Basic

' Using a string literal
Dim lGravity As Quantity = "9.81 m/s^2"

' Using a constructor
Dim lGravity As Quantity = New Quantity( 9.81D, "m/s^2" )
```

Using any of the example methods above, one can create a `Quantity` object with any combination and
exponent of supported units.

#### Prefixes
Metrology also supports scaling prefixes such as `Kilo` and `Mega`. Simply include them in the
unit vector when declaring Quantities.

```csharp
// C#
Quantity lKilograms = "1 kg";
Quantity lGrams = "1 g";
Quantity lMillimeters = "1 mm";
Quantity lKilogramPerMillimeter = "1 kg/mm";
```
```vb
' Visual Basic
Dim lKilograms As Quantity = "1 kg"
Dim lGrams As Quantity = "1 g"
Dim lMillimeters As Quantity = "1 mm"
Dim lKilogramPerMillimeter As Quantity = "1 kg/mm"
```

### Using a `Quantity`
Quantities implement all the standard arithmetic operators (add, subtract, multiply, divide) in conjunction
with other Quantities. When these operations are performed, unit checking, manipulation and unit conversion are 
handled automatically.

It's important to know that all operations performed on a Quantity (e.g. addition, subtraction, conversion) do
_not_ modify the existing Quantity. These operations produce a new Quantity that represents the result of the
operation.

#### Multiplication and Division
Any Quantity can be multiplied or divided by any other Quantity. When this is done, units are automatically
handled and adjusted.

```csharp
// C#
// Acceleration
Quantity lGravity = "9.81 m/s^2";

// Time
Quantity lTime = "5 s";

// When executed, will be 49.05 m/s
Quantity lVelocity = lGravity * lTime;

// When executed, will be 9.81 m/s^2
Quantity lOriginalAcceleration = lVelocity / lTime;
```
```vb
' Visual Basic
' Acceleration
Dim lGravity As Quantity = "9.81 m/s^2"

' Time
Dim lTime As Quantity = "5 s"

' When executed, will be 49.05 m/s
Dim lVelocity As Quantity = lGravity * lTime

' When executed, will be 9.81 m/s^2
Dim lOriginalAcceleration As Quantity = lVelocity / lTime
```
In the above example, when a value representing acceleration (`length/time^2`) is multiplied by time (`time`), the
calculated value is unit-adjusted to a velocity (`length/time`), since the multiplication of `length/time^2` and `time`
would cause one of the `time` components in the denominator to cancel out.

The above example shows two or more quantities being operated on that have the same `length` and `time` units (meters and seconds
respectively). It's also possible to operate on quantities that have differing units in the same dimension (e.g. meters and feet).
When this happens, unit conversions are automatically performed and the final Quantity after the operation takes on the units of
the left-hand side of the operation.

_(For more information on unit vectors and dimensionality, see the Unit Vectors and Dimensionality section below.)_

```csharp
// C#
Quantity lLengthA = "5 ft";

Quantity lLengthB = "1 m";

// When executed, will be 16.4 ft^2, because the left-hand operand is in feet.
Quantity lAreaInSquareFeet = lLengthA * lLengthB;

// When executed, will be 1.524 m^2, because the left-hand operand is in meters.
Quantity lAreaInSquareMeters = lLengthB * lLengthA;
```
```vb
' Visual Basic
Dim lLengthA As Quantity = "5 ft"

Dim lLengthB As Quantity = "1 m"

' When executed, will be 16.4 ft^2, because the left-hand operand is in feet.
Dim lAreaInSquareFeet As Quantity = lLengthA * lLengthB

' When executed, will be 1.524 m^2, because the left-hand operand is in meters.
Dim lAreaInSquareMeters As Quantity = lLengthB * lLengthA
```

#### Addition, Subtraction and Congruency
Any Quantity can be added to, or subtracted from, another Quantity, provided that the two quantities are 
_dimensionally congruent_. Dimensional congruence requires both Quantities to have the same dimensionality
(e.g. if A is `length/time^2`, B must also be `length/time^2`). If the Quantities are not dimensionally congruent,
addition and subtraction operations will throw a `InvalidOperationException`.

If the operands are dimensionally congruent, just like for multiplication and division, unit conversions will be 
automatically handled, with the left-hand operand's units being used in the final result.

```csharp
// C#

// -----------------------
// Congruent Units Example
// -----------------------
Quantity lLengthA = "5 ft";

Quantity lLengthB = "1 m";

// When executed, will be 8.281 ft
Quantity lTotalLengthInFeet = lLengthA + lLengthB;

// When executed, will be -0.524 m
Quantity lLengthDifferenceInMeters = lLengthB - lLengthA;

// -----------------------
// Incongruent Units Example
// -----------------------

// Acceleration
Quantity lGravity = "9.81 m/s^2";

// Time
Quantity lTime = "5 s";

// When executed, throws an InvalidOperationException
Quantity lQuestionable = lGravity + lTime;
```
```vb
' Visual Basic

' -----------------------
' Congruent Units Example
' -----------------------
Dim lLengthA As Quantity = "5 ft"

Dim lLengthB As Quantity = "1 m"

' When executed, will be 8.281 ft
Dim lTotalLengthInFeet As Quantity = lLengthA + lLengthB

' When executed, will be -0.524 m
Dim lLengthDifferenceInMeters As Quantity = lLengthB - lLengthA

' -----------------------
' Incongruent Units Example
' -----------------------

' Acceleration
Dim lGravity As Quantity = "9.81 m/s^2"

' Time
Dim lTime As Quantity = "5 s"

' When executed, throws an InvalidOperationException
Dim lQuestionable As Quantity = lGravity + lTime
```

#### Conversion
Users can also do a simple conversion to existing Quantities by calling `convert` on the Quantity to convert. The unit 
vector string provided to the `convert` must be dimensionally congruent to the existing quantity.

```csharp
// C#
Quantity lFeet = "5 ft";

// When executed, will be 1.524 m
Quantity lMeters = lFeet.convert( "m" );

Quantity lAcceleration = "9.81 m/s^2";

// When executed, will be 32.185 ft/s^2
lAcceleration = lAcceleration.convert( "ft/s^2" );
```
```vb
' Visual Basic
Dim lFeet As Quantity = "5 ft"

' When executed, will be 1.524 m
Dim lMeters As Quantity = lFeet.convert( "m" )

Dim lAcceleration As Quantity = "9.81 m/s^2"

' When executed, will be 32.185 ft/s^2
lAcceleration = lAcceleration.convert( "ft/s^2" )
```

##### Units with Offsets (e.g. Celsius/Fahrenheit)
Units that have a different zero point than their base unit, such as the Celsius and Fahrenheit units,
work a little differently than others.

Conversions between these units when doing normal math operations will consider them "temperature differences" 
rather than absolute temperatures. This means that the offset is not considered and only the scaling factor is
applied. So, for Celsius Difference to Fahrenheit Difference, the scaling factor of `1.8` is applied.

To convert between absolute temperatures, the temperatures cannot be combined with other dimensions, and the
`convert` function must be used.

```csharp
// C#
Quantity lCelsius = "37 C";

// Will be "98.6 F"
Quantity lFahrenheit = lCelsius.convert( "F" );
```
```vb
' Visual Basic
Dim lCelsius As Quantity = "37 C"

' Will be "98.6 F"
Dim lFahrenheit As Quantity = lCelsius.convert( "F" )
```

#### Comparison and Equality
Quantity values support the following operators: `<`, `<=`, `==` (`=` in VB), `!=` (`<>` in VB), `>=`, `>`.

These comparisons will automatically take unit conversions into consideration.

```csharp
// C#
Quantity lFoot = "1 ft";
Quantity lMeter = "1 m";

// Will be "true" since a foot is shorter than a meter
bool lLessThan = lFoot < lMeter;
```
```vb
' Visual Basic
Dim lFoot As Quantity = "1 ft";
Dim lMeter As Quantity = "1 m";

' Will be "True" since a foot is shorter than a meter
Dim lLessThan As Boolean = lFoot < lMeter;
```
For less-than/greater-than style comparisons, the two quantities must be dimensionally congruent. 
Otherwise, they cannot be compared and an `InvalidOperationException` will be thrown.

For equality comparisons, the two quantities do not have to be congruent. However, equality 
will only return `true` if both operands are congruent and _exactly_ equal. These operations
suffer from the same problems as standard floating-point equality testing, and should be 
avoided. Instead, users are encouraged to use tolerance/delta comparison techniques to ensure
a value is within a tolerance of the desired value.

## Unit Vectors and Dimensionality

In the previous section, many examples of how to use Quantities in a basic way were presented. This section gives more detailed
information and rules for using unit vector strings (e.g. `"m/s^2"`).

### Dimensions
In Metrology, a dimension represents a single, independent type of quantity. For instance, in the SI system, there are 7 
dimensions: Length, Time, Mass, Electric Current, Temperature, Amount of Substance, Luminous Intensity. In the SI system, 
all units are either defined within one dimension, or some combination of these 7 dimensions.

Metrology builds on this concept to automatically track and handle units in a calculation.

### Congruence
In Metrology, dimensional congruence is a property between two quantities if those two quantities
contain the exact same set of dimensions, with the same exponents. This means the two quantities
represent the same physical concept (e.g. acceleration).

This property is required for a number of operations in Metrology to work (such as addition,
subtraction and comparison). If any of these operations are attempted, and the quantities are
not congruent, an InvalidOperationException will be thrown.

In situations where Quantity values come from an unknown source or have unknown dimensionality,
the `Quantity.isCongruent()` method can be used to check for dimensional congruence.

```csharp
// C#
Quantity lFootPerSecond = "1 ft/s";
Quantity lMeterPerSecond = "1 m/s";
Quantity lMeterPerSecondSq = "1 m/s^2";
Quantity lKilogramPerSecond = "1 kg/s";

// Will return "true" since both are "length/time"
bool lIsCongruent1 = lFootPerSecond.isCongruent( lMeterPerSecond );

// Will return "false" since one is "length/time" and the
// other is "mass/time".
bool lIsCongruent2 = lFootPerSecond.isCongruent( lKilogramPerSecond );

// Will return "false" since one is "length/time" and the
// other is "length/time^2". Dimensions AND exponents must match.
bool lIsCongruent2 = lFootPerSecond.isCongruent( lMeterPerSecondSq );
```
```vb
' Visual Basic
Dim lFootPerSecond As Quantity = "1 ft/s";
Dim lMeterPerSecond As Quantity = "1 m/s";
Dim lMeterPerSecondSq As Quantity = "1 m/s^2";
Dim lKilogramPerSecond As Quantity = "1 kg/s";

' Will return "True" since both are "length/time"
Dim lIsCongruent1 As Boolean = lFootPerSecond.isCongruent( lMeterPerSecond );

' Will return "False" since one is "length/time" and the
' other is "mass/time".
Dim lIsCongruent2 As Boolean = lFootPerSecond.isCongruent( lKilogramPerSecond );

' Will return "false" since one is "length/time" and the
' other is "length/time^2". Dimensions AND exponents must match.
Dim lIsCongruent2 As Boolean = lFootPerSecond.isCongruent( lMeterPerSecondSq );
```

### Unit Vectors
In Metrology, a Unit Vector represents the combination of dimensions that make up a physical quantity, along with the units
and exponent being used in that dimension. Each dimension+unit+exponent part of the vector is called a "component". For instance,
in this acceleration vector (`"m/s^2"`), the components are `m` and `s^2` (although it's really `s^-2` as it represents `1/s^2`).

### Unit Vector and Component Formats
Metrology employs a specific and mostly strict format for representing unit vectors as strings. Metrology is generally not smart 
enough to handle cases that deviate from the rules.

#### Vector Components
The format for an individual unit vector component is `(prefix abbreviation)(unit abbreviation)^(exponent)`. For instance,
to specify a unit vector for "square kilometers", the string `"km^2"` would be used. If the desired exponent is `1`, it may
be omitted.

#### Full Unit Vector
There are effectively two formats:

#### 1. With no division symbol
This style of unit vector is a simple series of Vector Components separated by multiplication symbols 
(`*` or `⋅` [22C5 in Unicode]). 

For instance, acceleration would be `"m*s^-2"`.

#### 2. With a division symbol
This style of unit vector is the same as the first, but provides one (and only one) division symbol (`/`) between the unit vector 
components. Components to the right of the `/` will be given negative exponents automatically. 

For instance, acceleration would be `"m/s^2"`.

This is the format that is used when a user calls `Quantity.ToString()`.

It's important to know that any other format or formatting mistakes (such as using prefix and unit abbreviations that are not 
recognized, or having units in the same dimension appear twice) will result in an `InvalidOperationException` being thrown.

## Built-in Units, Dimensions and Prefixes

Metrology comes with many built-in units, dimensions and prefixes. The following tables outline all of the built-in types.

### Real Dimensions
| Name                | 
| ------------------- |
| Length              |
| Time                |
| Mass                |
| Current             |
| Temperature         |
| Amount of Substance |
| Luminous Intensity  |

### Pseudo-Dimensions
To make the logic of Metrology more consistent, especially around dimensionless quantities, additional pseudo-dimensions
are provided.

| Name                | 
| ------------------- |
| Angle               |
| Data Storage        |

### Metric Units
| Name     | Symbol | Dimension                  |
| -------- | :----: | -------------------------- |
| Ampere   | `A`    | Current (base)             |
| Candela  | `cd`   | Luminous Intensity (base)  |
| Celsius* | `C`    | Temperature                |
| Gram     | `g`    | Mass (base)                |
| Kelvin   | `K`    | Temperature (base)         |
| Meter    | `m`    | Length (base)              |
| Mole     | `mol`  | Amount of Substance (base) |
| Second   | `s`    | Time (base)                |

#### Imperial Units
| Name        | Symbol | Dimension           |
| ----------- | :----: | ------------------- |
| Fahrenheit* | `F`    | Temperature         |
| Foot        | `ft`   | Length              |
| Inch        | `in`   | Length              |
| Mile        | `mi`   | Length              |
| Ounce       | `oz`   | Mass                |
| Pound       | `lb`   | Mass                |
| Rankine     | `R`    | Temperature         |
| Slug        | `slug` | Mass                |
| Thou        | `mil`  | Length              |
| Yard        | `yd`   | Length              |

#### Data Storage Units
| Name    | Symbol | Dimension           |
| ------- | :----: | ------------------- |
| Bit     | `b`    | Data Storage        |
| Byte    | `B`    | Data Storage (base) |
| Nibble  | `nib`  | Data Storage        |

#### Angular Units
| Name     | Symbol    | Dimension           |
| -------- | :-------: | ------------------- |
| Degree   | `deg`     | Angle               |
| Grad     | `grad`    | Angle               |
| Quadrant | `quad`    | Angle               |
| Radian   | `rad`     | Angle (base)        |
| Sextant  | `sextant` | Angle               |
| Turn     | `turn`    | Angle               |

#### Time Units
| Name     | Symbol | Dimension           |
| -------- | :----: | ------------------- |
| Day      | `day`  | Time                |
| Hour     | `hr`   | Time                |

_(Note: Minutes are currently omitted because of ambiguity with other
possible units+prefixes. This will be handled in a later version.)_

#### Esoteric/Fun Units
| Name    | Symbol | Dimension           |
| ------- | :----: | ------------------- |
| Smoot   | `sm`   | Length              |

_* - Indicates an Offset Unit. This unit will be treated as a measure of difference and will only be treated as an absolute value when 
converted using the `convert` function and during comparison operations._

_(base) - Indicates a Base Unit. This unit serves as the root basis for all other units in the same
dimension._

### Derived Units
| Name            | Symbol | Base Units             |
| --------------- | :----: | ---------------------- |
| Feet-per-second | `fps`  | `ft / s`               |
| Knot            | `kts`  | `NM / hr`              |
| Miles-per-hour  | `mph`  | `mi / hr`              |
| Newton          | `N`    | `kg * m / s^2`         |
| Ohm             | `Ω`    | `kg * m^2 / s^3 * A^2` |
| Pascal          | `Pa`   | `kg / m * s^2`         |
| Pound-Force     | `lbf`  | `slug * ft / s^2`      |
| Volt            | `V`    | `kg * m^2 / s^3 * A`   |

### Prefixes
#### SI Prefixes
| Name    | Symbol | Scale                | Name    | Symbol | Scale                |
| ------- | :----: | -------------------- | ------- | :----: | -------------------- |
| Yotta   | `Y`    | 10^24                | Deci    | `d`    | 10^-1                |
| Zetta   | `Z`    | 10^21                | Centi   | `c`    | 10^-2                |
| Exa     | `E`    | 10^18                | Milli   | `m`    | 10^-3                |
| Peta    | `P`    | 10^15                | Micro   | `µ`    | 10^-6                |
| Tera    | `T`    | 10^12                | Nano    | `n`    | 10^-9                |
| Giga    | `G`    | 10^9                 | Pico    | `p`    | 10^-12               |
| Mega    | `M`    | 10^6                 | Femto   | `f`    | 10^-15               |
| Kilo    | `k`    | 10^3                 | Atto    | `a`    | 10^-18               |
| Hecto   | `h`    | 10^2                 | Zepto   | `z`    | 10^-21               |
| Deca    | `da`   | 10^1                 | Yocto   | `y`    | 10^-24               |

#### Data Storage Prefixes
| Name    | Symbol | Scale                |
| ------- | :----: | -------------------- |
| Yobi    | `Yi`   | 1024^8               |
| Zebi    | `Zi`   | 1024^7               |
| Exbi    | `Ei`   | 1024^6               |
| Pebi    | `Pi`   | 1024^5               |
| Tebi    | `Ti`   | 1024^4               |
| Gibi    | `Gi`   | 1024^3               |
| Mebi    | `Mi`   | 1024^2               |
| Kibi    | `Ki`   | 1024                 |
