// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GEAviation.Metrology.MetricUnits;

namespace GEAviation.Metrology.EsotericUnits
{
    public sealed class Smoot : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Meter>();
        public IDimension Dimension => UnitManager.getDimension<LengthDimension>();
        public decimal ScaledFromRelative => 1.0m / 1.702m;
        public string Name => "smoot";
        public string Abbreviation => "sm";
    }
}
