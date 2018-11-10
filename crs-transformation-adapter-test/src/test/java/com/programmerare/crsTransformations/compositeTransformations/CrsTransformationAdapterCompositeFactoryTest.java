package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformations.CrsTransformationAdapter;
import org.junit.jupiter.api.Test;

import java.util.List;

import static com.programmerare.crsTransformations.CrsTransformationAdapterLeafFactoryTest.EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

public class CrsTransformationAdapterCompositeFactoryTest {

    @Test
    void createCrsTransformationAverage() {
        CrsTransformationAdapterComposite crsTransformationAverage = CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage();
        assertCompositeNotNullAndAggregatesManyImplementations(crsTransformationAverage);
    }

    @Test
    void createCrsTransformationMedian() {
        CrsTransformationAdapterComposite crsTransformationMedian = CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian();
        assertCompositeNotNullAndAggregatesManyImplementations(crsTransformationMedian);
    }

    @Test
    void createCrsTransformationChainOfResponsibility() {
        CrsTransformationAdapterComposite crsTransformationChainOfResponsibility = CrsTransformationAdapterCompositeFactory.createCrsTransformationChainOfResponsibility();
        assertCompositeNotNullAndAggregatesManyImplementations(crsTransformationChainOfResponsibility);
    }

    private void assertCompositeNotNullAndAggregatesManyImplementations(CrsTransformationAdapterComposite crsTransformationAdapterComposite) {
        assertNotNull(crsTransformationAdapterComposite);
        List<CrsTransformationAdapter> list = crsTransformationAdapterComposite.getCompositeStrategy().getAllTransformationAdaptersInTheOrderTheyShouldBeInvoked();
        assertEquals(EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS, list.size());
    }
}