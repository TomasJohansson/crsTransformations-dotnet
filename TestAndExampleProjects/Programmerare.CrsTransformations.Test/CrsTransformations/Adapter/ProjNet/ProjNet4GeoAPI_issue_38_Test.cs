using NUnit.Framework;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v10_036;
using System.Collections.Generic;

namespace Programmerare.CrsTransformations.Adapter.ProjNet {

    [TestFixture]
    class ProjNet4GeoAPI_issue_38_Test
    {
        [Test]
        public void Test_ProjNet4GeoAPI_ProjNet_issue_38() {
            // the purpose of this test was to verify if the reported issue for ProjNet4GeoAPI 1.4 
            // has been fixed for ProjNet 2.0 or if it is still necessary to use an 
            // additional CSV file (EmbeddedResourceFileWithCRSdefinitions) with RT90 definitions as in the test code below

            // https://github.com/NetTopologySuite/ProjNet4GeoAPI/issues/38
            // tested transformation from WGS84 (EPSG 4326) to "RT90 2.5 gon V" (EPSG 3021) ...
            //  input WGS84 coordinate:
            //  Longitude / Latitude: 18.059196 , 59.330231
            //  The expected result should be the following:
            //  X / Y: 1628294 , 6580994
            const double inputLatitudeWgs84 = 59.330231;
            const double inputLongitudeWgs84= 18.059196;
            const double expectedOutput_X_RT90 = 1628294;
            const double expectedOutput_Y_RT90 = 6580994;
            const int epsg_3021_RT9025gonv =  EpsgNumber.SWEDEN__2_5_GON_W__RT90_2_5_GON_V__3021;// RT90 2.5 gon V
            const int epsg_4326_WGS84 =       EpsgNumber.WORLD__WGS_84__4326;

            CrsCoordinate inputCoordinateWgs84 = CrsCoordinateFactory.LatLon(
                inputLatitudeWgs84, 
                inputLongitudeWgs84, 
                epsg_4326_WGS84
            );

            CrsCoordinate expectedCoordinateRT9025gonv =  CrsCoordinateFactory.YX(
                expectedOutput_Y_RT90,
                expectedOutput_X_RT90,
                epsg_3021_RT9025gonv
            );

            ICrsTransformationAdapter crsTransformationAdapter = new CrsTransformationAdapterProjNet(
                CrsCachingStrategy.CACHE_ALL_EPSG_CRS_CODES
                ,
                new SridReader(
                    new List<EmbeddedResourceFileWithCRSdefinitions>{
                        //EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_SHIPPED_WITH_ProjNet4GeoAPI_1_4_1,
                        //// When using ProjNet4GeoAPI version 1.4 it is necessary with the below "patch" file 
                        //// for RT90 but hopefully will not be needed in the next version
                        //EmbeddedResourceFileWithCRSdefinitions.SIX_SWEDISH_RT90_CRS_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml,
                        
                        // Previously (with version 1.4) the above two files were needed but 
                        // with version 2 the test succeeds with the CRS/SRID file whipped with that version
                        EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_SHIPPED_WITH_ProjNet_2_0_0
                    }
                )
            );

            CrsCoordinate actualCoordinateRT9025gonv = crsTransformationAdapter.TransformToCoordinate(
                inputCoordinateWgs84, 
                epsg_3021_RT9025gonv
            );

            const double maxDeltaDifference = 0.2;

            Assert.AreEqual(
                expectedCoordinateRT9025gonv.Y, 
                actualCoordinateRT9025gonv.Y, 
                maxDeltaDifference
            );
            Assert.AreEqual(
                expectedCoordinateRT9025gonv.X, 
                actualCoordinateRT9025gonv.X, 
                maxDeltaDifference
            );
        }
    }
}