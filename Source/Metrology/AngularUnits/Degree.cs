// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.AngularUnits
{
    public sealed class Degree : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Turn>();
        public IDimension Dimension => UnitManager.getDimension<AngularDimension>();
        public decimal ScaledFromRelative { get; } = 360.0m;
        public string Name => "degree";
        public string Abbreviation => "deg";
    }
}
