namespace Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI

open System.IO
open System.Text
open System.Linq
open System.Collections.Generic
open ProjNet.Converters.WellKnownText
open GeoAPI.CoordinateSystems

// "enum":
type EmbeddedResourceFileWithCRSdefinitions = 
    
    | STANDARD_FILE_SHIPPED_WITH_ProjNet4GeoAPI = 10

    // The only differences compared with the standard "SRID.csv" 
    // shipped with ProjNet4GeoAPI are the following five rows:
    //3019;PROJCS["RT90 7.5 gon V",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",11.30827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3019"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3020;PROJCS["RT90 5 gon V",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",13.55827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3020"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3021;PROJCS["RT90 2.5 gon V",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",15.80827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3021"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3022;PROJCS["RT90 0 gon",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",18.05827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3022"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3023;PROJCS["RT90 2.5 gon O",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",20.30827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3023"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3024;PROJCS["RT90 5 gon O",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",22.55827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3024"],AXIS["X",NORTH],AXIS["Y",EAST]]
    // The above five rows have been copied from the file below into a small CSV file embedded as resource.
    // SharpMap's SpatialRefSys.xml :
    // https://github.com/SharpMap/SharpMap/blob/Branches/1.0/SharpMap/SpatialRefSys.xml
    // Jul 20, 2015    
    // git commit 5fd5d7e7aaa641bc291ebe7b5a56d62970c0fdb4 :
    // https://github.com/SharpMap/SharpMap/commit/5fd5d7e7aaa641bc291ebe7b5a56d62970c0fdb4
    | SIX_SWEDISH_RT90_CRS_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml = 20

type SridReader private
    (
        functionReadingFromResourceFileOrExternalFilePath: int -> IDictionary<int, ICoordinateSystem>,
        stringForEqualityComparison: string
    ) =

    let GetSomeTypeInTheAssembly() = 
        let someTypeInTheNameSpace = typeof<CrsCachingStrategy>
        someTypeInTheNameSpace

    let GetSRIDsFromStreamReader
        (
            epsgNumberToLookFor: int,
            reader: StreamReader
        ) : IDictionary<int, ICoordinateSystem> = 

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

    // This is an "internal" method (for testing purposes).
    // The method may return for example a string like this:
    //  (depending on which of the two current constructors are used, i.e. file path or enum specifying a list of embedded resources files to use)
    //      "file:C:\temp\file1.csv"
    //      "embedded:STANDARD_FILE_SHIPPED_WITH_ProjNet4GeoAPI,SIX_SWEDISH_RT90_CRS_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml"
    // The exact format should not be used for anything else 
    // than the Equals and HashCode methods.
    // Therefore, the prefixes "file:" and "embedded:" can be considered as 
    // arbitrary just like the separator "," between potentially multiple names of enum values.
    member internal this._GetStringForEqualityComparison() =
        stringForEqualityComparison

    override this.GetHashCode() =
        stringForEqualityComparison.GetHashCode()
            
    override this.Equals(o) =
        match o with
        | :? SridReader as s -> s._GetStringForEqualityComparison() = stringForEqualityComparison
        | _ -> false

    static member private nameOfEmbeddedResourceFileDefaultShippedWithProjNet4GeoAPI_1_4_1 = "SRID_ShippedWithProjNet4GeoAPI_1_4_1.csv"
    // the above file was downloaded from here:
    // https://github.com/NetTopologySuite/ProjNet4GeoAPI/blob/develop/ProjNet.Tests/SRID.csv
    // (the file version dated "Jul 5, 2013" i.e. git commit c7a8b0c72d55ab64e26d40632abe2c85c2ff92df )

    static member private nameOfEmbeddedResourceFileWithSixSwedishRT90crsDefinitionsCopiedFromSharpMapSpatialRefSysXml = "SRID_SixSwedishCrsRT90_copiedFrom_SharpMapSpatialRefSysXml.csv"

    static member private GetNameOfEmbeddedResourceFile(embeddedResourceFileWithCRSdefinition) = 
        // if this code below will grow (i.e. if more small embedded resource files will be added) 
        // then create a hashtable lookup instead of adding more if statements
        if embeddedResourceFileWithCRSdefinition = EmbeddedResourceFileWithCRSdefinitions.SIX_SWEDISH_RT90_CRS_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml then
            SridReader.nameOfEmbeddedResourceFileWithSixSwedishRT90crsDefinitionsCopiedFromSharpMapSpatialRefSysXml
        else
            SridReader.nameOfEmbeddedResourceFileDefaultShippedWithProjNet4GeoAPI_1_4_1

    member this.GetSRIDs = functionReadingFromResourceFileOrExternalFilePath
        
    member this.GetSRIDsFromCsvFile
        (
            epsgNumberToLookFor: int,
            filePathForCsvFile: string
        ) : IDictionary<int, ICoordinateSystem> =     
        use reader = File.OpenText(filePathForCsvFile)
        GetSRIDsFromStreamReader(epsgNumberToLookFor, reader)


    member this.GetSRIDsFromEmbeddedResourceFile
        (
            epsgNumberToLookFor: int,
            nameOfEmbeddedResourceFile: string
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
        use stream = assembly.GetManifestResourceStream(someTypeInTheNameAssembly, nameOfEmbeddedResourceFile)
        if isNull stream then
            failwith ("the stream was null for the following namespace and filename: namespace:" + someTypeInTheNameAssembly.Namespace + "  filename: " + nameOfEmbeddedResourceFile)
        use reader = new StreamReader(stream)
        GetSRIDsFromStreamReader(epsgNumberToLookFor, reader)

    member this.GetSRIDsFromEmbeddedResourceFiles
        (
            epsgNumberToLookFor: int,
            orderedEmbeddedResourceFileWithCRSdefinitions: ICollection<EmbeddedResourceFileWithCRSdefinitions>
        ) : IDictionary<int, ICoordinateSystem> =
            let mutable dict = Dictionary<int, ICoordinateSystem>() :> IDictionary<int, ICoordinateSystem>
            for embeddedResourceFileWithCRSdefinitions in orderedEmbeddedResourceFileWithCRSdefinitions do
                let nameOfEmbeddedResourceFile: string = SridReader.GetNameOfEmbeddedResourceFile(embeddedResourceFileWithCRSdefinitions)
                let srids = this.GetSRIDsFromEmbeddedResourceFile(epsgNumberToLookFor,nameOfEmbeddedResourceFile)
                // typically the first item in the iterated collection 
                // will be the "big file", while the later may contain 
                // some modification. Therefore, inte first iteration
                // when there are still no items, we can replace everything
                // instead of iterating 
                if (dict.Count = 0) then
                    dict <- srids
                else
                    for srid in srids do
                        if (dict.ContainsKey(srid.Key)) then
                            dict.Remove(srid.Key) |> ignore
                        dict.Add(srid.Key, srid.Value)
            dict
                    

    /// <summary>Gets a coordinate system from the SRID.csv file</summary>
    /// <param name="id">EPSG ID</param>
    /// <returns>Coordinate system, or null if SRID was not found.</returns>
    member this.GetCSbyID(epsgId): ICoordinateSystem =
        //use stream = GetStreamReaderForTheCsvFile()
        //use reader = new StreamReader(stream)
        // let srids = GetSRIDs(reader, epsgId)
        // NOTE that the above kind of code causes problems
        // (see further comment in the below used function)
        let srids = this.GetSRIDs(epsgId)
        let mutable crs: ICoordinateSystem = null
        if srids.ContainsKey(epsgId) then
            crs <- srids.[epsgId]
        crs

    member this.GetAllCoordinateSystems() =
        //use stream = GetStreamReaderForTheCsvFile()
        //use reader = new StreamReader(stream)
        this.GetSRIDs(
            //reader //  NOTE that usage of reader as parameter caused problem, see more comments in the function
            -1 // epsgNumberToLookFor (since -1 will not be found, a fully populated dictionary will be returned instead)
        )

    new (pathToCsvFile: string) as this = 
        (
            let fkn: int -> IDictionary<int, ICoordinateSystem> = fun (id) -> this.GetSRIDsFromCsvFile(id, pathToCsvFile)
            let stringForEqualityComparison = "file:" + pathToCsvFile
            SridReader(fkn, stringForEqualityComparison)
        )

    new (
            //[<Optional; DefaultParameterValue(EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_SHIPPED_WITH_ProjNet4GeoAPI)>]
            //?embeddedResourceFileWithCRSdefinitions: EmbeddedResourceFileWithCRSdefinitions
            // no the above does not look nice when exposed to C# :
            // public SridReader([OptionalArgument] FSharpOption<EmbeddedResourceFileWithCRSdefinitions> embeddedResourceFileWithCRSdefinitions = 10);

            //embeddedResourceFileWithCRSdefinitions: EmbeddedResourceFileWithCRSdefinitions

            // The below semantic of "ordered" is that the resource files 
            // will be read in the order of the collection parameter
            // and if similar EPSG codes occur in the later, then the previous will be overridden.
            orderedEmbeddedResourceFileWithCRSdefinitions: ICollection<EmbeddedResourceFileWithCRSdefinitions>
        ) as this = 
        (
            let fkn: int -> IDictionary<int, ICoordinateSystem> = fun (id) -> this.GetSRIDsFromEmbeddedResourceFiles(id, orderedEmbeddedResourceFileWithCRSdefinitions)
            let stringForEqualityComparison = "embedded:" + orderedEmbeddedResourceFileWithCRSdefinitions.Select(fun e -> e.ToString()).Aggregate(fun i j -> i + "," + j)
            SridReader(fkn, stringForEqualityComparison)
        )
