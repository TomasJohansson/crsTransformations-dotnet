package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformations.CrsTransformationFacade;
import org.junit.jupiter.api.Test;

import java.util.List;

import static com.programmerare.crsTransformations.CrsTransformationFacadeLeafFactoryTest.NUMBER_OF_IMPLEMENTATIONS;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

public class CrsTransformationFacadeCompositeFactoryTest {

    @Test
    void createCrsTransformationAverage() {
        CrsTransformationFacadeComposite crsTransformationAverage = CrsTransformationFacadeCompositeFactory.createCrsTransformationAverage();
        assertCompositeNotNullAndAggregatesManyImplementations(crsTransformationAverage);
    }

    @Test
    void createCrsTransformationMedian() {
        CrsTransformationFacadeComposite crsTransformationMedian = CrsTransformationFacadeCompositeFactory.createCrsTransformationMedian();
        assertCompositeNotNullAndAggregatesManyImplementations(crsTransformationMedian);
    }

    @Test
    void createCrsTransformationChainOfResponsibility() {
        CrsTransformationFacadeComposite crsTransformationChainOfResponsibility = CrsTransformationFacadeCompositeFactory.createCrsTransformationChainOfResponsibility();
        assertCompositeNotNullAndAggregatesManyImplementations(crsTransformationChainOfResponsibility);
    }

    private void assertCompositeNotNullAndAggregatesManyImplementations(CrsTransformationFacadeComposite crsTransformationFacadeComposite) {
        assertNotNull(crsTransformationFacadeComposite);
        List<CrsTransformationFacade> list = crsTransformationFacadeComposite.getCompositeStrategy().getAllTransformationFacadesInTheOrderTheyShouldBeInvoked();
        assertEquals(NUMBER_OF_IMPLEMENTATIONS, list.size());
    }
}