package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationAdapterLeafFactory

class AdapterWeight private constructor(
        val crsTransformationAdapter: CrsTransformationAdapter,
        val weight: Double
) {
    companion object {

        /**
         * @param crsTransformationAdapterClassName the full class name (i.e. including the package name)
         *  of a class which must implement the interface CrsTransformationAdapter
         */
        @JvmStatic
        fun createFromStringWithFullClassNameForImplementation(
            crsTransformationAdapterClassName: String,
            weight: Double
        ): AdapterWeight {
            val crsTransformationAdapter = CrsTransformationAdapterLeafFactory.createCrsTransformationAdapter(crsTransformationAdapterClassName)
            return AdapterWeight(crsTransformationAdapter, weight)
        }

        @JvmStatic
        fun createFromInstance(
                crsTransformationAdapter: CrsTransformationAdapter,
                weight: Double
        ): AdapterWeight{
            return AdapterWeight(crsTransformationAdapter, weight)
        }
    }
}