using System;
using System.Collections.Generic;
using NUnit.Framework;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.CompositeTransformations;
using Programmerare.CrsTransformations.Adapter.DotSpatial;

namespace Programmerare.CrsTransformations {
// Base class with some common code reused by its subclasses 

[TestFixture]
public abstract class CrsTransformationResultTestBase {
    protected List<CrsTransformationResult> listOfSubresultsForStatisticsTest;
    protected double expectedLatDiffMax, expectedLonDiffMax;
    protected CrsCoordinate expectedCoordinateMean, expectedCoordinateAverage;
    protected CrsCoordinate inputCoordinateNotUsedInStatisticsTest, outputCoordinateNotUsedInStatisticsTest;
    protected ICrsTransformationAdapter compositeAdapterForResultTest;
    
    [SetUp]
    public void SetUpBase() {
        // The setup code below creates four coordinates 
        // representing results from four implementations.
        double lat1 = 59.330231;
        double lat2 = 59.330232;
        double lat3 = 59.330233;
        double lat4 = 59.330239;
        double latMean = (lat2 + lat3 ) / 2;
        double latAverage = (lat1 + lat2 + lat3 + lat4) / 4;
        expectedLatDiffMax = lat4-lat1;
        double lon1 = 18.059192;
        double lon2 = 18.059193;
        double lon3 = 18.059194;
        double lon4 = 18.059198;
        double lonMean = (lon2 + lon3 ) / 2;
        double lonAverage = (lon1 + lon2 + lon3 + lon4) / 4;
        expectedLonDiffMax = lon4-lon1;
        expectedCoordinateMean = CrsCoordinateFactory.LatLon(latMean, lonMean);
        expectedCoordinateAverage = CrsCoordinateFactory.LatLon(latAverage, lonAverage);

        CrsCoordinate outputCoordinate1 = CrsCoordinateFactory.LatLon(lat1, lon1);
        CrsCoordinate outputCoordinate2 = CrsCoordinateFactory.LatLon(lat2, lon2);
        CrsCoordinate outputCoordinate3 = CrsCoordinateFactory.LatLon(lat3, lon3);
        CrsCoordinate outputCoordinate4 = CrsCoordinateFactory.LatLon(lat4, lon4);

        var crsTransformationAdapterCompositeFactory = CrsTransformationAdapterCompositeFactory.Create();
        compositeAdapterForResultTest = crsTransformationAdapterCompositeFactory.CreateCrsTransformationMedian();
        inputCoordinateNotUsedInStatisticsTest = CrsCoordinateFactory.LatLon(0.0, 0.0); // input, not used here in this test
        outputCoordinateNotUsedInStatisticsTest = inputCoordinateNotUsedInStatisticsTest;

        ICrsTransformationAdapter leafAdapterForResultTest = new CrsTransformationAdapterDotSpatial();
        // Can use the same above adapter for all parts below. Not used much except that the object must be some leaf
        
        listOfSubresultsForStatisticsTest = new List<CrsTransformationResult>{
            CreateCrsTransformationResult(outputCoordinate1, leafAdapterForResultTest, inputCoordinateNotUsedInStatisticsTest),
            CreateCrsTransformationResult(outputCoordinate2, leafAdapterForResultTest, inputCoordinateNotUsedInStatisticsTest),
            CreateCrsTransformationResult(outputCoordinate3, leafAdapterForResultTest, inputCoordinateNotUsedInStatisticsTest),
            CreateCrsTransformationResult(outputCoordinate4, leafAdapterForResultTest, inputCoordinateNotUsedInStatisticsTest)
        };
    }

    private CrsTransformationResult CreateCrsTransformationResult(
        CrsCoordinate outputCoordinate,
        ICrsTransformationAdapter adapter,
        CrsCoordinate inputCoordinateNotUsedInThisTest
    ) {
        Exception exceptionNull = null;
        bool isSuccessTrue = true;
        return CrsTransformationResult._CreateCrsTransformationResult(
            inputCoordinateNotUsedInThisTest,
            outputCoordinate,
            exceptionNull,
            isSuccessTrue,
            adapter,
            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(new List<CrsTransformationResult>())
        );
    }

    protected void AssertCrsTransformationResultStatistic(CrsTransformationResultStatistic crsTransformationResultStatistic) {
        Assert.IsNotNull(crsTransformationResultStatistic);
        Assert.IsTrue(crsTransformationResultStatistic.IsStatisticsAvailable);
        Assert.AreEqual(
            listOfSubresultsForStatisticsTest.Count,
            crsTransformationResultStatistic.NumberOfPotentiallySuccesfulResults
        );
        Assert.AreEqual(
            this.expectedLatDiffMax, 
            crsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude
        );
        Assert.AreEqual(
            this.expectedLonDiffMax, 
            crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude
        );
        CrsCoordinate coordinateAverage = crsTransformationResultStatistic.CoordinateAverage;
        CrsCoordinate coordinateMean = crsTransformationResultStatistic.CoordinateMedian;
        Assert.IsNotNull(coordinateAverage);
        Assert.IsNotNull(coordinateMean);
        Assert.AreEqual(this.expectedCoordinateMean, coordinateMean);
        Assert.AreEqual(this.expectedCoordinateAverage, coordinateAverage);
    }
} // the test class ends here
} // namespace ends here