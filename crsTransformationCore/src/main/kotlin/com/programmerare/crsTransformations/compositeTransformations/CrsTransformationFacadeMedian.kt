package com.programmerare.crsTransformations.compositeTransformations

import com.programmerare.crsTransformations.*
import java.util.List

// TODO: remove this class

class CrsTransformationFacadeMedian(compositeStrategy: CompositeStrategy): CrsTransformationFacadeBaseComposite(compositeStrategy) {

//    override fun transformToResultObject(inputCoordinate: Coordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): TransformResult {
//        val crsTransformationFacades = compositeStrategy.getAllTransformationFacadesInTheOrderTheyShouldBeInvoked()
//        val successFulCoordinateResults = crsTransformationFacades.map { it.transformToResultObject(inputCoordinate, crsIdentifierForOutputCoordinateSystem) }.filter { it.isSuccess }.map { it.outputCoordinate }
//        if(successFulCoordinateResults.size == 0) {
//            return TransformResultImplementation(inputCoordinate, outputCoordinate = null, exception = null, isSuccess = false)
//        }
//        else {
//            val lats = successFulCoordinateResults.map { it.yLatitude }
//            val lons = successFulCoordinateResults.map { it.xLongitude }
//            val medianLat = getMedianValue(lats)
//            val medianLon = getMedianValue(lons)
//            val outputCoordinate = Coordinate.createFromYLatXLong(medianLat, medianLon, crsIdentifierForOutputCoordinateSystem)
//            return TransformResultImplementation(inputCoordinate, outputCoordinate = outputCoordinate, exception = null, isSuccess = true)
//        }
//    }

    companion object {
        @JvmStatic
          fun getMedianValue(lats: kotlin.collections.List<Double>): Double {
            return CompositeStrategyForMedianValue.getMedianValue(lats)
        }
    }
}