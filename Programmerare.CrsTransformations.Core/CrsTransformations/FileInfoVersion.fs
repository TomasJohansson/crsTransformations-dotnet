namespace Programmerare.CrsTransformations

open System
open System.IO
open System.Text.RegularExpressions

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

        override this.Equals(obj) =
            match obj with
            | :? FileInfoVersion as f -> (fileName, fileSize, version) = (f.FileName, f.FileSize, f.Version)
            | _ -> false

        override this.GetHashCode() =
            Tuple.Create(this.FileName, this.FileSize, this.Version).GetHashCode()

        member internal this.IsRepresentingThirdPartLibrary() = 
            not(this.Equals(FileInfoVersion.FileInfoVersionNOTrepresentingThirdPartLibrary))

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
            // but the version number 1.0.0.0 is not what I want which is 1.0.2 
            // which can be extracted from the path below
            // the code base will be some path like:
            // "...nuget/packages/mightylittlegeodesy/1.0.2/lib/net45/MightyLittleGeodesy.dll"
            // version between some slashes in the below regexp: 
            // e.g. "2.0.0-rc1" or "1.0.1"
            // However, this does not seem to always work, at least not for Linux and .NET5, 
            // so if the regexp is failing, then trying to retrieve a version from within the assembly instead.
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
                let versionAccordingToAssembly = assembly.GetName().Version.ToString()
                // printfn "GetFileInfoVersionHelper versionAccordingToAssembly %s" versionAccordingToAssembly
                FileInfoVersion
                    (
                        file.Name,
                        file.Length,
                        versionAccordingToAssembly
                    )
    end