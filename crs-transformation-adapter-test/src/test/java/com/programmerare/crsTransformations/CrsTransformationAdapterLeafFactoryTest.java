package com.programmerare.crsTransformations;

import com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA;
import com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools;
import com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL;
import com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS;
import com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J;
import org.junit.jupiter.api.Test;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

public class CrsTransformationAdapterLeafFactoryTest {

    @Test
    void createCrsTransformationAdapter() {
        CrsTransformationAdapter crsTransformationAdapter;

        final String classNameGoober = "com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL";
        assertEquals(classNameGoober, CrsTransformationAdapterGooberCTL.class.getName());

        crsTransformationAdapter = CrsTransformationAdapterLeafFactory.createCrsTransformationAdapter(CrsTransformationAdapterGooberCTL.class.getName());
        assertNotNull(crsTransformationAdapter);

        assertNotNull(CrsTransformationAdapterLeafFactory.createCrsTransformationAdapter(CrsTransformationAdapterGeoPackageNGA.class.getName()));
        assertNotNull(CrsTransformationAdapterLeafFactory.createCrsTransformationAdapter(CrsTransformationAdapterGeoTools.class.getName()));
        assertNotNull(CrsTransformationAdapterLeafFactory.createCrsTransformationAdapter(CrsTransformationAdapterOrbisgisCTS.class.getName()));
        assertNotNull(CrsTransformationAdapterLeafFactory.createCrsTransformationAdapter(CrsTransformationAdapterProj4J.class.getName()));

    }


    @Test
    void isCrsTransformationAdapter() {
        assertFalse(CrsTransformationAdapterLeafFactory.isCrsTransformationAdapter(null));
        assertFalse(CrsTransformationAdapterLeafFactory.isCrsTransformationAdapter(""));
        assertFalse(CrsTransformationAdapterLeafFactory.isCrsTransformationAdapter("  "));
        assertFalse(CrsTransformationAdapterLeafFactory.isCrsTransformationAdapter(" x "));
        assertFalse(CrsTransformationAdapterLeafFactory.isCrsTransformationAdapter("abc"));

        // this test class i.e. the below "this" does not imlpement the interface so therefore assertFalse below
        assertFalse(CrsTransformationAdapterLeafFactory.isCrsTransformationAdapter(this.getClass().getName()));

        assertTrue(CrsTransformationAdapterLeafFactory.isCrsTransformationAdapter(CrsTransformationAdapterGooberCTL.class.getName()));
        assertTrue(CrsTransformationAdapterLeafFactory.isCrsTransformationAdapter(CrsTransformationAdapterGeoPackageNGA.class.getName()));
        assertTrue(CrsTransformationAdapterLeafFactory.isCrsTransformationAdapter(CrsTransformationAdapterGeoTools.class.getName()));
        assertTrue(CrsTransformationAdapterLeafFactory.isCrsTransformationAdapter(CrsTransformationAdapterOrbisgisCTS.class.getName()));
        assertTrue(CrsTransformationAdapterLeafFactory.isCrsTransformationAdapter(CrsTransformationAdapterProj4J.class.getName()));

        List<String> classNamesForAllKnownImplementations = CrsTransformationAdapterLeafFactory.getClassNamesForAllKnownImplementations();
        for (String className : classNamesForAllKnownImplementations) {
            assertTrue(CrsTransformationAdapterLeafFactory.isCrsTransformationAdapter(className));
        }
    }

    public final static int NUMBER_OF_IMPLEMENTATIONS = 5;

    @Test
    void getInstancesOfAllKnownAvailableImplementations() {
        List<CrsTransformationAdapter> list = CrsTransformationAdapterLeafFactory.getInstancesOfAllKnownAvailableImplementations();
        assertEquals(NUMBER_OF_IMPLEMENTATIONS, list.size());
        for (CrsTransformationAdapter crsTransformationAdapter : list) {
            assertNotNull(crsTransformationAdapter);
        }
    }

    @Test
    void verifyThatNoKnownClassNamesHaveBeenRenamed() {
        //    System.out.println(CrsTransformationAdapterGooberCTL.class.getName());
        //    System.out.println(CrsTransformationAdapterGeoPackageNGA.class.getName());
        //    System.out.println(CrsTransformationAdapterGeoTools.class.getName());
        //    System.out.println(CrsTransformationAdapterOrbisgisCTS.class.getName());
        //    System.out.println(CrsTransformationAdapterProj4J.class.getName());

        List<String> classNamesForAllKnownImplementations = CrsTransformationAdapterLeafFactory.getClassNamesForAllKnownImplementations();
        assertEquals(NUMBER_OF_IMPLEMENTATIONS, classNamesForAllKnownImplementations.size());

        assertTrue(classNamesForAllKnownImplementations.contains("com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL"));
        assertTrue(classNamesForAllKnownImplementations.contains("com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA"));
        assertTrue(classNamesForAllKnownImplementations.contains("com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools"));
        assertTrue(classNamesForAllKnownImplementations.contains("com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS"));
        assertTrue(classNamesForAllKnownImplementations.contains("com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J"));

        assertTrue(classNamesForAllKnownImplementations.contains(CrsTransformationAdapterGooberCTL.class.getName()));
        assertTrue(classNamesForAllKnownImplementations.contains(CrsTransformationAdapterGeoPackageNGA.class.getName()));
        assertTrue(classNamesForAllKnownImplementations.contains(CrsTransformationAdapterGeoTools.class.getName()));
        assertTrue(classNamesForAllKnownImplementations.contains(CrsTransformationAdapterOrbisgisCTS.class.getName()));
        assertTrue(classNamesForAllKnownImplementations.contains(CrsTransformationAdapterProj4J.class.getName()));
    }
}