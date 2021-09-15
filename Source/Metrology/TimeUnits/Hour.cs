// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GEAviation.Metrology.MetricUnits;

namespace GEAviation.Metrology.TimeUnits
{
    public sealed class Hour : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Second>();
        public IDimension Dimension => UnitManager.getDimension<TimeDimension>();
        public decimal ScaledFromRelative { get; } = 1.0m / 3600.0m;
        public string Name => "hour";
        public string Abbreviation => "hr";
    }
}
