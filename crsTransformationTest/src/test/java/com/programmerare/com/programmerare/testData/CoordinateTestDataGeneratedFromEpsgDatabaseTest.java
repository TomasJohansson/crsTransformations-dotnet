package com.programmerare.com.programmerare.testData;

import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_3.EpsgCode;
import com.programmerare.crsTransformationFacadeGeoTools.CrsTransformationFacadeGeoTools;
import com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.CrsTransformationFacade;
import com.programmerare.crsTransformations.TransformResult;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvFileSource;
import static org.junit.jupiter.api.Assertions.*;

import com.google.common.io.Resources;

import java.io.IOException;
import java.net.URL;
import java.nio.charset.Charset;
import java.util.ArrayList;
import java.util.List;

class CoordinateTestDataGeneratedFromEpsgDatabaseTest {

    @ParameterizedTest
    @CsvFileSource(resources = "/generated/CoordinateTestDataGeneratedFromEpsgDatabase.csv", numLinesToSkip = 0, delimiter = '|')
    @DisplayName("TODO ...")
    void testCsvFileGeneratedCoordinates(
        int epsgCrsCode,
        int epsgAreaCode,
        String epsgAreaName,
        double centroidX,
        double centroidY
    ) {
        CrsTransformationFacadeGeoTools facade = new CrsTransformationFacadeGeoTools();
        Coordinate inputCoordinateWGS84 = Coordinate.createFromXLongYLat(centroidX, centroidY, EpsgCode.WORLD__WGS_84__4326);
        Coordinate outputCoordinate = facade.transform(inputCoordinateWGS84, epsgCrsCode);
        Coordinate wgs84Again = facade.transform(outputCoordinate, EpsgCode.WORLD__WGS_84__4326);
        double delta = 0.000001;
        assertEquals(inputCoordinateWGS84.getXLongitude(), wgs84Again.getXLongitude(), delta);
        assertEquals(inputCoordinateWGS84.getYLatitude(), wgs84Again.getYLatitude(), delta);
    }

    @Test
    void testar() {
        CrsTransformationFacade facade = new CrsTransformationFacadeGeoTools();
        facade = new CrsTransformationFacadeGooberCTL();

        EpsgCrsAndAreaCodeWithCoordinates item = createEpsgCrsAndAreaCodeWithCoordinatesFromLineInCsvFile("3006|1225|Sweden|17.083659606206545|61.98770256318016");
        assertEquals(3006, item.epsgCrsCode);
        assertEquals(3006, item.epsgCrsCode);
        final Coordinate inputCoordinateWGS84 = Coordinate.createFromXLongYLat(item.centroidX, item.centroidY, EpsgCode.WORLD__WGS_84__4326);
        //Coordinate outputCoordinate = facade.transform(inputCoordinateWGS84, item.epsgCrsCode);
        final TransformResult transformResult = facade.transformToResultObject(inputCoordinateWGS84, item.epsgCrsCode);
        boolean isSuccess = transformResult.isSuccess();
        System.out.println("isSuccess 1 " + isSuccess);
        if(isSuccess) {
            final TransformResult resultWgs84Again = facade.transformToResultObject(transformResult.getOutputCoordinate(), EpsgCode.WORLD__WGS_84__4326);
            isSuccess = resultWgs84Again.isSuccess();
            System.out.println("isSuccess 2 " + isSuccess);
            if(isSuccess) {
                final Coordinate wgs84Again = resultWgs84Again.getOutputCoordinate();
                final double deltaLong = Math.abs(inputCoordinateWGS84.getXLongitude() - wgs84Again.getXLongitude());
                final double deltaLat = Math.abs(inputCoordinateWGS84.getYLatitude() - wgs84Again.getYLatitude());
                isSuccess = deltaLong < deltaLimit && deltaLat < deltaLimit;
                System.out.printf("wgs84Again " + wgs84Again + " vs original wgs84 " + inputCoordinateWGS84);
            }
        }
    }

    @Test
    void testCsvFileGeneratedCoordinatesRead() {
        CrsTransformationFacadeGeoTools facade = new CrsTransformationFacadeGeoTools();

        int countOfFailures = 0;
        int countOfSuccess = 0;

        List<EpsgCrsAndAreaCodeWithCoordinates> list = getListOfEpsgCrsAndAreaCodeWithCoordinates();
        for (EpsgCrsAndAreaCodeWithCoordinates item : list) {
            final Coordinate inputCoordinateWGS84 = Coordinate.createFromXLongYLat(item.centroidX, item.centroidY, EpsgCode.WORLD__WGS_84__4326);
            //Coordinate outputCoordinate = facade.transform(inputCoordinateWGS84, item.epsgCrsCode);
            final TransformResult transformResult = facade.transformToResultObject(inputCoordinateWGS84, item.epsgCrsCode);
            boolean isSuccess = transformResult.isSuccess();
//            System.out.println("isSuccess 1 " + isSuccess);
            if(isSuccess) {
                final TransformResult resultWgs84Again = facade.transformToResultObject(transformResult.getOutputCoordinate(), EpsgCode.WORLD__WGS_84__4326);
                isSuccess = resultWgs84Again.isSuccess();
//                System.out.println("isSuccess 2 " + isSuccess);
                if(isSuccess) {
                    final Coordinate wgs84Again = resultWgs84Again.getOutputCoordinate();
                    final double deltaLong = Math.abs(inputCoordinateWGS84.getXLongitude() - wgs84Again.getXLongitude());
                    final double deltaLat = Math.abs(inputCoordinateWGS84.getYLatitude() - wgs84Again.getYLatitude());
                    isSuccess = deltaLong < deltaLimit && deltaLat < deltaLimit;
//                    System.out.printf("wgs84Again " + wgs84Again + " vs original wgs84 " + inputCoordinateWGS84);
                }
            }
            if(isSuccess) {
                countOfSuccess++;
            }
            else {
                countOfFailures++;
            }
            int tot = countOfFailures + countOfSuccess;
            if(tot % 500 == 0) {
                System.out.println("countOfSuccess: " + countOfSuccess);
                System.out.println("countOfFailures: " + countOfFailures);
            }
//            if(countOfFailures > 3) break;
//            assertEquals(inputCoordinateWGS84.getXLongitude(), wgs84Again.getXLongitude(), deltaLimit);
//            assertEquals(inputCoordinateWGS84.getYLatitude(), wgs84Again.getYLatitude(), deltaLimit);
        }
        System.out.println("countOfSuccess: " + countOfSuccess);
        System.out.println("countOfFailures: " + countOfFailures);
    }

    private final static double deltaLimit = 0.000001;

    private List<EpsgCrsAndAreaCodeWithCoordinates> getListOfEpsgCrsAndAreaCodeWithCoordinates() {
        ArrayList<EpsgCrsAndAreaCodeWithCoordinates> list = new ArrayList<>();
        try {
            String pathToTestDataFile = "generated/CoordinateTestDataGeneratedFromEpsgDatabase.csv";
            final URL url = Resources.getResource(pathToTestDataFile);
            final List<String> lines = Resources.readLines(url, Charset.forName("UTF-8"));
//            System.out.printf("lines " + lines.size());
            for (String line : lines) {
                EpsgCrsAndAreaCodeWithCoordinates epsgCrsAndAreaCodeWithCoordinates = createEpsgCrsAndAreaCodeWithCoordinatesFromLineInCsvFile(line);
//                System.out.println(epsgCrsAndAreaCodeWithCoordinates);
                list.add(epsgCrsAndAreaCodeWithCoordinates);
//                break;
            }
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
        return list;
    }

    private EpsgCrsAndAreaCodeWithCoordinates createEpsgCrsAndAreaCodeWithCoordinatesFromLineInCsvFile(String line) {
        final String trimmedLine = line.trim();
        // e.g. "3006|1225|Sweden|17.083659606206545|61.98770256318016"
        final String[] parts = trimmedLine.split("\\|");
        return new EpsgCrsAndAreaCodeWithCoordinates(
            Integer.parseInt(parts[0]),     // epsgCrsCode
            Integer.parseInt(parts[1]),     // epsgAreaCode
            parts[2],                       // epsgAreaName
            Double.parseDouble(parts[3]),   // centroidX
            Double.parseDouble(parts[4])    // centroidY
        );
    }

    class EpsgCrsAndAreaCodeWithCoordinates {
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
                "epsgAreaCode=" + epsgAreaCode +
                ", epsgCrsCode=" + epsgCrsCode +
                ", epsgAreaName='" + epsgAreaName + '\'' +
                ", centroidX=" + centroidX +
                ", centroidY=" + centroidY +
                '}';
        }
    }
}

// result with GeoTools with different values of delta
//double deltaLimit = 0.1;
//countOfSuccess: 4036
//countOfFailures: 2399
//
//double deltaLimit = 0.00001;
//countOfSuccess: 4036
//countOfFailures: 2399
//
//double deltaLimit = 0.000001;
//countOfSuccess: 4031
//countOfFailures: 2404
//
//double deltaLimit = 0.0000001;
//countOfSuccess: 3933
//countOfFailures: 2502
