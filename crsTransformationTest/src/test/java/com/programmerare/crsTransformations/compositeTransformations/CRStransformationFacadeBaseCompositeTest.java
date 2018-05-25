package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformationFacadeGeoTools.CrsTransformationFacadeGeoTools;
import com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL;
import com.programmerare.crsTransformationFacadeOrbisgisCTS.CrsTransformationFacadeOrbisgisCTS;
import com.programmerare.crsTransformationFacadeProj4J.CrsTransformationFacadeProj4J;
import com.programmerare.crsTransformations.CrsTransformationFacade;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.crsConstants.ConstantEpsgNumber;
import org.junit.jupiter.api.BeforeAll;

import java.util.Arrays;
import java.util.List;

abstract class CRStransformationFacadeBaseCompositeTest  {

    protected static CrsTransformationFacade facadeGeoTools;
    protected static CrsTransformationFacade facadeGooberCTL;
    protected static CrsTransformationFacade facadeOrbisgisCTS;
    protected static CrsTransformationFacade facadeProj4J;
    protected static List<CrsTransformationFacade> allFacades;

    protected static double wgs84Lat = 59.330231;
    protected static double wgs84Lon = 18.059196;
    protected static double sweref99_Y_expected = 6580822;
    protected static double sweref99_X_expected = 674032;

    protected static Coordinate wgs84coordinate;
    protected static Coordinate resultCoordinateGeoTools;
    protected static Coordinate resultCoordinateGooberCTL;
    protected static Coordinate resultCoordinateOrbisgisCTS;
    protected static Coordinate resultCoordinateProj4J;


    @BeforeAll
    final static void beforeAll() {
        facadeGeoTools = new CrsTransformationFacadeGeoTools();
        facadeGooberCTL = new CrsTransformationFacadeGooberCTL();
        facadeOrbisgisCTS = new CrsTransformationFacadeOrbisgisCTS();
        facadeProj4J = new CrsTransformationFacadeProj4J();

        allFacades = Arrays.asList(facadeGeoTools, facadeGooberCTL, facadeOrbisgisCTS);

        wgs84coordinate = Coordinate.createFromYLatXLong(wgs84Lat, wgs84Lon, ConstantEpsgNumber.WGS84);

        resultCoordinateGeoTools = facadeGeoTools.transform(wgs84coordinate, ConstantEpsgNumber.SWEREF99TM);
        resultCoordinateGooberCTL = facadeGooberCTL.transform(wgs84coordinate, ConstantEpsgNumber.SWEREF99TM);
        resultCoordinateOrbisgisCTS = facadeOrbisgisCTS.transform(wgs84coordinate, ConstantEpsgNumber.SWEREF99TM);
        resultCoordinateProj4J = facadeProj4J.transform(wgs84coordinate, ConstantEpsgNumber.SWEREF99TM);
    }
}
