package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsTransformations.CrsTransformationResultStatistic;
import com.programmerare.crsTransformations.CrsTransformationResult;
import com.programmerare.crsTransformations.coordinate.CrsCoordinateFactory;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

public class CrsTransformationAdapterCompositeTest {

    @Test
    void isReliableTest() {
        final CrsTransformationAdapterComposite crsTransformationComposite = CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian();

        final CrsCoordinate wgs84coordinateInSweden = CrsCoordinateFactory.latLon(59.31,18.04);
        final CrsTransformationResult resultWhenTransformingToSwedishCRS = crsTransformationComposite.transform(wgs84coordinateInSweden, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        assertNotNull(resultWhenTransformingToSwedishCRS);
        assertTrue(resultWhenTransformingToSwedishCRS.isSuccess());
        final CrsTransformationResultStatistic crsTransformationResultStatistic = resultWhenTransformingToSwedishCRS.getCrsTransformationResultStatistic();
        assertNotNull(crsTransformationResultStatistic);
        assertTrue(crsTransformationResultStatistic.isStatisticsAvailable());

        final int actualNumberOfResults = crsTransformationResultStatistic.getNumberOfResults();
        assertEquals(5, actualNumberOfResults); // fragile but will be very easy to detect and fix if/when a new implementation is added to the factory
        final double actualMaxDiffXLongitude = crsTransformationResultStatistic.getMaxDifferenceForXEastingLongitude();
        final double actualMaxDiffYLatitude = crsTransformationResultStatistic.getMaxDifferenceForYNorthingLatitude();
        final double actualMaxDiffXorY = Math.max(actualMaxDiffXLongitude, actualMaxDiffYLatitude);

        assertTrue(resultWhenTransformingToSwedishCRS.isReliable(actualNumberOfResults, actualMaxDiffXorY));

        // assertFalse below since trying to require one more result than available
        assertFalse(resultWhenTransformingToSwedishCRS.isReliable(actualNumberOfResults + 1, actualMaxDiffXorY));

        // assertFalse below since trying to require too small maxdiff
        assertFalse(resultWhenTransformingToSwedishCRS.isReliable(actualNumberOfResults, actualMaxDiffXorY - 0.00000000001));
    }
}
