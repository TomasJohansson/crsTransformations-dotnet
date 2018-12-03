using NUnit.Framework;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;

namespace Programmerare.CrsTransformations.CompositeTransformations 
{
public class CompositeStrategyForFirstSuccessTest : CompositeStrategyTestBase {

    [Test]
    public void transform_shouldReturnFirstResult_whenUsingFirstSuccessCompositeAdapter() {
        var crsTransformationAdapterCompositeFactory = new CrsTransformationAdapterCompositeFactory();
        ICrsTransformationAdapter firstSuccessCompositeAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess(
            // note that DotSpatial should be the first item in the below list defined in the baseclass,
            // and therefore DotSpatial should be the implementation providing the result
            base.allAdapters
        );
        CrsTransformationResult firstSuccessResult = firstSuccessCompositeAdapter.Transform(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        Assert.IsNotNull(firstSuccessResult);
        Assert.IsTrue(firstSuccessResult.IsSuccess);
        Assert.AreEqual(1, firstSuccessResult.GetTransformationResultChildren().Count);

        CrsCoordinate coordinateReturnedByCompositeAdapterFirstSuccess = firstSuccessResult.OutputCoordinate;
        // The above result of the composite should be equal to the result of DotSpatial since it
        // is first in the list of parameters to the constructor and it should produce a result for
        // the input coordinates ... so therefore below assert against the direct result of DotSpatial
        CrsCoordinate coordinateResultWhenUsingDotSpatial = adapterDotSpatial.TransformToCoordinate(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        Assert.AreEqual(
            coordinateResultWhenUsingDotSpatial, 
            coordinateReturnedByCompositeAdapterFirstSuccess
        );
    }
}
}