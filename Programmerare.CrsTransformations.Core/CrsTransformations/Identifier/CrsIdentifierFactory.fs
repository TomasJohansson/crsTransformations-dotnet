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
            invalidArg "epsgNumber" ("EPSG number must not be non-positive but was: " + epsgNumber.ToString())
        epsgNumber

    ///<param name="crsCode">
    ///a string which should begin with "EPSG:4326" 
    ///(although it is case insensitive and it is also acceptable 
    ///with leading white spaces and e.g. " ePsG:4326" but 
    ///it will then be internally canonicalized to "EPSG:4326")
    ///An exception is thrown if an EPSG number is zero or negative,
    ///or if the input string is null or only whitespace.
    ///</param>
    ///<returns>
    ///an instance of CrsIdentifier
    ///</returns>
    let CreateFromCrsCode(crsCode: String) =
        // these two default values will be used, unless the crsCode parameter is an EPSG string
        let mutable epsgNumber = 0
        let mutable isEpsgCode = false
        if isNull crsCode then
            nullArg "crsCode"

        let mutable crsIdentifierCode = crsCode.Trim() // but does not uppercase here (unless EPSG below)
        if crsIdentifierCode.Length = 0 then    
            invalidArg "crsCode" "CRS code must be non-empty"

        if crsIdentifierCode.ToUpper().StartsWith(EPSG_PREFIX_UPPERCASED) then
            let nonEpsgPartOfString = crsIdentifierCode.Substring(LENGTH_OF_EPSG_PREFIX);
            if Int32.TryParse(nonEpsgPartOfString, &epsgNumber) then
                epsgNumber <- getValidEpsgNumberOrThrowArgumentExceptionMessageIfNotValid(epsgNumber)
                isEpsgCode <- true
                crsIdentifierCode <- crsIdentifierCode.ToUpper()
        CrsIdentifier._internalCrsFactory(crsIdentifierCode, isEpsgCode, epsgNumber)

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
            epsgNumber = validatedEpsgNumber
        )