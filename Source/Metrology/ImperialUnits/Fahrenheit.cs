// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.ImperialUnits
{
    public sealed class Fahrenheit : ISimpleUnit, IOffsetUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Rankine>();
        public IDimension Dimension => UnitManager.getDimension<TemperatureDimension>();

        // Fahrenheit, like celsius, is scaled identically to it's "base" unit, in this
        // case Rankine. However, it sits at an offset. So the Scale is 1, and through an 
        // additional interface, the offset will applied in certain situations.
        public decimal ScaledFromRelative => 1.0m;
        public string Name => "fahrenheit";
        public string Abbreviation => "F";

        public decimal offsetFromRelativeUnit( decimal aValue ) => aValue - 459.67m;
        public decimal offsetToRelativeUnit( decimal aValue ) => aValue + 459.67m;
    }
}
