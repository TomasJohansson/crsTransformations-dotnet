package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformations.CrsTransformationAdapter;
import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsTransformations.coordinate.CrsCoordinateFactory;
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier;
import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.CrsTransformationResult;
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
    void createCRSTransformationAdapterAverage() {
        CrsCoordinate coordinateWithAverageLatitudeAndLongitude = calculateAverageCoordinate(super.allCoordinateResultsForTheDifferentImplementations);

        CrsTransformationAdapter averageCompositeAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage(
            allAdapters
        );
        CrsTransformationResult averageResult = averageCompositeAdapter.transform(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        assertNotNull(averageResult);
        assertTrue(averageResult.isSuccess());
        assertEquals(super.allCoordinateResultsForTheDifferentImplementations.size(), averageResult.getTransformationResultChildren().size());

        CrsCoordinate coordinateReturnedByCompositeAdapter = averageResult.getOutputCoordinate();

        assertEquals(coordinateWithAverageLatitudeAndLongitude.getXEastingLongitude(), coordinateReturnedByCompositeAdapter.getXEastingLongitude(), delta);
        assertEquals(coordinateWithAverageLatitudeAndLongitude.getYNorthingLatitude(), coordinateReturnedByCompositeAdapter.getYNorthingLatitude(), delta);
        // assertEquals(coordinateWithAverageLatitudeAndLongitude, coordinateReturnedByCompositeAdapter);
        // Expected :Coordinate(xEastingLongitude=674032.3572074446, yNorthingLatitude=6580821.991903967, crsIdentifier=CrsIdentifier(crsCode=EPSG:3006, isEpsgCode=true, epsgNumber=3006))
        // Actual   :Coordinate(xEastingLongitude=674032.3572074447, yNorthingLatitude=6580821.991903967, crsIdentifier=CrsIdentifier(crsCode=EPSG:3006, isEpsgCode=true, epsgNumber=3006))
    }

    private double getAverage(List<CrsCoordinate> resultCoordinates, ToDoubleFunction<? super CrsCoordinate> mapperReturningDoubleValueForAverageCalculation) {
        return resultCoordinates.stream().mapToDouble(mapperReturningDoubleValueForAverageCalculation).average().getAsDouble();
    }

    private CrsCoordinate calculateAverageCoordinate(List<CrsCoordinate> resultCoordinates) {
        double averageLat = getAverage(resultCoordinates, c -> c.getYNorthingLatitude());
        double averageLon = getAverage(resultCoordinates, c -> c.getXEastingLongitude());
        Set<CrsIdentifier> set = resultCoordinates.stream().map(c -> c.getCrsIdentifier()).collect(Collectors.toSet());
        assertEquals(1, set.size(), "all coordinates should have the same CRS, since thet should all be the result of a transform to the same CRS");
        return CrsCoordinateFactory.createFromYLatitudeXLongitude(averageLat, averageLon, resultCoordinates.get(0).getCrsIdentifier());
    }
}