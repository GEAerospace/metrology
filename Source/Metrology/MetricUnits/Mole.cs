// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.MetricUnits
{
    public sealed class Mole : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => this;
        public IDimension Dimension => UnitManager.getDimension<AmountOfSubstanceDimension>();
        public decimal ScaledFromRelative => 1.0m;
        public string Name => "mole";
        public string Abbreviation => "mol";
    }
}
