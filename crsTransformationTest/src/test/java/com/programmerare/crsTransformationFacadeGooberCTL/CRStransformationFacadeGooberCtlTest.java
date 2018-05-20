package com.programmerare.crsTransformationFacadeGooberCTL;

import com.programmerare.crsTransformations.CRStransformationFacadeBaseTest;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

class CRStransformationFacadeGooberCtlTest extends CRStransformationFacadeBaseTest {
    @DisplayName("Goober Cts transformation from WGS84 to SWEREF99")
    @Test
    void gooberCtlTest() {
        super.testTransformationFromWgs84ToSweref99TM(new CRStransformationFacadeGooberCTL());
    }
}