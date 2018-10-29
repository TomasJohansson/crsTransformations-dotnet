package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.CrsTransformationFacade;
import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.TransformResult;
import com.programmerare.crsTransformations.utils.MedianValueUtility;
import org.junit.jupiter.api.Test;
import java.util.List;
import java.util.stream.Collectors;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

class CompositeStrategyForMedianValueTest extends CompositeStrategyTestBase {

    private final static double delta = 0.00001;

    @Test
    void transformWithFacadeCompositeMedianTest() {
        Coordinate expectedCoordinateWithMedianLatitudeAndLongitude = calculateMedianCoordinate(super.allCoordinateResultsForTheDifferentImplementations);

        CrsTransformationFacade medianCompositeFacade = CrsTransformationFacadeComposite.createCrsTransformationMedian(
            allFacades
        );
        TransformResult medianResult = medianCompositeFacade.transform(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        assertNotNull(medianResult);
        assertTrue(medianResult.isSuccess());
        assertEquals(super.allCoordinateResultsForTheDifferentImplementations.size(), medianResult.getSubResults().size());

        Coordinate coordinateReturnedByMedianFacade = medianResult.getOutputCoordinate();

        // The same transformation as above has been done in the base class for the individual facades
        assertEquals(expectedCoordinateWithMedianLatitudeAndLongitude.getXLongitude(), coordinateReturnedByMedianFacade.getXLongitude(), delta);
        assertEquals(expectedCoordinateWithMedianLatitudeAndLongitude.getYLatitude(), coordinateReturnedByMedianFacade.getYLatitude(), delta);
    }

    private Coordinate calculateMedianCoordinate(List<Coordinate> coordinateResultsForTheDifferentImplementations) {
        List<Double> longitudesSorted = coordinateResultsForTheDifferentImplementations.stream().map(x -> x.getXLongitude()).collect(Collectors.toList());
        List<Double> latitudesSorted = coordinateResultsForTheDifferentImplementations.stream().map(x -> x.getYLatitude()).collect(Collectors.toList());
        double medianLongitude = MedianValueUtility.getMedianValue(longitudesSorted);
        double medianLatitude = MedianValueUtility.getMedianValue(latitudesSorted);
        return Coordinate.createFromXLongitudeYLatitude(medianLongitude, medianLatitude, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
    }

}