package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.CrsTransformationFacade;
import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
import org.junit.jupiter.api.Test;
import java.util.Arrays;
import static org.junit.jupiter.api.Assertions.assertEquals;

class CompositeStrategyForMedianValueTest extends CompositeStrategyTestBase {

    private final static double delta = 0.00001;

    @Test
    void transformWithFacadeCompositeMedianTest() {
        CrsTransformationFacade facadeComposite = CrsTransformationFacadeComposite.createCrsTransformationMedian(
            Arrays.asList(
                facadeGeoTools,
                facadeGooberCTL,
                facadeProj4J,
                facadeOrbisgisCTS
                    // TODO: add usage of the added implementation GeoPackageNGA
            )
        );

        System.out.println(resultCoordinateProj4J);

        Coordinate coordinateReturnedByMedianFacade = facadeComposite.transform(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        // The same transformation as above has been done in the base class for the individual facades
        // The motviation for the below asserted values, read further down in the method.
        double expectedMedianLongitude = (resultCoordinateOrbisgisCTS.getXLongitude() + resultCoordinateGeoTools.getXLongitude()) / 2.0;
        double expectedMedianLatitude = (resultCoordinateOrbisgisCTS.getYLatitude() + resultCoordinateProj4J.getYLatitude()) / 2.0;
        assertEquals(expectedMedianLongitude, coordinateReturnedByMedianFacade.getXLongitude(), delta);
        assertEquals(expectedMedianLatitude, coordinateReturnedByMedianFacade.getYLatitude(), delta);

        // The median values are the following (using test data in the base class):
        // longitude: 674032.3571771549 (the average of the longitude from GeoTools and Orbis, see below)
        // latitude: 6580821.991121078 (the average of the latitude from Proj4J and Orbis, see below)

//        System.out.println(resultCoordinateGeoTools);
//        System.out.println(resultCoordinateGooberCTL);
//        System.out.println(resultCoordinateOrbisgisCTS);
//        System.out.println(resultCoordinateProj4J);
// Below are the outputs of the above statements (in the same order as above)
//        Coordinate(xLongitude=674032.3571771549, yLatitude=6580821.994371211, crsIdentifier=CrsIdentifier(crsCode=EPSG:3006, isEpsgCode=true, epsgNumber=3006))
//        Coordinate(xLongitude=674032.357, yLatitude=6580821.991, crsIdentifier=CrsIdentifier(crsCode=EPSG:3006, isEpsgCode=true, epsgNumber=3006))
//        Coordinate(xLongitude=674032.3573261796, yLatitude=6580821.991121078, crsIdentifier=CrsIdentifier(crsCode=EPSG:3006, isEpsgCode=true, epsgNumber=3006))
//        Coordinate(xLongitude=674032.357326444, yLatitude=6580821.991123579, crsIdentifier=CrsIdentifier(crsCode=EPSG:3006, isEpsgCode=true, epsgNumber=3006))

        // The above longitudes in sorted order, i.e. with the median in the middle
        assertEquals(674032.357, resultCoordinateGooberCTL.getXLongitude(), delta);
        assertEquals(674032.3571771549, resultCoordinateGeoTools.getXLongitude(), delta);
        assertEquals(674032.3573261796, resultCoordinateOrbisgisCTS.getXLongitude(), delta);
        assertEquals(674032.357326444, resultCoordinateProj4J.getXLongitude(), delta);

        // The latitudes in sorted order, i.e. with the median in the middle
        assertEquals(6580821.991, resultCoordinateGooberCTL.getYLatitude(), delta);
        assertEquals(6580821.991121078, resultCoordinateOrbisgisCTS.getYLatitude(), delta);
        assertEquals(6580821.991123579, resultCoordinateProj4J.getYLatitude(), delta);
        assertEquals(6580821.994371211, resultCoordinateGeoTools.getYLatitude(), delta);
    }

    @Test
    void getMedianFrom3values() {
        double medianValue = CompositeStrategyForMedianValue.getMedianValue(Arrays.asList(55.0, 33.0, 77.0));
        assertEquals(55.0, medianValue, delta);
    }

    @Test
    void getMedianFrom4values() {
        double medianValue = CompositeStrategyForMedianValue.getMedianValue(Arrays.asList(55.0, 33.0, 77.0, 35.0));
        // the average of the two middle values 35 and 55
        assertEquals(45.0, medianValue, delta);
    }

    @Test
    void getMedianFrom7values() {
        double medianValue = CompositeStrategyForMedianValue.getMedianValue(Arrays.asList(9.0, 6.0, 1.0, 7.0, 8.0, 5.0, 3.0));
        assertEquals(6.0, medianValue, delta);
    }

    @Test
    void getMedianFrom8values() {
        double medianValue = CompositeStrategyForMedianValue.getMedianValue(Arrays.asList(9.0, 6.0, 1.0, 7.0, 8.0, 5.0, 3.0, 6.5));
        // the average of the two middle values 6.0 and 6.5
        assertEquals(6.25, medianValue, delta);
    }

}