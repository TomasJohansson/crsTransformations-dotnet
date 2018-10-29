package com.programmerare.crsTransformations

object CrsTransformationFacadeFactory {

    /**
     * @param crsTransformationFacadeClassName the full class name (i.e. including the package name)
     *  of a class which must implement the interface CrsTransformationFacade
     */
    @JvmStatic
    fun createCrsTransformationFacade(crsTransformationFacadeClassName: String): CrsTransformationFacade {
        val crsTransformationFacade = Class.forName(crsTransformationFacadeClassName).getDeclaredConstructor().newInstance() as CrsTransformationFacade
        return crsTransformationFacade
    }

    @JvmStatic
    fun isCrsTransformationFacade(crsTransformationFacadeClassName: String?): Boolean {
        try {
            if(crsTransformationFacadeClassName == null) return false;
            return createCrsTransformationFacade(crsTransformationFacadeClassName) != null
        }
        catch (e: Throwable) {
            return false
        }
    }


    // Note that the test code verifies that all these hadrcoded class names exist,
    // i.e. if they are renamed, it will detected by the test code
    private val classNamesForAllKnownImplementations = listOf(
        "com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL",
        "com.programmerare.crsTransformationFacadeGeoPackageNGA.CrsTransformationFacadeGeoPackageNGA",
        "com.programmerare.crsTransformationFacadeGeoTools.CrsTransformationFacadeGeoTools",
        "com.programmerare.crsTransformationFacadeOrbisgisCTS.CrsTransformationFacadeOrbisgisCTS",
        "com.programmerare.crsTransformationFacadeProj4J.CrsTransformationFacadeProj4J"
    )

    @JvmStatic
    fun getClassNamesForAllKnownImplementations(): List<String> {
        return classNamesForAllKnownImplementations
    }

    @JvmStatic
    fun getInstancesOfAllKnownAvailableImplementations(): List<CrsTransformationFacade> {
        return crsTransformationFacades
    }

    private val crsTransformationFacades: List<CrsTransformationFacade> by lazy {
        val classNames = getClassNamesForAllKnownImplementations()
        classNames.filter { isCrsTransformationFacade(it) }.map { createCrsTransformationFacade(it) }
    }
}