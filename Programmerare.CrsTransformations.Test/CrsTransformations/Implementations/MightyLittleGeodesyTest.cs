using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using NUnit.Framework;

namespace Programmerare.CrsTransformations.Test.Implementations
{
    [TestFixture]
    class MightyLittleGeodesyTest : AdaptersTestBase
    {
        [SetUp]
        public void SetUp()
        {
            base.SetUpbase(
                new CrsTransformationAdapterMightyLittleGeodesy(),
                CrsTransformationAdapteeType.LEAF_SWEDISH_CRS_MLG_1_0_1,
                0.5, // maxMeterDifferenceForSuccessfulTest
                0.00001 // maxLatLongDifferenceForSuccessfulTest
            );
        }
    }
}