/**
 * From Java code this will look like a class 'CoordinateFactory'
 * with public static factory methods.
 * The Java class name: com.programmerare.crsTransformations.CoordinateFactory
 * From Kotlin code the methods are package level functions
 * and each function can be imported as if it would be a class, for example:
 *  import com.programmerare.crsTransformations.coordinate.createFromXLongitudeYLatitude
 *
 *  One advantage with using package level function instead of Kotlin object
 *  is that with a Kotlin object you can get the same kind of static
 *  method from Java code by using the Kotlin annotation '@JvmStatic'
 *  but then you would also see an "INSTANCES" like this:
 *      CoordinateFactory.INSTANCE.createFromXLongitudeYLatitude ...
 *  even though you can ignore it and just use:
 *      CoordinateFactory.createFromXLongitudeYLatitude ...
 *  but with package level function the Java clients will not even see such an "INSTANCE".
 */
@file:JvmName("CrsCoordinateFactory")
package com.programmerare.crsTransformations.coordinate
// The reason for having Coordinate and this CoordinateFactory
// in a package of its own is to avoid "polluting" the base
// package from lots of package level function defined in this file
// when using Kotlin code.
// (when using Java we do not see that problem but rather a class
//   CoordinateFactory with all these function as static method in that class)

import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier
import com.programmerare.crsTransformations.crsIdentifier.createFromCrsCode
import com.programmerare.crsTransformations.crsIdentifier.createFromEpsgNumber

// -------------------------------------------------------------------------

fun createFromXLongitudeYLatitude(
        xLongitude: Double,
        yLatitude: Double,
        epsgNumber: Int
): CrsCoordinate {
    return createFromXLongitudeYLatitude(
            xLongitude,
            yLatitude,
            createFromEpsgNumber(epsgNumber)
    )
}

fun xy(
        xLongitude: Double,
        yLatitude: Double,
        epsgNumber: Int
): CrsCoordinate {
    return createFromXLongitudeYLatitude(
            xLongitude,
            yLatitude,
            epsgNumber
    )
}

fun eastingNorthing(
    easting: Double,
    northing: Double,
    epsgNumber: Int
): CrsCoordinate {
    return createFromXLongitudeYLatitude(
        easting,
        northing,
        epsgNumber
    )
}

fun lonLat(
        xLongitude: Double,
        yLatitude: Double,
        epsgNumber: Int
): CrsCoordinate {
    return createFromXLongitudeYLatitude(
            xLongitude,
            yLatitude,
            epsgNumber
    )
}
// -------------------------------------------------------------------------


// -------------------------------------------------------------------------

fun createFromYLatitudeXLongitude(
        yLatitude: Double,
        xLongitude: Double,
        epsgNumber: Int
): CrsCoordinate {
    return createFromXLongitudeYLatitude(
            xLongitude,
            yLatitude,
            createFromEpsgNumber(epsgNumber)
    )
}


fun yx(
        yLatitude: Double,
        xLongitude: Double,
        epsgNumber: Int
): CrsCoordinate {
    return createFromYLatitudeXLongitude(
            yLatitude,
            xLongitude,
            epsgNumber
    )
}

fun northingEasting(
    northing: Double,
    easting: Double,
    epsgNumber: Int
): CrsCoordinate {
    return createFromXLongitudeYLatitude(
        easting,
        northing,
        epsgNumber
    )
}

fun latLon(
        yLatitude: Double,
        xLongitude: Double,
        epsgNumber: Int
): CrsCoordinate {
    return createFromYLatitudeXLongitude(
            yLatitude,
            xLongitude,
            epsgNumber
    )
}
// -------------------------------------------------------------------------

// -------------------------------------------------------------------------

fun createFromXLongitudeYLatitude(
        xLongitude: Double,
        yLatitude: Double,
        crsCode: String
): CrsCoordinate {
    return createFromXLongitudeYLatitude(
            xLongitude,
            yLatitude,
            createFromCrsCode(crsCode)
    )
}


fun xy(
        xLongitude: Double,
        yLatitude: Double,
        crsCode: String
): CrsCoordinate {
    return createFromXLongitudeYLatitude(
            xLongitude,
            yLatitude,
            crsCode
    )
}

fun eastingNorthing(
    easting: Double,
    northing: Double,
    crsCode: String
): CrsCoordinate {
    return createFromXLongitudeYLatitude(
        easting,
        northing,
        crsCode
    )
}

fun lonLat(
        xLongitude: Double,
        yLatitude: Double,
        crsCode: String
): CrsCoordinate {
    return createFromXLongitudeYLatitude(
            xLongitude,
            yLatitude,
            crsCode
    )
}
// -------------------------------------------------------------------------


// -------------------------------------------------------------------------

fun createFromYLatitudeXLongitude(
        yLatitude: Double,
        xLongitude: Double,
        crsCode: String
): CrsCoordinate {
    return createFromXLongitudeYLatitude(
            xLongitude,
            yLatitude,
            createFromCrsCode(crsCode)
    )
}

fun yx(
        yLatitude: Double,
        xLongitude: Double,
        crsCode: String
): CrsCoordinate {
    return createFromYLatitudeXLongitude(
            yLatitude,
            xLongitude,
            crsCode
    )
}

fun northingEasting(
    northing: Double,
    easting: Double,
    crsCode: String
): CrsCoordinate {
    return createFromXLongitudeYLatitude(
        easting,
        northing,
        crsCode
    )
}


fun latLon(
        yLatitude: Double,
        xLongitude: Double,
        crsCode: String
): CrsCoordinate {
    return createFromYLatitudeXLongitude(
            yLatitude,
            xLongitude,
            crsCode
    )
}
// -------------------------------------------------------------------------

// -------------------------------------------------------------------------

fun createFromYLatitudeXLongitude(
        yLatitude: Double,
        xLongitude: Double,
        crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return createFromXLongitudeYLatitude(
            xLongitude,
            yLatitude,
            crsIdentifier
    )
}

fun yx(
        yLatitude: Double,
        xLongitude: Double,
        crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return createFromYLatitudeXLongitude(
            yLatitude,
            xLongitude,
            crsIdentifier
    )
}

fun northingEasting(
    northing: Double,
    easting: Double,
    crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return createFromYLatitudeXLongitude(
        northing,
        easting,
        crsIdentifier
    )
}

fun latLon(
        yLatitude: Double,
        xLongitude: Double,
        crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return createFromYLatitudeXLongitude(
            yLatitude,
            xLongitude,
            crsIdentifier
    )
}
// -------------------------------------------------------------------------

// -------------------------------------------------------------------------

fun createFromXLongitudeYLatitude(
        xLongitude: Double,
        yLatitude: Double,
        crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return CrsCoordinate(
            xLongitude,
            yLatitude,
            crsIdentifier
    )
}

fun xy(
        xLongitude: Double,
        yLatitude: Double,
        crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return createFromXLongitudeYLatitude(
            xLongitude,
            yLatitude,
            crsIdentifier
    )
}

fun eastingNorthing(
    easting: Double,
    northing: Double,
    crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return createFromYLatitudeXLongitude(
        northing,
        easting,
        crsIdentifier
    )
}

fun lonLat(
        xLongitude: Double,
        yLatitude: Double,
        crsIdentifier: CrsIdentifier
): CrsCoordinate {
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
private val COORDINATE_REFERENCE_SYSTEM_WGS84 = createFromEpsgNumber(4326)

// -------------------------------------------------------------------------
/**
 * The "GPS coordinate system" WGS84 is assumed when using this factory method.
 */

fun createFromLongitudeLatitude(
        longitude: Double,
        latitude: Double
): CrsCoordinate {
    return CrsCoordinate(
            longitude,
            latitude,
            COORDINATE_REFERENCE_SYSTEM_WGS84
    )
}

fun lonLat(
        longitude: Double,
        latitude: Double
): CrsCoordinate {
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
fun createFromLatitudeLongitude(
        latitude: Double,
        longitude: Double
): CrsCoordinate {
    return CrsCoordinate(
            longitude,
            latitude,
            COORDINATE_REFERENCE_SYSTEM_WGS84
    )
}

fun latLon(
        latitude: Double,
        longitude: Double
): CrsCoordinate {
    return createFromLatitudeLongitude(
            latitude,
            longitude
    )
}
// -------------------------------------------------------------------------
