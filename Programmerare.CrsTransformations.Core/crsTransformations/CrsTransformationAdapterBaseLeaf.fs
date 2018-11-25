namespace Programmerare.CrsTransformations

open System.Collections.Generic
open Programmerare.CrsTransformations.Coordinate
open Programmerare.CrsTransformations.Identifier

(*
 * Base class for the 'leaf' adapters.
 * @see CrsTransformationAdapterBase
 * @see CrsTransformationAdapter
 *
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-adapter-core" project is licensed with MIT.
 * Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
 * Please find more information in the license file at the root directory of each subproject
 * (e.g. the subprojects "crs-transformation-adapter-impl-geotools" , "crs-transformation-adapter-impl-proj4j" and so on)
 *)
[<AbstractClass>]
type CrsTransformationAdapterBaseLeaf() = // CrsTransformationAdapterBase() =
    class
        inherit CrsTransformationAdapterBase()

        abstract _TransformToCoordinateHookLeaf : CrsCoordinate * CrsIdentifier -> CrsCoordinate

        override this._TransformHook(inputCoordinate: CrsCoordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): CrsTransformationResult =
            try
                let outputCoordinate = this._TransformToCoordinateHookLeaf(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
                CrsTransformationResult(
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
                    CrsTransformationResult(
                        inputCoordinate,
                        null,
                        exc,
                        false,
                        this,
                        CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(new List<CrsTransformationResult>())
                    )

                //if true then
                //    //java.lang.Double.isNaN(outputCoordinate.yNorthingLatitude)
                //    //||
                //    //java.lang.Double.isNaN(outputCoordinate.xEastingLongitude)
                // .NET :
                //System.Double.IsInfinity
                //System.Double.IsNaN
                //System.Double.IsNegativeInfinity
                //System.Double.IsPositiveInfinity
                    
                //    return CrsTransformationResult._createCrsTransformationResult(
                //        inputCoordinate,
                //        null,
                //        exception = null, 
                //        isSuccess = false,
                //        crsTransformationAdapterResultSource = this
                //    )
                //}
                //else {
                //    return CrsTransformationResult._createCrsTransformationResult(
                //        inputCoordinate,
                //        outputCoordinate,
                //        exception = null,
                //        isSuccess = outputCoordinate != null,
                //        crsTransformationAdapterResultSource = this

            //catch (e: Throwable) {
            //    return CrsTransformationResult._createCrsTransformationResult(
            //        inputCoordinate,
            //        null,
            //        exception = e,
            //        isSuccess = false,
            //        crsTransformationAdapterResultSource = this
            //    )
            //}
        

        //override this._GetTransformationAdapterChildren(): List<CrsTransformationAdapter> {
        //    return listOf<CrsTransformationAdapter>()
        //}

        //override final fun isComposite(): Boolean {
        //    return false
        //}

        override this.AdapteeType =
            // Should be overridden by subclasses
            CrsTransformationAdapteeType.UNSPECIFIED_LEAF

        override this.IsComposite = false

        // empty list of children adapters for leafs:
        override this.GetTransformationAdapterChildren() =
            new List<ICrsTransformationAdapter>() :> IList<ICrsTransformationAdapter>

    end