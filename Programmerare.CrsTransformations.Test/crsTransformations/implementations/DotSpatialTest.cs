using com.programmerare.crsTransformations;
using com.programmerare.crsTransformations.coordinate;
using com.programmerare.crsTransformations.crsIdentifier;
using NUnit.Framework;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;
using com.programmerare.crsTransformations.Adapter.DotSpatial;

namespace Programmerare.CrsTransformations.Test.crsTransformations.implementations
{

    // TODO: Virtually everything in this class 
    // except from the first test method  AdapteeTypeTest
    // and the instantiation "new CrsTransformationAdapterDotSpatial()"
    // was copied from "MightyLittleGeodesyTest"
    // i.e. it should be refactored to remove the duplication

    [TestFixture]
    class DotSpatialTest
    {
    [Test]
    public void AdapteeTypeTest() {
        Assert.IsNotNull(crsTransformationAdapter.AdapteeType);
        Assert.AreEqual(
            CrsTransformationAdapteeType.LEAF_DOT_SPATIAL_1_9_0, 
            crsTransformationAdapter.AdapteeType
        );
    }

        // TODO instead of adding lots of test to this class:
        // Refactor MightyLittleGeodesyTest to reuse those tests !

        private CrsTransformationAdapter crsTransformationAdapter;

        double maxMeterDifferenceForSuccessfulTest = 0.5; // 0.5 meter
        double maxLatLongDifferenceForSuccessfulTest = 0.00001;


        private const double wgs84Lat = 59.330231;
        private const double wgs84Lon = 18.059196;
        private const double sweref99Y = 6580822;
        private const double sweref99X = 674032;
        private const string EPSG_PREFIX = "EPSG:";
        private static readonly string crsCodeWGS84 = EPSG_PREFIX + epsgWGS84;
        private static readonly string crsCodeSweref99 = EPSG_PREFIX + epsgSweref99;

        // These three are used as input parameters and as expected values
        private CrsCoordinate coordinateWgs84, coordinateSweref99, coordinateRT90;
        // These below will be used as the actual values and be asserted with the expected values (i.e. the above objects)
        private CrsCoordinate resultWgs84, resultSweref99, resultRT90;

        private const int epsgWGS84 =       EpsgNumber.WORLD__WGS_84__4326;
        private const int epsgSweref99 =    EpsgNumber.SWEDEN__SWEREF99_TM__3006;

        [SetUp]
        public void SetUp()
        {
            crsTransformationAdapter = new CrsTransformationAdapterDotSpatial();
            coordinateWgs84 = CrsCoordinateFactory.LatLon(wgs84Lat, wgs84Lon, epsgWGS84);
            coordinateSweref99 = CrsCoordinateFactory.LatLon(sweref99Y, sweref99X, epsgSweref99);
        }

        ////////////////////////////////////
        ///////////////////////////////////////
        ///////////////////////////////////////
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

        ////////////////////////////////////
        ///////////////////////////////////////
        ///////////////////////////////////////
    }
}
