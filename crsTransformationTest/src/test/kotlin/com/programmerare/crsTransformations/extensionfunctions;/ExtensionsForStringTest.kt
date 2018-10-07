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
        assertNotNull(crsTransformationFacade)
        assertEquals(className, crsTransformationFacade.javaClass.name)

        // below trying to use the above created object to really make sure it works
        val coordinateWgs84 = Coordinate.Companion.createFromYLatXLong(59.330231, 18.059196, EpsgNumber._4326__WGS_84__WORLD)
        val resultSweref99 = crsTransformationFacade.transformToResultObject(coordinateWgs84, EpsgNumber._3006__SWEREF99_TM__SWEDEN)
        assertTrue(resultSweref99.isSuccess)
    }

    @Test
    fun createCrsTransformationFacadeFromUnvalidClassNameShouldFail() {
        val classNameNotExisting = "classNameNotExisting"
        val illegalStateException = assertThrows(
            ClassNotFoundException::class.java,
            { classNameNotExisting.createCrsTransformationFacadeFromFullClassName() },
            "Should not be able to create class from unvalid classname"
        )
    }
}