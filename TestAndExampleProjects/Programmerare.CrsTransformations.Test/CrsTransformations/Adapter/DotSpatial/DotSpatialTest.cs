using NUnit.Framework;

// https://www.nuget.org/packages/DotSpatial.Projections
// The current (as of november 2018) latest versions:
// 2.0.0-rc1    2017-03-11
// 1.9.0        2016-04-28

// Version 1.9.0 caused problems (e.g. in this test class below) with very 
// bad results for some Swedish coordinate reference systems.
// Therefore the release candadidate is currently used.
// The problem was fixed in october 2016 
// (i.e. about half a year after relase 1.9.0)
// https://github.com/DotSpatial/DotSpatial/pull/865

namespace Programmerare.CrsTransformations.Adapter.DotSpatial {

    [TestFixture]
    class DotSpatialTest : AdaptersTestBase {

        [SetUp]
        public void SetUp() {
            base.SetUpbase(
                new CrsTransformationAdapterDotSpatial(),
                CrsTransformationAdapteeType.LEAF_DOT_SPATIAL_2_0_0_RC1,

                // DotSpatial 2.0.0-rc1:
                0.5, // maxMeterDifferenceForSuccessfulTest
                // DotSpatial 1.9.0:
                // 195, // maxMeterDifferenceForSuccessfulTest
                // Note that one of the tests fails (for DotSpatial 1.9.0) 
                // if the above value was changed to 190 meters !
                // But the problem is fixed with version 2.0.0-rc1
                // https://github.com/DotSpatial/DotSpatial/pull/865

                // DotSpatial 2.0.0-rc1:
                0.00001 // maxLatLongDifferenceForSuccessfulTest
                // DotSpatial 1.9.0:
                // 0.01 // maxLatLongDifferenceForSuccessfulTest
            );
        }
    }
}