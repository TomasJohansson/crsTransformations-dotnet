using Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI;
using NUnit.Framework;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;

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

                // The implementation ProjNet4GeoAPI
                // is currently (version 1.4.1) 
                // producing bad results when transforming 
                // to the Swedish CRS "RT90 2.5 gon V"
                // https://github.com/NetTopologySuite/ProjNet4GeoAPI/issues/38


                // It is questionable where to draw the limit 
                // between a failing and succeeding test 
                // when the result is not accurate,
                // and here below I it is indeed reasonable to claim 
                // that I have "cheated" with high values 
                // to get "succeeding" tests ...

                //195, // maxMeterDifferenceForSuccessfulTest
                //0.01 // maxLatLongDifferenceForSuccessfulTest

                // something like the above limits are 
                // currently required for the shipped CSV file
                // but if modifying the CRS definition for 
                // RT90 then the below will work:

                0.5, // maxMeterDifferenceForSuccessfulTest
                0.00001 // maxLatLongDifferenceForSuccessfulTest

                // from the implementation class:
                //let mutable _sridReader = SridReader(EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_SHIPPED_WITH_ProjNet4GeoAPI)
                // the above causes failing tests for Swedish CRS RT90
                //let mutable _sridReader = SridReader(EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_EXCEPT_FOR_SWEDISH_CRS_WITH_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml)
            );


            // The implementation ProjNet4GeoAPI
            // does not contain a lot of hardcoded definitions 
            // of coordinate systems but instead it is shipped with a CSV file 
            // with such definitions, and their website shows 
            // example code of how to parse it, which indeed has 
            // been implemented in the adapter project.
            // However, for performance reason you do not want to read 
            // the CSV file every time an EPSG code is looked up.
            // For that reason, caching has been implemented 
            // as an option, which is tested in this class
            // (plus all the other tests not related to caching which is inherited from the base test class)
            cacheTestForOutputEpsgNumberNotExisting = 123; // EPSG code 123 does NOT exist
            cacheTestInputCoordinateWgs84 = CrsCoordinateFactory.LatLon(60, 18);
            
            cacheTestTransformationAdapterProjNet4GeoAPI = new CrsTransformationAdapterProjNet4GeoAPI();
            cacheTestTransformationAdapter = cacheTestTransformationAdapterProjNet4GeoAPI;
            // TODO: (regarding the above two typed variables) 
            // Deal with implicit versus explicit interfaces.
            // Currently:
                // These method exists:
                //      cacheTestTransformationAdapter.Transform
                //      cacheTestTransformationAdapterProjNet4GeoAPI.SetCrsCachingStrategy
                // These method do NOT exist:
                //      cacheTestTransformationAdapterProjNet4GeoAPI.Transform
                //      cacheTestTransformationAdapter.SetCrsCachingStrategy
        }

        private CrsCoordinate cacheTestInputCoordinateWgs84;
        private int cacheTestForOutputEpsgNumberNotExisting;
        private ICrsTransformationAdapter cacheTestTransformationAdapter;
        private CrsTransformationAdapterProjNet4GeoAPI cacheTestTransformationAdapterProjNet4GeoAPI;

        [Test]
        public void TestCachingWhenAllEpsgCodesAreCachedInOneReadOfTheCsvFile()
        {
            VerifyExpectedCachingBehaviour(
                CrsCachingStrategy.CACHE_ALL_EPSG_CRS_CODES
            );
        }
        
        [Test]
        public void TestCachingWhenEpsgCodesAreCachedAtFirstRequest()
        {
            VerifyExpectedCachingBehaviour(
                CrsCachingStrategy.CACHE_EPSG_CRS_CODE_WHEN_FIRST_USED
            );
        }
        
        [Test]
        public void TestBehaviourWhenNoCachingIsChosen()
        {
            cacheTestTransformationAdapterProjNet4GeoAPI.SetCrsCachingStrategy(
                CrsCachingStrategy.NO_CACHING
            );            
            Assert.IsFalse(
                cacheTestTransformationAdapterProjNet4GeoAPI.IsEpsgCached(
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );
            Assert.IsFalse(
                cacheTestTransformationAdapterProjNet4GeoAPI.IsEpsgCached(
                    EpsgNumber.SWEDEN__SWEREF99_TM__3006
                )
            );

            // The transform method below may potentially trigger 
            // the used epsg code (the second parameter) to become cached
            // (but that behaviour depends on the caching strategy)
            cacheTestTransformationAdapter.Transform(
                cacheTestInputCoordinateWgs84, 
                cacheTestForOutputEpsgNumberNotExisting
            );
            // The above output CRS was a non-existing EPSG number
            // but below is an existing EPSG number used.
            cacheTestTransformationAdapter.Transform(
                cacheTestInputCoordinateWgs84, 
                EpsgNumber.SWEDEN__SWEREF99_TM__3006
            );

            // Neither of the above are still not cached since caching is disabled
            Assert.IsFalse(
                cacheTestTransformationAdapterProjNet4GeoAPI.IsEpsgCached(
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );
            Assert.IsFalse(
                cacheTestTransformationAdapterProjNet4GeoAPI.IsEpsgCached(
                    EpsgNumber.SWEDEN__SWEREF99_TM__3006
                )
            );
        }        

        private void VerifyExpectedCachingBehaviour(
            CrsCachingStrategy crsCachingStrategy
        )
        {
            // First we initialize with "NO_CACHING" but later in the method 
            // we will set the strategy which was parameter to the method
            cacheTestTransformationAdapterProjNet4GeoAPI.SetCrsCachingStrategy(
                CrsCachingStrategy.NO_CACHING
            );
            Assert.IsFalse(
                cacheTestTransformationAdapterProjNet4GeoAPI.IsEpsgCached(
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );
            cacheTestTransformationAdapter.Transform(
                cacheTestInputCoordinateWgs84, 
                cacheTestForOutputEpsgNumberNotExisting
            );
            Assert.IsFalse(
                cacheTestTransformationAdapterProjNet4GeoAPI.IsEpsgCached(
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );
            
            cacheTestTransformationAdapterProjNet4GeoAPI.SetCrsCachingStrategy(
                crsCachingStrategy
            );

            // still false (since it will not be cached until looked up again)
            Assert.IsFalse(
                cacheTestTransformationAdapterProjNet4GeoAPI.IsEpsgCached(
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );

            // The transform method below may potentially trigger 
            // the used epsg code (the second parameter) to become cached
            // (but that behaviour depends on the caching strategy)
            cacheTestTransformationAdapter.Transform(
                cacheTestInputCoordinateWgs84, 
                cacheTestForOutputEpsgNumberNotExisting
            );

            // But this time it should be true since the caching was changed above 
            // and indeed the lookup is cached even though it is a non-existin
            // EPSG number (i.e. "null" is cached)
            // since you do likely not want to read the file 
            // many times (if you have specified that you want caching)
            // just to get null again, so therefore the null value 
            // is also cached as a kind of value in the semantic of the below method
            Assert.IsTrue(
                cacheTestTransformationAdapterProjNet4GeoAPI.IsEpsgCached(
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );

            if(crsCachingStrategy == CrsCachingStrategy.CACHE_ALL_EPSG_CRS_CODES) {
                // The EPSG number below have not been directly used yet in this test method
                // (e.g. as the input coordinate or the output CRS for a transformation)
                // but since everything (according to the above if statement we are now within)
                // should have been cached it should be cached already anyway now
                Assert.IsTrue(
                    cacheTestTransformationAdapterProjNet4GeoAPI.IsEpsgCached(
                        EpsgNumber.SWEDEN__SWEREF99_TM__3006
                    )
                );
            }
            else if(crsCachingStrategy == CrsCachingStrategy.CACHE_EPSG_CRS_CODE_WHEN_FIRST_USED) {
                Assert.IsFalse(
                    cacheTestTransformationAdapterProjNet4GeoAPI.IsEpsgCached(
                        EpsgNumber.SWEDEN__SWEREF99_TM__3006
                    )
                );
                // but after a transformation below it should be cached
                
                cacheTestTransformationAdapter.Transform(
                    cacheTestInputCoordinateWgs84, 
                    EpsgNumber.SWEDEN__SWEREF99_TM__3006
                );

                Assert.IsTrue(
                    cacheTestTransformationAdapterProjNet4GeoAPI.IsEpsgCached(
                        EpsgNumber.SWEDEN__SWEREF99_TM__3006
                    )
                );
            }
        }
    }
}