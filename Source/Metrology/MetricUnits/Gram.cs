// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.MetricUnits
{
    public sealed class Gram : ISimpleUnit
    {
        // While the actual base SI unit for mass is the kilogram, it made it
        // more consistent to make this base the gram, otherwise it
        // would be the only base pre-scaled to a quantity prefix (e.g. kilo-, mega-).
        public ISimpleUnit RelativeTo => this;
        public IDimension Dimension => UnitManager.getDimension<MassDimension>();
        public decimal ScaledFromRelative => 1.0m;
        public string Name => "gram";
        public string Abbreviation => "g";
    }
}
