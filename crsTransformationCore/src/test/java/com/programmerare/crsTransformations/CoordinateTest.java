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
    void createCoordinatesWithEpsgNumberButWithDifferentOrderOfLatLongParameters() {
        Coordinate coordinate1 = Coordinate.createFromXLongYLat(xLongitude, yLatitude, epsgNumber);
        Coordinate coordinate2 = Coordinate.createFromYLatXLong(yLatitude, xLongitude, epsgNumber);
        assertEqualCoordinates(coordinate1, coordinate2);
    }

    @Test
    void createCoordinatesWithCrsCodeButWithDifferentOrderOfLatLongParameters() {
        String crsCode = "EPSG:3006";
        Coordinate coordinate1 = Coordinate.createFromXLongYLat(xLongitude, yLatitude, crsCode);
        Coordinate coordinate2 = Coordinate.createFromYLatXLong(yLatitude, xLongitude, crsCode);
        assertEqualCoordinates(coordinate1, coordinate2);
    }

    @Test
    void createCoordinatesWithCrsIdentifierButWithDifferentOrderOfLatLongParameters() {
        CrsIdentifier crsIdentifier = CrsIdentifier.createFromEpsgNumber(3006);
        Coordinate coordinate1 = Coordinate.createFromXLongYLat(xLongitude, yLatitude, crsIdentifier);
        Coordinate coordinate2 = Coordinate.createFromYLatXLong(yLatitude, xLongitude, crsIdentifier);
        assertEqualCoordinates(coordinate1, coordinate2);
    }

    private void assertEqualCoordinates(Coordinate coordinate1, Coordinate coordinate2) {
        assertEquals(coordinate1.getXLongitude(), coordinate2.getXLongitude(), deltaTolerance);
        assertEquals(coordinate1.getYLatitude(), coordinate2.getYLatitude(), deltaTolerance);
        assertEquals(coordinate1.getCrsIdentifier(), coordinate2.getCrsIdentifier()); // data class
        assertEquals(coordinate1.getCrsIdentifier().getEpsgNumber(), coordinate2.getCrsIdentifier().getEpsgNumber());
    }
}