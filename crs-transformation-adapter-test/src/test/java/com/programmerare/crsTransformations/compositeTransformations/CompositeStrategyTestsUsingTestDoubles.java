package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.*;
import com.programmerare.crsTransformations.coordinate.Coordinate;
import com.programmerare.crsTransformations.coordinate.CoordinateFactory;
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier;
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifierFactory;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

public class CompositeStrategyTestsUsingTestDoubles {

    private static double medianLatitude, averageLatitude, medianLongitude, averageLongitude;
    private static Coordinate inputCoordinateSweref99;
    private static Coordinate outputCoordinateWgs84ForImplementation_1, outputCoordinateWgs84ForImplementation_2, outputCoordinateWgs84ForImplementation_3, outputCoordinateWgs84ForImplementation_4, outputCoordinateWgs84ForImplementation_5;
    private static List<Coordinate> outputCoordinates;
    private static CrsTransformationAdapter leafAdapterImplementation_1, leafAdapterImplementation_2, leafAdapterImplementation_3, leafAdapterImplementation_4, leafAdapterImplementation_5;
    private static List<CrsTransformationAdapter> allLeafAdapters;

    private static CrsIdentifier crsIdentifierWGS84;

    @BeforeAll
    static void beforeAll() {
        double[] outputLatitudes = {
            59.1,
            59.2,
            59.3,
            59.4,
            59.6,
        };
        medianLatitude = 59.3;
        averageLatitude = 59.32;

        double[] outputLongitudes = {
            18.2,
            18.3,
            18.4,
            18.8,
            18.9
        };
        medianLongitude = 18.4;
        averageLongitude = 18.52;

        outputCoordinateWgs84ForImplementation_1 = CoordinateFactory.createFromLatitudeLongitude(outputLatitudes[0],outputLongitudes[3]);
        outputCoordinateWgs84ForImplementation_2 = CoordinateFactory.createFromLatitudeLongitude(outputLatitudes[2],outputLongitudes[1]);
        outputCoordinateWgs84ForImplementation_3 = CoordinateFactory.createFromLatitudeLongitude(outputLatitudes[4],outputLongitudes[4]);
        outputCoordinateWgs84ForImplementation_4 = CoordinateFactory.createFromLatitudeLongitude(outputLatitudes[1],outputLongitudes[0]);
        outputCoordinateWgs84ForImplementation_5 = CoordinateFactory.createFromLatitudeLongitude(outputLatitudes[3],outputLongitudes[2]);
        outputCoordinates = Arrays.asList(outputCoordinateWgs84ForImplementation_1, outputCoordinateWgs84ForImplementation_2, outputCoordinateWgs84ForImplementation_3, outputCoordinateWgs84ForImplementation_4, outputCoordinateWgs84ForImplementation_5);

        leafAdapterImplementation_1 = mock(CrsTransformationAdapter.class);
        leafAdapterImplementation_2 = mock(CrsTransformationAdapter.class);
        leafAdapterImplementation_3 = mock(CrsTransformationAdapter.class);
        leafAdapterImplementation_4 = mock(CrsTransformationAdapter.class);
        leafAdapterImplementation_5 = mock(CrsTransformationAdapter.class);

        inputCoordinateSweref99 = CoordinateFactory.createFromYLatitudeXLongitude(6580822, 674032, EpsgNumber._3006__SWEREF99_TM__SWEDEN);

        CrsTransformationResult leafResult1 = new CrsTransformationResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_1,
            null,
            true,
            leafAdapterImplementation_1,
            new ArrayList<CrsTransformationResult>(),
            null
        );
        CrsTransformationResult leafResult2 = new CrsTransformationResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_2,
            null,
            true,
            leafAdapterImplementation_2,
            new ArrayList<CrsTransformationResult>(),
            null
        );
        CrsTransformationResult leafResult3 = new CrsTransformationResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_3,
            null,
            true,
            leafAdapterImplementation_3,
            new ArrayList<CrsTransformationResult>(),
            null
        );
        CrsTransformationResult leafResult4 = new CrsTransformationResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_4,
            null,
            true,
            leafAdapterImplementation_4,
            new ArrayList<CrsTransformationResult>(),
            null
        );
        CrsTransformationResult r5 = new CrsTransformationResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_5,
            null,
            true,
            leafAdapterImplementation_5,
            new ArrayList<CrsTransformationResult>(),
            null
        );
        crsIdentifierWGS84 = CrsIdentifierFactory.createFromEpsgNumber(EpsgNumber._4326__WGS_84__WORLD);

        when(leafAdapterImplementation_1.transform(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(leafResult1);
        when(leafAdapterImplementation_2.transform(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(leafResult2);
        when(leafAdapterImplementation_3.transform(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(leafResult3);
        when(leafAdapterImplementation_4.transform(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(leafResult4);
        when(leafAdapterImplementation_5.transform(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(r5);

        allLeafAdapters = Arrays.asList(
            leafAdapterImplementation_1,
            leafAdapterImplementation_2,
            leafAdapterImplementation_3,
            leafAdapterImplementation_4,
            leafAdapterImplementation_5
        );
    }

    private final static double SMALL_DELTA_VALUE_FOR_COMPARISONS = 0.00000000000001;

    @Test
    void averageAdapterTest() {
        CrsTransformationAdapterComposite averageCompositeAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage(allLeafAdapters);
        Coordinate resultCoordinate = averageCompositeAdapter.transformToCoordinate(inputCoordinateSweref99, EpsgNumber._4326__WGS_84__WORLD);
        assertNotNull(resultCoordinate);

        // Coordinate expectedAverageCoordinate = CoordinateFactory.createFromLatitudeLongitude(averageLatitude, averageLongitude);
        // assertEquals(expectedAverageCoordinate, result); // this failed because latitude was 59.31999999999999 instead of 59.32
        assertEquals(averageLatitude,  resultCoordinate.getYLatitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
        assertEquals(averageLongitude, resultCoordinate.getXLongitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);

        assertCompositeResultHasLeafSubResults(
            averageCompositeAdapter,
            allLeafAdapters.size() // expectedNumberOfLeafResults
        );
    }

    @Test
    void medianAdapterTest() {
        CrsTransformationAdapterComposite medianCompositeAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian(allLeafAdapters);
        Coordinate resultCoordinate = medianCompositeAdapter.transformToCoordinate(inputCoordinateSweref99, EpsgNumber._4326__WGS_84__WORLD);
        assertNotNull(resultCoordinate);

        assertEquals(medianLatitude,  resultCoordinate.getYLatitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
        assertEquals(medianLongitude, resultCoordinate.getXLongitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);

        assertCompositeResultHasLeafSubResults(
            medianCompositeAdapter,
            allLeafAdapters.size() // expectedNumberOfLeafResults
        );
    }

    @Test
    void ChainOfResponsibilityAdapterTest() {
        CrsTransformationAdapterComposite chainOfResponsibilityCompositeAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationChainOfResponsibility(allLeafAdapters);
        Coordinate resultCoordinate = chainOfResponsibilityCompositeAdapter.transformToCoordinate(inputCoordinateSweref99, EpsgNumber._4326__WGS_84__WORLD);
        assertNotNull(resultCoordinate);

        // The assumption below (according to the setup code in the "before" method in this JUnit class)
        // is that the first adapter in the above list allLeafAdapters will return the result outputCoordinateWgs84ForImplementation_1
        assertEquals(outputCoordinateWgs84ForImplementation_1.getYLatitude(),  resultCoordinate.getYLatitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
        assertEquals(outputCoordinateWgs84ForImplementation_1.getXLongitude(), resultCoordinate.getXLongitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);

        assertCompositeResultHasLeafSubResults(
            chainOfResponsibilityCompositeAdapter,
            1 // expectedNumberOfLeafResults
        );
    }

    @Test
    void weightedAverageAdapterTest() {
        when(leafAdapterImplementation_1.getLongNameOfImplementation()).thenReturn("1");
        when(leafAdapterImplementation_2.getLongNameOfImplementation()).thenReturn("2");
        when(leafAdapterImplementation_3.getLongNameOfImplementation()).thenReturn("3");
        when(leafAdapterImplementation_4.getLongNameOfImplementation()).thenReturn("4");
        when(leafAdapterImplementation_5.getLongNameOfImplementation()).thenReturn("5");

        final double[] weights = {1,2,4,5,9};
        double totWeights = 0;
        double totLats = 0;
        double totLons = 0;
        for (int i = 0; i <weights.length ; i++) {
            final double weight = weights[i];
            totWeights += weight;
            final Coordinate coordinate = outputCoordinates.get(i);
            totLats += weight * coordinate.getYLatitude();
            totLons += weight * coordinate.getXLongitude();
        }
        final double weightedLat = totLats / totWeights;
        final double weightedLon = totLons / totWeights;
        final Coordinate expectedWeightedAverage = CoordinateFactory.createFromLatitudeLongitude(weightedLat, weightedLon);

        List<CrsTransformationAdapterWeight> weightedAdapters = Arrays.asList(
            CrsTransformationAdapterWeight.createFromInstance(leafAdapterImplementation_1, weights[0]),
            CrsTransformationAdapterWeight.createFromInstance(leafAdapterImplementation_2, weights[1]),
            CrsTransformationAdapterWeight.createFromInstance(leafAdapterImplementation_3, weights[2]),
            CrsTransformationAdapterWeight.createFromInstance(leafAdapterImplementation_4, weights[3]),
            CrsTransformationAdapterWeight.createFromInstance(leafAdapterImplementation_5, weights[4])
        );

        final CrsTransformationAdapterComposite weightedAverageCompositeAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationWeightedAverage(weightedAdapters);
        final Coordinate result = weightedAverageCompositeAdapter.transformToCoordinate(inputCoordinateSweref99, EpsgNumber._4326__WGS_84__WORLD);
        assertNotNull(result);

        assertEquals(expectedWeightedAverage.getYLatitude(),  result.getYLatitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
        assertEquals(expectedWeightedAverage.getXLongitude(), result.getXLongitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);

        assertCompositeResultHasLeafSubResults(
            weightedAverageCompositeAdapter,
            allLeafAdapters.size() // expectedNumberOfLeafResults
        );
    }

    private void assertCompositeResultHasLeafSubResults(
            CrsTransformationAdapterComposite compositeAdapter,
            int expectedNumberOfLeafResults
    ) {
        CrsTransformationResult compositeTransformResult = compositeAdapter.transform(inputCoordinateSweref99, crsIdentifierWGS84);
        assertNotNull(compositeTransformResult);
        assertTrue(compositeTransformResult.isSuccess());
        //assertEquals(expectedNumberOfLeafResults, allLeafAdapters.size()); // five "leafs" were used to calculate the composite
        assertEquals(expectedNumberOfLeafResults, compositeTransformResult.getTransformationResultChildren().size());

        List<CrsTransformationResult> subResults = compositeTransformResult.getTransformationResultChildren();
        for (int i = 0; i < subResults.size(); i++) {
            CrsTransformationResult transformResult = subResults.get(i);
            CrsTransformationAdapter leafAdapter = allLeafAdapters.get(i);
            CrsTransformationResult transformResultForLeaf = leafAdapter.transform(inputCoordinateSweref99, crsIdentifierWGS84);
            assertNotNull(transformResultForLeaf);
            assertTrue(transformResultForLeaf.isSuccess());
            assertEqualCoordinate(transformResult.getOutputCoordinate(), transformResultForLeaf.getOutputCoordinate());
            assertEquals(0, transformResultForLeaf.getTransformationResultChildren().size()); // no subresults for a leaf
        }
    }

    private void assertEqualCoordinate(
        Coordinate c1,
        Coordinate c2
    ) {
        assertEquals(c1.getYLatitude(), c2.getYLatitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
        assertEquals(c1.getXLongitude(), c2.getXLongitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
    }

    // --------------------------------------------------------------
    @Test
    void simpleExampleShowingHowToUseTestStubbingWithMockito() {
        // Mockito documentation:
        // http://static.javadoc.io/org.mockito/mockito-core/2.23.0/org/mockito/Mockito.html

        List<String> mockedList = mock(List.class);

        when(mockedList.get(0)).thenReturn("first");
        when(mockedList.get(1)).thenReturn("second");

        // Note that the method/parameter combination "get(0)" and "get(1)"
        // can be invoked multiple times and in different order sompared to the order defined above
        assertEquals("second", mockedList.get(1));
        assertEquals("first", mockedList.get(0));
        assertEquals("first", mockedList.get(0));
        assertEquals("second", mockedList.get(1));
    }
    // --------------------------------------------------------------
}