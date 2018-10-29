package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CrsTransformationFacade

class FacadeWeight private constructor(
    val crsTransformationFacade: CrsTransformationFacade,
    val weight: Double
) {
    companion object {
        @JvmStatic
        fun createFromStringWithFullClassNameForImplementation(
            crsTransformationFacadeClassName: String,
            weight: Double
        ): FacadeWeight {
            val crsTransformationFacade = Class.forName(crsTransformationFacadeClassName).getDeclaredConstructor().newInstance() as CrsTransformationFacade
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
