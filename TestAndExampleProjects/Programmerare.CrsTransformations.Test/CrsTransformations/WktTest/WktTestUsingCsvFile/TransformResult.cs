using System.Linq;
using System.Collections.Generic;
using System.Text;
using System;

namespace Programmerare.CrsTransformations.Test.CrsTransformations.WktTest.WktTestUsingCsvFile {
// The csv file described below is created with the class "FileGeneratorForCsvFileWithWktResults"
// and it can then be read from the class 'CrsTransformationAdapterTransformationResultTest'.
// (the csv files, currently two files with the same content/rows, but sorted differently, are created into the following diretory: Programmerare.CrsTransformations.Test\resources\wkt_transformation_results )

// One instance of this below class 'TransformResult' represents the data in a row of the csv file, as described below:
// The "outer/main" columns in the produced CSV file are separated with | and some inner columns are separated with ;
//      The coordinates in this file is always in the order x;y  i.e. separated with a colon
//  col 01: an EPSG number
//  col 02: the original coordinate which should be a centroid for the EPSG area in the above col 01
//            which is defined with the CRS WGS84 (i.e. NOT with the system defined by the above EPSG and the WKT in the last column)
//          (the x;y coordinate numbers are separated with a colon, and in the order x;y)
//  col 03: six semi-colon separated columns with diff-values as described below:
//              1: "T" or "F" (True or False). If "T" then there should be a "T" within all "pipe-columns" (the below outer columns 04+)
//                  i.e. in the "inner-column-3" values, but if any of those are "F" it means that 
//                  something failed and there is a missing value, and then the below column 2-3 (and also 5-6) should be empty
//              2:  max-X-diff between the "inner-column-4" values in "PIPE-column 04+" (i.e. column 04 and later below, except the very last column)
//                      (this is the maximal difference between any of the resulting transformed values when transforming to the EPSG/WKT defined CRS)
//              3:  max-Y-diff (same as above but for Y instead of X) i.e. the "inner-column-5" values in outer col 04+ below
//              4: "T" or "F" (True or False). If "T" then there should be a "T" within all "pipe-columns" (outer column 04+ below)
//                  i.e. in the "inner-column-6" values, but if any of those are "F" it means that 
//                  something failed and there is a missing value, and then the below column 5-6 should be empty
//              5:  max-X-diff between the "inner-column-7" values in "PIPE-column 04+" (i.e. column 04 and later below, except the very last column)
//                      (this is the maximal difference between any of the resulting transformed values when transforming back to WGS84 from the EPSG/WKT defined CRS)
//              6:  max-Y-diff (same as above but for Y instead of X) i.e. the "inner-column-8" values in outer col 04+ below
//  col 04- and each of the rest of the (outer PIPE-separated) columns (except for the very last column )
//          are all containg a semi-colon separated list with these items and in the below order:
//              1:  "EPSG" or "WKT" depending on if this (within this outer "pipe-column") item originated from EPSG or WKT
//              2:  a string describing which adapter was used for these transformations (within this outer "pipe-column") for example "DotSpatial" or "ProjNet"
//              3:  "T" or "F" (True or False), should be True if there was a "success" i.e. a value in the below column 4-5
//                  and if it is "F" then the below columns 4-5 should not have any values i.e. empty string
//              4-5:    the  x;y  coordinate for the transformation to the target defined with either EPSG or WKT
//              6:  "T" or "F" (True or False), should be True if there was a "success" i.e. a value in the below column 7-8
//                  and if it is "F" then the below columns 7-8 should not have any values i.e. empty string
//              7-8:    the  x;y  (long;lat) coordinate for the transformation back to WGS85 from the above coordinate
//              Note that the above 8 'inner columns' values separated with ";" are repeated, and each 
//              such group of eight values are corresponding to an instance of the class 'TransformResultItem'.
//  col LAST:  the very last column should contain the WKT-CRS string
public class TransformResult {
    public int epsgNumber;
    public double xCentroidOfEpsgArea;
    public double yCentroidOfEpsgArea;
    
    public bool diffMaxTargetCrsExists;
    public double xDiffMaxTargetCrs = 0.0;
    public double yDiffMaxTargetCrs = 0.0;

    // the variables below are used when transforming back to Wgs84 (from the above target CRS)
    public bool diffMaxWgs84Exists;
    public double xDiffMaxWgs84 = 0.0; 
    public double yDiffMaxWgs84 = 0.0;

    public List<TransformResultItem> transformResultItems;

    public string wkt;
    private TransformResult(
        int epsgNumber,
        double xCentroidOfEpsgArea,
        double yCentroidOfEpsgArea,
        List<TransformResultItem> transformResultItems,
        string wkt

    ) {
        this.epsgNumber = epsgNumber;
        this.xCentroidOfEpsgArea = xCentroidOfEpsgArea;
        this.yCentroidOfEpsgArea = yCentroidOfEpsgArea;
        this.transformResultItems = transformResultItems;
        this.wkt = wkt;
    }

    public static TransformResult Create(
        int epsgNumber,
        double xCentroidOfEpsgArea,
        double yCentroidOfEpsgArea,
        List<TransformResultItem> transformResultItems,
        string wkt
    ) {
        var res = new TransformResult(
            epsgNumber,
            xCentroidOfEpsgArea,
            yCentroidOfEpsgArea,
            transformResultItems,
            wkt
        );

        res.diffMaxTargetCrsExists = transformResultItems.Count(item => !item.successTargetCrs) == 0;
        res.diffMaxWgs84Exists = transformResultItems.Count(item => !item.successWgs84) == 0;

        if(res.diffMaxTargetCrsExists) {
            var xTargetCrs = transformResultItems.Select(item => item.xTargetCrs);
            double xTargetCrsMin = xTargetCrs.Min();
            double xTargetCrsMax = xTargetCrs.Max();
            res.xDiffMaxTargetCrs = Math.Abs(xTargetCrsMin - xTargetCrsMax);

            var yTargetCrs = transformResultItems.Select(item => item.yTargetCrs);
            double yTargetCrsMin = yTargetCrs.Min();
            double yTargetCrsMax = yTargetCrs.Max();
            res.yDiffMaxTargetCrs = Math.Abs(yTargetCrsMin - yTargetCrsMax);
        }
        if(res.diffMaxWgs84Exists) {
            var xWgs84 = transformResultItems.Select(item => item.xWgs84);
            double xWgs84Min = xWgs84.Min();
            double xWgs84Max = xWgs84.Max();
            res.xDiffMaxWgs84 = Math.Abs(xWgs84Min- xWgs84Max);

            var yWgs84 = transformResultItems.Select(item => item.yWgs84);
            double yWgs84Min = yWgs84.Min();
            double yWgs84Max = yWgs84.Max();
            res.yDiffMaxWgs84 = Math.Abs(yWgs84Min- yWgs84Max);
        }
        return res;
    }

    private const string PIPE = "|";
    private const string SEMICOLON = ";";

    private static Tuple<double, double> GetTwoDoubles(string s) {
        var array = s.Split(SEMICOLON);
        return Tuple.Create( double.Parse(array[0]), double.Parse(array[1]) );
    }

    private static bool GetAsBool(string s) {
        return s.ToUpper().StartsWith("T") ? true : false;
    }
    char GetBoolFirstLetter(bool b)
    {
        return b.ToString().ToUpper().First();
    }

    private static Tuple<bool, double, double> GetOneBooleanAndTwoDoubles(string s) {
        var array = s.Split(SEMICOLON);
        return Tuple.Create(GetAsBool(array[0]), double.Parse(array[1]), double.Parse(array[2]) );
    }

    public static TransformResult CreateFromRowInFile(string line) {
        var array = line.Split('|');
        int epsgNumber = int.Parse(array[0]);
        
        var centroidOfEpsgArea = GetTwoDoubles(array[1]); // x and y separated with ;
        double xCentroidOfEpsgArea = centroidOfEpsgArea.Item1;
        double yCentroidOfEpsgArea = centroidOfEpsgArea.Item2;

        var diffMaxTargetCrs = GetOneBooleanAndTwoDoubles(array[2]); // three parts separated with ;
        var diffMaxWgs84 = GetOneBooleanAndTwoDoubles(array[3]); // three parts separated with ;

        var transformResultItems = new List<TransformResultItem>();
        for(int i=4; i<array.Length-1; i++) {
            string transformResultItemAsString = array[i];
            var arr = transformResultItemAsString.Split(SEMICOLON);
            string epsgOrWkt = arr[0];
            string adapterName = arr[1];

            bool successTargetCrs = GetAsBool(arr[2]);
            double xTargetCrs = double.Parse(arr[3]);
            double yTargetCrs = double.Parse(arr[4]);

            bool successWgs84 = GetAsBool(arr[5]);
            double xWgs84 = double.Parse(arr[6]);
            double yWgs84 = double.Parse(arr[7]);
            var item = new TransformResultItem(
                epsgOrWkt,
                adapterName,
                successTargetCrs,
                xTargetCrs,
                yTargetCrs,
                successWgs84,
                xWgs84,
                yWgs84
            );
            transformResultItems.Add(item);
        }
        string wkt = array[array.Length-1];
        var res = new TransformResult(
            epsgNumber,
            xCentroidOfEpsgArea,
            yCentroidOfEpsgArea,
            transformResultItems,
            wkt
        );
        
        res.diffMaxTargetCrsExists = diffMaxTargetCrs.Item1;
        res.xDiffMaxTargetCrs= diffMaxTargetCrs.Item2;
        res.yDiffMaxTargetCrs= diffMaxTargetCrs.Item3;

        res.diffMaxWgs84Exists = diffMaxWgs84.Item1;
        res.xDiffMaxWgs84 = diffMaxWgs84.Item2;
        res.yDiffMaxWgs84 = diffMaxWgs84.Item3;

        return res;
    }
    public string GetAsRowForFile() {
        var sb = new StringBuilder();
        sb.Append(epsgNumber).Append(PIPE);
        
        sb.Append(xCentroidOfEpsgArea).Append(SEMICOLON);
        sb.Append(yCentroidOfEpsgArea).Append(PIPE);

        sb.Append(GetBoolFirstLetter(diffMaxTargetCrsExists)).Append(SEMICOLON);
        sb.Append(xDiffMaxTargetCrs).Append(SEMICOLON);
        sb.Append(yDiffMaxTargetCrs).Append(PIPE);

        sb.Append(GetBoolFirstLetter(diffMaxWgs84Exists)).Append(SEMICOLON);
        sb.Append(xDiffMaxWgs84).Append(SEMICOLON);
        sb.Append(yDiffMaxWgs84).Append(PIPE);

        foreach(var res in transformResultItems) {
            sb.Append(res.epsgOrWkt).Append(SEMICOLON);
            sb.Append(res.adapterName).Append(SEMICOLON);
            sb.Append(GetBoolFirstLetter(res.successTargetCrs)).Append(SEMICOLON);
            sb.Append(res.xTargetCrs).Append(SEMICOLON);
            sb.Append(res.yTargetCrs).Append(SEMICOLON);
            sb.Append(GetBoolFirstLetter(res.successWgs84)).Append(SEMICOLON);
            sb.Append(res.xWgs84).Append(SEMICOLON);
            sb.Append(res.yWgs84).Append(SEMICOLON);
            sb.Append(PIPE);
        }
        
        sb.Append(wkt);
        return sb.ToString();
    }

}
}
