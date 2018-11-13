package com.programmerare.crsTransformations;

import com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA;
import com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools;
import com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL;
import com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS;
import com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J;
import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterComposite;
import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterCompositeFactory;
import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsTransformations.coordinate.CrsCoordinateFactory;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static org.hamcrest.MatcherAssert.assertThat;
import static org.hamcrest.number.OrderingComparison.greaterThanOrEqualTo;
import static org.junit.jupiter.api.Assertions.*;
import static org.junit.jupiter.api.Assertions.assertThrows;

final class CrsTransformationAdapterTest extends CrsTransformationTestBase {

    // the keyword "super" is not needed but is still used in this test class 
    // to make it obvious that some variables ar defined and populated in a base class

    @Test
    void theBaseClass_shouldHaveCreatedFiveLeafAndFourCompositeImplementations() {
        final int expectedNumberOfLeafs = 5; // will not change often and if/when changed then will be easy to fix
        int expectedNumberOfComposites = 4; // will not change often and if/when changed then will be easy to fix
        assertEquals(expectedNumberOfLeafs, super.crsTransformationAdapterLeafImplementations.size());
        assertEquals(expectedNumberOfComposites, super.crsTransformationAdapterCompositeImplementations.size());
        assertEquals(expectedNumberOfLeafs + expectedNumberOfComposites, super.crsTransformationAdapterImplementations.size());
    }
    
    @Test
    void transformToCoordinate_shouldReturnCorrectSweref99TMcoordinate_whenTransformingFromWgs84withAllAdapterImplementations() {
        for (CrsTransformationAdapter crsTransformationAdapter : super.crsTransformationAdapterImplementations) {
            transformToCoordinate_shouldReturnCorrectSweref99TMcoordinate_whenTransformingFromWgs84(crsTransformationAdapter);
        }
    }

    @Test
    void transformToCoordinate_shouldReturnCorrectWgs84coordinate_whenTransformingFromRT90withAllAdapterImplementations() {
        for (CrsTransformationAdapter crsTransformationAdapter : super.crsTransformationAdapterImplementations) {
            transformToCoordinate_shouldReturnCorrectWgs84coordinate_whenTransformingFromRT90(crsTransformationAdapter);
        }
    }

    @DisplayName("Testing CrsTransformationResult with expected successe")
    @Test
    void transform_shouldSuccess_whenInputCoordinateIsCorrect() {
        double wgs84Lat = 59.330231;
        double wgs84Lon = 18.059196;
        CrsCoordinate wgs84InputCoordinate = CrsCoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude(wgs84Lon, wgs84Lat, super.epsgNumberForWgs84);

        for (CrsTransformationAdapter crsTransformationAdapter : super.crsTransformationAdapterImplementations) {
            CrsTransformationResult transformResult = crsTransformationAdapter.transform(wgs84InputCoordinate, super.epsgNumberForSweref99TM);
            assertNotNull(transformResult);
            assertTrue(transformResult.isSuccess());
            assertNull(transformResult.getException());
            CrsCoordinate outputCoordinate = transformResult.getOutputCoordinate();
            assertNotNull(outputCoordinate);
            assertEquals(outputCoordinate.getCrsIdentifier().getEpsgNumber(), super.epsgNumberForSweref99TM);
            if(!crsTransformationAdapter.isComposite()) {
                assertResultStatisticsForLeafImplementation(transformResult);
            }
        }
    }


    @Test
    void transform_shouldReturnSuccessFalseButNotThrowException_whenLongitudeIsNotValid() {
        final CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.latLon(
            60.0, // ok wgs84 latitude
            -9999999999.0, // NOT ok wgs84 longitude
            epsgNumberForWgs84
        );
        transform_shouldReturnSuccessFalseButNotThrowException_whenCoordinateIsNotValid(unvalidInputCoordinate);
    }

    @Test
    void transform_shouldReturnSuccessFalseButNotThrowException_whenLatitudeIsNotValid() {
        final CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.latLon(
            -9999999999.0, // NOT ok wgs84 latitude
            20.0, // ok wgs84 longitude
            epsgNumberForWgs84
        );
        transform_shouldReturnSuccessFalseButNotThrowException_whenCoordinateIsNotValid(unvalidInputCoordinate);
    }

    @Test
    void transform_shouldReturnSuccessFalseButNotThrowException_whenCrsCodeIsNotValid() {
        final CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.latLon(
            60.0, // ok wgs84 latitude
            20.0, // ok wgs84 longitude
            "This string is NOT a correct crs/EPSG code"
        );
        transform_shouldReturnSuccessFalseButNotThrowException_whenCoordinateIsNotValid(unvalidInputCoordinate);
    }
    
    private void transform_shouldReturnSuccessFalseButNotThrowException_whenCoordinateIsNotValid(
        CrsCoordinate unvalidInputCoordinate
    ) {
        for (CrsTransformationAdapter crsTransformationAdapter : crsTransformationAdapterImplementations) {
            final String messageWhenError = "Problem with the implementation " + crsTransformationAdapter.getAdapteeType();
            CrsTransformationResult transformResult = crsTransformationAdapter.transform(unvalidInputCoordinate, epsgNumberForSweref99TM);
            assertNotNull(transformResult, messageWhenError);
            if(!unvalidInputCoordinate.getCrsIdentifier().isEpsgCode()) {
                // if the coordinate is unvalid becasue of incorrect crs code
                // then we can do the below assertions but if the coordinate values 
                // are unreasonable it may not be detected by the implementations
                // and we can not expect them to return success=false 
                assertFalse(transformResult.isSuccess(), messageWhenError);
                assertNotNull(transformResult.getException(), messageWhenError);
                assertEquals(unvalidInputCoordinate, transformResult.getInputCoordinate(), messageWhenError);                
            }
        }        
    }
// These two test below could not be used since all implementations are 
// not throwing exceptions for unreasonable coordinate values    
//    @Test
//    void transformToCoordinate_shouldThrowException_whenLongitudeIsNotValid() {
//        final CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.latLon(
//            60.0, // ok wgs84 latitude
//            -9999999999.0, // NOT ok wgs84 longitude
//            epsgNumberForWgs84
//        );
//        transformToCoordinate_shouldThrowException_whenCoordinateIsNotValid(unvalidInputCoordinate);
//    }
//
//    @Test
//    void transformToCoordinate_shouldThrowException_whenLatitudeIsNotValid() {
//        final CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.latLon(
//            -9999999999.0, // NOT ok wgs84 latitude
//            20.0, // ok wgs84 longitude
//            epsgNumberForWgs84
//        );
//        transformToCoordinate_shouldThrowException_whenCoordinateIsNotValid(unvalidInputCoordinate);
//    }

    @Test
    void transformToCoordinate_shouldThrowException_whenCrsCodeIsNotValid() {
        final CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.latLon(
            60.0, // ok wgs84 latitude
            20.0, // ok wgs84 longitude
            "This string is NOT a correct crs/EPSG code"
        );
        transformToCoordinate_shouldThrowException_whenCoordinateIsNotValid(unvalidInputCoordinate);
    }

    private final static List<String> classNamesForExpectedPotentialExceptionsWhenIncorrectEPSGcode =
            Arrays.asList(
                IllegalArgumentException.class.getName(), // Goober implementation throws this
                "org.opengis.referencing.NoSuchAuthorityCodeException",
                "org.osgeo.proj4j.UnknownAuthorityCodeException",
                "org.cts.crs.CRSException",
                "mil.nga.sf.util.SFException",
                RuntimeException.class.getName() // composite throws this
            );
    
    private void transformToCoordinate_shouldThrowException_whenCoordinateIsNotValid(
        CrsCoordinate unvalidInputCoordinate
    ) {
        for (CrsTransformationAdapter crsTransformationAdapter : crsTransformationAdapterImplementations) {
            Exception exception = assertThrows(
                Exception.class,
                () -> crsTransformationAdapter.transformToCoordinate(unvalidInputCoordinate, super.epsgNumberForSweref99TM),
                () -> "Exception was not thrown but SHOULD have been thrown for implementation " + crsTransformationAdapter.getAdapteeType() + " and coordinate " + unvalidInputCoordinate 
            );

            boolean isExpectedException = classNamesForExpectedPotentialExceptionsWhenIncorrectEPSGcode.stream().anyMatch(it -> it.equals(exception.getClass().getName()));
            assertTrue(isExpectedException, () -> "Unexpected exception: " + exception.getClass().getName() + " for adapter " + crsTransformationAdapter.getAdapteeType());
        }
    }
    
    
    @Test
    void getLongNameOfImplementation_shouldReturnFullClassNameIncludingPackageName() {
        // Of course fragile, but the class/package name will not change
        // often and if/when it does the test will fail but will be trivial to fix.
        // The purpose of this test is not only to "test" but rather to
        // illustrate what the method returns
        assertEquals(
            "com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools",
            (new CrsTransformationAdapterGeoTools()).getLongNameOfImplementation()
        );
        // There are more related tests in 'CrsTransformationAdapterLeafFactoryTest'
        // which will detect problems if a class is renamed        
    }

    @Test
    void getShortNameOfImplementation_shouldReturnUniqueSuffixPartOfTheClassName() {
        // Of course fragile, but the class/package name will not change
        // often and if/when it does the test will fail but will be trivial to fix.
        // The purpose of this test is not only to "test" but also to
        // illustrate what the method returns
        assertEquals(
            "GeoTools",
            (new CrsTransformationAdapterGeoTools()).getShortNameOfImplementation()
        );

        assertEquals(
            "GooberCTL",
            (new CrsTransformationAdapterGooberCTL()).getShortNameOfImplementation()
        );

        assertEquals(
            "Proj4J",
            (new CrsTransformationAdapterProj4J()).getShortNameOfImplementation()
        );

        assertEquals(
            "OrbisgisCTS",
            (new CrsTransformationAdapterOrbisgisCTS()).getShortNameOfImplementation()
        );

        assertEquals(
            "GeoPackageNGA",
            (new CrsTransformationAdapterGeoPackageNGA()).getShortNameOfImplementation()
        );

        // The above tests are for the "Leaf" implementations.
        // Below is a "Composite" created
        CrsTransformationAdapterComposite compositeAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage();
        // The class name for the above adapter is "CrsTransformationAdapterComposite"
        // and the prefix part "CrsTransformationAdapter" is removed from the name
        // to get the short implementation i.e. just "Composite"
        assertEquals(
            "Composite",
            compositeAdapter.getShortNameOfImplementation()
        );
    }

    @Test
    void isComposite_shouldReturnTrue_whenComposite() {
        for (CrsTransformationAdapter compositeAdapter : super.crsTransformationAdapterCompositeImplementations) {
            assertTrue(compositeAdapter.isComposite());    
        }
    }
    
    @Test
    void isComposite_shouldReturnFalse_whenLeaf() {
        for (CrsTransformationAdapter leafAdapter : super.crsTransformationAdapterLeafImplementations) {
            assertFalse(leafAdapter.isComposite());
        }
    }    

    @Test
    void getTransformationAdapterChildren_shouldReturnNonEmptyList_whenComposite() {
        // all leafs should be children 
        final int expectedNumberOfChildrenForTheComposites = super.crsTransformationAdapterLeafImplementations.size();
        assertThat(
            "Has the number of leaf implementations been reduced?",
            expectedNumberOfChildrenForTheComposites, greaterThanOrEqualTo(5) // currently five
        ); 
        for (CrsTransformationAdapter compositeAdapter : super.crsTransformationAdapterCompositeImplementations) {
            assertEquals(
                expectedNumberOfChildrenForTheComposites, 
                compositeAdapter.getTransformationAdapterChildren().size()
            );    
        }
    }

    @Test
    void getTransformationAdapterChildren_shouldReturnEmptyList_whenLeaf() {
        final int zeroExpectedNumberOfChildren = 0;
        for (CrsTransformationAdapter leafAdapter : super.crsTransformationAdapterLeafImplementations) {
            assertEquals(
                zeroExpectedNumberOfChildren,
                leafAdapter.getTransformationAdapterChildren().size()
            );
        }
    }
    
    @Test
    void isReliable_shouldReturnTrueForLeafs_whenUsingCriteriaNumberOfResultsOneAndMaxDiffZero() {
        final int criteriaNumberOfResults = 1; // always one for a Leaf
        final double criteriaMaxDiff = 0.0; // always zero for a Leaf
        // The tested method 'isReliable' is actually relevant only for aggregated
        // transformations, but nevertheless there is a reaonable behavouor also
        // for the "Leaf" implementations regarding the number of results (always 1)
        // and the "differences" in lat/long for the "different" implementations
        // i.e. the "difference" should always be zero since there is only one implementation

        final List<CrsTransformationAdapter> crsTransformationAdapterImplementationsExpectingOneResult = new ArrayList<>();
        crsTransformationAdapterImplementationsExpectingOneResult.addAll(super.crsTransformationAdapterLeafImplementations);
        crsTransformationAdapterImplementationsExpectingOneResult.add(CrsTransformationAdapterCompositeFactory.createCrsTransformationFirstSuccess());        
        assertEquals(6, crsTransformationAdapterImplementationsExpectingOneResult.size()); // 5 leafs plus 1 
        
        for (CrsTransformationAdapter crsTransformationAdapterLeaf : crsTransformationAdapterImplementationsExpectingOneResult) {
            // suffix "Leaf" is not quite true, but in one of the iterations
            // it will be the Composite FirstSuccess which also will only have one result 
            // and thus can be tested in the same way as the leafs in this method
            final CrsCoordinate wgs84coordinateInSweden = CrsCoordinateFactory.latLon(59.29,18.03);
            final CrsTransformationResult resultWhenTransformingToSwedishCRS = crsTransformationAdapterLeaf.transform(wgs84coordinateInSweden, com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber.SWEDEN__SWEREF99_TM__3006);
            assertNotNull(resultWhenTransformingToSwedishCRS);
            assertTrue(resultWhenTransformingToSwedishCRS.isSuccess());
            final CrsTransformationResultStatistic crsTransformationResultStatistic = resultWhenTransformingToSwedishCRS.getCrsTransformationResultStatistic();
            assertNotNull(crsTransformationResultStatistic);
            assertTrue(crsTransformationResultStatistic.isStatisticsAvailable());
    
            final int actualNumberOfResults = crsTransformationResultStatistic.getNumberOfResults();
            assertEquals(criteriaNumberOfResults, actualNumberOfResults);
            final double actualMaxDiffXLongitude = crsTransformationResultStatistic.getMaxDifferenceForXEastingLongitude();
            final double actualMaxDiffYLatitude = crsTransformationResultStatistic.getMaxDifferenceForYNorthingLatitude();
            final double actualMaxDiffXorY = Math.max(actualMaxDiffXLongitude, actualMaxDiffYLatitude);
            assertEquals(criteriaMaxDiff, actualMaxDiffXorY); // zero differences since there should be only one result !

            // method "isReliable" used below is the method under test
            
            assertTrue(resultWhenTransformingToSwedishCRS.isReliable(criteriaNumberOfResults, criteriaMaxDiff));
    
            // assertFalse below since trying to require one more result than available
            assertFalse(resultWhenTransformingToSwedishCRS.isReliable(criteriaNumberOfResults + 1, criteriaMaxDiff));
    
            // assertFalse below since trying to require too small maxdiff
            assertFalse(resultWhenTransformingToSwedishCRS.isReliable(criteriaNumberOfResults, criteriaMaxDiff - 0.00000000001));
        }
    }

    @Test
    void isReliable_shouldReturnTrueOrFalseForComposites_dependingOnCriteriasUsedAsMethodParameter() {
        // the below value is the max difference when comparing the five leaf implementations 
        final double actualMaxDiffXorY = 0.0032574664801359177;
       
        final int criteriaNumberOfResultsSuccess = 5; // all 5 should succeed
        final int criteriaNumberOfResultsFailure = 6; // 6 implementations can not succeed since there are not so many implementations
        final double criteriaMaxDiffFailure = actualMaxDiffXorY - 0.0001; // a little too small requirement for max difference
        final double criteriaMaxDiffSuccess  = actualMaxDiffXorY + 0.0001; // should result in success since the actual number is smaller 

        final List<CrsTransformationAdapter> crsTransformationAdapterImplementationsExpectingManyResults = new ArrayList<>();
        for (CrsTransformationAdapter crsTransformationAdapterComposite : super.crsTransformationAdapterCompositeImplementations) {
            if(crsTransformationAdapterComposite.getAdapteeType() != CrsTransformationAdapteeType.COMPOSITE_FIRST_SUCCESS) {
                crsTransformationAdapterImplementationsExpectingManyResults.add(crsTransformationAdapterComposite);
            }
        }
        assertEquals(3, crsTransformationAdapterImplementationsExpectingManyResults.size()); // 3 composites i.e. all composite except COMPOSITE_FIRST_SUCCESS
        
        for (CrsTransformationAdapter crsTransformationAdapterComposite : crsTransformationAdapterImplementationsExpectingManyResults) {
            final CrsCoordinate wgs84coordinateInSweden = CrsCoordinateFactory.latLon(59.29,18.03);
            final CrsTransformationResult resultWhenTransformingToSwedishCRS = crsTransformationAdapterComposite.transform(wgs84coordinateInSweden, com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber.SWEDEN__SWEREF99_TM__3006);
            assertNotNull(resultWhenTransformingToSwedishCRS);
            assertTrue(resultWhenTransformingToSwedishCRS.isSuccess());
            final CrsTransformationResultStatistic crsTransformationResultStatistic = resultWhenTransformingToSwedishCRS.getCrsTransformationResultStatistic();
            assertNotNull(crsTransformationResultStatistic);
            assertTrue(crsTransformationResultStatistic.isStatisticsAvailable());
            // so far the same code as the previous test method for the test method with leafs
            
            final int actualNumberOfResults = crsTransformationResultStatistic.getNumberOfResults();
            assertEquals(criteriaNumberOfResultsSuccess, actualNumberOfResults);
            final double actualMaxDiffXLongitude = crsTransformationResultStatistic.getMaxDifferenceForXEastingLongitude();
            final double actualMaxDiffYLatitude = crsTransformationResultStatistic.getMaxDifferenceForYNorthingLatitude();
            // final double actualMaxDiffXorY = Math.max(actualMaxDiffXLongitude, actualMaxDiffYLatitude);
            // System.out.println("actualMaxDiffXorY " + actualMaxDiffXorY + "  " + crsTransformationAdapterComposite.getAdapteeType());

            // method "isReliable" used below is the method under test
            assertTrue(resultWhenTransformingToSwedishCRS.isReliable(criteriaNumberOfResultsSuccess, criteriaMaxDiffSuccess));
            assertFalse(resultWhenTransformingToSwedishCRS.isReliable(criteriaNumberOfResultsSuccess, criteriaMaxDiffFailure));
            assertFalse(resultWhenTransformingToSwedishCRS.isReliable(criteriaNumberOfResultsFailure, criteriaMaxDiffSuccess));
            assertFalse(resultWhenTransformingToSwedishCRS.isReliable(criteriaNumberOfResultsFailure, criteriaMaxDiffFailure));
        }
    }

    private void transformToCoordinate_shouldReturnCorrectSweref99TMcoordinate_whenTransformingFromWgs84(
        CrsTransformationAdapter crsTransformationAdapter
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

        CrsCoordinate inputCoordinate = CrsCoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude(wgs84Lon, wgs84Lat, super.epsgNumberForWgs84);
        CrsCoordinate outputCoordinate = crsTransformationAdapter.transformToCoordinate(inputCoordinate, super.epsgNumberForSweref99TM);
        assertEquals(sweref99_Y_expected, outputCoordinate.getYNorthingLatitude(), 0.5);
        assertEquals(sweref99_X_expected, outputCoordinate.getXEastingLongitude(), 0.5);
    }

    private void transformToCoordinate_shouldReturnCorrectWgs84coordinate_whenTransformingFromRT90(
        CrsTransformationAdapter crsTransformationAdapter
    ) {
        double rt90_Y = 6580994;
        double rt90_X = 1628294;

        double wgs84Lat_expected = 59.330231;
        double wgs84Lon_expected = 18.059196;

        CrsCoordinate inputCoordinate = CrsCoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude(rt90_X, rt90_Y, super.epsgNumberForRT90);

        CrsCoordinate outputCoordinate = crsTransformationAdapter.transformToCoordinate(inputCoordinate, super.epsgNumberForWgs84);
        assertEquals(wgs84Lat_expected, outputCoordinate.getYNorthingLatitude(), 0.1);
        assertEquals(wgs84Lon_expected, outputCoordinate.getXEastingLongitude(), 0.1);
    }

    private void assertResultStatisticsForLeafImplementation(
        CrsTransformationResult transformResult
    ) {
        final CrsTransformationResultStatistic crsTransformationResultStatistic = transformResult.getCrsTransformationResultStatistic();
        assertNotNull(crsTransformationResultStatistic);
        assertTrue(crsTransformationResultStatistic.isStatisticsAvailable());
        assertEquals(1, crsTransformationResultStatistic.getNumberOfResults());
        assertEquals(0, crsTransformationResultStatistic.getMaxDifferenceForYNorthingLatitude());
        assertEquals(0, crsTransformationResultStatistic.getMaxDifferenceForXEastingLongitude());
        assertEquals(transformResult.getOutputCoordinate(), crsTransformationResultStatistic.getCoordinateAverage());
        assertEquals(transformResult.getOutputCoordinate(), crsTransformationResultStatistic.getCoordinateMedian());
    }
    
}