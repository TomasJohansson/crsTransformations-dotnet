package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformations.CrsTransformationFacade;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.crsConstants.ConstantEpsgNumber;
import org.junit.jupiter.api.Test;
import java.util.Arrays;
import static org.junit.jupiter.api.Assertions.assertEquals;

public class CompositeStrategyForChainOfResponsibilityTest extends CompositeStrategyTestBase {

    @Test
    void createCRStransformationFacadeChainOfResponsibility() {
        CrsTransformationFacade facadeComposite = CrsTransformationFacadeComposite.createCrsTransformationChainOfResponsibility(
            Arrays.asList(
                facadeGeoTools, // since geotools is first here in this list, it should be the implementation providing the result
                facadeGooberCTL,
                facadeOrbisgisCTS
                    // TODO: add usage of the added implementation Proj4J
                    // TODO: add usage of the added implementation GeoPackageNGA
            )
        );
        Coordinate coordinateReturnedByCompositeFacadeChainOfResponsibility = facadeComposite.transform(wgs84coordinate, ConstantEpsgNumber.SWEREF99TM);
        // The above result of the composite should be equal to the result of GeoTools since it
        // is first in the list of parameters to the constructor and it should produce a result for
        // the input coordinates ... so therefore below assert against the direct result of geotools
        Coordinate coordinateResultWhenUsingGeoTools = facadeGeoTools.transform(wgs84coordinate, ConstantEpsgNumber.SWEREF99TM);
        assertEquals(coordinateResultWhenUsingGeoTools, coordinateReturnedByCompositeFacadeChainOfResponsibility);
    }
}
