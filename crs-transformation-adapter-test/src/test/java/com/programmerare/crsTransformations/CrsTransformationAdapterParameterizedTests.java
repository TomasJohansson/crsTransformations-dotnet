package com.programmerare.crsTransformations;

import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsTransformations.coordinate.CrsCoordinateFactory;
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvFileSource;

import java.util.List;
import java.util.function.Supplier;
import java.util.stream.IntStream;

import static java.util.stream.Collectors.toList;
import static org.junit.jupiter.api.Assertions.*;

public class CrsTransformationAdapterParameterizedTests extends CrsTransformationTestBase {

    private final static String testFileWithSomeCoordinatesForSweden = "/coordinatesForSweden.csv";

    private final static int lowerEpsgIntervalForSwedishProjectionsUsingMeterAsUnit = EpsgNumber.SWEDEN__SWEREF99_TM__3006; // 3006;
    private final static int upperEpsgIntervalForSwedishProjectionsUsingMeterAsUnit = EpsgNumber.SWEDEN__5_GON_E__RT90_5_GON_O__3024; // 3024;

    // When doing assertions, the delta value should depend on whether the units 
    // are meters or degrees (for degree latitudes the value "1" is very big but 
    // only one meter for the swedish projections with EPSG codes in the below list) 
    private static List<Integer> epsgNumbersForSwedishProjectionsUsingMeterAsUnit;

    @BeforeAll
    static void beforeAll() {
        epsgNumbersForSwedishProjectionsUsingMeterAsUnit = IntStream.rangeClosed(lowerEpsgIntervalForSwedishProjectionsUsingMeterAsUnit, upperEpsgIntervalForSwedishProjectionsUsingMeterAsUnit).boxed().collect(toList());
    }

    @ParameterizedTest
    @CsvFileSource(resources = testFileWithSomeCoordinatesForSweden, numLinesToSkip = 3, delimiter = ';')
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
    
    @ParameterizedTest
    @CsvFileSource(resources = testFileWithSomeCoordinatesForSweden, numLinesToSkip = 3, delimiter = ';')
    @DisplayName("Transformation back and forth from WGS84 cordinates to RT90/SWEREF99 projections should result in the same WGS84 coordinates")
    void verifyTransformationsBackAndForthFromWgs84ToSwedishProjections(
        String description,
        double wgs84Lat, double wgs84Lon // ignore the rest of columns for this test method
    ) {
        final CrsCoordinate inputCoordinateWGS84 = CrsCoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude(wgs84Lon, wgs84Lat, epsgNumberForWgs84);
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
    

    @ParameterizedTest
    @CsvFileSource(resources = testFileWithSomeCoordinatesForSweden, numLinesToSkip = 3, delimiter = ';')
    @DisplayName("The same transformation but with different implementations should produce the same coordinates")
    void transformToCoordinate_shouldReturnTheSameCoordinate_whenTransformingWithDifferentImplementations(
        String description,
        double wgs84Lat, double wgs84Lon // ignore the rest of columns for this test method
    ) {
        final CrsCoordinate inputCoordinateWGS84 = CrsCoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude(wgs84Lon, wgs84Lat, epsgNumberForWgs84);
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

    private enum CoordinateReferenceSystemUnit {
        DEGREES,
        METERS,
        UNKNOWN
    }
}