package com.programmerare.crsTransformations;

import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.assertEquals;

public class CrsIdentifierTest {

    @Test
    void createFromCrsCodeTest() {
        CrsIdentifier crsIdentifier = CrsIdentifier.createFromCrsCode("abc");
        assertEquals("abc", crsIdentifier.getCrsCode());
    }

    @Test
    void createFromEpsgNumberTest() {
        CrsIdentifier crsIdentifier = CrsIdentifier.createFromEpsgNumber(4321);
        assertEquals("EPSG:4321", crsIdentifier.getCrsCode());
    }
}
