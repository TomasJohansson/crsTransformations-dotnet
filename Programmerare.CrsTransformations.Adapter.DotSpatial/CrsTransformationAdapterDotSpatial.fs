namespace com.programmerare.crsTransformations.Adapter.DotSpatial

open DotSpatial.Projections
open com.programmerare.crsTransformations
open com.programmerare.crsTransformations.coordinate

type CrsTransformationAdapterDotSpatial() =
    class
        inherit CrsTransformationAdapterBaseLeaf()

        override this._TransformToCoordinateHookLeaf(inputCoordinate, crsIdentifierForOutputCoordinateSystem) = 
            let projInfoSourceCrs = ProjectionInfo.FromEpsgCode(inputCoordinate.CrsIdentifier.EpsgNumber);
            let projInfoTargetCrs = ProjectionInfo.FromEpsgCode(crsIdentifierForOutputCoordinateSystem.EpsgNumber);

            let xy: double[] = [| inputCoordinate.X; inputCoordinate.Y |];
            Reproject.ReprojectPoints(xy, null, projInfoSourceCrs, projInfoTargetCrs, 0, 1);

            CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude
                (
                    xy.[0],
                    xy.[1],
                    crsIdentifierForOutputCoordinateSystem
                )

        override this._TransformToCoordinateHook(inputCoordinate, crsIdentifier) = 
            this._TransformToCoordinateHookLeaf(inputCoordinate, crsIdentifier)

        override this.AdapteeType =
            CrsTransformationAdapteeType.LEAF_DOT_SPATIAL_1_9_0

        override this.LongNameOfImplementation = this.GetType().FullName

        // it is not a problem to hardcode the name below 
        // if the type name would become renamed since a 
        // renaming would then be detected by test code which 
        // verfifies that this short name actually is the 
        // suffix of the type name
        override this.ShortNameOfImplementation = "DotSpatial"
    end