using System;
using Programmerare.CrsTransformations.Coordinate;

namespace Programmerare.CrsTransformations.TestData
{

public class TestResultItem {
    private String wgs84sourceX , wgs84sourceY , epsgCrsCode;
    private String epsgTargetSourceX = "", epsgTargetSourceY = "", wgs84targetX = "", wgs84targetY = "";
    private const string SEPARATOR = "|";

    public String getResultStringForRegressionFile() {
        return
            wgs84sourceX + SEPARATOR +
            wgs84sourceY + SEPARATOR +
            epsgCrsCode + SEPARATOR +
            epsgTargetSourceX + SEPARATOR +
            epsgTargetSourceY + SEPARATOR +
            wgs84targetX + SEPARATOR +
            wgs84targetY;
    }

    public TestResultItem(String lineFromRow) {
        String[] array = lineFromRow.Split("\\" + SEPARATOR);
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

    public CrsCoordinate getInputCoordinateWGS84() {
        double lat = Double.Parse(wgs84sourceY);
        double lon = Double.Parse(wgs84sourceX);
        return CrsCoordinateFactory.LatLon(lat, lon);
    }

    public bool isSuccessfulTransformationFromWGS84() {
        if(IsNullOrEmpty(epsgTargetSourceX)) return false;
        if(IsNullOrEmpty(epsgTargetSourceY)) return false;
        // TODO: to improve this we should also verify that the values are doubles
        return true;
    }

    private bool IsNullOrEmpty(string s)
    {
        return (s == null || s.Trim().Equals(""));
    }

    public bool isSuccessfulTransformationBackToWGS84() {
        if(IsNullOrEmpty(wgs84targetX)) return false;
        if(IsNullOrEmpty(wgs84targetY)) return false;
        // TODO: to improve this we should also verify that the values are doubles
        return true;
    }

    public CrsCoordinate getCoordinateOutputTransformationBackToWGS84() {
        if(!isSuccessfulTransformationBackToWGS84()) {
            return null;
        }
        // TODO: to improve this we should also verify that the values are doubles
        // i.e. exception might be thrown below
        double lat = Double.Parse(wgs84targetY);
        double lon = Double.Parse(wgs84targetX);
        return CrsCoordinateFactory.LatLon(lat, lon);
    }

    /**
     * @param that
     * @param deltaValueForDifferencesToIgnore
     * @return
     */
    public DifferenceWhenComparingCoordinateValues isDeltaDifferenceSignificant(
            TestResultItem that,
            double deltaValueForDifferencesToIgnore
    ) {
        bool thisXIsDouble = isValueExistingAndDouble(this.wgs84targetX);
        bool thisYIsDouble = isValueExistingAndDouble(this.wgs84targetY);
        bool thatXIsDouble = isValueExistingAndDouble(that.wgs84targetX);
        bool thatYIsDouble = isValueExistingAndDouble(that.wgs84targetY);
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

            //    System.out.println("diffLat " + diffLat);
            //    System.out.println("diffLon " + diffLon);
            //    System.out.println("thisLon " + thisLon);
            //    System.out.println("thatLon " + thatLon);

            if(diffLon > deltaValueForDifferencesToIgnore || diffLat > deltaValueForDifferencesToIgnore) {
                return DifferenceWhenComparingCoordinateValues.SIGNIFICANT_VALUE_DIFFERENCE;
            }
        }
        return DifferenceWhenComparingCoordinateValues.NO;
    }

    private bool isValueExistingAndDouble(string value) {
        if (value == null) return false;
        if (value.Trim().Equals("")) return false;
        // TODO improve the code below, maybe with a regular expression instead,
        // see the documentation of 'Double.valueOf(String)'
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