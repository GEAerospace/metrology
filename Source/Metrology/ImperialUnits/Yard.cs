// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.ImperialUnits
{
    public sealed class Yard : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<MetricUnits.Meter>();
        public IDimension Dimension => UnitManager.getDimension<LengthDimension>();
        public decimal ScaledFromRelative { get; } = ( 1250m / 381m ) / 3m;
        public string Name => "yard";
        public string Abbreviation => "yd";
    }
}
