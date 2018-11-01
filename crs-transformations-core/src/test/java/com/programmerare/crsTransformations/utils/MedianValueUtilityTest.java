package com.programmerare.crsTransformations.utils;

import org.junit.jupiter.api.Test;

import java.util.Arrays;

import static org.junit.jupiter.api.Assertions.assertEquals;

public class MedianValueUtilityTest {

    private final static double delta = 0.00001;

    @Test
    void getMedianFrom3values() {
        double medianValue = MedianValueUtility.getMedianValue(Arrays.asList(55.0, 33.0, 77.0));
        assertEquals(55.0, medianValue, delta);
    }

    @Test
    void getMedianFrom4values() {
        double medianValue = MedianValueUtility.getMedianValue(Arrays.asList(55.0, 33.0, 77.0, 35.0));
        // the average of the two middle values 35 and 55
        assertEquals(45.0, medianValue, delta);
    }

    @Test
    void getMedianFrom7values() {
        double medianValue = MedianValueUtility.getMedianValue(Arrays.asList(9.0, 6.0, 1.0, 7.0, 8.0, 5.0, 3.0));
        assertEquals(6.0, medianValue, delta);
    }

    @Test
    void getMedianFrom8values() {
        double medianValue = MedianValueUtility.getMedianValue(Arrays.asList(9.0, 6.0, 1.0, 7.0, 8.0, 5.0, 3.0, 6.5));
        // the average of the two middle values 6.0 and 6.5
        assertEquals(6.25, medianValue, delta);
    }

}