namespace Programmerare.CrsTransformations.Coordinate

open Programmerare.CrsTransformations.Identifier

(*
Copyright (c) Tomas Johansson , http://programmerare.com
The code in the "Core" project is licensed with MIT.
Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
Please find more information in the license file at the root directory of each subproject
(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")
*)

///<summary>
///<para>
///Factory for creating instances of CrsCoordinate.
///A coordinate is defined by three values:
/// - X / Easting / Longitude
/// - Y / Northing / Latitude
/// - CRS (Coordinate Reference System) identifier with the EPSG code which defines the coordinate system
///Almost all factory methods take the above three kind of parameters but in different order and with different names.
///There are also a few methods without the last CRS parameter which then assumes WGS84 as the CRS.
///</para>
///<para/>
///<para>
///The names of the factory methods reflect the expected order of the first two parameters, for example the method
/// "YX(y: double, x: double, epsgNumber: int)"
/// versus the method
/// "XY(x: double, y: double, epsgNumber: int)"
///</para>
///<para/>
///<para>
///The reason for all the different factory methods is that you may have preferences regarding using
///short or long method names, and regarding the order of the x/Latitude and y/Longitude values.
///Also, it may be convenient with different alternatives for the last parameter (e.g. string or integer as explained below).
///The last parameter which specifies the CRS occurs with three different types:
/// - integer: a number which is a so called EPSG code e.g. 4326 for the CRS WGS84
/// - string: also an EPSG code but prefixed with "EPSG:" e.g. "EPSG:4326" for the CRS WGS84
/// - CrsIdentifier: an object which will be the parameter to the transform method implementations,
///     and it provides accessors for either the above integer or the above string,
///     which is convenient since some adaptee implementations (i.e. third part libraries)
///     use the integer number and some use the string with prefix "EPSG:"
///</para>
///<para/>
///<para>
///Regarding the last parameter which specifies the CRS, if integer or string are used,
///then the CrsIdentifier will be created internally.
///Therefore, if multiple coordinates are to be created with the same CRS,
///then you may prefer to use the object version i.e. create it once yourself,
///instead of inmplicitly letting many instances becmoe created internally.
///Otherwise you may find it more convenient to use factory method versions
///with integer or string as the last parameter.
///</para>
///</summary>
[<AbstractClass; Sealed>]
type CrsCoordinateFactory private() =

    ///<summary>
    ///WGS84 is probably the most common coordinate reference system,
    ///the coordinates typically used with GPS.
    ///Therefore it is default for those factory methods 
    ///which do not specify the coordinate reference system.
    ///</summary>
    static let COORDINATE_REFERENCE_SYSTEM_WGS84 = CrsIdentifierFactory.CreateFromEpsgNumber(4326)

    // -------------------------------------------------------------------------
    static let _CreateXYcrs
        (
            xEastingLongitude: double,
            yNorthingLatitude: double,
            crsIdentifier: CrsIdentifier
        ) : CrsCoordinate =
        CrsCoordinate._internalXYfactory(
            xEastingLongitude,
            yNorthingLatitude,
            crsIdentifier
        )

    static let _CreateXYint
        (
            xEastingLongitude: double,
            yNorthingLatitude: double,
            epsgNumber: int
        ) =
        _CreateXYcrs(
            xEastingLongitude,
            yNorthingLatitude,
            CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumber)
        )

    static let _CreateXYstring
        (
            xEastingLongitude: double,
            yNorthingLatitude: double,
            crsCode: string
        ) =
        _CreateXYcrs(
            xEastingLongitude,
            yNorthingLatitude,
            CrsIdentifierFactory.CreateFromCrsCode(crsCode)
        )

    static member CreateFromXEastingLongitudeAndYNorthingLatitude
        (
            xEastingLongitude: double,
            yNorthingLatitude: double,
            crsIdentifier: CrsIdentifier
        ) =
        _CreateXYcrs(
            xEastingLongitude,
            yNorthingLatitude,
            crsIdentifier
        )

    static member XY
        (
            x: double,
            y: double,
            crsIdentifier: CrsIdentifier
        ) =
        _CreateXYcrs(
            x,
            y,
            crsIdentifier
        )

    static member EastingNorthing
        (
            easting: double,
            northing: double,
            crsIdentifier: CrsIdentifier
        ) =
        _CreateXYcrs(
            easting,
            northing,
            crsIdentifier
        )

    static member LonLat
        (
            longitude: double,
            latitude: double,
            crsIdentifier: CrsIdentifier
        ) =
        _CreateXYcrs(
            longitude,
            latitude,
            crsIdentifier
        )

    // -------------------------------------------------------------------------

    static member CreateFromXEastingLongitudeAndYNorthingLatitude
        (
            xEastingLongitude: double,
            yNorthingLatitude: double,
            epsgNumber: int
        ) =
        _CreateXYint(
            xEastingLongitude,
            yNorthingLatitude,
            epsgNumber
        )

    static member XY
        (
            x: double,
            y: double,
            epsgNumber: int
        ) =
        _CreateXYint(
            x,
            y,
            epsgNumber
        )


    static member EastingNorthing
        (
            easting: double,
            northing: double,
            epsgNumber: int
        ) =
        _CreateXYint(
            easting,
            northing,
            epsgNumber
        )

    static member LonLat
        (
            longitude: double,
            latitude: double,
            epsgNumber: int
        ) =
        _CreateXYint(
            longitude,
            latitude,
            epsgNumber
        )
// -------------------------------------------------------------------------

    static member CreateFromYNorthingLatitudeAndXEastingLongitude
        (
            yNorthingLatitude: double,
            xEastingLongitude: double,
            epsgNumber: int
        ) =
        _CreateXYint(
            xEastingLongitude,
            yNorthingLatitude,
            epsgNumber
        )

    static member YX
        (
            y: double,
            x: double,
            epsgNumber: int
        ) =
        _CreateXYint(
            x,
            y,
            epsgNumber
        )

    static member NorthingEasting
        (
            northing: double,
            easting: double,
            epsgNumber: int
        ) =
        _CreateXYint(
            easting,
            northing,
            epsgNumber
        )

    static member LatLon
        (
            latitude: double,
            longitude: double,
            epsgNumber: int
        ) =
        _CreateXYint(
            longitude,
            latitude,
            epsgNumber
        )
// -------------------------------------------------------------------------

    static member CreateFromXEastingLongitudeAndYNorthingLatitude
        (
            xEastingLongitude: double,
            yNorthingLatitude: double,
            crsCode: string
        ) =
        _CreateXYstring(
            xEastingLongitude,
            yNorthingLatitude,
            crsCode
        )


    static member XY
        (
            x: double,
            y: double,
            crsCode: string
        ) =
        _CreateXYstring(
            x,
            y,
            crsCode
        )

    static member EastingNorthing
        (
            easting: double,
            northing: double,
            crsCode: string
        ) =
        _CreateXYstring(
            easting,
            northing,
            crsCode
        )



    static member LonLat
        (
            longitude: double,
            latitude: double,
            crsCode: string
        ) =
        _CreateXYstring(
            longitude,
            latitude,
            crsCode
        )


// -------------------------------------------------------------------------

    static member CreateFromYNorthingLatitudeAndXEastingLongitude
        (
            yNorthingLatitude: double,
            xEastingLongitude: double,
            crsCode: string
        ) =
        _CreateXYstring(
            xEastingLongitude,
            yNorthingLatitude,
            crsCode
        )

    static member YX
        (
            y: double,
            x: double,
            crsCode: string
        ) =
        _CreateXYstring(
            x,
            y,
            crsCode
        )

    static member NorthingEasting
        (
            northing: double,
            easting: double,
            crsCode: string
        ) =
        _CreateXYstring(
            easting,
            northing,
            crsCode
        )

    static member LatLon
        (
            latitude: double,
            longitude: double,
            crsCode: string
        ) =
        _CreateXYstring(
            longitude,
            latitude,
            crsCode
        )

// -------------------------------------------------------------------------

    static member CreateFromYNorthingLatitudeAndXEastingLongitude
        (
            yNorthingLatitude: double,
            xEastingLongitude: double,
            crsIdentifier: CrsIdentifier
        ) =
        _CreateXYcrs(
            xEastingLongitude,
            yNorthingLatitude,
            crsIdentifier
        )

    static member YX
        (
            y: double,
            x: double,
            crsIdentifier: CrsIdentifier
        ) =
        _CreateXYcrs(
            x,
            y,
            crsIdentifier
        )

    static member NorthingEasting
        (
            northing: double,
            easting: double,
            crsIdentifier: CrsIdentifier
        ) =
        _CreateXYcrs(
            easting,
            northing,
            crsIdentifier
        )


    static member LatLon
        (
            latitude: double,
            longitude: double,
            crsIdentifier: CrsIdentifier
        ) =
        _CreateXYcrs(
            longitude,
            latitude,
            crsIdentifier
        )
// -------------------------------------------------------------------------

    ///<summary>
    ///The "GPS coordinate system" WGS84 is assumed when using this factory method.
    ///</summary>
    static member CreateFromLongitudeLatitude
        (
            longitude: double,
            latitude: double
        ) = 
        _CreateXYcrs(
            longitude,
            latitude,
            COORDINATE_REFERENCE_SYSTEM_WGS84
        )

    ///<summary>
    ///The "GPS coordinate system" WGS84 is assumed when using this factory method.
    ///</summary>
    static member LonLat
        (
            longitude: double,
            latitude: double
        ) =
        _CreateXYcrs(
            longitude,
            latitude,
            COORDINATE_REFERENCE_SYSTEM_WGS84
        )


    ///<summary>
    ///The "GPS coordinate system" WGS84 is assumed when using this factory method.
    ///</summary>
    static member CreateFromLatitudeLongitude
        (
            latitude: double,
            longitude: double
        ) =
        _CreateXYcrs(
            longitude,
            latitude,
            COORDINATE_REFERENCE_SYSTEM_WGS84
        )


    ///<summary>
    ///The "GPS coordinate system" WGS84 is assumed when using this factory method.
    ///</summary>
    static member LatLon
        (
            latitude: double,
            longitude: double
        ) =
        _CreateXYcrs(
            longitude,
            latitude,
            COORDINATE_REFERENCE_SYSTEM_WGS84
        )
// -------------------------------------------------------------------------