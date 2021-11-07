using Programmerare.CrsTransformations.Coordinate;

namespace Programmerare.CrsTransformations.Test.CrsTransformations.WktTest.WktTestUsingCsvFile {
    public class CoordinateAndTargetEpsg {
        public CrsCoordinate crsCoordinate;
        public int epsg;
        public CoordinateAndTargetEpsg(CrsCoordinate crsCoordinate, int epsg) {
            this.crsCoordinate = crsCoordinate;
            this.epsg = epsg;
        }
    }
}
