package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.Coordinate
import com.programmerare.crsTransformations.CrsIdentifier
import com.programmerare.crsTransformations.CrsTransformationFacade
import com.programmerare.crsTransformations.CrsTransformationFacadeBase
import com.programmerare.crsTransformations.TransformResult
import java.lang.RuntimeException

final class CrsTransformationFacadeComposite internal constructor(protected val compositeStrategy: CompositeStrategy) : CrsTransformationFacadeBase(), CrsTransformationFacade {

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
        val allCrsTransformationFacades = compositeStrategy.getAllTransformationFacadesInTheOrderTheyShouldBeInvoked()
        val list = mutableListOf<TransformResult>()
        var lastResultOrNullIfNoPrevious: TransformResult? = null
        for (crsTransformationFacade: CrsTransformationFacade in allCrsTransformationFacades) {
            if(!compositeStrategy.shouldContinueIterationOfFacadesToInvoke(lastResultOrNullIfNoPrevious)) {
                break
            }
            val res = crsTransformationFacade.transform(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
            list.add(res)
            lastResultOrNullIfNoPrevious = res
        }
        return compositeStrategy.calculateAggregatedResult(list, inputCoordinate, crsIdentifierForOutputCoordinateSystem, this)
    }
}