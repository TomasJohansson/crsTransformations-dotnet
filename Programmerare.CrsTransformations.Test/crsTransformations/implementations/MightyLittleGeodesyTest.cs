using com.programmerare.crsTransformations;
using com.programmerare.crsTransformations.coordinate;
using com.programmerare.crsTransformations.crsIdentifier;
using NUnit.Framework;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;
using System;
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
        private const int epsgWGS84 =       EpsgNumber.WORLD__WGS_84__4326;
        private const int epsgSweref99 =    EpsgNumber.SWEDEN__SWEREF99_TM__3006;
        private const int epsgRT9025gonv =  EpsgNumber.SWEDEN__2_5_GON_W__RT90_2_5_GON_V__3021;// RT90 2.5 gon V
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

        [Test]
        public void transformResult_fromRT90_toSweref99()
        {
            CrsTransformationResult result = crsTransformationAdapter.Transform(coordinateRT90, epsgSweref99);
            AssertTransformationResultSuccess(
                result, 
                coordinateRT90, 
                coordinateSweref99, 
                crsTransformationAdapter,
                maxMeterDifferenceForSuccessfulTest
            );

            // testing the same transform as above but with the overloaded 
            // method taking a string as last parameter instead of integer
            AssertTransformationResultSuccess(
                crsTransformationAdapter.Transform(coordinateRT90, crsCodeSweref99), 
                coordinateRT90, 
                coordinateSweref99, 
                crsTransformationAdapter,
                maxMeterDifferenceForSuccessfulTest
            );
            
            // testing the same transform as above but with the overloaded 
            // method taking a string as last parameter instead of string or integer
            AssertTransformationResultSuccess(
                crsTransformationAdapter.Transform(coordinateRT90, CrsIdentifierFactory.CreateFromEpsgNumber(epsgSweref99)),
                coordinateRT90, 
                coordinateSweref99, 
                crsTransformationAdapter,
                maxMeterDifferenceForSuccessfulTest
            );
        }

        private void AssertTransformationResultSuccess(
            CrsTransformationResult result, 
            CrsCoordinate inputCoordinate, 
            CrsCoordinate expectedOutputCoordinate, 
            CrsTransformationAdapter crsTransformationAdapterSource,
            double maxDeltaDifference
        )
        {
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.Exception);
            AssertCoordinateResult(
                result.OutputCoordinate,
                expectedOutputCoordinate, 
                maxDeltaDifference
            );
            IList<CrsTransformationResult> subresults = result.GetTransformationResultChildren();
            Assert.IsNotNull(subresults);
            Assert.AreEqual(0, subresults.Count); // Leaf should have no children
            Assert.AreEqual(this.crsTransformationAdapter, result.CrsTransformationAdapterResultSource);

            AssertStatisticsForLeaf(result);
        }

        [Test]
        public void transformToCoordinate_WhenCrsIsUnvalidForSpecificImplementation()
        {
            int epsgNotSupported = 123; // not supported by MightyLittleGeodesy
            // TransformToCoordinate SHOULD (unlike the transform method) 
            // throw exception 
            ArgumentException exception = Assert.Throws<ArgumentException>( () => {
                crsTransformationAdapter.TransformToCoordinate(coordinateRT90, epsgNotSupported);
            });
        }


        [Test]
        public void transformToCoordinate_WhenCrsIsUnvalidForAllImplementations()
        {
            int epsgNotSupported = -99999999;
            // TransformToCoordinate SHOULD (unlike the transform method) 
            // throw exception 
            ArgumentException exception = Assert.Throws<ArgumentException>( () => {
                crsTransformationAdapter.TransformToCoordinate(coordinateRT90, epsgNotSupported);
            });
        }

        [Test]
        public void transformToCoordinate_WhenCrsCodeIsNull()
        {
            string crsCode = null;
            // TransformToCoordinate SHOULD (unlike the transform method) 
            // throw exception 
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>( () => {
                crsTransformationAdapter.TransformToCoordinate(coordinateRT90, crsCode);
            });
        }

        [Test]
        public void transformToCoordinate_WhenInputCoordinateIsNull()
        {
            CrsIdentifier crsWgs84 = coordinateWgs84.CrsIdentifier;
            Assert.IsNotNull(crsWgs84);
            CrsCoordinate nullCordinate = null;
            // TransformToCoordinate SHOULD (unlike the transform method) 
            // throw exception 
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>( () => {
                crsTransformationAdapter.TransformToCoordinate(nullCordinate, crsWgs84);
            });
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
        public void transformResult_WhenCrsIsUnvalidForSpecificImplementation()
        {
            // Negative epsgEPSG values are generally unvalid 
            // and should be handle in a generic way i.e. 
            // without having to implement it in all implementations
            //int epsgNotSupported = -99999999; // test this in another methods
            int epsgNotSupported = 123; // not supported by MightyLittleGeodesy
            // The transform should not throw exception and not be null
            // but instead it should return a result object with 
            // IsSuccess property being false
            CrsTransformationResult result = crsTransformationAdapter.Transform(coordinateRT90, epsgNotSupported);
            AssertTransformationResultFailure(
                result,
                coordinateRT90,
                crsTransformationAdapter
            );
        }


        [Test]
        public void transformResult_WhenCrsIsUnvalidForAllImplementations()
        {
            // Negative epsgEPSG values are generally unvalid 
            // and should be handle in a generic way i.e. 
            // without having to implement it in all implementations
            int epsgNotSupported = -99999999;
            //int epsgNotSupported = 123; // test this in another method
            // The transform should not throw exception and not be null
            // but instead it should return a result object with 
            // IsSuccess property being false
            CrsTransformationResult result = crsTransformationAdapter.Transform(coordinateRT90, epsgNotSupported);
            AssertTransformationResultFailure(
                result,
                coordinateRT90,
                crsTransformationAdapter
            );
        }

        [Test]
        public void transformResult_WhenCrsCodeIsNull()
        {
            string crsCode = null;
            CrsTransformationResult result = crsTransformationAdapter.Transform(coordinateRT90, crsCode);
            AssertTransformationResultFailure(
                result,
                coordinateRT90,
                crsTransformationAdapter
            );
        }

        [Test]
        public void transformResult_WhenInputCoordinateIsNull()
        {
            CrsIdentifier crsWgs84 = coordinateWgs84.CrsIdentifier;
            Assert.IsNotNull(crsWgs84);
            CrsCoordinate nullCordinate = null;
            CrsTransformationResult result = crsTransformationAdapter.Transform(nullCordinate, crsWgs84);
            AssertTransformationResultFailure(
                result,
                nullCordinate,
                crsTransformationAdapter
            );
        }

        private void AssertTransformationResultFailure(
            CrsTransformationResult result, 
            CrsCoordinate inputCoordinate, 
            //CrsCoordinate expectedOutputCoordinate, 
            CrsTransformationAdapter crsTransformationAdapterSource
        )
        {
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.IsNull(result.OutputCoordinate);
            Assert.AreEqual(inputCoordinate, result.InputCoordinate);
            IList<CrsTransformationResult> subresults = result.GetTransformationResultChildren();
            Assert.IsNotNull(subresults);
            Assert.AreEqual(0, subresults.Count); // Leaf should have no children
            Assert.AreEqual(this.crsTransformationAdapter, result.CrsTransformationAdapterResultSource);

            AssertStatisticsForLeaf(result);
        }
        private void AssertStatisticsForLeaf(CrsTransformationResult result)
        {
            var stat = result.CrsTransformationResultStatistic;
            Assert.IsNotNull(stat);

            // The below assertions should be true for leafs
            Assert.AreEqual(result.IsSuccess, stat.IsStatisticsAvailable);

            int expectedNumberOfSuccessResultsForLeaf = result.IsSuccess ? 1 : 0;
            // note that there is no guarantee for a correct result 
            // i.e. "success" might just mean that an exception was not thrown ...
            Assert.AreEqual(
                expectedNumberOfSuccessResultsForLeaf, 
                stat.NumberOfPotentiallySuccesfulResults 
            );

            IList<CrsTransformationResult> allResults = stat.GetAllCrsTransformationResults();
            Assert.AreEqual(1, allResults.Count);

            if (stat.IsStatisticsAvailable)
            {
                // Since we are not testing a Leaf
                // there should only one result
                // and therefore now differences in the 
                // below tests
                Assert.AreEqual(
                    0.0, 
                    stat.MaxDifferenceForXEastingLongitude
                );
                Assert.AreEqual(
                    0.0, 
                    stat.MaxDifferenceForYNorthingLatitude
                );
                //double smallValue = 0.000000000000001;
                //Assert.That(stat.MaxDifferenceForXEastingLongitude, Is.LessThan(smallValue));
            
                Assert.AreEqual(
                    result.OutputCoordinate, 
                    stat.CoordinateAverage
                );

                Assert.AreEqual(
                    result.OutputCoordinate, 
                    stat.CoordinateMedian
                );
            }
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
