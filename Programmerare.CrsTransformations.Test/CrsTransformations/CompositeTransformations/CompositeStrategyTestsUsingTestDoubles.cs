using System.Collections.Generic;
using NUnit.Framework;
using Moq; // https://github.com/Moq/moq4
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.Identifier;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;

namespace Programmerare.CrsTransformations.CompositeTransformations 
{
public class CompositeStrategyTestsUsingTestDoubles {

    private double expectedMedianLatitude, expectedAverageLatitude, expectedMedianLongitude, expectedAverageLongitude;
    private CrsCoordinate inputCoordinateSweref99;
    private CrsCoordinate outputCoordinateWgs84ForImplementation_1, outputCoordinateWgs84ForImplementation_2, outputCoordinateWgs84ForImplementation_3, outputCoordinateWgs84ForImplementation_4, outputCoordinateWgs84ForImplementation_5;
    private List<CrsCoordinate> outputCoordinates;
    private ICrsTransformationAdapter leafAdapterImplementation_1, leafAdapterImplementation_2, leafAdapterImplementation_3, leafAdapterImplementation_4, leafAdapterImplementation_5;
    private IList<ICrsTransformationAdapter> allLeafAdapters;

    private CrsIdentifier crsIdentifierWGS84;

    private Mock<ICrsTransformationAdapter> mock1, mock2, mock3, mock4, mock5;

    private CrsTransformationAdapterCompositeFactory crsTransformationAdapterCompositeFactory;

    [SetUp]
    public void SetUp() {
        crsTransformationAdapterCompositeFactory = CrsTransformationAdapterCompositeFactory.Create();
        double[] outputLatitudes = {
            59.1,
            59.2,
            59.3,
            59.4,
            59.6,
        };
        expectedMedianLatitude = 59.3;
        expectedAverageLatitude = 59.32;

        double[] outputLongitudes = {
            18.2,
            18.3,
            18.4,
            18.8,
            18.9
        };
        expectedMedianLongitude = 18.4;
        expectedAverageLongitude = 18.52;

        outputCoordinateWgs84ForImplementation_1 = CrsCoordinateFactory.CreateFromLatitudeLongitude(outputLatitudes[0],outputLongitudes[3]);
        outputCoordinateWgs84ForImplementation_2 = CrsCoordinateFactory.CreateFromLatitudeLongitude(outputLatitudes[2],outputLongitudes[1]);
        outputCoordinateWgs84ForImplementation_3 = CrsCoordinateFactory.CreateFromLatitudeLongitude(outputLatitudes[4],outputLongitudes[4]);
        outputCoordinateWgs84ForImplementation_4 = CrsCoordinateFactory.CreateFromLatitudeLongitude(outputLatitudes[1],outputLongitudes[0]);
        outputCoordinateWgs84ForImplementation_5 = CrsCoordinateFactory.CreateFromLatitudeLongitude(outputLatitudes[3],outputLongitudes[2]);
        outputCoordinates = new List<CrsCoordinate>{ 
            outputCoordinateWgs84ForImplementation_1, 
            outputCoordinateWgs84ForImplementation_2, 
            outputCoordinateWgs84ForImplementation_3, 
            outputCoordinateWgs84ForImplementation_4, 
            outputCoordinateWgs84ForImplementation_5
        };
        
        mock1 = new Mock<ICrsTransformationAdapter>();
        mock2 = new Mock<ICrsTransformationAdapter>();
        mock3 = new Mock<ICrsTransformationAdapter>();
        mock4 = new Mock<ICrsTransformationAdapter>();
        mock5 = new Mock<ICrsTransformationAdapter>();

        leafAdapterImplementation_1 = mock1.Object;
        leafAdapterImplementation_2 = mock2.Object;
        leafAdapterImplementation_3 = mock3.Object;
        leafAdapterImplementation_4 = mock4.Object;
        leafAdapterImplementation_5 = mock5.Object;

        inputCoordinateSweref99 = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(6580822.0, 674032.0, EpsgNumber.SWEDEN__SWEREF99_TM__3006);

        CrsTransformationResult leafResult1 = CrsTransformationResult._CreateCrsTransformationResult(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_1,
            null,
            true,
            leafAdapterImplementation_1,
            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(new List<CrsTransformationResult>())
        );
        CrsTransformationResult leafResult2 = CrsTransformationResult._CreateCrsTransformationResult(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_2,
            null,
            true,
            leafAdapterImplementation_2,
            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(new List<CrsTransformationResult>())
        );
        CrsTransformationResult leafResult3 = CrsTransformationResult._CreateCrsTransformationResult(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_3,
            null,
            true,
            leafAdapterImplementation_3,
            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(new List<CrsTransformationResult>())
        );
        CrsTransformationResult leafResult4 = CrsTransformationResult._CreateCrsTransformationResult(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_4,
            null,
            true,
            leafAdapterImplementation_4,
            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(new List<CrsTransformationResult>())
        );
        CrsTransformationResult leafResult5 = CrsTransformationResult._CreateCrsTransformationResult(
            inputCoordinateSweref99,
            outputCoordinateWgs84ForImplementation_5,
            null,
            true,
            leafAdapterImplementation_5,
            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(new List<CrsTransformationResult>())
        );
        crsIdentifierWGS84 = CrsIdentifierFactory.CreateFromEpsgNumber(EpsgNumber.WORLD__WGS_84__4326);

        mock1.Setup(leaf => leaf.Transform(inputCoordinateSweref99, crsIdentifierWGS84)).Returns(leafResult1);
        mock2.Setup(leaf => leaf.Transform(inputCoordinateSweref99, crsIdentifierWGS84)).Returns(leafResult2);
        mock3.Setup(leaf => leaf.Transform(inputCoordinateSweref99, crsIdentifierWGS84)).Returns(leafResult3);
        mock4.Setup(leaf => leaf.Transform(inputCoordinateSweref99, crsIdentifierWGS84)).Returns(leafResult4);
        mock5.Setup(leaf => leaf.Transform(inputCoordinateSweref99, crsIdentifierWGS84)).Returns(leafResult5);

        //mock1.Setup(leaf => leaf.LongNameOfImplementation).Returns("1");
        //mock2.Setup(leaf => leaf.LongNameOfImplementation).Returns("2");
        //mock3.Setup(leaf => leaf.LongNameOfImplementation).Returns("3");
        //mock4.Setup(leaf => leaf.LongNameOfImplementation).Returns("4");
        //mock5.Setup(leaf => leaf.LongNameOfImplementation).Returns("5");
        mock1.Setup(leaf => leaf.AdapteeType).Returns(CrsTransformationAdapteeType.LEAF_DOT_SPATIAL_2_0_0_RC1);
        mock2.Setup(leaf => leaf.AdapteeType).Returns(CrsTransformationAdapteeType.LEAF_PROJ_NET_4_GEO_API_1_4_1);
        mock3.Setup(leaf => leaf.AdapteeType).Returns(CrsTransformationAdapteeType.LEAF_SWEDISH_CRS_MLG_1_0_1);
        // The type must be different but there are only three concrete types as above to use
        // but then instead can use the ones below (and the purpose of this enum is to use it as key in a dictionary/hashtable)
        mock4.Setup(leaf => leaf.AdapteeType).Returns(CrsTransformationAdapteeType.UNSPECIFIED_LEAF);
        mock5.Setup(leaf => leaf.AdapteeType).Returns(CrsTransformationAdapteeType.UNSPECIFIED);

        allLeafAdapters = new List<ICrsTransformationAdapter>{
            leafAdapterImplementation_1,
            leafAdapterImplementation_2,
            leafAdapterImplementation_3,
            leafAdapterImplementation_4,
            leafAdapterImplementation_5
        };
    }

    private const double SMALL_DELTA_VALUE_FOR_COMPARISONS = 0.00000000000001;

    [Test]
    public void transformToCoordinate_shouldReturnAverageResult_whenUsingAverageCompositeAdapter() {
        CrsTransformationAdapterComposite averageCompositeAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage(allLeafAdapters);
        CrsCoordinate resultCoordinate = averageCompositeAdapter.TransformToCoordinate(inputCoordinateSweref99, EpsgNumber.WORLD__WGS_84__4326);
        Assert.IsNotNull(resultCoordinate);

        Assert.AreEqual(
            expectedAverageLatitude,  
            resultCoordinate.YNorthingLatitude, 
            SMALL_DELTA_VALUE_FOR_COMPARISONS
        );
        Assert.AreEqual(
            expectedAverageLongitude, 
            resultCoordinate.XEastingLongitude, 
            SMALL_DELTA_VALUE_FOR_COMPARISONS
        );

        assertCompositeResultHasLeafSubResults(
            averageCompositeAdapter,
            allLeafAdapters.Count // expectedNumberOfLeafResults
        );
    }

    [Test]
    public void transformToCoordinate_shouldReturnMedianResult_whenUsingMedianCompositeAdapter() {
        CrsTransformationAdapterComposite medianCompositeAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian(allLeafAdapters);
        CrsCoordinate resultCoordinate = medianCompositeAdapter.TransformToCoordinate(inputCoordinateSweref99, EpsgNumber.WORLD__WGS_84__4326);
        Assert.IsNotNull(resultCoordinate);

        Assert.AreEqual(
            expectedMedianLatitude,  
            resultCoordinate.YNorthingLatitude, 
            SMALL_DELTA_VALUE_FOR_COMPARISONS
        );
        Assert.AreEqual(
            expectedMedianLongitude, 
            resultCoordinate.XEastingLongitude, 
            SMALL_DELTA_VALUE_FOR_COMPARISONS
        );

        assertCompositeResultHasLeafSubResults(
            medianCompositeAdapter,
            allLeafAdapters.Count // expectedNumberOfLeafResults
        );
    }

    [Test]
    public void transformToCoordinate_shouldReturnFirstResult_whenUsingFirstSuccessCompositeAdapter() {
        CrsTransformationAdapterComposite firstSuccessCompositeAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess(allLeafAdapters);
        CrsCoordinate resultCoordinate = firstSuccessCompositeAdapter.TransformToCoordinate(inputCoordinateSweref99, EpsgNumber.WORLD__WGS_84__4326);
        Assert.IsNotNull(resultCoordinate);

        // The assumption below (according to the setup code in the "before" method in this JUnit class)
        // is that the first adapter in the above list allLeafAdapters will return the result outputCoordinateWgs84ForImplementation_1
        Assert.AreEqual(
            outputCoordinateWgs84ForImplementation_1.YNorthingLatitude,  
            resultCoordinate.YNorthingLatitude, 
            SMALL_DELTA_VALUE_FOR_COMPARISONS
        );
        Assert.AreEqual(
            outputCoordinateWgs84ForImplementation_1.XEastingLongitude, 
            resultCoordinate.XEastingLongitude, 
            SMALL_DELTA_VALUE_FOR_COMPARISONS
        );

        assertCompositeResultHasLeafSubResults(
            firstSuccessCompositeAdapter,
            1 // expectedNumberOfLeafResults
        );
    }

    [Test]
    public void transformToCoordinate_shouldReturnWeightedAverageResult_whenUsingWeightedAverageCompositeAdapter() {
        double[] weights = {1,2,4,5,9};
        double totWeights = 0;
        double totLats = 0;
        double totLons = 0;
        for (int i = 0; i <weights.Length; i++) {
            double weight = weights[i];
            totWeights += weight;
            CrsCoordinate coordinate = outputCoordinates[i];
            totLats += weight * coordinate.YNorthingLatitude;
            totLons += weight * coordinate.XEastingLongitude;
        }
        double weightedLat = totLats / totWeights;
        double weightedLon = totLons / totWeights;
        CrsCoordinate expectedWeightedAverage = CrsCoordinateFactory.CreateFromLatitudeLongitude(weightedLat, weightedLon);

        List<CrsTransformationAdapterWeight> weightedAdapters = new List<CrsTransformationAdapterWeight>{
            CrsTransformationAdapterWeight.CreateFromInstance(leafAdapterImplementation_1, weights[0]),
            CrsTransformationAdapterWeight.CreateFromInstance(leafAdapterImplementation_2, weights[1]),
            CrsTransformationAdapterWeight.CreateFromInstance(leafAdapterImplementation_3, weights[2]),
            CrsTransformationAdapterWeight.CreateFromInstance(leafAdapterImplementation_4, weights[3]),
            CrsTransformationAdapterWeight.CreateFromInstance(leafAdapterImplementation_5, weights[4])
        };

        CrsTransformationAdapterComposite weightedAverageCompositeAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationWeightedAverage(weightedAdapters);
        CrsCoordinate result = weightedAverageCompositeAdapter.TransformToCoordinate(inputCoordinateSweref99, EpsgNumber.WORLD__WGS_84__4326);
        Assert.IsNotNull(result);

        Assert.AreEqual(expectedWeightedAverage.YNorthingLatitude,  result.YNorthingLatitude, SMALL_DELTA_VALUE_FOR_COMPARISONS);
        Assert.AreEqual(expectedWeightedAverage.XEastingLongitude, result.XEastingLongitude, SMALL_DELTA_VALUE_FOR_COMPARISONS);

        assertCompositeResultHasLeafSubResults(
            weightedAverageCompositeAdapter,
            allLeafAdapters.Count
        );
    }

    private void assertCompositeResultHasLeafSubResults(
        CrsTransformationAdapterComposite compositeAdapter,
        int expectedNumberOfLeafResults
    ) {
        CrsTransformationResult compositeTransformResult = compositeAdapter.Transform(inputCoordinateSweref99, crsIdentifierWGS84);
        Assert.IsNotNull(compositeTransformResult);
        Assert.IsTrue(compositeTransformResult.IsSuccess);
        //assertEquals(expectedNumberOfLeafResults, allLeafAdapters.size()); // five "leafs" were used to calculate the composite
        Assert.AreEqual(expectedNumberOfLeafResults, compositeTransformResult.GetTransformationResultChildren().Count);

        IList<CrsTransformationResult> subResults = compositeTransformResult.GetTransformationResultChildren();
        for (int i = 0; i < subResults.Count; i++) {
            CrsTransformationResult transformResult = subResults[i];
            ICrsTransformationAdapter leafAdapter = allLeafAdapters[i];
            CrsTransformationResult transformResultForLeaf = leafAdapter.Transform(inputCoordinateSweref99, crsIdentifierWGS84);
            Assert.IsNotNull(transformResultForLeaf);
            Assert.IsTrue(transformResultForLeaf.IsSuccess);
            assertEqualCoordinate(transformResult.OutputCoordinate, transformResultForLeaf.OutputCoordinate);
            Assert.AreEqual(0, transformResultForLeaf.GetTransformationResultChildren().Count); // no subresults for a leaf
        }
    }

    private void assertEqualCoordinate(
        CrsCoordinate c1,
        CrsCoordinate c2
    ) {
        Assert.AreEqual(c1.YNorthingLatitude, c2.YNorthingLatitude, SMALL_DELTA_VALUE_FOR_COMPARISONS);
        Assert.AreEqual(c1.XEastingLongitude, c2.XEastingLongitude, SMALL_DELTA_VALUE_FOR_COMPARISONS);
    }

    // --------------------------------------------------------------
    [Test]
    public void simpleExampleShowingHowToUseTestStubbingWithMoq() {
        // Moq: https://github.com/Moq/moq4

        var mock = new Mock<IList<string>>();
        mock.Setup(list => list[0]).Returns("first");
        mock.Setup(list => list[1]).Returns("second");
        IList<string> mockedList = mock.Object;

        // Note that the method/parameter combination "get(0)" and "get(1)"
        // can be invoked multiple times and in different order sompared to the order defined above
        Assert.AreEqual("second", mockedList[1]);
        Assert.AreEqual("first", mockedList[0]);
        Assert.AreEqual("first", mockedList[0]);
        Assert.AreEqual("second", mockedList[1]);
    }
    // --------------------------------------------------------------
}
}