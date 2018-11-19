package com.programmerare.crsTransformations.compositeTransformations;

import static org.junit.jupiter.api.Assertions.*;

import com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools;
import com.programmerare.crsTransformations.CrsTransformationAdapter;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;


class CrsTransformationAdapterWeightTest {

    private CrsTransformationAdapter crsTransformationAdapterInstanceNotNull;
    
    @BeforeEach
    void setup() {
        crsTransformationAdapterInstanceNotNull = new CrsTransformationAdapterGeoTools();    
    }
    
    @Test
    void createFromInstance_shouldThrowException_whenAdapterParameterIsNull() {
        assertThrows(
            IllegalArgumentException.class,
            () -> CrsTransformationAdapterWeight.createFromInstance(
                null, // adapter
                123 // weight
            )
        );
    }

    @Test
    void createFromInstance_shouldThrowException_whenWeightParameterIsNegative() {
        assertThrows(
            IllegalArgumentException.class,
            () -> CrsTransformationAdapterWeight.createFromInstance(
                crsTransformationAdapterInstanceNotNull,
                -1 // weight.  null weight leads to compiler error so that it imposslble
            )
        );
    }

    @Test
    void createFromInstance_shouldThrowException_whenWeightParameterIsZero() {
        assertThrows(
            IllegalArgumentException.class,
            () -> CrsTransformationAdapterWeight.createFromInstance(
                crsTransformationAdapterInstanceNotNull,
                0 // weight.  null weight leads to compiler error so that it imposslble
            )
        );
    }

    @Test
    void createFromStringWithFullClassNameForImplementation_shouldThrowException_whenTheClassIsNotImplementingTheExpectedInterface() {
        final String nameOfClassNotImplementingTheInterfaceCrsTransformationAdapter = this.getClass().getName();
        assertThrows(
            Exception.class, // not testing exactly what exception type but just some class inheriting from Exception 
            () -> CrsTransformationAdapterWeight.createFromStringWithFullClassNameForImplementation(
                nameOfClassNotImplementingTheInterfaceCrsTransformationAdapter,
                123 // weight
            )
        );        
    }

    @Test
    void createFromStringWithFullClassNameForImplementation_shouldSucceed_whenTheClassIsImplementingTheExpectedInterface() {
        final String nameOfClassImplementingTheInterfaceCrsTransformationAdapter = crsTransformationAdapterInstanceNotNull.getClass().getName();
        final double weightValue = 123;
        CrsTransformationAdapterWeight crsTransformationAdapterWeight = CrsTransformationAdapterWeight.createFromStringWithFullClassNameForImplementation(
            nameOfClassImplementingTheInterfaceCrsTransformationAdapter,
            weightValue
        );
        assertNotNull(crsTransformationAdapterWeight);
        assertEquals(
            nameOfClassImplementingTheInterfaceCrsTransformationAdapter,  
            crsTransformationAdapterWeight.getCrsTransformationAdapter().getClass().getName()
        );
        assertEquals(
            weightValue, // expected
            crsTransformationAdapterWeight.getWeight()
        );
    }

}