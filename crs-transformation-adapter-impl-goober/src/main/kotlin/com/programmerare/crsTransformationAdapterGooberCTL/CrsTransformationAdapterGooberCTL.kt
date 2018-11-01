package com.programmerare.crsTransformationAdapterGooberCTL

import com.github.goober.coordinatetransformation.Position
import com.github.goober.coordinatetransformation.positions.RT90Position.RT90Projection
import com.github.goober.coordinatetransformation.positions.RT90Position
import com.github.goober.coordinatetransformation.positions.SWEREF99Position
import com.github.goober.coordinatetransformation.positions.SWEREF99Position.SWEREFProjection
import com.github.goober.coordinatetransformation.positions.WGS84Position
import com.programmerare.crsTransformations.CrsTransformationAdapter
import com.programmerare.crsTransformations.CrsTransformationAdapterBaseLeaf
import com.programmerare.crsTransformations.Coordinate
import com.programmerare.crsTransformations.CrsIdentifier
import com.programmerare.crsTransformations.createFromYLatitudeXLongitude
import java.util.*

// " goober/coordinate-transformation-library "
// https://github.com/goober/coordinate-transformation-library
class CrsTransformationAdapterGooberCTL : CrsTransformationAdapterBaseLeaf(), CrsTransformationAdapter {

    override protected fun transformHook(
        inputCoordinate: Coordinate,
        crsIdentifierForOutputCoordinateSystem: CrsIdentifier
    ): Coordinate {
        if(!inputCoordinate.crsIdentifier.isEpsgCode) {
            throwIllegalArgumentException(inputCoordinate.crsIdentifier)
        }
        if(!crsIdentifierForOutputCoordinateSystem.isEpsgCode) {
            throwIllegalArgumentException(crsIdentifierForOutputCoordinateSystem)
        }
        val epsgNumberForInputCoordinateSystem = inputCoordinate.crsIdentifier.epsgNumber
        var positionToReturn: Position? = null

        // shorter names below for readibility purpose (lots os usages further down)
        val input = epsgNumberForInputCoordinateSystem
        val output = crsIdentifierForOutputCoordinateSystem.epsgNumber
        // "Int" is the data type for the above input and output
        // and below in the if statements they are used with extension functions
        // for semantic reasons i.e. readability.
        if(input.isRT90() && output.isWgs84()) { // procedural alternative: "if(isRT90(input) && isWgs84(output))"
            val rt90Position = RT90Position(inputCoordinate.yLatitude, inputCoordinate.xLongitude, rt90Projections[input])
            positionToReturn = rt90Position.toWGS84()

        } else if(input.isWgs84() && output.isRT90()) {
            val wgs84Position = WGS84Position(inputCoordinate.yLatitude, inputCoordinate.xLongitude)
            positionToReturn = RT90Position(wgs84Position, rt90Projections[output])

        } else if(input.isSweref99() && output.isWgs84()) {
            val sweREF99Position = SWEREF99Position(inputCoordinate.yLatitude, inputCoordinate.xLongitude, sweREFProjections[input])
            positionToReturn = sweREF99Position.toWGS84()

        } else if(input.isWgs84() && output.isSweref99()) {
            val wgs84Position = WGS84Position(inputCoordinate.yLatitude, inputCoordinate.xLongitude)
            positionToReturn = SWEREF99Position(wgs84Position, sweREFProjections[output])

        }

        if (positionToReturn != null) {
            return createFromYLatitudeXLongitude(yLatitude = positionToReturn.latitude, xLongitude = positionToReturn.longitude, crsIdentifier = crsIdentifierForOutputCoordinateSystem)
        } else if (
            // not direct support for transforming directly between SWEREF99 and RT90
            // but can do it by first transforming to WGS84 and then to the other
            (input.isSweref99() && output.isRT90())
            ||
            (input.isRT90() && output.isSweref99())
            || // transform between different Sweref systems
            (input.isSweref99() && output.isSweref99())
            || // transform between different RT90 systems
            (input.isRT90() && output.isRT90())
        ) {
            // first transform to WGS84
            val wgs84Coordinate = transformToCoordinate(inputCoordinate, WGS84)
            // then transform from WGS84
            return transformToCoordinate(wgs84Coordinate, crsIdentifierForOutputCoordinateSystem)
        } else {
            throw IllegalArgumentException("Unsupported transformation from $epsgNumberForInputCoordinateSystem to ${crsIdentifierForOutputCoordinateSystem.crsCode}")
        }
    }

    private fun throwIllegalArgumentException(crsIdentifier: CrsIdentifier) {
        throw IllegalArgumentException("Unsupported CRS: ${crsIdentifier.crsCode}")
    }

    private fun Int.isWgs84(): Boolean {
        return this == WGS84
    }

    private fun Int.isSweref99(): Boolean {
        return sweREFProjections.containsKey(this)
    }

    private fun Int.isRT90(): Boolean {
        return rt90Projections.containsKey(this)
    }

    private companion object {
        private val WGS84 = 4326

        private val rt90Projections = HashMap<Int, RT90Position.RT90Projection>()
        private val sweREFProjections = HashMap<Int, SWEREFProjection>()

        init {

            // TODO: maybe use the constants in an object such as ConstantEpsgNumber
            // instead of hardcoding the EPSG numbers as below
            // http://spatialreference.org/ref/?search=rt90
            rt90Projections.put(3019, RT90Projection.rt90_7_5_gon_v)    // EPSG:3019: RT90 7.5 gon V		https://epsg.io/3019
            rt90Projections.put(3020, RT90Projection.rt90_5_0_gon_v)    // EPSG:3020: RT90 5 gon V			https://epsg.io/3020
            rt90Projections.put(3021, RT90Projection.rt90_2_5_gon_v)    // EPSG:3021: RT90 2.5 gon V		https://epsg.io/3021
            rt90Projections.put(3022, RT90Projection.rt90_0_0_gon_v)    // EPSG:3022: RT90 0 gon			https://epsg.io/3022
            rt90Projections.put(3023, RT90Projection.rt90_2_5_gon_o)    // EPSG:3023: RT90 2.5 gon O		https://epsg.io/3023
            rt90Projections.put(3024, RT90Projection.rt90_5_0_gon_o)    // EPSG:3024: RT90 5 gon O			https://epsg.io/3024

            // http://spatialreference.org/ref/?search=sweref
            sweREFProjections.put(3006, SWEREFProjection.sweref_99_tm)       // EPSG:3006: SWEREF99 TM		https://epsg.io/3006
            sweREFProjections.put(3007, SWEREFProjection.sweref_99_12_00)    // EPSG:3007: SWEREF99 12 00	https://epsg.io/3007
            sweREFProjections.put(3008, SWEREFProjection.sweref_99_13_30)    // EPSG:3008: SWEREF99 13 30	https://epsg.io/3008
            sweREFProjections.put(3009, SWEREFProjection.sweref_99_15_00)    // EPSG:3009: SWEREF99 15 00	https://epsg.io/3009
            sweREFProjections.put(3010, SWEREFProjection.sweref_99_16_30)    // EPSG:3010: SWEREF99 16 30	https://epsg.io/3010
            sweREFProjections.put(3011, SWEREFProjection.sweref_99_18_00)    // EPSG:3011: SWEREF99 18 00	https://epsg.io/3011
            sweREFProjections.put(3012, SWEREFProjection.sweref_99_14_15)    // EPSG:3012: SWEREF99 14 15	https://epsg.io/3012
            sweREFProjections.put(3013, SWEREFProjection.sweref_99_15_45)    // EPSG:3013: SWEREF99 15 45	https://epsg.io/3013
            sweREFProjections.put(3014, SWEREFProjection.sweref_99_17_15)    // EPSG:3014: SWEREF99 17 15	https://epsg.io/3014
            sweREFProjections.put(3015, SWEREFProjection.sweref_99_18_45)    // EPSG:3015: SWEREF99 18 45	https://epsg.io/3015
            sweREFProjections.put(3016, SWEREFProjection.sweref_99_20_15)    // EPSG:3016: SWEREF99 20 15	https://epsg.io/3016
            sweREFProjections.put(3017, SWEREFProjection.sweref_99_21_45)    // EPSG:3017: SWEREF99 21 45	https://epsg.io/3017
            sweREFProjections.put(3018, SWEREFProjection.sweref_99_23_15)    // EPSG:3018: SWEREF99 23 15	https://epsg.io/3018
        }
    }
}