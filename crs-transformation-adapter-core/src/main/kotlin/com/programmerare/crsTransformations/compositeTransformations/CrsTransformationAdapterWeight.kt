package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationAdapterLeafFactory

/**
 * An instance of this class defines how much relative weight
 * a certain adapter implementation should have.
 * 
 * A list of these instances should be passed to the factory 
 * method for a composite object used for returning a weighted average 
 * of the longitude and latitude values originating from different leaf 
 * adapter implementations doing coordinate transformations.
 * @see CrsTransformationAdapterCompositeFactory.createCrsTransformationWeightedAverage
 * @param crsTransformationAdapter an object implementing the interface CrsTransformationAdapter
 * @param weight the relative weight value to assign for the adapter specified by the adapter parameter
 */
class CrsTransformationAdapterWeight private constructor(
    val crsTransformationAdapter: CrsTransformationAdapter,
    val weight: Double
) {
    init {
        if(weight <=0) {
            throw IllegalArgumentException("The weight value must be positive value. It does not make sense to try using negative values, and there would be useless with a zero value. The weight value was: " + weight  + " for the adapter " + crsTransformationAdapter.getShortNameOfImplementation())
        }
    }

    /**
     * Not intended to be used with ".Companion" from client code.
     * The reason for its existence has to do with the fact that the
     * JVM class has been created with the programming language Kotlin.
     */
    companion object {

        /**
         * @param crsTransformationAdapterClassName the full class name (i.e. including the package name)
         *  of a class which must implement the interface CrsTransformationAdapter
         * @param weight the relative weight value to assign for the adapter specified by the string parameter 
         */
        @JvmStatic
        fun createFromStringWithFullClassNameForImplementation(
            crsTransformationAdapterClassName: String,
            weight: Double
        ): CrsTransformationAdapterWeight {
            val crsTransformationAdapter = CrsTransformationAdapterLeafFactory.createCrsTransformationAdapter(crsTransformationAdapterClassName)
            return CrsTransformationAdapterWeight(crsTransformationAdapter, weight)
        }

        /**
         * @param crsTransformationAdapter an object implementing the interface CrsTransformationAdapter
         * @param weight the relative weight value to assign for the adapter specified by the adapter parameter 
         */        
        @JvmStatic
        fun createFromInstance(
            crsTransformationAdapter: CrsTransformationAdapter,
            weight: Double
        ): CrsTransformationAdapterWeight{
            return CrsTransformationAdapterWeight(crsTransformationAdapter, weight)
        }
    }
}