package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationAdapterLeafFactory
import java.lang.RuntimeException

object CrsTransformationAdapterCompositeFactory {

    // ----------------------------------------------
    // Two Median factory methods:
    @JvmStatic
    fun createCrsTransformationMedian(): CrsTransformationAdapterComposite {
        val list = CrsTransformationAdapterLeafFactory.getInstancesOfAllKnownAvailableImplementations()
        throwExceptionIfNoKnownInstancesAreAvailable(list)
        return createCrsTransformationMedian(
            list
        )
    }

    @JvmStatic
    fun createCrsTransformationMedian(list: List<CrsTransformationAdapter>): CrsTransformationAdapterComposite {
        return CrsTransformationAdapterComposite(
            CompositeStrategyForMedianValue(
                list
            )
        )
    }
    // ----------------------------------------------


    // ----------------------------------------------
    // Two Average factory methods:
    @JvmStatic
    fun createCrsTransformationAverage(): CrsTransformationAdapterComposite {
        val list = CrsTransformationAdapterLeafFactory.getInstancesOfAllKnownAvailableImplementations()
        throwExceptionIfNoKnownInstancesAreAvailable(list)
        return createCrsTransformationAverage(
            list
        )
    }

    @JvmStatic
    fun createCrsTransformationAverage(list: List<CrsTransformationAdapter>): CrsTransformationAdapterComposite {
        return CrsTransformationAdapterComposite(
            CompositeStrategyForAverageValue(
                list
            )
        )
    }
    // ----------------------------------------------

    // ----------------------------------------------
    // Two ChainOfResponsibility factory methods:
    @JvmStatic
    fun createCrsTransformationChainOfResponsibility(): CrsTransformationAdapterComposite {
        val list = CrsTransformationAdapterLeafFactory.getInstancesOfAllKnownAvailableImplementations()
        throwExceptionIfNoKnownInstancesAreAvailable(list)
        return createCrsTransformationChainOfResponsibility(
                list
        )
    }

    @JvmStatic
    fun createCrsTransformationChainOfResponsibility(list: List<CrsTransformationAdapter>): CrsTransformationAdapterComposite {
        return CrsTransformationAdapterComposite(
            CompositeStrategyForChainOfResponsibility(
                list
            )
        )
    }
    // ----------------------------------------------


    @JvmStatic
    fun createCrsTransformationWeightedAverage(weightedCrsTransformationAdapters: List<CrsTransformationAdapterWeight>): CrsTransformationAdapterComposite {
        return CrsTransformationAdapterComposite(
            CompositeStrategyForWeightedAverageValue.createCompositeStrategyForWeightedAverageValue(weightedCrsTransformationAdapters)
        )
    }

    private fun throwExceptionIfNoKnownInstancesAreAvailable(list: List<CrsTransformationAdapter>) {
        if(list.size < 1) {
            throw RuntimeException("No known CRS transformation implementation was found")
        }
    }

}