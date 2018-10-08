package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*

class CompositeStrategyForWeightedAverageValue(
        private val crsTransformationFacades: List<CrsTransformationFacade>,
        private val weights: Map<String, Double>
) : CompositeStrategy {
    override fun getAllTransformationFacadesInTheOrderTheyShouldBeInvoked(): List<CrsTransformationFacade> {
        return crsTransformationFacades
    }

    override fun shouldInvokeNextFacade(allResultsSoFar: List<TransformResult>, lastResultOrNullIfNoPrevious: TransformResult?, nextFacadeToInvoke: CrsTransformationFacade): Boolean {
        return true
    }

    override fun shouldContinueIterationOfFacadesToInvoke(allResultsSoFar: List<TransformResult>, lastResultOrNullIfNoPrevious: TransformResult?): Boolean {
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
            val coordRes = Coordinate.createFromYLatXLong(avgLat, avgLon, crsIdentifierForOutputCoordinateSystem)
            return TransformResultImplementation(inputCoordinate, outputCoordinate = coordRes, exception = null, isSuccess = true, crsTransformationFacadeThatCreatedTheResult = crsTransformationFacadeThatCreatedTheResult)
        }
        else {
            // TODO: aggregate mroe from the results e.g. exception messages
            return TransformResultImplementation(inputCoordinate, outputCoordinate = null, exception = null, isSuccess = false, crsTransformationFacadeThatCreatedTheResult = crsTransformationFacadeThatCreatedTheResult)
        }
    }

    companion object {
        @JvmStatic
        fun createCompositeStrategyForWeightedAverageValue(
            weightedFacades: List<FacadeAndWeight>
        ): CompositeStrategyForWeightedAverageValue {
            val facades: List<CrsTransformationFacade> = weightedFacades.map { it -> it.crsTransformationFacade }
            val map = HashMap<String, Double>()
            for (faw: FacadeAndWeight in weightedFacades) {
                map[faw.crsTransformationFacade.getNameOfImplementation()] = faw.weight
            }
            return CompositeStrategyForWeightedAverageValue(facades, map)
        }

    }
}