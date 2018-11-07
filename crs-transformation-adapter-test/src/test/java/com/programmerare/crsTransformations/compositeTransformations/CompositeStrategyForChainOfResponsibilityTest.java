package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsTransformations.CrsTransformationAdapter;
import com.programmerare.crsTransformations.CrsTransformationResult;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class CompositeStrategyForChainOfResponsibilityTest extends CompositeStrategyTestBase {

    @Test
    void createCRSTransformationAdapterChainOfResponsibility() {
        CrsTransformationAdapter chainOfResponsibilityCompositeAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationChainOfResponsibility(
            // note that geotools should be the first item in the below list defined in the baseclass,
            // and therefore geotools should be the implementation providing the result
            super.allAdapters
        );
        CrsTransformationResult chainOfResponsibilityResult = chainOfResponsibilityCompositeAdapter.transform(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        assertNotNull(chainOfResponsibilityResult);
        assertTrue(chainOfResponsibilityResult.isSuccess());
        assertEquals(1, chainOfResponsibilityResult.getTransformationResultChildren().size());

        CrsCoordinate coordinateReturnedByCompositeAdapterChainOfResponsibility = chainOfResponsibilityResult.getOutputCoordinate();
        // The above result of the composite should be equal to the result of GeoTools since it
        // is first in the list of parameters to the constructor and it should produce a result for
        // the input coordinates ... so therefore below assert against the direct result of geotools
        CrsCoordinate coordinateResultWhenUsingGeoTools = adapterGeoTools.transformToCoordinate(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        assertEquals(coordinateResultWhenUsingGeoTools, coordinateReturnedByCompositeAdapterChainOfResponsibility);
    }
}
