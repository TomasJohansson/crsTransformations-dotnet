using NUnit.Framework;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v10_027;
using System.Collections.Generic;

namespace Programmerare.CrsTransformations.Core.CompositeTransformations  {

[TestFixture]
public class CompositeStrategyFirstSuccessTest : CompositeStrategyTestBase {

    [Test]
    public void Transform_ShouldReturnFirstResult_WhenUsingFirstSuccessCompositeAdapter() {
        ICrsTransformationAdapter firstSuccessCompositeAdapter = crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess(
            // note that DotSpatial should be the first item in the below list defined in the baseclass,
            // and therefore DotSpatial should be the implementation providing the result
            base.allAdapters
        );
        CrsTransformationResult firstSuccessResult = firstSuccessCompositeAdapter.Transform(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        Assert.IsNotNull(firstSuccessResult);
        Assert.IsTrue(firstSuccessResult.IsSuccess);
        Assert.AreEqual(1, firstSuccessResult.TransformationResultChildren.Count);

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

    [Test]
    public void FirstSuccessAdapter_ShouldNotBeEqual_WhenLeafAdaptersAreAggregatedInDifferentOrder() {
        ICrsTransformationAdapter firstSuccess1, firstSuccess2, firstSuccess3;
        firstSuccess1 = crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess(
            new List<ICrsTransformationAdapter>{
                base.adapterDotSpatial,
                base.adapterMightyLittleGeodesy
            }
        );
        // Now creating the same as above but in reversed 
        // order (and thus they should NOT be considered Equal)
        firstSuccess2 = crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess(
            new List<ICrsTransformationAdapter>{
                base.adapterMightyLittleGeodesy,
                base.adapterDotSpatial
            }
        );
        Assert.AreNotEqual(firstSuccess1, firstSuccess2);


        // Now below creating a new instance "firstSuccess3" with the same 
        // adapters as above "firstSuccess2" and thus they should be considered Equal
        firstSuccess3 = crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess(
            new List<ICrsTransformationAdapter>{
                base.adapterMightyLittleGeodesy,
                base.adapterDotSpatial
            }
        );            
        Assert.AreEqual(firstSuccess2, firstSuccess3);
        Assert.AreEqual(firstSuccess2.GetHashCode(), firstSuccess3.GetHashCode());
    }
}
}