package com.programmerare.crsTransformations

import com.programmerare.crsTransformations.extensionfunctions.createCrsTransformationAdapterFromFullClassName

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
        val crsTransformationAdapter: CrsTransformationAdapter = crsTransformationAdapterClassName.createCrsTransformationAdapterFromFullClassName()
        return crsTransformationAdapter
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

//    @file:JvmName("StringUtils")
//    package com.programmerare.crsTransformations.extensionfunctions
//    import com.programmerare.crsTransformations.CrsTransformationAdapter
    /**
     * Utility method for creating an adapter instance by using reflection
     * for a string that should be the full class name for an implementation.
     * When using Kotlin the method is available as an extension method for strings,
     * but when using Java the method is available for a class named 'StringUtils'.
     */
    @JvmStatic
    //fun String.createCrsTransformationAdapterFromFullClassName2(): CrsTransformationAdapter {
    fun createCrsTransformationAdapterFromFullClassName2(className: String): CrsTransformationAdapter {
        val crsTransformationAdapter = Class.forName(className).getDeclaredConstructor().newInstance() as CrsTransformationAdapter
        return crsTransformationAdapter
    }    
}