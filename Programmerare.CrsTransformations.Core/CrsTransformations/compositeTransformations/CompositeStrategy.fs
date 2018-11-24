namespace Programmerare.CrsTransformations.CompositeTransformations

open System.Collections.Generic
open Programmerare.CrsTransformations
open Programmerare.CrsTransformations.Coordinate
open Programmerare.CrsTransformations.Identifier

// TODO: rewrite comments below for .NET ...

(*
 * Kotlin "internal" interface. 
 * The methods are not intended for public use from client code.
 *
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-adapter-core" project is licensed with MIT.
 * Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
 * Please find more information in the license file at the root directory of each subproject
 * (e.g. the subprojects "crs-transformation-adapter-impl-geotools" , "crs-transformation-adapter-impl-proj4j" and so on)
 *)
type ICompositeStrategy =
    interface

        (*
         * The method is not intended for public use from client code. 
         *)
        abstract member _GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked : unit -> IList<ICrsTransformationAdapter>

        (*
         * The method is not intended for public use from client code.
         *)    
        abstract member _ShouldContinueIterationOfAdaptersToInvoke : CrsTransformationResult -> bool

        (*
         * The method is not intended for public use from client code.
         *)    
        abstract member _CalculateAggregatedResult : 
            IList<CrsTransformationResult> * // allResults
            CrsCoordinate * // inputCoordinate
            CrsIdentifier * // crsIdentifierForOutputCoordinateSystem
            ICrsTransformationAdapter // crsTransformationAdapterThatCreatedTheResult
                -> CrsTransformationResult

        (*
         * The method is not intended for public use from client code.
         *)    
        abstract member _GetAdapteeType : unit -> CrsTransformationAdapteeType
    end