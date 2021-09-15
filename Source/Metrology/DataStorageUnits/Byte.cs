// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GEAviation.Metrology.MetricUnits;

namespace GEAviation.Metrology.DataStorageUnits
{
    public sealed class Byte : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => this;
        public IDimension Dimension => UnitManager.getDimension<DataStorageDimension>();
        public decimal ScaledFromRelative => 1.0m;
        public string Name => "byte";
        public string Abbreviation => "B";
    }
}
