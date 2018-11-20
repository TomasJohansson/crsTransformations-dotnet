using com.programmerare.crsTransformations;
using com.programmerare.crsTransformations.coordinate;
using com.programmerare.crsTransformations.crsIdentifier;
using NUnit.Framework;
using System.Collections.Generic;

namespace Programmerare.CrsTransformations.Test.crsTransformations.implementations
{
    [TestFixture]
    class MightyLittleGeodesyTest
    {
        // These coordinate should be corresponding:
        private const double wgs84Lat = 59.330231;
        private const double wgs84Lon = 18.059196;
        private const double sweref99Y = 6580822;
        private const double sweref99X = 674032;
        private const double rt90Y = 6580994;
        private const double rt90X = 1628294;
        private const int epsgWGS84 = 4326;
        private const int epsgSweref99 = 3006;
        private const int epsgRT9025gonv = 3021;// RT90 2.5 gon V
        private const string EPSG_PREFIX = "EPSG:";
        private static readonly string crsCodeWGS84 = EPSG_PREFIX + epsgWGS84;
        private static readonly string crsCodeSweref99 = EPSG_PREFIX + epsgSweref99;
        private static readonly string crsCodeRT9025gonv = EPSG_PREFIX + epsgRT9025gonv;// RT90 2.5 gon V

        private CrsTransformationAdapter crsTransformationAdapter;
        
        // These three are used as input parameters and as expected values
        private CrsCoordinate coordinateWgs84, coordinateSweref99, coordinateRT90;
        // These below will be used as the actual values and be asserted with the expected values (i.e. the above objects)
        private CrsCoordinate resultWgs84, resultSweref99, resultRT90;

        double maxMeterDifferenceForSuccessfulTest = 0.5; // 0.5 meter
        double maxLatLongDifferenceForSuccessfulTest = 0.00001;

        [SetUp]
        public void SetUp()
        {
            coordinateWgs84 = CrsCoordinateFactory.LatLon(wgs84Lat, wgs84Lon, epsgWGS84);
            coordinateSweref99 = CrsCoordinateFactory.LatLon(sweref99Y, sweref99X, epsgSweref99);
            coordinateRT90 = CrsCoordinateFactory.LatLon(rt90Y, rt90X, epsgRT9025gonv);

            crsTransformationAdapter = new CrsTransformationAdapterMightyLittleGeodesy();
        }

        [Test]
        public void transform_fromWgs84_toSweref99()
        {
            resultSweref99 = crsTransformationAdapter.TransformToCoordinate(coordinateWgs84, epsgSweref99);
            AssertCoordinateResult(
                resultSweref99,
                coordinateSweref99,
                maxMeterDifferenceForSuccessfulTest
            );

            // testing the same transform as above but with the overloaded 
            // method taking a string as last parameter instead of integer
            AssertCoordinateResult(
                crsTransformationAdapter.TransformToCoordinate(coordinateWgs84, crsCodeSweref99),
                coordinateSweref99,
                maxMeterDifferenceForSuccessfulTest
            );

            // testing the same transform as above but with the overloaded 
            // method taking a string as last parameter instead of string or integer
            AssertCoordinateResult(
                crsTransformationAdapter.TransformToCoordinate(coordinateWgs84, CrsIdentifierFactory.CreateFromEpsgNumber(epsgSweref99)),
                coordinateSweref99,
                maxMeterDifferenceForSuccessfulTest
            );
        }

        [Test]
        public void transform_fromSweref99_toWgs84()
        {
            resultWgs84 = crsTransformationAdapter.TransformToCoordinate(coordinateSweref99, epsgWGS84);
            AssertCoordinateResult(
                resultWgs84,
                coordinateWgs84,
                maxLatLongDifferenceForSuccessfulTest
            );

            // testing the same transform as above but with the overloaded 
            // method taking a string as last parameter instead of integer
            AssertCoordinateResult(
                crsTransformationAdapter.TransformToCoordinate(coordinateSweref99, crsCodeWGS84),
                coordinateWgs84,
                maxLatLongDifferenceForSuccessfulTest
            );

            // testing the same transform as above but with the overloaded 
            // method taking a string as last parameter instead of string or integer
            AssertCoordinateResult(
                crsTransformationAdapter.TransformToCoordinate(coordinateSweref99, CrsIdentifierFactory.CreateFromEpsgNumber(epsgWGS84)),
                coordinateWgs84,
                maxLatLongDifferenceForSuccessfulTest
            );
        }

        [Test]
        public void transform_fromWgs84_toRT90()
        {
            resultRT90 = crsTransformationAdapter.TransformToCoordinate(coordinateWgs84, epsgRT9025gonv);
            AssertCoordinateResult(
                resultRT90,
                coordinateRT90,
                maxMeterDifferenceForSuccessfulTest
            );
        }


        [Test]
        public void transform_fromRT90_toWgs84()
        {
            resultWgs84 = crsTransformationAdapter.TransformToCoordinate(coordinateRT90, epsgWGS84);
            AssertCoordinateResult(
                resultWgs84,
                coordinateWgs84,
                maxLatLongDifferenceForSuccessfulTest
            );
        }

        [Test]
        public void transform_fromSweref99_toRT90()
        {
            resultRT90 = crsTransformationAdapter.TransformToCoordinate(coordinateSweref99, epsgRT9025gonv);
            AssertCoordinateResult(
                resultRT90,
                coordinateRT90,
                maxMeterDifferenceForSuccessfulTest
            );
        }
        
        [Test]
        public void transform_fromRT90_toSweref99()
        {
            resultSweref99 = crsTransformationAdapter.TransformToCoordinate(coordinateRT90, epsgSweref99);
            AssertCoordinateResult(
                resultSweref99,
                coordinateSweref99,
                maxMeterDifferenceForSuccessfulTest
            );
        }

        private void AssertCoordinateResult(
            CrsCoordinate actual, 
            CrsCoordinate expected, 
            double maxDeltaDifference
        )
        {
            Assert.IsNotNull(coordinateSweref99);
            Assert.AreEqual(expected.Y, actual.Y, maxDeltaDifference);
            Assert.AreEqual(expected.X, actual.X, maxDeltaDifference);
        }

        [Test]
        public void AdapteeTypeTest() {
            Assert.IsNotNull(crsTransformationAdapter.AdapteeType);
            Assert.AreEqual(CrsTransformationAdapteeType.LEAF_SWEDISH_CRS_MLG_1_0_1, crsTransformationAdapter.AdapteeType);
        }

        [Test]
        public void IsCompositeTest() {
            Assert.IsFalse(crsTransformationAdapter.IsComposite);
        }

        [Test]
        public void LongNameOfImplementationTest() {
            string fullClassName = crsTransformationAdapter.GetType().FullName;
            Assert.AreEqual(
                fullClassName,  // expected
                crsTransformationAdapter.LongNameOfImplementation
            );
        }

        // CrsTransformationAdapterMightyLittleGeodesy
        private readonly static string PrefixForImplementations = "CrsTransformationAdapter";

        [Test]
        public void ShortNameOfImplementationTest() {
            string classNameWithoutNamespace = crsTransformationAdapter.GetType().Name;
            Assert.That(classNameWithoutNamespace, Does.StartWith(PrefixForImplementations));
            string suffix = classNameWithoutNamespace.Substring(PrefixForImplementations.Length);
            Assert.That(classNameWithoutNamespace, Does.EndWith(suffix));
            Assert.AreEqual(
                suffix,  // expected
                crsTransformationAdapter.ShortNameOfImplementation
            );
            Assert.AreEqual(
                classNameWithoutNamespace, // expected
                PrefixForImplementations + suffix
            );
        }

        [Test]
        public void GetTransformationAdapterChildrenTest() {
            IList<CrsTransformationAdapter> children = crsTransformationAdapter.GetTransformationAdapterChildren();
            Assert.AreEqual(
                0, 
                children.Count
            );
        }
    }
}
