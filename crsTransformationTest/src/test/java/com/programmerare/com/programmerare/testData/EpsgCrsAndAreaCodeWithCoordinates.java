package com.programmerare.com.programmerare.testData;

public class EpsgCrsAndAreaCodeWithCoordinates {
    final int epsgCrsCode;
    final int epsgAreaCode;
    final String epsgAreaName;
    final double centroidX;
    final double centroidY;

    EpsgCrsAndAreaCodeWithCoordinates(
            int epsgCrsCode,
            int epsgAreaCode,
            String epsgAreaName,
            double centroidX,
            double centroidY
    ) {
        this.epsgCrsCode = epsgCrsCode;
        this.epsgAreaCode = epsgAreaCode;
        this.epsgAreaName = epsgAreaName;
        this.centroidX = centroidX;
        this.centroidY = centroidY;
    }

    @Override
    public String toString() {
        return
            "EpsgCrsAndAreaCodeWithCoordinates{" +
            " epsgAreaCode=" + epsgAreaCode +
            ", epsgCrsCode=" + epsgCrsCode +
            ", epsgAreaName='" + epsgAreaName + '\'' +
            ", centroidX=" + centroidX +
            ", centroidY=" + centroidY +
            '}';
    }
}