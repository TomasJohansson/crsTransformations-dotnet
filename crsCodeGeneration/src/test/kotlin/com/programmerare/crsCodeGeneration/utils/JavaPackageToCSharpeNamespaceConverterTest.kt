package com.programmerare.crsCodeGeneration.utils

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.*

internal class JavaPackageToCSharpeNamespaceConverterTest {
    @Test
    fun getAsNameOfCSharpeNameSpace() {
        val nameOfNamespace = JavaPackageToCSharpeNamespaceConverter.getAsNameOfCSharpeNameSpace("com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4")
        assertEquals("Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4", nameOfNamespace)
    }
}