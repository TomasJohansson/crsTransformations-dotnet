package com.programmerare.crsTransformations.crsConstants;

import com.programmerare.crsTransformations.CrsIdentifier;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

public class ConstantCrsIdentifierTest {
    @Test
    void assertWGS84() {
        CrsIdentifier wgs84 = ConstantCrsIdentifier.WGS84;
        assertEquals(4326, wgs84.getEpsgNumber());
    }
}
