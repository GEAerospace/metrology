// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.ImperialUnits
{
    public sealed class Foot : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Yard>();
        public IDimension Dimension => UnitManager.getDimension<LengthDimension>();
        public decimal ScaledFromRelative => 3m;
        public string Name => "foot";
        public string Abbreviation => "ft";
    }
}
