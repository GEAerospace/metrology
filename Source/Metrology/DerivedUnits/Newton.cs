// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.DerivedUnits
{
    public sealed class Newton : IDerivedUnit
    {
        public string Name => "newton";
        public string Abbreviation => "N";

        public DimensionVector BaseVector { get; } = "kg * m / s^2";
    }
}
