using Programmerare.CrsTransformations.Adapter.DotSpatial;
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

namespace Programmerare.CrsTransformations.Test.Implementations
{
    [TestFixture]
    class DotSpatialTest : AdaptersTestBase
    {
        // The below comment does not apply to DotSpatial anymore 
        // after the upgrade 2.0.0-rc1 as commented above.

        // The two implementations DotSpatial and ProjNet4GeoAPI
        // (DotSpatial is tested below, and ProjNet4GeoAPI from another test class)
        // are currently producing similarly bad results when transforming 
        // to the Swedish CRS "RT90 2.5 gon V"
        // Then it may seem reasonable to suspect that those 
        // two are actually correct, but the reason for considering them to 
        // be non-accurate is the fact that the tested coordinates
        // (which are defined in the above inherited base class)
        // have been retrieved with FIVE different websites
        // (among others the two websites hitta.se and Eniro which are 
        //   very well-known and visited websites for Swedes
        //   and those two should really have made sure to implement 
        //   correct transformation to the two Swedish coordinate reference systems RT90 and SWEREF99)

        [SetUp]
        public void SetUp()
        {
            base.SetUpbase(
                new CrsTransformationAdapterDotSpatial(),
                CrsTransformationAdapteeType.LEAF_DOT_SPATIAL_1_9_0,
                
                // It is questionable where to draw the limit 
                // between a failing and succeeding test 
                // when the result is not accurate,
                // and here below I it is indeed reasonable to claim 
                // that I have "cheated" with high values 
                // to get "succeeding" tests ...

                // DotSpatial 2.0.0-rc1:
                0.5, // maxMeterDifferenceForSuccessfulTest
                // DotSpatial 1.9.0:
                // 195, // maxMeterDifferenceForSuccessfulTest

                // Note that one of the test fails if the above value 
                // is changed to 190 meters !
                // Yes METERS ! i.e. this is not very accurate results 
                // with the current DotSpatial implementation for the Swedish CRS used

                // DotSpatial 2.0.0-rc1:
                0.00001 // maxLatLongDifferenceForSuccessfulTest
                // DotSpatial 1.9.0:
                // 0.01 // maxLatLongDifferenceForSuccessfulTest
            );
        }
    }
}