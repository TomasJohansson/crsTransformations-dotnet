using NUnit.Framework;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v10_036;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet;

namespace Programmerare.CrsTransformations.Test.GitHub_issues {
    class TestGitHubIssues {
        
        [Test]
        public void issue_1() {
            // https://github.com/TomasJohansson/crsTransformations-dotnet/issues/1

            int epsgWgs84 = EpsgNumber.WORLD__WGS_84__4326;
            int epsg28406 = EpsgNumber.EUROPE__FSU_ONSHORE_30_E_TO_36_E__PULKOVO_1942__GAUSS_KRUGER_ZONE_6__28406;
            
            CrsCoordinate inputCoordinate = CrsCoordinateFactory.LatLon(59.991230000, 30.754563333, epsgWgs84);
            // epsg.io gives 6374811.15 and 6655337.15
            // https://epsg.io/transform#s_srs=4326&t_srs=28406&x=30.754563333&y=59.991230000
            CrsCoordinate expectedOutputCoordinate = CrsCoordinateFactory.LatLon(6655337.15, 6374811.15, epsg28406);

            var crsTransformationAdapterProjNet     = new CrsTransformationAdapterProjNet();
            var crsTransformationAdapterDotSpatial  = new CrsTransformationAdapterDotSpatial();

            CrsCoordinate outputCoordinateProjNet   = crsTransformationAdapterProjNet.TransformToCoordinate(inputCoordinate, epsg28406);
            CrsCoordinate outputCoordinateDotSpatial= crsTransformationAdapterDotSpatial.TransformToCoordinate(inputCoordinate, epsg28406);

            double delta = 0.01;

            Assert.AreEqual(expectedOutputCoordinate.Longitude, outputCoordinateDotSpatial.Longitude, delta);
            Assert.AreEqual(expectedOutputCoordinate.Latitude, outputCoordinateDotSpatial.Latitude, delta);

            Assert.AreEqual(expectedOutputCoordinate.Longitude, outputCoordinateProjNet.Longitude, delta);
            Assert.AreEqual(expectedOutputCoordinate.Latitude, outputCoordinateProjNet.Latitude, delta);
        }
    }
}