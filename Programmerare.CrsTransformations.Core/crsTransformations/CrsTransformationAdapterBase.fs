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
// 
///<summary>
///This small class is used as return type for a method 
///with the only purpose to be used in test code for detecting 
///upgraded version (of some NuGet package or an adaptee library)
///i.e. to help remembering to update the enum value 
///defining which adaptee library version is used
///</summary>
type FileInfoVersion internal
    (
        fileName: string,
        fileSize: int64,
        version: string
    ) = 
    class
        member internal this.FileName = fileName
        member internal this.FileSize = fileSize
        member internal this.Version = version
        
        static member internal FileInfoVersionNOTrepresentingThirdPartLibrary = FileInfoVersion("", -1L, "")

        member internal this.IsRepresentingThirdPartLibrary() = 
            let f = FileInfoVersion.FileInfoVersionNOTrepresentingThirdPartLibrary
            if (
                // TODO refactor the below code with an Equals method of the type FileInfoVersion
                f.FileName.Equals(this.FileName)
                &&
                f.Version.Equals(this.Version)
                &&
                f.FileSize = this.FileSize
            ) then
                false
            else
                true

        ///<summary>
        ///Helper method intended to be used from implementing adapters 
        ///when implementing a method that should return the name
        ///of a DLL file (and the version information extracted from the path) 
        ///file belonging to an adaptee library.
        ///    This helper method is NOT intended for  client code
        ///    but it needs to be public since it used by 
        ///    adapter implementations in other assemblies.
        ///</summary>
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
                failwith ("The 'FileInfoVersion' parts could not be extracted from the string " + inputString + " which was retrieved using the type " + someTypeInTheThidPartAdapteeLibrary.GetType().FullName)
    end
// ----------------------------------------------------
///<summary>
///The base class of the adapter interface implementing most of the 
///transform methods as "final" i.e. NOT being declared as overrideable by using "abstract".
///</summary>
[<AbstractClass>]
type CrsTransformationAdapterBase
    (
        functionReturningFileInfoVersion: unit -> FileInfoVersion,

        // Might throw an exception, i.e. it is okay if function implementations 
        // throw an exception when they can not return a valid coordinate
        transformToCoordinateStrategy : CrsCoordinate * CrsIdentifier -> CrsCoordinate ,

        // Should never throw an exception (if doing so, then consider it as a bug), 
        // i.e. it is NOT okay if a function implementation throw an exception, 
        // but instead it should return a result object with success property being false,
        // and potentially the result object might contain an exception object
        // which can be retrieved when it is desirable to try to get information about the problem.
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

        override this.GetHashCode() =
            this.AdapteeType.GetHashCode()
            
        override this.Equals(o) =
            if(isNull o) then
                false
            else
                this.GetType().Equals(o.GetType())

        abstract member IsComposite : bool
        default this.IsComposite = raise (System.NotImplementedException())

        abstract member LongNameOfImplementation : string
        default this.LongNameOfImplementation = raise (System.NotImplementedException())

        abstract member ShortNameOfImplementation : string
        default this.ShortNameOfImplementation = raise (System.NotImplementedException())

        abstract member GetTransformationAdapterChildren : unit -> IList<ICrsTransformationAdapter>
        default this.GetTransformationAdapterChildren() = raise (System.NotImplementedException())

        abstract member TransformToCoordinate : CrsCoordinate * int -> CrsCoordinate
        default this.TransformToCoordinate(inputCoordinate, epsgNumberForOutputCoordinateSystem: int) = 
            this._TransformToCoordinate(inputCoordinate, CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumberForOutputCoordinateSystem))

        abstract member TransformToCoordinate : CrsCoordinate * string -> CrsCoordinate
        default this.TransformToCoordinate(inputCoordinate, crsCode: string) =
            this._TransformToCoordinate(inputCoordinate, CrsIdentifierFactory.CreateFromCrsCode(crsCode))

        abstract member TransformToCoordinate : CrsCoordinate * CrsIdentifier -> CrsCoordinate
        default this.TransformToCoordinate(inputCoordinate, crsIdentifier: CrsIdentifier) = 
            this._TransformToCoordinate(inputCoordinate, crsIdentifier)

        abstract member Transform : CrsCoordinate * CrsIdentifier -> CrsTransformationResult
        default this.Transform(inputCoordinate, crsIdentifier: CrsIdentifier) = 
            transformStrategy(inputCoordinate, crsIdentifier)

        abstract member Transform : CrsCoordinate * int -> CrsTransformationResult
        default this.Transform(inputCoordinate, epsgNumberForOutputCoordinateSystem: int) = 
            // see the comment in below method '_Transform'
            this._Transform
                (
                    inputCoordinate, 
                    fun () -> CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumberForOutputCoordinateSystem)
                )

        abstract member Transform : CrsCoordinate * string -> CrsTransformationResult
        default this.Transform(inputCoordinate, crsCode: string) =
            // see the comment in below method '_Transform'
            this._Transform
                (
                    inputCoordinate, 
                    fun () -> CrsIdentifierFactory.CreateFromCrsCode(crsCode)
                )


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
            if isNull crsCoordinate then
                nullArg "crsCoordinate"
            // Most of the other validation has been moved into the 
            // creation of the coordinate i.e. it should not even be possible
            // to create a coordinate instance with some of the coordinate pair value
            // being e.g. "double.NaN" so therefore no more such validation here

        member private this._TransformToCoordinate(inputCoordinate, crsIdentifier) = 
            TrowExceptionIfCoordinateIsNull(inputCoordinate)
            let coord = transformToCoordinateStrategy(inputCoordinate, crsIdentifier)
            this.ValidateCoordinate(coord)
            coord

        member private this._Transform
            (
                inputCoordinate: CrsCoordinate, 
                functionCreatingCrsIdentifier: unit -> CrsIdentifier
            ) =
            // The Transform methods should never throw an exception
            // but the methods 
            // 'CrsIdentifierFactory.CreateFromCrsCode'
            // and
            // 'CrsIdentifierFactory.CreateFromEpsgNumber'
            // (which are used from the Transform methods in this class )
            // might throw an exception, but to reduce duplication 
            // of exception handling, we are using a function parameter
            // which is invoked here, i.e. in one place we use the 
            // try/with statement instead of duplicating it twice
            try
                let crsIdentifier = functionCreatingCrsIdentifier() // invoking either CrsIdentifierFactory.CreateFromCrsCode or CrsIdentifierFactory.CreateFromEpsgNumber
                // it is the row above which might throw an exception
                // but the row below should not through an exception
                transformStrategy(inputCoordinate, crsIdentifier)
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
            
        // F# implements explicit interfaces as below
        // but to make those methods also available through 
        // subtypes/classes, the above code makes it possible
        // i.e. the above "implicit interface" abstract methods 
        // can define a default implementation which is called 
        // from the explicit interface implementation below 
        // to get the same behaviour regardless how the methods
        // are invoked i.e. if they are invoked through an 
        // interface typed object or through some subtype.
        interface ICrsTransformationAdapter with
            member this.GetTransformationAdapterChildren() =  this.GetTransformationAdapterChildren()
                
            // -------------------------------------------------

            // The three below methods returning a coordinate object
            // are all final (i.e. not overrideable) and invokes
            // a so called "strategy" function (named so because of the design pattern Strategy)
            // which is passed as a constructor parameter.

            member this.TransformToCoordinate(inputCoordinate, crsCode: string) =
                this.TransformToCoordinate(inputCoordinate, crsCode)

            member this.TransformToCoordinate(inputCoordinate, epsgNumberForOutputCoordinateSystem: int) = 
                this.TransformToCoordinate(inputCoordinate, epsgNumberForOutputCoordinateSystem)

            member this.TransformToCoordinate(inputCoordinate, crsIdentifier: CrsIdentifier) = 
                this.TransformToCoordinate(inputCoordinate, crsIdentifier)
            // -------------------------------------------------

            // -------------------------------------------------
            member this.Transform(inputCoordinate, crsIdentifier: CrsIdentifier) = 
                this.Transform(inputCoordinate, crsIdentifier)

            member this.Transform(inputCoordinate, crsCode: string) =
                this.Transform(inputCoordinate, crsCode)
            
            member this.Transform(inputCoordinate, epsgNumberForOutputCoordinateSystem: int) = 
                this.Transform(inputCoordinate, epsgNumberForOutputCoordinateSystem)
            // -------------------------------------------------

            member this.LongNameOfImplementation = this.LongNameOfImplementation

            member this.ShortNameOfImplementation = this.ShortNameOfImplementation

            member this.AdapteeType = this.AdapteeType

            member this.IsComposite = this.IsComposite

    end