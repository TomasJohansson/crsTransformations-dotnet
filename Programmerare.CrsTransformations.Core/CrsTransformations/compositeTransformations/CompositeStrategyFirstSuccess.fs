namespace Programmerare.CrsTransformations.CompositeTransformations
open System.Collections.Generic
open Programmerare.CrsTransformations
open Programmerare.CrsTransformations.Coordinate
open Programmerare.CrsTransformations.Identifier
(*
Copyright (c) Tomas Johansson , http://programmerare.com
The code in the "Core" project is licensed with MIT.
Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
Please find more information in the license file at the root directory of each subproject
(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")
 *)
type internal CompositeStrategyFirstSuccess private
    (
        crsTransformationAdapters : IList<ICrsTransformationAdapter>
    ) =

    class
        inherit CompositeStrategyBase(crsTransformationAdapters)

        override this._EqualsWhenTypeAndSameLeafsHaveBeenChecked(compositeStrategy: CompositeStrategyBase) =
            let thisChildren = this._GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked();
            let thatChildren = compositeStrategy._GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked();
            let mutable i = 0
            let mutable areEqual = true
            // The order of the leafs might be different
            // and the order is relevant for "FirstSuccess"
            while(i < thisChildren.Count) do
                if(thisChildren.[i].Equals(thatChildren.[i])) then
                    i <- i + 1
                else
                    areEqual <- false
                    i <- thisChildren.Count // to stop the while-loop iteration
            areEqual

        override this._GetAdapteeType() : CrsTransformationAdapteeType =
            CrsTransformationAdapteeType.COMPOSITE_FIRST_SUCCESS

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
                        nullableCrsTransformationResultStatistic = CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(allResults)
                    )
                else
                    CrsTransformationResult._CreateCrsTransformationResult(
                        inputCoordinate,
                        outputCoordinate = null,
                        exceptionOrNull = null,
                        isSuccess = false,
                        crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                        nullableCrsTransformationResultStatistic = CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(allResults)
                    )

            override this._GetAdapteeType() : CrsTransformationAdapteeType = this._GetAdapteeType()

        (*
        * This method is not intended for public use, 
        * but instead the factory class should be used.
        * @see CrsTransformationAdapterCompositeFactory
        *)
        static member internal _CreateCompositeStrategyFirstSuccess
            (
                adapters: IList<ICrsTransformationAdapter>
            ) =
                CompositeStrategyFirstSuccess(adapters)

    end