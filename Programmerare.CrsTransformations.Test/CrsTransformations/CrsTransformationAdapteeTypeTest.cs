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
    
    void orbisgisAdapter_shouldMatchExpectedEnumAndJarfileNameWithVersion() {
        verifyExpectedEnumAndJarfileVersion(
            new CrsTransformationAdapterOrbisgisCTS(),
            "cts-1.5.1.jar",
            CrsTransformationAdapteeType.LEAF_ORBISGIS_1_5_1
        );
    }
    
    @Test
    void geotoolsAdapter_shouldMatchExpectedEnumAndJarfileNameWithVersion() {
        verifyExpectedEnumAndJarfileVersion(
            new CrsTransformationAdapterGeoTools(),
            "gt-api-20.0.jar",
            CrsTransformationAdapteeType.LEAF_GEOTOOLS_20_0
        );
    }

    @Test
    void geopackageNgaAdapter_shouldMatchExpectedEnumAndJarfileNameWithVersion() {
        verifyExpectedEnumAndJarfileVersion(
            new CrsTransformationAdapterGeoPackageNGA(),
            "geopackage-core-3.1.0.jar",
            CrsTransformationAdapteeType.LEAF_NGA_GEOPACKAGE_3_1_0
        );
    }
    
    @Test
    void proj4jAdapter_shouldMatchExpectedEnumAndJarfileNameWithVersion() {
        verifyExpectedEnumAndJarfileVersion(
            new CrsTransformationAdapterProj4J(),
            "proj4j-0.1.0.jar",
            CrsTransformationAdapteeType.LEAF_PROJ4J_0_1_0
        );
    }

    @Test
    void gooberAdapter_shouldMatchExpectedEnumAndJarfileNameWithVersion() {
        verifyExpectedEnumAndJarfileVersion(
            new CrsTransformationAdapterGooberCTL(),
            "coordinate-transformation-library-1.1.jar",
            CrsTransformationAdapteeType.LEAF_GOOBER_1_1
        );
    }

    private void verifyExpectedEnum(
        CrsTransformationAdapterBase crsTransformationAdapter,
        CrsTransformationAdapteeType expectedAdaptee
    ) {
        verifyExpectedEnumAndJarfileVersion(
            crsTransformationAdapter,
            "",
            expectedAdaptee
        );
    }
    private void verifyExpectedEnumAndJarfileVersion(
        CrsTransformationAdapterBase crsTransformationAdapter,
        String emptyStringOrExpectedNameOfJarFile,
        CrsTransformationAdapteeType expectedEnumWithMatchingNameInlcudingVersionNumber
    ) {
        assertEquals(
            expectedEnumWithMatchingNameInlcudingVersionNumber, 
            crsTransformationAdapter.getAdapteeType()
        );
        String fileNameIncludingPath = crsTransformationAdapter.getNameOfJarFileOrEmptyString();
        if(!emptyStringOrExpectedNameOfJarFile.equals("")) {
            assertThat(
                "Likely failure reason: You have upgraded a version. If so, then upgrade both the enum value and the filename",
                fileNameIncludingPath, endsWith(emptyStringOrExpectedNameOfJarFile)
            );
        }
    }

    @Test
    void testCompositeAverage() {
        verifyExpectedEnum(
            CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage(),
            CrsTransformationAdapteeType.COMPOSITE_AVERAGE
        );
    }

    @Test
    void testCompositeMedian() {
        verifyExpectedEnum(
            CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian(),
            CrsTransformationAdapteeType.COMPOSITE_MEDIAN
        );
    }

    @Test
    void testCompositeFirstSuccess() {
        verifyExpectedEnum(
            CrsTransformationAdapterCompositeFactory.createCrsTransformationFirstSuccess(),
            CrsTransformationAdapteeType.COMPOSITE_FIRST_SUCCESS
        );
    }

    @Test
    void testCompositeWeightedAverage() {
        verifyExpectedEnum(
            CrsTransformationAdapterCompositeFactory.createCrsTransformationWeightedAverage(
                Arrays.asList(CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterGeoPackageNGA(), 1))
            ),
            CrsTransformationAdapteeType.COMPOSITE_WEIGHTED_AVERAGE
        );
    }

}