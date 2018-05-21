package com.programmerare.crsTransformations

class Coordinate private constructor(
    val xLongitude: Double,
    val yLatitude: Double,
    val epsgNumber: Int
) {
    // The constructor is private to force client code to use the below factory methods
    // which are named to indicate the order of the longitude and latitude parameters.
    companion object {
        @JvmStatic // https://kotlinlang.org/docs/reference/java-to-kotlin-interop.html
        fun createFromXLongYLat(
            xLongitude: Double,
            yLatitude: Double,
            epsgNumber: Int
        ): Coordinate {
            return Coordinate(
                xLongitude,
                yLatitude,
                epsgNumber
            )
        }

        @JvmStatic
        fun createFromYLatXLong(
            yLatitude: Double,
            xLongitude: Double,
            epsgNumber: Int
        ): Coordinate {
            return Coordinate(
                xLongitude,
                yLatitude,
                epsgNumber
            )
        }
    }
}