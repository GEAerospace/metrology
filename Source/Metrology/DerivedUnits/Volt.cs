// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.DerivedUnits
{
    public sealed class Volt : IDerivedUnit
    {
        public string Name => "volt";
        public string Abbreviation => "V";

        public DimensionVector BaseVector { get; } = "kg * m^2 / s^3 * A";
    }
}
