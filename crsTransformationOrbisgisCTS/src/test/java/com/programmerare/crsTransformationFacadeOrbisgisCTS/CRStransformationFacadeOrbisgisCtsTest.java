package com.programmerare.crsTransformationFacadeOrbisgisCTS;

import java.util.Arrays;
import java.util.List;
import org.junit.Test;
import static org.junit.Assert.assertEquals;

public class CRStransformationFacadeOrbisgisCtsTest {

    @Test
    public void orbisgisCtsTest() {
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

        List<Double> doubles = CRStransformationFacadeOrbisgisCTS.transformWgs84CoordinateToSweref99TM(Arrays.asList(wgs84Lat, wgs84Lon));
        assertEquals(sweref99_Y_expected, doubles.get(0), 0.5);
        assertEquals(sweref99_X_expected, doubles.get(1), 0.5);
    }
}