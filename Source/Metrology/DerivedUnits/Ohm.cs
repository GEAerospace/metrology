// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.DerivedUnits
{
    public sealed class Ohm : IDerivedUnit
    {
        public string Name => "ohm";
        public string Abbreviation => "Ω";

        public DimensionVector BaseVector { get; } = "kg * m^2 / s^3 * A^2";
    }
}
