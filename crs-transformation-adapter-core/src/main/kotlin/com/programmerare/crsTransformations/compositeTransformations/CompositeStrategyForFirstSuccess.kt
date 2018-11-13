package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import com.programmerare.crsTransformations.coordinate.CrsCoordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier

internal class CompositeStrategyForFirstSuccess private constructor(
    private val crsTransformationAdapters: List<CrsTransformationAdapter>
) : CompositeStrategyBase(crsTransformationAdapters), CompositeStrategy {

    override fun _shouldContinueIterationOfAdaptersToInvoke(lastResultOrNullIfNoPrevious: CrsTransformationResult?): Boolean {
        return lastResultOrNullIfNoPrevious == null || !lastResultOrNullIfNoPrevious.isSuccess
    }

    override fun _calculateAggregatedResult(
            allResults: List<CrsTransformationResult>,
            inputCoordinate: CrsCoordinate,
            crsIdentifierForOutputCoordinateSystem: CrsIdentifier,
            crsTransformationAdapterThatCreatedTheResult: CrsTransformationAdapter
    ): CrsTransformationResult {
        if(allResults.size == 1 && allResults.get(0).isSuccess) {
            // there should never be more than one result with the FirstSuccess implementation
            // since the calculation is interrupted at the first succeful result
            return CrsTransformationResult._createCrsTransformationResult(
                inputCoordinate,
                outputCoordinate = allResults.get(0).outputCoordinate,
                exception = null,
                isSuccess = true,
                crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                transformationResultChildren = allResults
            )
        }
        else {
            return CrsTransformationResult._createCrsTransformationResult(
                inputCoordinate,
                outputCoordinate = null,
                exception = null,
                isSuccess = false,
                crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                transformationResultChildren = allResults
            )
        }
    }

    override fun _getAdapteeType() : CrsTransformationAdapteeType {
        return CrsTransformationAdapteeType.COMPOSITE_FIRST_SUCCESS
    }

    internal companion object {

        /**
         * This method is not intended for public use, 
         * but instead the factory class should be used.
         * @see CrsTransformationAdapterCompositeFactory
         */
        @JvmStatic
        internal fun _createCompositeStrategyForFirstSuccess (
            list: List<CrsTransformationAdapter>
        ): CompositeStrategyForFirstSuccess {
            return CompositeStrategyForFirstSuccess(list) 
        }
    }
}