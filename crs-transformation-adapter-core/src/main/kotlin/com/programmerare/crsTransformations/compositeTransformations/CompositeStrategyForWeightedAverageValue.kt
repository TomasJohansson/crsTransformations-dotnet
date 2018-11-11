package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.CrsTransformationAdapteeType
import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationResult
import com.programmerare.crsTransformations.coordinate.CrsCoordinate
import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier
import com.programmerare.crsTransformations.coordinate.createFromYNorthingLatitudeAndXEastingLongitude

internal class CompositeStrategyForWeightedAverageValue(
    private val crsTransformationAdapters: List<CrsTransformationAdapter>,
    private val weights: Map<String, Double>
) : CompositeStrategyBase(crsTransformationAdapters), CompositeStrategy {

    override fun shouldContinueIterationOfAdaptersToInvoke(lastResultOrNullIfNoPrevious: CrsTransformationResult?): Boolean {
        return true
    }

    override fun calculateAggregatedResult(
        allResults: List<CrsTransformationResult>,
        inputCoordinate: CrsCoordinate,
        crsIdentifierForOutputCoordinateSystem: CrsIdentifier,
        crsTransformationAdapterThatCreatedTheResult: CrsTransformationAdapter
    ): CrsTransformationResult {
        var successCount = 0
        var sumLat = 0.0
        var sumLon = 0.0
        var weightSum = 0.0
        for (res: CrsTransformationResult in allResults) {
            if(res.isSuccess) {
                val weight: Double = this.weights.getOrElse(res.crsTransformationAdapterResultSource.getLongNameOfImplementation(), { -1.0 })
                if(weight < 0) {
                    throw RuntimeException("The implementation was not configured with a non-null and non-negative weight value for the implementation "+ res.crsTransformationAdapterResultSource.getLongNameOfImplementation())
                }
                successCount++
                val coord = res.outputCoordinate
                sumLat += weight * coord.yNorthingLatitude
                sumLon += weight * coord.xEastingLongitude
                weightSum += weight
            }
        }
        if(successCount > 0) {
            var avgLat = sumLat / weightSum
            var avgLon = sumLon / weightSum
            val coordRes = createFromYNorthingLatitudeAndXEastingLongitude(avgLat, avgLon, crsIdentifierForOutputCoordinateSystem)
            return CrsTransformationResult(
                inputCoordinate,
                _outputCoordinate = coordRes,
                exception = null,
                isSuccess = true,
                crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                transformationResultChildren = allResults
            )
        }
        else {
            return CrsTransformationResult(
                inputCoordinate,
                _outputCoordinate = null,
                exception = null,
                isSuccess = false,
                crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                transformationResultChildren = allResults
            )
        }
    }

    /**
     * Not intended to be used with ".Companion" from client code.
     * The reason for its existence has to do with the fact that the
     * JVM class has been created with the programming language Kotlin.
     * Precondition: All weight values in the list must be non-negative.
     */
    companion object {
        @JvmStatic
        fun createCompositeStrategyForWeightedAverageValue(
            weightedCrsTransformationAdapters: List<CrsTransformationAdapterWeight>
        ): CompositeStrategyForWeightedAverageValue {
            val adapters: List<CrsTransformationAdapter> = weightedCrsTransformationAdapters.map { it -> it.crsTransformationAdapter }
            val map = HashMap<String, Double>()
            for (fw: CrsTransformationAdapterWeight in weightedCrsTransformationAdapters) {
                //  no need to check for negative weight values here since it 
                // should be enforced already at construction with an exception being thrown
                // if the below weight value would be non-positive 
                map[fw.crsTransformationAdapter.getLongNameOfImplementation()] = fw.weight
            }
            return CompositeStrategyForWeightedAverageValue(adapters, map)
        }
    }

    override fun getAdapteeType() : CrsTransformationAdapteeType {
        return CrsTransformationAdapteeType.COMPOSITE_WEIGHTED_AVERAGE
    }
}