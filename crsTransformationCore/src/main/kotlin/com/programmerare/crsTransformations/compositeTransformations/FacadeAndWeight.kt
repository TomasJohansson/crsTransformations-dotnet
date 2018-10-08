package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CrsTransformationFacade

class FacadeAndWeight private constructor(
    val crsTransformationFacade: CrsTransformationFacade,
    val weight: Double
) {
    companion object {
        @JvmStatic
        fun createFromStringWithFullClassNameForImplementation(
            crsTransformationFacadeClassName: String,
            weight: Double
        ): FacadeAndWeight {
            val crsTransformationFacade = Class.forName(crsTransformationFacadeClassName).getDeclaredConstructor().newInstance() as CrsTransformationFacade
            return FacadeAndWeight(crsTransformationFacade, weight)
        }
        @JvmStatic
        fun createFromInstance(
            crsTransformationFacade: CrsTransformationFacade,
            weight: Double
        ): FacadeAndWeight{

            return FacadeAndWeight(crsTransformationFacade, weight)
        }
    }
}
