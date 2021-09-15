// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GEAviation.Metrology.MetricUnits;

namespace GEAviation.Metrology.ImperialUnits
{
    public sealed class Rankine : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Kelvin>();
        public IDimension Dimension => UnitManager.getDimension<TemperatureDimension>();
        public decimal ScaledFromRelative => 1.8m;
        public string Name => "rankine";
        public string Abbreviation => "R";
    }
}
