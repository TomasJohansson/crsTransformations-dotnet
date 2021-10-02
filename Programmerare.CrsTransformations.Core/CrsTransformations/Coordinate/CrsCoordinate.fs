namespace Programmerare.CrsTransformations.Coordinate

open System
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
///An instance of this type 'CrsCoordinate' aggregates three values:
/// - X / Easting / Longitude  
/// - Y / Northing / Latitude  
/// - CRS (Coordinate Reference System) identifier with the EPSG code which defines the coordinate system  
///</para>
///<para/>
///<para>
///There are multiple accessors for each of the "x" and "y" values as described below.  
///Depending on the desired semantic in your context, you may want to use the different accessors like this:  
/// - x/y for a geocentric or cartesian system  
/// - longitude/latitude for a geodetic or geographic system  
/// - easting/northing for a cartographic or projected system  
/// - xEastingLongitude/yNorthingLatitude for general code handling different types of system
///</para>
///<para/>
///<para>
///There are three parameters for the constructor:
/// - XEastingLongitude
///     (the name is also used as one of the four accessors for the part of the coordinate that represents the east-west/X/Longitude direction)
/// - YNorthingLatitude
///     (the name is also used as one of the four accessors for the part of the coordinate that represents the north-south/Y/Latitude direction)
/// - CRS (Coordinate Reference System) identifier with the EPSG code 
///     which defines the coordinate reference system for the coordinate instance. 
///</para>
///</summary>
[<AllowNullLiteral>] // C# interop
type CrsCoordinate private 
    (
    xEastingLongitude: double,
    yNorthingLatitude: double,
    crsIdentifier: CrsIdentifier
    ) =
    // The above constructor is private to force 
    // the client code to use a factory method.

    // -----------------------
    // Four getters for X / Easting / Longitude :

    ///<value>
    ///One of the four accessors for the part of the coordinate that represents the east-west direction.
    ///</value>
    member this.XEastingLongitude = xEastingLongitude

    ///<value>
    ///One of the four accessors for the part of the coordinate that represents the east-west direction.
    ///"X" is typically used for a geocentric or cartesian coordinate reference system.
    ///</value>
    member this.X = xEastingLongitude

    ///<value>
    ///One of the four accessors for the part of the coordinate that represents the east-west direction.
    ///"Easting" is typically used for a cartographic or projected coordinate reference system.
    ///</value>
    member this.Easting = xEastingLongitude

    ///<value>
    ///One of the four accessors for the part of the coordinate that represents the east-west direction.
    ///"Longitude" is typically used for a geodetic or geographic coordinate reference system.
    ///</value>
    member this.Longitude = xEastingLongitude

    // -----------------------
    // Four getters for Y / Northing / Latitude :

    ///<value>
    ///One of the four accessors for the part of the coordinate that represents the north-south direction.
    ///"Y" is typically used for a geocentric or cartesian coordinate reference system.
    ///</value>
    member this.Y = yNorthingLatitude

    ///<value>
    ///One of the four accessors for the part of the coordinate that represents the north-south direction.
    ///"Northing" is typically used for a cartographic or projected coordinate reference system.
    ///</value>
    member this.Northing = yNorthingLatitude

    ///<value>
    ///One of the four accessors for the part of the coordinate that represents the north-south direction.
    ///"Latitude" is typically used for a geodetic or geographic coordinate reference system.
    ///</value>
    member this.Latitude = yNorthingLatitude

    ///<value>
    ///One of the four accessors for the part of the coordinate that represents the north-south direction.
    ///</value>
    member this.YNorthingLatitude = yNorthingLatitude
    //------------------------------------------------------------------

    member this.CrsIdentifier = crsIdentifier

    override this.Equals(obj) =
        match obj with
        | :? CrsCoordinate as c -> (xEastingLongitude, yNorthingLatitude, crsIdentifier) = (c.XEastingLongitude, c.YNorthingLatitude, c.CrsIdentifier)
        | _ -> false

    override this.GetHashCode() =
        //Is 'HashCode.Combine' below not available in .NET Standard ?
        //System.HashCode.Combine(this.XEastingLongitude, this.YNorthingLatitude, this.CrsIdentifier);
        Tuple.Create(this.XEastingLongitude, this.YNorthingLatitude, this.CrsIdentifier).GetHashCode()

    override this.ToString() =
        "CrsCoordinate(xEastingLongitude=" + this.X.ToString() + ", yNorthingLatitude=" + this.Y.ToString() + ", crsIdentifier=" + this.CrsIdentifier.ToString() + ")"

    //------------------------------------------------------------------

    ///<summary>
    ///F# factory method with access level "internal".
    ///NOT indended for public use.
    ///Please instead use the factory class CrsCoordinateFactory.
    ///</summary>
    static member internal _internalXYfactory
        (
            xEastingLongitude: double,
            yNorthingLatitude: double,
            crsIdentifier: CrsIdentifier
        ) =
        if isNull crsIdentifier then
            nullArg "crsIdentifier"
        let mutable isValidX = true
        let mutable isValidY = true
        if(Double.IsNaN(xEastingLongitude)) then isValidX <- false
        elif(Double.IsNaN(yNorthingLatitude)) then isValidY <- false
        elif(Double.IsInfinity(xEastingLongitude)) then isValidX <- false
        elif(Double.IsInfinity(yNorthingLatitude)) then isValidY <- false
        //above IsInfinity checks both IsNegativeInfinity and IsPositiveInfinity
        if not(isValidX) then
            invalidArg "xEastingLongitude" ("Coordinate not valid: " + xEastingLongitude.ToString())
        if not(isValidY) then
            invalidArg "yNorthingLatitude" ("Coordinate not valid: " + yNorthingLatitude.ToString())
        CrsCoordinate( // private constructor
            xEastingLongitude,
            yNorthingLatitude,
            crsIdentifier
        )