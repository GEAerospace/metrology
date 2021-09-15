// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace GEAviation.Metrology
{
    /// <summary>
    /// This class provides wrappers for <see cref="Math"/> functions that work on
    /// Quantities.
    /// </summary>
    public static class QuantityMath
    {
        /// <summary>
        /// Returns a Quantity that represents Pi (π).
        /// </summary>
        public static Quantity PI { get; } = new Quantity( (decimal)Math.PI, "1" );

        /// <summary>
        /// Returns a Quantity that represents Euler's Number (e).
        /// </summary>
        public static Quantity E { get; } = new Quantity( (decimal)Math.E, "1" );

        /// <summary>
        /// Returns a Quantity that represents the absolute value of the specified
        /// Quantity.
        /// </summary>
        /// <param name="aQuantity">
        /// The Quantity to get the absolute value of.
        /// </param>
        /// <returns>
        /// The Quantity that represents the absolute value of the specified Quantity.
        /// </returns>
        public static Quantity Abs( Quantity aQuantity )
        {
            return new Quantity( Math.Abs( aQuantity.Amount ), aQuantity.Units );
        }

        /// <summary>
        /// Returns the angle whose Cosine is the specified number. This is
        /// equivalent to <see cref="Math.Acos(double)"/>.
        /// </summary>
        /// <param name="aQuantity">
        /// The dimensionless Cosine number to get the angle for.
        /// </param>
        /// <returns>
        /// The angle.
        /// </returns>
        public static Quantity Acos( this Quantity aQuantity )
        {
            var lExpectedVector = (Quantity)"1";
            if ( !aQuantity.isCongruent( lExpectedVector ) )
            {
                throw new ArgumentException( "Value must be dimensionless.", nameof( aQuantity ) );
            }

            var lAngle = (decimal)Math.Acos( (double)aQuantity.Amount );
            return new Quantity( lAngle, "rad" );
        }

        /// <summary>
        /// Returns the angle whose Sine is the specified number. This is
        /// equivalent to <see cref="Math.Asin(double)"/>.
        /// </summary>
        /// <param name="aQuantity">
        /// The dimensionless Sine number to get the angle for.
        /// </param>
        /// <returns>
        /// The angle.
        /// </returns>
        public static Quantity Asin( this Quantity aQuantity )
        {
            var lExpectedVector = (Quantity)"1";
            if ( !aQuantity.isCongruent( lExpectedVector ) )
            {
                throw new ArgumentException( "Value must be dimensionless.", nameof( aQuantity ) );
            }

            var lAngle = (decimal)Math.Asin( (double)aQuantity.Amount );
            return new Quantity( lAngle, "rad" );
        }

        /// <summary>
        /// Returns the angle whose Tangent is the specified number. This is
        /// equivalent to <see cref="Math.Atan(double)"/>.
        /// </summary>
        /// <param name="aQuantity">
        /// The dimensionless Tangent number to get the angle for.
        /// </param>
        /// <returns>
        /// The angle.
        /// </returns>
        public static Quantity Atan( this Quantity aQuantity )
        {
            var lExpectedVector = (Quantity)"1";
            if ( !aQuantity.isCongruent( lExpectedVector ) )
            {
                throw new ArgumentException( "Value must be dimensionless.", nameof( aQuantity ) );
            }

            var lAngle = (decimal)Math.Atan( (double)aQuantity.Amount );
            return new Quantity( lAngle, "rad" );
        }

        /// <summary>
        /// Returns the Cosine of the provided angle. This is
        /// equivalent to <see cref="Math.Cos(double)"/>.
        /// </summary>
        /// <param name="aQuantity">
        /// The angle to get the Cosine for.
        /// </param>
        /// <returns>
        /// The dimensionless cosine number for the specified angle.
        /// </returns>
        public static Quantity Cos( this Quantity aQuantity )
        {
            var lExpectedVector = (Quantity)"1 rad";
            if ( !aQuantity.isCongruent( lExpectedVector ) )
            {
                throw new ArgumentException( "Value must be an angle.", nameof( aQuantity ) );
            }

            var lInRadians = aQuantity.convert( "rad" );
            return new Quantity( (decimal)Math.Cos( (double)lInRadians.Amount ), "" );
        }

        /// <summary>
        /// Returns the Hyperbolic Cosine of the provided angle. This is
        /// equivalent to <see cref="Math.Cosh(double)"/>.
        /// </summary>
        /// <param name="aQuantity">
        /// The angle to get the Hyperbolic Cosine for.
        /// </param>
        /// <returns>
        /// The dimensionless hyperbolic cosine number for the specified angle.
        /// </returns>
        public static Quantity Cosh( this Quantity aQuantity )
        {
            var lExpectedVector = (Quantity)"1 rad";
            if ( !aQuantity.isCongruent( lExpectedVector ) )
            {
                throw new ArgumentException( "Value must be an angle.", nameof( aQuantity ) );
            }

            var lInRadians = aQuantity.convert( "rad" );
            return new Quantity( (decimal)Math.Cosh( (double)lInRadians.Amount ), "" );
        }

        /// <summary>
        /// Returns the Sine of the provided angle. This is
        /// equivalent to <see cref="Math.Sin(double)"/>.
        /// </summary>
        /// <param name="aQuantity">
        /// The angle to get the Sine for.
        /// </param>
        /// <returns>
        /// The dimensionless sine number for the specified angle.
        /// </returns>
        public static Quantity Sin( this Quantity aQuantity )
        {
            var lExpectedVector = (Quantity)"1 rad";
            if ( !aQuantity.isCongruent( lExpectedVector ) )
            {
                throw new ArgumentException( "Value must be an angle.", nameof( aQuantity ) );
            }

            var lInRadians = aQuantity.convert( "rad" );
            return new Quantity( (decimal)Math.Sin( (double)lInRadians.Amount ), "" );
        }

        /// <summary>
        /// Returns the Hyperbolic Sine of the provided angle. This is
        /// equivalent to <see cref="Math.Sinh(double)"/>.
        /// </summary>
        /// <param name="aQuantity">
        /// The angle to get the Hyperbolic Sine for.
        /// </param>
        /// <returns>
        /// The dimensionless hyperbolic sine number for the specified angle.
        /// </returns>
        public static Quantity Sinh( this Quantity aQuantity )
        {
            var lExpectedVector = (Quantity)"1 rad";
            if ( !aQuantity.isCongruent( lExpectedVector ) )
            {
                throw new ArgumentException( "Value must be an angle.", nameof( aQuantity ) );
            }

            var lInRadians = aQuantity.convert( "rad" );
            return new Quantity( (decimal)Math.Sinh( (double)lInRadians.Amount ), "" );
        }

        /// <summary>
        /// Returns the Tangent of the provided angle. This is
        /// equivalent to <see cref="Math.Tan(double)"/>.
        /// </summary>
        /// <param name="aQuantity">
        /// The angle to get the Tangent for.
        /// </param>
        /// <returns>
        /// The dimensionless tangent number for the specified angle.
        /// </returns>
        public static Quantity Tan( this Quantity aQuantity )
        {
            var lExpectedVector = (Quantity)"1 rad";
            if ( !aQuantity.isCongruent( lExpectedVector ) )
            {
                throw new ArgumentException( "Value must be an angle.", nameof( aQuantity ) );
            }

            var lInRadians = aQuantity.convert( "rad" );
            return new Quantity( (decimal)Math.Tan( (double)lInRadians.Amount ), "" );
        }

        /// <summary>
        /// Returns the Hyperbolic Tangent of the provided angle. This is
        /// equivalent to <see cref="Math.Tanh(double)"/>.
        /// </summary>
        /// <param name="aQuantity">
        /// The angle to get the Hyperbolic Tangent for.
        /// </param>
        /// <returns>
        /// The dimensionless hyperbolic tangent number for the specified angle.
        /// </returns>
        public static Quantity Tanh( this Quantity aQuantity )
        {
            var lExpectedVector = (Quantity)"1 rad";
            if ( !aQuantity.isCongruent( lExpectedVector ) )
            {
                throw new ArgumentException( "Value must be an angle.", nameof( aQuantity ) );
            }

            var lInRadians = aQuantity.convert( "rad" );
            return new Quantity( (decimal)Math.Tanh( (double)lInRadians.Amount ), "" );
        }

        /// <summary>
        /// Returns a Quantity representing the smallest integral value that is greater 
        /// than or equal to the specified Quantity. This is equivalent to 
        /// <see cref="Math.Ceiling(decimal)"/>.
        /// </summary>
        /// <param name="aQuantity">
        /// The Quantity to get the Ceiling of.
        /// </param>
        /// <returns>
        /// The calculated Ceiling Quantity.
        /// </returns>
        public static Quantity Ceiling( this Quantity aQuantity )
        {
            return new Quantity( Math.Ceiling( aQuantity.Amount ), aQuantity.Units );
        }

        /// <summary>
        /// Returns a Quantity representing the largest integral value that is less 
        /// than or equal to the specified Quantity. This is equivalent to 
        /// <see cref="Math.Floor(decimal)"/>.
        /// </summary>
        /// <param name="aQuantity">
        /// The Quantity to get the Floor of.
        /// </param>
        /// <returns>
        /// The calculated Floor Quantity.
        /// </returns>
        public static Quantity Floor( this Quantity aQuantity )
        {
            return new Quantity( Math.Floor( aQuantity.Amount ), aQuantity.Units );
        }

        /// <summary>
        /// Rounds the quantity to the specified number of decimal digits, using the 
        /// specified <see cref="MidpointRounding"/> mode. This is equivalent to 
        /// <see cref="Math.Round(decimal,int,MidpointRounding)"/>.
        /// </summary>
        /// <param name="aQuantity">
        /// The Quantity to round.
        /// </param>
        /// <param name="aDigits">
        /// The number of decimal digits to round to.
        /// </param>
        /// <param name="aMidpointRoundingMode">
        /// The rounding mode to use.
        /// </param>
        /// <returns>
        /// The rounded Quantity.
        /// </returns>
        public static Quantity Round( this Quantity aQuantity, int aDigits, MidpointRounding aMidpointRoundingMode )
        {
            return new Quantity( Math.Round( aQuantity.Amount, aDigits, aMidpointRoundingMode ), aQuantity.Units );
        }

        /// <summary>
        /// Rounds the quantity to the specified number of decimal digits, using the 
        /// <see cref="MidpointRounding.ToEven"/> mode. This is equivalent to 
        /// <see cref="Math.Round(decimal,int)"/>.
        /// </summary>
        /// <param name="aQuantity">
        /// The Quantity to round.
        /// </param>
        /// <param name="aDigits">
        /// The number of decimal digits to round to.
        /// </param>
        /// <returns>
        /// The rounded Quantity.
        /// </returns>
        public static Quantity Round( this Quantity aQuantity, int aDigits )
        {
            return QuantityMath.Round( aQuantity, aDigits, MidpointRounding.ToEven );
        }

        /// <summary>
        /// Rounds the quantity to zero decimal digits, using the 
        /// <see cref="MidpointRounding.ToEven"/> mode. This is equivalent to 
        /// <see cref="Math.Round(decimal)"/>.
        /// </summary>
        /// <param name="aQuantity">
        /// The Quantity to round.
        /// </param>
        /// <param name="aDigits">
        /// The number of decimal digits to round to.
        /// </param>
        /// <returns>
        /// The rounded Quantity.
        /// </returns>
        public static Quantity Round( this Quantity aQuantity )
        {
            return QuantityMath.Round( aQuantity, 0, MidpointRounding.ToEven );
        }

        /// <summary>
        /// Returns an integer that indicates the sign of the specified Quantity.
        /// </summary>
        /// <param name="aQuantity">
        /// The Quantity to check the sign of.
        /// </param>
        /// <returns>
        /// -1 if the Quantity is less than zero, 0 if it is equal to zero, 
        /// 1 if it is greater than zero.
        /// </returns>
        public static int Sign( this Quantity aQuantity )
        {
            return Math.Sign( aQuantity.Amount );
        }

        /// <summary>
        /// Returns a Quantity representing just the integral part of the specified Quantity.
        /// </summary>
        /// <param name="aQuantity">
        /// The Quantity to Truncate.
        /// </param>
        /// <returns>
        /// The Truncated Quantity.
        /// </returns>
        public static Quantity Truncate( this Quantity aQuantity )
        {
            return new Quantity( Math.Truncate( aQuantity.Amount ), aQuantity.Units );
        }

        /// <summary>
        /// Returns a Quantity that has been raised to the specified power.
        /// </summary>
        /// <param name="aQuantity">
        /// The Quantity to raise to a power.
        /// </param>
        /// <param name="aExponent">
        /// The power to raise the Quantity by.
        /// </param>
        /// <returns>
        /// The Quantity that has been raised to the specified power.
        /// </returns>
        public static Quantity Pow( this Quantity aQuantity, int aExponent )
        {
            if ( aExponent == 0 )
            {
                return "1";
            }

            var lNewQuantity = aQuantity.copy();
            var lAbsExponent = Math.Abs( aExponent ) - 1;

            Func<Quantity, Quantity> lDivide = ( a ) => a / aQuantity;
            Func<Quantity, Quantity> lMultiply = ( a ) => a * aQuantity;

            var lOperation = lMultiply;

            if ( aExponent < 0 )
            {
                lOperation = lDivide;
            }

            while ( lAbsExponent > 0 )
            {
                lNewQuantity = lOperation( lNewQuantity );
                lAbsExponent--;
            }

            return lNewQuantity;
        }

        /// <summary>
        /// Returns the smaller of the two specified Quantities. If both are equal,
        /// the first Quantity specified is returned.
        /// </summary>
        /// <param name="aFirst">
        /// The first Quantity.
        /// </param>
        /// <param name="aSecond">
        /// The second Quantity.
        /// </param>
        /// <returns>
        /// The minimum of the two Quantities.
        /// </returns>
        public static Quantity Min( Quantity aFirst, Quantity aSecond )
        {
            if ( !aFirst.isCongruent( aSecond ) )
            {
                throw new InvalidOperationException( "The quantities must be congruent." );
            }

            if ( aFirst <= aSecond )
            {
                return aFirst;
            }

            return aSecond;
        }

        /// <summary>
        /// Returns the larger of the two specified Quantities. If both are equal,
        /// the first Quantity specified is returned.
        /// </summary>
        /// <param name="aFirst">
        /// The first Quantity.
        /// </param>
        /// <param name="aSecond">
        /// The second Quantity.
        /// </param>
        /// <returns>
        /// The maximum of the two Quantities.
        /// </returns>
        public static Quantity Max( Quantity aFirst, Quantity aSecond )
        {
            if ( !aFirst.isCongruent( aSecond ) )
            {
                throw new InvalidOperationException( "The quantities must be congruent." );
            }

            if ( aFirst >= aSecond )
            {
                return aFirst;
            }

            return aSecond;
        }
    }
}
