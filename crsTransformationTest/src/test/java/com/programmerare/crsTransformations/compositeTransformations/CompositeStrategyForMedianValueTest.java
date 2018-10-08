package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.CrsTransformationFacade;
import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
import org.junit.jupiter.api.Test;
import java.util.Arrays;
import java.util.List;
import java.util.stream.Collectors;

import static org.junit.jupiter.api.Assertions.assertEquals;

class CompositeStrategyForMedianValueTest extends CompositeStrategyTestBase {

    private final static double delta = 0.00001;

    @Test
    void transformWithFacadeCompositeMedianTest() {
        Coordinate expectedCoordinateWithMedianLatitudeAndLongitude = calculateMedianCoordinate(super.allCoordinateResultsForTheDifferentImplementations);

        CrsTransformationFacade facadeComposite = CrsTransformationFacadeComposite.createCrsTransformationMedian(
            allFacades
        );

        Coordinate coordinateReturnedByMedianFacade = facadeComposite.transform(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        // The same transformation as above has been done in the base class for the individual facades
        assertEquals(expectedCoordinateWithMedianLatitudeAndLongitude.getXLongitude(), coordinateReturnedByMedianFacade.getXLongitude(), delta);
        assertEquals(expectedCoordinateWithMedianLatitudeAndLongitude.getYLatitude(), coordinateReturnedByMedianFacade.getYLatitude(), delta);
    }

    private Coordinate calculateMedianCoordinate(List<Coordinate> coordinateResultsForTheDifferentImplementations) {
        List<Double> longitudesSorted = coordinateResultsForTheDifferentImplementations.stream().map(x -> x.getXLongitude()).collect(Collectors.toList());
        List<Double> latitudesSorted = coordinateResultsForTheDifferentImplementations.stream().map(x -> x.getYLatitude()).collect(Collectors.toList());
        double medianLongitude = CompositeStrategyForMedianValue.getMedianValue(longitudesSorted);
        double medianLatitude = CompositeStrategyForMedianValue.getMedianValue(latitudesSorted);
        return Coordinate.createFromXLongYLat(medianLongitude, medianLatitude, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
    }

    @Test
    void getMedianFrom3values() {
        double medianValue = CompositeStrategyForMedianValue.getMedianValue(Arrays.asList(55.0, 33.0, 77.0));
        assertEquals(55.0, medianValue, delta);
    }

    @Test
    void getMedianFrom4values() {
        double medianValue = CompositeStrategyForMedianValue.getMedianValue(Arrays.asList(55.0, 33.0, 77.0, 35.0));
        // the average of the two middle values 35 and 55
        assertEquals(45.0, medianValue, delta);
    }

    @Test
    void getMedianFrom7values() {
        double medianValue = CompositeStrategyForMedianValue.getMedianValue(Arrays.asList(9.0, 6.0, 1.0, 7.0, 8.0, 5.0, 3.0));
        assertEquals(6.0, medianValue, delta);
    }

    @Test
    void getMedianFrom8values() {
        double medianValue = CompositeStrategyForMedianValue.getMedianValue(Arrays.asList(9.0, 6.0, 1.0, 7.0, 8.0, 5.0, 3.0, 6.5));
        // the average of the two middle values 6.0 and 6.5
        assertEquals(6.25, medianValue, delta);
    }

}