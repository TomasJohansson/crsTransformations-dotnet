package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA;
import com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools;
import com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL;
import com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS;
import com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J;
import com.programmerare.crsTransformations.CrsTransformationAdapter;
import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber;

import com.programmerare.crsTransformations.coordinate.CrsCoordinateFactory;
import org.junit.jupiter.api.BeforeAll;

import java.util.Arrays;
import java.util.List;

abstract class CompositeStrategyTestBase {

    protected static CrsTransformationAdapter adapterGeoTools;
    protected static CrsTransformationAdapter adapterGooberCTL;
    protected static CrsTransformationAdapter adapterOrbisgisCTS;
    protected static CrsTransformationAdapter adapterProj4J;
    protected static CrsTransformationAdapter adapterGeoPackageNGA;

    protected static List<CrsTransformationAdapter> allAdapters;
    protected static List<CrsCoordinate> allCoordinateResultsForTheDifferentImplementations;

    protected static double wgs84Lat = 59.330231;
    protected static double wgs84Lon = 18.059196;
    protected static double sweref99_Y_expected = 6580822;
    protected static double sweref99_X_expected = 674032;

    protected static CrsCoordinate wgs84coordinate;
    protected static CrsCoordinate resultCoordinateGeoTools;
    protected static CrsCoordinate resultCoordinateGooberCTL;
    protected static CrsCoordinate resultCoordinateOrbisgisCTS;
    protected static CrsCoordinate resultCoordinateProj4J;
    protected static CrsCoordinate resultCoordinateGeoPackageNGA;

    @BeforeAll
    final static void beforeAll() {
        adapterGeoTools = new CrsTransformationAdapterGeoTools();
        adapterGooberCTL = new CrsTransformationAdapterGooberCTL();
        adapterOrbisgisCTS = new CrsTransformationAdapterOrbisgisCTS();
        adapterProj4J = new CrsTransformationAdapterProj4J();
        adapterGeoPackageNGA = new CrsTransformationAdapterGeoPackageNGA();

        allAdapters = Arrays.asList(
            // Regarding the order of the items in the list below:
            // GeoTools should be the first since it is assumed in the test by the subclass CompositeStrategyForFirstSuccessTest
            adapterGeoTools,
            adapterGooberCTL,
            adapterOrbisgisCTS,
            adapterProj4J,
            adapterGeoPackageNGA
        );

        wgs84coordinate = CrsCoordinateFactory.createFromYNorthingLatitudeAndXEastingLongitude(wgs84Lat, wgs84Lon, EpsgNumber.WORLD__WGS_84__4326);

        resultCoordinateGeoTools = adapterGeoTools.transformToCoordinate(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        resultCoordinateGooberCTL = adapterGooberCTL.transformToCoordinate(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        resultCoordinateOrbisgisCTS = adapterOrbisgisCTS.transformToCoordinate(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        resultCoordinateProj4J = adapterProj4J.transformToCoordinate(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        resultCoordinateGeoPackageNGA = adapterGeoPackageNGA.transformToCoordinate(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        allCoordinateResultsForTheDifferentImplementations = Arrays.asList(
            resultCoordinateGeoTools,
            resultCoordinateGooberCTL,
            resultCoordinateOrbisgisCTS,
            resultCoordinateProj4J,
            resultCoordinateGeoPackageNGA
        );
    }
}
