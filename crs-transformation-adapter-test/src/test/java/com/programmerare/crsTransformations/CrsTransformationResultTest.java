package com.programmerare.crsTransformations;

import static org.hamcrest.CoreMatchers.containsString;
import static org.hamcrest.MatcherAssert.assertThat;

import com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL;
import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsTransformations.coordinate.CrsCoordinateFactory;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.mock;

public class CrsTransformationResultTest {

    private CrsTransformationResult transformResultWithSuccessFalse;
    private CrsCoordinate inputCoordinate;
    private CrsCoordinate outputCoordinate;

    private CrsTransformationAdapter crsTransformationAdapter;

    @BeforeEach
    void beforeEach() {
        inputCoordinate = CrsCoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude(0.0, 0.0, 1234);
        outputCoordinate = inputCoordinate;

        // the mock below is currently not used any more than as a convenient way
        // of constructing an instance of the interface
        crsTransformationAdapter = mock(CrsTransformationAdapter.class);
    }

    @Test
    void transformResult_shouldNotReturnSuccess_whenSuccessParameterIsFalse() {
        transformResultWithSuccessFalse = new CrsTransformationResult(
            inputCoordinate,
            null,
            null,
            false, // parameter success = false !
            crsTransformationAdapter,
            new ArrayList<CrsTransformationResult>(),
            null
        );
        assertFalse(transformResultWithSuccessFalse.isSuccess()); // because of success parameter false
    }

    @Test
    void transformResult_shouldReturnSuccess_whenSuccessParameterIsTrue() {
        transformResultWithSuccessFalse = new CrsTransformationResult(
            inputCoordinate,
            outputCoordinate,
            null,
            true,
            crsTransformationAdapter,
            new ArrayList<CrsTransformationResult>(),
            null
        );
        assertTrue(transformResultWithSuccessFalse.isSuccess());
    }

    @Test
    void transformResultConstruction_shouldThrowException_whenParametersSuccessFalseAndOutputCoordinateNotNull() {
        assertThrows(
            IllegalStateException.class,
            () -> new CrsTransformationResult(
                inputCoordinate, 
                outputCoordinate, // not null (which it should be when success false as below)
                null,
                false,
                crsTransformationAdapter,
                new ArrayList<CrsTransformationResult>(),
                null
            ),
            "unvalid TransformResult object construction should throw exception when success false is combined with output coordinate not being null"
        );
    }

    @Test
    void transformResultConstruction_shouldThrowException_whenParametersSuccessTrueAndOutputCoordinateNull() {
        final CrsCoordinate outputCoordinateNull = null;
        assertThrows(
            IllegalStateException.class,
            () -> new CrsTransformationResult(
                inputCoordinate,
                outputCoordinateNull, // outputCoordinate = null, then success should be false !
                null,
                true,
                crsTransformationAdapter,
                new ArrayList<CrsTransformationResult>(),
                null
            ),
            "unvalid TransformResult object construction should throw exception when success true is combined with null as output coordinate"
        );
    }

    @Test
    void transformResult_shouldThrowException_whenTryingToGetCoordinateWhenSuccessIsFalse() {
        outputCoordinate = null;
        transformResultWithSuccessFalse = new CrsTransformationResult(
            inputCoordinate,
            outputCoordinate,
            null,
            false,
            crsTransformationAdapter,
            new ArrayList<CrsTransformationResult>(),
            null
        );
        Exception e = assertThrows(
            RuntimeException.class,
            () -> transformResultWithSuccessFalse.getOutputCoordinate(),
            "Should not try to get output coordinate unless the result was a success"
        );
        final String exceptionMessage = e.getMessage().toLowerCase().replaceAll("-", "");
        // the purpose of the above row is to make the test less sensitive to the exact message
        // e.g. will match text containing "Pre-condition" (including hyphen and uppercased first letter)
        // The exception message should be something like "Precondition violated ..."
        assertThat(exceptionMessage, containsString("precondition"));
    }

    @Test
    void transformResult_shouldReturnStatisticsObjectWithCorrectAverageAndMeanAndMaxDiffValues_whenCreatingResultWithListOfSubresults() {
        // The setup code below creates four coordinates 
        // representing results from four implementations.
        final double lat1 = 59.330231;
        final double lat2 = 59.330232;
        final double lat3 = 59.330233;
        final double lat4 = 59.330239;
        final double latMean = (lat2 + lat3 ) / 2;
        final double latAverage = (lat1 + lat2 + lat3 + lat4) / 4;
        final double expectedLatDiffMax = lat4-lat1;
        final double lon1 = 18.059192;
        final double lon2 = 18.059193;
        final double lon3 = 18.059194;
        final double lon4 = 18.059198;
        final double lonMean = (lon2 + lon3 ) / 2;
        final double lonAverage = (lon1 + lon2 + lon3 + lon4) / 4;
        final double expectedLonDiffMax = lon4-lon1;
        final CrsCoordinate expectedCoordinateMean = CrsCoordinateFactory.latLon(latMean, lonMean);
        final CrsCoordinate expectedCoordinateAverage = CrsCoordinateFactory.latLon(latAverage, lonAverage);
        
        final CrsCoordinate outputCoordinate1 = CrsCoordinateFactory.latLon(lat1, lon1);
        final CrsCoordinate outputCoordinate2 = CrsCoordinateFactory.latLon(lat2, lon2);
        final CrsCoordinate outputCoordinate3 = CrsCoordinateFactory.latLon(lat3, lon3);
        final CrsCoordinate outputCoordinate4 = CrsCoordinateFactory.latLon(lat4, lon4);

        final CrsTransformationAdapter adapter = new CrsTransformationAdapterGooberCTL(); // not used, might use a mock instead
        final CrsCoordinate inputCoordinateNotUsedInThisTest = CrsCoordinateFactory.latLon(0, 0); // input, not used here in this test
        
        final List<CrsTransformationResult> listOfSubresults = Arrays.asList(
            createCrsTransformationResult(outputCoordinate1, adapter, inputCoordinateNotUsedInThisTest),
            createCrsTransformationResult(outputCoordinate2, adapter, inputCoordinateNotUsedInThisTest),
            createCrsTransformationResult(outputCoordinate3, adapter, inputCoordinateNotUsedInThisTest),
            createCrsTransformationResult(outputCoordinate4, adapter, inputCoordinateNotUsedInThisTest)
        );
        final CrsCoordinate outputCoordinateNotUsedInThisTest = inputCoordinateNotUsedInThisTest;
        final CrsTransformationResult transformResult = new CrsTransformationResult(
            inputCoordinateNotUsedInThisTest,
            outputCoordinateNotUsedInThisTest,
            null,
            true,
            adapter,
            listOfSubresults,
            null
        );

        final CrsTransformationResultStatistic crsTransformationResultStatistic = transformResult.getCrsTransformationResultStatistic();
        assertNotNull(crsTransformationResultStatistic);
        assertTrue(crsTransformationResultStatistic.isStatisticsAvailable());

        assertEquals(4, crsTransformationResultStatistic.getNumberOfResults());
        assertEquals(expectedLatDiffMax, crsTransformationResultStatistic.getMaxDifferenceForYNorthingLatitude());
        assertEquals(expectedLonDiffMax, crsTransformationResultStatistic.getMaxDifferenceForXEastingLongitude());

        final CrsCoordinate coordinateAverage = crsTransformationResultStatistic.getCoordinateAverage();
        final CrsCoordinate coordinateMean = crsTransformationResultStatistic.getCoordinateMedian();
        assertNotNull(coordinateAverage);
        assertNotNull(coordinateMean);
        assertEquals(expectedCoordinateMean, coordinateMean);
        assertEquals(expectedCoordinateAverage, coordinateAverage);
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

}