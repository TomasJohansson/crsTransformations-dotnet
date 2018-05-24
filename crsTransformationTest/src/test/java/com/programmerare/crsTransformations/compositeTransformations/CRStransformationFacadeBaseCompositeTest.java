package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformationFacadeGeoTools.CRStransformationFacadeGeoTools;
import com.programmerare.crsTransformationFacadeGooberCTL.CRStransformationFacadeGooberCTL;
import com.programmerare.crsTransformationFacadeOrbisgisCTS.CRStransformationFacadeOrbisgisCTS;
import com.programmerare.crsTransformations.CRStransformationFacade;
import org.junit.jupiter.api.BeforeAll;

abstract class CRStransformationFacadeBaseCompositeTest  {

    protected static CRStransformationFacade facadeGeoTools;
    protected static CRStransformationFacade facadeGooberCTL;
    protected static CRStransformationFacade facadeOrbisgisCTS;

    @BeforeAll
    final static void beforeAll() {
        facadeGeoTools = new CRStransformationFacadeGeoTools();
        facadeGooberCTL = new CRStransformationFacadeGooberCTL();
        facadeOrbisgisCTS = new CRStransformationFacadeOrbisgisCTS();
    }
}
