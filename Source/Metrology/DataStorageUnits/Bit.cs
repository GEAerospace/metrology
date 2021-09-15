// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.DataStorageUnits
{
    public sealed class Bit : ISimpleUnit
    {
        public ISimpleUnit RelativeTo => UnitManager.getUnit<Byte>();
        public IDimension Dimension => UnitManager.getDimension<DataStorageDimension>();
        public decimal ScaledFromRelative => 8.0m;
        public string Name => "bit";
        public string Abbreviation => "b";
    }
}
