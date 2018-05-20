package com.programmerare.crsTransformations;

import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.assertEquals;

class CoordinateTest {

    @Test
    void coordinateTest() {
        double deltaTolerance = 0.00001;

        double xLongitude = 12.34;
        double yLatitude = 56.67;
        int epsgNumber = 3006;

        Coordinate coordinate = new Coordinate(xLongitude, yLatitude, epsgNumber);

        assertEquals(xLongitude, coordinate.getXLongitude(), deltaTolerance);
        assertEquals(yLatitude, coordinate.getYLatitude(), deltaTolerance);
        assertEquals(epsgNumber, coordinate.getEpsgNumber());
    }
}