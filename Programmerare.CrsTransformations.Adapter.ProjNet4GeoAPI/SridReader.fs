namespace Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI

open System.IO
open System.Text
open System.Collections.Generic
open ProjNet.Converters.WellKnownText
open GeoAPI.CoordinateSystems

module SridReader =

    let private fileName = "SRID.csv"
    // the above file was downloaded from here:
    // https://github.com/NetTopologySuite/ProjNet4GeoAPI/blob/develop/ProjNet.Tests/SRID.csv
    // (the file version dated "Jul 5, 2013" i.e. git commit c7a8b0c72d55ab64e26d40632abe2c85c2ff92df )

    let private GetSomeTypeInTheAssembly() = 
        let someTypeInTheNameSpace = typeof<CrsCachingStrategy>
        someTypeInTheNameSpace
        
    let private GetSRIDs
        (
            epsgNumberToLookFor: int
        ) : IDictionary<int, ICoordinateSystem> = 
        let someTypeInTheNameAssembly = GetSomeTypeInTheAssembly()
        let assembly = someTypeInTheNameAssembly.Assembly

        // Regarding the below F# keyword "use" instead of "let":
        // It tells the compiler to automatically dispose of the resource when it goes out of scope. 
        // This is equivalent to the C# "using" keyword.
        // This can only be used in conjunction with classes that implement IDisposable.
        // NOTE that there can be tricky problems if trying to use "use" 
        // from outside and then pass a strem or read to the function ...
        // (see comment further down in this function)
        use stream = assembly.GetManifestResourceStream(someTypeInTheNameAssembly, fileName)
        if isNull stream then
            failwith ("the stream was null for the following namespace and filename: namespace:" + someTypeInTheNameAssembly.Namespace + "  filename: " + fileName)
        use reader = new StreamReader(stream)

        let coordinateSystemsWithKeyEpsgNumber = new Dictionary<int, ICoordinateSystem>()
        let mutable valid = true
        while valid do 
            let line = reader.ReadLine()

            //use stream = GetStreamReaderForTheCsvFile()
            //use reader = new StreamReader(stream)
            // Previously the above stream/reader object was created outside of this function
            // and the reader was sent as a parameter but that caused problem 
            // which was tricky to find since the code seemed to silently stop somewhere 
            // and I found the problem by wrapping a try statement around 
            // the ReadLine as below:
            //try
            //    line <- reader.ReadLine()
            //with
            //    exc ->
            //        //Console.WriteLine("exception : " + exc.ToString())
            //        //exception : System.ObjectDisposedException: Cannot read from a closed TextReader.
            //        //   at System.IO.StreamReader.ReadLine()
            //        //   at Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI.SridReader.GetSRIDs(StreamReader reader, Int32 epsgNumberToLookFor) in F:\t\r\c\f\o\d\crsTransformations-dotnet\Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI\SridReader.fs:line 25
            //        () // just keep on iterating and try with the next
            if (isNull line) then
                valid <- false
            else
                let split = line.IndexOf(';')
                if (split > -1) then
                    let epsgNumber = System.Int32.Parse(line.Substring(0, split))
                    let wellKnownText = line.Substring(split + 1)
                    let crs = CoordinateSystemWktReader.Parse(wellKnownText, Encoding.UTF8) :?> ICoordinateSystem
                    coordinateSystemsWithKeyEpsgNumber.Add(epsgNumber, crs)
                    if epsgNumber = epsgNumberToLookFor then
                        // we were only looking for a specific number so no need to keep iterating
                        valid <- false
        coordinateSystemsWithKeyEpsgNumber :> IDictionary<int, ICoordinateSystem>

    /// <summary>Gets a coordinate system from the SRID.csv file</summary>
    /// <param name="id">EPSG ID</param>
    /// <returns>Coordinate system, or null if SRID was not found.</returns>
    let GetCSbyID(epsgId): ICoordinateSystem =
        //use stream = GetStreamReaderForTheCsvFile()
        //use reader = new StreamReader(stream)
        // let srids = GetSRIDs(reader, epsgId)
        // NOTE that the above kind of code causes problems
        // (see further comment in the below used function)
        let srids = GetSRIDs(epsgId)
        let mutable crs: ICoordinateSystem = null
        if srids.ContainsKey(epsgId) then
            crs <- srids.[epsgId]
        crs

    let GetAllCoordinateSystems() =
        //use stream = GetStreamReaderForTheCsvFile()
        //use reader = new StreamReader(stream)
        GetSRIDs(
            //reader //  NOTE that usage of reader as parameter caused problem, see more comments in the function
            -1 // epsgNumberToLookFor (since -1 will not be found, a fully populated dictionary will be returned instead)
        )