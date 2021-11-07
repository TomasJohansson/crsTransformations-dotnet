namespace Programmerare.CrsTransformations.Identifier

open System

(*
Copyright (c) Tomas Johansson , http://programmerare.com
The code in the "Core" project is licensed with MIT.
Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
Please find more information in the license file at the root directory of each subproject
(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")
*)

///<summary>
///Factory module/class for creating instances of CrsIdentifier.
///From C# code this module will look like a static class 'CrsIdentifierFactory'
///with public static factory methods.
///The C# class name: Programmerare.CrsTransformations.Identifier.CrsIdentifierFactory
///</summary>
module CrsIdentifierFactory =

    // The crsCode string will become trimmed, and if it is
    // "epsg" (or e.g. something like "ePsG") then it will be uppercased i.e. "EPSG"
    [<Literal>] 
    let private EPSG_PREFIX_UPPERCASED = "EPSG:"

    //[<Literal>] 
    let private LENGTH_OF_EPSG_PREFIX = EPSG_PREFIX_UPPERCASED.Length

    let private getValidEpsgNumberOrThrowArgumentExceptionMessageIfNotValid epsgNumber = 
        if epsgNumber < 0 then    
            invalidArg (nameof epsgNumber) ("EPSG number must not be non-positive but was: " + epsgNumber.ToString())
        epsgNumber

    ///<param name="crsCode">
    ///a string which must begin with "EPSG:" and then be followed by a number, e.g. "EPSG:4326"
    ///(although "EPSG" is case insensitive and it is also acceptable 
    ///with leading white spaces and e.g. " ePsG:4326" but 
    ///it will then be internally canonicalized to "EPSG:4326")
    ///An exception is thrown if an EPSG number is zero or negative,
    ///or if the input string is null or only whitespace.
    ///Also thrown if not following the specified format in som other way.
    ///</param>
    ///<returns>
    ///an instance of CrsIdentifier
    ///</returns>
    let CreateFromCrsCode(crsCode: String) =
        // these two default values will be used, unless the crsCode parameter is an EPSG string
        let mutable epsgNumber = 0
        let mutable isEpsgCode = false
        if isNull crsCode then
            nullArg (nameof crsCode)

        let crsIdentifierCode = crsCode.Trim().ToUpper()
        if crsIdentifierCode.Length = 0 then    
            invalidArg (nameof crsCode) "CRS code must be non-empty"

        if crsIdentifierCode.StartsWith(EPSG_PREFIX_UPPERCASED) then
            let nonEpsgPartOfString = crsIdentifierCode.Substring(LENGTH_OF_EPSG_PREFIX);
            if Int32.TryParse(nonEpsgPartOfString, &epsgNumber) then
                epsgNumber <- getValidEpsgNumberOrThrowArgumentExceptionMessageIfNotValid(epsgNumber)
                isEpsgCode <- true
        else
            // Note: Below is a breaking change, so maybe should bump the major version for the next release
            invalidArg (nameof crsCode) "CRS code must be an EPSG code i.e. start with the string 'EPSG:'"
        CrsIdentifier._internalCrsFactory(crsIdentifierCode, isEpsgCode, epsgNumber, "")

    ///<summary>
    ///Creates a CrsIdentifier from a positive integer.
    ///The only validation constraint is that the integer must be positive.
    ///An exception is thrown if the input number is negative.
    ///</summary>
    ///<param name="epsgNumber">
    ///an EPSG number, 
    ///for example 4326 for the frequently used coordinate reference system WGS84. 
    ///</param>
    ///<returns>
    ///an instance of CrsIdentifier
    ///</returns>
    let CreateFromEpsgNumber(epsgNumber: int) =
        let validatedEpsgNumber = getValidEpsgNumberOrThrowArgumentExceptionMessageIfNotValid(epsgNumber)
        CrsIdentifier._internalCrsFactory(
            crsCode = EPSG_PREFIX_UPPERCASED + validatedEpsgNumber.ToString(),
            isEpsgCode = true,
            epsgNumber = validatedEpsgNumber,
            wellKnownTextCrs = ""
        )

    ///<summary>
    ///Creates a CrsIdentifier from a WKT (Well-Known-Text) CRS string.
    ///https://en.wikipedia.org/wiki/Well-known_text_representation_of_coordinate_reference_systems#ESRI_vs_OGC
    ///Quote from above URL: "Well-known text representation of coordinate reference systems (WKT or WKT-CRS) is a text markup language for representing spatial reference systems and transformations between spatial reference systems."
    ///The only validation constraints are that the string must not be null and also not only containing white-space.
    ///All other strings are accepted by this library and it will be passed on to the implementation, and please note 
    ///that there are different versions of WKT-CRS (OGC vs ESRI, see the above wikipedia link)
    ///</summary>
    ///<param name="wellKnownTextCrs">
    ///Below is an example string for the CRS EPSG 3006 (SWEREF99 TM):
    ///PROJCS["SWEREF99 TM",GEOGCS["SWEREF99",DATUM["SWEREF99",SPHEROID["GRS 1980",6378137,298.257222101,AUTHORITY["EPSG","7019"]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY["EPSG","6619"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4619"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",15],PARAMETER["scale_factor",0.9996],PARAMETER["false_easting",500000],PARAMETER["false_northing",0],UNIT["metre",1,AUTHORITY["EPSG","9001"]],AUTHORITY["EPSG","3006"]]
    ///</param>
    ///<returns>
    ///an instance of CrsIdentifier
    ///</returns>
    let CreateFromWktCrs(wellKnownTextCrs: string) =
        if isNull wellKnownTextCrs then
            nullArg (nameof wellKnownTextCrs)
        if wellKnownTextCrs.Trim().Equals("") then
            invalidArg (nameof wellKnownTextCrs) ("Wkt-Crs must not be empty nor only contain white space characters")
            
        CrsIdentifier._internalCrsFactory(
            crsCode = "",
            isEpsgCode = false,
            epsgNumber = 0,
            wellKnownTextCrs = wellKnownTextCrs
        )