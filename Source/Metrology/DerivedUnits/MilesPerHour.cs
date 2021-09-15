// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.DerivedUnits
{
    public sealed class MilesPerHour : IDerivedUnit
    {
        public string Name => "miles-per-hour";
        public string Abbreviation => "mph";

        public DimensionVector BaseVector { get; } = "mi/hr";
    }
}
