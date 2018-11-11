package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationAdapterLeafFactory
import java.lang.RuntimeException

/**
 * Factory methods creating 'Composite' implementations of the adapter interface.
 */
object CrsTransformationAdapterCompositeFactory {

    // ----------------------------------------------
    // Two Median factory methods:

    /**
     * Creates a 'composite' implementation by first trying to instantiate
     * all known 'leaf' implementations available at the class path,
     * and then pass those to the constructor of the 'composite' 
     * through the overloaded method with the same name but receiving a list parameter. 
     * @see createCrsTransformationMedian
     */
    @JvmStatic
    fun createCrsTransformationMedian(): CrsTransformationAdapterComposite {
        val list = CrsTransformationAdapterLeafFactory.getInstancesOfAllKnownAvailableImplementations()
        throwExceptionIfNoKnownInstancesAreAvailable(list)
        return createCrsTransformationMedian(
            list
        )
    }

    /**
     * Creates a 'composite' implementation by passing 
     * a list of 'leaf' implementations to the constructor of the 'composite'.
     * The created composite is used when the median value is desired as
     * the output coordinate. However, the average will also be available
     * almost as easily by getting access to a statistics object from the result object.
     * @param list a list of implementations which will very probably be 'leafs' although 
     *      this is not strictly enforced by the type system, so in theory
     *      the list could be composites i.e. you might try to nest 
     *      composites with composites (but it is not obvious why anyone would want to do that)
     * @see com.programmerare.crsTransformations.CrsTransformationResultStatistic
     * @see CrsTransformationAdapterComposite
     * @see CrsTransformationAdapter
     */    
    @JvmStatic
    fun createCrsTransformationMedian(list: List<CrsTransformationAdapter>): CrsTransformationAdapterComposite {
        return CrsTransformationAdapterComposite._createCrsTransformationAdapterComposite(
            CompositeStrategyForMedianValue._createCompositeStrategyForMedianValue(
                list
            )
        )
    }
    // ----------------------------------------------


    // ----------------------------------------------
    // Two Average factory methods:

    /**
     * Please see the documentation for the factory methods creating a median composite.
     * The only difference is that the average will be returned instead of the median.
     * @see createCrsTransformationMedian 
     */
    @JvmStatic
    fun createCrsTransformationAverage(): CrsTransformationAdapterComposite {
        val list = CrsTransformationAdapterLeafFactory.getInstancesOfAllKnownAvailableImplementations()
        throwExceptionIfNoKnownInstancesAreAvailable(list)
        return createCrsTransformationAverage(
            list
        )
    }

    /**
     * Please see the documentation for the factory methods creating a median composite.
     * The only difference is that the average will be returned instead of the median.
     * @see createCrsTransformationMedian
     */    
    @JvmStatic
    fun createCrsTransformationAverage(list: List<CrsTransformationAdapter>): CrsTransformationAdapterComposite {
        return CrsTransformationAdapterComposite._createCrsTransformationAdapterComposite(
            CompositeStrategyForAverageValue._createCompositeStrategyForAverageValue(
                list
            )
        )
    }
    // ----------------------------------------------

    // ----------------------------------------------
    // Two ChainOfResponsibility factory methods:

    /**
     * Please see documentation for the overloaded method.
     * @see createCrsTransformationChainOfResponsibility
     */    
    @JvmStatic
    fun createCrsTransformationChainOfResponsibility(): CrsTransformationAdapterComposite {
        val list = CrsTransformationAdapterLeafFactory.getInstancesOfAllKnownAvailableImplementations()
        throwExceptionIfNoKnownInstancesAreAvailable(list)
        return createCrsTransformationChainOfResponsibility(
            list
        )
    }

    /**
     * Please also see the documentation for the factory methods creating a median composite.
     * The difference is that the chain of responsiblity will not try
     * to use all leaf implementations but will only continue with the next
     * leaf implementation until a succesful result has been found.
     * @see createCrsTransformationMedian
     */    
    @JvmStatic
    fun createCrsTransformationChainOfResponsibility(list: List<CrsTransformationAdapter>): CrsTransformationAdapterComposite {
        return CrsTransformationAdapterComposite._createCrsTransformationAdapterComposite(
            CompositeStrategyForChainOfResponsibility._createCompositeStrategyForChainOfResponsibility(
                list
            )
        )
    }
    // ----------------------------------------------


    /**
     * Similar to the factory method creating an implementation to calculate the average.
     * The difference is that this method with weighted average can be used when you want to assign
     * certain adaptee implementations different weights, i.e. when you feel that you have a reason 
     * to believe that certain leaf implementations tend to provide better results than others.
     * @see createCrsTransformationAverage
     * @see CrsTransformationAdapterWeight
     */
    @JvmStatic
    fun createCrsTransformationWeightedAverage(
        weightedCrsTransformationAdapters: List<CrsTransformationAdapterWeight>
    ): CrsTransformationAdapterComposite {
        return CrsTransformationAdapterComposite._createCrsTransformationAdapterComposite(
            CompositeStrategyForWeightedAverageValue._createCompositeStrategyForWeightedAverageValue(weightedCrsTransformationAdapters)
        )
    }

    private fun throwExceptionIfNoKnownInstancesAreAvailable(list: List<CrsTransformationAdapter>) {
        if(list.size < 1) {
            throw RuntimeException("No known CRS transformation implementation was found")
        }
    }

}