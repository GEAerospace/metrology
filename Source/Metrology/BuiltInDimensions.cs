// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology
{
    // The classes in this file represent unit dimensions built-in to this
    // library.

    // This includes the 7 SI dimensions.

    /// <summary>
    /// This class is for units that are in the "Length" dimension.
    /// </summary>
    public sealed class LengthDimension : IDimension { }

    /// <summary>
    /// This class is for units that are in the "Mass" dimension.
    /// </summary>
    public sealed class MassDimension : IDimension { }

    /// <summary>
    /// This class is for units that are in the "Temperature" dimension.
    /// </summary>
    public sealed class TemperatureDimension : IDimension { }

    /// <summary>
    /// This class is for units that are in the "LuminousIntensity" dimension.
    /// </summary>
    public sealed class LuminousIntensityDimension : IDimension { }

    /// <summary>
    /// This class is for units that are in the "Current" dimension.
    /// </summary>
    public sealed class CurrentDimension : IDimension { }

    /// <summary>
    /// This class is for units that are in the "Time" dimension.
    /// </summary>
    public sealed class TimeDimension : IDimension { }

    /// <summary>
    /// This class is for units that are in the "Amount Of Substance" dimension.
    /// </summary>
    public sealed class AmountOfSubstanceDimension : IDimension { }

    // Not real dimensions, but added to make the library more consistent.
    /// <summary>
    /// This class is for units that are in the "Angle" pseudo-dimension.
    /// </summary>
    public sealed class AngularDimension : IDimension { }

    /// <summary>
    /// This class is for units that are in the "Data Storage" psuedo-dimension.
    /// </summary>
    public sealed class DataStorageDimension : IDimension { }
}
