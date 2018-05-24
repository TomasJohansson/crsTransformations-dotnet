package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformationFacadeGeoTools.CRStransformationFacadeGeoTools;
import com.programmerare.crsTransformationFacadeGooberCTL.CRStransformationFacadeGooberCTL;
import com.programmerare.crsTransformationFacadeOrbisgisCTS.CRStransformationFacadeOrbisgisCTS;
import com.programmerare.crsTransformations.CRStransformationFacade;
import org.junit.jupiter.api.BeforeAll;

import java.util.Arrays;
import java.util.List;

abstract class CRStransformationFacadeBaseCompositeTest  {

    protected static CRStransformationFacade facadeGeoTools;
    protected static CRStransformationFacade facadeGooberCTL;
    protected static CRStransformationFacade facadeOrbisgisCTS;
    protected static List<CRStransformationFacade> allFacades;

    @BeforeAll
    final static void beforeAll() {
        facadeGeoTools = new CRStransformationFacadeGeoTools();
        facadeGooberCTL = new CRStransformationFacadeGooberCTL();
        facadeOrbisgisCTS = new CRStransformationFacadeOrbisgisCTS();
        allFacades = Arrays.asList(facadeGeoTools, facadeGooberCTL, facadeOrbisgisCTS);
    }
}
