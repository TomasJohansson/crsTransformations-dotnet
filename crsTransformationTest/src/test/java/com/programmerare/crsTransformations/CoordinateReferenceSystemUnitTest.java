package com.programmerare.crsTransformations;

import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotEquals;

class CoordinateReferenceSystemUnitTest {

    @Test
    void testingKotlinEnum() {
		assertEquals(CoordinateReferenceSystemUnit.DEGREES, CoordinateReferenceSystemUnit.DEGREES);
		assertNotEquals(CoordinateReferenceSystemUnit.DEGREES, CoordinateReferenceSystemUnit.METERS);
    }
}
