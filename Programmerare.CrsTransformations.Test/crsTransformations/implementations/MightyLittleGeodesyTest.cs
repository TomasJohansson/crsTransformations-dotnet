using System;
using System.Collections.Generic;
using System.Text;
using com.programmerare.crsTransformations;
using com.programmerare.crsTransformations.coordinate;
using com.programmerare.crsTransformations.crsIdentifier;
using NUnit.Framework;

namespace Programmerare.CrsTransformations.Test.crsTransformations.implementations
{
    [TestFixture]
    class MightyLittleGeodesyTest
    {
        [Test]
        public void transform_fromWgs84_toSweref99()
        {
            // These coordinate should be corresponding:
            double wgs84Lat = 59.330231;
            double wgs84Lon = 18.059196;
            double sweref99Y = 6580822;
            double sweref99X = 674032;
            int epsgWGS84 = 4326;
            int epsgSweref99 = 3006;
            CrsCoordinate coordinateWgs84 = CrsCoordinateFactory.LatLon(wgs84Lat, wgs84Lon, epsgWGS84);
            CrsTransformationAdapter crsTransformationAdapter = new CrsTransformationAdapterMightyLittleGeodesy();
            CrsCoordinate coordinateSweref99 = crsTransformationAdapter.TransformToCoordinate(coordinateWgs84, epsgSweref99);
            Assert.IsNotNull(coordinateSweref99);
            double maxDifferenceForSuccessfulTest = 0.5; // 0.5 meter
            Assert.AreEqual(sweref99Y, coordinateSweref99.Y, maxDifferenceForSuccessfulTest);
            Assert.AreEqual(sweref99X, coordinateSweref99.X, maxDifferenceForSuccessfulTest);
        }
    }
}
