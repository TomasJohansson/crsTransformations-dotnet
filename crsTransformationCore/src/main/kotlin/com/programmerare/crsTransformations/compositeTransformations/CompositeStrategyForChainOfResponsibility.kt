package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*

class CompositeStrategyForChainOfResponsibility(private val crsTransformationFacades: List<CrsTransformationFacade>) : CompositeStrategy {

    override fun getAllTransformationFacadesInTheOrderTheyShouldBeInvoked(): List<CrsTransformationFacade> {
        return crsTransformationFacades
    }

    override fun shouldContinueIterationOfFacadesToInvoke(lastResultOrNullIfNoPrevious: TransformResult?): Boolean {
        return lastResultOrNullIfNoPrevious == null || !lastResultOrNullIfNoPrevious.isSuccess
    }

    override fun calculateAggregatedResult(
            allResults: List<TransformResult>,
            inputCoordinate: Coordinate,
            crsIdentifierForOutputCoordinateSystem: CrsIdentifier,
            crsTransformationFacadeThatCreatedTheResult: CrsTransformationFacade
    ): TransformResult {
        if(allResults.size == 1 && allResults.get(0).isSuccess) {
            return TransformResultImplementation(inputCoordinate, outputCoordinate = allResults.get(0).outputCoordinate, exception = null, isSuccess = true, crsTransformationFacadeThatCreatedTheResult = crsTransformationFacadeThatCreatedTheResult)
        }
        else {
            return TransformResultImplementation(inputCoordinate, outputCoordinate = null, exception = null, isSuccess = false, crsTransformationFacadeThatCreatedTheResult = crsTransformationFacadeThatCreatedTheResult)
        }
    }
}