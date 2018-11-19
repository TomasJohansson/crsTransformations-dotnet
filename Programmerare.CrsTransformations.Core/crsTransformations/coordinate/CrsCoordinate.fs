namespace com.programmerare.crsTransformations.coordinate
open System
open com.programmerare.crsTransformations.crsIdentifier
(*

// TODO: rewrite comments below for .NET ...

 * An instance of the coordinate aggregates three values:
 * - X / Easting / Longitude  
 * - Y / Northing / Latitude  
 * - CRS (Coordinate Reference System) identifier with the EPSG code which defines the coordinate system  
 * 
 * There are multiple accessors for each of the "x" and "y" values as described below.  
 * Depending on the desired semantic in your context, you may want to use the different accessors like this:  
 *      - x/y for a geocentric or cartesian system  
 *      - longitude/latitude for a geodetic or geographic system  
 *      - easting/northing for a cartographic or projected system  
 *      - xEastingLongitude/yNorthingLatitude for general code handling different types of system
 *
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-adapter-core" project is licensed with MIT.
 * Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
 * Please find more information in the license file at the root directory of each subproject
 * (e.g. the subprojects "crs-transformation-adapter-impl-geotools" , "crs-transformation-adapter-impl-proj4j" and so on)

     * One of the four accessors for the part of the coordinate that represents the east-west/X/Longitude direction. 
     
    val xEastingLongitude: Double,

     * One of the four accessors for the part of the coordinate that represents the east-west/Y/Latitude direction.
     
    val yNorthingLatitude: Double,

     * CRS (Coordinate Reference System) identifier with the EPSG code 
     * which defines the coordinate reference system for the coordinate instance. 
     
    val crsIdentifier: CrsIdentifier

    // TODO: rewrite comments above for .NET ...
*)
type CrsCoordinate private 
    (
    xEastingLongitude: double,
    yNorthingLatitude: double,
    crsIdentifier: CrsIdentifier
    ) =
    // The constructor is private to force client code to use the below factory methods
    // which are named to indicate the order of the longitude and latitude parameters.

    // -----------------------
    // Four getters for X / Easting / Longitude :

    (*
     * One of the four accessors for the part of the coordinate that represents the east-west direction.
     * "X" is typically used for a geocentric or cartesian coordinate reference system.
     *)
    member this.XEastingLongitude = xEastingLongitude

    (*
     * One of the four accessors for the part of the coordinate that represents the east-west direction.
     * "X" is typically used for a geocentric or cartesian coordinate reference system.
     *)
    member this.X = xEastingLongitude

    (*
     * One of the four accessors for the part of the coordinate that represents the east-west direction.
     * "Easting" is typically used for a cartographic or projected coordinate reference system.
     *)
    member this.Easting = xEastingLongitude

    (*
     * One of the four accessors for the part of the coordinate that represents the east-west direction.
     * "Longitude" is typically used for a geodetic or geographic coordinate reference system.
     *)
    member this.Longitude = xEastingLongitude

    // -----------------------
    // Four getters for Y / Northing / Latitude :

    (*
     * One of the four accessors for the part of the coordinate that represents the north-south direction.
     * "Y" is typically used for a geocentric or cartesian coordinate reference system.
     *)
    member this.Y = yNorthingLatitude

    (*
     * One of the four accessors for the part of the coordinate that represents the north-south direction.
     * "Northing" is typically used for a cartographic or projected coordinate reference system.
     *)
    member this.Northing = yNorthingLatitude

    (*
     * One of the four accessors for the part of the coordinate that represents the north-south direction.
     * "Latitude" is typically used for a geodetic or geographic coordinate reference system.
     *)
    member this.Latitude = yNorthingLatitude

    (*
     * One of the four accessors for the part of the coordinate that represents the north-south direction.
     * "Latitude" is typically used for a geodetic or geographic coordinate reference system.
     *)
    member this.YNorthingLatitude = yNorthingLatitude
    //------------------------------------------------------------------

    member this.CrsIdentifier = crsIdentifier

    override this.Equals(obj) =
        // found this below at stack overflow
        match obj with
        | :? CrsCoordinate as c -> (xEastingLongitude, yNorthingLatitude, crsIdentifier) = (c.XEastingLongitude, c.YNorthingLatitude, c.CrsIdentifier)
        | _ -> false

    override this.GetHashCode() =
        //Is 'HashCode.Combine' below not available in .NET Standard ?
        //System.HashCode.Combine(this.XEastingLongitude, this.YNorthingLatitude, this.CrsIdentifier);
        Tuple.Create(this.XEastingLongitude, this.YNorthingLatitude, this.CrsIdentifier).GetHashCode()

    override this.ToString() =
        "Coordinate(xEastingLongitude=$xEastingLongitude, yNorthingLatitude=$yNorthingLatitude, crsIdentifier=$crsIdentifier)"

    //------------------------------------------------------------------

    (*
    * F# factory method with access level "internal".
    * NOT indended for public use.
    * Please instead use the factory class CrsCoordinateFactory.
    *)
    static member _internalXYfactory
        (
            xEastingLongitude: double,
            yNorthingLatitude: double,
            crsIdentifier: CrsIdentifier
        ) =
        CrsCoordinate( // private constructor
            xEastingLongitude,
            yNorthingLatitude,
            crsIdentifier
        )