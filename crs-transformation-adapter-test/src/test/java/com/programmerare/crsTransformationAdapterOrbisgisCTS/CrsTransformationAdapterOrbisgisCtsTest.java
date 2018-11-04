package com.programmerare.crsTransformationAdapterOrbisgisCTS;

import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
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
        final CrsTransformationResult transformResult = crsTransformationAdapterOrbisgis.transform(input, EpsgNumber._2163__US_NATIONAL_ATLAS_EQUAL_AREA__USA);
        assertNotNull(transformResult);
        assertFalse(transformResult.isSuccess());
        // System.out.println("transformResult : " + transformResult.getOutputCoordinate());
        // Output from the above before this bug was fixed by checking for NaN in class 'CrsTransformationAdapterBaseLeaf'
        // transformResult : Coordinate(xEastingLongitude=NaN, yNorthingLatitude=NaN, crsIdentifier=CrsIdentifier(crsCode=EPSG:2163, isEpsgCode=true, epsgNumber=2163))
    }

    // TODO: move this method below to some place where all "Leaf" implementations are tested in the same way and not only Orbis
    @Test
    void isReliableTest() {
        // The tested method 'isReliable' is actually relevant only for aggregated
        // transformations, but nevertheless there is a reaonable behavouor also
        // for the "Leaf" implementations regarding the number of results (always 1)
        // and the "differences" in lat/long for the "different" implementations
        // i.e. the "difference" should always be zero since there is only one implementation
        final CrsTransformationAdapterOrbisgisCTS crsTransformationAdapterOrbisgis = new CrsTransformationAdapterOrbisgisCTS();
        final CrsCoordinate wgs84coordinateInSweden = CrsCoordinateFactory.latLon(59.29,18.03);
        final CrsTransformationResult resultWhenTransformingToSwedishCRS = crsTransformationAdapterOrbisgis.transform(wgs84coordinateInSweden, com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        assertNotNull(resultWhenTransformingToSwedishCRS);
        assertTrue(resultWhenTransformingToSwedishCRS.isSuccess());
        final CrsTransformationResultStatistic crsTransformationResultStatistic = resultWhenTransformingToSwedishCRS.getCrsTransformationResultStatistic();
        assertNotNull(crsTransformationResultStatistic);
        assertTrue(crsTransformationResultStatistic.isStatisticsAvailable());

        final int actualNumberOfResults = crsTransformationResultStatistic.getNumberOfResults();
        assertEquals(1, actualNumberOfResults);
        final double actualMaxDiffXLongitude = crsTransformationResultStatistic.getMaxDiffXLongitude();
        final double actualMaxDiffYLatitude = crsTransformationResultStatistic.getMaxDiffYLatitude();
        final double actualMaxDiffXorY = Math.max(actualMaxDiffXLongitude, actualMaxDiffYLatitude);
        assertEquals(0, actualMaxDiffXorY); // zero differences since there should be only one result !

        assertTrue(resultWhenTransformingToSwedishCRS.isReliable(actualNumberOfResults, actualMaxDiffXorY));

        // assertFalse below since trying to require one more result than available
        assertFalse(resultWhenTransformingToSwedishCRS.isReliable(actualNumberOfResults + 1, actualMaxDiffXorY));

        // assertFalse below since trying to require too small maxdiff
        assertFalse(resultWhenTransformingToSwedishCRS.isReliable(actualNumberOfResults, actualMaxDiffXorY - 0.00000000001));
    }
}