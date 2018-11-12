package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CrsTransformationAdapteeType
import com.programmerare.crsTransformations.coordinate.CrsCoordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier
import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationAdapterBase
import com.programmerare.crsTransformations.CrsTransformationResult
import java.lang.RuntimeException

/**
 * Base class for the 'composite' adapters.
 * @see CrsTransformationAdapterBase
 * @see CompositeStrategy
 */
final class CrsTransformationAdapterComposite private constructor(

    /**
     * Interface for calculating the resulting coordinate in different ways, 
     * e.g. one stratefy implementation calculates the median and another the average.
     */        
    protected val compositeStrategy: CompositeStrategy

) : CrsTransformationAdapterBase(), CrsTransformationAdapter {

    override final protected fun transformHook(inputCoordinate: CrsCoordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): CrsCoordinate {
        val transformResult = transform(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
        if(transformResult.isSuccess) {
            return transformResult.outputCoordinate
        }
        else {
            throw RuntimeException("Transformation failed")
        }
    }

    override final fun transform(inputCoordinate: CrsCoordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): CrsTransformationResult {
        val allCrsTransformationAdapters = compositeStrategy._getAllTransformationAdaptersInTheOrderTheyShouldBeInvoked()
        val list = mutableListOf<CrsTransformationResult>()
        var lastResultOrNullIfNoPrevious: CrsTransformationResult? = null
        for (crsTransformationAdapter: CrsTransformationAdapter in allCrsTransformationAdapters) {
            if(!compositeStrategy._shouldContinueIterationOfAdaptersToInvoke(lastResultOrNullIfNoPrevious)) {
                break
            }
            val res = crsTransformationAdapter.transform(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
            list.add(res)
            lastResultOrNullIfNoPrevious = res
        }
        return compositeStrategy._calculateAggregatedResult(list, inputCoordinate, crsIdentifierForOutputCoordinateSystem, this)
    }

    override final fun getTransformationAdapterChildren(): List<CrsTransformationAdapter> {
        return compositeStrategy._getAllTransformationAdaptersInTheOrderTheyShouldBeInvoked()
    }

    override final fun isComposite(): Boolean {
        return true
    }

    override fun getAdapteeType() : CrsTransformationAdapteeType {
        return compositeStrategy._getAdapteeType()
    }

    internal companion object {
        /**
         * This method is not intended for public use,
         * but instead the factory class should be used.
         * @see CrsTransformationAdapterCompositeFactory
         */
        @JvmStatic
        internal fun _createCrsTransformationAdapterComposite(
            compositeStrategy: CompositeStrategy
        ): CrsTransformationAdapterComposite {
            return CrsTransformationAdapterComposite(compositeStrategy)
        }
    }
    
}