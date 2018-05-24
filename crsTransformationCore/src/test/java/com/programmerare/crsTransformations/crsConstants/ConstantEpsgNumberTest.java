package com.programmerare.crsTransformations.crsConstants;

import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.*;

public class ConstantEpsgNumberTest {
    @Test
    void assertSWEREF99TM() {
        assertEquals(3006, ConstantEpsgNumber.SWEREF99TM);
    }
}
