package com.programmerare.crsTransformationFacadeGooberCTL;

import java.util.Arrays;
import java.util.List;

import com.programmerare.crsTransformations.Coordinate;
import org.junit.Test;
import static org.junit.Assert.assertEquals;

public class CRStransformationFacadeGooberCtlTest {

    @Test
    public void gooberCtlTest() {
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

        int epsgNumberForWgs84 = 4326;

        Coordinate inputCoordinate = new Coordinate(wgs84Lon, wgs84Lat, epsgNumberForWgs84);
        Coordinate outputCoordinate = CRStransformationFacadeGooberCTL.transformWgs84CoordinateToSweref99TM(inputCoordinate);
        assertEquals(sweref99_Y_expected, outputCoordinate.getYLatitude(), 0.5);
        assertEquals(sweref99_X_expected, outputCoordinate.getXLongitude(), 0.5);
    }
}