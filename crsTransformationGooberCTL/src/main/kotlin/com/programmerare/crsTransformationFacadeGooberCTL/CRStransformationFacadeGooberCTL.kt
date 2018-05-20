package com.programmerare.crsTransformationFacadeGooberCTL

import com.github.goober.coordinatetransformation.positions.SWEREF99Position
import com.github.goober.coordinatetransformation.positions.SWEREF99Position.SWEREFProjection
import com.github.goober.coordinatetransformation.positions.WGS84Position
import java.util.*

// " goober/coordinate-transformation-library "
// https://github.com/goober/coordinate-transformation-library
object CRStransformationFacadeGooberCTL {

    // This ugly method signature (using a List<Double> as both input and output, will be refactored later
    // and will use a Coordinate object instead)
    @JvmStatic
    fun transformWgs84CoordinateToSweref99TM(
        inputCoordinate: List<Double>
    ): List<Double> {
        val wgs84Lat = inputCoordinate.get(0)
        val wgs84Lon = inputCoordinate.get(1)
        val wgs84Position = WGS84Position(wgs84Lat, wgs84Lon)
        val sweref99Position = SWEREF99Position(wgs84Position, SWEREFProjection.sweref_99_tm)
        return Arrays.asList(sweref99Position.latitude, sweref99Position.longitude)
    }
}