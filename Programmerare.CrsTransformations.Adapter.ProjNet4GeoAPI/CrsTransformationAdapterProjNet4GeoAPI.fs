namespace Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI
open System.Collections.Generic
open ProjNet // CoordinateSystemServices
open ProjNet.CoordinateSystems // CoordinateSystemFactory
open ProjNet.CoordinateSystems.Transformations // CoordinateTransformationFactory
open GeoAPI.CoordinateSystems // ICoordinateSystem
open GeoAPI.CoordinateSystems.Transformations // ICoordinateTransformation
open Programmerare.CrsTransformations
open Programmerare.CrsTransformations.Coordinate
(*
Copyright (c) Tomas Johansson , http://programmerare.com
The code in the "Core" project is licensed with MIT.
Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
Please find more information in the license file at the root directory of each subproject
(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")
*)
type CrsTransformationAdapterProjNet4GeoAPI() =
    class
        inherit CrsTransformationAdapterBaseLeaf()

        let mutable _crsCachingStrategy: CrsCachingStrategy = CrsCachingStrategy.CACHE_ALL_EPSG_CRS_CODES
        
        let mutable _cachedCoordinateSystem: IDictionary<int, ICoordinateSystem> = Dictionary<int, ICoordinateSystem>() :> IDictionary<int, ICoordinateSystem>
        
        let mutable _sridReader = 
            let list = new ResizeArray<EmbeddedResourceFileWithCRSdefinitions>([
                    EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_SHIPPED_WITH_ProjNet4GeoAPI
                    EmbeddedResourceFileWithCRSdefinitions.STANDARD_FILE_EXCEPT_FOR_SWEDISH_CRS_WITH_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml
                ]
            )
            SridReader(list)

        let GetCSbyID(epsgNumber) = 
            let mutable crs: ICoordinateSystem = null
            if _cachedCoordinateSystem.ContainsKey(epsgNumber) then
                crs <- _cachedCoordinateSystem.[epsgNumber]
                // note that the above might return null since null 
                // may have been cached if not existing, which is 
                // intentional behaviour since there is no point of parsing the csv
                // file again just to find null once again
            elif (_crsCachingStrategy = CrsCachingStrategy.CACHE_ALL_EPSG_CRS_CODES) then
                if(_cachedCoordinateSystem.Count = 0) then
                    _cachedCoordinateSystem <- _sridReader.GetAllCoordinateSystems()
                if(_cachedCoordinateSystem.ContainsKey(epsgNumber)) then
                    crs <- _cachedCoordinateSystem.[epsgNumber]
                else
                    // add it anyway (as null) now to avoid looking it up again
                    _cachedCoordinateSystem.Add(epsgNumber, null)
            elif (_crsCachingStrategy = CrsCachingStrategy.CACHE_EPSG_CRS_CODE_WHEN_FIRST_USED) then
                crs <- _sridReader.GetCSbyID(epsgNumber)
                _cachedCoordinateSystem.Add(epsgNumber, crs)
            else
                crs <- _sridReader.GetCSbyID(epsgNumber)
            crs

        //https://github.com/NetTopologySuite/ProjNet4GeoAPI/wiki/Loading-a-projection-by-Spatial-Reference-ID
        //Proj.NET doesn't have an embedded Spatial Reference ID database like the EPSG database, so there is no default logic for loading a spatial reference by ID. However Proj.NET does ship with a comma-separated file with EPSG codes, and you can easily iterate through these to load a specific ID. This is not as efficient as loading the data from an indexed database, but it's simple and easy to deploy.
        //Below is a simple class for loading a coordinate system by SRID.
        // public class SridReader ...
        
        // The kind of class at the above URL has now been implemented in this F# project

        override this._TransformToCoordinateHookLeaf(inputCoordinate, crsIdentifierForOutputCoordinateSystem) = 
            let css = 
                new CoordinateSystemServices
                    (
                    new CoordinateSystemFactory(),
                    new CoordinateTransformationFactory()
                    )
            //let sourceCrs: ICoordinateSystem = css.GetCoordinateSystem(inputCoordinate.CrsIdentifier.EpsgNumber)
            //let targetCrs: ICoordinateSystem = css.GetCoordinateSystem(crsIdentifierForOutputCoordinateSystem.EpsgNumber)
            // The above code does not support many CRS and therefore the code below was added
            // Note that it is possible to use a caching strategy since the code below 
            // would otherwise read the resource file twice, which it would do 
            // for every usage of the method even if doing multiple 
            // transformations between the same coordinate systems
            let sourceCrs: ICoordinateSystem = GetCSbyID(inputCoordinate.CrsIdentifier.EpsgNumber)
            let targetCrs: ICoordinateSystem = GetCSbyID(crsIdentifierForOutputCoordinateSystem.EpsgNumber)
            let ctFact: CoordinateTransformationFactory = new CoordinateTransformationFactory()
            let xy: double[] = [| inputCoordinate.X; inputCoordinate.Y |]
            let trans: ICoordinateTransformation  = ctFact.CreateFromCoordinateSystems(sourceCrs, targetCrs)
            let result: double[] = trans.MathTransform.Transform(xy)
            CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude
                (
                    result.[0],
                    result.[1],
                    crsIdentifierForOutputCoordinateSystem
                )

        override this._TransformToCoordinateHook(inputCoordinate, crsIdentifier) = 
            this._TransformToCoordinateHookLeaf(inputCoordinate, crsIdentifier)

        override this.AdapteeType =
            CrsTransformationAdapteeType.LEAF_PROJ_NET_4_GEO_API_1_4_1

        override this.LongNameOfImplementation = this.GetType().FullName

        // it is not a problem to hardcode the name below 
        // if the type name would become renamed since a 
        // renaming would then be detected by test code which 
        // verfifies that this short name actually is the 
        // suffix of the type name
        override this.ShortNameOfImplementation = "ProjNet4GeoAPI"

        override this._GetFileInfoVersion() =
            base._GetFileInfoVersionHelper(typeof<CoordinateSystemFactory>)

        member this.SetSridReader(sridReader: SridReader) = 
            _sridReader <- sridReader

        member this.SetCrsCachingStrategy(crsCachingStrategy: CrsCachingStrategy) = 
            if (
                    crsCachingStrategy = CrsCachingStrategy.NO_CACHING
                    ||
                    (
                        _crsCachingStrategy <> CrsCachingStrategy.CACHE_ALL_EPSG_CRS_CODES
                        &&
                        crsCachingStrategy = CrsCachingStrategy.CACHE_ALL_EPSG_CRS_CODES
                        // if it changed to looking up everything 
                        // from something else then now reset the hashtable below
                        // since the lookup method first check the hash table 
                    )
                ) then
                _cachedCoordinateSystem <- Dictionary<int, ICoordinateSystem>()
            _crsCachingStrategy <- crsCachingStrategy

        member this.GetCrsCachingStrategy() = 
            _crsCachingStrategy
        
        // mainly for testing purpose:
        member this.IsEpsgCached(epsgNumber) : bool =     
            _cachedCoordinateSystem.ContainsKey(epsgNumber)
    end
(*
https://github.com/NetTopologySuite/ProjNet4GeoAPI
https://www.nuget.org/packages/ProjNet4GeoAPI
*)