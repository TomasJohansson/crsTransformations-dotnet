package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import java.util.List

class CrsTransformationFacadeMedian(crsTransformationFacades: List<CrsTransformationFacade>) : CrsTransformationFacadeBaseComposite(crsTransformationFacades) {

    override fun transformToResultObject(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): TransformResult {
        val successFulCoordinateResults = crsTransformationFacades.map { it.transformToResultObject(inputCoordinate, crsIdentifierForOutputCoordinateSystem) }.filter { it.isSuccess }.map { it.outputCoordinate }
        if(successFulCoordinateResults.size == 0) {
            return TransformResultImplementation(inputCoordinate, outputCoordinate = null, exception = null, isSuccess = false)
        }
        else {
            val lats = successFulCoordinateResults.map { it.yLatitude }
            val lons = successFulCoordinateResults.map { it.xLongitude }
            val medianLat = getMedianValue(lats)
            val medianLon = getMedianValue(lons)
            val outputCoordinate = Coordinate.createFromYLatXLong(medianLat, medianLon, crsIdentifierForOutputCoordinateSystem)
            return TransformResultImplementation(inputCoordinate, outputCoordinate = outputCoordinate, exception = null, isSuccess = true)
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