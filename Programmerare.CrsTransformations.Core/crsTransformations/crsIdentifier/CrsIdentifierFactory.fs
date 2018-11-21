namespace Programmerare.CrsTransformations.Identifier

open System

// TODO: rewrite comments below for .NET ...

(*
 * From Java code this will look like a class 'CrsIdentifierFactory'
 * with public static factory methods.
 * The Java class name: Programmerare.CrsTransformations.Identifier.CrsIdentifierFactory  
 * 
 * From Kotlin code the methods are available as package level functions
 * and each function can be imported as if it would be a class, for example like this:  
 *  import Programmerare.CrsTransformations.Identifier.createFromEpsgNumber
 *
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-adapter-core" project is licensed with MIT.
 * Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
 * Please find more information in the license file at the root directory of each subproject
 * (e.g. the subprojects "crs-transformation-adapter-impl-geotools" , "crs-transformation-adapter-impl-proj4j" and so on)
 *)


// The reason for having CrsIdentifier and this CrsIdentifierFactory
// in a package of its own is to avoid "polluting" the base
// package from lots of package level function defined in this file
// when using Kotlin code.
// (when using Java we do not see that problem but rather a class
//   CrsIdentifierFactory with all these function as static method in that class)

// TODO: rewrite comments above for .NET ...

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

    (*
     * TODO: rewrite comments below for .NET ...
     * @param crsCode a string which should begin with "EPSG:4326" 
     *  (although it is case insensitive and it is also acceptable 
     *   with leading white spaces and e.g. " ePsG:4326" but 
     *   it will then be internally canonicalized to "EPSG:4326")
     *   An exception is thrown if an EPSG number is zero or negative,
     *   or if the input string is null or only whitespace.
     *)
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

    (*
     * TODO: rewrite comments below for .NET ...
     * Creates a CrsIdentifier from a positive integer.
     * The only validation constraints are that the integer must be positive (and not null).
     * An exception is thrown if the input number is null or zero or negative.
     * The reason to allow null in the Kotlin method signature is to provide java clients
     * with better error messages and "IllegalArgumentException" instead of "NullPointerException" 
     * @param epsgNumber an EPSG number, 
     *      for example 4326 for the frequently used coordinate reference system WGS84. 
     *)
    let CreateFromEpsgNumber(epsgNumber: int) =
        let validatedEpsgNumber = getValidEpsgNumberOrThrowArgumentExceptionMessageIfNotValid(epsgNumber)
        CrsIdentifier._internalCrsFactory(
            crsCode = EPSG_PREFIX_UPPERCASED + validatedEpsgNumber.ToString(),
            isEpsgCode = true,
            epsgNumber = validatedEpsgNumber
        )