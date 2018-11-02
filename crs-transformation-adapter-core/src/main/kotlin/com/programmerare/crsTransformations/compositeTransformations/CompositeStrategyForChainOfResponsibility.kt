package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import com.programmerare.crsTransformations.coordinate.Coordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier

internal class CompositeStrategyForChainOfResponsibility(
    private val crsTransformationAdapters: List<CrsTransformationAdapter>
) : CompositeStrategyBase(crsTransformationAdapters), CompositeStrategy {

    override fun shouldContinueIterationOfAdaptersToInvoke(lastResultOrNullIfNoPrevious: TransformResult?): Boolean {
        return lastResultOrNullIfNoPrevious == null || !lastResultOrNullIfNoPrevious.isSuccess
    }

    override fun calculateAggregatedResult(
            allResults: List<TransformResult>,
            inputCoordinate: Coordinate,
            crsIdentifierForOutputCoordinateSystem: CrsIdentifier,
            crsTransformationAdapterThatCreatedTheResult: CrsTransformationAdapter
    ): TransformResult {
        if(allResults.size == 1 && allResults.get(0).isSuccess) {
            // there should never be more than one result with the ChainOfResponsibility implementation
            // since the calculation is interrupted at the first succeful result
            return TransformResultImplementation(
                inputCoordinate,
                outputCoordinate = allResults.get(0).outputCoordinate,
                exception = null,
                isSuccess = true,
                crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                subResults = allResults
            )
        }
        else {
            return TransformResultImplementation(
                inputCoordinate,
                outputCoordinate = null,
                exception = null,
                isSuccess = false,
                crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                subResults = allResults
            )
        }
    }
}