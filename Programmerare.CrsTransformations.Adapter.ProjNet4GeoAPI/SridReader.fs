namespace Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI

open System.IO
open System.Text
open ProjNet.Converters.WellKnownText
open GeoAPI.CoordinateSystems
open System.Collections.Generic

module SridReader =
    
    type WKTstring(WKID, WKT)  =
        member this.WKID = WKID
        member this.WKT = WKT

    let GetSRIDs(reader: StreamReader) : List<WKTstring> = 
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
        wktStrings

    /// <summary>Gets a coordinate system from the SRID.csv file</summary>
    /// <param name="id">EPSG ID</param>
    /// <returns>Coordinate system, or null if SRID was not found.</returns>
    let GetCSbyID(epsgId): ICoordinateSystem =
        let typeInTheNameSpace = typeof<WKTstring>
        let assembly = typeInTheNameSpace.Assembly
        let fileName = "SRID.csv"
        // the above file was downloaded from here:
        // https://github.com/NetTopologySuite/ProjNet4GeoAPI/blob/develop/ProjNet.Tests/SRID.csv
        // (the file version dated "Jul 5, 2013" i.e. git commit c7a8b0c72d55ab64e26d40632abe2c85c2ff92df )

        // Regarding the below F# keyword "use" instead of "let":
        // It tells the compiler to automatically dispose of the resource when it goes out of scope. 
        // This is equivalent to the C# "using" keyword.
        // This can only be used in conjunction with classes that implement IDisposable.
        use stream = assembly.GetManifestResourceStream(typeInTheNameSpace, fileName)
        if isNull stream then
            failwith ("the stream was null for the following namespace and filename: namespace:" + typeInTheNameSpace.Namespace + "  filename: " + fileName)
        use reader = new StreamReader(stream)

        let srids = GetSRIDs(reader)
        let mutable crs: ICoordinateSystem = null
        for wkt in srids do
            if(wkt.WKID = epsgId) then
                crs <- CoordinateSystemWktReader.Parse(wkt.WKT, Encoding.UTF8) :?> ICoordinateSystem
        crs