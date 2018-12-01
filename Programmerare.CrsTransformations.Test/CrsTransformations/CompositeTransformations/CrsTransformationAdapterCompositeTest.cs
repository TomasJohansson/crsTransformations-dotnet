namespace Programmerare.CrsTransformations.CompositeTransformations {

using System;
using Programmerare.CrsTransformations;
using Programmerare.CrsTransformations.Coordinate;
using NUnit.Framework;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;

[TestFixture]
public class CrsTransformationAdapterCompositeTest {

    private const int EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS = CrsTransformationAdapterTest.EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS;

    [Test]
    public void isReliableTest() {
        CrsTransformationAdapterComposite crsTransformationComposite = CrsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage();
        var children = crsTransformationComposite.GetTransformationAdapterChildren();
        Assert.AreEqual(3, children.Count);

        CrsCoordinate wgs84coordinateInSweden = CrsCoordinateFactory.LatLon(59.31,18.04);
        CrsTransformationResult resultWhenTransformingToSwedishCRS = crsTransformationComposite.Transform(wgs84coordinateInSweden, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        Assert.IsNotNull(resultWhenTransformingToSwedishCRS);
        Assert.IsTrue(resultWhenTransformingToSwedishCRS.IsSuccess);
        CrsTransformationResultStatistic crsTransformationResultStatistic = resultWhenTransformingToSwedishCRS.CrsTransformationResultStatistic;
        Assert.IsNotNull(crsTransformationResultStatistic);
        Assert.IsTrue(crsTransformationResultStatistic.IsStatisticsAvailable);

        int actualNumberOfResults = crsTransformationResultStatistic.NumberOfPotentiallySuccesfulResults;
        Assert.AreEqual(
            EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS,
            actualNumberOfResults
        );
        double actualMaxDiffXLongitude = crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude;
        double actualMaxDiffYLatitude = crsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude;
        double actualMaxDiffXorY = Math.Max(actualMaxDiffXLongitude, actualMaxDiffYLatitude);
        Assert.That(actualMaxDiffXorY, Is.LessThan(0.01));

        Assert.IsTrue(resultWhenTransformingToSwedishCRS.IsReliable(actualNumberOfResults, actualMaxDiffXorY));

        // assertFalse below since trying to require one more result than available
        Assert.IsFalse(resultWhenTransformingToSwedishCRS.IsReliable(actualNumberOfResults + 1, actualMaxDiffXorY));

        // assertFalse below since trying to require too small maxdiff
        Assert.IsFalse(resultWhenTransformingToSwedishCRS.IsReliable(actualNumberOfResults, actualMaxDiffXorY - 0.00000000001));
    }
}
}