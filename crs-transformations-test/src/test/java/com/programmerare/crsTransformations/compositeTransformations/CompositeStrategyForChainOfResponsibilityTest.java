package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.CrsTransformationFacade;
import com.programmerare.crsTransformations.TransformResult;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class CompositeStrategyForChainOfResponsibilityTest extends CompositeStrategyTestBase {

    @Test
    void createCRStransformationFacadeChainOfResponsibility() {
        CrsTransformationFacade chainOfResponsibilityCompositeFacade = CrsTransformationFacadeCompositeFactory.createCrsTransformationChainOfResponsibility(
            // note that geotools should be the first item in the below list defined in the baseclass,
            // and therefore geotools should be the implementation providing the result
            super.allFacades
        );
        TransformResult chainOfResponsibilityResult = chainOfResponsibilityCompositeFacade.transform(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        assertNotNull(chainOfResponsibilityResult);
        assertTrue(chainOfResponsibilityResult.isSuccess());
        assertEquals(1, chainOfResponsibilityResult.getSubResults().size());

        Coordinate coordinateReturnedByCompositeFacadeChainOfResponsibility = chainOfResponsibilityResult.getOutputCoordinate();
        // The above result of the composite should be equal to the result of GeoTools since it
        // is first in the list of parameters to the constructor and it should produce a result for
        // the input coordinates ... so therefore below assert against the direct result of geotools
        Coordinate coordinateResultWhenUsingGeoTools = facadeGeoTools.transformToCoordinate(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        assertEquals(coordinateResultWhenUsingGeoTools, coordinateReturnedByCompositeFacadeChainOfResponsibility);
    }
}
