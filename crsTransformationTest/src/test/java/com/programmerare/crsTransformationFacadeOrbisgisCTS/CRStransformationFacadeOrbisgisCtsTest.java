package com.programmerare.crsTransformationFacadeOrbisgisCTS;

import com.programmerare.crsTransformations.CRStransformationFacadeBaseTest;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

class CRStransformationFacadeOrbisgisCtsTest extends CRStransformationFacadeBaseTest {
    @DisplayName("Orbisgis Cts transformation from WGS84 to SWEREF99")
    @Test
    void orbisgisCtsTest() {
        super.testTransformationFromWgs84ToSweref99TM(new CRStransformationFacadeOrbisgisCTS());
    }
}