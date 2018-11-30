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

(*
 * Base class for the 'leaf' adapters.
*)
[<AbstractClass>]
type CrsTransformationAdapterBaseLeaf() =
    class
        inherit CrsTransformationAdapterBase()

        abstract _TransformToCoordinateHookLeaf : CrsCoordinate * CrsIdentifier -> CrsCoordinate

        override this._TransformHook(inputCoordinate: CrsCoordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): CrsTransformationResult =
            try
                let outputCoordinate = this._TransformToCoordinateHookLeaf(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
                // TODO implement a check with things as below to see if it was a failure
                // (if the above invoked implementation method would return things as below
                // instead of throwing exception... but this is probably not the best place 
                // but rather in a general way to apply the validation also 
                // in one place for all the leafs i.e. the hook method 
                // above can be invkoked indirectly without executing through 
                // this code and therefore it should be placed somewhere else)
                //if true then
                //    //java.lang.Double.isNaN(outputCoordinate.yNorthingLatitude)
                //    //||
                //    //java.lang.Double.isNaN(outputCoordinate.xEastingLongitude)
                // .NET :
                //System.Double.IsInfinity
                //System.Double.IsNaN
                //System.Double.IsNegativeInfinity
                //System.Double.IsPositiveInfinity
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