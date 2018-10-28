package com.programmerare.crsCodeGeneration.constantsGenerator

import org.junit.jupiter.api.Test

import org.junit.jupiter.api.Assertions.*

internal class ConstantClassGeneratorTest {

    @Test
    fun getEmptyStringIfNoError() {
        val args: Array<String> = arrayOf("v9_5_3", "dbName", "dbUser", "dbPassword", "java")
        val result = ConstantClassGenerator.getValidationErrorMessageOrEmptyStringIfNoError(args)
        assertEquals("", result)
    }

    @Test
    fun getValidationErrorMessageWhenTheFirstVersionParameterIsNotCorrect() {
        val args: Array<String> = arrayOf("x9_5_3", "dbName", "dbUser", "dbPassword")
        // the first letter should be "v" and not "x" as above
        val result = ConstantClassGenerator.getValidationErrorMessageOrEmptyStringIfNoError(args)
        assertNotEquals("", result)
    }

    @Test
    fun getValidationErrorMessageWhenTooFewParameters() {
        val args: Array<String> = arrayOf("x9_5_3", "dbName", "dbUser")
        // there should be a fourth parameter with a password
        val result = ConstantClassGenerator.getValidationErrorMessageOrEmptyStringIfNoError(args)
        assertNotEquals("", result)
    }


    @Test
    fun isValidAsVersionPrefix() {
        assertTrue(ConstantClassGenerator.isValidAsVersionPrefix("v9"))
        assertTrue(ConstantClassGenerator.isValidAsVersionPrefix("v9_5"))
        assertTrue(ConstantClassGenerator.isValidAsVersionPrefix("v9_5_3"))

        assertFalse(ConstantClassGenerator.isValidAsVersionPrefix("v9_"))
        assertFalse(ConstantClassGenerator.isValidAsVersionPrefix("v9_5_3_"))
        assertFalse(ConstantClassGenerator.isValidAsVersionPrefix("x9_5_3"))
        assertFalse(ConstantClassGenerator.isValidAsVersionPrefix("v9x_5_3"))
        assertFalse(ConstantClassGenerator.isValidAsVersionPrefix("v9.5.3"))
        assertFalse(ConstantClassGenerator.isValidAsVersionPrefix("v9_5_x"))

    }
}