package com.programmerare.com.programmerare.testData;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvFileSource;

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
        // System.out.println("epsgAreaName " + epsgAreaName);
    }
}