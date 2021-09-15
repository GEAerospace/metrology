// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using GEAviation.Metrology;
using GEAviation.Metrology.MetricUnits;
using GEAviation.Metrology.ImperialUnits;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Metrology.Test
{
    public static class TestExtensions
    {
        public static void AssertException<TExpectedException>( this Action aShouldExcept )
            where TExpectedException : Exception
        {
            if ( aShouldExcept == null )
            {
                Assert.Fail();
            }

            var lGotException = false;
            try
            {
                aShouldExcept();
            }
            catch ( TExpectedException )
            {
                lGotException = true;
            }
            catch ( Exception )
            {
                Assert.Fail();
            }
            Assert.IsTrue( lGotException );
        }

        public static void AssertNoException( this Action aShouldExcept )
        {
            if ( aShouldExcept == null )
            {
                Assert.Fail();
            }

            var lGotException = false;
            try
            {
                aShouldExcept();
            }
            catch ( Exception )
            {
                lGotException = true;
            }
            Assert.IsFalse( lGotException );
        }

        public static void AssertIsNotNullOrWhitespace( this string aString )
        {
            Assert.IsFalse( string.IsNullOrWhiteSpace( aString ) );
        }

        public static void AssertQuantitiesEqual( Quantity aExpected, Quantity aActual, decimal aTolerance = 0.0m )
        {
            Assert.IsTrue( aExpected.isCongruent( aActual ) );

            var lTolerance = new Quantity( aTolerance, aExpected.Units );
            var lLower = aExpected - lTolerance;
            var lUpper = aExpected + lTolerance;

            Assert.IsTrue( lLower <= aActual && aActual <= lUpper );
        }
    }

    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void SimpleMultiplication()
        {
            // Tests multiplication operations where no unit conversion happens

            Quantity lValue = "9.81 m/s^2";

            {
                var lTest = lValue * 3.0m;
                TestExtensions.AssertQuantitiesEqual( "29.43 m/s^2", lTest, 0.005m );
            }

            {
                var lTest = 10.0m * lValue;
                TestExtensions.AssertQuantitiesEqual( "98.1 m/s^2", lTest, 0.005m );
            }

            {
                var lTest = lValue * "5 s";
                TestExtensions.AssertQuantitiesEqual( "49.05 m/s", lTest, 0.005m );
            }

            {
                var lTest = lValue * "5 s^2";
                TestExtensions.AssertQuantitiesEqual( "49.05 m", lTest, 0.005m );
            }

            {
                var lTest = lValue.multiply( "5 s^2" );
                TestExtensions.AssertQuantitiesEqual( "49.05 m", lTest, 0.005m );
            }
        }

        [TestMethod]
        public void MultiplicationWithConversion()
        {
            // Tests multiplication operations where unit conversions happen

            Quantity lValue = "9.81 m/s^2";

            {
                var lTest = lValue * "1 hr";
                TestExtensions.AssertQuantitiesEqual( "35316 m/s", lTest, 0.0005m );

                lTest = lTest.convert( "km/s" );
                TestExtensions.AssertQuantitiesEqual( "35.316 km/s", lTest, 0.0005m );
            }

            {
                var lTest = lValue * "1 hr^2";
                TestExtensions.AssertQuantitiesEqual( "127137600.0 m", lTest, 0.005m );
            }

            {
                var lTest = lValue.multiply( "2 cm" );
                TestExtensions.AssertQuantitiesEqual( "0.1962 m^2/s^2", lTest, 0.00005m );

                lTest = lTest.convert( "mm^2/hr^2" );
                TestExtensions.AssertQuantitiesEqual( "2542752000000.0 mm^2/hr^2", lTest, 0.0005m );
            }
        }

        [TestMethod]
        public void SimpleDivision()
        {
            // Tests division operations where no unit conversion happens

            Quantity lValue = "9.81 m/s^2";

            {
                var lTest = lValue / 3.0m;
                TestExtensions.AssertQuantitiesEqual( "3.27 m/s^2", lTest, 0.005m );
            }

            {
                var lTest = lValue / "3 g";
                TestExtensions.AssertQuantitiesEqual( "3.27 m/s^2*g", lTest, 0.005m );
            }

            {
                var lTest = lValue / "5 s";
                TestExtensions.AssertQuantitiesEqual( "1.962 m/s^3", lTest, 0.0005m );
            }

            {
                var lTest = lValue / "5 s^2";
                TestExtensions.AssertQuantitiesEqual( "1.962 m/s^4", lTest, 0.0005m );
            }

            {
                var lTest = lValue.divide( "5 s^-2" );
                TestExtensions.AssertQuantitiesEqual( "1.962 m", lTest, 0.0005m );
            }

            {
                // Divide by zero!
                var lGotException = false;
                try
                {
                    var lResult = (Quantity)"15 ft^2" / "0 ft";
                }
                catch ( DivideByZeroException )
                {
                    lGotException = true;
                }
                catch ( Exception )
                {
                    Assert.Fail();
                }
                Assert.IsTrue( lGotException );
            }

            {
                // Divide by zero!
                var lGotException = false;
                try
                {
                    var lResult = (Quantity)"15 ft^2" / 0.0m;
                }
                catch ( DivideByZeroException )
                {
                    lGotException = true;
                }
                catch ( Exception )
                {
                    Assert.Fail();
                }
                Assert.IsTrue( lGotException );
            }
        }

        [TestMethod]
        public void DivisionWithConversion()
        {
            // Tests division operations where unit conversions happen

            Quantity lValue = "9.81 m/s^2";

            {
                var lTest = lValue / "1 ft";
                TestExtensions.AssertQuantitiesEqual( "32.1850393700787072 1/s^2", lTest, 0.0005m );
            }

            {
                var lTest = lValue / "1 hr";
                TestExtensions.AssertQuantitiesEqual( "0.002725 m/s^3", lTest, 0.0000005m );
            }

            {
                var lTest = lValue.divide( "5 s^-2" );
                TestExtensions.AssertQuantitiesEqual( "1.962 m", lTest, 0.0005m );
            }
        }

        [TestMethod]
        public void SimpleAddition()
        {
            // Tests addition operations where no unit conversion happens

            {
                var lTest = (Quantity)"23.567 ft/s" + "4.557 ft/s";
                TestExtensions.AssertQuantitiesEqual( "28.124 ft/s", lTest, 0.0005m );
            }

            {
                var lTest = ( (Quantity)"-15 s^-2" ).add( "20 s^-2" );
                TestExtensions.AssertQuantitiesEqual( "5 1/s^2", lTest, 0.0005m );
            }

            {
                // Incongruent!
                TestExtensions.AssertException<InvalidOperationException>( () =>
                {
                    var lBadAdd = (Quantity)"15 ft" + "20 s";
                } );
            }
        }

        [TestMethod]
        public void AdditionWithConversion()
        {
            // Tests addition operations where unit conversions happen

            Quantity lValue = "5 km/hr";

            {
                var lTest = lValue + "4.557 ft/s";
                TestExtensions.AssertQuantitiesEqual( "10.00030496000 km/hr", lTest, 0.0005m );
            }

            {
                var lTest = (Quantity)"23 kft^2" + "1000000000000 mm^2";
                TestExtensions.AssertQuantitiesEqual( "33.76391041670970 kft^2", lTest, 0.0005m );
            }

            {
                var lTest = ( (Quantity)"-15 s^-2" ).add( "10000000 hr^-2" );
                TestExtensions.AssertQuantitiesEqual( "-14.228395061728395061728395061728 1/s^2", lTest, 0.0005m );
            }
        }

        [TestMethod]
        public void SimpleSubtraction()
        {
            // Tests addition operations where no unit conversion happens

            {
                var lTest = (Quantity)"23.567 ft/s" - "4.557 ft/s";
                TestExtensions.AssertQuantitiesEqual( "19.01 ft/s", lTest, 0.0005m );
            }

            {
                var lTest = ( (Quantity)"-15 s^-2" ).subtract( "20 s^-2" );
                TestExtensions.AssertQuantitiesEqual( "-35 1/s^2", lTest, 0.0005m );
            }

            {
                // Incongruent!
                TestExtensions.AssertException<InvalidOperationException>( () =>
                {
                    var lBadAdd = (Quantity)"15 ft" - "20 s";
                } );
            }
        }

        [TestMethod]
        public void Construction()
        {
            Quantity lImplicit = "56 g";
            var lExplicit = (Quantity)"56 g";
            var lConstructor = new Quantity( 56, "g" );
            var lCopy = new Quantity( lConstructor );

            var lExpected = (Quantity)"56 g";
            TestExtensions.AssertQuantitiesEqual( lExpected, lImplicit, 0.000000001m );
            TestExtensions.AssertQuantitiesEqual( lExpected, lExplicit, 0.000000001m );
            TestExtensions.AssertQuantitiesEqual( lExpected, lConstructor, 0.000000001m );
            TestExtensions.AssertQuantitiesEqual( lExpected, lCopy, 0.000000001m );

            var lTryParse = Quantity.TryParse( "56 g", out var lTryParseQuantity );
            Assert.IsTrue( lTryParse );
            TestExtensions.AssertQuantitiesEqual( lExpected, lTryParseQuantity, 0.000000001m );

            Quantity lNot;
            var lBadParse = Quantity.TryParse( "56 notreal", out lNot );
            Assert.IsFalse( lBadParse );
            Assert.IsNull( lNot );

            var lGotException = false;
            try
            {
                var lWillExcept = new Quantity( 56m, "notreal" );
            }
            catch ( InvalidOperationException )
            {
                lGotException = true;
            }
            catch ( Exception )
            {
                Assert.Fail();
            }

            Assert.IsTrue( lGotException );

            lGotException = false;
            try
            {
                var lWillExcept = new Quantity( null );
            }
            catch ( ArgumentNullException )
            {
                lGotException = true;
            }
            catch ( Exception )
            {
                Assert.Fail();
            }

            Assert.IsTrue( lGotException );
        }

        [TestMethod]
        public void ConstructionNoUnits()
        {
            Quantity lImplicit = "56";
            var lExplicit = (Quantity)"56";
            var lConstructor = new Quantity( 56, "" );
            var lCopy = new Quantity( lConstructor );

            var lExpected = (Quantity)"56";
            TestExtensions.AssertQuantitiesEqual( lExpected, lImplicit, 0.000000001m );
            TestExtensions.AssertQuantitiesEqual( lExpected, lExplicit, 0.000000001m );
            TestExtensions.AssertQuantitiesEqual( lExpected, lConstructor, 0.000000001m );
            TestExtensions.AssertQuantitiesEqual( lExpected, lCopy, 0.000000001m );

            var lTryParse = Quantity.TryParse( "56", out var lTryParseQuantity );
            Assert.IsTrue( lTryParse );
            TestExtensions.AssertQuantitiesEqual( lExpected, lTryParseQuantity, 0.000000001m );
        }

        [TestMethod]
        public void Sandbox()
        {
            // Tests addition operations where unit conversions happen
            //var lOther2 = Quantity.TryParse( "5 kN", out var _ );
            //var lThing = DerivedUnitVector.TryParse( "kN/mV", out var lVector );
            //var lThing2 = DerivedUnitVector.TryParse( "N", out var lVector2 );
            //var lOther = lVector.multiply( lVector2, out var lFactor );

            //var lGoodBase = DimensionVector.TryParse( "kg*m^2/s^3*A", out var lNewtonBase );
            //lGoodBase = DerivedUnitVector.TryParse( "kN", out var lNewtonVector );

            //var lNewVector = lNewtonBase.ApplyDerivedUnits( lNewtonVector.Components.Select( aComp => aComp.Value ), out var lFactor );

            var lTest = (Quantity)"1 slug";
            var lNew = lTest.convert( "kg" );
        }

        [TestMethod]
        public void Comparison()
        {
            {
                // Less Than
                var lLessThan = (Quantity)"56 g" < "1 lb";
                Assert.IsTrue( lLessThan );

                lLessThan = (Quantity)"1 lb" < "56 g";
                Assert.IsFalse( lLessThan );

                TestExtensions.AssertException<InvalidOperationException>( () =>
                {
                    var lIncongruent = (Quantity)"1 lb" < "5 s";
                } );
            }

            {
                // Less Than with Absolute Offsets
                var lLessThan = (Quantity)"10 C" < "50.1 F";
                Assert.IsTrue( lLessThan );

                lLessThan = (Quantity)"10 C" < "49.9 F";
                Assert.IsFalse( lLessThan );
            }

            {
                // Less Than with Relative Offsets
                var lLessThan = (Quantity)"10 C/s" < "18.1 F/s";
                Assert.IsTrue( lLessThan );

                lLessThan = (Quantity)"10 C/s" < "17.9 F/s";
                Assert.IsFalse( lLessThan );
            }

            {
                // Greater Than
                var lGreaterThan = (Quantity)"56 g" > "1 lb";
                Assert.IsFalse( lGreaterThan );

                lGreaterThan = (Quantity)"1 lb" > "56 g";
                Assert.IsTrue( lGreaterThan );

                TestExtensions.AssertException<InvalidOperationException>( () =>
                {
                    var lIncongruent = (Quantity)"1 lb" > "5 s";
                } );
            }

            {
                // Greater Than with Absolute Offsets
                var lGreaterThan = (Quantity)"10 C" > "49.9 F";
                Assert.IsTrue( lGreaterThan );

                lGreaterThan = (Quantity)"10 C" > "50.1 F";
                Assert.IsFalse( lGreaterThan );
            }

            {
                // Greater Than with Relative Offsets
                var lGreaterThan = (Quantity)"10 C/s" > "17.9 F/s";
                Assert.IsTrue( lGreaterThan );

                lGreaterThan = (Quantity)"10 C/s" > "18.1 F/s";
                Assert.IsFalse( lGreaterThan );
            }

            {
                // Less Than Or Equal To
                var lLessThan = (Quantity)"1 g" <= "1 lb";
                Assert.IsTrue( lLessThan );

                lLessThan = (Quantity)"1 g" <= "1 g";
                Assert.IsTrue( lLessThan );

                lLessThan = (Quantity)"1 lb" <= "56 g";
                Assert.IsFalse( lLessThan );

                TestExtensions.AssertException<InvalidOperationException>( () =>
                {
                    var lIncongruent = (Quantity)"1 lb" <= "5 s";
                } );
            }

            {
                // Less Than Or Equal To with Absolute Offsets
                var lLessThan = (Quantity)"10 C" <= "50 F";
                Assert.IsTrue( lLessThan );

                lLessThan = (Quantity)"10 C" <= "49.9 F";
                Assert.IsFalse( lLessThan );
            }

            {
                // Less Than Or Equal To with Relative Offsets
                var lLessThan = (Quantity)"10 C/s" <= "18 F/s";
                Assert.IsTrue( lLessThan );

                lLessThan = (Quantity)"10 C/s" <= "17.9 F/s";
                Assert.IsFalse( lLessThan );
            }

            {
                // Greater Than Or Equal To
                var lGreaterThan = (Quantity)"1 g" >= "1 lb";
                Assert.IsFalse( lGreaterThan );

                lGreaterThan = (Quantity)"1 g" >= "1 g";
                Assert.IsTrue( lGreaterThan );

                lGreaterThan = (Quantity)"1 lb" >= "56 g";
                Assert.IsTrue( lGreaterThan );

                TestExtensions.AssertException<InvalidOperationException>( () =>
                {
                    var lIncongruent = (Quantity)"1 lb" >= "5 s";
                } );
            }

            {
                // Greater Than Or Equal To with Absolute Offsets
                var lGreaterThan = (Quantity)"32 F" >= "0 C";
                Assert.IsTrue( lGreaterThan );

                lGreaterThan = (Quantity)"0 C" >= "32.01 F";
                Assert.IsFalse( lGreaterThan );
            }

            {
                // Greater Than Or Equal To with Relative Offsets
                var lGreaterThan = (Quantity)"18 F/s" >= "10 C/s";
                Assert.IsTrue( lGreaterThan );

                lGreaterThan = (Quantity)"10 C/s" >= "18.1 F/s";
                Assert.IsFalse( lGreaterThan );
            }

            {
                // Equal To
                var lEqualTo = (Quantity)"1 g" == "1 g";
                Assert.IsTrue( lEqualTo );

                lEqualTo = (Quantity)"1 g" == "1 lb";
                Assert.IsFalse( lEqualTo );

                var lIncongruent = (Quantity)"1 lb" == "5 s";
                Assert.IsFalse( lIncongruent );
            }

            {
                // Equal To with Absolute Offsets
                var lEqualTo = (Quantity)"50 F" == "10 C";
                Assert.IsTrue( lEqualTo );

                lEqualTo = (Quantity)"10 C" == "50.1 F";
                Assert.IsFalse( lEqualTo );
            }

            {
                // Equal To with Relative Offsets
                var lEqualTo = (Quantity)"18 F/s" == "10 C/s";
                Assert.IsTrue( lEqualTo );

                lEqualTo = (Quantity)"10 C/s" == "18.1 F/s";
                Assert.IsFalse( lEqualTo );
            }

            {
                // Not Equal To
                var lNotEqualTo = (Quantity)"1 g" != "1 g";
                Assert.IsFalse( lNotEqualTo );

                lNotEqualTo = (Quantity)"1 g" != "1 lb";
                Assert.IsTrue( lNotEqualTo );

                var lIncongruent = (Quantity)"1 lb" != "5 s";
                Assert.IsTrue( lIncongruent );
            }

            {
                // Not Equal To with Absolute Offsets
                var lNotEqualTo = (Quantity)"10 C" != "50.1 F";
                Assert.IsTrue( lNotEqualTo );

                lNotEqualTo = (Quantity)"50 F" != "10 C";
                Assert.IsFalse( lNotEqualTo );
            }

            {
                // Not Equal To with Relative Offsets
                var lNotEqualTo = (Quantity)"10 C/s" != "18.1 F/s";
                Assert.IsTrue( lNotEqualTo );

                lNotEqualTo = (Quantity)"18 F/s" != "10 C/s";
                Assert.IsFalse( lNotEqualTo );
            }

            {
                // Comparison with tolerance
                var lTolerantComparison = ( (Quantity)"10 C" ).CompareToWithTolerance( "50 F", "0.1 F" );
                Assert.AreEqual( 0, lTolerantComparison );

                lTolerantComparison = ( (Quantity)"10.1 C" ).CompareToWithTolerance( "50 F", "0.1 F" );
                Assert.AreEqual( 1, lTolerantComparison );

                lTolerantComparison = ( (Quantity)"9.9 C" ).CompareToWithTolerance( "50 F", "0.1 F" );
                Assert.AreEqual( -1, lTolerantComparison );

                TestExtensions.AssertException<InvalidOperationException>( () =>
                {
                    ( (Quantity)"10 C" ).CompareToWithTolerance( "50 F/s", "0.1 F" );
                } );

                TestExtensions.AssertException<InvalidOperationException>( () =>
                {
                    ( (Quantity)"10 C" ).CompareToWithTolerance( "50 F", "0.1 F/s" );
                } );
            }
        }

        [TestMethod]
        public void DimensionVectorBaseMiscellaneous()
        {
            var lCongruent = ( (DimensionVector)"g" ).isCongruent( "g*s" );
            Assert.IsFalse( lCongruent );

            lCongruent = ( (DimensionVector)"g^2*s" ).isCongruent( "g*s" );
            Assert.IsFalse( lCongruent );
        }

        [TestMethod]
        public void DimensionVectorMiscellaneous()
        {
            // This test is "chasing" code coverage in a less organized group of
            // functionality from the DimensionVector class.

            {
                TestExtensions.AssertException<InvalidOperationException>( () =>
                {
                    var lVectorGood = DimensionVector.TryParse( "g", out var lVector );
                    Assert.IsTrue( lVectorGood );

                    // Should die
                    lVector.convert( "s" );
                } );
            }

            {
                // Branch coverage for Pow function.
                var lPowResult = DimensionVector.Pow( "g", 0 );
                Assert.AreSame( DimensionVector.Zero, lPowResult );

                lPowResult = DimensionVector.Pow( "g", -1 );
                Assert.AreEqual( "1/g", lPowResult.ToString() );
            }

            {
                var lComps = new List<VectorComponent<ISimpleUnit>>()
                {
                    new VectorComponent<ISimpleUnit>( UnitManager.getUnit<Gram>(), 0 )
                };

                var lCreate = DimensionVector.create( lComps );
            }

            {
                TestExtensions.AssertException<InvalidOperationException>( () =>
                {
                    var lImplicit = (DimensionVector)"notaunit";
                } );
            }

            {
                // Convert with prefix to cover interesting branches in the convert function.
                var lFactor = ( (DimensionVector)"g" ).convert( "kg" );
                Assert.AreEqual( 1000m, lFactor );
                lFactor = ( (DimensionVector)"g^-1" ).convert( "kg^-1" );
                Assert.AreEqual( .001m, lFactor );
            }

            {
                // This "chase" test is just to get coverage on VectorComponent.GetHashCode().
                var lVector = (DimensionVector)"km*s/mol";
                var lHashCode = lVector.Components.First().Value.GetHashCode();
            }
        }

        [TestMethod]
        public void QuantityMiscellaneous()
        {
            var lToString = ( (Quantity)"56 g" ).ToString();
            Assert.AreEqual( "56 g", lToString );

            var lTryParseBadNumber = Quantity.TryParse( "g", out var lBadQuantity );
            Assert.IsFalse( lTryParseBadNumber );
            Assert.IsNull( lBadQuantity );

            TestExtensions.AssertException<InvalidOperationException>( () =>
            {
                var lBadImplicit = (Quantity)"g";
            } );

            TestExtensions.AssertException<InvalidOperationException>( () =>
            {
                Quantity lGood = "5 s";
                var lBadVectorConvert = lGood.convert( "notaunit" );
            } );

            TestExtensions.AssertException<InvalidOperationException>( () =>
            {
                Quantity lGood = "5 s";
                var lBadVectorConvert = lGood.convert( "g" );
            } );

            {
                var lCelsius = (Quantity)"37 C";
                var lFahrenheit = lCelsius.convert( "F" );

                Assert.AreEqual( 98.6m, lFahrenheit.Amount );
                Assert.AreEqual( "F", lFahrenheit.Units );

                lCelsius = "45 C/m";
                lFahrenheit = lCelsius.convert( "F/ft" );

                Assert.AreEqual( 24.6887992, (double)lFahrenheit.Amount, 0.000001 );
                Assert.AreEqual( "F/ft", lFahrenheit.Units );
            }

            {
                var lOriginal = (Quantity)"56 g";
                var lCopyTest = lOriginal.copy();

                Assert.AreEqual( lOriginal, lCopyTest );
                Assert.AreNotSame( lOriginal, lCopyTest );
            }

            {
                //Equals(object) overload test
                var lQuantity = (Quantity)"56 g";
                var lEquals = lQuantity.Equals( DateTime.Now );
                Assert.IsFalse( lEquals );
            }

            {
                var lHashCode = ( (Quantity)"56 g" ).GetHashCode();
                Assert.AreNotEqual( (int)0, lHashCode );
            }

            // This test currently fails because Metrology doesn't track derived units
            // in a way that allows two that overlap in their base vector (e.g. a Volt is partially a Newton)
            // and still have it output both derived units. The answer is technically correct, but
            // the test remains so that it can be one day patched to track derived unit usage better.
            var lDerivedMath = (Quantity)"5 N" * "6 V";
            Assert.AreEqual( 30m, lDerivedMath.Amount );
            Assert.AreEqual( "N⋅V", lDerivedMath.Units );
        }

        // The following two classes are meant to help test a particular branch of the UnitConversion.convert method.
        // They are both length units, but do not share a common ancestor, so no conversion between them
        // exists.
        public class FakeLengthUnitA : ISimpleUnit
        {
            public ISimpleUnit RelativeTo => this;
            public IDimension Dimension => UnitManager.getDimension<LengthDimension>();
            public decimal ScaledFromRelative => 1.0m;
            public string Name => "fake length A";
            public string Abbreviation => "fakeA";
        }

        public class FakeLengthUnitB : ISimpleUnit
        {
            public ISimpleUnit RelativeTo => this;
            public IDimension Dimension => UnitManager.getDimension<LengthDimension>();
            public decimal ScaledFromRelative => 1.0m;
            public string Name => "fake length B";
            public string Abbreviation => "fakeB";
        }

        [TestMethod]
        public void UnitConversionMiscellaneous()
        {
            // This test is "chasing" code coverage in a less organized group of
            // functionality from the UnitConversion class.

            TestExtensions.AssertException<InvalidOperationException>( () =>
            {
                UnitConversion.convert( typeof( FakeLengthUnitA ), typeof( FakeLengthUnitB ) );
            } );
        }

        [TestMethod]
        public void UnitExensionsMiscellaneous()
        {
            // This test is "chasing" code coverage in a less organized group of
            // functionality from the UnitExtensions class.

            var lSuccess = "g*lb".TryParseUnitString<ISimpleUnit>( out var lResult );
            Assert.IsFalse( lSuccess );
            Assert.IsNull( lResult );

        }

        [TestMethod]
        public void UnitManagerMiscellaneous()
        {
            // This test is "chasing" code coverage in a less organized group of
            // functionality from the UnitManager class.

            var lSomething = UnitManager.lookupUnitAndPrefix<IUnit>( "nope" );
            Assert.IsNull( lSomething );
        }

        [TestMethod]
        public void VisitStuff()
        {
            // This test is designed to visit all of the in-built units, dimensions and prefixes
            // and make sure all of the declared properties work, are "reasonable" and don't cause
            // exceptions. This test is *not* intended to test the *correctness* of anything.

            // This also forces code coverage for units, prefixes and dimensions.

            {
                var lDimensions = UnitManager.getAllDimensions();

                foreach ( var lDimension in lDimensions )
                {
                    // IDimensions have no members. The visit is forced to ensure each class is really
                    // instantiated.
                }
            }

            {
                var lPrefixes = UnitManager.getAllPrefixes();

                foreach ( var lPrefix in lPrefixes )
                {
                    if ( lPrefix is NoPrefix )
                    {
                        // The no-prefix prefix is a special case.
                        Assert.AreEqual( String.Empty, lPrefix.Abbreviation );
                        Assert.AreEqual( String.Empty, lPrefix.Name );
                    }
                    else
                    {
                        lPrefix.Abbreviation.AssertIsNotNullOrWhitespace();
                        lPrefix.Name.AssertIsNotNullOrWhitespace();
                    }

                    Assert.AreNotEqual( 0.0m, lPrefix.Scale );
                }
            }

            {
                var lUnits = UnitManager.getAllUnits();

                foreach ( var lUnit in lUnits )
                {
                    lUnit.Abbreviation.AssertIsNotNullOrWhitespace();
                    lUnit.Name.AssertIsNotNullOrWhitespace();

                    // Doing this here to make it easy to exercise "getDimension()";
                    var lDimension = lUnit.getDimension();

                    if ( lUnit is ISimpleUnit lSimpleUnit )
                    {
                        Assert.IsNotNull( lSimpleUnit.Dimension );
                        Assert.IsNotNull( lSimpleUnit.RelativeTo );
                        Assert.AreNotEqual( 0.0m, lSimpleUnit.ScaledFromRelative );

                        Assert.AreSame( lDimension, lSimpleUnit.Dimension );
                    }

                    if ( lUnit is IDerivedUnit lDerivedUnit )
                    {
                        Assert.IsNotNull( lDerivedUnit.BaseVector );
                        Assert.AreSame( lDimension, lDerivedUnit );
                    }
                }
            }

        }
    }

    [TestClass]
    [DeploymentItem( "TrigTables.csv" )]
    public class QuantityMathTests
    {
        [TestMethod]
        public void PowTests()
        {
            {
                var lValue = QuantityMath.Pow( "15 ft^2", -4 );
                TestExtensions.AssertQuantitiesEqual( "0.00444444 1/ft^4", lValue, 0.00000001m );
            }

            {
                var lValue = QuantityMath.Pow( "15 ft^2/s^2", 2 );
                TestExtensions.AssertQuantitiesEqual( "225 ft^4/s^4", lValue, 0.00000001m );
            }

            {
                var lValue = QuantityMath.Pow( "15 ft^2/s^2", 0 );
                TestExtensions.AssertQuantitiesEqual( "1", lValue, 0.00000001m );
            }
        }

        [TestMethod]
        public void MinMaxTests()
        {
            {
                var lValue = QuantityMath.Min( "56.23 ft", "18.7433333 m" );
                TestExtensions.AssertQuantitiesEqual( "56.23 ft", lValue, 0.005m );
            }

            {
                var lValue = QuantityMath.Max( "56.23 ft", "18.7433333 m" );
                TestExtensions.AssertQuantitiesEqual( "18.7433333 m", lValue, 0.00000005m );
            }

            {
                var lValue = QuantityMath.Min( "62 ft", "18.7433333 m" );
                TestExtensions.AssertQuantitiesEqual( "18.7433333 m", lValue, 0.00000005m );
            }

            {
                var lValue = QuantityMath.Max( "62 ft", "18.7433333 m" );
                TestExtensions.AssertQuantitiesEqual( "62 ft", lValue, 0.00000005m );
            }

            TestExtensions.AssertException<InvalidOperationException>( () =>
            {
                var lValue = QuantityMath.Min( "62 ft", "62 mol" );
            } );

            TestExtensions.AssertException<InvalidOperationException>( () =>
            {
                var lValue = QuantityMath.Max( "62 ft", "62 mol" );
            } );
        }

        [TestMethod]
        public void RoundingTests()
        {
            // Truncate
            {
                var lValue = QuantityMath.Truncate( "56.23 ft" );
                TestExtensions.AssertQuantitiesEqual( "56 ft", lValue, 0.005m );
            }

            {
                var lValue = QuantityMath.Truncate( "56.73 ft" );
                TestExtensions.AssertQuantitiesEqual( "56 ft", lValue, 0.005m );
            }

            {
                var lValue = QuantityMath.Truncate( "-56.23 ft" );
                TestExtensions.AssertQuantitiesEqual( "-56 ft", lValue, 0.005m );
            }

            {
                var lValue = QuantityMath.Truncate( "-56.73 ft" );
                TestExtensions.AssertQuantitiesEqual( "-56 ft", lValue, 0.005m );
            }

            // Floor
            {
                var lValue = QuantityMath.Floor( "56.23 ft" );
                TestExtensions.AssertQuantitiesEqual( "56 ft", lValue, 0.005m );
            }

            {
                var lValue = QuantityMath.Floor( "56.73 ft" );
                TestExtensions.AssertQuantitiesEqual( "56 ft", lValue, 0.005m );
            }

            {
                var lValue = QuantityMath.Floor( "-56.23 ft" );
                TestExtensions.AssertQuantitiesEqual( "-57 ft", lValue, 0.005m );
            }

            {
                var lValue = QuantityMath.Floor( "-56.73 ft" );
                TestExtensions.AssertQuantitiesEqual( "-57 ft", lValue, 0.005m );
            }

            // Ceiling
            {
                var lValue = QuantityMath.Ceiling( "56.23 ft" );
                TestExtensions.AssertQuantitiesEqual( "57 ft", lValue, 0.005m );
            }

            {
                var lValue = QuantityMath.Ceiling( "56.73 ft" );
                TestExtensions.AssertQuantitiesEqual( "57 ft", lValue, 0.005m );
            }

            {
                var lValue = QuantityMath.Ceiling( "-56.23 ft" );
                TestExtensions.AssertQuantitiesEqual( "-56 ft", lValue, 0.005m );
            }

            {
                var lValue = QuantityMath.Ceiling( "-56.73 ft" );
                TestExtensions.AssertQuantitiesEqual( "-56 ft", lValue, 0.005m );
            }

            // Round
            // TODO
        }

        [TestMethod]
        public void SignTests()
        {
            Assert.AreEqual( -1, QuantityMath.Sign( "-56 km/s" ) );
            Assert.AreEqual( 0, QuantityMath.Sign( "0 km/s" ) );
            Assert.AreEqual( 1, QuantityMath.Sign( "56 km/s" ) );

            {
                var lActual = QuantityMath.Abs( "-88 mph" );
                TestExtensions.AssertQuantitiesEqual( "88 mph", lActual, 0.1m );
            }

            {
                var lActual = QuantityMath.Abs( "88 mph" );
                TestExtensions.AssertQuantitiesEqual( "88 mph", lActual, 0.1m );
            }
        }

        [TestMethod]
        public void TrigTests()
        {
            bool lFirstLine = true;
            using ( StreamReader lSR = new StreamReader( "TrigTables.csv" ) )
            {
                if ( lFirstLine )
                {
                    // Burn the header line
                    lSR.ReadLine();
                    lFirstLine = false;
                }

                var lCurLine = lSR.ReadLine();

                // Split: degrees, radians, sin, sinh, cos, cosh, tan, tanh
                var lSplit = lCurLine.Split( ',' );

                var lAngle = (Quantity)$"{lSplit[0]} deg";
                var lAngleRad = lAngle.convert( "rad" );

                // Sin/Asin
                if ( !string.IsNullOrWhiteSpace( lSplit[2].Trim() ) )
                {
                    var lSineNumber = (Quantity)$"{lSplit[2]}";

                    {
                        var lActual = QuantityMath.Sin( lAngle );
                        Assert.AreEqual( (double)lSineNumber.Amount, (double)lActual.Amount, 0.00000000001 );
                        Assert.AreEqual( lActual.Units, "" );
                    }

                    {
                        var lActual = QuantityMath.Asin( lSineNumber );
                        Assert.AreEqual( (double)lAngleRad.Amount, (double)lActual.Amount, 0.00000000001 );
                        Assert.AreEqual( lActual.Units, "rad" );
                    }
                }

                // Sinh
                if ( !string.IsNullOrWhiteSpace( lSplit[3].Trim() ) )
                {
                    var lSineNumber = (Quantity)$"{lSplit[3]}";

                    {
                        var lActual = QuantityMath.Sinh( lAngle );
                        Assert.AreEqual( (double)lSineNumber.Amount, (double)lActual.Amount, 0.00000000001 );
                        Assert.AreEqual( lActual.Units, "" );
                    }
                }

                // Cos/Acos
                if ( !string.IsNullOrWhiteSpace( lSplit[4].Trim() ) )
                {
                    var lCosineNumber = (Quantity)$"{lSplit[4]}";

                    {
                        var lActual = QuantityMath.Cos( lAngle );
                        Assert.AreEqual( (double)lCosineNumber.Amount, (double)lActual.Amount, 0.00000000001 );
                        Assert.AreEqual( lActual.Units, "" );
                    }

                    {
                        var lActual = QuantityMath.Acos( lCosineNumber );
                        Assert.AreEqual( (double)lAngleRad.Amount, (double)lActual.Amount, 0.00000000001 );
                        Assert.AreEqual( lActual.Units, "rad" );
                    }
                }

                // Cosh
                if ( !string.IsNullOrWhiteSpace( lSplit[5].Trim() ) )
                {
                    var lHCosineNumber = (Quantity)$"{lSplit[5]}";

                    {
                        var lActual = QuantityMath.Cosh( lAngle );
                        Assert.AreEqual( (double)lHCosineNumber.Amount, (double)lActual.Amount, 0.00000000001 );
                        Assert.AreEqual( lActual.Units, "" );
                    }
                }

                // Tan/Atan
                if ( !string.IsNullOrWhiteSpace( lSplit[6].Trim() ) )
                {
                    var lTangentNumber = (Quantity)$"{lSplit[6]}";

                    {
                        var lActual = QuantityMath.Tan( lAngle );
                        Assert.AreEqual( (double)lTangentNumber.Amount, (double)lActual.Amount, 0.00000000001 );
                        Assert.AreEqual( lActual.Units, "" );
                    }

                    {
                        var lActual = QuantityMath.Atan( lTangentNumber );
                        Assert.AreEqual( (double)lAngleRad.Amount, (double)lActual.Amount, 0.00000000001 );
                        Assert.AreEqual( lActual.Units, "rad" );
                    }
                }

                // Tanh
                if ( !string.IsNullOrWhiteSpace( lSplit[7].Trim() ) )
                {
                    var lHTangentNumber = (Quantity)$"{lSplit[7]}";

                    {
                        var lActual = QuantityMath.Tanh( lAngle );
                        Assert.AreEqual( (double)lHTangentNumber.Amount, (double)lActual.Amount, 0.00000000001 );
                        Assert.AreEqual( lActual.Units, "" );
                    }
                }
            }

            // Exception Tests
            TestExtensions.AssertException<ArgumentException>( () =>
            {
                var lValue = QuantityMath.Sin( "62 ft" );
            } );

            TestExtensions.AssertException<ArgumentException>( () =>
            {
                var lValue = QuantityMath.Sinh( "62 ft" );
            } );

            TestExtensions.AssertException<ArgumentException>( () =>
            {
                var lValue = QuantityMath.Asin( "62 ft" );
            } );

            TestExtensions.AssertException<ArgumentException>( () =>
            {
                var lValue = QuantityMath.Cos( "62 ft" );
            } );

            TestExtensions.AssertException<ArgumentException>( () =>
            {
                var lValue = QuantityMath.Cosh( "62 ft" );
            } );

            TestExtensions.AssertException<ArgumentException>( () =>
            {
                var lValue = QuantityMath.Acos( "62 ft" );
            } );

            TestExtensions.AssertException<ArgumentException>( () =>
            {
                var lValue = QuantityMath.Tan( "62 ft" );
            } );

            TestExtensions.AssertException<ArgumentException>( () =>
            {
                var lValue = QuantityMath.Tanh( "62 ft" );
            } );

            TestExtensions.AssertException<ArgumentException>( () =>
            {
                var lValue = QuantityMath.Atan( "62 ft" );
            } );
        }
    }
}
