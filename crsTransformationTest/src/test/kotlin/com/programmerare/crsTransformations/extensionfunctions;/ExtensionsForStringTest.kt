package com.programmerare.crsTransformations.extensionfunctions

import com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL
import com.programmerare.crsTransformations.Coordinate
import com.programmerare.crsTransformations.CrsTransformationFacade
import com.programmerare.crsTransformations.crsConstants.ConstantEpsgNumber
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.*

class ExtensionsForStringTest {

    @Test
    fun createCrsTransformationFacadeFromFullClassName() {
        val className = "com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL"
        assertEquals((CrsTransformationFacadeGooberCTL()).javaClass.name, className, "The class or package seem to have been renamed")

        val crsTransformationFacade: CrsTransformationFacade = className.createCrsTransformationFacadeFromFullClassName()

        // below trying to use the above created object to really make sure it works
        val coordinate = Coordinate.Companion.createFromYLatXLong(59.330231, 18.059196, ConstantEpsgNumber.WGS84)
        val result = crsTransformationFacade.transformToResultObject(coordinate, ConstantEpsgNumber.SWEREF99TM)
        assertTrue(result.isSuccess)
    }

    @Test
    fun createCrsTransformationFacadeFromUnvalidClassNameShouldFail() {
        val classNameNotExisting = "classNameNotExisting"
        val illegalStateException = assertThrows(
            ClassNotFoundException::class.java,
            { classNameNotExisting.createCrsTransformationFacadeFromFullClassName() },
            "should not be able to create class from unvalid classname"
        )
    }

}