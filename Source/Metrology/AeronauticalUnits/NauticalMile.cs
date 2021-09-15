// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GEAviation.Metrology.MetricUnits;

namespace GEAviation.Metrology.AeronauticalUnits
{
    public sealed class NauticalMile : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Meter>();
        public IDimension Dimension => UnitManager.getDimension<LengthDimension>();
        public decimal ScaledFromRelative => 1 / 1852m;
        public string Name => "nautical mile";
        public string Abbreviation => "NM";
    }
}
