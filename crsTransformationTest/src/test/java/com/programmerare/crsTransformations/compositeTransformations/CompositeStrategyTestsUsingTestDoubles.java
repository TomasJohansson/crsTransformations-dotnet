package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.*;
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
    private static CrsTransformationFacade leafFacadeImplementation_1, leafFacadeImplementation_2, leafFacadeImplementation_3, leafFacadeImplementation_4, leafFacadeImplementation_5;
    private static List<CrsTransformationFacade> allLeafFacades;

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

        outputCoordinateWgs84ForImplementation_1 = Coordinate.createFromLatitudeLongitude(outputLatitudes[0],outputLongitudes[3]);
        outputCoordinateWgs84ForImplementation_2 = Coordinate.createFromLatitudeLongitude(outputLatitudes[2],outputLongitudes[1]);
        outputCoordinateWgs84ForImplementation_3 = Coordinate.createFromLatitudeLongitude(outputLatitudes[4],outputLongitudes[4]);
        outputCoordinateWgs84ForImplementation_4 = Coordinate.createFromLatitudeLongitude(outputLatitudes[1],outputLongitudes[0]);
        outputCoordinateWgs84ForImplementation_5 = Coordinate.createFromLatitudeLongitude(outputLatitudes[3],outputLongitudes[2]);
        outputCoordinates = Arrays.asList(outputCoordinateWgs84ForImplementation_1, outputCoordinateWgs84ForImplementation_2, outputCoordinateWgs84ForImplementation_3, outputCoordinateWgs84ForImplementation_4, outputCoordinateWgs84ForImplementation_5);

        leafFacadeImplementation_1 = mock(CrsTransformationFacade.class);
        leafFacadeImplementation_2 = mock(CrsTransformationFacade.class);
        leafFacadeImplementation_3 = mock(CrsTransformationFacade.class);
        leafFacadeImplementation_4 = mock(CrsTransformationFacade.class);
        leafFacadeImplementation_5 = mock(CrsTransformationFacade.class);

        inputCoordinateSweref99 = Coordinate.createFromYLatitudeXLongitude(6580822, 674032, EpsgNumber._3006__SWEREF99_TM__SWEDEN);

        TransformResult leafResult1 = new TransformResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_1,
            null,
            true,
            leafFacadeImplementation_1,
            new ArrayList<TransformResult>(),
            null
        );
        TransformResult leafResult2 = new TransformResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_2,
            null,
            true,
            leafFacadeImplementation_2,
            new ArrayList<TransformResult>(),
            null
        );
        TransformResult leafResult3 = new TransformResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_3,
            null,
            true,
            leafFacadeImplementation_3,
            new ArrayList<TransformResult>(),
            null
        );
        TransformResult leafResult4 = new TransformResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_4,
            null,
            true,
            leafFacadeImplementation_4,
            new ArrayList<TransformResult>(),
            null
        );
        TransformResult r5 = new TransformResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_5,
            null,
            true,
            leafFacadeImplementation_5,
            new ArrayList<TransformResult>(),
            null
        );
        crsIdentifierWGS84 = CrsIdentifier.createFromEpsgNumber(EpsgNumber._4326__WGS_84__WORLD);

        when(leafFacadeImplementation_1.transform(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(leafResult1);
        when(leafFacadeImplementation_2.transform(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(leafResult2);
        when(leafFacadeImplementation_3.transform(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(leafResult3);
        when(leafFacadeImplementation_4.transform(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(leafResult4);
        when(leafFacadeImplementation_5.transform(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(r5);

        allLeafFacades = Arrays.asList(
            leafFacadeImplementation_1,
            leafFacadeImplementation_2,
            leafFacadeImplementation_3,
            leafFacadeImplementation_4,
            leafFacadeImplementation_5
        );
    }

    private final static double SMALL_DELTA_VALUE_FOR_COMPARISONS = 0.00000000000001;

    @Test
    void averageFacadeTest() {
        CrsTransformationFacadeComposite averageCompositeFacade = CrsTransformationFacadeCompositeFactory.createCrsTransformationAverage(allLeafFacades);
        Coordinate resultCoordinate = averageCompositeFacade.transformToCoordinate(inputCoordinateSweref99, EpsgNumber._4326__WGS_84__WORLD);
        assertNotNull(resultCoordinate);

        // Coordinate expectedAverageCoordinate = Coordinate.createFromLatitudeLongitude(averageLatitude, averageLongitude);
        // assertEquals(expectedAverageCoordinate, result); // this failed because latitude was 59.31999999999999 instead of 59.32
        assertEquals(averageLatitude,  resultCoordinate.getYLatitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
        assertEquals(averageLongitude, resultCoordinate.getXLongitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);

        assertCompositeResultHasLeafSubResults(
            averageCompositeFacade,
            allLeafFacades.size() // expectedNumberOfLeafResults
        );
    }

    @Test
    void medianFacadeTest() {
        CrsTransformationFacadeComposite medianCompositeFacade = CrsTransformationFacadeCompositeFactory.createCrsTransformationMedian(allLeafFacades);
        Coordinate resultCoordinate = medianCompositeFacade.transformToCoordinate(inputCoordinateSweref99, EpsgNumber._4326__WGS_84__WORLD);
        assertNotNull(resultCoordinate);

        assertEquals(medianLatitude,  resultCoordinate.getYLatitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
        assertEquals(medianLongitude, resultCoordinate.getXLongitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);

        assertCompositeResultHasLeafSubResults(
            medianCompositeFacade,
            allLeafFacades.size() // expectedNumberOfLeafResults
        );
    }

    @Test
    void ChainOfResponsibilityFacadeTest() {
        CrsTransformationFacadeComposite chainOfResponsibilityCompositeFacade = CrsTransformationFacadeCompositeFactory.createCrsTransformationChainOfResponsibility(allLeafFacades);
        Coordinate resultCoordinate = chainOfResponsibilityCompositeFacade.transformToCoordinate(inputCoordinateSweref99, EpsgNumber._4326__WGS_84__WORLD);
        assertNotNull(resultCoordinate);

        // The assumption below (according to the setup code in the "before" method in this JUnit class)
        // is that the first facade in the above list allLeafFacades will return the result outputCoordinateWgs84ForImplementation_1
        assertEquals(outputCoordinateWgs84ForImplementation_1.getYLatitude(),  resultCoordinate.getYLatitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
        assertEquals(outputCoordinateWgs84ForImplementation_1.getXLongitude(), resultCoordinate.getXLongitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);

        assertCompositeResultHasLeafSubResults(
            chainOfResponsibilityCompositeFacade,
            1 // expectedNumberOfLeafResults
        );
    }

    @Test
    void weightedAverageFacadeTest() {
        when(leafFacadeImplementation_1.getNameOfImplementation()).thenReturn("1");
        when(leafFacadeImplementation_2.getNameOfImplementation()).thenReturn("2");
        when(leafFacadeImplementation_3.getNameOfImplementation()).thenReturn("3");
        when(leafFacadeImplementation_4.getNameOfImplementation()).thenReturn("4");
        when(leafFacadeImplementation_5.getNameOfImplementation()).thenReturn("5");

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
        final Coordinate expectedWeightedAverage = Coordinate.createFromLatitudeLongitude(weightedLat, weightedLon);

        List<FacadeWeight> weightedFacades = Arrays.asList(
            FacadeWeight.createFromInstance(leafFacadeImplementation_1, weights[0]),
            FacadeWeight.createFromInstance(leafFacadeImplementation_2, weights[1]),
            FacadeWeight.createFromInstance(leafFacadeImplementation_3, weights[2]),
            FacadeWeight.createFromInstance(leafFacadeImplementation_4, weights[3]),
            FacadeWeight.createFromInstance(leafFacadeImplementation_5, weights[4])
        );

        final CrsTransformationFacadeComposite weightedAverageCompositeFacade = CrsTransformationFacadeCompositeFactory.createCrsTransformationWeightedAverage(weightedFacades);
        final Coordinate result = weightedAverageCompositeFacade.transformToCoordinate(inputCoordinateSweref99, EpsgNumber._4326__WGS_84__WORLD);
        assertNotNull(result);

        assertEquals(expectedWeightedAverage.getYLatitude(),  result.getYLatitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
        assertEquals(expectedWeightedAverage.getXLongitude(), result.getXLongitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);

        assertCompositeResultHasLeafSubResults(
            weightedAverageCompositeFacade,
            allLeafFacades.size() // expectedNumberOfLeafResults
        );
    }

    private void assertCompositeResultHasLeafSubResults(
        CrsTransformationFacadeComposite compositeFacade,
        int expectedNumberOfLeafResults
    ) {
        TransformResult compositeTransformResult = compositeFacade.transform(inputCoordinateSweref99, crsIdentifierWGS84);
        assertNotNull(compositeTransformResult);
        assertTrue(compositeTransformResult.isSuccess());
        //assertEquals(expectedNumberOfLeafResults, allLeafFacades.size()); // five "leafs" were used to calculate the composite
        assertEquals(expectedNumberOfLeafResults, compositeTransformResult.getSubResults().size());

        List<TransformResult> subResults = compositeTransformResult.getSubResults();
        for (int i = 0; i < subResults.size(); i++) {
            TransformResult transformResult = subResults.get(i);
            CrsTransformationFacade leafFacade = allLeafFacades.get(i);
            TransformResult transformResultForLeaf = leafFacade.transform(inputCoordinateSweref99, crsIdentifierWGS84);
            assertNotNull(transformResultForLeaf);
            assertTrue(transformResultForLeaf.isSuccess());
            assertEqualCoordinate(transformResult.getOutputCoordinate(), transformResultForLeaf.getOutputCoordinate());
            assertEquals(0, transformResultForLeaf.getSubResults().size()); // no subresults for a leaf
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