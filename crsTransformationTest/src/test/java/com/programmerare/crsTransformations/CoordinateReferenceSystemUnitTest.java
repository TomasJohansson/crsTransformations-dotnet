package com.programmerare.crsTransformations;

import org.junit.Test;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotEquals;

public class CoordinateReferenceSystemUnitTest {

    @Test
    public void testingKotlinEnum() {
		assertEquals(CoordinateReferenceSystemUnit.DEGREES, CoordinateReferenceSystemUnit.DEGREES);
		assertNotEquals(CoordinateReferenceSystemUnit.DEGREES, CoordinateReferenceSystemUnit.METERS);
    }
}
