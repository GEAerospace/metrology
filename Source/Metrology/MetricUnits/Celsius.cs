// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.MetricUnits
{
    public sealed class Celsius : ISimpleUnit, IOffsetUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Kelvin>();
        public IDimension Dimension => UnitManager.getDimension<TemperatureDimension>();

        // Celsius, like fahrenheit, is scaled identically to it's "base" unit, in this
        // case Kelvin. However, it sits at an offset. So the Scale is 1, and through an 
        // additional interface, the offset will applied in certain situations.
        public decimal ScaledFromRelative => 1.0m;
        public string Name => "celsius";
        public string Abbreviation => "C";

        public decimal offsetFromRelativeUnit( decimal aValue ) => aValue - 273.15m;
        public decimal offsetToRelativeUnit( decimal aValue ) => aValue + 273.15m;
    }
}
