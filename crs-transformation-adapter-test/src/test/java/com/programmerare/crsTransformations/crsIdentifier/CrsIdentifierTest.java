package com.programmerare.crsTransformations.crsIdentifier;

import static org.hamcrest.CoreMatchers.containsString;
import static org.hamcrest.MatcherAssert.assertThat;
import static org.junit.jupiter.api.Assertions.assertThrows;

import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.assertEquals;

public class CrsIdentifierTest {

    @Test
    void createFromCrsCodeTrimmedButWithoutUppercasingButNotEpsg() {
        CrsIdentifier crsIdentifier = CrsIdentifierFactory.createFromCrsCode("  abc  ");
        assertEquals("abc", crsIdentifier.getCrsCode());
        assertEquals(false, crsIdentifier.isEpsgCode());
    }

    @Test
    void createFromEpsgNumber() {
        // SWEREF99TM = 3006
        CrsIdentifier crsIdentifier = CrsIdentifierFactory.createFromEpsgNumber(3006);
        //Assertions.assertEquals(EpsgCode.SWEDEN__SWEREF99_TM__3006, crsIdentifier.getCrsCode());
        Assertions.assertEquals("EPSG:3006", crsIdentifier.getCrsCode());
        assertEquals(true, crsIdentifier.isEpsgCode());
        Assertions.assertEquals(EpsgNumber.SWEDEN__SWEREF99_TM__3006, crsIdentifier.getEpsgNumber());
    }

    @Test
    void createFromCrsCodeWithLowerCasedEpsg() {
        CrsIdentifier crsIdentifier = CrsIdentifierFactory.createFromCrsCode("  epsg:4326  ");
        // the input should become trimmed and uppercased "EPSG"
        assertEquals("EPSG:4326", crsIdentifier.getCrsCode());
        assertEquals(true, crsIdentifier.isEpsgCode());
        assertEquals(4326, crsIdentifier.getEpsgNumber());
    }

    @Test
    void createFromCrsCodeNullShouldFail() {
        IllegalArgumentException exception = assertThrows(
            IllegalArgumentException.class,
            () -> CrsIdentifierFactory.createFromCrsCode(null), // should fail
            "Must not be null"
        );
        assertExceptionMessageWhenArgumentWasNullOrEmptyString(exception, "non-null");
    }

    @Test
    void createFromCrsCodeWithOnlyWhiteSpaceShouldFail() {
        IllegalArgumentException exception = assertThrows(
            IllegalArgumentException.class,
            () -> CrsIdentifierFactory.createFromCrsCode("   "), // should fail
            "Must not be empty string"
        );
        assertExceptionMessageWhenArgumentWasNullOrEmptyString(exception, "non-empty");
    }

    @Test
    void equalsTestWhenCreatingFromEpsgNumberAndFromCrsCode() {
        CrsIdentifier fromEpsgNumber    = CrsIdentifierFactory.createFromEpsgNumber(3006);
        CrsIdentifier fromCrsCode       = CrsIdentifierFactory.createFromCrsCode("  epsg:3006   ");
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
        // the exception message is currently something like this: "Parameter specified as non-null is null: method com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier$Companion.createFromCrsCode, parameter crsCode"
        // (potentially fragile to test the message strings but it does not really change often, and in such a rare scenario, then easy to fix)
        assertThat(exception.getMessage(), containsString(expectedStringToBeContainedInExceptionMessage));
    }
}