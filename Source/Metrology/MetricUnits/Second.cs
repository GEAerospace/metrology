// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.MetricUnits
{
    public sealed class Second : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => this;
        public IDimension Dimension => UnitManager.getDimension<TimeDimension>();
        public decimal ScaledFromRelative => 1.0m;
        public string Name => "second";
        public string Abbreviation => "s";
    }
}
