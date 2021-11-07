namespace Programmerare.CrsTransformations.Test.CrsTransformations.WktTest.WktTestUsingCsvFile {
// The eight values of an instance of this class is corresponding to
// repeated columns in a csv file described at the top of the file 'TransformResult.cs' 
// and those parts are described at "col 04-" there, and since they are repeated, they 
// are aggregated with the list 'List<TransformResultItem>' in the TransformResult class.
public class TransformResultItem {
    public readonly string epsgOrWkt; // "EPSG" or "WKT"
    public readonly string adapterName; // "DotSpatial" or "ProjNet"

    public readonly bool successTargetCrs; // "T" or "F" in the file
    public readonly double xTargetCrs;
    public readonly double yTargetCrs;

    public readonly bool successWgs84; // "T" or "F" in the file
    public readonly double xWgs84;
    public readonly double yWgs84;
    public TransformResultItem(
        string epsgOrWkt,
        string adapterName,

        bool successTargetCrs,
        double xTargetCrs,
        double yTargetCrs,

        bool successWgs84,
        double xWgs84,
        double yWgs84
    ) {
        this.epsgOrWkt = epsgOrWkt;
        this.adapterName = adapterName;
        this.successTargetCrs = successTargetCrs;
        this.xTargetCrs= xTargetCrs;
        this.yTargetCrs = yTargetCrs;
        this.successWgs84 = successWgs84;
        this.xWgs84 = xWgs84;
        this.yWgs84 = yWgs84;
    }
}
}