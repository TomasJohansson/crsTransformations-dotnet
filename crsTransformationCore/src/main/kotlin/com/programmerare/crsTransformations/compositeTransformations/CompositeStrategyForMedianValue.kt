package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*

class CompositeStrategyForMedianValue(private val crsTransformationFacades: List<CrsTransformationFacade>) : CompositeStrategy {

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
        val successFulCoordinateResults = allResults.filter { it.isSuccess }.map { it.outputCoordinate }
        if(allResults.size == 0) {
            return TransformResultImplementation(inputCoordinate, outputCoordinate = null, exception = null, isSuccess = false, crsTransformationFacadeThatCreatedTheResult = crsTransformationFacadeThatCreatedTheResult)
        }
        else {
            val lats = successFulCoordinateResults.map { it.yLatitude }
            val lons = successFulCoordinateResults.map { it.xLongitude }
            val medianLat = getMedianValue(lats)
            val medianLon = getMedianValue(lons)
            val outputCoordinate = Coordinate.createFromYLatXLong(medianLat, medianLon, crsIdentifierForOutputCoordinateSystem)
            return TransformResultImplementation(inputCoordinate, outputCoordinate = outputCoordinate, exception = null, isSuccess = true, crsTransformationFacadeThatCreatedTheResult = crsTransformationFacadeThatCreatedTheResult)
        }
    }

    companion object {
        @JvmStatic
        // Visibility "internal" did not seem to work, regarding getting Java access for
        // Java unit testing but "protected" works...
        // (and it did not work before when the test really was internal
        //  in the same module, but now it is moved to a separate test module)
        protected fun getMedianValue(lats: kotlin.collections.List<Double>): Double {
            val sortedDescending = lats.sortedDescending()
            val middle = sortedDescending.size / 2
            return if (sortedDescending.size % 2 == 1) {
                sortedDescending.get(middle)
            } else {
                return (sortedDescending.get(middle-1) + sortedDescending.get(middle)) / 2;
            }
        }
    }
}