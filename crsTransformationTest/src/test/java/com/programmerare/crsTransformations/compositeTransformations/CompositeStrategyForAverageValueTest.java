package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformations.CrsTransformationFacade;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.CrsIdentifier;
import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.TransformResult;
import org.junit.jupiter.api.Test;

import java.util.List;
import java.util.Set;
import java.util.function.ToDoubleFunction;
import java.util.stream.Collectors;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class CompositeStrategyForAverageValueTest extends CompositeStrategyTestBase {

    private final static double delta = 0.000000001;

    @Test
    void createCRStransformationFacadeAverage() {
        Coordinate coordinateWithAverageLatitudeAndLongitude = calculateAverageCoordinate(super.allCoordinateResultsForTheDifferentImplementations);

        CrsTransformationFacade averageCompositeFacade = CrsTransformationFacadeCompositeFactory.createCrsTransformationAverage(
            allFacades
        );
        TransformResult averageResult = averageCompositeFacade.transform(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        assertNotNull(averageResult);
        assertTrue(averageResult.isSuccess());
        assertEquals(super.allCoordinateResultsForTheDifferentImplementations.size(), averageResult.getSubResults().size());

        Coordinate coordinateReturnedByCompositeFacade = averageResult.getOutputCoordinate();

        assertEquals(coordinateWithAverageLatitudeAndLongitude.getXLongitude(), coordinateReturnedByCompositeFacade.getXLongitude(), delta);
        assertEquals(coordinateWithAverageLatitudeAndLongitude.getYLatitude(), coordinateReturnedByCompositeFacade.getYLatitude(), delta);
        // assertEquals(coordinateWithAverageLatitudeAndLongitude, coordinateReturnedByCompositeFacade);
        // Expected :Coordinate(xLongitude=674032.3572074446, yLatitude=6580821.991903967, crsIdentifier=CrsIdentifier(crsCode=EPSG:3006, isEpsgCode=true, epsgNumber=3006))
        // Actual   :Coordinate(xLongitude=674032.3572074447, yLatitude=6580821.991903967, crsIdentifier=CrsIdentifier(crsCode=EPSG:3006, isEpsgCode=true, epsgNumber=3006))
    }

    private double getAverage(List<Coordinate> resultCoordinates, ToDoubleFunction<? super Coordinate> mapperReturningDoubleValueForAverageCalculation) {
        return resultCoordinates.stream().mapToDouble(mapperReturningDoubleValueForAverageCalculation).average().getAsDouble();
    }

    private Coordinate calculateAverageCoordinate(List<Coordinate> resultCoordinates) {
        double averageLat = getAverage(resultCoordinates, c -> c.getYLatitude());
        double averageLon = getAverage(resultCoordinates, c -> c.getXLongitude());
        Set<CrsIdentifier> set = resultCoordinates.stream().map(c -> c.getCrsIdentifier()).collect(Collectors.toSet());
        assertEquals(1, set.size(), "all coordinates should have the same CRS, since thet should all be the result of a transform to the same CRS");
        return Coordinate.createFromYLatitudeXLongitude(averageLat, averageLon, resultCoordinates.get(0).getCrsIdentifier());
    }
}