namespace com.programmerare.crsTransformations.crsIdentifier

open System

// TODO: rewrite comments below for .NET ...

(*
 * An instance of this class is representing a coordinate reference system 
 * defined by its so called EPSG code.
 * Instances are created from factory methods with an EPSG code as the parameter.
 * When using Java, the factory methods are available in a class named 'CrsIdentifierFactory'.
 * When using Kotlin, the factory methods are available as package level functions.
 * 
 * CRS = Coordinate Reference System (a.k.a. SRS = Spatial Reference System)  
 * 
 * https://en.wikipedia.org/wiki/Spatial_reference_system
 *
 * EPSG = European Petroleum Survey Group.  
 * 
 * EPSG was a scientific organization which created a widely used database
 * of Earth ellipsoids, geodetic datums, geographic and projected coordinate systems,
 * units of measurement, etc.  
 * 
 * https://en.wikipedia.org/wiki/International_Association_of_Oil_%26_Gas_Producers  
 * 
 * http://www.epsg-registry.org  
 * 
 * http://www.epsg.org  
 * 
 * http://epsg.io/
 *
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-adapter-core" project is licensed with MIT.
 * Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
 * Please find more information in the license file at the root directory of each subproject
 * (e.g. the subprojects "crs-transformation-adapter-impl-geotools" , "crs-transformation-adapter-impl-proj4j" and so on)
 *)
(*
crsCode
* a code representing a coordinate reference system, 
* which should be a string beginning with "EPSG:" and 
* then an EPSG number such as '4326' i.e. the return string 
* should be something like "EPSG:4326" 
*)
(*
isEpsgCode
* @return true if the instance represents an EPSG code and false otherwise.
* 
* Normally the method should always be true since EPSG code are expected 
* by the transform methods, and if this method would return false then 
* there was some problem at the construction of the instance, e.g. 
* trying to create the instance with a string not beginning with "EPSG:"
*)
(*
epsgNumber
* @return an EPSG number, for example 4326 for the frequently used coordinate reference system WGS84.
*)

// TODO: rewrite comments above for .NET ...

[<AllowNullLiteral>] // C# interop
type CrsIdentifier private 
    (
        crsCode: string,
        isEpsgCode: bool,
        epsgNumber: int
    ) =
    member this.CrsCode = crsCode
    member this.EpsgNumber = epsgNumber
    member this.IsEpsgCode = isEpsgCode
    
    static member internal _internalCrsFactory
        (
            crsCode: string,
            isEpsgCode: bool,
            epsgNumber: int
        ) = 
        CrsIdentifier(crsCode, isEpsgCode, epsgNumber)

    override this.Equals(obj) =
        // found similar code as below at stackoverflow
        match obj with
        | :? CrsIdentifier as c -> (crsCode, isEpsgCode, epsgNumber) = (c.CrsCode, c.IsEpsgCode, c.EpsgNumber)
        | _ -> false

    override this.GetHashCode() =
        //Is 'HashCode.Combine' below not available in .NET Standard ?
        //System.HashCode.Combine(CrsCode, IsEpsgCode, EpsgNumber);
        Tuple.Create(this.CrsCode, this.IsEpsgCode, this.EpsgNumber).GetHashCode()

    override this.ToString() =
        "CrsIdentifier(crsCode='" + this.CrsCode + "', isEpsgCode=" + this.IsEpsgCode.ToString() + ", epsgNumber=" + this.EpsgNumber.ToString() + ")"