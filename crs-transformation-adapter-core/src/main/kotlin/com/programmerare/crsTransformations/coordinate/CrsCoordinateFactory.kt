/**
 * From Java code this will look like a class 'CoordinateFactory'
 * with public static factory methods.
 * The Java class name: com.programmerare.crsTransformations.CoordinateFactory
 * From Kotlin code the methods are package level functions
 * and each function can be imported as if it would be a class, for example:
 *  import com.programmerare.crsTransformations.coordinate.createFromXEastingLongitudeAndYNorthingLatitude
 *
 *  One advantage with using package level function instead of Kotlin object
 *  is that with a Kotlin object you can get the same kind of static
 *  method from Java code by using the Kotlin annotation '@JvmStatic'
 *  but then you would also see an "INSTANCES" like this:
 *      CoordinateFactory.INSTANCE.createFromXEastingLongitudeAndYNorthingLatitude ...
 *  even though you can ignore it and just use:
 *      CoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude ...
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

fun createFromXEastingLongitudeAndYNorthingLatitude(
    xEastingLongitude: Double,
    yNorthingLatitude: Double,
    epsgNumber: Int
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        xEastingLongitude,
        yNorthingLatitude,
        createFromEpsgNumber(epsgNumber)
    )
}

fun xy(
    x: Double,
    y: Double,
    epsgNumber: Int
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        x,
        y,
        epsgNumber
    )
}

fun eastingNorthing(
    easting: Double,
    northing: Double,
    epsgNumber: Int
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        easting,
        northing,
        epsgNumber
    )
}

fun lonLat(
    longitude: Double,
    latitude: Double,
    epsgNumber: Int
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        longitude,
        latitude,
        epsgNumber
    )
}
// -------------------------------------------------------------------------


// -------------------------------------------------------------------------

fun createFromYNorthingLatitudeAndXEastingLongitude(
    yNorthingLatitude: Double,
    xEastingLongitude: Double,
    epsgNumber: Int
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        xEastingLongitude,
        yNorthingLatitude,
        createFromEpsgNumber(epsgNumber)
    )
}


fun yx(
    y: Double,
    x: Double,
    epsgNumber: Int
): CrsCoordinate {
    return createFromYNorthingLatitudeAndXEastingLongitude(
        y,
        x,
        epsgNumber
    )
}

fun northingEasting(
    northing: Double,
    easting: Double,
    epsgNumber: Int
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        easting,
        northing,
        epsgNumber
    )
}

fun latLon(
    latitude: Double,
    longitude: Double,
    epsgNumber: Int
): CrsCoordinate {
    return createFromYNorthingLatitudeAndXEastingLongitude(
        latitude,
        longitude,
        epsgNumber
    )
}
// -------------------------------------------------------------------------

// -------------------------------------------------------------------------

fun createFromXEastingLongitudeAndYNorthingLatitude(
    xEastingLongitude: Double,
    yNorthingLatitude: Double,
    crsCode: String
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        xEastingLongitude,
        yNorthingLatitude,
        createFromCrsCode(crsCode)
    )
}


fun xy(
    x: Double,
    y: Double,
    crsCode: String
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        x,
        y,
        crsCode
    )
}

fun eastingNorthing(
    easting: Double,
    northing: Double,
    crsCode: String
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        easting,
        northing,
        crsCode
    )
}

fun lonLat(
    longitude: Double,
    latitude: Double,
    crsCode: String
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        longitude,
        latitude,
        crsCode
    )
}
// -------------------------------------------------------------------------


// -------------------------------------------------------------------------

fun createFromYNorthingLatitudeAndXEastingLongitude(
    yNorthingLatitude: Double,
    xEastingLongitude: Double,
    crsCode: String
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        xEastingLongitude,
        yNorthingLatitude,
        createFromCrsCode(crsCode)
    )
}

fun yx(
    y: Double,
    x: Double,
    crsCode: String
): CrsCoordinate {
    return createFromYNorthingLatitudeAndXEastingLongitude(
        y,
        x,
        crsCode
    )
}

fun northingEasting(
    northing: Double,
    easting: Double,
    crsCode: String
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        easting,
        northing,
        crsCode
    )
}


fun latLon(
    latitude: Double,
    longitude: Double,
    crsCode: String
): CrsCoordinate {
    return createFromYNorthingLatitudeAndXEastingLongitude(
        latitude,
        longitude,
        crsCode
    )
}
// -------------------------------------------------------------------------

// -------------------------------------------------------------------------

fun createFromYNorthingLatitudeAndXEastingLongitude(
    yNorthingLatitude: Double,
    xEastingLongitude: Double,
    crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        xEastingLongitude,
        yNorthingLatitude,
        crsIdentifier
    )
}

fun yx(
    y: Double,
    x: Double,
    crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return createFromYNorthingLatitudeAndXEastingLongitude(
        y,
        x,
        crsIdentifier
    )
}

fun northingEasting(
    northing: Double,
    easting: Double,
    crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return createFromYNorthingLatitudeAndXEastingLongitude(
        northing,
        easting,
        crsIdentifier
    )
}

fun latLon(
    latitude: Double,
    longitude: Double,
    crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return createFromYNorthingLatitudeAndXEastingLongitude(
        latitude,
        longitude,
        crsIdentifier
    )
}
// -------------------------------------------------------------------------

// -------------------------------------------------------------------------

fun createFromXEastingLongitudeAndYNorthingLatitude(
    xEastingLongitude: Double,
    yNorthingLatitude: Double,
    crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return CrsCoordinate(
        xEastingLongitude,
        yNorthingLatitude,
        crsIdentifier
    )
}

fun xy(
    x: Double,
    y: Double,
    crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        x,
        y,
        crsIdentifier
    )
}

fun eastingNorthing(
    easting: Double,
    northing: Double,
    crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return createFromYNorthingLatitudeAndXEastingLongitude(
        northing,
        easting,
        crsIdentifier
    )
}

fun lonLat(
    longitude: Double,
    latitude: Double,
    crsIdentifier: CrsIdentifier
): CrsCoordinate {
    return createFromXEastingLongitudeAndYNorthingLatitude(
        longitude,
        latitude,
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
