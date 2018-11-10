package com.programmerare.crsTransformations;

import com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA;
import com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools;
import com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL;
import com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS;
import com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;

import java.util.Arrays;
import java.util.List;

import static org.hamcrest.CoreMatchers.hasItem;
import static org.hamcrest.MatcherAssert.assertThat;
import static org.hamcrest.Matchers.greaterThanOrEqualTo;
import static org.junit.jupiter.api.Assertions.*;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class CrsTransformationAdapterLeafFactoryTest {

    public final static int EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS = 5;

    private static List<String> actualClassNamesForAllKnownImplementations;
    
    @BeforeAll
    static void beforeAll() {
        actualClassNamesForAllKnownImplementations = Arrays.asList(
            CrsTransformationAdapterGooberCTL.class.getName(),
            CrsTransformationAdapterGeoPackageNGA.class.getName(),
            CrsTransformationAdapterGeoTools.class.getName(),
            CrsTransformationAdapterOrbisgisCTS.class.getName(),
            CrsTransformationAdapterProj4J.class.getName()
        );        
    }

    @Test
    void listOfNonClassNamesForAdapters_shouldNotBeRecognizedAsAdapters() {
        final List<String> stringsNotBeingClassNameForAnyAdapter = Arrays.asList(
            null,
            "",
            "  ",
            " x ",
            "abc",

            // this test class i.e. the below "this" does not imlpement the interface so therefore assertFalse below
            this.getClass().getName()
        );

        for (String stringNotBeingClassNameForAnyAdapter : stringsNotBeingClassNameForAnyAdapter) {
            assertFalse(
                CrsTransformationAdapterLeafFactory.isCrsTransformationAdapter(stringNotBeingClassNameForAnyAdapter),
                "Should not have been recognized as adapter : " +  stringNotBeingClassNameForAnyAdapter
            );
        }        
    }

    @Test
    void listOfHardcodedClassnames_shouldBeCrsTransformationAdapters() {
        final List<String> hardcodedClassNamesForAllKnownImplementations = CrsTransformationAdapterLeafFactory.getClassNamesForAllKnownImplementations();
        for (String hardcodedClassNameForKnownImplementation : hardcodedClassNamesForAllKnownImplementations) {
            assertTrue(
                CrsTransformationAdapterLeafFactory.isCrsTransformationAdapter(hardcodedClassNameForKnownImplementation),
                "Name of failing class: " +  hardcodedClassNameForKnownImplementation
            );
        }
    }
    @Test
    void listOfHardcodedClassnames_shouldBeCreateableAsNonNullCrsTransformationAdapters() {
        final List<String> hardcodedClassNamesForAllKnownImplementations = CrsTransformationAdapterLeafFactory.getClassNamesForAllKnownImplementations();
        for (String hardcodedClassNameForKnownImplementation : hardcodedClassNamesForAllKnownImplementations) {
            assertNotNull(CrsTransformationAdapterLeafFactory.createCrsTransformationAdapter(hardcodedClassNameForKnownImplementation));
        }

    }    

    @Test
    void listOfKnownInstances_shouldOnlyContainNonNullObjectsAndTheNumberOfItemsShouldBeAtLeastFive() {
        List<CrsTransformationAdapter> list = CrsTransformationAdapterLeafFactory.getInstancesOfAllKnownAvailableImplementations();
        assertThat(list.size(), greaterThanOrEqualTo(EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS));
        for (CrsTransformationAdapter crsTransformationAdapter : list) {
            assertNotNull(crsTransformationAdapter);
        }
    }

    @Test
    void listOfHardcodedClassnames_shouldCorrespondToActualClassNames() {
       final List<String> hardcodedClassNamesForAllKnownImplementations = CrsTransformationAdapterLeafFactory.getClassNamesForAllKnownImplementations();
        assertEquals(EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS, hardcodedClassNamesForAllKnownImplementations.size());

        for (String actualClassNameForAnImplementation : actualClassNamesForAllKnownImplementations) {
            assertThat(hardcodedClassNamesForAllKnownImplementations, hasItem(actualClassNameForAnImplementation));
        }
    }
}