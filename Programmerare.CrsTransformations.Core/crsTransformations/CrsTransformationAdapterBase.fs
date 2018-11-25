namespace Programmerare.CrsTransformations

open System
open System.IO
open System.Text.RegularExpressions
open System.Collections.Generic
open Programmerare.CrsTransformations.Coordinate
open Programmerare.CrsTransformations.Identifier

// ----------------------------------------------------
// this class is used as return type for a methods 
// with the only purpose to be used in test code for detecting 
// upgraded version (of some NuGet package or an adaptee library)
// i.e. to help remembering to update the enum value 
// defining which adaptee library version is used
// maybe use [<AllowNullLiteral>] at FileInfoVersion instead for C# interop
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
    end
// ----------------------------------------------------

(*
 * The base class of the adapter interface implementing most of the 
 * transform methods as final i.e. not overridden by subclasses.  
 * 
 * @see CrsTransformationAdapter
 *
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-adapter-core" project is licensed with MIT.
 * Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
 * Please find more information in the license file at the root directory of each subproject
 * (e.g. the subprojects "crs-transformation-adapter-impl-geotools" , "crs-transformation-adapter-impl-proj4j" and so on)
 *)
[<AbstractClass>]
type CrsTransformationAdapterBase
    (
    ) =
    class

        // TODO maybe use [<AllowNullLiteral>] at FileInfoVersion declaration 
        // i.e. use null instead of the below values 
        // for C# interopability, i.e. like this
        // let defaultFileInfoVersion: FileInfoVersion = null
        let defaultFileInfoVersion = FileInfoVersion("", -1L, "")

        let TrowExceptionIfCoordinateIsNull(inputCoordinate) : unit = 
            if isNull inputCoordinate then
                nullArg "inputCoordinate"
            // The above would cause this error:
            // "Value cannot be null. Parameter name: inputCoordinate"
            // instead of the following which would occur later:
            // "Object reference not set to an instance of an object"
                

        (*
         * Transforms a coordinate to another coordinate reference system.  
         * 
         * This is a "hook" method (as it is named in the design pattern Template Method)   
         * which must be implemented by subclasses.
         *)
        // TODO: try making this method "internal" ... ? since "protected" is not currently used in F# ...
        abstract _TransformToCoordinateHook : CrsCoordinate * CrsIdentifier -> CrsCoordinate

        abstract _TransformHook : CrsCoordinate * CrsIdentifier -> CrsTransformationResult

        static member private classNamePrefix = "CrsTransformationAdapter"
        // if the above string would change because of class renamings
        // then it will be detected by a failing test

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

        // TODO: rewrite the below Kotlin/JVM comments below for .NET ...
        // The purpose of the method below is to use it in test code
        // for detecting upgrades to a new version (and then update the above method returned enum value)
        // Future failure will be a reminder to update the above enum value
        (*
            * This helper method is protected since it is NOT intended for
            * client code but only for test code purposes.
            * 
            * It should be overridden by subclasses.
            * @return empty string is returned as the default value
            *      which should also be returned byt the composites (i.e. they should not override).
            *      
            *      The 'leaf' adapter implementations should return the
            *      name of the jar file (potentially including a path)
            *      for the used adaptee library.
            *      
            *      The reason is that the jar files (retrieved through Maven)
            *      includes the version name and can be asserted in test code
            *      to help remembering that the value of an enum specifying
            *      the 'adaptee' (and version) should be updated after an adaptee upgrade.
            * @see CrsTransformationAdapteeType
            *)
        abstract member _GetFileInfoVersion : unit -> FileInfoVersion
        default this._GetFileInfoVersion() = defaultFileInfoVersion
        
        // TODO: rewrite the below Kotlin/JVM comments below for .NET ...
        (*
            * Helper method intended to be used from subclasses
            * when implementing the method that should return the name
            * of a jar file belonging to an adaptee library.
            *)
        member this._GetFileInfoVersionHelper
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
                defaultFileInfoVersion

        interface ICrsTransformationAdapter with
            member this.GetTransformationAdapterChildren() =  this.GetTransformationAdapterChildren()
                
            // -------------------------------------------------
            // The three below methods returning a coordinate object
            // are all final (i.e. not overridden) and invokes
            // a so called "hook" method(named so in the Template Method pattern)
            // which is an abstract method that must be implemented in a subclass.

            member this.TransformToCoordinate(inputCoordinate, crsCode) =
                TrowExceptionIfCoordinateIsNull(inputCoordinate)
                this._TransformToCoordinateHook(inputCoordinate, CrsIdentifierFactory.CreateFromCrsCode(crsCode))

            member this.TransformToCoordinate(inputCoordinate, epsgNumberForOutputCoordinateSystem) = 
                TrowExceptionIfCoordinateIsNull(inputCoordinate)
                this._TransformToCoordinateHook(inputCoordinate, CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumberForOutputCoordinateSystem))

            member this.TransformToCoordinate(inputCoordinate, crsIdentifier) = 
                TrowExceptionIfCoordinateIsNull(inputCoordinate)
                this._TransformToCoordinateHook(inputCoordinate, crsIdentifier)
            // -------------------------------------------------

            // -------------------------------------------------
            member this.Transform(inputCoordinate, crsIdentifier) = 
                this._TransformHook(inputCoordinate, crsIdentifier)

            member this.Transform(inputCoordinate, crsCode) =
                try
                    let crs = CrsIdentifierFactory.CreateFromCrsCode(crsCode)
                    // it is the row above which might throw an exception
                    // but the row below should not through an exception
                    this._TransformHook(inputCoordinate, crs)
                with
                    // | :? System.Exception as exc -> 
                    // alternative to the above:
                    | exc -> 
                        CrsTransformationResult(
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
                    this._TransformHook(inputCoordinate, crs)
                with
                    // | :? System.Exception as exc -> 
                    // alternative to the above:
                    | exc -> 
                        CrsTransformationResult(
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