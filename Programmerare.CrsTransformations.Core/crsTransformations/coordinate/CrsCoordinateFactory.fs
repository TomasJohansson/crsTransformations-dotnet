namespace  com.programmerare.crsTransformations.coordinate

// TODO: rewrite comments below for .NET ...

// The reason for having Coordinate and this CoordinateFactory
// in a package of its own is to avoid "polluting" the base
// package from lots of package level function defined in this file
// when using Kotlin code.
// (when using Java we do not see that problem but rather a class
//   CoordinateFactory with all these function as static method in that class)

//import com.programmerare.crsTransformations.crsIdentifier.CrsIdentifier
//import com.programmerare.crsTransformations.crsIdentifier.createFromCrsCode
//import com.programmerare.crsTransformations.crsIdentifier.createFromEpsgNumber
//import java.lang.IllegalArgumentException

// -------------------------------------------------------------------------

(*
 * This is one of many factory methods for creating a coordinate.
 * The documentation below is generally describing all methods.
 * (and the reason for not putting it at the class level is documented at the bottom below)
 *
 * A coordinate is defined by three values:
 * 
 *      - X / Easting / Longitude
 *      
 *      - Y / Northing / Latitude
 *      
 *      - CRS (Coordinate Reference System) identifier with the EPSG code which defines the coordinate system
 *
 * 
 * Almost all factory methods takes the above three kind of parameters but in different order and with different names.
 * There are also a few methods without the last CRS parameter which then assumes WGS84 as the CRS.
 *
 * The names of the factory methods reflects the expected order of the first two parameters, for example the method
 * 
 *  "yx(y: Double?, x: Double?, epsgNumber: Int?)"
 *  
 *  versus the method
 *  
 *  "xy(x: Double?, y: Double?, epsgNumber: Int?)"
 *
 * The reason to allow null (as above with Double? and Int?) in the Kotlin method signatures is to 
 * provide java clients with better error messages and "IllegalArgumentException" instead of "NullPointerException".
 * 
 * The reason for all the different factory methods is that you may have preferences regarding using
 * short or long method names, and regarding the order of the x/Latitude and y/Longitude values.
 * Also, it may be convenient with different alternatives for the last parameter (e.g. string or integer as explained below).

 * The last parameter which specifies the CRS occurs with three different types:
 * 
 *      - integer: a number which is a so called EPSG code e.g. 4326 for the CRS WGS84
 *      
 *      - string: also an EPSG code but prefixed with "EPSG:" e.g. "EPSG:4326" for the CRS WGS84
 *      
 *      - CrsIdentifier: an object which will be the parameter to the transform method implementations,
 *                      and it provides easy accessors for either the above integer or the above string,
 *                      which is convenient since some adaptee implementations (i.e. third part libraries)
 *                      uses the integer number and some use the string with prefix "EPSG:"
 *                      
 *
 * Regarding the last parameter which specifies the CRS, if integer or string are used,
 * then the CrsIdentifier will be created internally.
 * Therefore, if multiple coordinates are to be created with the same CRS,
 * then you may prefer to use the object version i.e. create it once yourself,
 * instead of inmplicitly letting many instances becmoe created internally.
 * Otherwise you may find it more convenient to use factory method versions
 * with integer or string as the last parameter.
 *
 * From Java code these methods will look like a class 'CrsCoordinateFactory' with public static factory methods.
 * From Kotlin code the methods are package level functions, and each function can be imported as
 * if it would be a class, for example like this:
 *  import com.programmerare.crsTransformations.coordinate.createFromXEastingLongitudeAndYNorthingLatitude
 *
 * Regarding the usage of package level functions like this instead of using a Kotlin object:
 * One advantage is that with a Kotlin object you can actually get the same kind of static
 * method from Java code by using the Kotlin annotation '@JvmStatic'
 * but then you would also see an "INSTANCES" like this:
 *      CoordinateFactory.INSTANCE.createFromXEastingLongitudeAndYNorthingLatitude ...
 *  even though you can ignore it and just use:
 *      CoordinateFactory.createFromXEastingLongitudeAndYNorthingLatitude ...
 *  but with package level function the Java clients will not even see such an "INSTANCE".
 *  One disadvantage with creating the methods as package level functions is 
 *  the problem with generating the documentation as described below.  
 *
 * The reason for not putting the above general documentation at the class level is that
 * the code is written with Kotlin and in the Kotlin language the package level functions
 * documented here are not methods in a class, and while generating the documentation to
 * javadoc it does not show up at the "class" specified in Kotlin with '@file:JvmName("CrsCoordinateFactory")'
 * so therefore putting the general documentation here in one of the methods instead)
 *
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-adapter-core" project is licensed with MIT.
 * Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
 * Please find more information in the license file at the root directory of each subproject
 * (e.g. the subprojects "crs-transformation-adapter-impl-geotools" , "crs-transformation-adapter-impl-proj4j" and so on)
*)

// TODO: rewrite comments above for .NET ...
open com.programmerare.crsTransformations.crsIdentifier

type CrsCoordinateFactory =

    // TODO: try to eliminate frequent usage of the factory 
    // classname in the below F# code e.g. try to use 
    // something like "using static" in C#

    // -------------------------------------------------------------------------
    static member private _CreateXYcrs
        (
            xEastingLongitude: double,
            yNorthingLatitude: double,
            crsIdentifier: CrsIdentifier
        ) : CrsCoordinate =
        if isNull crsIdentifier then
            nullArg "crsIdentifier"
        CrsCoordinate._internalXYfactory(
            xEastingLongitude,
            yNorthingLatitude,
            crsIdentifier
        )

    static member private _CreateXYint
        (
            xEastingLongitude: double,
            yNorthingLatitude: double,
            epsgNumber: int
        ) =
        CrsCoordinateFactory._CreateXYcrs(
            xEastingLongitude,
            yNorthingLatitude,
            CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumber)
        )

    static member private _CreateXYstring
        (
            xEastingLongitude: double,
            yNorthingLatitude: double,
            crsCode: string
        ) =
        CrsCoordinateFactory._CreateXYcrs(
            xEastingLongitude,
            yNorthingLatitude,
            CrsIdentifierFactory.CreateFromCrsCode(crsCode)
        )
    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member CreateFromXEastingLongitudeAndYNorthingLatitude
        (
            xEastingLongitude: double,
            yNorthingLatitude: double,
            crsIdentifier: CrsIdentifier
        ) =
        CrsCoordinateFactory._CreateXYcrs(
            xEastingLongitude,
            yNorthingLatitude,
            crsIdentifier
        )


    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member XY
        (
            x: double,
            y: double,
            crsIdentifier: CrsIdentifier
        ) =
        CrsCoordinateFactory._CreateXYcrs(
            x,
            y,
            crsIdentifier
        )

    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member EastingNorthing
        (
            easting: double,
            northing: double,
            crsIdentifier: CrsIdentifier
        ) =
        CrsCoordinateFactory._CreateXYcrs(
            easting,
            northing,
            crsIdentifier
        )

    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member LonLat
        (
            longitude: double,
            latitude: double,
            crsIdentifier: CrsIdentifier
        ) =
        CrsCoordinateFactory._CreateXYcrs(
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
        CrsCoordinateFactory._CreateXYint(
            xEastingLongitude,
            yNorthingLatitude,
            epsgNumber
        )

    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member XY
        (
            x: double,
            y: double,
            epsgNumber: int
        ) =
        CrsCoordinateFactory._CreateXYint(
            x,
            y,
            epsgNumber
        )


    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member EastingNorthing
        (
            easting: double,
            northing: double,
            epsgNumber: int
        ) =
        CrsCoordinateFactory._CreateXYint(
            easting,
            northing,
            epsgNumber
        )


    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member LonLat
        (
            longitude: double,
            latitude: double,
            epsgNumber: int
        ) =
        CrsCoordinateFactory._CreateXYint(
            longitude,
            latitude,
            epsgNumber
        )
// -------------------------------------------------------------------------


// -------------------------------------------------------------------------

    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member CreateFromYNorthingLatitudeAndXEastingLongitude
        (
            yNorthingLatitude: double,
            xEastingLongitude: double,
            epsgNumber: int
        ) =
        CrsCoordinateFactory._CreateXYint(
            xEastingLongitude,
            yNorthingLatitude,
            epsgNumber
        )


    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member YX
        (
            y: double,
            x: double,
            epsgNumber: int
        ) =
        CrsCoordinateFactory._CreateXYint(
            x,
            y,
            epsgNumber
        )

    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member NorthingEasting
        (
            northing: double,
            easting: double,
            epsgNumber: int
        ) =
        CrsCoordinateFactory._CreateXYint(
            easting,
            northing,
            epsgNumber
        )


    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member LatLon
        (
            latitude: double,
            longitude: double,
            epsgNumber: int
        ) =
        CrsCoordinateFactory._CreateXYint(
            longitude,
            latitude,
            epsgNumber
        )
// -------------------------------------------------------------------------

// -------------------------------------------------------------------------

    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member CreateFromXEastingLongitudeAndYNorthingLatitude
        (
            xEastingLongitude: double,
            yNorthingLatitude: double,
            crsCode: string
        ) =
        CrsCoordinateFactory._CreateXYstring(
            xEastingLongitude,
            yNorthingLatitude,
            crsCode
        )


    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member XY
        (
            x: double,
            y: double,
            crsCode: string
        ) =
        CrsCoordinateFactory._CreateXYstring(
            x,
            y,
            crsCode
        )


    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member EastingNorthing
        (
            easting: double,
            northing: double,
            crsCode: string
        ) =
        CrsCoordinateFactory._CreateXYstring(
            easting,
            northing,
            crsCode
        )



    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member LonLat
        (
            longitude: double,
            latitude: double,
            crsCode: string
        ) =
        CrsCoordinateFactory._CreateXYstring(
            longitude,
            latitude,
            crsCode
        )


// -------------------------------------------------------------------------


// -------------------------------------------------------------------------

    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member CreateFromYNorthingLatitudeAndXEastingLongitude
        (
            yNorthingLatitude: double,
            xEastingLongitude: double,
            crsCode: string
        ) =
        CrsCoordinateFactory._CreateXYstring(
            xEastingLongitude,
            yNorthingLatitude,
            crsCode
        )

    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member YX
        (
            y: double,
            x: double,
            crsCode: string
        ) =
        CrsCoordinateFactory._CreateXYstring(
            x,
            y,
            crsCode
        )


    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member NorthingEasting
        (
            northing: double,
            easting: double,
            crsCode: string
        ) =
        CrsCoordinateFactory._CreateXYstring(
            easting,
            northing,
            crsCode
        )



    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member LatLon
        (
            latitude: double,
            longitude: double,
            crsCode: string
        ) =
        CrsCoordinateFactory._CreateXYstring(
            longitude,
            latitude,
            crsCode
        )

// -------------------------------------------------------------------------

// -------------------------------------------------------------------------

    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member CreateFromYNorthingLatitudeAndXEastingLongitude
        (
            yNorthingLatitude: double,
            xEastingLongitude: double,
            crsIdentifier: CrsIdentifier
        ) =
        CrsCoordinateFactory._CreateXYcrs(
            xEastingLongitude,
            yNorthingLatitude,
            crsIdentifier
        )


    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member YX
        (
            y: double,
            x: double,
            crsIdentifier: CrsIdentifier
        ) =
        CrsCoordinateFactory._CreateXYcrs(
            x,
            y,
            crsIdentifier
        )


    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member NorthingEasting
        (
            northing: double,
            easting: double,
            crsIdentifier: CrsIdentifier
        ) =
        CrsCoordinateFactory._CreateXYcrs(
            easting,
            northing,
            crsIdentifier
        )


    (*
     * See the documentation at method "createFromXEastingLongitudeAndYNorthingLatitude"
     *)
    static member LatLon
        (
            latitude: double,
            longitude: double,
            crsIdentifier: CrsIdentifier
        ) =
        CrsCoordinateFactory._CreateXYcrs(
            longitude,
            latitude,
            crsIdentifier
        )
// -------------------------------------------------------------------------


    (*
     * WGS84 is probably the most common coordinate reference system,
     * the coordinates typically used with GPS.
     * Therefore it is default for the factory methods not specifying
     * the coordinate reference system.
     *)
    static member private COORDINATE_REFERENCE_SYSTEM_WGS84 = CrsIdentifierFactory.CreateFromEpsgNumber(4326)

    (*
     * The "GPS coordinate system" WGS84 is assumed when using this factory method.
     *)
    static member CreateFromLongitudeLatitude
        (
            longitude: double,
            latitude: double
        ) = 
        CrsCoordinateFactory._CreateXYcrs(
            longitude,
            latitude,
            CrsCoordinateFactory.COORDINATE_REFERENCE_SYSTEM_WGS84
        )

    (*
     * The "GPS coordinate system" WGS84 is assumed when using this factory method.
     *)
    static member LonLat
        (
            longitude: double,
            latitude: double
        ) =
        CrsCoordinateFactory._CreateXYcrs(
            longitude,
            latitude,
            CrsCoordinateFactory.COORDINATE_REFERENCE_SYSTEM_WGS84
        )


    (*
     * The "GPS coordinate system" WGS84 is assumed when using this factory method.
     *)
    static member CreateFromLatitudeLongitude
        (
            latitude: double,
            longitude: double
        ) =
        CrsCoordinateFactory._CreateXYcrs(
            longitude,
            latitude,
            CrsCoordinateFactory.COORDINATE_REFERENCE_SYSTEM_WGS84
        )


    (*
     * The "GPS coordinate system" WGS84 is assumed when using this factory method.
     *)
    static member LatLon
        (
            latitude: double,
            longitude: double
        ) =
        CrsCoordinateFactory._CreateXYcrs(
            longitude,
            latitude,
            CrsCoordinateFactory.COORDINATE_REFERENCE_SYSTEM_WGS84
        )

// -------------------------------------------------------------------------
