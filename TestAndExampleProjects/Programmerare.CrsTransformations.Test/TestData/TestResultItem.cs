using System;
using Programmerare.CrsTransformations.Coordinate;

namespace Programmerare.CrsTransformations.TestData
{

public class TestResultItem {
    private string wgs84sourceX , wgs84sourceY , epsgCrsCode;
    private string epsgTargetSourceX = "", epsgTargetSourceY = "", wgs84targetX = "", wgs84targetY = "";
    private const char SEPARATOR_CHARACTER = '|';
    private readonly static string SEPARATOR = "" + SEPARATOR_CHARACTER;

    public string GetResultStringForRegressionFile() {
        return
            wgs84sourceX + SEPARATOR +
            wgs84sourceY + SEPARATOR +
            epsgCrsCode + SEPARATOR +
            epsgTargetSourceX + SEPARATOR +
            epsgTargetSourceY + SEPARATOR +
            wgs84targetX + SEPARATOR +
            wgs84targetY;
    }

    public TestResultItem(string lineFromRow) {
        string[] array = lineFromRow.Split(SEPARATOR_CHARACTER);
        if(array.Length < 2) throw new Exception("Too short array for lineFromRow: " + lineFromRow);
        this.wgs84sourceX = array[0];
        this.wgs84sourceY = array[1];
        this.epsgCrsCode = array[2];
        if(array.Length > 4) {
            this.epsgTargetSourceX = array[3];
            this.epsgTargetSourceY = array[4];
            if(array.Length > 6) {
                this.wgs84targetX = array[5];
                this.wgs84targetY = array[6];
            }
        }
    }

    public TestResultItem(
        EpsgCrsAndAreaCodeWithCoordinates item,
        CrsCoordinate inputCoordinateWGS84,
        CrsTransformationResult resultOfTransformationFromWGS84,
        CrsTransformationResult resultOfTransformationBackToWGS84
    ) {
        wgs84sourceX = "" + item.centroidX;
        wgs84sourceY = "" + item.centroidY;
        epsgCrsCode = "" + item.epsgCrsCode;
        if (resultOfTransformationFromWGS84 != null && resultOfTransformationFromWGS84.IsSuccess) {
            CrsCoordinate outputCoordinate = resultOfTransformationFromWGS84.OutputCoordinate;
            epsgTargetSourceX = "" + outputCoordinate.XEastingLongitude;
            epsgTargetSourceY = "" + outputCoordinate.YNorthingLatitude;
        }
        if (resultOfTransformationBackToWGS84 != null && resultOfTransformationBackToWGS84.IsSuccess) {
            CrsCoordinate outputCoordinate = resultOfTransformationBackToWGS84.OutputCoordinate;
            wgs84targetX = "" + outputCoordinate.XEastingLongitude;
            wgs84targetY = "" + outputCoordinate.YNorthingLatitude;
        }
    }

    public CrsCoordinate GetInputCoordinateWGS84() {
        double lat = Double.Parse(wgs84sourceY);
        double lon = Double.Parse(wgs84sourceX);
        return CrsCoordinateFactory.LatLon(lat, lon);
    }

    public bool IsSuccessfulTransformationFromWGS84() {
        // the code below would be improved by also verifying that 
        // the values in string variables are doubles since
        // they may otherwise throw an exception when trying to parse as double
        if(IsNullOrEmpty(epsgTargetSourceX)) return false;
        if(IsNullOrEmpty(epsgTargetSourceY)) return false;
        return true;
    }

    private bool IsNullOrEmpty(string s)
    {
        return (s == null || s.Trim().Equals(""));
    }

    public bool IsSuccessfulTransformationBackToWGS84() {
        // the code below would be improved by also verifying that 
        // the values in string variables are doubles since
        // they may otherwise throw an exception when trying to parse as double
        if(IsNullOrEmpty(wgs84targetX)) return false;
        if(IsNullOrEmpty(wgs84targetY)) return false;
        return true;
    }

    public CrsCoordinate GetCoordinateOutputTransformationBackToWGS84() {
        if(!IsSuccessfulTransformationBackToWGS84()) {
            return null;
        }
        // this code would be improved by also verifying that 
        // the values in string variables are doubles since
        // they may otherwise throw an exception when 
        // below trying to parse as double
        double lat = Double.Parse(wgs84targetY);
        double lon = Double.Parse(wgs84targetX);
        return CrsCoordinateFactory.LatLon(lat, lon);
    }

    /**
     * @param that
     * @param deltaValueForDifferencesToIgnore
     * @return
     */
    public DifferenceWhenComparingCoordinateValues IsDeltaDifferenceSignificant(
        TestResultItem that,
        double deltaValueForDifferencesToIgnore
    ) {
        bool thisXIsDouble = IsValueExistingAndDouble(this.wgs84targetX);
        bool thisYIsDouble = IsValueExistingAndDouble(this.wgs84targetY);
        bool thatXIsDouble = IsValueExistingAndDouble(that.wgs84targetX);
        bool thatYIsDouble = IsValueExistingAndDouble(that.wgs84targetY);
        if(thisXIsDouble != thatXIsDouble) {
            return DifferenceWhenComparingCoordinateValues.EXISTING_VS_NOT_EXISTING;
        }
        if(thisYIsDouble != thatYIsDouble) {
            return DifferenceWhenComparingCoordinateValues.EXISTING_VS_NOT_EXISTING;
        }
        if(thisYIsDouble && thisXIsDouble) { // then the others are also double according to above
            double thisLat = Double.Parse(this.wgs84targetY);
            double thisLon = Double.Parse(this.wgs84targetX);

            double thatLat = Double.Parse(that.wgs84targetY);
            double thatLon = Double.Parse(that.wgs84targetX);

            double diffLat = Math.Abs(thisLat - thatLat);
            double diffLon = Math.Abs(thisLon - thatLon);

            //    Console.WriteLine("diffLat " + diffLat);
            //    Console.WriteLine("diffLon " + diffLon);
            //    Console.WriteLine("thisLon " + thisLon);
            //    Console.WriteLine("thatLon " + thatLon);

            if(diffLon > deltaValueForDifferencesToIgnore || diffLat > deltaValueForDifferencesToIgnore) {
                return DifferenceWhenComparingCoordinateValues.SIGNIFICANT_VALUE_DIFFERENCE;
            }
        }
        return DifferenceWhenComparingCoordinateValues.NO;
    }

    private bool IsValueExistingAndDouble(string value) {
        if (value == null) return false;
        if (value.Trim().Equals("")) return false;
        // this code might be improved with a regular expression instead
        // see the documentation of Java's 'Double.valueOf(String)'
        try {
            Double.Parse(value);
            return true;
        }
        catch(Exception e) {
            return false;
        }
    }
}
}