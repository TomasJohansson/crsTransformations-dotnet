namespace Programmerare.CrsTransformations.CompositeTransformations

open System.Collections.Generic
open Programmerare.CrsTransformations
open Programmerare.CrsTransformations.Coordinate
open Programmerare.CrsTransformations.Identifier

// TODO: rewrite comments below for .NET ...

(*
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-adapter-core" project is licensed with MIT.
 * Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
 * Please find more information in the license file at the root directory of each subproject
 * (e.g. the subprojects "crs-transformation-adapter-impl-geotools" , "crs-transformation-adapter-impl-proj4j" and so on)
 *)
type CompositeStrategyForFirstSuccess internal
    (
        crsTransformationAdapters : IList<ICrsTransformationAdapter>
    ) =

    class
        inherit CompositeStrategyBase(crsTransformationAdapters)


        interface ICompositeStrategy with
            override this._ShouldContinueIterationOfAdaptersToInvoke(lastResultOrNullIfNoPrevious: CrsTransformationResult): bool = 
                not(lastResultOrNullIfNoPrevious.IsSuccess)

            override this._CalculateAggregatedResult
                (
                    allResults: IList<CrsTransformationResult>,
                    inputCoordinate: CrsCoordinate,
                    crsIdentifierForOutputCoordinateSystem: CrsIdentifier,
                    crsTransformationAdapterThatCreatedTheResult: ICrsTransformationAdapter
                ): CrsTransformationResult =
                if(allResults.Count = 1 && allResults.[0].IsSuccess) then
                    // there should never be more than one result with the FirstSuccess implementation
                    // since the calculation is interrupted at the first succeful result
                    CrsTransformationResult._CreateCrsTransformationResult(
                        inputCoordinate,
                        outputCoordinate = allResults.[0].OutputCoordinate,
                        exceptionOrNull = null,
                        isSuccess = true,
                        crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                        nullableCrsTransformationResultStatistic = CrsTransformationResultStatistic(allResults)
                    )
                else
                    CrsTransformationResult._CreateCrsTransformationResult(
                        inputCoordinate,
                        outputCoordinate = null,
                        exceptionOrNull = null,
                        isSuccess = false,
                        crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                        nullableCrsTransformationResultStatistic = CrsTransformationResultStatistic(allResults)
                    )

            override this._GetAdapteeType() : CrsTransformationAdapteeType =
                CrsTransformationAdapteeType.COMPOSITE_FIRST_SUCCESS

        //    (*
        //     * This method is not intended for public use, 
        //     * but instead the factory class should be used.
        //     * @see CrsTransformationAdapterCompositeFactory
        //     *)
        //    @JvmStatic
        static member _CreateCompositeStrategyForFirstSuccess
            (
                adapters: IList<ICrsTransformationAdapter>
            ) =
                CompositeStrategyForFirstSuccess(adapters)

    end