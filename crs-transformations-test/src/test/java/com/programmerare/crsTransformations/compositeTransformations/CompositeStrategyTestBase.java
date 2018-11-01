package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformationFacadeGeoPackageNGA.CrsTransformationFacadeGeoPackageNGA;
import com.programmerare.crsTransformationFacadeGeoTools.CrsTransformationFacadeGeoTools;
import com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL;
import com.programmerare.crsTransformationFacadeOrbisgisCTS.CrsTransformationFacadeOrbisgisCTS;
import com.programmerare.crsTransformationFacadeProj4J.CrsTransformationFacadeProj4J;
import com.programmerare.crsTransformations.CrsTransformationFacade;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;

import org.junit.jupiter.api.BeforeAll;

import java.util.Arrays;
import java.util.List;

abstract class CompositeStrategyTestBase {

    protected static CrsTransformationFacade facadeGeoTools;
    protected static CrsTransformationFacade facadeGooberCTL;
    protected static CrsTransformationFacade facadeOrbisgisCTS;
    protected static CrsTransformationFacade facadeProj4J;
    protected static CrsTransformationFacade facadeGeoPackageNGA;

    protected static List<CrsTransformationFacade> allFacades;
    protected static List<Coordinate> allCoordinateResultsForTheDifferentImplementations;

    protected static double wgs84Lat = 59.330231;
    protected static double wgs84Lon = 18.059196;
    protected static double sweref99_Y_expected = 6580822;
    protected static double sweref99_X_expected = 674032;

    protected static Coordinate wgs84coordinate;
    protected static Coordinate resultCoordinateGeoTools;
    protected static Coordinate resultCoordinateGooberCTL;
    protected static Coordinate resultCoordinateOrbisgisCTS;
    protected static Coordinate resultCoordinateProj4J;
    protected static Coordinate resultCoordinateGeoPackageNGA;

    @BeforeAll
    final static void beforeAll() {
        facadeGeoTools = new CrsTransformationFacadeGeoTools();
        facadeGooberCTL = new CrsTransformationFacadeGooberCTL();
        facadeOrbisgisCTS = new CrsTransformationFacadeOrbisgisCTS();
        facadeProj4J = new CrsTransformationFacadeProj4J();
        facadeGeoPackageNGA = new CrsTransformationFacadeGeoPackageNGA();

        allFacades = Arrays.asList(
            // Regarding the order of the items in the list below:
            // GeoTools should be the first since it is assumed in the test by the subclass CompositeStrategyForChainOfResponsibilityTest
            facadeGeoTools,
            facadeGooberCTL,
            facadeOrbisgisCTS,
            facadeProj4J,
            facadeGeoPackageNGA
        );

        wgs84coordinate = Coordinate.createFromYLatitudeXLongitude(wgs84Lat, wgs84Lon, EpsgNumber._4326__WGS_84__WORLD);

        resultCoordinateGeoTools = facadeGeoTools.transformToCoordinate(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        resultCoordinateGooberCTL = facadeGooberCTL.transformToCoordinate(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        resultCoordinateOrbisgisCTS = facadeOrbisgisCTS.transformToCoordinate(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        resultCoordinateProj4J = facadeProj4J.transformToCoordinate(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        resultCoordinateGeoPackageNGA = facadeGeoPackageNGA.transformToCoordinate(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        allCoordinateResultsForTheDifferentImplementations = Arrays.asList(
            resultCoordinateGeoTools,
            resultCoordinateGooberCTL,
            resultCoordinateOrbisgisCTS,
            resultCoordinateProj4J,
            resultCoordinateGeoPackageNGA
        );
    }
}
