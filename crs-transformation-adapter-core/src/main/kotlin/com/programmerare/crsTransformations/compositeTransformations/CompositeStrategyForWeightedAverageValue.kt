package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.TransformResult
import com.programmerare.crsTransformations.Coordinate
import com.programmerare.crsTransformations.CrsIdentifier
import com.programmerare.crsTransformations.TransformResultImplementation
import com.programmerare.crsTransformations.createFromYLatitudeXLongitude

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
            val coordRes = createFromYLatitudeXLongitude(avgLat, avgLon, crsIdentifierForOutputCoordinateSystem)
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

    /**
     * Not intended to be used with ".Companion" from client code.
     * The reason for its existence has to do with the fact that the
     * JVM class has been created with the programming language Kotlin.
     */
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