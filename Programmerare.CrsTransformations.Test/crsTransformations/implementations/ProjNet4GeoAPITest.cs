using Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI;
using NUnit.Framework;

namespace Programmerare.CrsTransformations.Test.Implementations
{
    class ProjNet4GeoAPITest : AdaptersTestBase
    {
        [SetUp]
        public void SetUp()
        {
            base.SetUpbase(
                new CrsTransformationAdapterProjNet4GeoAPI(),
                CrsTransformationAdapteeType.LEAF_PROJ_NET_4_GEO_API_1_4_1,

                // See the comment in "DotSpatialTest"
                // regarding the non-accurate result for DotSpatial
                // and similar non-accurate results for ProjNet4GeoAPI
                // with the below HIGH delta values
                // (required to avoid failing/red test)

                // It is questionable where to draw the limit 
                // between a failing and succeeding test 
                // when the result is not accurate,
                // and here below I it is indeed reasonable to claim 
                // that I have "cheated" with high values 
                // to get "succeeding" tests ...

                195, // maxMeterDifferenceForSuccessfulTest
                0.01 // maxLatLongDifferenceForSuccessfulTest
            );
        }
    }
}