namespace Programmerare.CrsTransformations.Adapter.DotSpatial
open DotSpatial.Projections
open Programmerare.CrsTransformations
open Programmerare.CrsTransformations.Coordinate
(*
Copyright (c) Tomas Johansson , http://programmerare.com
The code in the "Core" project is licensed with MIT.
Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
Please find more information in the license file at the root directory of each subproject
(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")
*)
type CrsTransformationAdapterDotSpatial() as this =
    class
        inherit CrsTransformationAdapterBaseLeaf
            ( 
                 ( fun () -> this._GetFileInfoVersion() ),
                 ( fun (inputCoordinate, crsIdentifierForOutputCoordinateSystem) -> this._TransformToCoordinateStrategy(inputCoordinate, crsIdentifierForOutputCoordinateSystem) )
            )

        override this.Equals(o) =
            // the base implementation will return true
            // if the same type, and currently that is 
            // ok for this subimplementation
            // but the method is put here anyway with this comment 
            // as a reminder that if thic class would become 
            // configurable then update the implementation of this method 
            // (for example as in the subclass 'CrsTransformationAdapterProjNet4GeoAPI')
            base.Equals(o)

        // added the below method to get rid of the warning
        override this.GetHashCode() = base.GetHashCode()

        member private this._TransformToCoordinateStrategy(inputCoordinate, crsIdentifierForOutputCoordinateSystem) = 
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

        override this.AdapteeType =
            CrsTransformationAdapteeType.LEAF_DOT_SPATIAL_2_0_0_RC1

        // it is not a problem to hardcode the name below 
        // if the type name would become renamed since a 
        // renaming would then be detected by test code which 
        // verfifies that this short name actually is the 
        // suffix of the type name
        override this.ShortNameOfImplementation = "DotSpatial"

        member private this._GetFileInfoVersion() =
            FileInfoVersion.GetFileInfoVersionHelper(typeof<ProjectionInfo>)

    end

(*
https://github.com/DotSpatial/DotSpatial
https://www.nuget.org/packages/DotSpatial.Projections
*)