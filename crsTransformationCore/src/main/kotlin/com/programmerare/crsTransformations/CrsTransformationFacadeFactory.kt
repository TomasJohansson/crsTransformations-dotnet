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
}