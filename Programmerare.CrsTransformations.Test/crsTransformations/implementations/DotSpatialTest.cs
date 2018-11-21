using com.programmerare.crsTransformations.Adapter.DotSpatial;
using com.programmerare.crsTransformations;
using NUnit.Framework;

namespace Programmerare.CrsTransformations.Test.crsTransformations.implementations
{
    [TestFixture]
    class DotSpatialTest : AdaptersTestBase
    {
        [SetUp]
        public void SetUp()
        {
            base.SetUpbase(
                new CrsTransformationAdapterDotSpatial(),
                CrsTransformationAdapteeType.LEAF_DOT_SPATIAL_1_9_0,
                
                195, // maxMeterDifferenceForSuccessfulTest
                // Note that one of the test fails if the above value 
                // is changed to 190 meters !
                // Yes METERS ! i.e. this is not very accurate results 
                // with the current DotSpatial implementation for the Swedish CRS used

                0.01 // maxLatLongDifferenceForSuccessfulTest
            );
        }
    }
}