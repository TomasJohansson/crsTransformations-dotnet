package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformationFacadeGeoTools.CRStransformationFacadeGeoTools;
import com.programmerare.crsTransformationFacadeGooberCTL.CRStransformationFacadeGooberCTL;
import com.programmerare.crsTransformationFacadeOrbisgisCTS.CRStransformationFacadeOrbisgisCTS;
import com.programmerare.crsTransformations.CRStransformationFacade;
import org.geotools.factory.GeoTools;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;

import java.util.Arrays;
import java.util.List;
import java.util.stream.IntStream;

import static org.junit.jupiter.api.Assertions.*;
import static java.util.stream.Collectors.toList;

public class CRStransformationFacadeChainOfResponsibilityTest extends CRStransformationFacadeBaseCompositeTest {

    @Test
    void createCRStransformationFacadeChainOfResponsibility() {
        assertNotNull(facadeGeoTools); // should have been created in the base class
        new CRStransformationFacadeChainOfResponsibility(Arrays.asList(facadeGeoTools, facadeGooberCTL, facadeOrbisgisCTS));
    }
}
