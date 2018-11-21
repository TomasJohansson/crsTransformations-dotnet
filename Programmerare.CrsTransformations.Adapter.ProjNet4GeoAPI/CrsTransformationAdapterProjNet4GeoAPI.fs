namespace com.programmerare.crsTransformations.Adapter.ProjNet4GeoAPI

open GeoAPI.CoordinateSystems
open GeoAPI.CoordinateSystems.Transformations
open ProjNet
open ProjNet.CoordinateSystems
open ProjNet.CoordinateSystems.Transformations
open System.Collections.Generic
open com.programmerare.crsTransformations
open com.programmerare.crsTransformations.coordinate
open com.programmerare.crsTransformations.crsIdentifier

type CrsTransformationAdapterProjNet4GeoAPI() =
    class
        inherit CrsTransformationAdapterBaseLeaf()

        //https://github.com/NetTopologySuite/ProjNet4GeoAPI/wiki/Loading-a-projection-by-Spatial-Reference-ID
        //Proj.NET doesn't have an embedded Spatial Reference ID database like the EPSG database, so there is no default logic for loading a spatial reference by ID. However Proj.NET does ship with a comma-separated file with EPSG codes, and you can easily iterate through these to load a specific ID. This is not as efficient as loading the data from an indexed database, but it's simple and easy to deploy.
        //Below is a simple class for loading a coordinate system by SRID.
        // public class SridReader ...
        // TODO ... see above url

        override this._TransformToCoordinateHookLeaf(inputCoordinate, crsIdentifierForOutputCoordinateSystem) = 
            let css = 
                new CoordinateSystemServices
                    (
                    new CoordinateSystemFactory(),
                    new CoordinateTransformationFactory()
                    )
            let sourceCrs: ICoordinateSystem = css.GetCoordinateSystem(inputCoordinate.CrsIdentifier.EpsgNumber)
            let targetCrs: ICoordinateSystem = css.GetCoordinateSystem(crsIdentifierForOutputCoordinateSystem.EpsgNumber)
            //let csFact: CoordinateSystemFactory = new CoordinateSystemFactory()
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
    end