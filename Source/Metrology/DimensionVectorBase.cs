// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace GEAviation.Metrology
{
    /// <summary>
    /// This class represents an immutable "vector" of unit components. As mathematical operations
    /// are performed, this vector keeps track of and perform unit conversion operations. 
    /// </summary>
    public abstract class DimensionVectorBase<TDimensionType, TUnitType, TVectorType>
        where TDimensionType : IDimension
        where TUnitType : IUnit
        where TVectorType : DimensionVectorBase<TDimensionType, TUnitType, TVectorType>
    {
        protected DimensionVectorBase()
        { }

        // Value is a tuple of the actual Unit class and the associated exponent
        // within the vector. This setup is to allow aligned/componentized vector operations later.
        protected Dictionary<TDimensionType, VectorComponent<TUnitType>> mComponents = new Dictionary<TDimensionType, VectorComponent<TUnitType>>();

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<TDimensionType, VectorComponent<TUnitType>> Components
        {
            get
            {
                return mComponents.ToDictionary( aKey => aKey.Key, aValue => new VectorComponent<TUnitType>( aValue.Value ) );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Dictionary<TDimensionType, VectorComponent<TUnitType>> copyComponents()
        {
            var lNewComponents = new Dictionary<TDimensionType, VectorComponent<TUnitType>>();
            foreach ( var lComponent in this.mComponents )
            {
                // Key and Value.Item1 are not being copied because they're supposed to be singleton
                // instances of dimension and unit classes. 
                lNewComponents[lComponent.Key] = new VectorComponent<TUnitType>( lComponent.Value.Prefix, lComponent.Value.Unit, lComponent.Value.Exponent );
            }
            return lNewComponents;
        }

        /// <summary>
        /// Copy constructor for private use. Whenever this class returns a new immutable
        /// instance of itself, it will need to copy then modify the copy.
        /// </summary>
        /// <param name="aToCopy">
        /// The <see cref="DimensionVector"/> to copy.
        /// </param>
        public DimensionVectorBase( DimensionVectorBase<TDimensionType, TUnitType, TVectorType> aToCopy )
        {
            mComponents = aToCopy.copyComponents();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract TVectorType copy();

        /// <summary>
        /// Inverts this Dimension Vector producing a new vector with all inverted exponents.
        /// </summary>
        /// <returns>
        /// The new <see cref="DimensionVector"/>.
        /// </returns>
        public virtual TVectorType invert()
        {
            var lNewVector = this.copy();

            foreach ( var lComponent in this.mComponents )
            {
                lNewVector.mComponents[lComponent.Key] = new VectorComponent<TUnitType>(lComponent.Value.Prefix, lComponent.Value.Unit, -lComponent.Value.Exponent );
            }

            return lNewVector;
        }

        /// <summary>
        /// This method checks if this vector is congruent (i.e. both contain the same set of
        /// dimensions with the same exponents).
        /// </summary>
        /// <param name="aVector"></param>
        /// <returns></returns>
        public bool isCongruent( TVectorType aVector )
        {
            if ( this.mComponents.Count != aVector.mComponents.Count )
            {
                return false;
            }

            foreach ( var lComponent in this.mComponents )
            {
                if ( !aVector.mComponents.ContainsKey( lComponent.Key ) )
                {
                    return false;
                }

                if ( aVector.mComponents[lComponent.Key].Exponent != lComponent.Value.Exponent )
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Calculates the conversion factor for going from one prefix to another.
        /// </summary>
        /// <param name="aFromComponent">
        /// The starting prefix.
        /// </param>
        /// <param name="aToComponent">
        /// The target prefix.
        /// </param>
        /// <returns>
        /// The conversion factor.
        /// </returns>
        public static decimal getPrefixConversionFactor( VectorComponent<TUnitType> aFromComponent, VectorComponent<TUnitType> aToComponent )
        {
            var lFromPrefixScale = aFromComponent.Prefix.Scale.decimalPow( aFromComponent.Exponent );
            var lToPrefixScale = aToComponent.Prefix.Scale.decimalPow( aToComponent.Exponent );

            return ( lFromPrefixScale / lToPrefixScale );
        }

        /// <summary>
        /// Default implementation of ToString. Creates a string representation of the components
        /// of the vector.
        /// </summary>
        /// <returns>
        /// The string representation of the vector.
        /// </returns>
        public override string ToString()
        {
            var lNumerator = mComponents.Where( aComponent => aComponent.Value.Exponent > 0 ).Select( aComponent => aComponent.Value );
            var lDenominator = mComponents.Where( aComponent => aComponent.Value.Exponent < 0 ).Select( aComponent => aComponent.Value );

            var lNumeratorString = string.Join( "\u22C5", lNumerator.Select( aItem => $"{aItem.Prefix.Abbreviation}{aItem.Unit.Abbreviation}^{aItem.Exponent}" ) );
            var lDenominatorString = "/" + string.Join( "\u22C5", lDenominator.Select( aItem => $"{aItem.Prefix.Abbreviation}{aItem.Unit.Abbreviation}^{-aItem.Exponent}" ) );

            if ( string.IsNullOrWhiteSpace( lNumeratorString ) ) lNumeratorString = "1";
            if ( lDenominatorString == "/" ) lDenominatorString = "";

            return $"{lNumeratorString}{lDenominatorString}".Replace( "^1", "" );
        }
    }
}
