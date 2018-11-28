namespace Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI

open System.IO
open System.Text
open System.Collections.Generic
open ProjNet.Converters.WellKnownText
open GeoAPI.CoordinateSystems

// "enum":
type EmbeddedResourceFileWithCRSdefinitions = 
    
    | STANDARD_FILE_SHIPPED_WITH_ProjNet4GeoAPI = 10

    // The only difference compared with the standard "SRID.csv" 
    // shipped with ProjNet4GeoAPI are the following five rows:
    //3019;PROJCS["RT90 7.5 gon V",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",11.30827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3019"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3020;PROJCS["RT90 5 gon V",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",13.55827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3020"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3021;PROJCS["RT90 2.5 gon V",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",15.80827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3021"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3022;PROJCS["RT90 0 gon",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",18.05827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3022"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3023;PROJCS["RT90 2.5 gon O",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",20.30827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3023"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3024;PROJCS["RT90 5 gon O",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",22.55827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3024"],AXIS["X",NORTH],AXIS["Y",EAST]]
    // The above five rows have been copied from the following file:
    // SharpMap/SpatialRefSys.xml
    // ( TODO use the link to github and maybe also the git commit id to specify the version of the source )
    | STANDARD_FILE_EXCEPT_FOR_SWEDISH_CRS_WITH_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml = 20


type SridReader private
    (
        functionReadingFromResourceFileOrExternalFilePath: int -> IDictionary<int, ICoordinateSystem>
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

    static member private nameOfEmbeddedResourceFileDefaultShippedWithProjNet4GeoAPI_1_4_1 = "SRID_ShippedWithProjNet4GeoAPI_1_4_1.csv"
    // the above file was downloaded from here:
    // https://github.com/NetTopologySuite/ProjNet4GeoAPI/blob/develop/ProjNet.Tests/SRID.csv
    // (the file version dated "Jul 5, 2013" i.e. git commit c7a8b0c72d55ab64e26d40632abe2c85c2ff92df )

    static member private nameOfEmbeddedResourceFileAdjustedWithSharpMap_SpatialRefSys_xml = "SRID_ShippedWithProjNet4GeoAPI_1_4_1_butUsing_SharpMap_SpatialRefSys_forSwedishRT90.csv"

    // TODO: override the six RT90 CRS in some better way than (as below currently) bundling 
    // two files with 1.5 MB each with the only modifications being those six rows in the csv file
    static member private GetNameOfEmbeddedResourceFile(embeddedResourceFileWithCRSdefinition) = 
        if embeddedResourceFileWithCRSdefinition = EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_EXCEPT_FOR_SWEDISH_CRS_WITH_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml then
            SridReader.nameOfEmbeddedResourceFileAdjustedWithSharpMap_SpatialRefSys_xml
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
            SridReader(fkn)
        )

    new (
            //[<Optional; DefaultParameterValue(EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_SHIPPED_WITH_ProjNet4GeoAPI)>]
            //?embeddedResourceFileWithCRSdefinitions: EmbeddedResourceFileWithCRSdefinitions
            embeddedResourceFileWithCRSdefinitions: EmbeddedResourceFileWithCRSdefinitions
            // no the above does not look nice when exposed to C# :
            // public SridReader([OptionalArgument] FSharpOption<EmbeddedResourceFileWithCRSdefinitions> embeddedResourceFileWithCRSdefinitions = 10);
        ) as this = 
        (
            let nameOfEmbeddedResourceFile = SridReader.GetNameOfEmbeddedResourceFile(embeddedResourceFileWithCRSdefinitions)
            let fkn: int -> IDictionary<int, ICoordinateSystem> = fun (id) -> this.GetSRIDsFromEmbeddedResourceFile(id, nameOfEmbeddedResourceFile)
            SridReader(fkn)
        )
