package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CrsTransformationFacade
import com.programmerare.crsTransformations.CrsTransformationFacadeFactory

class FacadeWeight private constructor(
    val crsTransformationFacade: CrsTransformationFacade,
    val weight: Double
) {
    companion object {

        /**
         * @param crsTransformationFacadeClassName the full class name (i.e. including the package name)
         *  of a class which must implement the interface CrsTransformationFacade
         */
        @JvmStatic
        fun createFromStringWithFullClassNameForImplementation(
            crsTransformationFacadeClassName: String,
            weight: Double
        ): FacadeWeight {
            val crsTransformationFacade = CrsTransformationFacadeFactory.createCrsTransformationFacade(crsTransformationFacadeClassName)
            return FacadeWeight(crsTransformationFacade, weight)
        }

        @JvmStatic
        fun createFromInstance(
            crsTransformationFacade: CrsTransformationFacade,
            weight: Double
        ): FacadeWeight{
            return FacadeWeight(crsTransformationFacade, weight)
        }
    }
}