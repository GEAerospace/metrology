// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace GEAviation.Metrology
{

    /// <summary>
    /// Apply this attribute to fields, properties and method parameters that take <see cref="Quantity"/>
    /// values to indicate the specific dimensional shape (e.g. length/mass) of quantity the field,
    /// property or parameter expects.
    /// 
    /// This attribute has no runtime effect, but is intended to assist with code analysis
    /// tools, such as Roslyn Code Analyzers, in compile-time checks.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field)]
    public class ExpectedQuantityAttribute : Attribute
    {
        /// <summary>
        /// The dimensional shape string (e.g. "length/time^2") that this attribute was created with.
        /// </summary>
        public string DimensionalShape { get; }

        /// <summary>
        /// Constructs a new <see cref="ExpectedQuantityAttribute"/> with the
        /// specified dimensional shape. 
        /// </summary>
        /// <param name="aDimensionalShape">
        /// A string representing the expected dimensional shape. For example, "length/time^2"
        /// for an acceleration. The names of dimensions used in this string should exactly 
        /// match (except for case) the names of known <see cref="IDimension"/> implementations,
        /// without the "Dimension" suffix (such as "length" for <see cref="LengthDimension"/>.
        /// </param>
        public ExpectedQuantityAttribute( string aDimensionalShape )
        {
            DimensionalShape = aDimensionalShape;
        }
    }
    
}
