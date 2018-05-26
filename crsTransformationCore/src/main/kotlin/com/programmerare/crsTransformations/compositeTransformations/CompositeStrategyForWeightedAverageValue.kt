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
                val weight: Double = weights[res.crsTransformationFacadeThatCreatedTheResult.javaClass.name]!!
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
            weightedFacades: List<Pair<CrsTransformationFacade, Double>>
        ): CompositeStrategyForWeightedAverageValue {

            val name: String = "com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL"
            val obj: CrsTransformationFacade = Class.forName(name).getDeclaredConstructor().newInstance() as CrsTransformationFacade

            val facades: List<CrsTransformationFacade> = weightedFacades.map { it -> it.first }
            val map = HashMap<String, Double>()
            for ((k, v) in weightedFacades) {
                println("$k -> $v")
                println("k: $k")
                println("v: $v")
                map[k.javaClass.name] = v
            }
            return CompositeStrategyForWeightedAverageValue(facades, map)
        }

        @JvmStatic
        fun createCompositeStrategyForWeightedAverageValueByReflection(
            weightedFacades: List<Pair<String, Double>>
        ): CompositeStrategyForWeightedAverageValue {
            val weights: List<Pair<CrsTransformationFacade, Double>> = weightedFacades.map { it -> Pair(Class.forName(it.first).getDeclaredConstructor().newInstance() as CrsTransformationFacade, it.second) }
            return createCompositeStrategyForWeightedAverageValue(weights)
        }
    }
}