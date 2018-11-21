using com.programmerare.crsTransformations.Adapter.MightyLittleGeodesy;
using com.programmerare.crsTransformations;
using NUnit.Framework;

namespace Programmerare.CrsTransformations.Test.crsTransformations.implementations
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