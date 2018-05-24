package com.programmerare.crsTransformations.compositeTransformations;

import org.junit.jupiter.api.Test;

import java.util.Arrays;

import static org.junit.jupiter.api.Assertions.*;

public class CRStransformationFacadeAverageTest extends CRStransformationFacadeBaseCompositeTest {

    @Test
    void createCRStransformationFacadeAverage() {
        assertNotNull(facadeGooberCTL); // should have been created in the base class
        new CRStransformationFacadeAverage(Arrays.asList(facadeGeoTools, facadeGooberCTL, facadeOrbisgisCTS));
    }
}
