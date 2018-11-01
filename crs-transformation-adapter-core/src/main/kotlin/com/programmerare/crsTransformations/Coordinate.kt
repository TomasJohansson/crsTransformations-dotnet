package com.programmerare.crsTransformations

/**
 * TODO general (class level) documentation about the different factory methods
 * Roughly something like this:
 * three parameters: x , y and coordinate system
 * coodinate system is EPSG, and can be of three types, integer, string (with ESPG: prefix) or an object which is internally created by the others ..
 * (if multiple coordinates are to be created, use the object version, otherwise more convenient to use integer or string constants)
 * The factory methods have different names depending on which order you want to specify
 * the two coordinates, and also depending on how long names you want and if you prefer x/y or lon/lat.
 * These three methods are alternatives, doing exatly the same with the same parameters:
 *  createFromXLongitudeYLatitude , xy , lonLat
 * These three methods are alternatives, doing exatly the same with the same parameters:
 *  createFromYLatitudeXLongitude , yx , latLon
 */
class Coordinate private constructor(
    val xLongitude: Double,
    val yLatitude: Double,
    val crsIdentifier: CrsIdentifier
) {
    // The constructor is private to force client code to use the below factory methods
    // which are named to indicate the order of the longitude and latitude parameters.

    /**
     * Not intended to be used with ".Companion" from client code.
     * The reason for its existence has to do with the fact that the
     * JVM class has been created with the programming language Kotlin.
     */
    companion object {
        // -------------------------------------------------------------------------
        @JvmStatic // https://kotlinlang.org/docs/reference/java-to-kotlin-interop.html
        fun createFromXLongitudeYLatitude(
            xLongitude: Double,
            yLatitude: Double,
            epsgNumber: Int
        ): Coordinate {
            return createFromXLongitudeYLatitude(
                xLongitude,
                yLatitude,
                CrsIdentifier.createFromEpsgNumber(epsgNumber)
            )
        }
        @JvmStatic // https://kotlinlang.org/docs/reference/java-to-kotlin-interop.html
        fun xy(
            xLongitude: Double,
            yLatitude: Double,
            epsgNumber: Int
        ): Coordinate {
            return createFromXLongitudeYLatitude(
                xLongitude,
                yLatitude,
                epsgNumber
            )
        }
        @JvmStatic // https://kotlinlang.org/docs/reference/java-to-kotlin-interop.html
        fun lonLat(
            xLongitude: Double,
            yLatitude: Double,
            epsgNumber: Int
        ): Coordinate {
            return createFromXLongitudeYLatitude(
                xLongitude,
                yLatitude,
                epsgNumber
            )
        }
        // -------------------------------------------------------------------------


        // -------------------------------------------------------------------------
        @JvmStatic
        fun createFromYLatitudeXLongitude(
            yLatitude: Double,
            xLongitude: Double,
            epsgNumber: Int
        ): Coordinate {
            return createFromXLongitudeYLatitude(
                xLongitude,
                yLatitude,
                CrsIdentifier.createFromEpsgNumber(epsgNumber)
            )
        }

        @JvmStatic
        fun yx(
            yLatitude: Double,
            xLongitude: Double,
            epsgNumber: Int
        ): Coordinate {
            return createFromYLatitudeXLongitude(
                yLatitude,
                xLongitude,
                epsgNumber
            )
        }

        @JvmStatic
        fun latLon(
            yLatitude: Double,
            xLongitude: Double,
            epsgNumber: Int
        ): Coordinate {
            return createFromYLatitudeXLongitude(
                yLatitude,
                xLongitude,
                epsgNumber
            )
        }
        // -------------------------------------------------------------------------

        // -------------------------------------------------------------------------
        @JvmStatic
        fun createFromXLongitudeYLatitude(
            xLongitude: Double,
            yLatitude: Double,
            crsCode: String
        ): Coordinate {
            return createFromXLongitudeYLatitude(
                xLongitude,
                yLatitude,
                CrsIdentifier.createFromCrsCode(crsCode)
            )
        }

        @JvmStatic
        fun xy(
            xLongitude: Double,
            yLatitude: Double,
            crsCode: String
        ): Coordinate {
            return createFromXLongitudeYLatitude(
                xLongitude,
                yLatitude,
                crsCode
            )
        }

        @JvmStatic
        fun lonLat(
            xLongitude: Double,
            yLatitude: Double,
            crsCode: String
        ): Coordinate {
            return createFromXLongitudeYLatitude(
                xLongitude,
                yLatitude,
                crsCode
            )
        }
        // -------------------------------------------------------------------------


        // -------------------------------------------------------------------------
        @JvmStatic
        fun createFromYLatitudeXLongitude(
            yLatitude: Double,
            xLongitude: Double,
            crsCode: String
        ): Coordinate {
            return createFromXLongitudeYLatitude(
                xLongitude,
                yLatitude,
                CrsIdentifier.createFromCrsCode(crsCode)
            )
        }

        @JvmStatic
        fun yx(
            yLatitude: Double,
            xLongitude: Double,
            crsCode: String
        ): Coordinate {
            return createFromYLatitudeXLongitude(
                yLatitude,
                xLongitude,
                crsCode
            )
        }

        @JvmStatic
        fun latLon(
            yLatitude: Double,
            xLongitude: Double,
            crsCode: String
        ): Coordinate {
            return createFromYLatitudeXLongitude(
                yLatitude,
                xLongitude,
                crsCode
            )
        }
        // -------------------------------------------------------------------------

        // -------------------------------------------------------------------------
        @JvmStatic
        fun createFromYLatitudeXLongitude(
            yLatitude: Double,
            xLongitude: Double,
            crsIdentifier: CrsIdentifier
        ): Coordinate {
            return createFromXLongitudeYLatitude(
                xLongitude,
                yLatitude,
                crsIdentifier
            )
        }

        @JvmStatic
        fun yx(
            yLatitude: Double,
            xLongitude: Double,
            crsIdentifier: CrsIdentifier
        ): Coordinate {
            return createFromYLatitudeXLongitude(
                yLatitude,
                xLongitude,
                crsIdentifier
            )
        }

        @JvmStatic
        fun latLon(
            yLatitude: Double,
            xLongitude: Double,
            crsIdentifier: CrsIdentifier
        ): Coordinate {
            return createFromYLatitudeXLongitude(
                yLatitude,
                xLongitude,
                crsIdentifier
            )
        }
        // -------------------------------------------------------------------------

        // -------------------------------------------------------------------------
        @JvmStatic
        fun createFromXLongitudeYLatitude(
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

        @JvmStatic
        fun xy(
            xLongitude: Double,
            yLatitude: Double,
            crsIdentifier: CrsIdentifier
        ): Coordinate {
            return createFromXLongitudeYLatitude(
                xLongitude,
                yLatitude,
                crsIdentifier
            )
        }

        @JvmStatic
        fun lonLat(
            xLongitude: Double,
            yLatitude: Double,
            crsIdentifier: CrsIdentifier
        ): Coordinate {
            return createFromXLongitudeYLatitude(
                xLongitude,
                yLatitude,
                crsIdentifier
            )
        }
        // -------------------------------------------------------------------------

        /**
         * WGS84 is probably the most common coordinate reference system,
         * the coordinates typically used with GPS.
         * Therefore it is default for the factory method not specifying
         * the coordinate reference system.
         */
        private val COORDINATE_REFERENCE_SYSTEM_WGS84 = CrsIdentifier.createFromEpsgNumber(4326)

        // -------------------------------------------------------------------------
        /**
         * The "GPS coordinate system" WGS84 is assumed when using this factory method.
         */
        @JvmStatic
        fun createFromLongitudeLatitude(
            longitude: Double,
            latitude: Double
        ): Coordinate {
            return Coordinate(
                longitude,
                latitude,
                COORDINATE_REFERENCE_SYSTEM_WGS84
            )
        }

        @JvmStatic
        fun lonLat(
            longitude: Double,
            latitude: Double
        ): Coordinate {
            return createFromLongitudeLatitude(
                longitude,
                latitude
            )
        }
        // -------------------------------------------------------------------------

        // -------------------------------------------------------------------------
        /**
         * The "GPS coordinate system" WGS84 is assumed when using this factory method.
         */
        @JvmStatic
        fun createFromLatitudeLongitude(
            latitude: Double,
            longitude: Double
        ): Coordinate {
            return Coordinate(
                longitude,
                latitude,
                COORDINATE_REFERENCE_SYSTEM_WGS84
            )
        }

        @JvmStatic
        fun latLon(
            latitude: Double,
            longitude: Double
        ): Coordinate {
            return createFromLatitudeLongitude(
                latitude,
                longitude
            )
        }
        // -------------------------------------------------------------------------
    }

    //------------------------------------------------------------------
    // The implementation of the following three methods were generated by IntelliJ IDEA
    override fun equals(other: Any?): Boolean {
        if (this === other) return true
        if (javaClass != other?.javaClass) return false

        other as Coordinate

        if (xLongitude != other.xLongitude) return false
        if (yLatitude != other.yLatitude) return false
        if (crsIdentifier != other.crsIdentifier) return false

        return true
    }

    override fun hashCode(): Int {
        var result = xLongitude.hashCode()
        result = 31 * result + yLatitude.hashCode()
        result = 31 * result + crsIdentifier.hashCode()
        return result
    }

    override fun toString(): String {
        return "Coordinate(xLongitude=$xLongitude, yLatitude=$yLatitude, crsIdentifier=$crsIdentifier)"
    }
    //------------------------------------------------------------------
}