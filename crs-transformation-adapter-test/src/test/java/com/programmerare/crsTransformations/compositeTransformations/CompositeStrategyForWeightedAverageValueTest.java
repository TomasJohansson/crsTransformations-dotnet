package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA;
import com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools;
import com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL;
import com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS;
import com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J;
import com.programmerare.crsTransformations.coordinate.Coordinate;
import com.programmerare.crsTransformations.TransformResult;
import com.programmerare.crsTransformations.coordinate.CoordinateFactory;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;

import java.util.Arrays;
import java.util.List;

import static org.hamcrest.MatcherAssert.assertThat;
import static org.hamcrest.number.OrderingComparison.greaterThan;
import static org.hamcrest.number.OrderingComparison.lessThan;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

class CompositeStrategyForWeightedAverageValueTest extends CompositeStrategyTestBase {

    private final static double SMALL_DELTA_VALUE = 0.0000000001;

    private static double weightForGeoTools = 40;
    private static double weightForGoober = 30;
    private static double weightForOrbis = 20;
    private static double weightForProj4J = 10;
    private static double weightForGeoPackageNGA = 5;
    // Note : The sum of the weights do NOT have to be 100 (e.g. above it is 105)
    // but the percentage of the weight will become calculated by the implementation

    private static Coordinate coordinateWithExpectedWeightedValues;

    @BeforeAll
    static void before() {
        coordinateWithExpectedWeightedValues = createWeightedValue();
    }

    @Test
    void createCompositeStrategyForWeightedAverageValue() {

        final List<AdapterWeight> weights = Arrays.asList(
            AdapterWeight.createFromInstance(new CrsTransformationAdapterGeoTools(), weightForGeoTools),
            AdapterWeight.createFromInstance(new CrsTransformationAdapterGooberCTL(), weightForGoober),
            AdapterWeight.createFromInstance(new CrsTransformationAdapterOrbisgisCTS(), weightForOrbis),
            AdapterWeight.createFromInstance(new CrsTransformationAdapterProj4J(), weightForProj4J),
            AdapterWeight.createFromInstance(new CrsTransformationAdapterGeoPackageNGA(), weightForGeoPackageNGA)
        );
        final CrsTransformationAdapterComposite adapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationWeightedAverage(weights);
        createCompositeStrategyForWeightedAverageValueHelper(adapter);
    }

    @Test
    void createCompositeStrategyForWeightedAverageValueFromStringsWithReflection() {
        final String classNameGeoTools = CrsTransformationAdapterGeoTools.class.getName() ;
        final String classNameGoober = CrsTransformationAdapterGooberCTL.class.getName() ;
        final String classNameOrbis = CrsTransformationAdapterOrbisgisCTS.class.getName() ;
        final String classNameProj4J = CrsTransformationAdapterProj4J.class.getName() ;
        final String classNameGeoPackageNGA = CrsTransformationAdapterGeoPackageNGA.class.getName() ;

        final List<AdapterWeight> weights = Arrays.asList(
            AdapterWeight.createFromStringWithFullClassNameForImplementation(classNameGeoTools, weightForGeoTools),
            AdapterWeight.createFromStringWithFullClassNameForImplementation(classNameGoober, weightForGoober),
            AdapterWeight.createFromStringWithFullClassNameForImplementation(classNameOrbis, weightForOrbis),
            AdapterWeight.createFromStringWithFullClassNameForImplementation(classNameProj4J, weightForProj4J),
            AdapterWeight.createFromStringWithFullClassNameForImplementation(classNameGeoPackageNGA, weightForGeoPackageNGA)
        );

        final CrsTransformationAdapterComposite weightedAverageCompositeAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationWeightedAverage(weights);
        createCompositeStrategyForWeightedAverageValueHelper(weightedAverageCompositeAdapter);
    }

    private void createCompositeStrategyForWeightedAverageValueHelper(
            CrsTransformationAdapterComposite weightedAverageCompositeAdapter
    ) {
        TransformResult weightedAverageResult = weightedAverageCompositeAdapter.transform(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        assertNotNull(weightedAverageResult);
        assertTrue(weightedAverageResult.isSuccess());
        assertEquals(super.allCoordinateResultsForTheDifferentImplementations.size(), weightedAverageResult.getSubResults().size());

        Coordinate weightedAverageCoordinate = weightedAverageResult.getOutputCoordinate();

        assertEquals(coordinateWithExpectedWeightedValues.getYLatitude(), weightedAverageCoordinate.getYLatitude(), SMALL_DELTA_VALUE);
        assertEquals(coordinateWithExpectedWeightedValues.getXLongitude(), weightedAverageCoordinate.getXLongitude(), SMALL_DELTA_VALUE);

        // The logic for the tests below:
        // The tested result should of course be very close to the expected result,
        // i.e. the differences (longitude and latitude differences)
        // // should be less than a very small SMALL_DELTA_VALUE value
        final double diffLatTestedAdapter = Math.abs(coordinateWithExpectedWeightedValues.getYLatitude() - weightedAverageCoordinate.getYLatitude());
        final double diffLonTestedAdapter = Math.abs(coordinateWithExpectedWeightedValues.getXLongitude() - weightedAverageCoordinate.getXLongitude());
        assertThat(diffLatTestedAdapter, lessThan(SMALL_DELTA_VALUE));// assertTrue(diffLatTestedAdapter < SMALL_DELTA_VALUE);
        assertThat(diffLonTestedAdapter, lessThan(SMALL_DELTA_VALUE));

        // Now in the rest of the assertions below,
        // the difference between the individual results which were weighted
        // should not be quite as close to that same small SMALL_DELTA_VALUE value,
        // and thus the assertions below are that the difference should be greater
        // than the SMALL_DELTA_VALUE value.
        // Of course, in theory some of the individual values below might
        // come very very close to the weighted result, and then some assertion might fail.
        // However, it turned out to not be like that with the chosen test values,
        // and thus they are asserted here as part of regression testing.
        // If this test would break, it needs to be investigated since these values
        // have benn working fine to assert like below.
        assertDiffsAreGreaterThanSmallDelta(resultCoordinateGeoTools, coordinateWithExpectedWeightedValues);
        assertDiffsAreGreaterThanSmallDelta(resultCoordinateGooberCTL, coordinateWithExpectedWeightedValues);
        assertDiffsAreGreaterThanSmallDelta(resultCoordinateOrbisgisCTS, coordinateWithExpectedWeightedValues);
        assertDiffsAreGreaterThanSmallDelta(resultCoordinateProj4J, coordinateWithExpectedWeightedValues);
        assertDiffsAreGreaterThanSmallDelta(resultCoordinateGeoPackageNGA, coordinateWithExpectedWeightedValues);
    }

    private void assertDiffsAreGreaterThanSmallDelta(
        final Coordinate resultCoordinateIndividualImplementation,
        final Coordinate coordinateWithExpectedWeightedValues
    ) {
        final double diffLatIndividualImplementation = Math.abs(
            coordinateWithExpectedWeightedValues.getYLatitude() - resultCoordinateIndividualImplementation.getYLatitude()
        );
        final double diffLonIndividualImplementation = Math.abs(
            coordinateWithExpectedWeightedValues.getXLongitude() - resultCoordinateIndividualImplementation.getXLongitude()
        );
        assertThat(diffLatIndividualImplementation, greaterThan(SMALL_DELTA_VALUE));
        assertThat(diffLonIndividualImplementation, greaterThan(SMALL_DELTA_VALUE));
    }

    private static Coordinate createWeightedValue() {
        final double latitudeWeightedSum =
                weightForGeoTools * resultCoordinateGeoTools.getYLatitude() +
                weightForGoober * resultCoordinateGooberCTL.getYLatitude() +
                weightForOrbis * resultCoordinateOrbisgisCTS.getYLatitude() +
                weightForProj4J * resultCoordinateProj4J.getYLatitude() +
                weightForGeoPackageNGA * resultCoordinateProj4J.getYLatitude();

        final double longitutdeWeightedSum =
                weightForGeoTools * resultCoordinateGeoTools.getXLongitude() +
                weightForGoober * resultCoordinateGooberCTL.getXLongitude() +
                weightForOrbis * resultCoordinateOrbisgisCTS.getXLongitude() +
                weightForProj4J * resultCoordinateProj4J.getXLongitude() +
                weightForGeoPackageNGA * resultCoordinateProj4J.getXLongitude();

        final double totWeights = weightForGeoTools + weightForGoober + weightForOrbis + weightForProj4J + weightForGeoPackageNGA;
        return CoordinateFactory.createFromYLatitudeXLongitude( latitudeWeightedSum/totWeights, longitutdeWeightedSum/totWeights, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
    }
}