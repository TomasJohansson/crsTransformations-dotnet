package com.programmerare.crsTransformationFacadeOrbisgisCTS;

import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.TransformResult;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotNull;

public class CrsTransformationFacadeOrbisgisCtsTest {

    /**
     * Added this method when I dicovered NaN longitude and 0 latitude in a result ... for EPSG 2163
     *      Coordinate(xLongitude=NaN, yLatitude=0.0, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationFacadeOrbisgisCTS.CrsTransformationFacadeOrbisgisCTS
     */
    @Test
    void testingNAN() {
        // Coordinate(xLongitude=NaN, yLatitude=0.0, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationFacadeOrbisgisCTS.CrsTransformationFacadeOrbisgisCTS
        final CrsTransformationFacadeOrbisgisCTS crsTransformationFacadeOrbisgis = new CrsTransformationFacadeOrbisgisCTS();
        //crsTransformationFacadeOrbisgis.transform()
        // 2163|1245|USA|-127.23566196580043|47.32837112124157
        // EPSG 2163 xLong -127.23566196580043  yLat  47.32837112124157
        final Coordinate input = Coordinate.lonLat(-127.23566196580043 , 47.32837112124157);
        final TransformResult transformResult = crsTransformationFacadeOrbisgis.transform(input, EpsgNumber._2163__US_NATIONAL_ATLAS_EQUAL_AREA__USA);
        assertNotNull(transformResult);
        assertFalse(transformResult.isSuccess());
        // System.out.println("transformResult : " + transformResult.getOutputCoordinate());
        // Output from the above before this bug was fixed by checking for NaN in class 'CrsTransformationFacadeBaseLeaf'
        // transformResult : Coordinate(xLongitude=NaN, yLatitude=NaN, crsIdentifier=CrsIdentifier(crsCode=EPSG:2163, isEpsgCode=true, epsgNumber=2163))
    }
}