package com.programmerare.crsTransformations.extensionfunctions

import com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL
import com.programmerare.crsTransformations.coordinate.createFromYNorthingLatitudeAndXEastingLongitude
import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsConstants.constantsByAreaNameNumber.v9_5_4.EpsgNumber
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.*

// TODO: move this code somewhere else since it is not really general string code
class ExtensionsForStringTest {

    @Test
    fun createCrsTransformationAdapterFromFullClassName() {
        val className = "com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL"
        assertEquals((CrsTransformationAdapterGooberCTL()).javaClass.name, className, "The class or package seem to have been renamed")

        val crsTransformationAdapter: CrsTransformationAdapter = className.createCrsTransformationAdapterFromFullClassName()
        assertNotNull(crsTransformationAdapter)
        assertEquals(className, crsTransformationAdapter.javaClass.name)

        // below trying to use the above created object to really make sure it works
        val coordinateWgs84 = createFromYNorthingLatitudeAndXEastingLongitude(59.330231, 18.059196, EpsgNumber.WORLD__WGS_84__4326)
        val resultSweref99 = crsTransformationAdapter.transform(coordinateWgs84, EpsgNumber.SWEDEN__SWEREF99_TM__3006)
        assertTrue(resultSweref99.isSuccess)
    }

    @Test
    fun createCrsTransformationAdapterFromUnvalidClassNameShouldFail() {
        val classNameNotExisting = "classNameNotExisting"
        val illegalStateException = assertThrows(
            ClassNotFoundException::class.java,
            { classNameNotExisting.createCrsTransformationAdapterFromFullClassName() },
            "Should not be able to create class from unvalid classname"
        )
    }
}