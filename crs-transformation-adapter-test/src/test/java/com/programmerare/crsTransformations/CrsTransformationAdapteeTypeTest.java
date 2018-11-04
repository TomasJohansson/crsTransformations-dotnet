package com.programmerare.crsTransformations;

import static org.hamcrest.CoreMatchers.endsWith;
import com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA;
import com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools;
import com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL;
import com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS;
import com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J;
import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterCompositeFactory;
import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterWeight;
import org.junit.jupiter.api.Test;

import java.util.Arrays;

import static org.hamcrest.MatcherAssert.assertThat;
import static org.junit.jupiter.api.Assertions.assertEquals;

class CrsTransformationAdapteeTypeTest {

    @Test
    void testOrbisgisCTSVersion() {
        verifyExpectedVersion(
            new CrsTransformationAdapterOrbisgisCTS(),
            "cts-1.5.1.jar",
            CrsTransformationAdapteeType.LEAF_ORBISGIS_1_5_1
        );
    }
    @Test
    void testGeoToolsVersion() {
        verifyExpectedVersion(
            new CrsTransformationAdapterGeoTools(),
            "gt-api-20.0.jar",
            CrsTransformationAdapteeType.LEAF_GEOTOOLS_20_0
        );
    }
    @Test
    void testGeoPackageNGAVersion() {
        verifyExpectedVersion(
            new CrsTransformationAdapterGeoPackageNGA(),
            "geopackage-core-3.1.0.jar",
            CrsTransformationAdapteeType.LEAF_NGA_GEOPACKAGE_3_1_0
        );
    }
    @Test
    void testProj4JVersion() {
        verifyExpectedVersion(
            new CrsTransformationAdapterProj4J(),
            "proj4j-0.1.0.jar",
            CrsTransformationAdapteeType.LEAF_PROJ4J_0_1_0
        );
    }

    @Test
    void testGooberVersion() {
        verifyExpectedVersion(
            new CrsTransformationAdapterGooberCTL(),
            "coordinate-transformation-library-1.1.jar",
            CrsTransformationAdapteeType.LEAF_GOOBER_1_1
        );
    }

    private void verifyExpectedVersion(
        CrsTransformationAdapterBase crsTransformationAdapter,
        CrsTransformationAdapteeType expectedAdaptee
    ) {
        verifyExpectedVersion(
            crsTransformationAdapter,
            "",
            expectedAdaptee
        );
    }
    private void verifyExpectedVersion(
        CrsTransformationAdapterBase crsTransformationAdapter,
        String expectedFileNameOrEmptyString,
        CrsTransformationAdapteeType expectedAdaptee
    ) {
        assertEquals(expectedAdaptee, crsTransformationAdapter.getAdapteeType());
        String fileNameIncludingPath = crsTransformationAdapter.getNameOfJarFileOrEmptyString();
        if(!expectedFileNameOrEmptyString.equals("")) {
            assertThat(
                "Likely failure reason: You have upgraded a version. If so, then upgrade both the enum value and the filename",
                fileNameIncludingPath, endsWith(expectedFileNameOrEmptyString)
            );
        }
    }

    @Test
    void testCompositeAverage() {
        verifyExpectedVersion(
            CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage(),
            CrsTransformationAdapteeType.COMPOSITE_AVERAGE
        );
    }

    @Test
    void testCompositeMedian() {
        verifyExpectedVersion(
            CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian(),
            CrsTransformationAdapteeType.COMPOSITE_MEDIAN
        );
    }

    @Test
    void testCompositeChainOfResponsibility() {
        verifyExpectedVersion(
            CrsTransformationAdapterCompositeFactory.createCrsTransformationChainOfResponsibility(),
            CrsTransformationAdapteeType.COMPOSITE_CHAIN_OF_RESPONSIBILITY
        );
    }

    @Test
    void testCompositeWeightedAverage() {
        verifyExpectedVersion(
            CrsTransformationAdapterCompositeFactory.createCrsTransformationWeightedAverage(
                Arrays.asList(CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterGeoPackageNGA(), 1))
            ),
            CrsTransformationAdapteeType.COMPOSITE_WEIGHTED_AVERAGE
        );
    }

}