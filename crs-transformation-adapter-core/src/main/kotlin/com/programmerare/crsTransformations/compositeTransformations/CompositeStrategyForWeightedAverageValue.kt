package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*

internal class CompositeStrategyForWeightedAverageValue(
        private val crsTransformationAdapters: List<CrsTransformationAdapter>,
        private val weights: Map<String, Double>
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
        var successCount = 0
        var sumLat = 0.0
        var sumLon = 0.0
        var weightSum = 0.0
        for (res: TransformResult in allResults) {
            if(res.isSuccess) {
                val weight: Double = weights[res.crsTransformationAdapterThatCreatedTheResult.getNameOfImplementation()]!!
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
                crsTransformationAdapterThatCreatedTheResult = crsTransformationAdapterThatCreatedTheResult,
                subResults = allResults
            )
        }
        else {
            return TransformResultImplementation(
                inputCoordinate,
                outputCoordinate = null,
                exception = null,
                isSuccess = false,
                crsTransformationAdapterThatCreatedTheResult = crsTransformationAdapterThatCreatedTheResult,
                subResults = allResults
            )
        }
    }

    companion object {
        @JvmStatic
        fun createCompositeStrategyForWeightedAverageValue(
                weightedAdapters: List<AdapterWeight>
        ): CompositeStrategyForWeightedAverageValue {
            val adapters: List<CrsTransformationAdapter> = weightedAdapters.map { it -> it.crsTransformationAdapter }
            val map = HashMap<String, Double>()
            for (fw: AdapterWeight in weightedAdapters) {
                map[fw.crsTransformationAdapter.getNameOfImplementation()] = fw.weight
            }
            return CompositeStrategyForWeightedAverageValue(adapters, map)
        }
    }
}