// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.DerivedUnits
{
    public sealed class Pascal : IDerivedUnit
    {
        public string Name => "pascal";
        public string Abbreviation => "Pa";

        public DimensionVector BaseVector { get; } = "kg / m * s^2";
    }
}
