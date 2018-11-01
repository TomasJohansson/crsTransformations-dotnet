package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import com.programmerare.crsTransformations.coordinate.Coordinate

internal class CompositeStrategyForMedianValue(
    private val crsTransformationAdapters: List<CrsTransformationAdapter>
) : CompositeStrategyBase(crsTransformationAdapters), CompositeStrategy {

    override fun shouldContinueIterationOfAdaptersToInvoke(lastResultOrNullIfNoPrevious: TransformResult?): Boolean {
        return true
    }

    override fun calculateAggregatedResult(
            allResults: List<TransformResult>,
            inputCoordinate: Coordinate,
            crsIdentifierForOutputCoordinateSystem: CrsIdentifier,
            crsTransformationAdapterThatCreatedTheResult: CrsTransformationAdapter
    ): TransformResult {
        val resultsStatistic = ResultsStatistic(allResults)
        return this.calculateAggregatedResultBase(
            allResults,
            inputCoordinate,
            crsTransformationAdapterThatCreatedTheResult,
            resultsStatistic,
            { resultsStatistic.getCoordinateMedian() }
        )
    }
}