// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace GEAviation.Metrology
{
    /// <summary>
    /// 
    /// </summary>
    public class DerivedUnitVector : DimensionVectorBase<IDimension, IDerivedUnit, DerivedUnitVector>
    {
        /// <summary>
        /// 
        /// </summary>
        private DerivedUnitVector()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aToCopy"></param>
        public DerivedUnitVector( DerivedUnitVector aToCopy )
            : base( aToCopy )
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DerivedUnitVector copy()
        {
            return new DerivedUnitVector( this );
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual decimal PrefixFactor
        {
            get
            {
                decimal lFinalFactor = 1.0m;
                foreach ( var lComponent in mComponents )
                {
                    lFinalFactor *= lComponent.Value.Prefix.Scale.decimalPow( lComponent.Value.Exponent );
                }
                return lFinalFactor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aVector"></param>
        /// <returns></returns>
        public virtual decimal convert( DerivedUnitVector aVector )
        {
            if ( !isCongruent( aVector ) )
            {
                throw new InvalidOperationException( "Cannot convert provided vector to the same units as this vector. They are incongruent." );
            }

            var lNewVector = new DerivedUnitVector( this );

            decimal lConversionFactor = 1.0m;

            foreach ( var lComponent in lNewVector.mComponents )
            {
                // Congruency (established above) provides an invariance related to both sets of mComponents having the same exact
                // set of keys. As a result, there's no reason to check if keys exist during this part of the
                // operation.

                var lOtherComponent = aVector.mComponents[lComponent.Key];

                if ( lComponent.Value.Prefix != lOtherComponent.Prefix )
                {
                    var lPrefixFactor = getPrefixConversionFactor( lOtherComponent, lComponent.Value );
                    lPrefixFactor = lOtherComponent.Exponent < 0 ? 1.0m / lPrefixFactor : lPrefixFactor;
                    lConversionFactor *= lPrefixFactor;
                }
            }

            return lConversionFactor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aVector"></param>
        /// <param name="aConversionFactor"></param>
        /// <returns></returns>
        public virtual DerivedUnitVector multiply( DerivedUnitVector aVector, out decimal aConversionFactor )
        {
            var lNewVector = new DerivedUnitVector( this );

            aConversionFactor = 1.0m;

            foreach ( var lComponent in aVector.Components )
            {
                var lTargetUnit = lComponent.Value.Unit;
                var lTargetPrefix = lComponent.Value.Prefix;
                var lNewPower = lComponent.Value.Exponent;

                if ( lNewVector.mComponents.ContainsKey( lComponent.Key ) )
                {
                    var lPrevious = lNewVector.mComponents[lComponent.Key];

                    if ( lPrevious.Prefix != lComponent.Value.Prefix )
                    {
                        aConversionFactor *= getPrefixConversionFactor( lComponent.Value, lPrevious );
                    }

                    lNewPower += lPrevious.Exponent;
                }

                lNewVector.mComponents[lTargetUnit] = new VectorComponent<IDerivedUnit>( lTargetPrefix, lTargetUnit, lNewPower );
            }

            return lNewVector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aVector"></param>
        /// <param name="aConversionFactor"></param>
        /// <returns></returns>
        public virtual DerivedUnitVector divide( DerivedUnitVector aVector, out decimal aConversionFactor )
        {
            var lInverseOperand = aVector.invert();
            return multiply( lInverseOperand, out aConversionFactor );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aToParse"></param>
        /// <param name="aResult"></param>
        /// <returns></returns>
        public static bool TryParse( string aToParse, out DerivedUnitVector aResult )
        {
            aResult = null;

            if ( aToParse.TryParseUnitString<IDerivedUnit>( out var lComponents ) )
            {
                aResult = create( lComponents );
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aComponents"></param>
        /// <returns></returns>
        private static DerivedUnitVector create( IEnumerable<VectorComponent<IDerivedUnit>> aComponents )
        {
            var lVector = new DerivedUnitVector();

            foreach ( var lComponent in aComponents )
            {
                if ( lComponent.Exponent != 0 )
                {
                    var lNewVectorComponent = new VectorComponent<IDerivedUnit>( lComponent );
                    lVector.mComponents[lNewVectorComponent.Unit] = lNewVectorComponent;
                }
            }

            return lVector;
        }
    }
}
