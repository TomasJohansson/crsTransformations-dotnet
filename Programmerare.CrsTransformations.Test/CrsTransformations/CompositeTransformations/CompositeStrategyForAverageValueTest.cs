using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.Identifier;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;

namespace Programmerare.CrsTransformations.CompositeTransformations 
{
public class CompositeStrategyForAverageValueTest : CompositeStrategyTestBase {

    private const double delta = 0.000000001;

    [Test]
    public void transform_shouldReturnAverageResult_whenUsingAverageCompositeAdapter() {
        CrsCoordinate coordinateWithAverageLatitudeAndLongitude = calculateAverageCoordinate(
            base.allCoordinateResultsForTheDifferentImplementations
        );

        var crsTransformationAdapterCompositeFactory = new CrsTransformationAdapterCompositeFactory();
        ICrsTransformationAdapter averageCompositeAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationAverage(
            allAdapters
        );
        CrsTransformationResult averageResult = averageCompositeAdapter.Transform(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        Assert.IsNotNull(averageResult);
        Assert.IsTrue(averageResult.IsSuccess);
        Assert.AreEqual(base.allCoordinateResultsForTheDifferentImplementations.Count, averageResult.GetTransformationResultChildren().Count);

        CrsCoordinate coordinateReturnedByCompositeAdapter = averageResult.OutputCoordinate;

        Assert.AreEqual(
            coordinateWithAverageLatitudeAndLongitude.XEastingLongitude, 
            coordinateReturnedByCompositeAdapter.XEastingLongitude, 
            delta
        );
        Assert.AreEqual(
            coordinateWithAverageLatitudeAndLongitude.YNorthingLatitude, 
            coordinateReturnedByCompositeAdapter.YNorthingLatitude, 
            delta
        );
        // assertEquals(coordinateWithAverageLatitudeAndLongitude, coordinateReturnedByCompositeAdapter);
        // Expected :Coordinate(xEastingLongitude=674032.3572074446, yNorthingLatitude=6580821.991903967, crsIdentifier=CrsIdentifier(crsCode=EPSG:3006, isEpsgCode=true, epsgNumber=3006))
        // Actual   :Coordinate(xEastingLongitude=674032.3572074447, yNorthingLatitude=6580821.991903967, crsIdentifier=CrsIdentifier(crsCode=EPSG:3006, isEpsgCode=true, epsgNumber=3006))
    }

    private CrsCoordinate calculateAverageCoordinate(
        List<CrsCoordinate> resultCoordinates
    ) {
        double averageLat = resultCoordinates.Select(c => c.Latitude).Average();
        double averageLon = resultCoordinates.Select(c => c.Longitude).Average();
        return CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(averageLat, averageLon, resultCoordinates[0].CrsIdentifier);
    }

    [Test]
    public void resultCoordinates_mustBeDefinedWithTheSameCoordinateReferenceSystem()
    {
        var resultCoordinates = base.allCoordinateResultsForTheDifferentImplementations;
        List<IGrouping<CrsIdentifier, CrsCoordinate>> res = resultCoordinates.GroupBy(c => c.CrsIdentifier).ToList();
        string messageIfFailing = "all coordinates should have the same CRS, since thet should all be the result of a transform to the same CRS";
        Assert.AreEqual(1, res.Count, messageIfFailing);
        CrsIdentifier crsIdentifier = res[0].Key;
        Assert.AreEqual(
            resultCoordinates[0].CrsIdentifier,
            crsIdentifier
        );
    }
}
}