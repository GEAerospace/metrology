// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace GEAviation.Metrology
{
    public static class UnitExtensions
    {

        public static IDimension getDimension( this IUnit aUnit )
        {
            switch ( aUnit )
            {
                case ISimpleUnit lSimple:
                    return lSimple.Dimension;

                case IDerivedUnit lDerived:
                    return lDerived;
            }

            return null;
        }

        public static IEnumerable<VectorComponent<TUnitOut>> getSpecificComponents<TUnitIn, TUnitOut>( this IEnumerable<VectorComponent<TUnitIn>> aComponents )
            where TUnitIn : class, IUnit
            where TUnitOut : class, IUnit
        {
            return aComponents.Where( aComponent => aComponent.Unit is TUnitOut )
                              .Select( aComponent => new VectorComponent<TUnitOut>( aComponent.Prefix,
                                                                                    aComponent.Unit as TUnitOut,
                                                                                    aComponent.Exponent ) );
        }

        public static string ToUnitString<TUnit>( this IEnumerable<VectorComponent<TUnit>> aGenericVector )
            where TUnit : class, IUnit
        {
            var lNumerator = aGenericVector.Where( aComponent => aComponent.Exponent > 0 );
            var lDenominator = aGenericVector.Where( aComponent => aComponent.Exponent < 0 );

            var lNumeratorString = string.Join( "\u22C5", lNumerator.Select( aItem => $"{aItem.Prefix.Abbreviation}{aItem.Unit.Abbreviation}^{aItem.Exponent}" ) );
            var lDenominatorString = "/" + string.Join( "\u22C5", lDenominator.Select( aItem => $"{aItem.Prefix.Abbreviation}{aItem.Unit.Abbreviation}^{-aItem.Exponent}" ) );

            if ( string.IsNullOrWhiteSpace( lNumeratorString ) ) lNumeratorString = "1";
            if ( lDenominatorString == "/" ) lDenominatorString = "";

            var lFinalString = $"{lNumeratorString}{lDenominatorString}".Replace( "^1", "" ).Trim();
            if ( lFinalString == "1" ) lFinalString = String.Empty;

            return lFinalString;
        }

        public static bool TryParseUnitString<TUnitType>( this string aToParse, out IEnumerable<VectorComponent<TUnitType>> aResult )
            where TUnitType : IUnit
        {
            IEnumerable<VectorComponent<TUnitType>> subParse( string aPartial, bool aIsDenominator )
            {
                var lParts = aPartial.Split( new char[] { '*', '\u22c5' } );
                var lToReturn = new List<VectorComponent<TUnitType>>();

                if ( aPartial.Trim() == "1" ) return lToReturn;

                foreach ( var lPart in lParts )
                {
                    var lUnitAndExponent = lPart.Split( '^' );

                    var lAbbreviation = lUnitAndExponent[0].Trim();
                    var lExponent = lUnitAndExponent.Length == 2 ? lUnitAndExponent[1] : "1";

                    if ( string.IsNullOrWhiteSpace( lAbbreviation ) )
                    {
                        return null;
                    }

                    if ( !int.TryParse( lExponent, out var lExponentAsInt ) )
                    {
                        return null;
                    }

                    var lFoundUnit = UnitManager.lookupUnitAndPrefix<TUnitType>( lAbbreviation );

                    if ( lFoundUnit == null )
                    {
                        return null;
                    }

                    lToReturn.Add( new VectorComponent<TUnitType>( lFoundUnit.Prefix, lFoundUnit.Unit, aIsDenominator ? -lExponentAsInt : lExponentAsInt ) );
                }

                return lToReturn;
            }

            bool hasDuplicateDimensions( IEnumerable<VectorComponent<TUnitType>> aComponents )
            {
                return aComponents.GroupBy( aItem => aItem.Unit.getDimension() ).Any( aItem => aItem.Count() > 1 );
            }

            aResult = null;

            if( aToParse.Trim() == String.Empty )
            {
                aResult = Enumerable.Empty<VectorComponent<TUnitType>>();
                return true;
            }

            var lInitialSplit = aToParse.Split( '/' );

            var lComponents = new List<VectorComponent<TUnitType>>();

            var lNumerator = subParse( lInitialSplit[0], false );
            var lDenominator = subParse( lInitialSplit.Length == 2 ? lInitialSplit[1] : "1", true );

            if ( lNumerator == null || lDenominator == null )
            {
                return false;
            }

            if ( hasDuplicateDimensions( lNumerator ) || hasDuplicateDimensions( lDenominator ) )
            {
                return false;
            }

            lComponents.AddRange( lNumerator );
            lComponents.AddRange( lDenominator );

            aResult = lComponents;
            return true;
        }

        public static IEnumerable<VectorComponent<IUnit>> ApplyDerivedUnits( this DimensionVector aBaseVector, IEnumerable<VectorComponent<IDerivedUnit>> aDerivedComponents, out decimal aConversionFactor )
        {
            aConversionFactor = 1.0m;

            var lBaseComponents = aBaseVector;
            var lNewVector = new List<VectorComponent<IUnit>>();

            foreach ( var lDerivedComponent in aDerivedComponents )
            {
                // Check for multiplicity/fit in original vector
                var lFactorList = new List<double>();

                foreach(var lUnit in lDerivedComponent.Unit.BaseVector.Components)
                {
                    var lMatchingComponent = lBaseComponents.Components.Where( aComponent => aComponent.Value.Unit == lUnit.Value.Unit && aComponent.Value.Prefix == lUnit.Value.Prefix )
                                                                       .Select( aComponent => aComponent.Value )
                                                                       .SingleOrDefault();

                    if(lMatchingComponent == null)
                    {
                        lFactorList.Add( 0.0 );
                        continue;
                    }

                    lFactorList.Add( lMatchingComponent.Exponent / lUnit.Value.Exponent );
                }

                var lNumberFit = lFactorList.Select( aFactor => (int)Math.Floor( Math.Abs( aFactor ) ) ).Min();
                var lMultiply = lFactorList.All( aFactor => aFactor < 0.0 );
                var lDivide = lFactorList.All( aFactor => aFactor > 0.0 );

                if(lNumberFit >= 1 && (lMultiply ^ lDivide) )
                {
                    var lOperand = DimensionVector.Pow( lDerivedComponent.Unit.BaseVector, lNumberFit );
                    var lPrefixValue = lDerivedComponent.Prefix.Scale.decimalPow( lNumberFit );

                    if(lMultiply)
                    {
                        lBaseComponents = lBaseComponents.multiply( lOperand, out var lFactor );
                        aConversionFactor *= lFactor;
                        aConversionFactor *= lPrefixValue;
                    }
                    else
                    {
                        lBaseComponents = lBaseComponents.divide( lOperand, out var lFactor );
                        aConversionFactor *= lFactor;
                        aConversionFactor /= lPrefixValue;
                    }

                    var lNewExponent = lDivide ? lNumberFit : -lNumberFit;
                    lNewVector.Add( new VectorComponent<IUnit>( lDerivedComponent.Prefix, lDerivedComponent.Unit, lNewExponent ) );
                }
            }

            lNewVector.AddRange( lBaseComponents.Components.Select( aComponent => new VectorComponent<IUnit>( aComponent.Value.Prefix, aComponent.Value.Unit, aComponent.Value.Exponent ) ) );

            return lNewVector;
        }
    }
}

namespace GEAviation.Metrology
{
    internal static class MathExtensions
    {
        /// <summary>
        /// Calculates powers of decimal values, since <see cref="Math.Pow(double, double)"/> doesn't include
        /// an overload for <see cref="decimal"/> types.
        /// </summary>
        /// <param name="aValue">
        /// The decimal value to raise to a power.
        /// </param>
        /// <param name="aPower">
        /// The power to raise the decimal by.
        /// </param>
        /// <returns>
        /// The decimal value resulting from the operation.
        /// </returns>
        public static decimal decimalPow( this decimal aValue, int aPower )
        {
            return (decimal)Math.Pow( (double)aValue, aPower );
        }
    }

    internal static class InternalExtensions
    {
        public static T OnlyOneOrDefault<T>( this IEnumerable<T> aEnumerable )
            where T : class
        {
            var lFirst = aEnumerable.FirstOrDefault();
            var lSecond = aEnumerable.Skip( 1 ).FirstOrDefault();

            if( lFirst != null && lSecond == null )
            {
                return lFirst;
            }

            return default( T );
        }
    }
}