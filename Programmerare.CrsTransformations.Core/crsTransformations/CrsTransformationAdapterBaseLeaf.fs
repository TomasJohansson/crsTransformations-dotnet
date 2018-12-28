namespace Programmerare.CrsTransformations

open System.Collections.Generic
open Programmerare.CrsTransformations.Coordinate
open Programmerare.CrsTransformations.Identifier
(*
Copyright (c) Tomas Johansson , http://programmerare.com
The code in the "Core" project is licensed with MIT.
Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
Please find more information in the license file at the root directory of each subproject
(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")
*)

///<summary>
///Base class for the 'leaf' adapters.
///</summary>
[<AbstractClass>]
type CrsTransformationAdapterBaseLeaf
    (
        functionReturningFileInfoVersion: unit -> FileInfoVersion,
        transformToCoordinateStrategyLeaf : CrsCoordinate * CrsIdentifier -> CrsCoordinate
    ) as this =
    class
        inherit CrsTransformationAdapterBase
            (
                functionReturningFileInfoVersion ,
                transformToCoordinateStrategyLeaf  ,
                fun (inputCoordinate, crsIdentifierForOutputCoordinateSystem) -> this._TransformStrategy(inputCoordinate, crsIdentifierForOutputCoordinateSystem) 
            )

        member private this._TransformStrategy(inputCoordinate: CrsCoordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): CrsTransformationResult =
            try
                base.ValidateCoordinateAndIdentifierNotNull(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
                let outputCoordinate = transformToCoordinateStrategyLeaf(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
                base.ValidateNonNullCoordinate(outputCoordinate)
                CrsTransformationResult._CreateCrsTransformationResult(
                    inputCoordinate,
                    outputCoordinate,
                    null,
                    true,
                    this,
                    CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(new List<CrsTransformationResult>())
                )
            with
                // | :? System.Exception as exc -> 
                // alternative to the above:
                | exc -> 
                    CrsTransformationResult._CreateCrsTransformationResult(
                        inputCoordinate,
                        null,
                        exc,
                        false,
                        this,
                        CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(new List<CrsTransformationResult>())
                    )

        override this.AdapteeType =
            // Should be overridden by subclasses
            CrsTransformationAdapteeType.UNSPECIFIED_LEAF

        override this.IsComposite = false

        // empty list of children adapters for leafs:
        override this.GetTransformationAdapterChildren() =
            new List<ICrsTransformationAdapter>() :> IList<ICrsTransformationAdapter>

    end