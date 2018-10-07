package com.programmerare.crsTransformations.extensionfunctions

import com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL
import com.programmerare.crsTransformations.Coordinate
import com.programmerare.crsTransformations.CrsTransformationFacade
import com.programmerare.crsConstants.constantsByNumberNameArea.v9_5_4.EpsgNumber
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.*

class ExtensionsForStringTest {

    @Test
    fun createCrsTransformationFacadeFromFullClassName() {
        val className = "com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL"
        assertEquals((CrsTransformationFacadeGooberCTL()).javaClass.name, className, "The class or package seem to have been renamed")

        val crsTransformationFacade: CrsTransformationFacade = className.createCrsTransformationFacadeFromFullClassName()

        // below trying to use the above created object to really make sure it works
        val coordinate = Coordinate.Companion.createFromYLatXLong(59.330231, 18.059196, EpsgNumber._4326__WGS_84__WORLD)
        val result = crsTransformationFacade.transformToResultObject(coordinate, EpsgNumber._3006__SWEREF99_TM__SWEDEN)
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