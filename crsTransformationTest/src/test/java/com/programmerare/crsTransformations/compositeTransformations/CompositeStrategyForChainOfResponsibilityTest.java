package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformations.CrsTransformationFacade;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber;
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
                facadeOrbisgisCTS,
                facadeProj4J,
                facadeGeoPackageNGA
            )
        );
        Coordinate coordinateReturnedByCompositeFacadeChainOfResponsibility = facadeComposite.transform(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        // The above result of the composite should be equal to the result of GeoTools since it
        // is first in the list of parameters to the constructor and it should produce a result for
        // the input coordinates ... so therefore below assert against the direct result of geotools
        Coordinate coordinateResultWhenUsingGeoTools = facadeGeoTools.transform(wgs84coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN);
        assertEquals(coordinateResultWhenUsingGeoTools, coordinateReturnedByCompositeFacadeChainOfResponsibility);
    }
}
