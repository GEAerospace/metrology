// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.MetricUnits
{
    public sealed class Kelvin : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => this;
        public IDimension Dimension => UnitManager.getDimension<TemperatureDimension>();
        public decimal ScaledFromRelative => 1.0m;
        public string Name => "kelvin";
        public string Abbreviation => "K";
    }
}
