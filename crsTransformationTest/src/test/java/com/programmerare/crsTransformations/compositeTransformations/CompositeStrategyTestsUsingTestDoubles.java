package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.*;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;

import java.util.Arrays;
import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

public class CompositeStrategyTestsUsingTestDoubles {

    private static double medianLatitude, averageLatitude, medianLongitude, averageLongitude;
    private static Coordinate inputCoordinateSweref99;
    private static Coordinate outputCoordinateWgs84ForImplementation_1, outputCoordinateWgs84ForImplementation_2, outputCoordinateWgs84ForImplementation_3, outputCoordinateWgs84ForImplementation_4, outputCoordinateWgs84ForImplementation_5;
    private static CrsTransformationFacade facadeImplementation_1, facadeImplementation_2, facadeImplementation_3, facadeImplementation_4, facadeImplementation_5;
    private static List<CrsTransformationFacade> allFacades;

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

        outputCoordinateWgs84ForImplementation_1 = Coordinate.createFromLatLong(outputLatitudes[0],outputLongitudes[3]);
        outputCoordinateWgs84ForImplementation_2 = Coordinate.createFromLatLong(outputLatitudes[2],outputLongitudes[1]);
        outputCoordinateWgs84ForImplementation_3 = Coordinate.createFromLatLong(outputLatitudes[4],outputLongitudes[4]);
        outputCoordinateWgs84ForImplementation_4 = Coordinate.createFromLatLong(outputLatitudes[1],outputLongitudes[0]);
        outputCoordinateWgs84ForImplementation_5 = Coordinate.createFromLatLong(outputLatitudes[3],outputLongitudes[2]);

        facadeImplementation_1 = mock(CrsTransformationFacade.class);
        facadeImplementation_2 = mock(CrsTransformationFacade.class);
        facadeImplementation_3 = mock(CrsTransformationFacade.class);
        facadeImplementation_4 = mock(CrsTransformationFacade.class);
        facadeImplementation_5 = mock(CrsTransformationFacade.class);

        inputCoordinateSweref99 = Coordinate.createFromYLatXLong(6580822, 674032, EpsgNumber._3006__SWEREF99_TM__SWEDEN);

        TransformResult r1 = new TransformResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_1,
            null,
            true,
            facadeImplementation_1
        );
        TransformResult r2 = new TransformResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_2,
            null,
            true,
            facadeImplementation_2
        );
        TransformResult r3 = new TransformResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_3,
            null,
            true,
            facadeImplementation_3
        );
        TransformResult r4 = new TransformResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_4,
            null,
            true,
            facadeImplementation_4
        );
        TransformResult r5 = new TransformResultImplementation(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_5,
            null,
            true,
            facadeImplementation_5
        );
        CrsIdentifier crsIdentifierWGS84 = CrsIdentifier.createFromEpsgNumber(EpsgNumber._4326__WGS_84__WORLD);

        when(facadeImplementation_1.transformToResultObject(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(r1);
        when(facadeImplementation_2.transformToResultObject(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(r2);
        when(facadeImplementation_3.transformToResultObject(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(r3);
        when(facadeImplementation_4.transformToResultObject(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(r4);
        when(facadeImplementation_5.transformToResultObject(inputCoordinateSweref99, crsIdentifierWGS84)).thenReturn(r5);

        allFacades = Arrays.asList(
            facadeImplementation_1,
            facadeImplementation_2,
            facadeImplementation_3,
            facadeImplementation_4,
            facadeImplementation_5
        );
    }

    private final static double SMALL_DELTA_VALUE_FOR_COMPARISONS = 0.00000000000001;

    @Test
    void averageFacadeTest() {
        CrsTransformationFacade averageFacade = CrsTransformationFacadeComposite.createCrsTransformationAverage(allFacades);
        Coordinate result = averageFacade.transform(inputCoordinateSweref99, EpsgNumber._4326__WGS_84__WORLD);
        assertNotNull(result);

        // Coordinate expectedAverageCoordinate = Coordinate.createFromLatLong(averageLatitude, averageLongitude);
        // assertEquals(expectedAverageCoordinate, result); // this failed because latitude was 59.31999999999999 instead of 59.32
        assertEquals(averageLatitude,  result.getYLatitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
        assertEquals(averageLongitude, result.getXLongitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
    }

    @Test
    void medianFacadeTest() {
        CrsTransformationFacade averageFacade = CrsTransformationFacadeComposite.createCrsTransformationMedian(allFacades);
        Coordinate result = averageFacade.transform(inputCoordinateSweref99, EpsgNumber._4326__WGS_84__WORLD);
        assertNotNull(result);

        assertEquals(medianLatitude,  result.getYLatitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
        assertEquals(medianLongitude, result.getXLongitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
    }

    @Test
    void ChainOfResponsibilityFacadeTest() {
        CrsTransformationFacade averageFacade = CrsTransformationFacadeComposite.createCrsTransformationChainOfResponsibility(allFacades);
        Coordinate result = averageFacade.transform(inputCoordinateSweref99, EpsgNumber._4326__WGS_84__WORLD);
        assertNotNull(result);

        // The assumption below (according to the setup code in the "before" method in this JUnit class)
        // is that the first facade in the above list allFacades will return the result outputCoordinateWgs84ForImplementation_1
        assertEquals(outputCoordinateWgs84ForImplementation_1.getYLatitude(),  result.getYLatitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
        assertEquals(outputCoordinateWgs84ForImplementation_1.getXLongitude(), result.getXLongitude(), SMALL_DELTA_VALUE_FOR_COMPARISONS);
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