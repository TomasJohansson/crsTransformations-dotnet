package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformations.CrsTransformationAdapter;
import org.junit.jupiter.api.Test;

import java.util.ArrayList;
import java.util.List;
import java.util.function.Function;

import static com.programmerare.crsTransformations.CrsTransformationAdapterLeafFactoryTest.EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS;
import static org.hamcrest.CoreMatchers.containsString;
import static org.hamcrest.MatcherAssert.assertThat;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertThrows;

public class CrsTransformationAdapterCompositeFactoryTest {

    @Test
    void createCrsTransformationAverage_shouldBeCreatedWithManyImplementations_whenInstantiatingWithoutParameters() {
        CrsTransformationAdapterComposite crsTransformationAverage = CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage();
        assertCompositeNotNullAndAggregatesManyImplementations(crsTransformationAverage);
    }

    @Test
    void createCrsTransformationMedian_shouldBeCreatedWithManyImplementations_whenInstantiatingWithoutParameters() {
        CrsTransformationAdapterComposite crsTransformationMedian = CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian();
        assertCompositeNotNullAndAggregatesManyImplementations(crsTransformationMedian);
    }

    @Test
    void createCrsTransformationFirstSuccess_shouldBeCreatedWithManyImplementations_whenInstantiatingWithoutParameters() {
        CrsTransformationAdapterComposite crsTransformationFirstSuccess = CrsTransformationAdapterCompositeFactory.createCrsTransformationFirstSuccess();
        assertCompositeNotNullAndAggregatesManyImplementations(crsTransformationFirstSuccess);
    }

    private void assertCompositeNotNullAndAggregatesManyImplementations(CrsTransformationAdapterComposite crsTransformationAdapterComposite) {
        assertNotNull(crsTransformationAdapterComposite);
        List<CrsTransformationAdapter> list = crsTransformationAdapterComposite.getCompositeStrategy()._getAllTransformationAdaptersInTheOrderTheyShouldBeInvoked();
        assertEquals(EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS, list.size());
    }

    private final static List<CrsTransformationAdapter> emptyListOfCrsTransformationAdapters = new ArrayList<>();

    @Test
    void createCrsTransformationAverage_shouldThrowException_whenInstantiatingWithEmptyList() {
        helper_shouldThrowException_whenInstantiatingWithEmptyList(
            list -> CrsTransformationAdapterCompositeFactory.createCrsTransformationAverage(list)
        );
    }

    @Test
    void createCrsTransformationMedian_shouldThrowException_whenInstantiatingWithEmptyList() {
        helper_shouldThrowException_whenInstantiatingWithEmptyList(
            list -> CrsTransformationAdapterCompositeFactory.createCrsTransformationMedian(list)
        );
    }

    @Test
    void createCrsTransformationFirstSuccess_shouldThrowException_whenInstantiatingWithEmptyList() {
        helper_shouldThrowException_whenInstantiatingWithEmptyList(
            list -> CrsTransformationAdapterCompositeFactory.createCrsTransformationFirstSuccess(list)
        );
    }    
    
    private void helper_shouldThrowException_whenInstantiatingWithEmptyList(
        final Function<List<CrsTransformationAdapter>, CrsTransformationAdapterComposite> compositeCreator            
    ) {
        Throwable exception = assertThrows(
            Throwable.class,
            () -> compositeCreator.apply(emptyListOfCrsTransformationAdapters)
        );
        assertNotNull(exception);
        // the exception message might be something like:
        // 'Composite' adapter can not be created with an empty list of 'leaf' adapters
        // At least it should contain the word "empty" (is the assumption in the below test)
        assertThat(exception.getMessage(), containsString("empty"));
    }    
}