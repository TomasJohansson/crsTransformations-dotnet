package com.programmerare.crsTransformationAdapterOrbisgisCTS;

import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.ResultsStatistic;
import com.programmerare.crsTransformations.TransformResult;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class CrsTransformationAdapterOrbisgisCtsTest {

    /**
     * Added this method when I dicovered NaN longitude and 0 latitude in a result ... for EPSG 2163
     *      Coordinate(xLongitude=NaN, yLatitude=0.0, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
     */
    @Test
    void testingNAN() {
        // Coordinate(xLongitude=NaN, yLatitude=0.0, crsIdentifier=CrsIdentifier(crsCode=EPSG:4326, isEpsgCode=true, epsgNumber=4326)) , com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS
        final CrsTransformationAdapterOrbisgisCTS crsTransformationAdapterOrbisgis = new CrsTransformationAdapterOrbisgisCTS();
        //crsTransformationAdapterOrbisgis.transform()
        // 2163|1245|USA|-127.23566196580043|47.32837112124157
        // EPSG 2163 xLong -127.23566196580043  yLat  47.32837112124157
        final Coordinate input = Coordinate.lonLat(-127.23566196580043 , 47.32837112124157);
        final TransformResult transformResult = crsTransformationAdapterOrbisgis.transform(input, EpsgNumber._2163__US_NATIONAL_ATLAS_EQUAL_AREA__USA);
        assertNotNull(transformResult);
        assertFalse(transformResult.isSuccess());
        // System.out.println("transformResult : " + transformResult.getOutputCoordinate());
        // Output from the above before this bug was fixed by checking for NaN in class 'CrsTransformationAdapterBaseLeaf'
        // transformResult : Coordinate(xLongitude=NaN, yLatitude=NaN, crsIdentifier=CrsIdentifier(crsCode=EPSG:2163, isEpsgCode=true, epsgNumber=2163))
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
        final Coordinate wgs84coordinateInSweden = Coordinate.latLon(59.29,18.03);
        final TransformResult resultWhenTransformingToSwedishCRS = crsTransformationAdapterOrbisgis.transform(wgs84coordinateInSweden, com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        assertNotNull(resultWhenTransformingToSwedishCRS);
        assertTrue(resultWhenTransformingToSwedishCRS.isSuccess());
        final ResultsStatistic resultsStatistic = resultWhenTransformingToSwedishCRS.getResultsStatistic();
        assertNotNull(resultsStatistic);
        assertTrue(resultsStatistic.isStatisticsAvailable());

        final int actualNumberOfResults = resultsStatistic.getNumberOfResults();
        assertEquals(1, actualNumberOfResults);
        final double actualMaxDiffXLongitude = resultsStatistic.getMaxDiffXLongitude();
        final double actualMaxDiffYLatitude = resultsStatistic.getMaxDiffYLatitude();
        final double actualMaxDiffXorY = Math.max(actualMaxDiffXLongitude, actualMaxDiffYLatitude);
        assertEquals(0, actualMaxDiffXorY); // zero differences since there should be only one result !

        assertTrue(resultWhenTransformingToSwedishCRS.isReliable(actualNumberOfResults, actualMaxDiffXorY));

        // assertFalse below since trying to require one more result than available
        assertFalse(resultWhenTransformingToSwedishCRS.isReliable(actualNumberOfResults + 1, actualMaxDiffXorY));

        // assertFalse below since trying to require too small maxdiff
        assertFalse(resultWhenTransformingToSwedishCRS.isReliable(actualNumberOfResults, actualMaxDiffXorY - 0.00000000001));
    }
}