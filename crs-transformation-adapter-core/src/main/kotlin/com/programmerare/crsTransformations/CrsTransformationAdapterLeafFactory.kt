package com.programmerare.crsTransformations

import java.lang.RuntimeException

/**
 * Factory used by 'composites' for creating 'leaf' implementations available at the classpath.  
 * 
 * The factory uses reflection code to instantiate the implementations from strings with full class names.  
 * 
 * The reason for these string based instantiations is that the core library avoids 
 * statically predefined enforced dependencies to all leaf adapter implementations.  
 * 
 * Instead the users can choose which implementations to use e.g. through Maven or Gradle dependencies.
 * @see com.programmerare.crsTransformations.compositeTransformations.CrsTransformationAdapterCompositeFactory
 */
object CrsTransformationAdapterLeafFactory {

    /**
     * @param crsTransformationAdapterClassName the full class name (i.e. including the package name)
     *      of a class which must implement the interface CrsTransformationAdapter
     * @return an instance if it could be created but otherwise an exception      
     */
    @JvmStatic
    fun createCrsTransformationAdapter(crsTransformationAdapterClassName: String): CrsTransformationAdapter {
        try {
            val crsTransformationAdapter = Class.forName(crsTransformationAdapterClassName).getDeclaredConstructor().newInstance() as CrsTransformationAdapter
            return crsTransformationAdapter
        }
        catch (e: Throwable) {
            val nameOfInterfaceThatShouldBeImplemented = CrsTransformationAdapter::class.java.name
            val message = "Failed to instantiate a class with the name '" + crsTransformationAdapterClassName + "' . The parameter must be the name of a class which implements the interface '" + nameOfInterfaceThatShouldBeImplemented + "'"
            throw RuntimeException(message, e)
                        
        }        
    }

    /**
     * @param the full class name (i.e. including the package name)
     *      of a class which must implement the interface CrsTransformationAdapter
     * @return true if it possible to create an instance from the input string 
     */
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

    /**
     * @return a list of strings with full class names for known 
     *  leaf implementations of the adapter interface
     */
    @JvmStatic
    fun getClassNamesForAllKnownImplementations(): List<String> {
        return classNamesForAllKnownImplementations
    }

    /**
     * @return a list of instances for all known leaf implementations 
     *      of the adapter interface, which are available at the class path.
     */    
    @JvmStatic
    fun getInstancesOfAllKnownAvailableImplementations(): List<CrsTransformationAdapter> {
        return crsTransformationAdapters
    }

    private val crsTransformationAdapters: List<CrsTransformationAdapter> by lazy {
        val classNames = getClassNamesForAllKnownImplementations()
        classNames.filter { isCrsTransformationAdapter(it) }.map { createCrsTransformationAdapter(it) }
    }

}