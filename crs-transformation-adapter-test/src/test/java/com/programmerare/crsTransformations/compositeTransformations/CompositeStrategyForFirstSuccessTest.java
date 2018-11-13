package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber;
import com.programmerare.crsTransformations.coordinate.CrsCoordinate;
import com.programmerare.crsTransformations.CrsTransformationAdapter;
import com.programmerare.crsTransformations.CrsTransformationResult;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class CompositeStrategyForFirstSuccessTest extends CompositeStrategyTestBase {

    @Test
    void transform_shouldReturnFirstResult_whenUsingFirstSuccessCompositeAdapter() {
        CrsTransformationAdapter firstSuccessCompositeAdapter = CrsTransformationAdapterCompositeFactory.createCrsTransformationFirstSuccess(
            // note that geotools should be the first item in the below list defined in the baseclass,
            // and therefore geotools should be the implementation providing the result
            super.allAdapters
        );
        CrsTransformationResult firstSuccessResult = firstSuccessCompositeAdapter.transform(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        assertNotNull(firstSuccessResult);
        assertTrue(firstSuccessResult.isSuccess());
        assertEquals(1, firstSuccessResult.getTransformationResultChildren().size());

        CrsCoordinate coordinateReturnedByCompositeAdapterFirstSuccess = firstSuccessResult.getOutputCoordinate();
        // The above result of the composite should be equal to the result of GeoTools since it
        // is first in the list of parameters to the constructor and it should produce a result for
        // the input coordinates ... so therefore below assert against the direct result of geotools
        CrsCoordinate coordinateResultWhenUsingGeoTools = adapterGeoTools.transformToCoordinate(wgs84coordinate, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
        assertEquals(coordinateResultWhenUsingGeoTools, coordinateReturnedByCompositeAdapterFirstSuccess);
    }
}
