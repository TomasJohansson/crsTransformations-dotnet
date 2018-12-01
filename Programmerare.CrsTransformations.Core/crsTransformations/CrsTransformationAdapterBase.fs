namespace Programmerare.CrsTransformations
open System
open System.IO
open System.Text.RegularExpressions
open System.Collections.Generic
open Programmerare.CrsTransformations.Coordinate
open Programmerare.CrsTransformations.Identifier
open System.Diagnostics

(*
Copyright (c) Tomas Johansson , http://programmerare.com
The code in the "Core" project is licensed with MIT.
Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
Please find more information in the license file at the root directory of each subproject
(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")

There are TWO types in this file:
    - FileInfoVersion
    - CrsTransformationAdapterBase
*)

// ----------------------------------------------------
// This small class is used as return type for a method 
// with the only purpose to be used in test code for detecting 
// upgraded version (of some NuGet package or an adaptee library)
// i.e. to help remembering to update the enum value 
// defining which adaptee library version is used
// TODO maybe use [<AllowNullLiteral>] at FileInfoVersion instead for C# interoperability
type FileInfoVersion
    (
        fileName: string,
        fileSize: int64,
        version: string
    ) = 
    class
        member this.FileName = fileName
        member this.FileSize = fileSize
        member this.Version = version
        // TODO maybe use [<AllowNullLiteral>] at FileInfoVersion declaration 
        // i.e. use null instead of the below values 
        // for C# interoperability, i.e. like this:
        // let defaultFileInfoVersion: FileInfoVersion = null
        static member DefaultFileInfoVersion = FileInfoVersion("", -1L, "")

        (*
         Helper method intended to be used from implementing adapters 
         when implementing a method that should return the name
         of a DLL file (and the version information extracted from the path) 
         file belonging to an adaptee library.
             This helper method is NOT intended for
             client code.
             Therefore it is named with "_" as prefix.
        *)
        static member GetFileInfoVersionHelper
            (
                someTypeInTheThidPartAdapteeLibrary: Type
            ) =
            let assembly = someTypeInTheThidPartAdapteeLibrary.Assembly
            let codeBase = assembly.CodeBase
            let file = FileInfo(assembly.Location)
            // AssemblyQualifiedName is something like this: 
            // "MightyLittleGeodesy.Positions.RT90Position, MightyLittleGeodesy, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
            // but the version number 1.0.0.0 is not what I want which is 1.0.1 
            // which can be extracted from the path below
            // the code base will be some path like:
            // "...nuget/packages/mightylittlegeodesy/1.0.1/lib/net45/MightyLittleGeodesy.dll"
            // version between some slashes in the below regexp: 
            // e.g. "2.0.0-rc1" or "1.0.1"
            let inputString = codeBase.ToLower().Replace('\\','/')
            // printfn "GetFileInfoVersionHelper inputString: %s" inputString
            let regExp = new Regex("^.*\\/(.{0,10}?[\\d\\.]{3,9}.{0,10}?)\\/.*\\/(.+)$")
            let regExpMatch = regExp.Match(inputString)
            if regExpMatch.Success then
                FileInfoVersion
                    (
                    regExpMatch.Groups.[2].Value,
                    file.Length,
                    regExpMatch.Groups.[1].Value
                    )
            else
                FileInfoVersion.DefaultFileInfoVersion
    end
// ----------------------------------------------------
(*
 * The base class of the adapter interface implementing most of the 
 * transform methods as "final" i.e. NOT being declared as overrideable by using "abstract".
 *)
[<AbstractClass>]
type CrsTransformationAdapterBase
    (
        functionReturningFileInfoVersion: unit -> FileInfoVersion,
        transformToCoordinateStrategy : CrsCoordinate * CrsIdentifier -> CrsCoordinate ,
        transformStrategy : CrsCoordinate * CrsIdentifier -> CrsTransformationResult
    ) =
    class

        let TrowExceptionIfCoordinateIsNull(inputCoordinate) : unit = 
            if isNull inputCoordinate then
                nullArg "inputCoordinate"
            // The above would cause this error:
            // "Value cannot be null. Parameter name: inputCoordinate"
            // instead of the following which otherwise might occur later:
            // "Object reference not set to an instance of an object"

        abstract member AdapteeType : CrsTransformationAdapteeType
        default this.AdapteeType = CrsTransformationAdapteeType.UNSPECIFIED

        abstract member IsComposite : bool
        default this.IsComposite = raise (System.NotImplementedException())

        abstract member LongNameOfImplementation : string
        default this.LongNameOfImplementation = raise (System.NotImplementedException())

        abstract member ShortNameOfImplementation : string
        default this.ShortNameOfImplementation = raise (System.NotImplementedException())

        abstract member GetTransformationAdapterChildren : unit -> IList<ICrsTransformationAdapter>
        default this.GetTransformationAdapterChildren() = raise (System.NotImplementedException())

        // The purpose of the method below is to use it in test code
        // for detecting upgrades to a new version (and then update the above method returned enum value)
        // Future failure will be a reminder to update a corresponding enum value.
        (*
            * This helper method is NOT intended for
            * client code but only for test code purposes.
             Therefore it is named with "_" as prefix.
            * 
            * It should be overridden by subclasses.
            * returns a default value should be returned by the composites (i.e. they should not override).
            *      
            *      The 'leaf' adapter implementations should return the
            *      name of the DLL file and version number retrieved through the file path.
            *      
            *      The reason is that the DLL files (retrieved through NuGet)
            *      includes the version name it the path and can be asserted in test code
            *      to help remembering that the value of an enum specifying
            *      the 'adaptee' (and version) should be updated after an adaptee upgrade.
                Example of a nuget path:
                // ...\.nuget\packages\mightylittlegeodesy\1.0.1\lib\net45\MightyLittleGeodesy.dll
                // From the above file we can extract two things i.e. the file name "MightyLittleGeodesy.dll"
                and the version number "1.0.1" and both these components can be put into 
                a FileInfoVersion instance
            *)
        member internal this._GetFileInfoVersion() : FileInfoVersion = 
            functionReturningFileInfoVersion()
        // Previously, the above method was exposed as public 
        // by being defined as abstract as below:
        // (and it should not be "abstract internal" since the access have to 
        //  be the same as the type, and the type should be inherited 
        //  by implementations in other assemblies)
        //abstract member _GetFileInfoVersion : unit -> FileInfoVersion
        //default this._GetFileInfoVersion() = defaultFileInfoVersion
        // However, to avoid exposing the above method as public
        // it is not "internal" instead, and the implementing 
        // subclasses (in other assemblies) can pass the function 
        // as a constructor parameter.

        member internal this.ValidateCoordinate(crsCoordinate: CrsCoordinate) = 
            // TODO add some tests for this validation
            // but actually I think the validation should become moved into
            // the creation of the coordinate i.e. throw exception already 
            // at creation with these kind of values,
            // and then the test should be in a class testing 
            // CrsCoordinate or CrsCoordinateFactory
            if isNull crsCoordinate then
                nullArg "crsCoordinate"
            let mutable isValid = true
            if(Double.IsInfinity(crsCoordinate.X)) then isValid <- false
            elif(Double.IsInfinity(crsCoordinate.Y)) then isValid <- false
            elif(Double.IsNaN(crsCoordinate.X)) then isValid <- false
            elif(Double.IsNaN(crsCoordinate.Y)) then isValid <- false
            elif(Double.IsNegativeInfinity(crsCoordinate.X)) then isValid <- false
            elif(Double.IsNegativeInfinity(crsCoordinate.Y)) then isValid <- false
            elif(Double.IsPositiveInfinity(crsCoordinate.X)) then isValid <- false
            elif(Double.IsPositiveInfinity(crsCoordinate.Y)) then isValid <- false
            if not(isValid) then
                failwith ("Coordinate not valid: " + crsCoordinate.ToString())

        member private this._TransformToCoordinate(inputCoordinate, crsIdentifier) = 
            TrowExceptionIfCoordinateIsNull(inputCoordinate)
            let coord = transformToCoordinateStrategy(inputCoordinate, crsIdentifier)
            this.ValidateCoordinate(coord)
            coord

        interface ICrsTransformationAdapter with
            member this.GetTransformationAdapterChildren() =  this.GetTransformationAdapterChildren()
                
            // -------------------------------------------------

            // The three below methods returning a coordinate object
            // are all final (i.e. not overrideable) and invokes
            // a so called "strategy" function (named so because of the design pattern Strategy)
            // which is passed as a constructor parameter.

            member this.TransformToCoordinate(inputCoordinate, crsCode) =
                this._TransformToCoordinate(inputCoordinate, CrsIdentifierFactory.CreateFromCrsCode(crsCode))

            member this.TransformToCoordinate(inputCoordinate, epsgNumberForOutputCoordinateSystem) = 
                this._TransformToCoordinate(inputCoordinate, CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumberForOutputCoordinateSystem))

            member this.TransformToCoordinate(inputCoordinate, crsIdentifier) = 
                this._TransformToCoordinate(inputCoordinate, crsIdentifier)
            // -------------------------------------------------

            // -------------------------------------------------
            member this.Transform(inputCoordinate, crsIdentifier) = 
                transformStrategy(inputCoordinate, crsIdentifier)

            member this.Transform(inputCoordinate, crsCode) =
                try
                    let crs = CrsIdentifierFactory.CreateFromCrsCode(crsCode)
                    // it is the row above which might throw an exception
                    // but the row below should not through an exception
                    transformStrategy(inputCoordinate, crs)
                with
                    // | :? System.Exception as exc -> 
                    // alternative to the above:
                    | exc -> 
                        CrsTransformationResult._CreateCrsTransformationResult(
                            inputCoordinate,
                            null,
                            exc,
                            false,
                            this,
                            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(new List<CrsTransformationResult>())
                        )
            // TODO refactor the try/with code duplicated above/below 
            member this.Transform(inputCoordinate, epsgNumberForOutputCoordinateSystem) = 
                try
                    let crs = CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumberForOutputCoordinateSystem)
                    // it is the row above which might throw an exception
                    // but the row below should not through an exception
                    transformStrategy(inputCoordinate, crs)
                with
                    // | :? System.Exception as exc -> 
                    // alternative to the above:
                    | exc -> 
                        CrsTransformationResult._CreateCrsTransformationResult(
                            inputCoordinate,
                            null,
                            exc,
                            false,
                            this,
                            CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(new List<CrsTransformationResult>())
                        )

            // -------------------------------------------------

            member this.LongNameOfImplementation = this.LongNameOfImplementation

            member this.ShortNameOfImplementation = this.ShortNameOfImplementation

            member this.AdapteeType = this.AdapteeType

            member this.IsComposite = this.IsComposite



    end