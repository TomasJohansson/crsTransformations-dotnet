package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import java.lang.RuntimeException

// TODO make this class concrete and final and then also rename it (regarding "Base")
abstract class CrsTransformationFacadeBaseComposite(protected val compositeStrategy: CompositeStrategy) : CrsTransformationFacadeBase(), CrsTransformationFacade {

    override final fun transform(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): Coordinate {
        val transformResult = transformToResultObject(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
        if(transformResult.isSuccess) {
            return transformResult.outputCoordinate
        }
        else {
            throw RuntimeException("Transformation failed")
        }
    }

    override final fun transformToResultObject(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): TransformResult {
        val crsTransformationFacades = compositeStrategy.getAllTransformationFacadesInTheOrderTheyShouldBeInvoked()
        val list = mutableListOf<TransformResult>()
        var lastResultOrNullIfNoPrevious: TransformResult? = null
        for (facade: CrsTransformationFacade in crsTransformationFacades) {
            if(compositeStrategy.shouldInvokeNextFacade(list, lastResultOrNullIfNoPrevious, facade)) {
                val res = facade.transformToResultObject(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
                list.add(res)
                lastResultOrNullIfNoPrevious = res
            }
            if(!compositeStrategy.shouldContinueIterationOfFacadesToInvoke(list, lastResultOrNullIfNoPrevious)) {
                break
            }
        }
        return compositeStrategy.calculateAggregatedResult(list, inputCoordinate, crsIdentifierForOutputCoordinateSystem)
    }
}
