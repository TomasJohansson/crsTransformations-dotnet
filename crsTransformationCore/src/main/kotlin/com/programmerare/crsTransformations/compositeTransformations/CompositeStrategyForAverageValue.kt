package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*

internal class CompositeStrategyForAverageValue(
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
        // TODO: reuse the new ResultsStatistic for the calculation
        
        val successfulCoordinates = allResults.filter { it.isSuccess }.map { it.outputCoordinate }
        val successCount = successfulCoordinates.size
        if(successCount > 0) {
            val sumLat = successfulCoordinates.map { it.yLatitude }.sum()
            val sumLon = successfulCoordinates.map { it.xLongitude }.sum()
            val avgLat = sumLat / successCount
            val avgLon = sumLon / successCount
            val coordRes = Coordinate.createFromYLatitudeXLongitude(avgLat, avgLon, crsIdentifierForOutputCoordinateSystem)
            return TransformResultImplementation(
                inputCoordinate,
                outputCoordinate = coordRes,
                exception = null,
                isSuccess = true,
                crsTransformationFacadeThatCreatedTheResult = crsTransformationFacadeThatCreatedTheResult,
                subResults = allResults
            )
        }
        else {
            return TransformResultImplementation(
                inputCoordinate,
                outputCoordinate = null,
                exception = null,
                isSuccess = false,
                crsTransformationFacadeThatCreatedTheResult = crsTransformationFacadeThatCreatedTheResult,
                subResults = allResults
            )
        }
   }
}