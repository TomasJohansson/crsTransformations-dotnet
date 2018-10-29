package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*

internal class CompositeStrategyForWeightedAverageValue(
    private val crsTransformationFacades: List<CrsTransformationFacade>,
    private val weights: Map<String, Double>
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
        var successCount = 0
        var sumLat = 0.0
        var sumLon = 0.0
        var weightSum = 0.0
        for (res: TransformResult in allResults) {
            if(res.isSuccess) {
                val weight: Double = weights[res.crsTransformationFacadeThatCreatedTheResult.getNameOfImplementation()]!!
                // TODO: ugly !! above
                successCount++
                val coord = res.outputCoordinate
                sumLat += weight * coord.yLatitude
                sumLon += weight * coord.xLongitude
                weightSum += weight
            }
        }
        if(successCount > 0) {
            var avgLat = sumLat / weightSum
            var avgLon = sumLon / weightSum
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

    companion object {
        @JvmStatic
        fun createCompositeStrategyForWeightedAverageValue(
            weightedFacades: List<FacadeWeight>
        ): CompositeStrategyForWeightedAverageValue {
            val facades: List<CrsTransformationFacade> = weightedFacades.map { it -> it.crsTransformationFacade }
            val map = HashMap<String, Double>()
            for (fw: FacadeWeight in weightedFacades) {
                map[fw.crsTransformationFacade.getNameOfImplementation()] = fw.weight
            }
            return CompositeStrategyForWeightedAverageValue(facades, map)
        }
    }
}