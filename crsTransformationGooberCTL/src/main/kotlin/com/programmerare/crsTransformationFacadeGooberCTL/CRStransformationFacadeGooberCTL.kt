package com.programmerare.crsTransformationFacadeGooberCTL

import com.github.goober.coordinatetransformation.positions.SWEREF99Position
import com.github.goober.coordinatetransformation.positions.SWEREF99Position.SWEREFProjection
import com.github.goober.coordinatetransformation.positions.WGS84Position
import com.programmerare.crsTransformations.Coordinate
import java.lang.UnsupportedOperationException

// " goober/coordinate-transformation-library "
// https://github.com/goober/coordinate-transformation-library
object CRStransformationFacadeGooberCTL {

    // This ugly method signature (using a List<Double> as both input and output, will be refactored later
    // and will use a Coordinate object instead)
    @JvmStatic
    fun transform(
        inputCoordinate: Coordinate,
        epsgNumberForOutputCoordinateSystem: Int
    ): Coordinate {
        val errorMessage = "TODO: implement support for more EPSG codes than the only currently supported transform below"
        if(inputCoordinate.epsgNumber != 4326) throw UnsupportedOperationException(errorMessage)
        if(epsgNumberForOutputCoordinateSystem != 3006) throw UnsupportedOperationException(errorMessage)

        val wgs84Position = WGS84Position(inputCoordinate.yLatitude, inputCoordinate.xLongitude)
        val sweref99Position = SWEREF99Position(wgs84Position, SWEREFProjection.sweref_99_tm)
        return Coordinate(yLatitude = sweref99Position.latitude, xLongitude = sweref99Position.longitude, epsgNumber = epsgNumberForOutputCoordinateSystem)
    }
}