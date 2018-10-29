package com.programmerare.crsTransformations;

import com.programmerare.crsTransformationFacadeGeoPackageNGA.CrsTransformationFacadeGeoPackageNGA;
import com.programmerare.crsTransformationFacadeGeoTools.CrsTransformationFacadeGeoTools;
import com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL;
import com.programmerare.crsTransformationFacadeOrbisgisCTS.CrsTransformationFacadeOrbisgisCTS;
import com.programmerare.crsTransformationFacadeProj4J.CrsTransformationFacadeProj4J;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

public class CrsTransformationFacadeFactoryTest {

    @Test
    void createCrsTransformationFacade() {
        CrsTransformationFacade crsTransformationFacade;

        final String classNameGoober = "com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL";
        assertEquals(classNameGoober, CrsTransformationFacadeGooberCTL.class.getName());

        crsTransformationFacade = CrsTransformationFacadeFactory.createCrsTransformationFacade(CrsTransformationFacadeGooberCTL.class.getName());
        assertNotNull(crsTransformationFacade);

        assertNotNull(CrsTransformationFacadeFactory.createCrsTransformationFacade(CrsTransformationFacadeGeoPackageNGA.class.getName()));
        assertNotNull(CrsTransformationFacadeFactory.createCrsTransformationFacade(CrsTransformationFacadeGeoTools.class.getName()));
        assertNotNull(CrsTransformationFacadeFactory.createCrsTransformationFacade(CrsTransformationFacadeOrbisgisCTS.class.getName()));
        assertNotNull(CrsTransformationFacadeFactory.createCrsTransformationFacade(CrsTransformationFacadeProj4J.class.getName()));
    }


    @Test
    void isCrsTransformationFacade() {
        assertFalse(CrsTransformationFacadeFactory.isCrsTransformationFacade(null));
        assertFalse(CrsTransformationFacadeFactory.isCrsTransformationFacade(""));
        assertFalse(CrsTransformationFacadeFactory.isCrsTransformationFacade("  "));
        assertFalse(CrsTransformationFacadeFactory.isCrsTransformationFacade(" x "));
        assertFalse(CrsTransformationFacadeFactory.isCrsTransformationFacade("abc"));

        // this test class i.e. the below "this" does not imlpement the interface so therefore assertFalse below
        assertFalse(CrsTransformationFacadeFactory.isCrsTransformationFacade(this.getClass().getName()));

        assertTrue(CrsTransformationFacadeFactory.isCrsTransformationFacade(CrsTransformationFacadeGooberCTL.class.getName()));
        assertTrue(CrsTransformationFacadeFactory.isCrsTransformationFacade(CrsTransformationFacadeGeoPackageNGA.class.getName()));
        assertTrue(CrsTransformationFacadeFactory.isCrsTransformationFacade(CrsTransformationFacadeGeoTools.class.getName()));
        assertTrue(CrsTransformationFacadeFactory.isCrsTransformationFacade(CrsTransformationFacadeOrbisgisCTS.class.getName()));
        assertTrue(CrsTransformationFacadeFactory.isCrsTransformationFacade(CrsTransformationFacadeProj4J.class.getName()));
    }
}