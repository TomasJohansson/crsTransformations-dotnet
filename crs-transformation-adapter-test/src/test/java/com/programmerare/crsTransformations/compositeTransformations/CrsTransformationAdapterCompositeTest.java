package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.ResultsStatistic;
import com.programmerare.crsTransformations.TransformResult;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

public class CrsTransformationAdapterCompositeTest {

    @Test
    void isReliableTest() {
        final CrsTransformationAdapterComposite crsTransformationComposite = CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian();

        final Coordinate wgs84coordinateInSweden = Coordinate.latLon(59.31,18.04);
        final TransformResult resultWhenTransformingToSwedishCRS = crsTransformationComposite.transform(wgs84coordinateInSweden, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        assertNotNull(resultWhenTransformingToSwedishCRS);
        assertTrue(resultWhenTransformingToSwedishCRS.isSuccess());
        final ResultsStatistic resultsStatistic = resultWhenTransformingToSwedishCRS.getResultsStatistic();
        assertNotNull(resultsStatistic);
        assertTrue(resultsStatistic.isStatisticsAvailable());

        final int actualNumberOfResults = resultsStatistic.getNumberOfResults();
        assertEquals(5, actualNumberOfResults); // fragile but will be very easy to detect and fix if/when a new implementation is added to the factory
        final double actualMaxDiffXLongitude = resultsStatistic.getMaxDiffXLongitude();
        final double actualMaxDiffYLatitude = resultsStatistic.getMaxDiffYLatitude();
        final double actualMaxDiffXorY = Math.max(actualMaxDiffXLongitude, actualMaxDiffYLatitude);

        assertTrue(resultWhenTransformingToSwedishCRS.isReliable(actualNumberOfResults, actualMaxDiffXorY));

        // assertFalse below since trying to require one more result than available
        assertFalse(resultWhenTransformingToSwedishCRS.isReliable(actualNumberOfResults + 1, actualMaxDiffXorY));

        // assertFalse below since trying to require too small maxdiff
        assertFalse(resultWhenTransformingToSwedishCRS.isReliable(actualNumberOfResults, actualMaxDiffXorY - 0.00000000001));
    }
}
