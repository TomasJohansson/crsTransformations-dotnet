package com.programmerare.crsTransformations;

import static org.hamcrest.CoreMatchers.containsString;
import static org.hamcrest.MatcherAssert.assertThat;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.mock;

public class TransformResultImplementationTest {

    private TransformResult transformResult;
    private Coordinate inputCoordinate;
    private Coordinate outputCoordinate;

    private CrsTransformationFacade crsTransformationFacade;

    @BeforeEach
    void beforeEach() {
        inputCoordinate = Coordinate.createFromXLongYLat(0.0, 0.0, 1234);
        outputCoordinate = inputCoordinate;

        // the mock below is currently not used any more than as a convenient way
        // of constructing an instance of the interface
        crsTransformationFacade = mock(CrsTransformationFacade.class);
    }

    @Test
    void transformNotSucessTest() {
        transformResult = new TransformResultImplementation(inputCoordinate, null, null, false, crsTransformationFacade);
        assertFalse(transformResult.isSuccess());
    }

    @Test
    void transformSucessTest() {
        transformResult = new TransformResultImplementation(inputCoordinate, outputCoordinate, null, true, crsTransformationFacade);
        assertTrue(transformResult.isSuccess());
    }

    @Test
    void unvalidTransformResultConstructionWhenSuccessIsFalseButCoordinateProvided() {
        assertThrows(
            IllegalStateException.class,
            () -> new TransformResultImplementation(inputCoordinate, outputCoordinate, null, false, crsTransformationFacade),
            "unvalid TransformResult object construction should throw exception when success false is combined with output coordinate"
        );
    }

    @Test
    void unvalidTransformResultConstructionWhenSuccessIsTrueButCoordinateNotProvided() {
        outputCoordinate = null;
        assertThrows(
            IllegalStateException.class,
            () -> new TransformResultImplementation(inputCoordinate, outputCoordinate, null, true, crsTransformationFacade),
            "unvalid TransformResult object construction should throw exception when success true is combined with null as output coordinate"
        );
    }

    @Test
    void preconditionViolationWhenTryingToAccessCoordinateWhenSuccessFalse() {
        outputCoordinate = null;
        transformResult = new TransformResultImplementation(inputCoordinate, outputCoordinate, null, false, crsTransformationFacade);
        Exception e = assertThrows(
            RuntimeException.class,
            () -> transformResult.getOutputCoordinate(),
            "Should not try to get output coordinate unless the result was a success"
        );
        String exceptionMessage = e.getMessage().toLowerCase().replaceAll("-", "");
        // the purpose of ther above is to make the test less sensitive to the exact message
        // e.g. will match text containing "Pre-condition" (including hyphen and uppercased first letter)
        assertThat(exceptionMessage, containsString("precondition"));
    }
}