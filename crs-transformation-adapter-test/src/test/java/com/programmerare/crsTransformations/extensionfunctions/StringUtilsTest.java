package com.programmerare.crsTransformations.extensionfunctions;

// Java test for Kotlin file "ExtensionsForString.kt"
// labeled with @file:JvmName("StringUtils")

import com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS;
import com.programmerare.crsTransformations.CrsTransformationAdapter;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertThrows;

class StringUtilsTest {

    @Test
    void createCrsTransformationAdapterFromFullClassName() {
        String className = CrsTransformationAdapterOrbisgisCTS.class.getName();
        CrsTransformationAdapter crsTransformationAdapter = StringUtils.createCrsTransformationAdapterFromFullClassName(className);
        assertNotNull(crsTransformationAdapter);
    }

    @Test
    void createCrsTransformationAdapterFromUnvalidClassNameShouldFail() {
        String classNameNotExisting = "classNameNotExisting";
        Exception illegalStateException = assertThrows(
            ClassNotFoundException.class,
            () -> {
                StringUtils.createCrsTransformationAdapterFromFullClassName(classNameNotExisting);
            },
            "should not be aqble to create class from unvalid classname"
        );
    }

}
