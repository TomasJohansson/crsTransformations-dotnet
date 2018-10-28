package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*

internal class CompositeStrategyForChainOfResponsibility(
    private val crsTransformationFacades: List<CrsTransformationFacade>
) : CompositeStrategyBase(crsTransformationFacades), CompositeStrategy {

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
            // there should never be more than one result with the ChainOfResponsibility implementation
            // since the calculation is interrupted at the first succeful result
            return TransformResultImplementation(inputCoordinate, outputCoordinate = allResults.get(0).outputCoordinate, exception = null, isSuccess = true, crsTransformationFacadeThatCreatedTheResult = crsTransformationFacadeThatCreatedTheResult)
        }
        else {
            return TransformResultImplementation(inputCoordinate, outputCoordinate = null, exception = null, isSuccess = false, crsTransformationFacadeThatCreatedTheResult = crsTransformationFacadeThatCreatedTheResult)
        }
    }
}