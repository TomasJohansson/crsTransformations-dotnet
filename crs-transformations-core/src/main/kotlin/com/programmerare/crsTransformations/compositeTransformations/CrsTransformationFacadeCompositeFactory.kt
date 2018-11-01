package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CrsTransformationFacade
import com.programmerare.crsTransformations.CrsTransformationFacadeLeafFactory
import java.lang.RuntimeException

object CrsTransformationFacadeCompositeFactory {

    // ----------------------------------------------
    // Two Median factory methods:
    @JvmStatic
    fun createCrsTransformationMedian(): CrsTransformationFacadeComposite {
        val list = CrsTransformationFacadeLeafFactory.getInstancesOfAllKnownAvailableImplementations()
        throwExceptionIfNoKnownInstancesAreAvailable(list)
        return createCrsTransformationMedian(
            list
        )
    }

    @JvmStatic
    fun createCrsTransformationMedian(list: List<CrsTransformationFacade>): CrsTransformationFacadeComposite {
        return CrsTransformationFacadeComposite(
            CompositeStrategyForMedianValue(
                list
            )
        )
    }
    // ----------------------------------------------


    // ----------------------------------------------
    // Two Average factory methods:
    @JvmStatic
    fun createCrsTransformationAverage(): CrsTransformationFacadeComposite {
        val list = CrsTransformationFacadeLeafFactory.getInstancesOfAllKnownAvailableImplementations()
        throwExceptionIfNoKnownInstancesAreAvailable(list)
        return createCrsTransformationAverage(
            list
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
    // ----------------------------------------------

    // ----------------------------------------------
    // Two ChainOfResponsibility factory methods:
    @JvmStatic
    fun createCrsTransformationChainOfResponsibility(): CrsTransformationFacadeComposite {
        val list = CrsTransformationFacadeLeafFactory.getInstancesOfAllKnownAvailableImplementations()
        throwExceptionIfNoKnownInstancesAreAvailable(list)
        return createCrsTransformationChainOfResponsibility(
                list
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
    // ----------------------------------------------


    @JvmStatic
    fun createCrsTransformationWeightedAverage(weightedFacades: List<FacadeWeight>): CrsTransformationFacadeComposite {
        return CrsTransformationFacadeComposite(
            CompositeStrategyForWeightedAverageValue.createCompositeStrategyForWeightedAverageValue(weightedFacades)
        )
    }

    private fun throwExceptionIfNoKnownInstancesAreAvailable(list: List<CrsTransformationFacade>) {
        if(list.size < 1) {
            throw RuntimeException("No known CRS transformation implementation was found")
        }
    }

}