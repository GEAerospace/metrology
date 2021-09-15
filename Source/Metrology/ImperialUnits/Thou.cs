// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.ImperialUnits
{
    public sealed class Thou : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Inch>();
        public IDimension Dimension => UnitManager.getDimension<LengthDimension>();
        public decimal ScaledFromRelative => 1000.0m;
        public string Name => "thou";
        public string Abbreviation => "mil";
    }
}
