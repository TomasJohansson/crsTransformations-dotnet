package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CrsTransformationFacade

object CrsTransformationFacadeCompositeFactory {

    @JvmStatic
    fun createCrsTransformationMedian(list: List<CrsTransformationFacade>): CrsTransformationFacadeComposite {
        return CrsTransformationFacadeComposite(
            CompositeStrategyForMedianValue(
                list
            )
        )
    }

    @JvmStatic
    fun createCrsTransformationAverage(list: List<CrsTransformationFacade>): CrsTransformationFacadeComposite {
        return CrsTransformationFacadeComposite(
            CompositeStrategyForAverageValue(
                list
            )
    )
    }

    @JvmStatic
    fun createCrsTransformationWeightedAverage(weightedFacades: List<FacadeWeight>): CrsTransformationFacadeComposite {
        return CrsTransformationFacadeComposite(
            CompositeStrategyForWeightedAverageValue.createCompositeStrategyForWeightedAverageValue(weightedFacades)
        )
    }

    @JvmStatic
    fun createCrsTransformationChainOfResponsibility(list: List<CrsTransformationFacade>): CrsTransformationFacadeComposite {
        return CrsTransformationFacadeComposite(
            CompositeStrategyForChainOfResponsibility(
                list
            )
        )
    }

}