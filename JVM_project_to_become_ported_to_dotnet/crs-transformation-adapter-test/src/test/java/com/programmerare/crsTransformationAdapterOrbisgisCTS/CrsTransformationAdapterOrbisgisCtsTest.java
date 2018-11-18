package com.programmerare.crsTransformationAdapterOrbisgisCTS;

import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsTransformations.CrsTransformationResultStatistic;
import com.programmerare.crsTransformations.CrsTransformationResult;
import com.programmerare.crsTransformations.coordinate.CrsCoordinateFactory;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class CrsTransformationAdapterOrbisgisCtsTest {

    /**
     * Added this method when I dicovered NaN longitude and 0 latitude in a result ... for EPSG 2163
     *      Coordinate(xEastingLongitude=NaN, yNorthingLatitude=0.0, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
     */
    @Test
    void testingNAN() {
        // Coordinate(xEastingLongitude=NaN, yNorthingLatitude=0.0, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
        final CrsTransformationAdapterOrbisgisCTS crsTransformationAdapterOrbisgis = new CrsTransformationAdapterOrbisgisCTS();
        //crsTransformationAdapterOrbisgis.transform()
        // 2163|1245|USA|-127.23566196580043|47.32837112124157
        // EPSG 2163 xLong -127.23566196580043  yLat  47.32837112124157
        final CrsCoordinate input = CrsCoordinateFactory.lonLat(-127.23566196580043 , 47.32837112124157);
        final CrsTransformationResult transformResult = crsTransformationAdapterOrbisgis.transform(input, EpsgNumber.USA__US_NATIONAL_ATLAS_EQUAL_AREA__2163);
        assertNotNull(transformResult);
        assertFalse(transformResult.isSuccess());
        // System.out.println("transformResult : " + transformResult.getOutputCoordinate());
        // Output from the above before this bug was fixed by checking for NaN in class 'CrsTransformationAdapterBaseLeaf'
        // transformResult : Coordinate(xEastingLongitude=NaN, yNorthingLatitude=NaN, crsIdentifier=CrsIdentifier(crsCode=EPSG:2163, isEpsgCode=true, epsgNumber=2163))
    }

}