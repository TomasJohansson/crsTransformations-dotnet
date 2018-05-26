package com.programmerare.crsTransformations.compositeTransformations;

import static org.hamcrest.MatcherAssert.assertThat;
import static org.hamcrest.number.OrderingComparison.greaterThan;
import static org.hamcrest.number.OrderingComparison.lessThan;

import com.programmerare.crsTransformationFacadeGeoTools.CrsTransformationFacadeGeoTools;
import com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL;
import com.programmerare.crsTransformationFacadeOrbisgisCTS.CrsTransformationFacadeOrbisgisCTS;
import com.programmerare.crsTransformationFacadeProj4J.CrsTransformationFacadeProj4J;
import com.programmerare.crsTransformations.CrsTransformationFacade;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.crsConstants.ConstantEpsgNumber;
import kotlin.Pair;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;
import java.util.Arrays;
import java.util.List;
import static org.junit.jupiter.api.Assertions.assertEquals;

class CompositeStrategyForWeightedAverageValueTest extends CompositeStrategyTestBase {

    private static double weightForGeoTools = 0.4;
    private static double weightForGoober = 0.3;
    private static double weightForOrbis = 0.2;
    private static double weightForProj4J = 0.1;

    private static Coordinate coordinateWithExpectedWeightedValues;

    @BeforeAll
    static void before() {
        coordinateWithExpectedWeightedValues = createWeightedValue();
    }

    @Test
    void createCompositeStrategyForWeightedAverageValue() {
        final List<Pair<CrsTransformationFacade, Double>> weights = Arrays.asList(
            new Pair(new CrsTransformationFacadeGeoTools(), weightForGeoTools),
            new Pair(new CrsTransformationFacadeGooberCTL(), weightForGoober),
            new Pair(new CrsTransformationFacadeOrbisgisCTS(), weightForOrbis),
            new Pair(new CrsTransformationFacadeProj4J(), weightForProj4J)
        );
        final CrsTransformationFacadeComposite facade = CrsTransformationFacadeComposite.createCrsTransformationWeightedAverage(weights);
        createCompositeStrategyForWeightedAverageValueHelper(facade);
    }

    @Test
    void createCompositeStrategyForWeightedAverageValueFromStringsWithReflection() {
        final String classNameGeoTools = CrsTransformationFacadeGeoTools.class.getName() ;
        final String classNameGoober = CrsTransformationFacadeGooberCTL.class.getName() ;
        final String classNameOrbis = CrsTransformationFacadeOrbisgisCTS.class.getName() ;
        final String classNameProj4J = CrsTransformationFacadeProj4J.class.getName() ;

        final List<Pair<String, Double>> weights = Arrays.asList(
            new Pair(classNameGeoTools, weightForGeoTools),
            new Pair(classNameGoober, weightForGoober),
            new Pair(classNameOrbis, weightForOrbis),
            new Pair(classNameProj4J, weightForProj4J)
        );
        final CrsTransformationFacadeComposite facade = CrsTransformationFacadeComposite.createCrsTransformationWeightedAverageByReflection(weights);
        createCompositeStrategyForWeightedAverageValueHelper(facade);
    }

    private void createCompositeStrategyForWeightedAverageValueHelper(CrsTransformationFacadeComposite facade) {
        final Coordinate coordinateResult = facade.transform(wgs84coordinate, ConstantEpsgNumber.SWEREF99TM);

        final double delta = 0.0000000001;
        assertEquals(coordinateWithExpectedWeightedValues.getYLatitude(), coordinateResult.getYLatitude(), delta);
        assertEquals(coordinateWithExpectedWeightedValues.getXLongitude(), coordinateResult.getXLongitude(), delta);

        // The logic for the tests below:
        // The tested result should of course be very close to the expected result,
        // i.e. the differences (longitude and latitude differences)
        // // should be less than a very small delta value
        final double diffLatTestedFacade = Math.abs(coordinateWithExpectedWeightedValues.getYLatitude() - coordinateResult.getYLatitude());
        final double diffLonTestedFacade = Math.abs(coordinateWithExpectedWeightedValues.getXLongitude() - coordinateResult.getXLongitude());
        assertThat(diffLatTestedFacade, lessThan(delta));// assertTrue(diffLatTestedFacade < delta);
        assertThat(diffLonTestedFacade, lessThan(delta));

        // Now in the rest of the assertions below,
        // the difference between the individual results which were weighted
        // should not be quite as close to that same small delta value,
        // and thus the assertions below are that the difference should be greater
        // than the delta value.
        // Of course, in theory some of the individual values below might
        // come very very close to the weighted result, and then some assertion might fail.
        // However, it turned out to not be like that with the chosen test values,
        // and thus they are asserted here as part of regression testing.
        // If this test would break, it needs to be investigated since these values
        // have benn working fine to assert like below.
        final double diffLatGeoTools = Math.abs(coordinateWithExpectedWeightedValues.getYLatitude() - resultCoordinateGeoTools.getYLatitude());
        final double diffLonGeoTools = Math.abs(coordinateWithExpectedWeightedValues.getXLongitude() - resultCoordinateGeoTools.getXLongitude());
        assertThat(diffLatGeoTools, greaterThan(delta)); // assertTrue(diffLatGeoTools > delta);
        assertThat(diffLonGeoTools, greaterThan(delta));

        final double diffLatGoober = Math.abs(coordinateWithExpectedWeightedValues.getYLatitude() - resultCoordinateGooberCTL.getYLatitude());
        final double diffLonGoober = Math.abs(coordinateWithExpectedWeightedValues.getXLongitude() - resultCoordinateGooberCTL.getXLongitude());
        assertThat(diffLatGoober, greaterThan(delta));
        assertThat(diffLonGoober , greaterThan(delta));

        final double diffLatOrbis = Math.abs(coordinateWithExpectedWeightedValues.getYLatitude() - resultCoordinateOrbisgisCTS.getYLatitude());
        final double diffLonOrbis = Math.abs(coordinateWithExpectedWeightedValues.getXLongitude() - resultCoordinateOrbisgisCTS.getXLongitude());
        assertThat(diffLatOrbis, greaterThan(delta));
        assertThat(diffLonOrbis, greaterThan(delta));

        final double diffLatProj4J = Math.abs(coordinateWithExpectedWeightedValues.getYLatitude() - resultCoordinateProj4J.getYLatitude());
        final double diffLonProj4J = Math.abs(coordinateWithExpectedWeightedValues.getXLongitude() - resultCoordinateProj4J.getXLongitude());
        assertThat(diffLatProj4J, greaterThan(delta));
        assertThat(diffLonProj4J, greaterThan(delta));
    }

    private static Coordinate createWeightedValue() {
        final double latitudeWeightedSum =
                weightForGeoTools * resultCoordinateGeoTools.getYLatitude() +
                weightForGoober * resultCoordinateGooberCTL.getYLatitude() +
                weightForOrbis * resultCoordinateOrbisgisCTS.getYLatitude() +
                weightForProj4J* resultCoordinateProj4J.getYLatitude();

        final double longitutdeWeightedSum =
                weightForGeoTools * resultCoordinateGeoTools.getXLongitude() +
                weightForGoober * resultCoordinateGooberCTL.getXLongitude() +
                weightForOrbis * resultCoordinateOrbisgisCTS.getXLongitude() +
                weightForProj4J* resultCoordinateProj4J.getXLongitude();

        final double totWeights = weightForGeoTools + weightForGoober + weightForOrbis + weightForProj4J;
        return Coordinate.createFromYLatXLong( latitudeWeightedSum/totWeights, longitutdeWeightedSum/totWeights, ConstantEpsgNumber.SWEREF99TM);
    }
}