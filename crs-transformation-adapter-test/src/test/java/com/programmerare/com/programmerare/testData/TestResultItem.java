package com.programmerare.com.programmerare.testData;

import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsTransformations.CrsTransformationResult;
import com.programmerare.crsTransformations.coordinate.CrsCoordinateFactory;

public class TestResultItem {
    private String wgs84sourceX , wgs84sourceY , epsgCrsCode;
    private String epsgTargetSourceX = "", epsgTargetSourceY = "", wgs84targetX = "", wgs84targetY = "";
    private final static String SEPARATOR = "|";

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

    TestResultItem(String lineFromRow) {
        String[] array = lineFromRow.split("\\" + SEPARATOR);
        this.wgs84sourceX = array[0];
        this.wgs84sourceY = array[1];
        this.epsgCrsCode = array[2];
        if(array.length > 4) {
            this.epsgTargetSourceX = array[3];
            this.epsgTargetSourceY = array[4];
            if(array.length > 6) {
                this.wgs84targetX = array[5];
                this.wgs84targetY = array[6];
            }
        }
    }

    TestResultItem(
            EpsgCrsAndAreaCodeWithCoordinates item,
            CrsCoordinate inputCoordinateWGS84,
            CrsTransformationResult resultOfTransformationFromWGS84,
            CrsTransformationResult resultOfTransformationBackToWGS84
    ) {
        wgs84sourceX = "" + item.centroidX;
        wgs84sourceY = "" + item.centroidY;
        epsgCrsCode = "" + item.epsgCrsCode;
        if (resultOfTransformationFromWGS84 != null && resultOfTransformationFromWGS84.isSuccess()) {
            final CrsCoordinate outputCoordinate = resultOfTransformationFromWGS84.getOutputCoordinate();
            epsgTargetSourceX = "" + outputCoordinate.getXLongitude();
            epsgTargetSourceY = "" + outputCoordinate.getYLatitude();
        }
        if (resultOfTransformationBackToWGS84 != null && resultOfTransformationBackToWGS84.isSuccess()) {
            final CrsCoordinate outputCoordinate = resultOfTransformationBackToWGS84.getOutputCoordinate();
            wgs84targetX = "" + outputCoordinate.getXLongitude();
            wgs84targetY = "" + outputCoordinate.getYLatitude();
        }
    }

    public CrsCoordinate getInputCoordinateWGS84() {
        double lat = Double.parseDouble(wgs84sourceY);
        double lon = Double.parseDouble(wgs84sourceX);
        return CrsCoordinateFactory.latLon(lat, lon);
    }

    public boolean isSuccessfulTransformationFromWGS84() {
        if(epsgTargetSourceX == null || epsgTargetSourceY== null) return false;
        if(epsgTargetSourceX.isEmpty() || epsgTargetSourceY.isEmpty()) return false;
        // TODO: to improve this we should also verify that the values are doubles
        return true;
    }

    public boolean isSuccessfulTransformationBackToWGS84() {
        if(wgs84targetX == null || wgs84targetY == null) return false;
        if(wgs84targetX.isEmpty() || wgs84targetY.isEmpty()) return false;
        // TODO: to improve this we should also verify that the values are doubles
        return true;
    }

    public CrsCoordinate getCoordinateOutputTransformationBackToWGS84() {
        if(!isSuccessfulTransformationBackToWGS84()) {
            return null;
        }
        // TODO: to improve this we should also verify that the values are doubles
        // i.e. exception might be thrown below
        double lat = Double.parseDouble(wgs84targetY);
        double lon = Double.parseDouble(wgs84targetX);
        return CrsCoordinateFactory.latLon(lat, lon);
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
        boolean thisXIsDouble = isValueExistingAndDouble(this.wgs84targetX);
        boolean thisYIsDouble = isValueExistingAndDouble(this.wgs84targetY);
        boolean thatXIsDouble = isValueExistingAndDouble(that.wgs84targetX);
        boolean thatYIsDouble = isValueExistingAndDouble(that.wgs84targetY);
        if(thisXIsDouble != thatXIsDouble) {
            return DifferenceWhenComparingCoordinateValues.EXISTING_VS_NOT_EXISTING;
        }
        if(thisYIsDouble != thatYIsDouble) {
            return DifferenceWhenComparingCoordinateValues.EXISTING_VS_NOT_EXISTING;
        }
        if(thisYIsDouble && thisXIsDouble) { // then the others are also double according to above
            double thisLat = Double.parseDouble(this.wgs84targetY);
            double thisLon = Double.parseDouble(this.wgs84targetX);

            double thatLat = Double.parseDouble(that.wgs84targetY);
            double thatLon = Double.parseDouble(that.wgs84targetX);

            double diffLat = Math.abs(thisLat - thatLat);
            double diffLon = Math.abs(thisLon - thatLon);

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

    private boolean isValueExistingAndDouble(String value) {
        if (value == null) return false;
        if (value.isEmpty()) return false;
        // TODO improve the code below, maybe with a regular expression instead,
        // see the documentation of 'Double.valueOf(String)'
        try {
            Double.parseDouble(value);
            return true;
        }
        catch(Exception e) {
            return false;
        }
    }
}