package com.programmerare.crsTransformations;

import static org.hamcrest.CoreMatchers.containsString;
import static org.hamcrest.MatcherAssert.assertThat;
import static org.junit.jupiter.api.Assertions.assertThrows;

import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.assertEquals;

public class CrsIdentifierTest {

    @Test
    void createFromCrsCodeTrimmedButWithoutUppercasingSinceNotEpsg() {
        CrsIdentifier crsIdentifier = CrsIdentifier.createFromCrsCode("  abc  ");
        assertEquals("abc", crsIdentifier.getCrsCode());
        assertEquals(false, crsIdentifier.isEpsgCode());
    }

    @Test
    void createFromEpsgNumber() {
        CrsIdentifier crsIdentifier = CrsIdentifier.createFromEpsgNumber(3006);
        assertEquals("EPSG:3006", crsIdentifier.getCrsCode());
        assertEquals(true, crsIdentifier.isEpsgCode());
        assertEquals(3006, crsIdentifier.getEpsgNumber());
    }

    @Test
    void createFromCrsCodeWithUppercasingSinceEpsg() {
        CrsIdentifier crsIdentifier = CrsIdentifier.createFromCrsCode("  epsg:4326  ");
        assertEquals("EPSG:4326", crsIdentifier.getCrsCode());
        assertEquals(true, crsIdentifier.isEpsgCode());
        assertEquals(4326, crsIdentifier.getEpsgNumber());
    }

    @Test
    void createFromCrsCodeNull() {
        IllegalArgumentException exception = assertThrows(IllegalArgumentException.class, () -> CrsIdentifier.createFromCrsCode(null), "Must not be null");
        // the exception message is currently something like this: "Parameter specified as non-null is null: method com.programmerare.crsTransformations.CrsIdentifier$Companion.createFromCrsCode, parameter crsCode"
        // (potentially fragile to test the message strings but it does not really change often, and in such a rare scenario, then easy to fix)
        assertThat(exception.getMessage(), containsString("non-null"));
    }

    @Test
    void createFromCrsCodeEmpty() {
        IllegalArgumentException exception = assertThrows(IllegalArgumentException.class, () -> CrsIdentifier.createFromCrsCode("   "), "Must not be empty string");
        // Fragile to test the message string below but easy to fix here if it would break, and it will not break/change often.
        assertThat(exception.getMessage(), containsString("non-empty"));
    }
}