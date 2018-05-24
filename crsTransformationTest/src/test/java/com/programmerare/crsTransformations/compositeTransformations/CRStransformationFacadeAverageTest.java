package com.programmerare.crsTransformations.compositeTransformations;

// TODO: REFACTOR this test class !

import com.programmerare.crsTransformations.CRStransformationFacade;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.crsConstants.ConstantEpsgNumber;
import org.junit.jupiter.api.Test;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

public class CRStransformationFacadeAverageTest extends CRStransformationFacadeBaseCompositeTest {

    @Test
    void createCRStransformationFacadeAverage() {
        assertNotNull(facadeGooberCTL); // should have been created in the base class

        double wgs84Lat = 59.330231;
        double wgs84Lon = 18.059196;

        Coordinate wgs84coordinate = Coordinate.createFromYLatXLong(wgs84Lat, wgs84Lon, ConstantEpsgNumber.WGS84);

        double sweref99_Y_expected = 6580822;
        double sweref99_X_expected = 674032;

        List<Coordinate> coordinateRersults = new ArrayList<>();
        double sumLat = 0.0;
        double sumLon = 0.0;
        for (CRStransformationFacade facade : allFacades) {
            Coordinate coordReasult = facade.transform(wgs84coordinate, ConstantEpsgNumber.SWEREF99TM);
            coordinateRersults.add(coordReasult);
            System.out.println(coordReasult);
            sumLat += coordReasult.getYLatitude();
            sumLon += coordReasult.getXLongitude();
        }
        double avgLat = sumLat / allFacades.size();
        double avgLon = sumLon / allFacades.size();

        CRStransformationFacade facadeComposite = new CRStransformationFacadeAverage(Arrays.asList(facadeGeoTools, facadeGooberCTL, facadeOrbisgisCTS));
        Coordinate coordResultComposite = facadeComposite.transform(wgs84coordinate, ConstantEpsgNumber.SWEREF99TM);
        coordinateRersults.add(coordResultComposite);
        System.out.println("coordResultComposite: " + coordResultComposite);

        double delta = 0.01;
        assertEquals(avgLat, coordResultComposite.getYLatitude(), delta);
        assertEquals(avgLon, coordResultComposite.getXLongitude(), delta);    }
}
