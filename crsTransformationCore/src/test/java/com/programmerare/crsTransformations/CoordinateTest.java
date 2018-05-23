package com.programmerare.crsTransformations;

import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.assertEquals;

class CoordinateTest {
    private final double deltaTolerance = 0.00001;
    private final double xLongitude = 12.34;
    private final double yLatitude = 56.67;
    private final int epsgNumber = 3006;

    @Test
    void trivialCreateCoordinateTest() {
        Coordinate coordinate = Coordinate.createFromXLongYLat(xLongitude, yLatitude, epsgNumber);
        assertEquals(xLongitude, coordinate.getXLongitude(), deltaTolerance);
        assertEquals(yLatitude, coordinate.getYLatitude(), deltaTolerance);
        assertEquals(epsgNumber, coordinate.getCrsIdentifier().getEpsgNumber());
    }

    @Test
    void createCoordinatesWithDifferentCreationMethods() {
        Coordinate coordinate1 = Coordinate.createFromXLongYLat(xLongitude, yLatitude, epsgNumber);
        Coordinate coordinate2 = Coordinate.createFromYLatXLong(yLatitude, xLongitude, epsgNumber);
        assertEquals(coordinate1.getXLongitude(), coordinate2.getXLongitude(), deltaTolerance);
        assertEquals(coordinate1.getYLatitude(), coordinate2.getYLatitude(), deltaTolerance);
        assertEquals(coordinate1.getCrsIdentifier().getEpsgNumber(), coordinate2.getCrsIdentifier().getEpsgNumber());
    }

}