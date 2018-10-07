package com.programmerare.crsTransformations.extensionfunctions;

// Java test for Kotlin file "ExtensionsForString.kt"
// labeled with @file:JvmName("StringUtils")

import com.programmerare.crsTransformationFacadeOrbisgisCTS.CrsTransformationFacadeOrbisgisCTS;
import com.programmerare.crsTransformations.CrsTransformationFacade;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertThrows;

class StringUtilsTest {

    @Test
    void createCrsTransformationFacadeFromFullClassName() {
        String className = CrsTransformationFacadeOrbisgisCTS.class.getName();
        CrsTransformationFacade crsTransformationFacade = StringUtils.createCrsTransformationFacadeFromFullClassName(className);
        assertNotNull(crsTransformationFacade);
    }

    @Test
    void createCrsTransformationFacadeFromUnvalidClassNameShouldFail() {
        String classNameNotExisting = "classNameNotExisting";
        Exception illegalStateException = assertThrows(
            ClassNotFoundException.class,
            () -> {
                StringUtils.createCrsTransformationFacadeFromFullClassName(classNameNotExisting);
            },
            "should not be aqble to create class from unvalid classname"
        );
    }

}
