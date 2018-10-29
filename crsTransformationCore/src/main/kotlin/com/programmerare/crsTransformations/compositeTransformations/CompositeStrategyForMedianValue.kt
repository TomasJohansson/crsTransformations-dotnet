package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import com.programmerare.crsTransformations.utils.MedianValueUtility

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
        val successFulCoordinateResults = allResults.filter { it.isSuccess }.map { it.outputCoordinate }
        if(allResults.size == 0) {
            return TransformResultImplementation(
                inputCoordinate,
                outputCoordinate = null,
                exception = null,
                isSuccess = false,
                crsTransformationFacadeThatCreatedTheResult = crsTransformationFacadeThatCreatedTheResult,
                subResults = allResults
            )
        }
        else {
            val lats = successFulCoordinateResults.map { it.yLatitude }
            val lons = successFulCoordinateResults.map { it.xLongitude }
            val medianLat = MedianValueUtility.getMedianValue(lats)
            val medianLon = MedianValueUtility.getMedianValue(lons)
            val outputCoordinate = Coordinate.createFromYLatitudeXLongitude(medianLat, medianLon, crsIdentifierForOutputCoordinateSystem)
            return TransformResultImplementation(
                inputCoordinate,
                outputCoordinate = outputCoordinate,
                exception = null,
                isSuccess = true,
                crsTransformationFacadeThatCreatedTheResult = crsTransformationFacadeThatCreatedTheResult,
                subResults = allResults
            )
        }
    }
}