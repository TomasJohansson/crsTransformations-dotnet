using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.Identifier;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Adapter.ProjNet;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using Programmerare.CrsTransformations.CompositeTransformations;

namespace Programmerare.CrsTransformations.Test.CrsTransformations.WktTest.WktTestUsingCsvFile {

// This class iterates a CSV file that should have been created by the class 'FileGeneratorForCsvFileWithWktResults'
// as long as the results are 'good' (thwe results are sorted with the best first).
// The above mentioned class that generated the CSV file were using individual CRS adapters
// and also different methods for creating CRS identifier (by EPSG or by WKT)
// and compared the results, and the "best" results (sorted first) are those that succeed 
// for all combinations of adapters/identifiers, and also small differences in the calculated values.

// This test class below is instead using a composite adapter (median value) 
// and verify that 2 of the 3 adapters succeds, amd that the result is 'reliable'
// in the sense that the differences are small (i.e. the differences in the values by the leaf adapters used within the composite)

[TestFixture]
class CrsTransformationAdapterTransformationResultTest {

    [Test]
    public void TestCsvFileWithTransformResultsForBothEpsgAndWkt() {
        var allCrsAdapters = new List<ICrsTransformationAdapter>(){
            new CrsTransformationAdapterProjNet(),
            new CrsTransformationAdapterDotSpatial(),
            new CrsTransformationAdapterMightyLittleGeodesy() // this adapter does not support WKT but is used to show it does not throw exception
        };
        var crsAdapter = CrsTransformationAdapterCompositeFactory.Create().CreateCrsTransformationMedian(allCrsAdapters);

        var file = FileGeneratorForCsvFileWithWktResults.Get_file_with_results_sorted_with_best_results_first();
        Assert.IsTrue(file.Exists, "Generate the file first with the class " + nameof(FileGeneratorForCsvFileWithWktResults) + " and you may need to activate it by disabling a [Ignore] attribute at the class declaration for that file");
        var lines = File.ReadAllLines(file.FullName, Encoding.UTF8).ToList();
        Assert.That(lines.Count, Is.GreaterThan(5000)); // 5190
        var transformResults = lines.Select(line => TransformResult.CreateFromRowInFile(line));
        double maxDeltaTargetEpsg = 0.01;
        double maxDeltaWgs84 = 0.01; // when transforming back

        // only iterate the first "best" results from the above file, regarding the max difference when caculating results with different combinations of adapters/identifiers
        transformResults = transformResults.Where(
            item => item.diffMaxTargetCrsExists 
            && item.xDiffMaxTargetCrs < maxDeltaTargetEpsg 
            && item.yDiffMaxTargetCrs < maxDeltaTargetEpsg
        );
        Assert.That(transformResults .Count, Is.GreaterThan(2200)); // 2247

        int rowCount = 0;
        foreach(var resultFromFile in transformResults) {
            rowCount++;
            string wkt = resultFromFile.wkt;
            var crs = CrsIdentifierFactory.CreateFromWktCrs(wkt);
            var coord = CrsCoordinateFactory.LonLat(resultFromFile.xCentroidOfEpsgArea, resultFromFile.yCentroidOfEpsgArea);
            var resultTargetCrs = crsAdapter.Transform(coord, crs);
            string message = "problem row : " + rowCount;
            Assert.IsTrue(resultTargetCrs.isSuccess, message);
            Assert.IsTrue(resultTargetCrs.IsReliable(2, maxDeltaTargetEpsg), message);
            // Below transforming back to WGS84
            var resultWgs84 = crsAdapter.Transform(resultTargetCrs.outputCoordinate, CrsCoordinateFactory.COORDINATE_REFERENCE_SYSTEM_WGS84);
            Assert.IsTrue(resultWgs84.isSuccess, message);
            Assert.IsTrue(resultWgs84.IsReliable(2, maxDeltaWgs84), message);
        }
    }

} // class
} // namespace