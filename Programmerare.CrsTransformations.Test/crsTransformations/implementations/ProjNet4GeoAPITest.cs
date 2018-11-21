using com.programmerare.crsTransformations.Adapter.ProjNet4GeoAPI;
using com.programmerare.crsTransformations;
using NUnit.Framework;

namespace Programmerare.CrsTransformations.Test.crsTransformations.implementations
{
    [TestFixture(Ignore="Need to implement a 'SridReader", IgnoreReason ="System.NotSupportedException : No support for transforming between the two specified coordinate systems")]
    class ProjNet4GeoAPITest : AdaptersTestBase
    {
        [SetUp]
        public void SetUp()
        {
            base.SetUpbase(
                new CrsTransformationAdapterProjNet4GeoAPI(),
                CrsTransformationAdapteeType.LEAF_PROJ_NET_4_GEO_API_1_4_1,
                300, // maxMeterDifferenceForSuccessfulTest
                0.1 // maxLatLongDifferenceForSuccessfulTest
            );
        }
    }
}