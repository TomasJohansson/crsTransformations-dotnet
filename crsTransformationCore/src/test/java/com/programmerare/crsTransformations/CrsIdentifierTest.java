package com.programmerare.crsTransformations;

import static org.hamcrest.CoreMatchers.containsString;
import static org.hamcrest.MatcherAssert.assertThat;
import static org.junit.jupiter.api.Assertions.assertThrows;

import com.programmerare.crsTransformations.crsConstants.ConstantCrsCode;
import com.programmerare.crsTransformations.crsConstants.ConstantEpsgNumber;
import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.assertEquals;

public class CrsIdentifierTest {

    @Test
    void createFromCrsCodeTrimmedButWithoutUppercasingButNotEpsg() {
        CrsIdentifier crsIdentifier = CrsIdentifier.createFromCrsCode("  abc  ");
        assertEquals("abc", crsIdentifier.getCrsCode());
        assertEquals(false, crsIdentifier.isEpsgCode());
    }

    @Test
    void createFromEpsgNumber() {
        // SWEREF99TM = 3006
        CrsIdentifier crsIdentifier = CrsIdentifier.createFromEpsgNumber(3006);
        assertEquals(ConstantCrsCode.SWEREF99TM, crsIdentifier.getCrsCode());
        assertEquals(true, crsIdentifier.isEpsgCode());
        assertEquals(ConstantEpsgNumber.SWEREF99TM, crsIdentifier.getEpsgNumber());
    }

    @Test
    void createFromCrsCodeWithLowerCasedEpsg() {
        CrsIdentifier crsIdentifier = CrsIdentifier.createFromCrsCode("  epsg:4326  ");
        // the input should become trimmed and uppercased "EPSG"
        assertEquals("EPSG:4326", crsIdentifier.getCrsCode());
        assertEquals(true, crsIdentifier.isEpsgCode());
        assertEquals(4326, crsIdentifier.getEpsgNumber());
    }

    @Test
    void createFromCrsCodeNullShouldFail() {
        IllegalArgumentException exception = assertThrows(
            IllegalArgumentException.class,
            () -> CrsIdentifier.createFromCrsCode(null), // should fail
            "Must not be null"
        );
        assertExceptionMessageWhenArgumentWasNullOrEmptyString(exception, "non-null");
    }

    @Test
    void createFromCrsCodeWithOnlyWhiteSpaceShouldFail() {
        IllegalArgumentException exception = assertThrows(
            IllegalArgumentException.class,
            () -> CrsIdentifier.createFromCrsCode("   "), // should fail
            "Must not be empty string"
        );
        assertExceptionMessageWhenArgumentWasNullOrEmptyString(exception, "non-empty");
    }

    @Test
    void equalsTestWhenCreatingFromEpsgNumberAndFromCrsCode() {
        CrsIdentifier fromEpsgNumber    = CrsIdentifier.createFromEpsgNumber(3006);
        CrsIdentifier fromCrsCode       = CrsIdentifier.createFromCrsCode("  epsg:3006   ");
        assertEquals(fromEpsgNumber, fromCrsCode);
        assertEquals(fromEpsgNumber.hashCode(), fromCrsCode.hashCode());
    }

    /**
     * @param exception
     * @param expectedStringToBeContainedInExceptionMessage e.g. "non-null" or "non-empty"
     */
    private void assertExceptionMessageWhenArgumentWasNullOrEmptyString(
        IllegalArgumentException exception,
        String expectedStringToBeContainedInExceptionMessage
    ) {
        // the exception message is currently something like this: "Parameter specified as non-null is null: method com.programmerare.crsTransformations.CrsIdentifier$Companion.createFromCrsCode, parameter crsCode"
        // (potentially fragile to test the message strings but it does not really change often, and in such a rare scenario, then easy to fix)
        assertThat(exception.getMessage(), containsString(expectedStringToBeContainedInExceptionMessage));
    }
}