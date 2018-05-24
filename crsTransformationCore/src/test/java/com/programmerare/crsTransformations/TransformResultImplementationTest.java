package com.programmerare.crsTransformations;

import static org.hamcrest.CoreMatchers.containsString;
import static org.hamcrest.MatcherAssert.assertThat;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;
import static org.junit.jupiter.api.Assumptions.assumingThat;

public class TransformResultImplementationTest {

    private TransformResult transformResult;
    private Coordinate inputCoordinate;
    private Coordinate outputCoordinate;

    @BeforeEach
    void beforeEach() {
        inputCoordinate = Coordinate.createFromXLongYLat(0.0, 0.0, 1234);
        outputCoordinate = inputCoordinate;
    }

    @Test
    void transformNotSucessTest() {
        transformResult = new TransformResultImplementation(inputCoordinate, null, null, false);
        assertNotNull(transformResult);
        assertFalse(transformResult.isSuccess());
    }

    @Test
    void transformSucessTest() {
        transformResult = new TransformResultImplementation(inputCoordinate, outputCoordinate, null, true);
        assertNotNull(transformResult);
        assertTrue(transformResult.isSuccess());
    }

    @Test
    void unvalidTransformResultConstructionWhenSuccessIsFalseButCoordinateProvided() {
        assertThrows(
            IllegalStateException.class,
            () -> new TransformResultImplementation(inputCoordinate, outputCoordinate, null, false),
            "unvalid TransformResult object construction should throw exception"
        );
    }

    @Test
    void unvalidTransformResultConstructionWhenSuccessIsTrueButCoordinateNotProvided() {
        outputCoordinate = null;
        assertThrows(
            IllegalStateException.class,
            () -> new TransformResultImplementation(inputCoordinate, outputCoordinate, null, true),
            "unvalid TransformResult object construction should throw exception"
        );
    }

    @Test
    void preconditionViolationWhenTryingToAccessCordinateWhenSuccessFalse() {
        outputCoordinate = null;
        transformResult = new TransformResultImplementation(inputCoordinate, outputCoordinate, null, false);
        Exception e = assertThrows(
            RuntimeException.class,
            () -> transformResult.getOutputCoordinate(),
            "should not try to get output coordinate unless the result was a success"
        );
        String exceptionMessage = e.getMessage().toLowerCase().replaceAll("-", "");
        // the purpose of ther above is to make the test less sensitive to the exact message
        // e.g. will match text containing "Pre-condition" (including hyphen and uppercased first letter)
        assertThat(exceptionMessage, containsString("precondition"));
    }
}
