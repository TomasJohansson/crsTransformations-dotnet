package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA;
import com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools;
import com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL;
import com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS;
import com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J;
import com.programmerare.crsTransformations.CrsTransformationAdapter;
import com.programmerare.crsTransformations.CrsTransformationResult;
import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsTransformations.CrsTransformationResult;
import com.programmerare.crsTransformations.coordinate.CrsCoordinateFactory;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static org.hamcrest.MatcherAssert.assertThat;
import static org.hamcrest.number.OrderingComparison.greaterThan;
import static org.hamcrest.number.OrderingComparison.lessThan;
import static org.junit.jupiter.api.Assertions.*;
import static org.junit.jupiter.api.Assertions.assertNotNull;

class CompositeStrategyForWeightedAverageValueTest extends CompositeStrategyTestBase {

    private final static double SMALL_DELTA_VALUE = 0.0000000001;

    private static double weightForGeoTools = 40;
    private static double weightForGoober = 30;
    private static double weightForOrbis = 20;
    private static double weightForProj4J = 10;
    private static double weightForGeoPackageNGA = 5;
    // Note : The sum of the weights do NOT have to be 100 (e.g. above it is 105)
    // but the percentage of the weight will become calculated by the implementation

    private static CrsCoordinate coordinateWithExpectedWeightedValues;

    @BeforeAll
    static void before() {
        coordinateWithExpectedWeightedValues = createWeightedValue();
    }

    @Test
    void createCompositeStrategyForWeightedAverageValue() {

        final List<CrsTransformationAdapterWeight> weights = Arrays.asList(
            CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterGeoTools(), weightForGeoTools),
            CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterGooberCTL(), weightForGoober),
            CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterOrbisgisCTS(), weightForOrbis),
            CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterProj4J(), weightForProj4J),
            CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterGeoPackageNGA(), weightForGeoPackageNGA)
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

        final List<CrsTransformationAdapterWeight> weights = Arrays.asList(
            CrsTransformationAdapterWeight.createFromStringWithFullClassNameForImplementation(classNameGeoTools, weightForGeoTools),
            CrsTransformationAdapterWeight.createFromStringWithFullClassNameForImplementation(classNameGoober, weightForGoober),
            CrsTransformationAdapterWeight.createFromStringWithFullClassNameForImplementation(classNameOrbis, weightForOrbis),
            CrsTransformationAdapterWeight.createFromStringWithFullClassNameForImplementation(classNameProj4J, weightForProj4J),
            CrsTransformationAdapterWeight.createFromStringWithFullClassNameForImplementation(classNameGeoPackageNGA, weightForGeoPackageNGA)
        );

        final CrsTransformationAdapterComposite weightedAverageCompositeAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationWeightedAverage(weights);
        createCompositeStrategyForWeightedAverageValueHelper(weightedAverageCompositeAdapter);
    }

    private void createCompositeStrategyForWeightedAverageValueHelper(
            CrsTransformationAdapterComposite weightedAverageCompositeAdapter
    ) {
        CrsTransformationResult weightedAverageResult = weightedAverageCompositeAdapter.transform(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        assertNotNull(weightedAverageResult);
        assertTrue(weightedAverageResult.isSuccess());
        assertEquals(super.allCoordinateResultsForTheDifferentImplementations.size(), weightedAverageResult.getTransformationResultChildren().size());

        CrsCoordinate weightedAverageCoordinate = weightedAverageResult.getOutputCoordinate();

        assertEquals(coordinateWithExpectedWeightedValues.getYNorthingLatitude(), weightedAverageCoordinate.getYNorthingLatitude(), SMALL_DELTA_VALUE);
        assertEquals(coordinateWithExpectedWeightedValues.getXEastingLongitude(), weightedAverageCoordinate.getXEastingLongitude(), SMALL_DELTA_VALUE);

        // The logic for the tests below:
        // The tested result should of course be very close to the expected result,
        // i.e. the differences (longitude and latitude differences)
        // // should be less than a very small SMALL_DELTA_VALUE value
        final double diffLatTestedAdapter = Math.abs(coordinateWithExpectedWeightedValues.getYNorthingLatitude() - weightedAverageCoordinate.getYNorthingLatitude());
        final double diffLonTestedAdapter = Math.abs(coordinateWithExpectedWeightedValues.getXEastingLongitude() - weightedAverageCoordinate.getXEastingLongitude());
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
        final CrsCoordinate resultCoordinateIndividualImplementation,
        final CrsCoordinate coordinateWithExpectedWeightedValues
    ) {
        final double diffLatIndividualImplementation = Math.abs(
            coordinateWithExpectedWeightedValues.getYNorthingLatitude() - resultCoordinateIndividualImplementation.getYNorthingLatitude()
        );
        final double diffLonIndividualImplementation = Math.abs(
            coordinateWithExpectedWeightedValues.getXEastingLongitude() - resultCoordinateIndividualImplementation.getXEastingLongitude()
        );
        assertThat(diffLatIndividualImplementation, greaterThan(SMALL_DELTA_VALUE));
        assertThat(diffLonIndividualImplementation, greaterThan(SMALL_DELTA_VALUE));
    }

    private static CrsCoordinate createWeightedValue() {
        final double latitudeWeightedSum =
                weightForGeoTools * resultCoordinateGeoTools.getYNorthingLatitude() +
                weightForGoober * resultCoordinateGooberCTL.getYNorthingLatitude() +
                weightForOrbis * resultCoordinateOrbisgisCTS.getYNorthingLatitude() +
                weightForProj4J * resultCoordinateProj4J.getYNorthingLatitude() +
                weightForGeoPackageNGA * resultCoordinateProj4J.getYNorthingLatitude();

        final double longitutdeWeightedSum =
                weightForGeoTools * resultCoordinateGeoTools.getXEastingLongitude() +
                weightForGoober * resultCoordinateGooberCTL.getXEastingLongitude() +
                weightForOrbis * resultCoordinateOrbisgisCTS.getXEastingLongitude() +
                weightForProj4J * resultCoordinateProj4J.getXEastingLongitude() +
                weightForGeoPackageNGA * resultCoordinateProj4J.getXEastingLongitude();

        final double totWeights = weightForGeoTools + weightForGoober + weightForOrbis + weightForProj4J + weightForGeoPackageNGA;
        return CrsCoordinateFactory.createFromYNorthingLatitudeAndXEastingLongitude( latitudeWeightedSum/totWeights, longitutdeWeightedSum/totWeights, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
    }

    /*
    // The below methods is currently disabled since the strategy was made more hidden
    // i.e. more difficult to instantiate and thus also more difficult from test code ...
    @Test
    void createCompositeStrategyForWeightedAverageValue_whenAllWeightsArePositive__shouldNotThrowException() {
        final List<CrsTransformationAdapterWeight> weightedCrsTransformationAdapters =
            Arrays.asList(
                CrsTransformationAdapterWeight.createFromInstance(
                    new CrsTransformationAdapterGeoPackageNGA(),
                    1 // null is not possible (compiling error) which is good !
                )
            );
        CompositeStrategyForWeightedAverageValue compositeStrategyForWeightedAverageValue =
                CompositeStrategyForWeightedAverageValue._createCompositeStrategyForWeightedAverageValue(weightedCrsTransformationAdapters);
        // The main test of this test method is that the above create method does not throw an exception
        assertNotNull(compositeStrategyForWeightedAverageValue);
    }

    @Test    
    void calculateAggregatedResultTest() {
        // TODO refactor this too long test method
        final CrsTransformationAdapter crsTransformationAdapterResultSource = new CrsTransformationAdapterGeoPackageNGA();
        final List<CrsTransformationAdapterWeight> crsTransformationAdapterWeights = Arrays.asList(
            CrsTransformationAdapterWeight.createFromInstance(
                crsTransformationAdapterResultSource,
                1
            )
        );

        final CrsCoordinate coordinate = CrsCoordinateFactory.latLon(59,18);
        final CrsTransformationResult crsTransformationResult = new CrsTransformationResult(
            coordinate, // inputCoordinate irrelevant in this test so okay to use the same as the output
            coordinate, // outputCoordinate
            null, // exception
            true, // isSuccess
            crsTransformationAdapterResultSource,
            new ArrayList<CrsTransformationResult>(), // transformationResultChildren
            null // _nullableCrsTransformationResultStatistic
        );

        final CompositeStrategy compositeStrategyForWeightedAverageValue = CompositeStrategyForWeightedAverageValue._createCompositeStrategyForWeightedAverageValue(crsTransformationAdapterWeights);
        // 
        // the above composite was created with only one leaf in the list 
        // i.e. the object crsTransformationAdapterResultSource which is also used below    
                
        CrsTransformationResult crsTransformationResult1 = compositeStrategyForWeightedAverageValue.calculateAggregatedResult(
            Arrays.asList(crsTransformationResult), // allResults
            coordinate,
            coordinate.getCrsIdentifier(), //  crsIdentifierForOutputCoordinateSystem
            crsTransformationAdapterResultSource
        );
        assertNotNull(crsTransformationResult1);
        assertTrue(crsTransformationResult1.isSuccess());
        assertEquals(coordinate, crsTransformationResult1.getOutputCoordinate());

        final CrsTransformationAdapter crsTransformationAdapterNotInTheComposite = new CrsTransformationAdapterGooberCTL();
        final CrsTransformationResult crsTransformationResultProblem = new CrsTransformationResult(
            coordinate, // inputCoordinate irrelevant in this test so okay to use the same as the output
            coordinate, // outputCoordinate
            null, // exception
            true, // isSuccess
            crsTransformationAdapterNotInTheComposite, // crsTransformationAdapterResultSource,
            new ArrayList<CrsTransformationResult>(), // transformationResultChildren
            null // _nullableCrsTransformationResultStatistic
        );
        
        assertThrows(
            RuntimeException.class,
            () -> {
                compositeStrategyForWeightedAverageValue.calculateAggregatedResult(
                    Arrays.asList(crsTransformationResultProblem), // allResults
                    coordinate,
                    coordinate.getCrsIdentifier(), //  crsIdentifierForOutputCoordinateSystem
                    crsTransformationAdapterNotInTheComposite // SHOULD CAUSE EXCEPTION !
                );
            },
            "The result adapter was not part of the weighted average composite adapter"
        );        

    }
    */
}