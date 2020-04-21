using System;
using System.Collections.Generic;
using NUnit.Framework;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_8_9;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using Programmerare.CrsTransformations.CompositeTransformations;

namespace Programmerare.CrsTransformations.Core.CompositeTransformations {

[TestFixture]
public class CompositeStrategyWeightedAverageTest : CompositeStrategyTestBase {

    private const double SMALL_DELTA_VALUE = 0.0000000001;

    private const double weightForDotSpatial = 40;
    private const double weightForProjNet = 30;
    private const double weightForMightyLittleGeodesy = 20;
    // Note : The sum of the weights do NOT have to be 100 (e.g. above it is 90)
    // but the percentage of the weight will become calculated by the implementation

    private CrsCoordinate coordinateWithExpectedWeightedValues;

    private CrsTransformationAdapterWeightFactory weightFactory;

    [SetUp]
    public void SetUp() {
        coordinateWithExpectedWeightedValues = CreateWeightedValue();
        weightFactory = CrsTransformationAdapterWeightFactory.Create();
    }
    

    [Test]
    public void Transform_ShouldReturnWeightedAverageResult_WhenUsingWeightedAverageCompositeAdapter() {
        
        var weights = new List<CrsTransformationAdapterWeight>{
            weightFactory.CreateFromInstance(new CrsTransformationAdapterDotSpatial(), weightForDotSpatial),
            weightFactory.CreateFromInstance(new CrsTransformationAdapterProjNet(), weightForProjNet),
            weightFactory.CreateFromInstance(new CrsTransformationAdapterMightyLittleGeodesy(), weightForMightyLittleGeodesy)
        };
        CrsTransformationAdapterComposite adapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationWeightedAverage(weights);
        AssertWeightedAverageResult(adapter);
    }

    [Test]
    public void Transform_ShouldReturnWeightedAverageResult_WhenUsingWeightedAverageCompositeAdapterAndLeafsInstantiatedFromStringsWithClassNames() {
            
        string classNameadapterDotSpatial = adapterDotSpatial.LongNameOfImplementation;
        string classNameProjNet = adapterProjNet.LongNameOfImplementation;
        string classNameMightyLittleGeodesy = adapterMightyLittleGeodesy.LongNameOfImplementation;


        List<CrsTransformationAdapterWeight> weights = new List<CrsTransformationAdapterWeight>{
            weightFactory.CreateFromStringWithFullClassNameForImplementation(classNameadapterDotSpatial, weightForDotSpatial),
            weightFactory.CreateFromStringWithFullClassNameForImplementation(classNameProjNet, weightForProjNet),
            weightFactory.CreateFromStringWithFullClassNameForImplementation(classNameMightyLittleGeodesy, weightForMightyLittleGeodesy)
        };


        CrsTransformationAdapterComposite weightedAverageCompositeAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationWeightedAverage(weights);
        AssertWeightedAverageResult(weightedAverageCompositeAdapter);
    }

    private void AssertWeightedAverageResult(
        CrsTransformationAdapterComposite weightedAverageCompositeAdapter
    ) {
        CrsTransformationResult weightedAverageResult = weightedAverageCompositeAdapter.Transform(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        Assert.IsNotNull(weightedAverageResult);
        Assert.IsTrue(weightedAverageResult.IsSuccess);
        Assert.AreEqual(base.allCoordinateResultsForTheDifferentImplementations.Count, weightedAverageResult.TransformationResultChildren.Count);

        CrsCoordinate weightedAverageCoordinate = weightedAverageResult.OutputCoordinate;

        Assert.AreEqual(coordinateWithExpectedWeightedValues.YNorthingLatitude, weightedAverageCoordinate.YNorthingLatitude, SMALL_DELTA_VALUE);
        Assert.AreEqual(coordinateWithExpectedWeightedValues.XEastingLongitude, weightedAverageCoordinate.XEastingLongitude, SMALL_DELTA_VALUE);

        // The logic for the tests below:
        // The tested result should of course be very close to the expected result,
        // i.e. the differences (longitude and latitude differences)
        // // should be less than a very small SMALL_DELTA_VALUE value
        double diffLatTestedAdapter = Math.Abs(coordinateWithExpectedWeightedValues.YNorthingLatitude - weightedAverageCoordinate.YNorthingLatitude);
        double diffLonTestedAdapter = Math.Abs(coordinateWithExpectedWeightedValues.XEastingLongitude - weightedAverageCoordinate.XEastingLongitude);
        Assert.That(diffLatTestedAdapter, Is.LessThan(SMALL_DELTA_VALUE));// assertTrue(diffLatTestedAdapter < SMALL_DELTA_VALUE);
        Assert.That(diffLonTestedAdapter, Is.LessThan(SMALL_DELTA_VALUE));

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
        // have been working fine to assert like below.
        AssertDiffsAreGreaterThanSmallDelta(resultCoordinateDotSpatial, coordinateWithExpectedWeightedValues);
        AssertDiffsAreGreaterThanSmallDelta(resultCoordinateMightyLittleGeodesy, coordinateWithExpectedWeightedValues);
        AssertDiffsAreGreaterThanSmallDelta(resultCoordinateProjNet, coordinateWithExpectedWeightedValues);
    }

    private void AssertDiffsAreGreaterThanSmallDelta(
        CrsCoordinate resultCoordinateIndividualImplementation,
        CrsCoordinate coordinateWithExpectedWeightedValues
    ) {
        double diffLatIndividualImplementation = Math.Abs(
            coordinateWithExpectedWeightedValues.YNorthingLatitude - resultCoordinateIndividualImplementation.YNorthingLatitude
        );
        double diffLonIndividualImplementation = Math.Abs(
            coordinateWithExpectedWeightedValues.XEastingLongitude - resultCoordinateIndividualImplementation.XEastingLongitude
        );
        Assert.That(diffLatIndividualImplementation, Is.GreaterThan(SMALL_DELTA_VALUE));
        Assert.That(diffLonIndividualImplementation, Is.GreaterThan(SMALL_DELTA_VALUE));
    }

    private CrsCoordinate CreateWeightedValue() {
        double latitudeWeightedSum =
            weightForDotSpatial * resultCoordinateDotSpatial.YNorthingLatitude +
            weightForMightyLittleGeodesy * resultCoordinateMightyLittleGeodesy.YNorthingLatitude +
            weightForProjNet * resultCoordinateProjNet.YNorthingLatitude;

        double longitutdeWeightedSum =
            weightForDotSpatial * resultCoordinateDotSpatial.XEastingLongitude +
            weightForMightyLittleGeodesy * resultCoordinateMightyLittleGeodesy.XEastingLongitude +
            weightForProjNet * resultCoordinateProjNet.XEastingLongitude;

        double totWeights = weightForDotSpatial + weightForMightyLittleGeodesy + weightForProjNet;
        return CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude( latitudeWeightedSum/totWeights, longitutdeWeightedSum/totWeights, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
    }


    [Test]
    public void CreateCompositeStrategyWeightedAverage_whenAllWeightsArePositive__shouldNotThrowException() {
        List<CrsTransformationAdapterWeight> weightedCrsTransformationAdapters =
            new List<CrsTransformationAdapterWeight>{
                weightFactory.CreateFromInstance(
                    new CrsTransformationAdapterMightyLittleGeodesy(),
                    1 // null is not possible (compiling error) which is good !
                )
            };
        CompositeStrategyWeightedAverage compositeStrategyWeightedAverage =
                CompositeStrategyWeightedAverage._CreateCompositeStrategyWeightedAverage(weightedCrsTransformationAdapters);
        // (the above method is "internal" in the F# project but still available from here 
        //  because of "InternalsVisibleTo" configuration in the .fsproj file)
        // The main test of this test method is that the above create method does not throw an exception
        Assert.IsNotNull(compositeStrategyWeightedAverage);
    }

    [Test]
    public void CalculateAggregatedResult_ShouldThrowException_WhenResultIsBasedOnLeafsNotBeingPartOfTheWeightedAverageAdapter() {
        CrsCoordinate coordinate = CrsCoordinateFactory.LatLon(59, 18);
        List<CrsTransformationResult> emptyListOfTransformationResults = new List<CrsTransformationResult>();
            
        ICrsTransformationAdapter leafMightyLittleGeodesy = new CrsTransformationAdapterMightyLittleGeodesy();
        List<CrsTransformationAdapterWeight> leafWeightsForOnlyMightyLittleGeodesy = new List<CrsTransformationAdapterWeight>{
            weightFactory.CreateFromInstance(
                leafMightyLittleGeodesy,
                1
            )
        };
        // The below type ICompositeStrategy is "internal" in the F# project but still available from here 
        //  because of "InternalsVisibleTo" configuration in the .fsproj file.
        ICompositeStrategy compositeStrategyWeightedAverageForOnlyMightyLittleGeodesy = CompositeStrategyWeightedAverage._CreateCompositeStrategyWeightedAverage(leafWeightsForOnlyMightyLittleGeodesy);
        // The above composite was created with only one leaf in the list

        ICrsTransformationAdapter leafDotSpatial = new CrsTransformationAdapterDotSpatial();
        CrsTransformationResult crsTransformationResultProblem = new CrsTransformationResult(
            coordinate, // inputCoordinate irrelevant in this test so okay to use the same as the output
            coordinate, // outputCoordinate
            null, // exception
            true, // isSuccess
            leafDotSpatial, // crsTransformationAdapterResultSource,
            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(emptyListOfTransformationResults)
        );

        // The composite strategy used below was created with only MightyLittleGeodesy,
        // and therefore if the result (as below) would be based on "DotSpatial" 
        // then there is a bug somewhere i.e. an exception is thrown 
        // which is tested below
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>( () => {
            compositeStrategyWeightedAverageForOnlyMightyLittleGeodesy._CalculateAggregatedResult(
                    new List<CrsTransformationResult>{crsTransformationResultProblem}, // allResults
                    coordinate,
                    coordinate.CrsIdentifier, //  crsIdentifier for OutputCoordinateSystem
                    leafDotSpatial // SHOULD CAUSE EXCEPTION !
                );            
            },
            "The result adapter was not part of the weighted average composite adapter"
        );
    }

    [Test]
    public void WeightedAverageAdapter_ShouldBeEqual_WhenHavingTheSameLeafAdaptersRegardlessOfTheOrder() {
        ICrsTransformationAdapter weightedAverage1, weightedAverage2, weightedAverage3;

        var weights1 = new List<CrsTransformationAdapterWeight>{
            weightFactory.CreateFromInstance(new CrsTransformationAdapterDotSpatial(), weightForDotSpatial),
            weightFactory.CreateFromInstance(new CrsTransformationAdapterProjNet(), weightForProjNet),
            weightFactory.CreateFromInstance(new CrsTransformationAdapterMightyLittleGeodesy(), weightForMightyLittleGeodesy)
        };
        weightedAverage1 = crsTransformationAdapterCompositeFactory.CreateCrsTransformationWeightedAverage(weights1);
        // The below weights are exactly the same as above
        // but in different order (the first two are reversed), but the order is not relevant 
        // and thus they should be considered as Equal
        var weights2 = new List<CrsTransformationAdapterWeight>{
            weightFactory.CreateFromInstance(new CrsTransformationAdapterProjNet(), weightForProjNet),
            weightFactory.CreateFromInstance(new CrsTransformationAdapterDotSpatial(), weightForDotSpatial),
            weightFactory.CreateFromInstance(new CrsTransformationAdapterMightyLittleGeodesy(), weightForMightyLittleGeodesy)
        };
        weightedAverage2 = crsTransformationAdapterCompositeFactory.CreateCrsTransformationWeightedAverage(weights2);

        Assert.AreEqual(weightedAverage1, weightedAverage2);
        Assert.AreEqual(weightedAverage1.GetHashCode(), weightedAverage2.GetHashCode());

        // Now below creating a new instance "weightedAverage3" with one of the 
        // modified compared to above "weightedAverage2" and thus they should NOT be considered Equal
        var weights3 = new List<CrsTransformationAdapterWeight>{
            weightFactory.CreateFromInstance(new CrsTransformationAdapterProjNet(), weightForProjNet + 0.01),
            weightFactory.CreateFromInstance(new CrsTransformationAdapterDotSpatial(), weightForDotSpatial),
            weightFactory.CreateFromInstance(new CrsTransformationAdapterMightyLittleGeodesy(), weightForMightyLittleGeodesy)
        };
        weightedAverage3 = crsTransformationAdapterCompositeFactory.CreateCrsTransformationWeightedAverage(weights3);
        Assert.AreNotEqual(weightedAverage2, weightedAverage3);
    }

}
}