package com.programmerare.crsTransformations.compositeTransformations;

// TODO: REFACTOR this test class !

import com.programmerare.crsTransformationFacadeGeoTools.CRStransformationFacadeGeoTools;
import com.programmerare.crsTransformationFacadeGooberCTL.CRStransformationFacadeGooberCTL;
import com.programmerare.crsTransformationFacadeOrbisgisCTS.CRStransformationFacadeOrbisgisCTS;
import com.programmerare.crsTransformations.CRStransformationFacade;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.crsConstants.ConstantEpsgNumber;
import org.geotools.factory.GeoTools;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.stream.IntStream;

import static org.junit.jupiter.api.Assertions.*;
import static java.util.stream.Collectors.toList;
import static org.junit.jupiter.api.Assertions.assertEquals;

public class CRStransformationFacadeChainOfResponsibilityTest extends CRStransformationFacadeBaseCompositeTest {

    @Test
    void createCRStransformationFacadeChainOfResponsibility() {
        assertNotNull(facadeGeoTools); // should have been created in the base class

        double wgs84Lat = 59.330231;
        double wgs84Lon = 18.059196;

        Coordinate wgs84coordinate = Coordinate.createFromYLatXLong(wgs84Lat, wgs84Lon, ConstantEpsgNumber.WGS84);

//        double sweref99_Y_expected = 6580822;
//        double sweref99_X_expected = 674032;
        CRStransformationFacade facadeComposite = new CRStransformationFacadeChainOfResponsibility(Arrays.asList(facadeGeoTools, facadeGooberCTL, facadeOrbisgisCTS));
        Coordinate coordResultComposite = facadeComposite.transform(wgs84coordinate, ConstantEpsgNumber.SWEREF99TM);
        System.out.println("coordResultComposite: " + coordResultComposite);

        // the above result of the composite should be equal to the result of GeoTools since it
        // is first in the list of parameters to the constructor and it should produce a result for
        // the input coordinates ... so therefore below assert against the direct result of geotools

        Coordinate coordinateResultWithGeoTools = facadeGeoTools.transform(wgs84coordinate, ConstantEpsgNumber.SWEREF99TM);

        double delta = 0.0000001;
        assertEquals(coordinateResultWithGeoTools.getYLatitude(), coordResultComposite.getYLatitude(), delta);
        assertEquals(coordinateResultWithGeoTools.getXLongitude(), coordResultComposite.getXLongitude(), delta);
        assertEquals(coordinateResultWithGeoTools, coordResultComposite);
    }
}
