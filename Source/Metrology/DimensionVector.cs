// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace GEAviation.Metrology
{
    /// <summary>
    /// 
    /// </summary>
    public class DimensionVector : DimensionVectorBase<IDimension, ISimpleUnit, DimensionVector>
    {
        public static DimensionVector Zero { get; } = new DimensionVector();

        /// <summary>
        /// 
        /// </summary>
        private DimensionVector()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aToCopy"></param>
        public DimensionVector( DimensionVector aToCopy )
            : base( aToCopy )
        { }

        /// <summary>
        /// Multiplies this Dimension Vector with the specified vector, producing a new
        /// vector. The <paramref name="aConversionFactor"/> returned should be applied
        /// to any associated quantities to compensate for unit conversions that may
        /// have taken place.
        /// </summary>
        /// <param name="aVector">
        /// The vector to multiply with this vector.
        /// </param>
        /// <param name="aConversionFactor">
        /// The conversion factor produced by the multiply operation.
        /// </param>
        /// <returns>
        /// The new <see cref="DimensionVector"/>.
        /// </returns>
        public virtual DimensionVector multiply( DimensionVector aVector, out decimal aConversionFactor )
        {
            var lNewVector = new DimensionVector( this );

            aConversionFactor = 1.0m;

            foreach ( var lComponent in aVector.Components )
            {
                var lTargetUnit = lComponent.Value.Unit;
                var lTargetPrefix = lComponent.Value.Prefix;
                var lSourceUnit = lComponent.Value.Unit;
                var lNewPower = lComponent.Value.Exponent;

                if ( lNewVector.mComponents.ContainsKey( lComponent.Key ) )
                {
                    var lPrevious = lNewVector.mComponents[lComponent.Key];
                    lTargetUnit = lPrevious.Unit;
                    lTargetPrefix = lPrevious.Prefix;
                    if ( lTargetUnit != lSourceUnit )
                    {
                        var lFactor = UnitConversion.convert( lSourceUnit.GetType(), lTargetUnit.GetType() );
                        aConversionFactor *= lFactor.decimalPow( lNewPower );
                    }
                    if ( lPrevious.Prefix != lComponent.Value.Prefix )
                    {
                        aConversionFactor *= getPrefixConversionFactor( lComponent.Value, lPrevious );
                    }
                    lNewPower += lPrevious.Exponent;
                }

                lNewVector.mComponents[lTargetUnit.Dimension] = new VectorComponent<ISimpleUnit>( lTargetPrefix, lTargetUnit, lNewPower );
            }

            var lZeroComponents = lNewVector.mComponents.Where( aItem => aItem.Value.Exponent == 0 ).ToList();

            foreach(var lZeroComponent in lZeroComponents)
            {
                lNewVector.mComponents.Remove( lZeroComponent.Key );
            }

            return lNewVector;
        }


        /// <summary>
        /// Divides this Dimension Vector by the specified vector, producing a new
        /// vector. The <paramref name="aConversionFactor"/> returned should be applied
        /// to any associated quantities to compensate for unit conversions that may
        /// have taken place.
        /// </summary>
        /// <param name="aVector">
        /// The vector to divide this vector by.
        /// </param>
        /// <param name="aConversionFactor">
        /// The conversion factor produced by the division operation.
        /// </param>
        /// <returns>
        /// The new <see cref="DimensionVector"/>.
        /// </returns>
        public virtual DimensionVector divide( DimensionVector aVector, out decimal aConversionFactor )
        {
            var lInverseOperand = aVector.invert();
            return multiply( lInverseOperand, out aConversionFactor );
        }

        /// <summary>
        /// Calculates the conversion factor needed to convert the provided vector to the
        /// same units as this vector. This requires both vectors to be congruent.
        /// </summary>
        /// <param name="aVector">
        /// The vector to calculate a conversion factor for.
        /// </param>
        /// <returns>
        /// The conversion factor.
        /// </returns>
        public virtual decimal convert( DimensionVector aVector )
        {
            if ( !isCongruent( aVector ) )
            {
                throw new InvalidOperationException( "Cannot convert provided vector to the same units as this vector. They are incongruent." );
            }

            var lNewVector = new DimensionVector( this );

            decimal lConversionFactor = 1.0m;

            foreach ( var lComponent in lNewVector.mComponents )
            {
                // Congruency (established above) provides an invariance related to both sets of mComponents having the same exact
                // set of keys. As a result, there's no reason to check if keys exist during this part of the
                // operation.

                var lOtherComponent = aVector.mComponents[lComponent.Key];
                if ( lComponent.Value.Unit != lOtherComponent.Unit )
                {
                    var lFactor = UnitConversion.convert( lOtherComponent.Unit.GetType(), lComponent.Value.Unit.GetType() );
                    lConversionFactor *= lFactor.decimalPow( lComponent.Value.Exponent );
                }

                if ( lComponent.Value.Prefix != lOtherComponent.Prefix )
                {
                    lConversionFactor *= DimensionVector.getPrefixConversionFactor( lOtherComponent, lComponent.Value );
                }
            }

            return lConversionFactor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DimensionVector copy()
        {
            return new DimensionVector( this );
        }

        /// <summary>
        /// Parses a string that represents a unit vector into a Dimension Vector.
        /// </summary>
        /// <param name="aToParse">
        /// The string representation of the vector to parse.
        /// </param>
        /// <param name="aResult">
        /// The resulting DimensionVector if the parsing was successful. Null otherwise.
        /// </param>
        /// <returns>
        /// True if parsing was successful, false otherwise.
        /// </returns>
        public static bool TryParse( string aToParse, out DimensionVector aResult )
        {
            aResult = null;

            if ( aToParse.TryParseUnitString<ISimpleUnit>( out var lComponents ) )
            {
                aResult = create( lComponents );
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aPrefixes"></param>
        /// <param name="aUnits"></param>
        /// <param name="aPowers"></param>
        /// <returns></returns>
        public static DimensionVector create( IEnumerable<VectorComponent<ISimpleUnit>> aComponents )
        {
            var lVector = new DimensionVector();

            foreach ( var lComponent in aComponents )
            {
                if ( lComponent.Exponent != 0 )
                {
                    var lNewVectorComponent = new VectorComponent<ISimpleUnit>( lComponent );
                    lVector.mComponents[lNewVectorComponent.Unit.Dimension] = lNewVectorComponent;
                }
            }

            return lVector;
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
        public static implicit operator DimensionVector( string aLiteral )
        {
            if ( !DimensionVector.TryParse( aLiteral, out var lVector ) )
            {
                throw new InvalidOperationException( "Could not parse vector string. Please check the format." );
            }
            return lVector;
        }

        /// <summary>
        /// Performs a power/exponent operation on a vector.
        /// </summary>
        /// <param name="aOperand">
        /// The vector to perform the operation on.
        /// </param>
        /// <param name="aExponent">
        /// The desired exponent to raise the vector to.
        /// </param>
        /// <returns></returns>
        public static DimensionVector Pow( DimensionVector aOperand, int aExponent )
        {
            if ( aExponent == 0 ) return DimensionVector.Zero;

            var lVector = aOperand;
            if ( aExponent < 0 ) lVector = lVector.invert();

            var lIterations = Math.Abs( aExponent ) - 1;

            for( int lIteration = 0; lIteration < lIterations; lIteration++ )
            {
                lVector = lVector.multiply( lVector, out var _ );
            }

            return lVector;
        }
    }
}
