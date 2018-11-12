package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import com.programmerare.crsTransformations.coordinate.CrsCoordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier

internal class CompositeStrategyForAverageValue private constructor(
    private val crsTransformationAdapters: List<CrsTransformationAdapter>
) : CompositeStrategyBase(crsTransformationAdapters), CompositeStrategy {

    override fun _shouldContinueIterationOfAdaptersToInvoke(lastResultOrNullIfNoPrevious: CrsTransformationResult?): Boolean {
        return true
    }

    override fun _calculateAggregatedResult(
            allResults: List<CrsTransformationResult>,
            inputCoordinate: CrsCoordinate,
            crsIdentifierForOutputCoordinateSystem: CrsIdentifier,
            crsTransformationAdapterThatCreatedTheResult: CrsTransformationAdapter
    ): CrsTransformationResult {
        val resultsStatistic = CrsTransformationResultStatistic._createCrsTransformationResultStatistic(allResults)
        return this.calculateAggregatedResultBase(
            allResults,
            inputCoordinate,
            crsTransformationAdapterThatCreatedTheResult,
            resultsStatistic,
            { r: CrsTransformationResultStatistic -> r.getCoordinateAverage() }
        )
   }

    override fun _getAdapteeType() : CrsTransformationAdapteeType {
        return CrsTransformationAdapteeType.COMPOSITE_AVERAGE
    }

    internal companion object {
        /**
         * This method is not intended for public use,
         * but instead the factory class should be used.
         * @see CrsTransformationAdapterCompositeFactory
         */
        @JvmStatic
        internal fun _createCompositeStrategyForAverageValue (
            list: List<CrsTransformationAdapter>
        ): CompositeStrategyForAverageValue {
            return CompositeStrategyForAverageValue(list)
        }
    }

}