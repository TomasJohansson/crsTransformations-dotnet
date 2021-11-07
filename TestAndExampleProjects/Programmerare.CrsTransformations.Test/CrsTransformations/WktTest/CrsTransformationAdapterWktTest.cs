using NUnit.Framework;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using Programmerare.CrsTransformations.Adapter.ProjNet;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.Identifier;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Programmerare.CrsTransformations.Test.CrsTransformations.WktTest {
    
    [TestFixture]
    class CrsTransformationAdapterWktTest {

        private static readonly int _epsgWgs84 = CrsCoordinateFactory.COORDINATE_REFERENCE_SYSTEM_WGS84.EpsgNumber;

        private static ICrsTransformationAdapter _crsDotSpatial;
        private static ICrsTransformationAdapter _crsProjNet;
        private static ICrsTransformationAdapter _crsMightyLittleGeodesy;

        private static int epsgNumberForSweref99tm = 3006; // SWEREF99 TM
        // The below Wkt-Crs string is copied from the file "... crsTransformations-dotnet\Programmerare.CrsTransformations.Adapter.ProjNet\SRID_ShippedWithProjNet_2_0_0.csv"
        private static string wktCrsForSweref99tm = "PROJCS[\"SWEREF99 TM\",GEOGCS[\"SWEREF99\",DATUM[\"SWEREF99\",SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",\"6619\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4619\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",15],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AUTHORITY[\"EPSG\",\"3006\"]]";

        private static CrsCoordinate coordinateWgs84ForCentroidOfSweref99tm;

        [OneTimeSetUp] // https://docs.nunit.org/articles/nunit/writing-tests/attributes/onetimesetup.html
        public static void SetUp() {
            _crsDotSpatial = new CrsTransformationAdapterDotSpatial();
            _crsProjNet = new CrsTransformationAdapterProjNet();    
            _crsMightyLittleGeodesy = new CrsTransformationAdapterMightyLittleGeodesy();

            // The below used coordinate is copied from the file "... crsTransformations-dotnet\TestAndExampleProjects\Programmerare.CrsTransformations.Test\resources\generated\CoordinateTestDataGeneratedFromEpsgDatabase.csv"
            // and has been calculated as the centroid coordinate for the area covered by EPSG 3006
            // (by another project of mine, https://github.com/TomasJohansson/crsTransformations  , crsTransformations/crs-transformation-code-generation/src/main/kotlin/com/programmerare/crsCodeGeneration/coordinateTestDataGenerator/CoordinateTestDataGenerator.kt )
            coordinateWgs84ForCentroidOfSweref99tm = CrsCoordinateFactory.LatLon(
                61.98770256318016,  // WGS84 Latitude for the centroid of the area for SWEREF99 TM (EPSG 3006)
                17.083659606206545, // WGS84 Longitude for the centroid of the area for SWEREF99 TM (EPSG 3006)
                _epsgWgs84
            );
        }

        [Test]
        public void Test_the_same_result_when_using_epsg_or_wkt() {
            CrsIdentifier crsEpsg = CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumberForSweref99tm);
            CrsIdentifier crsWkt = CrsIdentifierFactory.CreateFromWktCrs(wktCrsForSweref99tm);

            var resultDotSpatialEpsg    = _crsDotSpatial.Transform(coordinateWgs84ForCentroidOfSweref99tm, crsEpsg);
            var resultDotSpatialWkt     = _crsDotSpatial.Transform(coordinateWgs84ForCentroidOfSweref99tm, crsWkt);
            var resultProjNetEpsg       = _crsProjNet.Transform(coordinateWgs84ForCentroidOfSweref99tm, crsEpsg);
            var resultProjNetWkt        = _crsProjNet.Transform(coordinateWgs84ForCentroidOfSweref99tm, crsWkt);
            this.AssertResults(
                new List<CrsTransformationResult>{resultDotSpatialEpsg, resultDotSpatialWkt, resultProjNetEpsg, resultProjNetWkt},
                delta: 0.004 // the precision is within 4 mm since meters is the unit for SWEREF99 TM (EPSG 3006)
            );

            // Above: the original coordinate (specified with CRS WGS84) was transformed to EPSG 3006
            //          with different adapter implementations and different CrsIdentifier objects
            //          (created either from EPSG number or WKT string)
            // Below: the above resulting EPSG 3006 coordinates are transformed back to WGS84

            var resultDotSpatialEpsg2   = _crsDotSpatial.Transform(resultDotSpatialEpsg.outputCoordinate, _epsgWgs84);
            var resultDotSpatialWkt2    = _crsDotSpatial.Transform(resultDotSpatialWkt.outputCoordinate, _epsgWgs84);
            var resultProjNetEpsg2      = _crsProjNet.Transform(resultProjNetEpsg.outputCoordinate, _epsgWgs84);
            var resultProjNetWkt2       = _crsProjNet.Transform(resultProjNetWkt.outputCoordinate, _epsgWgs84);
            this.AssertResults(
                new List<CrsTransformationResult>{resultDotSpatialEpsg2, resultDotSpatialWkt2, resultProjNetEpsg2, resultProjNetWkt2},
                delta: 0.0000001
            );
        }

        private void AssertResults(
            List<CrsTransformationResult> crsTransformationResults,
            double delta
        ) {
            foreach(var result in crsTransformationResults) {
                Assert.IsTrue(result.isSuccess, result.crsTransformationAdapterResultSource.ShortNameOfImplementation);
            }
            var coordinates = crsTransformationResults.Select(r => r.outputCoordinate).ToList();
            for(int i=0; i<coordinates.Count-1; i++) {
                for(int j=i+1; j<coordinates.Count; j++) {
                    var coord1 = coordinates[i];
                    var coord2 = coordinates[j];
                    Assert.AreEqual(coord1.X, coord2.X, delta);
                    Assert.AreEqual(coord1.Y, coord2.Y, delta);
                    //Console.WriteLine("has now compared " + coord1 + " with " + coord2);
                }
            }
        }


        [Test]
        public void Test_adapter_not_supporting_WKT() {
            // MightyLittleGeodesy does not support WKT
            CrsIdentifier crsWkt = CrsIdentifierFactory.CreateFromWktCrs(wktCrsForSweref99tm);
            var result = _crsMightyLittleGeodesy.Transform(coordinateWgs84ForCentroidOfSweref99tm, crsWkt);
            // MightyLittleGeodesy does NOT support WKT-CRS, so the below should be false (but an exception should not be thrown)
            Assert.IsFalse(result.isSuccess);

            // The avove Transform method should never throw an exception 
            // but instead return an object with an isSuccess property as above,
            // but the method TransformToCoordinate (tested below) should throw an exception
            // since the CrsIdentifier created from WktCrs is NOT supported
            Assert.That(
                () => _crsMightyLittleGeodesy.TransformToCoordinate(coordinateWgs84ForCentroidOfSweref99tm, crsWkt)
                ,
                Throws.Exception 
                    .TypeOf<ArgumentException>()
            );
        }
    }

}