// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.ImperialUnits
{
    public sealed class Ounce : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Pound>();
        public IDimension Dimension => UnitManager.getDimension<MassDimension>();
        public decimal ScaledFromRelative => 16m;
        public string Name => "ounce";
        public string Abbreviation => "oz";
    }
}
