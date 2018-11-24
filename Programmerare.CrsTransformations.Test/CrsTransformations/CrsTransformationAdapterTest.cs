using System;
using System.Collections.Generic;
using NUnit.Framework;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.CompositeTransformations;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI;

namespace Programmerare.CrsTransformations
{
public class CrsTransformationAdapterTest : CrsTransformationTestBase {
	
	private const int NUMBER_OF_LEAF_IMPLEMENTATIONS = 3;
	// TODO the above is defined in some other place too.. find it and remove duplication ....

    // the keyword "base" is not needed but is still used in this test class 
    // to make it obvious that some variables ar defined and populated in a base class

    [Test]
    public void theBaseClass_shouldHaveCreatedThreeLeafAndFourCompositeImplementations() {
        int expectedNumberOfLeafs = NUMBER_OF_LEAF_IMPLEMENTATIONS;
        int expectedNumberOfComposites = 4; // will not change often and if/when changed then will be easy to fix
        Assert.AreEqual(expectedNumberOfLeafs, base.crsTransformationAdapterLeafImplementations.Count);
        Assert.AreEqual(expectedNumberOfComposites, base.crsTransformationAdapterCompositeImplementations.Count);
        Assert.AreEqual(expectedNumberOfLeafs + expectedNumberOfComposites, base.crsTransformationAdapterImplementations.Count);
    }
    
    [Test]
    public void transformToCoordinate_shouldReturnCorrectSweref99TMcoordinate_whenTransformingFromWgs84withAllAdapterImplementations() {
        foreach (ICrsTransformationAdapter crsTransformationAdapter in base.crsTransformationAdapterImplementations) {
            transformToCoordinate_shouldReturnCorrectSweref99TMcoordinate_whenTransformingFromWgs84(crsTransformationAdapter);
        }
    }

    [Test]
    public void transformToCoordinate_shouldReturnCorrectWgs84coordinate_whenTransformingFromRT90withAllAdapterImplementations() {
        foreach (ICrsTransformationAdapter crsTransformationAdapter in base.crsTransformationAdapterImplementations) {
            transformToCoordinate_shouldReturnCorrectWgs84coordinate_whenTransformingFromRT90(crsTransformationAdapter);
        }
    }

    [Test]
    public void transform_shouldSuccess_whenInputCoordinateIsCorrect() {
        double wgs84Lat = 59.330231;
        double wgs84Lon = 18.059196;
        CrsCoordinate wgs84InputCoordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(wgs84Lon, wgs84Lat, base.epsgNumberForWgs84);

        foreach (ICrsTransformationAdapter crsTransformationAdapter in base.crsTransformationAdapterImplementations) {
            CrsTransformationResult transformResult = crsTransformationAdapter.Transform(wgs84InputCoordinate, base.epsgNumberForSweref99TM);
            string errorMessage = "Problem implementation: " + crsTransformationAdapter.AdapteeType;
            Assert.IsNotNull(transformResult, errorMessage);
            Assert.IsTrue(transformResult.IsSuccess, errorMessage);
            Assert.IsNull(transformResult.Exception, errorMessage);
            CrsCoordinate outputCoordinate = transformResult.OutputCoordinate;
            Assert.IsNotNull(outputCoordinate, errorMessage);
            Assert.AreEqual(outputCoordinate.CrsIdentifier.EpsgNumber, base.epsgNumberForSweref99TM, errorMessage);
            if(!crsTransformationAdapter.IsComposite) {
                assertResultStatisticsForLeafImplementation(transformResult);
            }
        }
    }


    [Test]
    public void transform_shouldReturnSuccessFalseButNotThrowException_whenLongitudeIsNotValid() {
        CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.LatLon(
            60.0, // ok wgs84 latitude
            -9999999999.0, // NOT ok wgs84 longitude
            epsgNumberForWgs84
        );
        transform_shouldReturnSuccessFalseButNotThrowException_whenCoordinateIsNotValid(unvalidInputCoordinate);
    }

    [Test]
    public void transform_shouldReturnSuccessFalseButNotThrowException_whenLatitudeIsNotValid() {
        CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.LatLon(
            -9999999999.0, // NOT ok wgs84 latitude
            20.0, // ok wgs84 longitude
            epsgNumberForWgs84
        );
        transform_shouldReturnSuccessFalseButNotThrowException_whenCoordinateIsNotValid(unvalidInputCoordinate);
    }

    [Test]
    public void transform_shouldReturnSuccessFalseButNotThrowException_whenCrsCodeIsNotValid() {
        CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.LatLon(
            60.0, // ok wgs84 latitude
            20.0, // ok wgs84 longitude
            "This string is NOT a correct crs/EPSG code"
        );
        transform_shouldReturnSuccessFalseButNotThrowException_whenCoordinateIsNotValid(unvalidInputCoordinate);
    }
    
    private void transform_shouldReturnSuccessFalseButNotThrowException_whenCoordinateIsNotValid(
        CrsCoordinate unvalidInputCoordinate
    ) {
        foreach (ICrsTransformationAdapter crsTransformationAdapter in crsTransformationAdapterImplementations) {
            string messageWhenError = "Problem with the implementation " + crsTransformationAdapter.AdapteeType;
            CrsTransformationResult transformResult = crsTransformationAdapter.Transform(unvalidInputCoordinate, epsgNumberForSweref99TM);
            Assert.IsNotNull(transformResult, messageWhenError);
            if(!unvalidInputCoordinate.CrsIdentifier.IsEpsgCode) {
                // if the coordinate is unvalid becasue of incorrect crs code
                // then we can do the below assertions but if the coordinate values 
                // are unreasonable it may not be detected by the implementations
                // and we can not expect them to return success=false 
                Assert.IsFalse(transformResult.IsSuccess, messageWhenError);
                Assert.IsNotNull(transformResult.Exception, messageWhenError);
                Assert.AreEqual(unvalidInputCoordinate, transformResult.InputCoordinate, messageWhenError);
            }
        }        
    }
// These two test below could not be used since all implementations are 
// not throwing exceptions for unreasonable coordinate values    
//    @Test
//    void transformToCoordinate_shouldThrowException_whenLongitudeIsNotValid() {
//        CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.latLon(
//            60.0, // ok wgs84 latitude
//            -9999999999.0, // NOT ok wgs84 longitude
//            epsgNumberForWgs84
//        );
//        transformToCoordinate_shouldThrowException_whenCoordinateIsNotValid(unvalidInputCoordinate);
//    }
//
//    @Test
//    void transformToCoordinate_shouldThrowException_whenLatitudeIsNotValid() {
//        CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.latLon(
//            -9999999999.0, // NOT ok wgs84 latitude
//            20.0, // ok wgs84 longitude
//            epsgNumberForWgs84
//        );
//        transformToCoordinate_shouldThrowException_whenCoordinateIsNotValid(unvalidInputCoordinate);
//    }

    [Test]
    public void transformToCoordinate_shouldThrowException_whenCrsCodeIsNotValid() {
        CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.LatLon(
            60.0, // ok wgs84 latitude
            20.0, // ok wgs84 longitude
            "This string is NOT a correct crs/EPSG code"
        );
        transformToCoordinate_shouldThrowException_whenCoordinateIsNotValid(unvalidInputCoordinate);
    }

    private static List<String> classNamesForExpectedPotentialExceptionsWhenIncorrectEPSGcode =
			// TODO: these below were implemented for the Java adapeters ...
            new List<string>{
                //IllegalArgumentException.class.getName(), // Goober implementation throws this
                "org.opengis.referencing.NoSuchAuthorityCodeException",
                "org.osgeo.proj4j.UnknownAuthorityCodeException",
                "org.cts.crs.CRSException",
                "mil.nga.sf.util.SFException"
                //RuntimeException.class.getName() // composite throws this
            };
    
    private void transformToCoordinate_shouldThrowException_whenCoordinateIsNotValid(
        CrsCoordinate unvalidInputCoordinate
    ) {
			// TODO implement below for .NET
        foreach (ICrsTransformationAdapter crsTransformationAdapter in crsTransformationAdapterImplementations) {
            //Exception exception = assertThrows(
            //    Exception.class,
            //    () -> crsTransformationAdapter.transformToCoordinate(unvalidInputCoordinate, base.epsgNumberForSweref99TM),
            //    () -> "Exception was not thrown but SHOULD have been thrown for implementation " + crsTransformationAdapter.getAdapteeType() + " and coordinate " + unvalidInputCoordinate 
            //);

            //bool isExpectedException = classNamesForExpectedPotentialExceptionsWhenIncorrectEPSGcode.stream().anyMatch(it -> it.equals(exception.getClass().getName()));
            //Assert.IsTrue(isExpectedException, () -> "Unexpected exception: " + exception.getClass().getName() + " for adapter " + crsTransformationAdapter.getAdapteeType());
        }
    }
    
    
    [Test]
    public void getLongNameOfImplementation_shouldReturnFullClassNameIncludingPackageName() {
        // Of course fragile, but the class/package name will not change
        // often and if/when it does the test will fail but will be trivial to fix.
        // The purpose of this test is not only to "test" but rather to
        // illustrate what the method returns
        Assert.AreEqual(
            "Programmerare.CrsTransformations.Adapter.DotSpatial.CrsTransformationAdapterDotSpatial",
            (new CrsTransformationAdapterDotSpatial()).LongNameOfImplementation
        );
        // There are more related tests in 'CrsTransformationAdapterLeafFactoryTest'
        // which will detect problems if a class is renamed        
    }

    [Test]
    public void getShortNameOfImplementation_shouldReturnUniqueSuffixPartOfTheClassName() {
        // Of course fragile, but the class/package name will not change
        // often and if/when it does the test will fail but will be trivial to fix.
        // The purpose of this test is not only to "test" but also to
        // illustrate what the method returns
        Assert.AreEqual(
            "DotSpatial",
            (new CrsTransformationAdapterDotSpatial()).ShortNameOfImplementation
        );

        Assert.AreEqual(
            "MightyLittleGeodesy",
            (new CrsTransformationAdapterMightyLittleGeodesy()).ShortNameOfImplementation
        );

        Assert.AreEqual(
            "ProjNet4GeoAPI",
            (new CrsTransformationAdapterProjNet4GeoAPI()).ShortNameOfImplementation
        );

        // The above tests are for the "Leaf" implementations.
        // Below is a "Composite" created
        CrsTransformationAdapterComposite compositeAdapter = CrsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage();
        // The class name for the above adapter is "CrsTransformationAdapterComposite"
        // and the prefix part "CrsTransformationAdapter" is removed from the name
        // to get the short implementation i.e. just "Composite"
        Assert.AreEqual(
            "Composite",
            compositeAdapter.ShortNameOfImplementation
        );
    }

    [Test]
    public void isComposite_shouldReturnTrue_whenComposite() {
        foreach (ICrsTransformationAdapter compositeAdapter in base.crsTransformationAdapterCompositeImplementations) {
            Assert.IsTrue(compositeAdapter.IsComposite);
        }
    }
    
    [Test]
    public void isComposite_shouldReturnFalse_whenLeaf() {
        foreach (ICrsTransformationAdapter leafAdapter in base.crsTransformationAdapterLeafImplementations) {
            Assert.IsFalse(leafAdapter.IsComposite);
        }
    }    

    [Test]
    public void getTransformationAdapterChildren_shouldReturnNonEmptyList_whenComposite() {
        // all leafs should be children 
        int expectedNumberOfChildrenForTheComposites = base.crsTransformationAdapterLeafImplementations.Count;
        Assert.That(
            //"Has the number of leaf implementations been reduced?",
            expectedNumberOfChildrenForTheComposites, Is.GreaterThanOrEqualTo(NUMBER_OF_LEAF_IMPLEMENTATIONS)
        ); 
        foreach (ICrsTransformationAdapter compositeAdapter in base.crsTransformationAdapterCompositeImplementations) {
            Assert.AreEqual(
                expectedNumberOfChildrenForTheComposites, 
                compositeAdapter.GetTransformationAdapterChildren().Count
            );    
        }
    }

    [Test]
    public void getTransformationAdapterChildren_shouldReturnEmptyList_whenLeaf() {
        int zeroExpectedNumberOfChildren = 0;
        foreach (ICrsTransformationAdapter leafAdapter in base.crsTransformationAdapterLeafImplementations) {
            Assert.AreEqual(
                zeroExpectedNumberOfChildren,
                leafAdapter.GetTransformationAdapterChildren().Count
            );
        }
    }
    
    [Test]
    public void isReliable_shouldReturnTrueForLeafs_whenUsingCriteriaNumberOfResultsOneAndMaxDiffZero() {
        int criteriaNumberOfResults = 1; // always one for a Leaf
        double criteriaMaxDiff = 0.0; // always zero for a Leaf
        // The tested method 'isReliable' is actually relevant only for aggregated
        // transformations, but nevertheless there is a reaonable behavouor also
        // for the "Leaf" implementations regarding the number of results (always 1)
        // and the "differences" in lat/long for the "different" implementations
        // i.e. the "difference" should always be zero since there is only one implementation

        List<ICrsTransformationAdapter> crsTransformationAdapterImplementationsExpectingOneResult = new List<ICrsTransformationAdapter>();
        crsTransformationAdapterImplementationsExpectingOneResult.AddRange(base.crsTransformationAdapterLeafImplementations);
        crsTransformationAdapterImplementationsExpectingOneResult.Add(CrsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess());
        Assert.AreEqual(
			NUMBER_OF_LEAF_IMPLEMENTATIONS + 1,  // 3 leafs plus 1 
			crsTransformationAdapterImplementationsExpectingOneResult.Count
		);
        
        foreach (ICrsTransformationAdapter crsTransformationAdapterLeaf in crsTransformationAdapterImplementationsExpectingOneResult) {
            // suffix "Leaf" is not quite true, but in one of the iterations
            // it will be the Composite FirstSuccess which also will only have one result 
            // and thus can be tested in the same way as the leafs in this method
            CrsCoordinate wgs84coordinateInSweden = CrsCoordinateFactory.LatLon(59.29,18.03);
            CrsTransformationResult resultWhenTransformingToSwedishCRS = crsTransformationAdapterLeaf.Transform(wgs84coordinateInSweden, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
            Assert.IsNotNull(resultWhenTransformingToSwedishCRS);
            Assert.IsTrue(resultWhenTransformingToSwedishCRS.IsSuccess);
            CrsTransformationResultStatistic crsTransformationResultStatistic = resultWhenTransformingToSwedishCRS.CrsTransformationResultStatistic;
            Assert.IsNotNull(crsTransformationResultStatistic);
            Assert.IsTrue(crsTransformationResultStatistic.IsStatisticsAvailable);
    
            int actualNumberOfResults = crsTransformationResultStatistic.NumberOfPotentiallySuccesfulResults;
            Assert.AreEqual(criteriaNumberOfResults, actualNumberOfResults);
            double actualMaxDiffXLongitude = crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude;
            double actualMaxDiffYLatitude = crsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude;
            double actualMaxDiffXorY = Math.Max(actualMaxDiffXLongitude, actualMaxDiffYLatitude);
            Assert.AreEqual(criteriaMaxDiff, actualMaxDiffXorY); // zero differences since there should be only one result !

            // method "isReliable" used below is the method under test
            
            Assert.IsTrue(resultWhenTransformingToSwedishCRS.IsReliable(criteriaNumberOfResults, criteriaMaxDiff));
    
            // Assert.IsFalse below since trying to require one more result than available
            Assert.IsFalse(resultWhenTransformingToSwedishCRS.IsReliable(criteriaNumberOfResults + 1, criteriaMaxDiff));
    
            // Assert.IsFalse below since trying to require too small maxdiff
            Assert.IsFalse(resultWhenTransformingToSwedishCRS.IsReliable(criteriaNumberOfResults, criteriaMaxDiff - 0.00000000001));
        }
    }

    [Test]
    public void isReliable_shouldReturnTrueOrFalseForComposites_dependingOnCriteriasUsedAsMethodParameter() {
        // the below value is the max difference when comparing the five leaf implementations 
        double actualMaxDiffXorY = 0.0032574664801359177;
       
        int criteriaNumberOfResultsSuccess = NUMBER_OF_LEAF_IMPLEMENTATIONS; // all 5 should succeed
        int criteriaNumberOfResultsFailure = NUMBER_OF_LEAF_IMPLEMENTATIONS + 1; // 6 implementations can not succeed since there are not so many implementations
        double criteriaMaxDiffFailure = actualMaxDiffXorY - 0.0001; // a little too small requirement for max difference
        double criteriaMaxDiffSuccess  = actualMaxDiffXorY + 0.0001; // should result in success since the actual number is smaller 

        List<ICrsTransformationAdapter> crsTransformationAdapterImplementationsExpectingManyResults = new List<ICrsTransformationAdapter>();
        foreach (ICrsTransformationAdapter crsTransformationAdapterComposite in base.crsTransformationAdapterCompositeImplementations) {
            if(crsTransformationAdapterComposite.AdapteeType != CrsTransformationAdapteeType.COMPOSITE_FIRST_SUCCESS) {
                crsTransformationAdapterImplementationsExpectingManyResults.Add(crsTransformationAdapterComposite);
            }
        }
        Assert.AreEqual(3, crsTransformationAdapterImplementationsExpectingManyResults.Count); // 3 composites i.e. all composite except COMPOSITE_FIRST_SUCCESS
        
        foreach (ICrsTransformationAdapter crsTransformationAdapterComposite in crsTransformationAdapterImplementationsExpectingManyResults) {
            CrsCoordinate wgs84coordinateInSweden = CrsCoordinateFactory.LatLon(59.29,18.03);
            CrsTransformationResult resultWhenTransformingToSwedishCRS = crsTransformationAdapterComposite.Transform(wgs84coordinateInSweden, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
            Assert.IsNotNull(resultWhenTransformingToSwedishCRS);
            Assert.IsTrue(resultWhenTransformingToSwedishCRS.IsSuccess);
            CrsTransformationResultStatistic crsTransformationResultStatistic = resultWhenTransformingToSwedishCRS.CrsTransformationResultStatistic;
            Assert.IsNotNull(crsTransformationResultStatistic);
            Assert.IsTrue(crsTransformationResultStatistic.IsStatisticsAvailable);
            // so far the same code as the previous test method for the test method with leafs
            
            int actualNumberOfResults = crsTransformationResultStatistic.NumberOfPotentiallySuccesfulResults;
            Assert.AreEqual(
				criteriaNumberOfResultsSuccess, 
				actualNumberOfResults
			);
            double actualMaxDiffXLongitude = crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude;
            double actualMaxDiffYLatitude = crsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude;
            // double actualMaxDiffXorY = Math.max(actualMaxDiffXLongitude, actualMaxDiffYLatitude);
            // System.out.println("actualMaxDiffXorY " + actualMaxDiffXorY + "  " + crsTransformationAdapterComposite.getAdapteeType());

            // method "isReliable" used below is the method under test
            Assert.IsTrue(resultWhenTransformingToSwedishCRS.IsReliable(criteriaNumberOfResultsSuccess, criteriaMaxDiffSuccess));
            Assert.IsFalse(resultWhenTransformingToSwedishCRS.IsReliable(criteriaNumberOfResultsSuccess, criteriaMaxDiffFailure));
            Assert.IsFalse(resultWhenTransformingToSwedishCRS.IsReliable(criteriaNumberOfResultsFailure, criteriaMaxDiffSuccess));
            Assert.IsFalse(resultWhenTransformingToSwedishCRS.IsReliable(criteriaNumberOfResultsFailure, criteriaMaxDiffFailure));
        }
    }

    private void transformToCoordinate_shouldReturnCorrectSweref99TMcoordinate_whenTransformingFromWgs84(
        ICrsTransformationAdapter crsTransformationAdapter
    ) {
        // This test is using the coordinates of Stockholm Centralstation (Sweden)
        // https://kartor.eniro.se/m/03Yxp
        // WGS84 decimal (lat, lon)
        // 59.330231, 18.059196
        // SWEREF99 TM (nord, öst)
        // 6580822, 674032

        double wgs84Lat = 59.330231;
        double wgs84Lon = 18.059196;

        double sweref99_Y_expected = 6580822;
        double sweref99_X_expected = 674032;

        CrsCoordinate inputCoordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(wgs84Lon, wgs84Lat, base.epsgNumberForWgs84);
        CrsCoordinate outputCoordinate = crsTransformationAdapter.TransformToCoordinate(inputCoordinate, base.epsgNumberForSweref99TM);
        Assert.AreEqual(sweref99_Y_expected, outputCoordinate.YNorthingLatitude, 0.5);
        Assert.AreEqual(sweref99_X_expected, outputCoordinate.XEastingLongitude, 0.5);
    }

    private void transformToCoordinate_shouldReturnCorrectWgs84coordinate_whenTransformingFromRT90(
        ICrsTransformationAdapter crsTransformationAdapter
    ) {
        double rt90_Y = 6580994;
        double rt90_X = 1628294;

        double wgs84Lat_expected = 59.330231;
        double wgs84Lon_expected = 18.059196;

        CrsCoordinate inputCoordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(rt90_X, rt90_Y, base.epsgNumberForRT90);

        CrsCoordinate outputCoordinate = crsTransformationAdapter.TransformToCoordinate(inputCoordinate, base.epsgNumberForWgs84);
        Assert.AreEqual(wgs84Lat_expected, outputCoordinate.YNorthingLatitude, 0.1);
        Assert.AreEqual(wgs84Lon_expected, outputCoordinate.XEastingLongitude, 0.1);
    }

    private void assertResultStatisticsForLeafImplementation(
        CrsTransformationResult transformResult
    ) {
        CrsTransformationResultStatistic crsTransformationResultStatistic = transformResult.CrsTransformationResultStatistic;
        Assert.IsNotNull(crsTransformationResultStatistic);
        Assert.IsTrue(crsTransformationResultStatistic.IsStatisticsAvailable);
        Assert.AreEqual(1, crsTransformationResultStatistic.NumberOfPotentiallySuccesfulResults);
        Assert.AreEqual(0, crsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude);
        Assert.AreEqual(0, crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude);
        Assert.AreEqual(transformResult.OutputCoordinate, crsTransformationResultStatistic.CoordinateAverage);
        Assert.AreEqual(transformResult.OutputCoordinate, crsTransformationResultStatistic.CoordinateMedian);
    }
    
}
}