// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace GEAviation.Metrology.AngularUnits
{
    public sealed class Turn : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Radian>();
        public IDimension Dimension => UnitManager.getDimension<AngularDimension>();
        public decimal ScaledFromRelative { get; } = (decimal)( 1.0 / ( 2.0 * Math.PI ) );
        public string Name => "turn";
        public string Abbreviation => "turn";
    }
}
