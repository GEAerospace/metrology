// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.DerivedUnits
{
    public sealed class FeetPerSecond : IDerivedUnit
    {
        public string Name => "feet-per-second";
        public string Abbreviation => "fps";

        public DimensionVector BaseVector { get; } = "ft / s";
    }
}
