package com.programmerare.crsTransformations;

import com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA;
import com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools;
import com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL;
import com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS;
import com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J;
import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterComposite;
import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterCompositeFactory;
import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterWeight;
import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsTransformations.coordinate.CrsCoordinateFactory;
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvFileSource;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.function.Supplier;
import java.util.stream.IntStream;
import static java.util.stream.Collectors.toList;
import static org.junit.jupiter.api.Assertions.*;

final class CrsTransformationAdapterTest {
    
    private final static int epsgNumberForWgs84         = EpsgNumber.WORLD__WGS_84__4326;
    private final static int epsgNumberForSweref99TM    = EpsgNumber.SWEDEN__SWEREF99_TM__3006;
    private final static int epsgNumberForRT90          = EpsgNumber.SWEDEN__2_5_GON_W__RT90_2_5_GON_V__3021;
    private final static int epsgNumberForSweref991200  = EpsgNumber.SWEDEN__12_00__SWEREF99_12_00__3007;
    private final static int epsgNumberForSweref991500  = EpsgNumber.SWEDEN__15_00__SWEREF99_15_00__3009;

    private final static int lowerEpsgIntervalForSwedishProjectionsUsingMeterAsUnit = EpsgNumber.SWEDEN__SWEREF99_TM__3006; // 3006;
    private final static int upperEpsgIntervalForSwedishProjectionsUsingMeterAsUnit = EpsgNumber.SWEDEN__5_GON_E__RT90_5_GON_O__3024; // 3024;

    private static List<Integer> epsgNumbersForSwedishProjectionsUsingMeterAsUnit;

    private static List<CrsTransformationAdapter> crsTransformationAdapterLeafImplementations;
    private static List<CrsTransformationAdapter> crsTransformationAdapterCompositeImplementations;
    private static List<CrsTransformationAdapter> crsTransformationAdapterImplementations;
    
    @BeforeAll
    static void beforeAll() {
        epsgNumbersForSwedishProjectionsUsingMeterAsUnit = IntStream.rangeClosed(lowerEpsgIntervalForSwedishProjectionsUsingMeterAsUnit, upperEpsgIntervalForSwedishProjectionsUsingMeterAsUnit).boxed().collect(toList());

        crsTransformationAdapterLeafImplementations = Arrays.asList(
            new CrsTransformationAdapterGeoTools(),
            new CrsTransformationAdapterGooberCTL(),
            new CrsTransformationAdapterProj4J(),
            new CrsTransformationAdapterOrbisgisCTS(),
            new CrsTransformationAdapterGeoPackageNGA()
        );

        crsTransformationAdapterCompositeImplementations = Arrays.asList(
            CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage(),
            CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian(),
            CrsTransformationAdapterCompositeFactory.createCrsTransformationChainOfResponsibility(),
            CrsTransformationAdapterCompositeFactory.createCrsTransformationWeightedAverage(Arrays.asList(
                CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterGeoTools(), 51.0),
                CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterGooberCTL(), 52.0),
                CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterProj4J(), 53.0),
                CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterOrbisgisCTS(), 54.0),
                CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterGeoPackageNGA(), 55.0)
            ))
        );
        crsTransformationAdapterImplementations = new ArrayList<>();
        crsTransformationAdapterImplementations.addAll(crsTransformationAdapterLeafImplementations);
        crsTransformationAdapterImplementations.addAll(crsTransformationAdapterCompositeImplementations);
    }

    @Test
    void assertThatTheSetupHaveCreatedFiveLeafAndFourCompositeImplementations() {
        final int expectedNumberOfLeafs = 5; // will not change often and if/when changed then will be easy to fix
        int expectedNumberOfComposites = 4; // will not change often and if/when changed then will be easy to fix
        assertEquals(expectedNumberOfLeafs, crsTransformationAdapterLeafImplementations.size());
        assertEquals(expectedNumberOfComposites, crsTransformationAdapterCompositeImplementations.size());
        assertEquals(expectedNumberOfLeafs + expectedNumberOfComposites, crsTransformationAdapterImplementations.size());
    }
    
    @Test
    void transformToCoordinate_shouldReturnCorrectSweref99TMcoordinate_whenTransformingFromWgs84withAllAdapterImplementations() {
        for (CrsTransformationAdapter crsTransformationAdapter : crsTransformationAdapterImplementations) {
            transformToCoordinate_shouldReturnCorrectSweref99TMcoordinate_whenTransformingFromWgs84(crsTransformationAdapter);
        }
    }

    // 
    
    @Test
    void transformToCoordinate_shouldReturnCorrectWgs84coordinate_whenTransformingFromRT90withAllAdapterImplementations() {
        for (CrsTransformationAdapter crsTransformationAdapter : crsTransformationAdapterImplementations) {
            transformToCoordinate_shouldReturnCorrectWgs84coordinate_whenTransformingFromRT90(crsTransformationAdapter);
        }
    }

    private void transformToCoordinate_shouldReturnTheOriginalCoordinate_whenTransformingBackAgainFromTheResult(
        CrsTransformationAdapter crsTransformationAdapter,
        CrsCoordinate inputCoordinateOriginalCRS,
        int epsgNumberForTransformTargetCRS
    ) {
        double delta = getDeltaValueForComparisons(inputCoordinateOriginalCRS.getCrsIdentifier());

        CrsCoordinate outputCoordinateForTransformTargetCRS = crsTransformationAdapter.transformToCoordinate(inputCoordinateOriginalCRS, epsgNumberForTransformTargetCRS);
        CrsCoordinate outputCoordinateOriginalCRS = crsTransformationAdapter.transformToCoordinate(outputCoordinateForTransformTargetCRS, inputCoordinateOriginalCRS.getCrsIdentifier().getEpsgNumber());

        assertEquals(inputCoordinateOriginalCRS.getXEastingLongitude(), outputCoordinateOriginalCRS.getXEastingLongitude(), delta);
        assertEquals(inputCoordinateOriginalCRS.getYNorthingLatitude(), outputCoordinateOriginalCRS.getYNorthingLatitude(), delta);
        assertEquals(inputCoordinateOriginalCRS.getCrsIdentifier().getEpsgNumber(), outputCoordinateOriginalCRS.getCrsIdentifier().getEpsgNumber());
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

        CrsCoordinate inputCoordinate = CrsCoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude(wgs84Lon, wgs84Lat, epsgNumberForWgs84);
        CrsCoordinate outputCoordinate = crsTransformationAdapter.transformToCoordinate(inputCoordinate, epsgNumberForSweref99TM);
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

        CrsCoordinate inputCoordinate = CrsCoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude(rt90_X, rt90_Y, epsgNumberForRT90);

        CrsCoordinate outputCoordinate = crsTransformationAdapter.transformToCoordinate(inputCoordinate, epsgNumberForWgs84);
        assertEquals(wgs84Lat_expected, outputCoordinate.getYNorthingLatitude(), 0.1);
        assertEquals(wgs84Lon_expected, outputCoordinate.getXEastingLongitude(), 0.1);
    }

    @ParameterizedTest
    @CsvFileSource(resources = "/coordinatesForSweden.csv", numLinesToSkip = 3, delimiter = ';')
    @DisplayName("Transformation result coordinates should match with coordinates in CSV file")
    void verifyTransformationsCorrespondToCsvFileCoordinates(
        String description,
        double wgs84Lat, double wgs84Lon,
        double rt90north, double rt90east,
        double sweref99north, double sweref99east,
        String url
    ) {
        // example row from the csv file:
        // Stockholm Centralstation;59.330231;18.059196;6580994;1628294;6580822;674032;https://kartor.eniro.se/m/03Yxp

        // These used coordinates (i.e. those in the csv file) were manually retrieved from the Eniro
        // site at the URL's for each row, and by clicking the coordinate feature
        // which shows the coordinates in the three systems WGS84, RT90, SWREF99

        for (CrsTransformationAdapter crsTransformationAdapter : crsTransformationAdapterImplementations) {
            
            transformToCoordinate_shouldReturnEqualCoordinates_whenTransformingBetweenTwoKnownCoordinatesToAndFromEachOther(
                crsTransformationAdapter, 
                epsgNumberForWgs84, epsgNumberForRT90, 
                wgs84Lat, wgs84Lon, 
                rt90north, rt90east, 
                description
            );
            
            transformToCoordinate_shouldReturnEqualCoordinates_whenTransformingBetweenTwoKnownCoordinatesToAndFromEachOther(
                crsTransformationAdapter, 
                epsgNumberForWgs84, epsgNumberForSweref99TM, 
                wgs84Lat, wgs84Lon, 
                sweref99north, sweref99east, 
                description
            );
            
            transformToCoordinate_shouldReturnEqualCoordinates_whenTransformingBetweenTwoKnownCoordinatesToAndFromEachOther(
                crsTransformationAdapter, 
                epsgNumberForRT90, epsgNumberForSweref99TM, 
                rt90north, rt90east, 
                sweref99north, sweref99east, 
                description
            );
        }
    }

    
    private void transformToCoordinate_shouldReturnEqualCoordinates_whenTransformingBetweenTwoKnownCoordinatesToAndFromEachOther(
        CrsTransformationAdapter crsTransformationAdapter,
        int epsgNumber1, int epsgNumber2,
        double yLat1, double xLon1,
        double yLat2, double xLon2,
        String description
    ) {
        final CrsCoordinate coordinate1 = CrsCoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude(xLon1, yLat1, epsgNumber1);
        final CrsCoordinate coordinate2 = CrsCoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude(xLon2, yLat2, epsgNumber2);
        final CrsCoordinate outputForCoordinate1 = crsTransformationAdapter.transformToCoordinate(coordinate1, epsgNumber2);
        final CrsCoordinate outputForCoordinate2 = crsTransformationAdapter.transformToCoordinate(coordinate2, epsgNumber1);

        double delta = getDeltaValueForComparisons(epsgNumber2);
        assertEquals(coordinate2.getXEastingLongitude(), outputForCoordinate1.getXEastingLongitude(), delta, description);
        assertEquals(coordinate2.getYNorthingLatitude(), outputForCoordinate1.getYNorthingLatitude(), delta, description);

        delta = getDeltaValueForComparisons(epsgNumber1);
        assertEquals(coordinate1.getXEastingLongitude(), outputForCoordinate2.getXEastingLongitude(), delta, description);
        assertEquals(coordinate1.getYNorthingLatitude(), outputForCoordinate2.getYNorthingLatitude(), delta, description);
    }

    private double getDeltaValueForComparisons(
        CrsIdentifier crsIdentifier
    ) {
        return getDeltaValueForComparisons(crsIdentifier.getEpsgNumber());

    }
    
    private double getDeltaValueForComparisons(
        int epsgNumber
    ) {
        CoordinateReferenceSystemUnit coordinateReferenceSystemUnit = CoordinateReferenceSystemUnit.UNKNOWN;
        if(epsgNumber == epsgNumberForWgs84) {
            coordinateReferenceSystemUnit = CoordinateReferenceSystemUnit.DEGREES;
        }
        // sweref : 3006 - 3018
        // RT90 :   3019 - 3024
        else if( // if(epsgNumber >= 3006 && epsgNumber <= 3024)
            lowerEpsgIntervalForSwedishProjectionsUsingMeterAsUnit <= epsgNumber
            &&
            epsgNumber <= upperEpsgIntervalForSwedishProjectionsUsingMeterAsUnit
        ) {
            coordinateReferenceSystemUnit = CoordinateReferenceSystemUnit.METERS;
        }
        return getDeltaValueForComparisons(coordinateReferenceSystemUnit, epsgNumber);
    }

    private double getDeltaValueForComparisons(
        CoordinateReferenceSystemUnit coordinateReferenceSystemUnit,
        int epsgNumberUsedOnlyInErrorMessage
    ) {
        if(coordinateReferenceSystemUnit == CoordinateReferenceSystemUnit.DEGREES) {
            // one of the results:
            // Expected :20.266843
            // Actual   :20.266853924440145
            // the above diff is about 0.0001
            return 0.0001;
        }
        else if(coordinateReferenceSystemUnit == CoordinateReferenceSystemUnit.METERS) {
            // one of the results:
            // Expected :386087.0
            // Actual   :386088.0820856609
            // the above diff is about 1.08
            return 1.1;
        }
        else { // if(coordinateReferenceSystemUnit == CoordinateReferenceSystemUnit.UNKNOWN) {
            throw new IllegalArgumentException("Not supported epsg number: " + epsgNumberUsedOnlyInErrorMessage);
        }
    }

    @ParameterizedTest
    @CsvFileSource(resources = "/coordinatesForSweden.csv", numLinesToSkip = 3, delimiter = ';')
    @DisplayName("Transformation back and forth from WGS84 cordinates to RT90/SWEREF99 projections should result in the same WGS84 coordinates")
    void verifyTransformationsBackAndForthFromWgs84ToSwedishProjections(
        String description,
        double wgs84Lat, double wgs84Lon // ignore the rest of columns for this test method
    ) {
       CrsCoordinate inputCoordinateWGS84 = CrsCoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude(wgs84Lon, wgs84Lat, epsgNumberForWgs84);
        for (CrsTransformationAdapter crsTransformationAdapter : crsTransformationAdapterImplementations) {
            for (Integer epsgNumber : epsgNumbersForSwedishProjectionsUsingMeterAsUnit) {
                transformToCoordinate_shouldReturnTheOriginalCoordinate_whenTransformingBackAgainFromTheResult(
                    crsTransformationAdapter, 
                    inputCoordinateWGS84, 
                    epsgNumber
                );
            }
        }
    }

    private void transformToCoordinate_shouldReturnTheSameCoordinate_whenTransformingWithTwoDifferentImplementations(
        CrsTransformationAdapter crsTransformationAdapter1,
        CrsTransformationAdapter crsTransformationAdapter2,
        CrsCoordinate inputCoordinate,
        int epsgNumberForOutputCoordinate
    ) {
        double delta = getDeltaValueForComparisons(epsgNumberForOutputCoordinate);
        
        CrsCoordinate outputCoordinate1 = crsTransformationAdapter1.transformToCoordinate(inputCoordinate, epsgNumberForOutputCoordinate);
        CrsCoordinate outputCoordinate2 = crsTransformationAdapter2.transformToCoordinate(inputCoordinate, epsgNumberForOutputCoordinate);

        Supplier<String> errorMessageLongitude = () -> "delta used: " + delta + " and the diff was " + Math.abs(outputCoordinate1.getXEastingLongitude() - outputCoordinate2.getXEastingLongitude());
        Supplier<String> errorMessageLatitude = () -> "delta used: " + delta + " and the diff was " + Math.abs(outputCoordinate1.getYNorthingLatitude() - outputCoordinate2.getYNorthingLatitude());
        assertEquals(outputCoordinate1.getXEastingLongitude(), outputCoordinate2.getXEastingLongitude(), delta, errorMessageLongitude);
        assertEquals(outputCoordinate1.getYNorthingLatitude(), outputCoordinate2.getYNorthingLatitude(), delta, errorMessageLatitude);
        assertEquals(outputCoordinate1.getCrsIdentifier().getEpsgNumber(), outputCoordinate2.getCrsIdentifier().getEpsgNumber());
    }

    @ParameterizedTest
    @CsvFileSource(resources = "/coordinatesForSweden.csv", numLinesToSkip = 3, delimiter = ';')
    @DisplayName("The same transformation but with different implementations should produce the same coordinates")
    void transformToCoordinate_shouldReturnTheSameCoordinate_whenTransformingWithDifferentImplementations(
        String description,
        double wgs84Lat, double wgs84Lon // ignore the rest of columns for this test method
    ) {
        CrsCoordinate inputCoordinateWGS84 = CrsCoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude(wgs84Lon, wgs84Lat, epsgNumberForWgs84);
        for (int i = 0; i < crsTransformationAdapterImplementations.size()-1; i++) {
            for (int j = i+1; j < crsTransformationAdapterImplementations.size(); j++) {
                for (Integer epsgNumber : epsgNumbersForSwedishProjectionsUsingMeterAsUnit) {
                    transformToCoordinate_shouldReturnTheSameCoordinate_whenTransformingWithTwoDifferentImplementations(
                        crsTransformationAdapterImplementations.get(i),
                        crsTransformationAdapterImplementations.get(j),
                        inputCoordinateWGS84,
                        epsgNumber
                    );
                }
            }
        }
    }

    @DisplayName("Testing CrsTransformationResult with expected failure")
    @Test
    void transformToResultObjectWithUnvalidInputCoordinate() {
        CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude(-999999.0, -999999.0, 1);
        for (CrsTransformationAdapter crsTransformationAdapter : crsTransformationAdapterImplementations) {
            final String messageWhenError = "Problem with the implementation " + crsTransformationAdapter.getAdapteeType();
            CrsTransformationResult transformResult = crsTransformationAdapter.transform(unvalidInputCoordinate, 2);
            assertNotNull(transformResult, messageWhenError);
            assertFalse(transformResult.isSuccess(), messageWhenError);
            assertNotNull(transformResult.getException(), messageWhenError);
            assertEquals(unvalidInputCoordinate, transformResult.getInputCoordinate(), messageWhenError);
        }
    }

    @DisplayName("Testing CrsTransformationResult with expected successe")
    @Test
    void transform_shouldSuccess_whenInputCoordinateIsCorrect() {
        double wgs84Lat = 59.330231;
        double wgs84Lon = 18.059196;
        CrsCoordinate wgs84InputCoordinate = CrsCoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude(wgs84Lon, wgs84Lat, epsgNumberForWgs84);

        for (CrsTransformationAdapter crsTransformationAdapter : crsTransformationAdapterImplementations) {
            CrsTransformationResult transformResult = crsTransformationAdapter.transform(wgs84InputCoordinate, epsgNumberForSweref99TM);
            assertNotNull(transformResult);
            assertTrue(transformResult.isSuccess());
            assertNull(transformResult.getException());
            CrsCoordinate outputCoordinate = transformResult.getOutputCoordinate();
            assertNotNull(outputCoordinate);
            assertEquals(outputCoordinate.getCrsIdentifier().getEpsgNumber(), epsgNumberForSweref99TM);
            if(!crsTransformationAdapter.isComposite()) {
                assertResultStatisticsForLeafImplementation(transformResult);    
            }
        }
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

    enum CoordinateReferenceSystemUnit {
        DEGREES,
        METERS,
        UNKNOWN
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
        CrsTransformationAdapter compositeAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage();
        assertTrue(compositeAdapter.isComposite());
        // TODO: iterate implementations e.g. for (CrsTransformationAdapter crsTransformationAdapterLeaf : crsTransformationAdapterLeafImplementations) {
    }
    
    @Test
    void isComposite_shouldReturnFalse_whenLeaf() {
        // TODO: iterate implementations e.g. for (CrsTransformationAdapter crsTransformationAdapterLeaf : crsTransformationAdapterLeafImplementations) {
        CrsTransformationAdapter goober = CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage();
        assertFalse( (new CrsTransformationAdapterGeoPackageNGA()).isComposite());
        assertFalse( (new CrsTransformationAdapterGooberCTL()).isComposite());
        assertFalse( (new CrsTransformationAdapterGeoTools()).isComposite());
        assertFalse( (new CrsTransformationAdapterOrbisgisCTS()).isComposite());
        assertFalse( (new CrsTransformationAdapterProj4J()).isComposite());
    }    

    @Test
    void getTransformationAdapterChildren_shouldReturnNonEmptyList_whenComposite() {
        CrsTransformationAdapter compositeAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage();
        // 5 below is fragile but will of course be very trivial to fix if ny implementations would be added
        assertEquals(5, compositeAdapter.getTransformationAdapterChildren().size());
    }
    // TODO iteration in above and below methods
    @Test
    void getTransformationAdapterChildren_shouldReturnEmptyList_whenLeaf() {
        assertEquals(0,(new CrsTransformationAdapterGeoPackageNGA()).getTransformationAdapterChildren().size());
        assertEquals(0,(new CrsTransformationAdapterGooberCTL()).getTransformationAdapterChildren().size());
        assertEquals(0,(new CrsTransformationAdapterGeoTools()).getTransformationAdapterChildren().size());
        assertEquals(0,(new CrsTransformationAdapterOrbisgisCTS()).getTransformationAdapterChildren().size());
        assertEquals(0,(new CrsTransformationAdapterProj4J()).getTransformationAdapterChildren().size());
    }
    

    @Test
    void isReliable_shoudReturnTrueForLeafs_whenUsingCriteriaNumberOfResultsOneAndMaxDiffZero() {
        final int criteriaNumberOfResults = 1; // always one for a Leaf
        final double criteriaMaxDiff = 0.0; // always zero for a Leaf
        // The tested method 'isReliable' is actually relevant only for aggregated
        // transformations, but nevertheless there is a reaonable behavouor also
        // for the "Leaf" implementations regarding the number of results (always 1)
        // and the "differences" in lat/long for the "different" implementations
        // i.e. the "difference" should always be zero since there is only one implementation
        for (CrsTransformationAdapter crsTransformationAdapterLeaf : crsTransformationAdapterLeafImplementations) {
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

}