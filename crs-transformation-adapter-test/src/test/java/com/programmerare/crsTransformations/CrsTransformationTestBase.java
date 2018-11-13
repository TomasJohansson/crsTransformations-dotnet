package com.programmerare.crsTransformations;

import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA;
import com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools;
import com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL;
import com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS;
import com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J;
import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterCompositeFactory;
import com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterWeight;
import org.junit.jupiter.api.BeforeAll;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

abstract class CrsTransformationTestBase {

    protected final static int epsgNumberForWgs84         = EpsgNumber.WORLD__WGS_84__4326;
    protected final static int epsgNumberForSweref99TM    = EpsgNumber.SWEDEN__SWEREF99_TM__3006;
    protected final static int epsgNumberForRT90          = EpsgNumber.SWEDEN__2_5_GON_W__RT90_2_5_GON_V__3021;

    protected static List<CrsTransformationAdapter> crsTransformationAdapterLeafImplementations;
    protected static List<CrsTransformationAdapter> crsTransformationAdapterCompositeImplementations;
    protected static List<CrsTransformationAdapter> crsTransformationAdapterImplementations;

    @BeforeAll
    static void beforeAllInBaseClass() {

        crsTransformationAdapterLeafImplementations = Arrays.asList(
            new CrsTransformationAdapterGeoTools(),
            new CrsTransformationAdapterGooberCTL(),
            new CrsTransformationAdapterProj4J(),
            new CrsTransformationAdapterOrbisgisCTS(),
            new CrsTransformationAdapterGeoPackageNGA()
        );

        crsTransformationAdapterCompositeImplementations = Arrays.asList(
            CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage(),
            CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian(),
            CrsTransformationAdapterCompositeFactory.createCrsTransformationChainOfResponsibility(),
            CrsTransformationAdapterCompositeFactory.createCrsTransformationWeightedAverage(Arrays.asList(
                CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterGeoTools(), 51.0),
                CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterGooberCTL(), 52.0),
                CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterProj4J(), 53.0),
                CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterOrbisgisCTS(), 54.0),
                CrsTransformationAdapterWeight.createFromInstance(new CrsTransformationAdapterGeoPackageNGA(), 55.0)
            ))
        );
        crsTransformationAdapterImplementations = new ArrayList<>();
        crsTransformationAdapterImplementations.addAll(crsTransformationAdapterLeafImplementations);
        crsTransformationAdapterImplementations.addAll(crsTransformationAdapterCompositeImplementations);
    }
}