package com.programmerare.crsTransformations

data class Coordinate private constructor(
    val xLongitude: Double,
    val yLatitude: Double,
    val crsIdentifier: CrsIdentifier
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
            return createFromXLongYLat(
                xLongitude,
                yLatitude,
                CrsIdentifier.createFromEpsgNumber(epsgNumber)
            )
        }

        @JvmStatic
        fun createFromYLatXLong(
            yLatitude: Double,
            xLongitude: Double,
            epsgNumber: Int
        ): Coordinate {
            return createFromXLongYLat(
                xLongitude,
                yLatitude,
                CrsIdentifier.createFromEpsgNumber(epsgNumber)
            )
        }

        @JvmStatic
        fun createFromXLongYLat(
            xLongitude: Double,
            yLatitude: Double,
            crsCode: String
        ): Coordinate {
            return createFromXLongYLat(
                xLongitude,
                yLatitude,
                CrsIdentifier.createFromCrsCode(crsCode)
            )
        }

        @JvmStatic
        fun createFromYLatXLong(
            yLatitude: Double,
            xLongitude: Double,
            crsCode: String
        ): Coordinate {
            return createFromXLongYLat(
                xLongitude,
                yLatitude,
                CrsIdentifier.createFromCrsCode(crsCode)
            )
        }

        @JvmStatic
        fun createFromYLatXLong(
            yLatitude: Double,
            xLongitude: Double,
            crsIdentifier: CrsIdentifier
        ): Coordinate {
            return createFromXLongYLat(
                xLongitude,
                yLatitude,
                crsIdentifier
            )
        }

        @JvmStatic
        fun createFromXLongYLat(
            xLongitude: Double,
            yLatitude: Double,
            crsIdentifier: CrsIdentifier
        ): Coordinate {
            return Coordinate(
                xLongitude,
                yLatitude,
                crsIdentifier
            )
        }

        /**
         * WGS84 is probably the most common coordinate reference system,
         * the coordinates typically used with GPS.
         * Therefore it is default for the factory method not specifying
         * the coordinate reference system.
         */
        private val COORDINATE_REFERENCE_SYSTEM_WGS84 = CrsIdentifier.createFromEpsgNumber(4326)

        /**
         * The "GPS coordinate system" WGS84 is assumed when using this factory method.
         */
        @JvmStatic
        fun createFromLongLat(
            longitude: Double,
            latitude: Double
        ): Coordinate {
            return Coordinate(
                longitude,
                latitude,
                COORDINATE_REFERENCE_SYSTEM_WGS84
            )
        }

        /**
         * The "GPS coordinate system" WGS84 is assumed when using this factory method.
         */
        @JvmStatic
        fun createFromLatLong(
            latitude: Double,
            longitude: Double
        ): Coordinate {
            return Coordinate(
                longitude,
                latitude,
                COORDINATE_REFERENCE_SYSTEM_WGS84
            )
        }
    }
}