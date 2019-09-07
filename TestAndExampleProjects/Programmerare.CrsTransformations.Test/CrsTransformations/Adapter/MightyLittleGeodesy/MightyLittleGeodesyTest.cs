using NUnit.Framework;

namespace Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy {

    [TestFixture]
    class MightyLittleGeodesyTest : AdaptersTestBase {

        [SetUp]
        public void SetUp() {
            base.SetUpbase(
                new CrsTransformationAdapterMightyLittleGeodesy(),
                CrsTransformationAdapteeType.LEAF_MIGHTY_LITTLE_GEODESY_1_0_2,
                0.5, // maxMeterDifferenceForSuccessfulTest
                0.00001 // maxLatLongDifferenceForSuccessfulTest
            );
        }
    }
}