package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CrsTransformationAdapteeType
import com.programmerare.crsTransformations.coordinate.CrsCoordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier
import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationAdapterBase
import com.programmerare.crsTransformations.CrsTransformationResult
import java.lang.RuntimeException

final class CrsTransformationAdapterComposite internal constructor(protected val compositeStrategy: CompositeStrategy) : CrsTransformationAdapterBase(), CrsTransformationAdapter {

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
        val allCrsTransformationAdapters = compositeStrategy.getAllTransformationAdaptersInTheOrderTheyShouldBeInvoked()
        val list = mutableListOf<CrsTransformationResult>()
        var lastResultOrNullIfNoPrevious: CrsTransformationResult? = null
        for (crsTransformationAdapter: CrsTransformationAdapter in allCrsTransformationAdapters) {
            if(!compositeStrategy.shouldContinueIterationOfAdaptersToInvoke(lastResultOrNullIfNoPrevious)) {
                break
            }
            val res = crsTransformationAdapter.transform(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
            list.add(res)
            lastResultOrNullIfNoPrevious = res
        }
        return compositeStrategy.calculateAggregatedResult(list, inputCoordinate, crsIdentifierForOutputCoordinateSystem, this)
    }

    override final fun getTransformationAdapterChildren(): List<CrsTransformationAdapter> {
        return compositeStrategy.getAllTransformationAdaptersInTheOrderTheyShouldBeInvoked()
    }

    override final fun isComposite(): Boolean {
        return true
    }

    override fun getAdapteeType() : CrsTransformationAdapteeType {
        return compositeStrategy.getAdapteeType()
    }
}