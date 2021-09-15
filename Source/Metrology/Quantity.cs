// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEAviation.Metrology
{
    /// <summary>
    /// This class represents a numeric quantity along with its associated units,
    /// and supports mathematical operations that account for the manipulation of those
    /// units.
    /// </summary>
    public class Quantity : IComparable<Quantity>, IEquatable<Quantity>
    {
        private decimal mBaseAmount = 0.0m;
        private DimensionVector mBaseUnits = DimensionVector.Zero;
        private IEnumerable<VectorComponent<IDerivedUnit>> mIntendedDerivedUnits = Enumerable.Empty<VectorComponent<IDerivedUnit>>();

        private decimal mDerivedCompensatedAmount = 0.0m;
        private IEnumerable<VectorComponent<IUnit>> mDerivedCompensatedUnits = Enumerable.Empty<VectorComponent<IUnit>>();

        /// <summary>
        /// Provides a string representation for this Quantity. The result of this
        /// function can be used with <see cref="TryParse(string, out Quantity)"/> in 
        /// order to convert it back into a Quantity object.
        /// </summary>
        /// <returns>
        /// The string representation of this Quantity.
        /// </returns>
        public override string ToString() => $"{mDerivedCompensatedAmount} {mDerivedCompensatedUnits.ToUnitString()}".Trim();

        /// <summary>
        /// This read-only property returns a string representation of this Quantity's
        /// unit vector.
        /// </summary>
        private string BaseUnits
        {
            get
            {
                return mBaseUnits.ToString();
            }
        }

        /// <summary>
        /// The decimal amount this quantity represents.
        /// </summary>
        public decimal Amount
        {
            get
            {
                return mDerivedCompensatedAmount;
            }
        }

        /// <summary>
        /// The units of measure this quantity represents.
        /// </summary>
        public string Units
        {
            get
            {
                return mDerivedCompensatedUnits.ToUnitString();
            }

        }

        /// <summary>
        /// Constructs an empty Quantity. It's intentionally private to force
        /// the use of other constructors and static construction methods.
        /// </summary>
        private Quantity() { }

        /// <summary>
        /// Constructs a Quantity with the associate amount and unit vector.
        /// </summary>
        /// <param name="aAmount">
        /// The decimal portion of the Quantity to construct.
        /// </param>
        /// <param name="aUnitVector">
        /// A string representation of the unit vector to give the constructed Quantity.
        /// </param>
        public Quantity( decimal aAmount, string aUnitVector )
        {
            mBaseAmount = aAmount;

            if ( aUnitVector.TryParseUnitString<IUnit>( out var lComponents ) )
            {
                mBaseUnits = flattenToBaseUnits( lComponents, out mIntendedDerivedUnits, out var lFactor );
                mBaseAmount *= lFactor;
            }
            else
            {
                throw new InvalidOperationException( "Could not parse unit vector." );
            }

            applyDerivedUnits();
        }

        /// <summary>
        /// Constructs a copy of the provided quantity.
        /// </summary>
        /// <param name="aToCopy"></param>
        public Quantity( Quantity aToCopy )
        {
            var lToCopy = aToCopy ?? throw new ArgumentNullException( nameof( aToCopy ) );

            mBaseAmount = lToCopy.mBaseAmount;
            mBaseUnits = new DimensionVector( lToCopy.mBaseUnits );
            mIntendedDerivedUnits = lToCopy.mIntendedDerivedUnits.Select( aItem => new VectorComponent<IDerivedUnit>( aItem.Prefix, aItem.Unit, aItem.Exponent ) );
            applyDerivedUnits();
        }

        /// <summary>
        /// Creates a copy of this Quantity.
        /// </summary>
        /// <returns>
        /// The duplicate Quantity.
        /// </returns>
        public Quantity copy()
        {
            return new Quantity( this );
        }

        /// <summary>
        /// Converts the quantity to use the specified unit vector. This operation
        /// adjusts the quantity's amount to account for the unit conversions.
        /// </summary>
        /// <param name="aUnitVector">
        /// The unit vector to convert this Quantity to. This vector must be 
        /// dimensionally congruent to this Quantity's vector in order for the
        /// the conversion to occur. Otherwise, an Exception will be thrown.
        /// </param>
        /// <returns>
        /// The converted Quantity.
        /// </returns>
        public Quantity convert( string aUnitVector )
        {
            if ( !aUnitVector.TryParseUnitString<IUnit>( out var lVector ) )
            {
                throw new InvalidOperationException( "Could not parse unit vector." );
            }

            var lBaseVector = flattenToBaseUnits( lVector, out var lDerivedUnits, out var _ );

            var lNewQuantity = new Quantity( this );
            if ( !lNewQuantity.mBaseUnits.isCongruent( lBaseVector ) )
            {
                throw new InvalidOperationException( "Cannot convert to incongruent unit vector." );
            }

            // For offset units, attempt offset conversion iff there is exactly 
            // one unit in both vectors and all exponents are 1...
            var lToUnit = lBaseVector.Components.Select( aItem => aItem.Value ).OnlyOneOrDefault();
            var lFromUnit = lNewQuantity.mBaseUnits.Components.Select( aItem => aItem.Value ).OnlyOneOrDefault();

            if ( lToUnit != null && lToUnit.Exponent == 1
                && lFromUnit != null && lFromUnit.Exponent == 1 )
            {
                lNewQuantity.mBaseAmount = UnitConversion.convertWithOffset( lNewQuantity.mBaseAmount, lFromUnit.Unit.GetType(), lFromUnit.Prefix.GetType(), lToUnit.Unit.GetType(), lToUnit.Prefix.GetType() );
            }
            // ...otherwise do normal conversion.
            else
            {
                lNewQuantity.mBaseAmount *= lBaseVector.convert( lNewQuantity.mBaseUnits );
            }

            lNewQuantity.mBaseUnits = lBaseVector;
            lNewQuantity.mIntendedDerivedUnits = lDerivedUnits;

            lNewQuantity.applyDerivedUnits();
            return lNewQuantity;
        }

        /// <summary>
        /// Determines if this Quantity is dimensionally congruent to the provided
        /// Quantity.
        /// </summary>
        /// <param name="aOther">
        /// The Quantity to check for congruence with.
        /// </param>
        /// <returns>
        /// True if the this Quantity is dimensionally congruent to the provided
        /// Quantity, false otherwise.
        /// </returns>
        public bool isCongruent( Quantity aOther )
        {
            return this.mBaseUnits.isCongruent( aOther.mBaseUnits );
        }

        /// <summary>
        /// Multiplies this Quantity with the provided Quantity.
        /// </summary>
        /// <param name="aOperand">
        /// The Quantity to multiply this Quantity with.
        /// </param>
        /// <returns>
        /// The result of the multiplication operation.
        /// </returns>
        public Quantity multiply( Quantity aOperand )
        {
            var lNewQuantity = new Quantity( this );
            var lNewVector = lNewQuantity.mBaseUnits.multiply( aOperand.mBaseUnits, out var lFactor );
            var lOperandAmount = aOperand.mBaseAmount * lFactor;

            lNewQuantity.mBaseAmount *= lOperandAmount;
            lNewQuantity.mBaseUnits = lNewVector;
            lNewQuantity.applyDerivedUnits();
            return lNewQuantity;
        }

        /// <summary>
        /// This operator overload allows Quantities to be multiplied using the multiplication
        /// operator (*).
        /// </summary>
        /// <param name="aLeftOperand">
        /// The left-side Quantity in the multiplication operation.
        /// </param>
        /// <param name="aRightOperand">
        /// The right-side Quantity in the multiplication operation.
        /// </param>
        /// <returns>
        /// The result of the multiplication operation.
        /// </returns>
        public static Quantity operator *( Quantity aLeftOperand, Quantity aRightOperand )
        {
            return aLeftOperand.multiply( aRightOperand );
        }

        /// <summary>
        /// This operator overload allows a Quantity to be scaled by a decimal using the multiplication
        /// operator (*).
        /// </summary>
        /// <param name="aLeftOperand">
        /// The left-side Quantity in the multiplication operation.
        /// </param>
        /// <param name="aRightOperand">
        /// The right-side decimal in the multiplication operation.
        /// </param>
        /// <returns>
        /// The result of the multiplication operation.
        /// </returns>
        public static Quantity operator *( Quantity aLeftOperand, decimal aRightOperand )
        {
            var lNewQuantity = new Quantity( aLeftOperand );
            lNewQuantity.mBaseAmount *= aRightOperand;
            lNewQuantity.applyDerivedUnits();
            return lNewQuantity;
        }

        /// <summary>
        /// This operator overload allows a Quantity to be scaled by a decimal using the multiplication
        /// operator (*). This supports operations where the decimal value is the left-operand.
        /// </summary>
        /// <param name="aLeftOperand">
        /// The left-side decimal in the multiplication operation.
        /// </param>
        /// <param name="aRightOperand">
        /// The right-side Quantity in the multiplication operation.
        /// </param>
        /// <returns>
        /// The result of the multiplication operation.
        /// </returns>
        public static Quantity operator *( decimal aLeftOperand, Quantity aRightOperand )
        {
            return aRightOperand * aLeftOperand;
        }

        /// <summary>
        /// Divides this Quantity with the provided Quantity.
        /// </summary>
        /// <param name="aOperand">
        /// The Quantity to divide this Quantity by. If the "amount" of this
        /// operand is equal to <see cref="Decimal.Zero"/> a <see cref="DivideByZeroException"/>
        /// will be thrown.
        /// </param>
        /// <returns>
        /// The result of the division operation.
        /// </returns>
        public Quantity divide( Quantity aOperand )
        {
            if ( aOperand.mBaseAmount == decimal.Zero )
                throw new DivideByZeroException( "Cannot divide by a zero quantity." );

            var lNewQuantity = new Quantity( this );
            var lNewVector = lNewQuantity.mBaseUnits.divide( aOperand.mBaseUnits, out var lFactor );
            var lOperandAmount = aOperand.mBaseAmount / lFactor;

            lNewQuantity.mBaseAmount /= lOperandAmount;
            lNewQuantity.mBaseUnits = lNewVector;
            lNewQuantity.applyDerivedUnits();
            return lNewQuantity;
        }

        /// <summary>
        /// This operator overload allows Quantities to be divided using the division
        /// operator (/).
        /// </summary>
        /// <param name="aLeftOperand">
        /// The left-side operand of the division operation.
        /// </param>
        /// <param name="aRightOperand">
        /// The right-side operand of the division operation. If the "amount" of this
        /// operand is equal to <see cref="Decimal.Zero"/> a <see cref="DivideByZeroException"/>
        /// will be thrown.
        /// </param>
        /// <returns>
        /// The result of the division operation.
        /// </returns>
        public static Quantity operator /( Quantity aLeftOperand, Quantity aRightOperand )
        {
            return aLeftOperand.divide( aRightOperand );
        }

        /// <summary>
        /// This operator overload allows a Quantity to be divided by a decimal value using the division
        /// operator (/).
        /// </summary>
        /// <param name="aLeftOperand">
        /// The left-side operand of the division operation.
        /// </param>
        /// <param name="aRightOperand">
        /// The right-side operand of the division operation. If the "amount" of this
        /// operand is equal to <see cref="Decimal.Zero"/> a <see cref="DivideByZeroException"/>
        /// will be thrown.
        /// </param>
        /// <returns>
        /// The result of the division operation.
        /// </returns>
        public static Quantity operator /( Quantity aLeftOperand, decimal aRightOperand )
        {
            if ( aRightOperand == decimal.Zero )
                throw new DivideByZeroException( "Cannot divide by zero." );

            var lNewQuantity = new Quantity( aLeftOperand );
            lNewQuantity.mBaseAmount /= aRightOperand;
            lNewQuantity.applyDerivedUnits();
            return lNewQuantity;
        }

        /// <summary>
        /// Adds the specified Quantity to this Quantity.
        /// </summary>
        /// <param name="aOperand">
        /// The Quantity to add to this Quantity. This operand must be dimensionally congruent to
        /// this Quantity in order to perform the operation. Otherwise, an <see cref="InvalidOperationException"/>
        /// will be thrown.
        /// </param>
        /// <returns>
        /// The Quantity that represents the sum.
        /// </returns>
        public Quantity add( Quantity aOperand )
        {
            if ( !this.mBaseUnits.isCongruent( aOperand.mBaseUnits ) )
            {
                throw new InvalidOperationException( "Cannot add incongruent Quantities. Unit dimensions and exponents must match." );
            }

            var lNewQuantity = new Quantity( this );
            lNewQuantity.mBaseAmount += aOperand.mBaseAmount * lNewQuantity.mBaseUnits.convert( aOperand.mBaseUnits );
            lNewQuantity.applyDerivedUnits();
            return lNewQuantity;
        }

        /// <summary>
        /// This operator overload allows two Quantities to be summed using the addition
        /// operator (+). Both operands must be dimensionally congruent to each other in 
        /// order to perform the operation. Otherwise, an <see cref="InvalidOperationException"/>
        /// will be thrown.
        /// </summary>
        /// <param name="aLeftOperand">
        /// The left-side operand of the addition operation. 
        /// </param>
        /// <param name="aRightOperand">
        /// The right-side operand of the addition operation. 
        /// </param>
        /// <returns>
        /// The Quantity that represents the sum of the two operands.
        /// </returns>
        public static Quantity operator +( Quantity aLeftOperand, Quantity aRightOperand )
        {
            return aLeftOperand.add( aRightOperand );
        }

        /// <summary>
        /// Subtracts the specified Quantity from this Quantity.
        /// </summary>
        /// <param name="aOperand">
        /// The Quantity to subtract from this Quantity. This operand must be dimensionally congruent to
        /// this Quantity in order to perform the operation. Otherwise, an <see cref="InvalidOperationException"/>
        /// will be thrown.
        /// </param>
        /// <returns>
        /// The Quantity that represents the subtraction.
        /// </returns>
        public Quantity subtract( Quantity aOperand )
        {
            if ( !this.mBaseUnits.isCongruent( aOperand.mBaseUnits ) )
            {
                throw new InvalidOperationException( "Cannot subtract incongruent Quantities. Unit dimensions and exponents must match." );
            }

            var lNewQuantity = new Quantity( this );
            lNewQuantity.mBaseAmount -= aOperand.mBaseAmount * lNewQuantity.mBaseUnits.convert( aOperand.mBaseUnits );
            lNewQuantity.applyDerivedUnits();
            return lNewQuantity;
        }

        /// <summary>
        /// Attempts to extract desired derived units from the "base vector". This treats derived
        /// units as "overlays" rather than first-class units. The mechanism is naive, but probably good
        /// enough for a first pass implementation. Later implementations/iterations should not track 
        /// individual derived units but rather derived unit combinations, otherwise, if the quantity is 
        /// meant to represent a combination of derived units that cancel each other out, the cancellation
        /// will prevent the display of the original combination, even though that representation would
        /// be valid and desired.
        /// </summary>
        private void applyDerivedUnits()
        {
            mDerivedCompensatedUnits = mBaseUnits.ApplyDerivedUnits( mIntendedDerivedUnits, out var lFactor );
            mDerivedCompensatedAmount = mBaseAmount * lFactor;

            // The following is a tiny trick that normalizes decimal values with a lot of zeroes
            // in its representation.
            mBaseAmount /= 1.000000000000000000000000000000000m;
            mDerivedCompensatedAmount /= 1.000000000000000000000000000000000m;
        }

        /// <summary>
        /// This operator overload allows two Quantities to be subtracted using the subtraction
        /// operator (-). Both operands must be dimensionally congruent to each other in 
        /// order to perform the operation. Otherwise, an <see cref="InvalidOperationException"/>
        /// will be thrown.
        /// </summary>
        /// <param name="aLeftOperand">
        /// The left-side operand of the subtraction operation. 
        /// </param>
        /// <param name="aRightOperand">
        /// The right-side operand of the subtraction operation. 
        /// </param>
        /// <returns>
        /// The Quantity that represents the subtraction of the two operands.
        /// </returns>
        public static Quantity operator -( Quantity aLeftOperand, Quantity aRightOperand )
        {
            return aLeftOperand.subtract( aRightOperand );
        }

        /// <summary>
        /// Attempts to parse a Quantity value from a string representation.
        /// </summary>
        /// <param name="aToParse">
        /// The string representation to attempt to parse.
        /// </param>
        /// <param name="aResult">
        /// The Quantity that was parsed from the string if successful. Null otherwise.
        /// </param>
        /// <returns>
        /// True if the parse was successful and <paramref name="aResult"/> contains a
        /// valid Quantity. False otherwise.
        /// </returns>
        public static bool TryParse( string aToParse, out Quantity aResult )
        {
            aResult = new Quantity();
            aResult.mBaseUnits = DimensionVector.Zero;

            var lAmountAndUnits = aToParse.Split( new string[] { " " }, StringSplitOptions.RemoveEmptyEntries );

            var lConversionFactor = 1.0m;

            if ( lAmountAndUnits.Length > 1 )
            {
                var lRecombined = string.Join( " ", lAmountAndUnits.Skip( 1 ) );

                if ( !lRecombined.TryParseUnitString<IUnit>( out var lComponents ) )
                {
                    aResult = null;
                    return false;
                }

                aResult.mBaseUnits = flattenToBaseUnits( lComponents, out aResult.mIntendedDerivedUnits, out var lFactor );
                lConversionFactor *= lFactor;
            }

            if ( !decimal.TryParse( lAmountAndUnits[0], out var lAmount ) )
            {
                aResult = null;
                return false;
            }

            aResult.mBaseAmount = lAmount * lConversionFactor;
            aResult.applyDerivedUnits();
            return true;
        }

        /// <summary>
        /// This method takes a unit vector that is mixed between any number of unit types (ISimpleUnit and IDerivedUnit)
        /// and "flattens" it into ISimpleUnits only. 
        /// </summary>
        /// <param name="aComplexVector">
        /// The vector to "flatten".
        /// </param>
        /// <param name="aDerivedUnitVector">
        /// This outputs a vector containing the derived components that were found in the full vector.
        /// </param>
        /// <param name="aConversionFactor">
        /// If conversions occur during the flattening, this value represents the conversion factor.
        /// </param>
        /// <returns>
        /// The "flattened" vector.
        /// </returns>
        private static DimensionVector flattenToBaseUnits( IEnumerable<VectorComponent<IUnit>> aComplexVector, out IEnumerable<VectorComponent<IDerivedUnit>> aDerivedUnitVector, out decimal aConversionFactor )
        {
            aConversionFactor = 1.0m;

            var lDerived = aComplexVector.getSpecificComponents<IUnit, IDerivedUnit>();
            var lSimple = aComplexVector.getSpecificComponents<IUnit, ISimpleUnit>();

            var lBaseVector = DimensionVector.Zero;

            foreach ( var lDerivedComponent in lDerived )
            {
                aConversionFactor *= lDerivedComponent.Prefix.Scale;
                lBaseVector = lBaseVector.multiply( DimensionVector.Pow( lDerivedComponent.Unit.BaseVector, lDerivedComponent.Exponent ), out var lAdditionalFactor );
                aConversionFactor *= lAdditionalFactor;
            }

            if ( lSimple.Any() )
            {
                var lSimpleVector = DimensionVector.create( lSimple );
                lBaseVector = lBaseVector.multiply( lSimpleVector, out var lAdditionalFactor );
                aConversionFactor *= lAdditionalFactor;
            }

            aDerivedUnitVector = lDerived;
            return lBaseVector;
        }

        /// <summary>
        /// This operator allows users to use strings a "literals" for Quantities by
        /// automatically converting a string to a Quantity if possible. This operation
        /// will throw a <see cref="InvalidOperationException"/> if the string does not 
        /// represent a valid Quantity.
        /// </summary>
        /// <param name="aLiteral">
        /// The string to use as a Quantity literal.
        /// </param>
        public static implicit operator Quantity( string aLiteral )
        {
            if ( !Quantity.TryParse( aLiteral, out var lQuantity ) )
            {
                throw new InvalidOperationException( "Could not parse quantity string. Please check the format." );
            }
            return lQuantity;
        }

        /// <summary>
        /// Implements <see cref="IComparable{T}.CompareTo(T)"/>.
        /// </summary>
        public int CompareTo( Quantity other )
        {
            if ( !this.isCongruent( other ) )
            {
                throw new InvalidOperationException( "Cannot compare incongruent quantities." );
            }

            // For offset units, attempt offset conversion iff there is exactly 
            // one unit in both vectors and all exponents are 1...
            var lToUnit = this.mBaseUnits.Components.Select( aItem => aItem.Value ).OnlyOneOrDefault();
            var lFromUnit = other.mBaseUnits.Components.Select( aItem => aItem.Value ).OnlyOneOrDefault();

            var lOtherAmount = 0.0m;

            if ( lToUnit != null && lToUnit.Exponent == 1
                && lFromUnit != null && lFromUnit.Exponent == 1 )
            {
                lOtherAmount = UnitConversion.convertWithOffset( other.mBaseAmount, lFromUnit.Unit.GetType(), lFromUnit.Prefix.GetType(), lToUnit.Unit.GetType(), lToUnit.Prefix.GetType() );

            }
            // ...otherwise do normal conversion.
            else
            {
                lOtherAmount = other.convert( this.BaseUnits ).mBaseAmount;
            }

            return this.mBaseAmount.CompareTo( lOtherAmount );
        }

        /// <summary>
        /// Compares this Quantity with the specified Quantity using the specified tolerance.
        /// </summary>
        /// <param name="aOther">
        /// The Quantity to compare to. This Quantity must be congruent to this Quantity. IF necessary,
        /// this parameter will be converted to the units of this Quantity in order to perform the
        /// comparison.
        /// </param>
        /// <param name="aTolerance">
        /// The tolerance to perform the comparison with.
        /// </param>
        /// <returns>
        /// If this Quantity is within the tolerance (inclusive) of the specified Quantity, 0 will be returned.
        /// If this Quantity is less than the specified Quantity and out of tolerance, -1 will be returned.
        /// If this Quantity is greater than the specified Quantity and out of tolerance, 1 will be returned.
        /// </returns>
        public int CompareToWithTolerance( Quantity aOther, Quantity aTolerance )
        {
            if ( !this.isCongruent( aOther ) || !this.isCongruent( aTolerance ) )
            {
                throw new InvalidOperationException( "Cannot compare incongruent quantities. Tolerance must also be congruent with compared quantities." );
            }

            var lLowerTolerance = this - aTolerance;
            var lUpperTolerance = this + aTolerance;

            if ( aOther < lLowerTolerance )
            {
                return 1;
            }
            else if ( aOther > lUpperTolerance )
            {
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Implements <see cref="IEquatable{T}.Equals(T)"/>.
        /// </summary>
        public bool Equals( Quantity other )
        {
            if ( other is null || !this.isCongruent( other ) )
            {
                return false;
            }

            // For offset units, attempt offset conversion iff there is exactly 
            // one unit in both vectors and all exponents are 1...
            var lToUnit = this.mBaseUnits.Components.Select( aItem => aItem.Value ).OnlyOneOrDefault();
            var lFromUnit = other.mBaseUnits.Components.Select( aItem => aItem.Value ).OnlyOneOrDefault();

            var lOtherAmount = 0.0m;

            if ( lToUnit != null && lToUnit.Exponent == 1
                && lFromUnit != null && lFromUnit.Exponent == 1 )
            {
                lOtherAmount = UnitConversion.convertWithOffset( other.mBaseAmount, lFromUnit.Unit.GetType(), lFromUnit.Prefix.GetType(), lToUnit.Unit.GetType(), lToUnit.Prefix.GetType() );
            }
            // ...otherwise do normal conversion.
            else
            {
                lOtherAmount = other.convert( this.BaseUnits ).mBaseAmount;
            }

            return this.mBaseAmount.Equals( lOtherAmount );
        }

        /// <summary>
        /// Overrides <see cref="Object.Equals(object)"/>.
        /// </summary>
        public override bool Equals( object obj )
        {
            if ( obj is Quantity lQuantity )
            {
                return this.Equals( lQuantity );
            }
            return false;
        }

        /// <summary>
        /// Operator overload for comparing if one quantity is less than another.
        /// </summary>
        /// <param name="aLeftOperand">
        /// The left-side operand.
        /// </param>
        /// <param name="aRightOperand">
        /// The right-side operand.
        /// </param>
        /// <returns>
        /// True if the left-side is less than the right-side. False otherwise.
        /// </returns>
        public static bool operator <( Quantity aLeftOperand, Quantity aRightOperand )
        {
            return aLeftOperand.CompareTo( aRightOperand ) < 0;
        }

        /// <summary>
        /// Operator overload for comparing if one quantity is less or equal to than another.
        /// </summary>
        /// <param name="aLeftOperand">
        /// The left-side operand.
        /// </param>
        /// <param name="aRightOperand">
        /// The right-side operand.
        /// </param>
        /// <returns>
        /// True if the left-side is less than or equal to the right-side. False otherwise.
        /// </returns>
        public static bool operator <=( Quantity aLeftOperand, Quantity aRightOperand )
        {
            return aLeftOperand.CompareTo( aRightOperand ) <= 0;
        }

        /// <summary>
        /// Operator overload for comparing if one quantity is greater than another.
        /// </summary>
        /// <param name="aLeftOperand">
        /// The left-side operand.
        /// </param>
        /// <param name="aRightOperand">
        /// The right-side operand.
        /// </param>
        /// <returns>
        /// True if the left-side is greater than the right-side. False otherwise.
        /// </returns>
        public static bool operator >( Quantity aLeftOperand, Quantity aRightOperand )
        {
            return aLeftOperand.CompareTo( aRightOperand ) > 0;
        }

        /// <summary>
        /// Operator overload for comparing if one quantity is equal to or greater than another.
        /// </summary>
        /// <param name="aLeftOperand">
        /// The left-side operand.
        /// </param>
        /// <param name="aRightOperand">
        /// The right-side operand.
        /// </param>
        /// <returns>
        /// True if the left-side is equal to or greater than the right-side. False otherwise.
        /// </returns>
        public static bool operator >=( Quantity aLeftOperand, Quantity aRightOperand )
        {
            return aLeftOperand.CompareTo( aRightOperand ) >= 0;
        }

        /// <summary>
        /// Operator overload for comparing if one quantity is equal to another.
        /// </summary>
        /// <param name="aLeftOperand">
        /// The left-side operand.
        /// </param>
        /// <param name="aRightOperand">
        /// The right-side operand.
        /// </param>
        /// <returns>
        /// True if the left-side is equal to the right-side. False otherwise.
        /// </returns>
        public static bool operator ==( Quantity aLeftOperand, Quantity aRightOperand )
        {
            if ( object.ReferenceEquals( aLeftOperand, null ) )
            {
                return object.ReferenceEquals( aRightOperand, null );
            }
            return aLeftOperand.Equals( aRightOperand );
        }

        /// <summary>
        /// Operator overload for comparing if one quantity is not equal to another.
        /// </summary>
        /// <param name="aLeftOperand">
        /// The left-side operand.
        /// </param>
        /// <param name="aRightOperand">
        /// The right-side operand.
        /// </param>
        /// <returns>
        /// True if the left-side is not equal to the right-side. False otherwise.
        /// </returns>
        public static bool operator !=( Quantity aLeftOperand, Quantity aRightOperand )
        {
            if ( object.ReferenceEquals( aLeftOperand, null ) )
            {
                return !object.ReferenceEquals( aRightOperand, null );
            }
            return !aLeftOperand.Equals( aRightOperand );
        }

        /// <summary>
        /// Overrides <see cref="object.GetHashCode"/>.
        /// </summary>
        public override int GetHashCode()
        {
            return mBaseAmount.GetHashCode() ^ mBaseUnits.GetHashCode();
        }

        /// <summary>
        /// Converts the provided <see cref="decimal"/> value from one unit to another.
        /// </summary>
        /// <param name="aFromAmount">
        /// The amount to convert.
        /// </param>
        /// <param name="aFromUnits">
        /// The original units of the amount.
        /// </param>
        /// <param name="aToUnits">
        /// The desired units of the amount.
        /// </param>
        /// <returns>
        /// The converted units.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if either unit string is null, empty or whitespace.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the provided units could not be parsed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the provided units are incongruent (cannot be converted between).
        /// </exception>
        /// <exception cref="AggregateException">
        /// Thrown when more than one exception occurs.
        /// </exception>
        public static decimal Convert( decimal aFromAmount, string aFromUnits, string aToUnits )
        {
            var lFromUnits = !string.IsNullOrWhiteSpace( aFromUnits ) ? aFromUnits : throw new ArgumentNullException( nameof( aFromUnits ) );
            var lToUnits = !string.IsNullOrWhiteSpace( aToUnits ) ? aToUnits : throw new ArgumentNullException( nameof( aToUnits ) );

            var lFromGood = DimensionVector.TryParse( lFromUnits, out var lFromVector );
            var lToGood = DimensionVector.TryParse( lToUnits, out var lToVector );

            if( lFromGood && lToGood && lFromVector.isCongruent(lToVector) )
            {
                return  new Quantity( aFromAmount, aFromUnits ).convert( aToUnits ).Amount;
            }
            else
            {
                var lExceptions = new List<Exception>();

                if( !lFromGood )
                {
                    lExceptions.Add( new ArgumentException( "Units could not be parsed.", nameof( aFromUnits ) ) );
                }

                if ( !lToGood )
                {
                    lExceptions.Add( new ArgumentException( "Units could not be parsed.", nameof( aToUnits ) ) );
                }

                if( lFromGood && lToGood )
                {
                    lExceptions.Add( new InvalidOperationException( "Provided units are not congruent." ) );
                }

                if( lExceptions.Count == 1 )
                {
                    throw lExceptions[0];
                }
                else
                {
                    throw new AggregateException( lExceptions );
                }
            }
        }

        /// <summary>
        /// Converts the provided <see cref="double"/> value from one unit to another.
        /// </summary>
        /// <param name="aFromAmount">
        /// The amount to convert.
        /// </param>
        /// <param name="aFromUnits">
        /// The original units of the amount.
        /// </param>
        /// <param name="aToUnits">
        /// The desired units of the amount.
        /// </param>
        /// <returns>
        /// The converted units.
        /// </returns>
        /// <remarks>
        /// Exceptions thrown are derived from <see cref="Quantity.Convert(decimal, string, string)"/>.
        /// </remarks>
        public static double Convert( double aFromAmount, string aFromUnits, string aToUnits )
        {
            return (double)Quantity.Convert( (decimal)aFromAmount, aFromUnits, aToUnits );
        }
    }
}
