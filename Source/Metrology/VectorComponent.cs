// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology
{
    /// <summary>
    /// This class is used to contain information for each component of a DimensionVector.
    /// </summary>
    public class VectorComponent<TUnitType> where TUnitType : IUnit
    {
        public UnitWithPrefix<TUnitType> UnitAndPrefix { get; } = new UnitWithPrefix<TUnitType>();

        public VectorComponent( VectorComponent<TUnitType> aToCopy )
        {
            UnitAndPrefix.Unit = aToCopy.Unit;
            UnitAndPrefix.Prefix = aToCopy.Prefix;
            Exponent = aToCopy.Exponent;
        }

        public VectorComponent( IPrefix aPrefix, TUnitType aUnit, int aExponent )
        {
            UnitAndPrefix.Unit = aUnit;
            UnitAndPrefix.Prefix = aPrefix;
            Exponent = aExponent;
        }

        public VectorComponent( TUnitType aUnit, int aExponent )
            : this( UnitManager.getPrefix<NoPrefix>(), aUnit, aExponent )
        { }

        public TUnitType Unit { get => UnitAndPrefix.Unit; }
        public IPrefix Prefix { get => UnitAndPrefix.Prefix; }
        public int Exponent { get; private set; }

        public override int GetHashCode()
        {
            int lHash = Exponent.GetHashCode();
            lHash ^= Unit.GetHashCode();
            lHash ^= Prefix.GetHashCode();
            return lHash;
        }
    }
}
