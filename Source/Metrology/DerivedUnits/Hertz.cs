// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.DerivedUnits
{
    public sealed class Hertz : IDerivedUnit
    {
        public string Name => "hertz";
        public string Abbreviation => "Hz";

        public DimensionVector BaseVector { get; } = "1 / s";
    }
}
