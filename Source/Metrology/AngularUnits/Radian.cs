// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.AngularUnits
{
    public sealed class Radian : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => this;
        public IDimension Dimension => UnitManager.getDimension<AngularDimension>();
        public decimal ScaledFromRelative => 1.0m;
        public string Name => "radian";
        public string Abbreviation => "rad";
    }
}
