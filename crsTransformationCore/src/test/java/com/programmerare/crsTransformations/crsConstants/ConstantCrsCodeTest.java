package com.programmerare.crsTransformations.crsConstants;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

public class ConstantCrsCodeTest {
    @Test
    void assertRT90() {
        assertEquals("EPSG:3021", ConstantCrsCode.RT90_25_GON_V);
    }
}
