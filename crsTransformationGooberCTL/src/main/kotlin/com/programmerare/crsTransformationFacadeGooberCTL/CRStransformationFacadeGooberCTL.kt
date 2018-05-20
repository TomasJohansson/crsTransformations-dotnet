package com.programmerare.crsTransformationFacadeGooberCTL

import com.github.goober.coordinatetransformation.positions.SWEREF99Position
import com.github.goober.coordinatetransformation.positions.SWEREF99Position.SWEREFProjection
import com.github.goober.coordinatetransformation.positions.WGS84Position
import com.programmerare.crsTransformations.Coordinate
import java.util.*

// " goober/coordinate-transformation-library "
// https://github.com/goober/coordinate-transformation-library
object CRStransformationFacadeGooberCTL {

    // This ugly method signature (using a List<Double> as both input and output, will be refactored later
    // and will use a Coordinate object instead)
    @JvmStatic
    fun transformWgs84CoordinateToSweref99TM(
        inputCoordinate: Coordinate
    ): Coordinate {
        val wgs84Position = WGS84Position(inputCoordinate.yLatitude, inputCoordinate.xLongitude)
        val sweref99Position = SWEREF99Position(wgs84Position, SWEREFProjection.sweref_99_tm)
        val epsgNumberForSweref99TM = 3006
        return Coordinate(yLatitude = sweref99Position.latitude, xLongitude = sweref99Position.longitude, epsgNumber = epsgNumberForSweref99TM)
    }
}