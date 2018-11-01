package com.programmerare.crsTransformations

import com.programmerare.crsTransformations.extensionfunctions.createCrsTransformationAdapterFromFullClassName

object CrsTransformationAdapterLeafFactory {

    /**
     * @param crsTransformationAdapterClassName the full class name (i.e. including the package name)
     *  of a class which must implement the interface CrsTransformationAdapter
     */
    @JvmStatic
    fun createCrsTransformationAdapter(crsTransformationAdapterClassName: String): CrsTransformationAdapter {
        val crsTransformationAdapter: CrsTransformationAdapter = crsTransformationAdapterClassName.createCrsTransformationAdapterFromFullClassName()
        return crsTransformationAdapter
    }

    @JvmStatic
    fun isCrsTransformationAdapter(crsTransformationAdapterClassName: String?): Boolean {
        try {
            if(crsTransformationAdapterClassName == null) return false;
            return createCrsTransformationAdapter(crsTransformationAdapterClassName) != null
        }
        catch (e: Throwable) {
            return false
        }
    }


    // Note that the test code verifies that all these hadrcoded class names exist,
    // i.e. if they are renamed, it will detected by the test code
    private val classNamesForAllKnownImplementations = listOf(
        "com.programmerare.crsTransformationAdapterGooberCTL.CrsTransformationAdapterGooberCTL",
        "com.programmerare.crsTransformationAdapterGeoPackageNGA.CrsTransformationAdapterGeoPackageNGA",
        "com.programmerare.crsTransformationAdapterGeoTools.CrsTransformationAdapterGeoTools",
        "com.programmerare.crsTransformationAdapterOrbisgisCTS.CrsTransformationAdapterOrbisgisCTS",
        "com.programmerare.crsTransformationAdapterProj4J.CrsTransformationAdapterProj4J"
    )

    @JvmStatic
    fun getClassNamesForAllKnownImplementations(): List<String> {
        return classNamesForAllKnownImplementations
    }

    @JvmStatic
    fun getInstancesOfAllKnownAvailableImplementations(): List<CrsTransformationAdapter> {
        return crsTransformationAdapters
    }

    private val crsTransformationAdapters: List<CrsTransformationAdapter> by lazy {
        val classNames = getClassNamesForAllKnownImplementations()
        classNames.filter { isCrsTransformationAdapter(it) }.map { createCrsTransformationAdapter(it) }
    }
}