package com.programmerare.crsTransformations;

import com.programmerare.crsTransformationFacadeGeoPackageNGA.CrsTransformationFacadeGeoPackageNGA;
import com.programmerare.crsTransformationFacadeGeoTools.CrsTransformationFacadeGeoTools;
import com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL;
import com.programmerare.crsTransformationFacadeOrbisgisCTS.CrsTransformationFacadeOrbisgisCTS;
import com.programmerare.crsTransformationFacadeProj4J.CrsTransformationFacadeProj4J;
import org.junit.jupiter.api.Test;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

public class CrsTransformationFacadeLeafFactoryTest {

    @Test
    void createCrsTransformationFacade() {
        CrsTransformationFacade crsTransformationFacade;

        final String classNameGoober = "com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL";
        assertEquals(classNameGoober, CrsTransformationFacadeGooberCTL.class.getName());

        crsTransformationFacade = CrsTransformationFacadeLeafFactory.createCrsTransformationFacade(CrsTransformationFacadeGooberCTL.class.getName());
        assertNotNull(crsTransformationFacade);

        assertNotNull(CrsTransformationFacadeLeafFactory.createCrsTransformationFacade(CrsTransformationFacadeGeoPackageNGA.class.getName()));
        assertNotNull(CrsTransformationFacadeLeafFactory.createCrsTransformationFacade(CrsTransformationFacadeGeoTools.class.getName()));
        assertNotNull(CrsTransformationFacadeLeafFactory.createCrsTransformationFacade(CrsTransformationFacadeOrbisgisCTS.class.getName()));
        assertNotNull(CrsTransformationFacadeLeafFactory.createCrsTransformationFacade(CrsTransformationFacadeProj4J.class.getName()));

    }


    @Test
    void isCrsTransformationFacade() {
        assertFalse(CrsTransformationFacadeLeafFactory.isCrsTransformationFacade(null));
        assertFalse(CrsTransformationFacadeLeafFactory.isCrsTransformationFacade(""));
        assertFalse(CrsTransformationFacadeLeafFactory.isCrsTransformationFacade("  "));
        assertFalse(CrsTransformationFacadeLeafFactory.isCrsTransformationFacade(" x "));
        assertFalse(CrsTransformationFacadeLeafFactory.isCrsTransformationFacade("abc"));

        // this test class i.e. the below "this" does not imlpement the interface so therefore assertFalse below
        assertFalse(CrsTransformationFacadeLeafFactory.isCrsTransformationFacade(this.getClass().getName()));

        assertTrue(CrsTransformationFacadeLeafFactory.isCrsTransformationFacade(CrsTransformationFacadeGooberCTL.class.getName()));
        assertTrue(CrsTransformationFacadeLeafFactory.isCrsTransformationFacade(CrsTransformationFacadeGeoPackageNGA.class.getName()));
        assertTrue(CrsTransformationFacadeLeafFactory.isCrsTransformationFacade(CrsTransformationFacadeGeoTools.class.getName()));
        assertTrue(CrsTransformationFacadeLeafFactory.isCrsTransformationFacade(CrsTransformationFacadeOrbisgisCTS.class.getName()));
        assertTrue(CrsTransformationFacadeLeafFactory.isCrsTransformationFacade(CrsTransformationFacadeProj4J.class.getName()));

        List<String> classNamesForAllKnownImplementations = CrsTransformationFacadeLeafFactory.getClassNamesForAllKnownImplementations();
        for (String className : classNamesForAllKnownImplementations) {
            assertTrue(CrsTransformationFacadeLeafFactory.isCrsTransformationFacade(className));
        }
    }

    private final static int NUMBER_OF_IMPLEMENTATIONS = 5;

    @Test
    void getInstancesOfAllKnownAvailableImplementations() {
        List<CrsTransformationFacade> list = CrsTransformationFacadeLeafFactory.getInstancesOfAllKnownAvailableImplementations();
        assertEquals(NUMBER_OF_IMPLEMENTATIONS, list.size());
        for (CrsTransformationFacade crsTransformationFacade : list) {
            assertNotNull(crsTransformationFacade);
        }
    }

    @Test
    void verifyThatNoKnownClassNamesHaveBeenRenamed() {
        //    System.out.println(CrsTransformationFacadeGooberCTL.class.getName());
        //    System.out.println(CrsTransformationFacadeGeoPackageNGA.class.getName());
        //    System.out.println(CrsTransformationFacadeGeoTools.class.getName());
        //    System.out.println(CrsTransformationFacadeOrbisgisCTS.class.getName());
        //    System.out.println(CrsTransformationFacadeProj4J.class.getName());

        List<String> classNamesForAllKnownImplementations = CrsTransformationFacadeLeafFactory.getClassNamesForAllKnownImplementations();
        assertEquals(NUMBER_OF_IMPLEMENTATIONS, classNamesForAllKnownImplementations.size());

        assertTrue(classNamesForAllKnownImplementations.contains("com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL"));
        assertTrue(classNamesForAllKnownImplementations.contains("com.programmerare.crsTransformationFacadeGeoPackageNGA.CrsTransformationFacadeGeoPackageNGA"));
        assertTrue(classNamesForAllKnownImplementations.contains("com.programmerare.crsTransformationFacadeGeoTools.CrsTransformationFacadeGeoTools"));
        assertTrue(classNamesForAllKnownImplementations.contains("com.programmerare.crsTransformationFacadeOrbisgisCTS.CrsTransformationFacadeOrbisgisCTS"));
        assertTrue(classNamesForAllKnownImplementations.contains("com.programmerare.crsTransformationFacadeProj4J.CrsTransformationFacadeProj4J"));

        assertTrue(classNamesForAllKnownImplementations.contains(CrsTransformationFacadeGooberCTL.class.getName()));
        assertTrue(classNamesForAllKnownImplementations.contains(CrsTransformationFacadeGeoPackageNGA.class.getName()));
        assertTrue(classNamesForAllKnownImplementations.contains(CrsTransformationFacadeGeoTools.class.getName()));
        assertTrue(classNamesForAllKnownImplementations.contains(CrsTransformationFacadeOrbisgisCTS.class.getName()));
        assertTrue(classNamesForAllKnownImplementations.contains(CrsTransformationFacadeProj4J.class.getName()));
    }
}