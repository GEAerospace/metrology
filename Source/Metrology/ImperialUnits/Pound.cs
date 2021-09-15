// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GEAviation.Metrology.MetricUnits;

namespace GEAviation.Metrology.ImperialUnits
{
    public sealed class Pound : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Gram>();
        public IDimension Dimension => UnitManager.getDimension<MassDimension>();
        public decimal ScaledFromRelative { get; } = 1 / 453.59237m;
        public string Name => "pound";
        public string Abbreviation => "lb";
    }
}
