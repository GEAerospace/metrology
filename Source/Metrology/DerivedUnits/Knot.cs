// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.DerivedUnits
{
    public sealed class Knot : IDerivedUnit
    {
        public string Name => "knot";
        public string Abbreviation => "kts";

        public DimensionVector BaseVector { get; } = "NM / hr";
    }
}
