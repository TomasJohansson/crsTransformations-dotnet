package com.programmerare.crsTransformations;

import com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL;
import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsTransformations.coordinate.CrsCoordinateFactory;
import org.junit.jupiter.api.BeforeEach;

import java.util.Arrays;
import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

// Base class with some common code reused by its subclasses 
abstract class CrsTransformationResultTestBase {
    protected List<CrsTransformationResult> listOfSubresultsForStatisticsTest;
    protected double expectedLatDiffMax, expectedLonDiffMax;
    protected CrsCoordinate expectedCoordinateMean, expectedCoordinateAverage;
    protected CrsCoordinate inputCoordinateNotUsedInStatisticsTest, outputCoordinateNotUsedInStatisticsTest;
    protected CrsTransformationAdapter adapterForStatisticsTest;
    
    @BeforeEach
    void setup() {
        // The setup code below creates four coordinates 
        // representing results from four implementations.
        final double lat1 = 59.330231;
        final double lat2 = 59.330232;
        final double lat3 = 59.330233;
        final double lat4 = 59.330239;
        final double latMean = (lat2 + lat3 ) / 2;
        final double latAverage = (lat1 + lat2 + lat3 + lat4) / 4;
        expectedLatDiffMax = lat4-lat1;
        final double lon1 = 18.059192;
        final double lon2 = 18.059193;
        final double lon3 = 18.059194;
        final double lon4 = 18.059198;
        final double lonMean = (lon2 + lon3 ) / 2;
        final double lonAverage = (lon1 + lon2 + lon3 + lon4) / 4;
        expectedLonDiffMax = lon4-lon1;
        expectedCoordinateMean = CrsCoordinateFactory.latLon(latMean, lonMean);
        expectedCoordinateAverage = CrsCoordinateFactory.latLon(latAverage, lonAverage);

        final CrsCoordinate outputCoordinate1 = CrsCoordinateFactory.latLon(lat1, lon1);
        final CrsCoordinate outputCoordinate2 = CrsCoordinateFactory.latLon(lat2, lon2);
        final CrsCoordinate outputCoordinate3 = CrsCoordinateFactory.latLon(lat3, lon3);
        final CrsCoordinate outputCoordinate4 = CrsCoordinateFactory.latLon(lat4, lon4);

        adapterForStatisticsTest = new CrsTransformationAdapterGooberCTL(); // not used, might use a mock instead
        inputCoordinateNotUsedInStatisticsTest = CrsCoordinateFactory.latLon(0, 0); // input, not used here in this test
        outputCoordinateNotUsedInStatisticsTest = inputCoordinateNotUsedInStatisticsTest;

        listOfSubresultsForStatisticsTest = Arrays.asList(
            createCrsTransformationResult(outputCoordinate1, adapterForStatisticsTest, inputCoordinateNotUsedInStatisticsTest),
            createCrsTransformationResult(outputCoordinate2, adapterForStatisticsTest, inputCoordinateNotUsedInStatisticsTest),
            createCrsTransformationResult(outputCoordinate3, adapterForStatisticsTest, inputCoordinateNotUsedInStatisticsTest),
            createCrsTransformationResult(outputCoordinate4, adapterForStatisticsTest, inputCoordinateNotUsedInStatisticsTest)
        );
    }

    private CrsTransformationResult createCrsTransformationResult(
        CrsCoordinate outputCoordinate1,
        CrsTransformationAdapter adapter,
        CrsCoordinate inputCoordinateNotUsedInThisTest
    ) {
        final List<CrsTransformationResult> resultList = Arrays.asList();
        final Exception exceptionNull = null;
        final boolean isSuccessTrue = true;
        return new CrsTransformationResult(inputCoordinateNotUsedInThisTest, outputCoordinate1, exceptionNull, isSuccessTrue, adapter, resultList, null);
    }

    protected void assertCrsTransformationResultStatistic(CrsTransformationResultStatistic crsTransformationResultStatistic) {
        assertNotNull(crsTransformationResultStatistic);
        assertTrue(crsTransformationResultStatistic.isStatisticsAvailable());

        assertEquals(4, crsTransformationResultStatistic.getNumberOfResults());
        assertEquals(this.expectedLatDiffMax, crsTransformationResultStatistic.getMaxDifferenceForYNorthingLatitude());
        assertEquals(this.expectedLonDiffMax, crsTransformationResultStatistic.getMaxDifferenceForXEastingLongitude());
        
        final CrsCoordinate coordinateAverage = crsTransformationResultStatistic.getCoordinateAverage();
        final CrsCoordinate coordinateMean = crsTransformationResultStatistic.getCoordinateMedian();
        assertNotNull(coordinateAverage);
        assertNotNull(coordinateMean);
        assertEquals(this.expectedCoordinateMean, coordinateMean);
        assertEquals(this.expectedCoordinateAverage, coordinateAverage);
        
    }
}
