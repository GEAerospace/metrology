// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.ImperialUnits
{
    public sealed class Mile : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Yard>();
        public IDimension Dimension => UnitManager.getDimension<LengthDimension>();
        public decimal ScaledFromRelative { get; } = 1m / 1760m;
        public string Name => "mile";
        public string Abbreviation => "mi";
    }
}
