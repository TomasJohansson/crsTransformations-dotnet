namespace Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI

open System.IO
open System.Reflection
open System.Text
open ProjNet.Converters.WellKnownText
open GeoAPI.CoordinateSystems
open System.Collections.Generic
open System.Linq

// just a workaround to get an available 
// type to get the assembly
// and to get the resource file SRID.csv
// because of problems with using e.g. "typeof"
// with a module (but it works for a type as below)
type MyType = 
    class
    end

module SridReader =

    type WKTstring(WKID, WKT)  =
        member this.WKID = WKID
        member this.WKT = WKT

    /// <summary>Enumerates all SRID's in the SRID.csv file.</summary>
    /// <returns>Enumerator</returns>
    //let GetSRIDs(filename) = // IEnumerable<WKTstring>
    let GetSRIDs(reader: StreamReader) = // IEnumerable<WKTstring>
        //let lines = System.IO.File.ReadLines(filename)
        //let res = lines.Select(fun line -> 
        let mutable valid = true
        let wktStrings = List<WKTstring>()
        while valid do
            let line = reader.ReadLine()
            if line = null then
                valid <- false
            else
                let split = line.IndexOf(';')
                if (split > -1) then
                    let id = System.Int32.Parse(line.Substring(0, split))
                    let str = line.Substring(split + 1)
                    wktStrings.Add(WKTstring(id, str))
        //let srids: IEnumerable<WKTstring> = res.Where(fun w -> w.WKID > -1)
        //srids
        wktStrings

    /// <summary>Gets a coordinate system from the SRID.csv file</summary>
    /// <param name="id">EPSG ID</param>
    /// <returns>Coordinate system, or null if SRID was not found.</returns>
    let GetCSbyID(id) = // ICoordinateSystem
        let typeInTheNameSpace = typeof<MyType>
        printfn "Namespace for typeInTheNameSpace: %s" typeInTheNameSpace.Namespace
        let assembly = typeInTheNameSpace.Assembly
        printfn "assembly name: %s" assembly.FullName
        //let assembly = Assembly.GetExecutingAssembly(); // would become e.g. the assembly with NUnit tests
        //let resourceName = "Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI.SRID.csv"
        // com.programmerare.crsTransformations.Adapter.ProjNet4GeoAPI.SRID.csv
        
        let fileName = "SRID.csv"
        // the above file was downloaded from here:
        // https://github.com/NetTopologySuite/ProjNet4GeoAPI/blob/develop/ProjNet.Tests/SRID.csv
        // (the file version dated "Jul 5, 2013" i.e. git commit c7a8b0c72d55ab64e26d40632abe2c85c2ff92df )

        //let resourceName = typeInTheNameSpace.Namespace + "." + fileName
        //let names = assembly.GetManifestResourceNames()
        //for name in names do
        //    printfn "res name: %s" name

        // "use" instead of let:
        // tells the compiler to automatically dispose of the resource when it goes out of scope. This is equivalent to the C# “using” keyword.
        // This can only be used in conjunction with classes that implement IDisposable.
        //use stream = assembly.GetManifestResourceStream(resourceName)
        use stream = assembly.GetManifestResourceStream(typeInTheNameSpace, fileName)
        if isNull stream then
            failwith ("the stream was null for the following namespace and filename: namespace:" + typeInTheNameSpace.Namespace + "  filename: " + fileName)
        use reader = new StreamReader(stream)
        //reader.ReadLine

        let srids = GetSRIDs(reader)
        let mutable crs: ICoordinateSystem = null
        for wkt in srids do
            if(wkt.WKID = id) then
                crs <- CoordinateSystemWktReader.Parse(wkt.WKT, Encoding.UTF8) :?> ICoordinateSystem
        crs