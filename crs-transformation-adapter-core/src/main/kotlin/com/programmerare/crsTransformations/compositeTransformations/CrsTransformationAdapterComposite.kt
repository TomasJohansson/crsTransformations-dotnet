package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.Coordinate
import com.programmerare.crsTransformations.CrsIdentifier
import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationAdapterBase
import com.programmerare.crsTransformations.TransformResult
import java.lang.RuntimeException

final class CrsTransformationAdapterComposite internal constructor(protected val compositeStrategy: CompositeStrategy) : CrsTransformationAdapterBase(), CrsTransformationAdapter {

    override final protected fun transformHook(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): Coordinate {
        val transformResult = transform(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
        if(transformResult.isSuccess) {
            return transformResult.outputCoordinate
        }
        else {
            throw RuntimeException("Transformation failed")
        }
    }

    override final fun transform(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): TransformResult {
        val allCrsTransformationAdapters = compositeStrategy.getAllTransformationAdaptersInTheOrderTheyShouldBeInvoked()
        val list = mutableListOf<TransformResult>()
        var lastResultOrNullIfNoPrevious: TransformResult? = null
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
}