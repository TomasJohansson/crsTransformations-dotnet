package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.CrsTransformationFacade;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

public class CompositeStrategyForChainOfResponsibilityTest extends CompositeStrategyTestBase {

    @Test
    void createCRStransformationFacadeChainOfResponsibility() {
        CrsTransformationFacade facadeComposite = CrsTransformationFacadeComposite.createCrsTransformationChainOfResponsibility(
            // note that geotools should be the first item in the below list defined in the baseclass,
            // and therefore geotools should be the implementation providing the result
            super.allFacades
        );
        Coordinate coordinateReturnedByCompositeFacadeChainOfResponsibility = facadeComposite.transformToCoordinate(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        // The above result of the composite should be equal to the result of GeoTools since it
        // is first in the list of parameters to the constructor and it should produce a result for
        // the input coordinates ... so therefore below assert against the direct result of geotools
        Coordinate coordinateResultWhenUsingGeoTools = facadeGeoTools.transformToCoordinate(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        assertEquals(coordinateResultWhenUsingGeoTools, coordinateReturnedByCompositeFacadeChainOfResponsibility);
    }
}
