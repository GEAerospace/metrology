// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology.DerivedUnits
{
    public sealed class PoundForce : IDerivedUnit
    {
        public string Name => "pound-force";
        public string Abbreviation => "lbf";

        public DimensionVector BaseVector { get; } = "slug * ft / s^2";
    }
}
