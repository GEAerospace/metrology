// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology
{
    /// <summary>
    /// This interface represents measurement dimensions (e.g. Length or Temperature).
    /// </summary>
    public interface IDimension { }

    /// <summary>
    /// This interface represents measurement scale prefixes (e.g. Kilo, Mega, etc.).
    /// </summary>
    public interface IPrefix
    {
        /// <summary>
        /// This property, when implemented, should provide the scale of the prefix
        /// as compared to 1.0. 
        /// 
        /// For instance, the metric "milli" prefix would have a scale of 0.001.
        /// </summary>
        decimal Scale { get; }

        /// <summary>
        /// This property, when implemented, should provide the the full prefix, in lowercase.
        /// 
        /// For instance, the metric prefix "Kilo" would have the name "kilo".
        /// </summary>
        string Name { get; }

        /// <summary>
        /// This property, when implemented, should provide the abbreviated form of the
        /// prefix, properly cased.
        /// 
        /// For instance, the metric prefix "Kilo" would have the abbreviation "k".
        /// </summary>
        string Abbreviation { get; }
    }

    /// <summary>
    /// The "real" base interface for all unit classes. This interface should be inherited into
    /// interfaces that represent each available dimension measurable (e.g. Length, Temperature, etc.)
    /// </summary>
    /// <typeparam name="UnitType">
    /// This parameter should be set to the interface implementing this interface. This ensures the
    /// "RelativeTo" property is of the same type as the inheriting interface. 
    /// </typeparam>
    public interface IUnit
    {
        /// <summary>
        /// When implemented in a Unit class, this property should return the full, singular name of
        /// the unit, in lowercase.
        /// 
        /// For instnace, if the implementing class represents US Inches, this property should return
        /// "inch".
        /// </summary>
        string Name { get; }

        /// <summary>
        /// When implemented in a Unit class, this property should return the common, accepted, 
        /// abbreviation or symbol for the unit, appropriately cased.
        /// 
        /// For instance, if the implementing class represents US Inches, this property should
        /// return "in". If the implementing class represents Kelvin, this property should
        /// return "K".
        /// </summary>
        string Abbreviation { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ISimpleUnit : IUnit
    {
        /// <summary>
        /// When implemented in a Unit class, this property returns an instance of the Unit class
        /// that represents the "base" unit for the dimension this unit is in.
        /// 
        /// For instance, if the implementing class represents US Inches, this property should
        /// return an instance of a Unit class that represents Feet.
        /// </summary>
        ISimpleUnit RelativeTo { get; }

        /// <summary>
        /// When implemented in a Unit class, this property returns an instance of the Dimension class
        /// that represents the Dimension that this Unit is in (e.g. Length or Temperature).
        /// </summary>
        IDimension Dimension { get; }

        /// <summary>
        /// When implemented in a Unit class, this property returns a decimal number that represents
        /// the amount of this unit for every 1.0 of the <see cref="RelativeTo"/> unit.
        /// 
        /// For instance, if the implementing class represents US Inches, and RelativeTo is Feet, 
        /// this property should return the number of Inches in a Foot, or 12.0.
        /// </summary>
        decimal ScaledFromRelative { get; }
    }

    public interface IOffsetUnit
    {
        decimal offsetToRelativeUnit( decimal aValue );
        decimal offsetFromRelativeUnit( decimal aValue );
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IDerivedUnit : IUnit, IDimension
    {
        DimensionVector BaseVector { get; }
    }
}
