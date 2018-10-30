package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*

internal class CompositeStrategyForMedianValue(
    private val crsTransformationFacades: List<CrsTransformationFacade>
) : CompositeStrategyBase(crsTransformationFacades), CompositeStrategy {

    override fun shouldContinueIterationOfFacadesToInvoke(lastResultOrNullIfNoPrevious: TransformResult?): Boolean {
        return true
    }

    override fun calculateAggregatedResult(
        allResults: List<TransformResult>,
        inputCoordinate: Coordinate,
        crsIdentifierForOutputCoordinateSystem: CrsIdentifier,
        crsTransformationFacadeThatCreatedTheResult: CrsTransformationFacade
    ): TransformResult {
        val resultsStatistic = ResultsStatistic(allResults)
        return this.calculateAggregatedResultBase(
            allResults,
            inputCoordinate,
            crsTransformationFacadeThatCreatedTheResult,
            { resultsStatistic.getCoordinateMean() }
        )
    }
}