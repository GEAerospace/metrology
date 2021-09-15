// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace GEAviation.Metrology
{
    internal static class UnitConversion
    {
        public static decimal convert( Type aFromUnit, Type aToUnit )
        {
            // Find "paths" to each side's base unit.
            var lUnitPathFrom = getUnitBasePath( aFromUnit );
            var lUnitPathTo = getUnitBasePath( aToUnit );

            ISimpleUnit lCommonBase = null;

            foreach ( var lFromElement in lUnitPathFrom )
            {
                foreach ( var lToElement in lUnitPathTo )
                {
                    if ( lFromElement == lToElement )
                    {
                        lCommonBase = lToElement;
                        goto loop_break;
                    }
                }
            }

        loop_break:
            if ( lCommonBase == null )
            {
                throw new InvalidOperationException( $"No conversion exists between {aFromUnit.Name} and {aToUnit.Name}, either directly or through related units." );
            }

            var lFactor = 1.0m;
            var lFromBaseIndex = lUnitPathFrom.IndexOf( lCommonBase );
            var lToBaseIndex = lUnitPathTo.IndexOf( lCommonBase );

            var lLongestPathLength = Math.Max( lFromBaseIndex, lToBaseIndex );

            for ( var lIndex = 0; lIndex <= lLongestPathLength; lIndex++ )
            {
                var lFromFactor = lIndex <= lFromBaseIndex ? lUnitPathFrom[lIndex].ScaledFromRelative : 1.0m;
                var lToFactor = lIndex <= lToBaseIndex ? lUnitPathTo[lIndex].ScaledFromRelative : 1.0m;

                lFactor *= ( lToFactor / lFromFactor );
            }

            return lFactor;
        }

        public static decimal convertWithOffset( decimal aInitialValue, Type aFromUnit, Type aFromPrefix, Type aToUnit, Type aToPrefix )
        {
            decimal lNewValue = aInitialValue;

            var lFromPrefix = UnitManager.getPrefix( aFromPrefix );
            var lToPrefix = UnitManager.getPrefix( aToPrefix );

            if ( !( lFromPrefix is NoPrefix ) )
            {
                lNewValue *= lFromPrefix.Scale;
            }

            if ( typeof( IOffsetUnit ).IsAssignableFrom( aFromUnit ) )
            {
                var lFromUnit = UnitManager.getUnit( aFromUnit ) as IOffsetUnit;
                lNewValue = lFromUnit.offsetToRelativeUnit( lNewValue );
            }

            lNewValue *= convert( aFromUnit, aToUnit );

            if ( typeof( IOffsetUnit ).IsAssignableFrom( aToUnit ) )
            {
                var lToUnit = UnitManager.getUnit( aToUnit ) as IOffsetUnit;
                lNewValue = lToUnit.offsetFromRelativeUnit( lNewValue );
            }

            if ( !( lToPrefix is NoPrefix ) )
            {
                lNewValue /= lToPrefix.Scale;
            }

            return lNewValue;
        }

        private static List<ISimpleUnit> getUnitBasePath( Type aUnitType )
        {
            var lList = new List<ISimpleUnit>();
            var lCurrentUnit = UnitManager.getUnit( aUnitType ) as ISimpleUnit;

            while ( lCurrentUnit.RelativeTo != lCurrentUnit )
            {
                lList.Add( lCurrentUnit );
                lCurrentUnit = lCurrentUnit.RelativeTo;
            }

            lList.Add( lCurrentUnit );

            return lList;
        }
    }
}
