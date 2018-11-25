namespace Programmerare.CrsTransformations.Adapter.DotSpatial

open DotSpatial.Projections
open Programmerare.CrsTransformations
open Programmerare.CrsTransformations.Coordinate

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
            CrsTransformationAdapteeType.LEAF_DOT_SPATIAL_2_0_0_RC1

        override this.LongNameOfImplementation = this.GetType().FullName

        // it is not a problem to hardcode the name below 
        // if the type name would become renamed since a 
        // renaming would then be detected by test code which 
        // verfifies that this short name actually is the 
        // suffix of the type name
        override this.ShortNameOfImplementation = "DotSpatial"

        override this._GetFileInfoVersion() =
            base._GetFileInfoVersionHelper(typeof<ProjectionInfo>)

    end