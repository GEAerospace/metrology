// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.ImperialUnits
{
    public sealed class Inch : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Foot>();
        public IDimension Dimension => UnitManager.getDimension<LengthDimension>();
        public decimal ScaledFromRelative => 12.0m;
        public string Name => "inch";
        public string Abbreviation => "in";
    }
}
