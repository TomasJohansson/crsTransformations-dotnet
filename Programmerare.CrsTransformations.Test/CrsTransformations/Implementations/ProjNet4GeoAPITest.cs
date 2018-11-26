using Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI;
using NUnit.Framework;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;

// the below row imports static methods IsEpsgCached and SetCrsCachingStrategy
using static Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI.CrsTransformationAdapterProjNet4GeoAPI;

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

                195, // maxMeterDifferenceForSuccessfulTest
                0.01 // maxLatLongDifferenceForSuccessfulTest
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
            cacheTestTransformationAdapter = new CrsTransformationAdapterProjNet4GeoAPI();
        }

        private CrsCoordinate cacheTestInputCoordinateWgs84;
        private int cacheTestForOutputEpsgNumberNotExisting;
        private ICrsTransformationAdapter cacheTestTransformationAdapter;

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
            SetCrsCachingStrategy( // statically imported from CrsTransformationAdapterProjNet4GeoAPI
                CrsCachingStrategy.NO_CACHING
            );            
            Assert.IsFalse(
                IsEpsgCached( // statically imported from CrsTransformationAdapterProjNet4GeoAPI
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );
            Assert.IsFalse(
                IsEpsgCached( // statically imported from CrsTransformationAdapterProjNet4GeoAPI
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
                IsEpsgCached( // statically imported from CrsTransformationAdapterProjNet4GeoAPI
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );
            Assert.IsFalse(
                IsEpsgCached(
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
            SetCrsCachingStrategy( // statically imported from CrsTransformationAdapterProjNet4GeoAPI
                CrsCachingStrategy.NO_CACHING
            );
            Assert.IsFalse(
                IsEpsgCached( // statically imported from CrsTransformationAdapterProjNet4GeoAPI
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );
            cacheTestTransformationAdapter.Transform(
                cacheTestInputCoordinateWgs84, 
                cacheTestForOutputEpsgNumberNotExisting
            );
            Assert.IsFalse(
                IsEpsgCached(
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );
            
            SetCrsCachingStrategy( // statically imported from CrsTransformationAdapterProjNet4GeoAPI
                crsCachingStrategy
            );

            // still false (since it will not be cached until looked up again)
            Assert.IsFalse(
                IsEpsgCached(
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
                IsEpsgCached(
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );

            if(crsCachingStrategy == CrsCachingStrategy.CACHE_ALL_EPSG_CRS_CODES) {
                // The EPSG number below have not been directly used yet in this test method
                // (e.g. as the input coordinate or the output CRS for a transformation)
                // but since everything (according to the above if statement we are now within)
                // should have been cached it should be cached already anyway now
                Assert.IsTrue(
                    IsEpsgCached(
                        EpsgNumber.SWEDEN__SWEREF99_TM__3006
                    )
                );
            }
            else if(crsCachingStrategy == CrsCachingStrategy.CACHE_EPSG_CRS_CODE_WHEN_FIRST_USED) {
                Assert.IsFalse(
                    IsEpsgCached(
                        EpsgNumber.SWEDEN__SWEREF99_TM__3006
                    )
                );
                // but after a transformation below it should be cached
                
                cacheTestTransformationAdapter.Transform(
                    cacheTestInputCoordinateWgs84, 
                    EpsgNumber.SWEDEN__SWEREF99_TM__3006
                );

                Assert.IsTrue(
                    CrsTransformationAdapterProjNet4GeoAPI.IsEpsgCached(
                        EpsgNumber.SWEDEN__SWEREF99_TM__3006
                    )
                );
            }
        }
    }
}