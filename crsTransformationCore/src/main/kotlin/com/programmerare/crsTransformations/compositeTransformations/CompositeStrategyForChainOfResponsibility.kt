package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*

class CompositeStrategyForChainOfResponsibility(private val crsTransformationFacades: List<CrsTransformationFacade>) : CompositeStrategy {

    override fun getAllTransformationFacadesInTheOrderTheyShouldBeInvoked(): List<CrsTransformationFacade> {
        return crsTransformationFacades
    }

    override fun shouldInvokeNextFacade(allResultsSoFar: List<TransformResult>, lastResultOrNullIfNoPrevious: TransformResult?, nextFacadeToInvoke: CrsTransformationFacade): Boolean {
        return shouldContinueIterationOfFacadesToInvoke(allResultsSoFar, lastResultOrNullIfNoPrevious)
    }

    override fun shouldContinueIterationOfFacadesToInvoke(allResultsSoFar: List<TransformResult>, lastResultOrNullIfNoPrevious: TransformResult?): Boolean {
        return lastResultOrNullIfNoPrevious == null || !lastResultOrNullIfNoPrevious.isSuccess
    }

    override fun calculateAggregatedResult(
        allResults: List<TransformResult>,
        inputCoordinate: Coordinate,
        crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): TransformResult {
        if(allResults.size == 1 && allResults.get(0).isSuccess) {
            return TransformResultImplementation(inputCoordinate, outputCoordinate = allResults.get(0).outputCoordinate, exception = null, isSuccess = true)
        }
        else {
            return TransformResultImplementation(inputCoordinate, outputCoordinate = null, exception = null, isSuccess = false)
        }
    }
}