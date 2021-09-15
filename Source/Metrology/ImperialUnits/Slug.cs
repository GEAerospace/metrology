// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.ImperialUnits
{
    public sealed class Slug : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Pound>();
        public IDimension Dimension => UnitManager.getDimension<MassDimension>();
        public decimal ScaledFromRelative { get; } = 1 / 32.1740m;
        public string Name => "slug";
        public string Abbreviation => "slug";
    }
}
