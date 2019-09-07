using Programmerare.CrsTransformations.Adapter.ProjNet;
using NUnit.Framework;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_7;
using System.Collections.Generic;

namespace Programmerare.CrsTransformations.Adapter.ProjNet {

    
    [TestFixture]
    class ProjNetTest : AdaptersTestBase {

        private CrsTransformationAdapterProjNet crsTransformationAdapterProjNetwithNoConstructorParameters;
        
        [SetUp]
        public void SetUp() {
            crsTransformationAdapterProjNetwithNoConstructorParameters = new CrsTransformationAdapterProjNet();

            base.SetUpbase(
                crsTransformationAdapterProjNetwithNoConstructorParameters,

                CrsTransformationAdapteeType.LEAF_PROJ_NET_2_0_0,

                // TODO: Obsolete comment maybe after upgrade to 2.0.0:
                // The implementation ProjNet
                // is currently (version 1.4.1) 
                // producing bad results when transforming 
                // to the Swedish CRS "RT90 2.5 gon V"
                // https://github.com/NetTopologySuite/ProjNet/issues/38
                // However, the results for those CRS have been improved by 
                // using other CRS definitions for those CRS.
                // See further comments in the file/type EmbeddedResourceFileWithCRSdefinitions 
                // in the project Programmerare.CrsTransformations.Adapter.ProjNet

                0.5, // maxMeterDifferenceForSuccessfulTest
                0.00001 // maxLatLongDifferenceForSuccessfulTest
            );

            // The implementation ProjNet
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
            
            cacheTestTransformationAdapterProjNet = new CrsTransformationAdapterProjNet();
        }

        private CrsCoordinate cacheTestInputCoordinateWgs84;
        private int cacheTestForOutputEpsgNumberNotExisting;
        private CrsTransformationAdapterProjNet cacheTestTransformationAdapterProjNet;

        [Test]
        public void TestCachingWhenAllEpsgCodesAreCachedInOneReadOfTheCsvFile() {
            VerifyExpectedCachingBehaviour(
                CrsCachingStrategy.CACHE_ALL_EPSG_CRS_CODES
            );
        }
        
        [Test]
        public void TestCachingWhenEpsgCodesAreCachedAtFirstRequest() {
            VerifyExpectedCachingBehaviour(
                CrsCachingStrategy.CACHE_EPSG_CRS_CODE_WHEN_FIRST_USED
            );
        }
        
        [Test]
        public void TestBehaviourWhenNoCachingIsChosen() {
            cacheTestTransformationAdapterProjNet  = new CrsTransformationAdapterProjNet(CrsCachingStrategy.NO_CACHING);
            Assert.IsFalse(
                cacheTestTransformationAdapterProjNet.IsEpsgCached(
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );
            Assert.IsFalse(
                cacheTestTransformationAdapterProjNet.IsEpsgCached(
                    EpsgNumber.SWEDEN__SWEREF99_TM__3006
                )
            );

            // The transform method below may potentially trigger 
            // the used epsg code (the second parameter) to become cached
            // (but that behaviour depends on the caching strategy)
            cacheTestTransformationAdapterProjNet.Transform(
                cacheTestInputCoordinateWgs84, 
                cacheTestForOutputEpsgNumberNotExisting
            );
            // The above output CRS was a non-existing EPSG number
            // but below is an existing EPSG number used.
            cacheTestTransformationAdapterProjNet.Transform(
                cacheTestInputCoordinateWgs84, 
                EpsgNumber.SWEDEN__SWEREF99_TM__3006
            );

            // Neither of the above are still not cached since caching is disabled
            Assert.IsFalse(
                cacheTestTransformationAdapterProjNet.IsEpsgCached(
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );
            Assert.IsFalse(
                cacheTestTransformationAdapterProjNet.IsEpsgCached(
                    EpsgNumber.SWEDEN__SWEREF99_TM__3006
                )
            );
        }        

        private void VerifyExpectedCachingBehaviour(
            CrsCachingStrategy crsCachingStrategy
        ) {
            cacheTestTransformationAdapterProjNet  = new CrsTransformationAdapterProjNet(crsCachingStrategy);
            Assert.IsFalse(
                cacheTestTransformationAdapterProjNet.IsEpsgCached(
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );

            // The transform method below may potentially trigger 
            // the used epsg code (the second parameter) to become cached
            // (but that behaviour depends on the caching strategy 
            //  i.e. it must not be NO_CACHING)
            cacheTestTransformationAdapterProjNet.Transform(
                cacheTestInputCoordinateWgs84, 
                cacheTestForOutputEpsgNumberNotExisting
            );

            // It should be true below.
            // Indeed the lookup is cached even though it is a non-existin
            // EPSG number (i.e. "null" is cached)
            // since you do likely not want to read the file 
            // many times (if you have specified that you want caching)
            // just to get null again, so therefore the null value 
            // is also cached as a kind of value in the semantic of the below method
            Assert.IsTrue(
                cacheTestTransformationAdapterProjNet.IsEpsgCached(
                    cacheTestForOutputEpsgNumberNotExisting
                )
            );

            if(crsCachingStrategy == CrsCachingStrategy.CACHE_ALL_EPSG_CRS_CODES) {
                // The EPSG number below have not been directly used yet in this test method
                // (e.g. as the input coordinate or the output CRS for a transformation)
                // but since everything (according to the above if statement we are now within)
                // should have been cached it should be cached already anyway now
                Assert.IsTrue(
                    cacheTestTransformationAdapterProjNet.IsEpsgCached(
                        EpsgNumber.SWEDEN__SWEREF99_TM__3006
                    )
                );
            }
            else if(crsCachingStrategy == CrsCachingStrategy.CACHE_EPSG_CRS_CODE_WHEN_FIRST_USED) {
                Assert.IsFalse(
                    cacheTestTransformationAdapterProjNet.IsEpsgCached(
                        EpsgNumber.SWEDEN__SWEREF99_TM__3006
                    )
                );
                // but after a transformation below it should be cached
                
                cacheTestTransformationAdapterProjNet.Transform(
                    cacheTestInputCoordinateWgs84, 
                    EpsgNumber.SWEDEN__SWEREF99_TM__3006
                );

                Assert.IsTrue(
                    cacheTestTransformationAdapterProjNet.IsEpsgCached(
                        EpsgNumber.SWEDEN__SWEREF99_TM__3006
                    )
                );
            }
        }

        [Test]
        public void AdapterInstance_ShouldBeConstructedUsingCachingAndStandardFileShippedWithProjnet_2_0_0_WhenUsingConstructorWithoutParameters() {
            // The first adapter below (i.e. the "expected") is 
            // used in the SetUp method, and is constructed without any parameters.
            // The purpose of this test is to illustrate which default values are being used by that constructor
            // i.e. the same as in the second parameter below 
            // i.e. the "actual" value in the method "AreEqual".
            Assert.AreEqual(
                this.crsTransformationAdapterProjNetwithNoConstructorParameters
                ,
                new CrsTransformationAdapterProjNet(
                    CrsCachingStrategy.CACHE_ALL_EPSG_CRS_CODES
                    ,
                    new SridReader(new List<EmbeddedResourceFileWithCRSdefinitions>{
                        //EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_SHIPPED_WITH_ProjNet,
                        //EmbeddedResourceFileWithCRSdefinitions.SIX_SWEDISH_RT90_CRS_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml,
                        // The above had to be used before (with version 1.4) since RT90 were not correctly defined , https://github.com/NetTopologySuite/ProjNet4GeoAPI/issues/38
                        EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_SHIPPED_WITH_ProjNet_2_0_0
                    })
                )
            );

            // The below test is just showing that when switching the order of 
            // the embedded resource files (which is the only different compared to the above code)
            // then they are NOT equal
            Assert.AreNotEqual(
                this.crsTransformationAdapterProjNetwithNoConstructorParameters
                ,
                new CrsTransformationAdapterProjNet(
                    CrsCachingStrategy.CACHE_ALL_EPSG_CRS_CODES
                    ,
                    new SridReader(new List<EmbeddedResourceFileWithCRSdefinitions>{
                        EmbeddedResourceFileWithCRSdefinitions.SIX_SWEDISH_RT90_CRS_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml,
                        EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_SHIPPED_WITH_ProjNet
                    })
                )
            );
        }

    }

}