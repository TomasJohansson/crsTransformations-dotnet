package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformations.CRStransformationFacade;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.CrsIdentifier;
import com.programmerare.crsTransformations.crsConstants.ConstantEpsgNumber;
import org.junit.jupiter.api.Test;
import java.util.Arrays;
import java.util.List;
import java.util.Set;
import java.util.function.ToDoubleFunction;
import java.util.stream.Collectors;
import static org.junit.jupiter.api.Assertions.assertEquals;

public class CRStransformationFacadeAverageTest extends CRStransformationFacadeBaseCompositeTest {

    @Test
    void createCRStransformationFacadeAverage() {
        List<Coordinate> coordinateResultsForTheDifferentImplementations = Arrays.asList(resultCoordinateGeoTools, resultCoordinateGooberCTL, resultCoordinateOrbisgisCTS);
        Coordinate coordinateWithAverageLatitudeAndLongitude = calculateAverageCoordinate(coordinateResultsForTheDifferentImplementations);

        CRStransformationFacade facadeCompositeCalculatingAverage = new CRStransformationFacadeAverage(Arrays.asList(facadeGeoTools, facadeGooberCTL, facadeOrbisgisCTS));
        Coordinate coordinateReturnedByCompositeFacade = facadeCompositeCalculatingAverage.transform(wgs84coordinate, ConstantEpsgNumber.SWEREF99TM);

        assertEquals(coordinateWithAverageLatitudeAndLongitude, coordinateReturnedByCompositeFacade);
    }

    private double getAverage(List<Coordinate> resultCoordinates, ToDoubleFunction<? super Coordinate> mapperReturningDoubleValueForAverageCalculation) {
        return resultCoordinates.stream().mapToDouble(mapperReturningDoubleValueForAverageCalculation).average().getAsDouble();
    }

    private Coordinate calculateAverageCoordinate(List<Coordinate> resultCoordinates) {
        double averageLat = getAverage(resultCoordinates, c -> c.getYLatitude());
        double averageLon = getAverage(resultCoordinates, c -> c.getXLongitude());
        Set<CrsIdentifier> set = resultCoordinates.stream().map(c -> c.getCrsIdentifier()).collect(Collectors.toSet());
        assertEquals(1, set.size(), "all coordinates should have the same CRS, since thet should all be the result of a transform to the same CRS");
        return Coordinate.createFromYLatXLong(averageLat, averageLon, resultCoordinates.get(0).getCrsIdentifier());
    }
}