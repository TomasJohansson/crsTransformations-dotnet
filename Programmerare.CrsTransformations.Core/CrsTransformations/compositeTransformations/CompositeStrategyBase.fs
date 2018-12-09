namespace Programmerare.CrsTransformations.CompositeTransformations
open System
open System.Linq
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
[<AbstractClass>]
type internal CompositeStrategyBase internal
    (
        crsTransformationAdapters : IList<ICrsTransformationAdapter>
    ) =
    class

        do
            let argumentName = "crsTransformationAdapters"
            if isNull crsTransformationAdapters then
                nullArg argumentName
            if crsTransformationAdapters.Count = 0 then
                invalidArg argumentName "'Composite' adapter can not be created with an empty list of 'leaf' adapters"
            // Verify that there were not any duplicates 
            // in the constructor parameter list:
            let grouped = crsTransformationAdapters.GroupBy(fun a -> a.AdapteeType)
            for group in grouped do
                let countAdapter = group.Count()
                if(countAdapter > 1) then
                    invalidArg argumentName ("Duplicates not allowed in the parameter with a list of adapters but there were " + countAdapter.ToString() + " of the adapter " + group.Key.ToString())

        abstract member _GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked : unit -> IList<ICrsTransformationAdapter>
        default this._GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked() = crsTransformationAdapters

        abstract member _GetAdapteeType : unit -> CrsTransformationAdapteeType
        default this._GetAdapteeType() = CrsTransformationAdapteeType.UNSPECIFIED_COMPOSITE

        abstract member _EqualsWhenTypeAndSameLeafsHaveBeenChecked : CompositeStrategyBase -> bool

        // precondition: the number of leafs must be the same
        member private this.AreLeafsEqual(that: CompositeStrategyBase) = 
            let thisLeafs = this._GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked()
            let thatLeafs = that._GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked()
            let mutable areLeafsEqual = true
            for thisLeaf in thisLeafs do
                let thatLeaf = thatLeafs.FirstOrDefault(fun a -> a.LongNameOfImplementation.Equals(thisLeaf.LongNameOfImplementation))
                if(isNull thatLeaf) then
                    areLeafsEqual <- false    
                elif(not(thatLeaf.Equals(thisLeaf))) then
                    areLeafsEqual <- false   
            areLeafsEqual

        override this.Equals(o) =
            if isNull o then
                false
            elif(o :? CompositeStrategyBase) then
                let that = o :?> CompositeStrategyBase
                if(that._GetAdapteeType() <> this._GetAdapteeType()) then
                    false
                elif(that._GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked().Count <> this._GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked().Count) then
                    false
                else
                    if(not(this.AreLeafsEqual(that))) then
                        false
                    else
                        // Now we know that "this" and "that"
                        // contain the same leafs, 
                        // and not only the same number of leafs.
                        // Therefore:
                        this._EqualsWhenTypeAndSameLeafsHaveBeenChecked(that)
            else
                false

        // added the below method to get rid of the warning
        override this.GetHashCode() = this._GetAdapteeType().GetHashCode()

        (*
        * This base class method is reusable from both subclasses
        * that calculates the median or average which is provided with the last
        * function parameter of the method
        *)
        member this._CalculateAggregatedResultBase
            (
                allResults: IList<CrsTransformationResult>,
                inputCoordinate: CrsCoordinate,
                crsTransformationAdapterThatCreatedTheResult: ICrsTransformationAdapter,
                crsTransformationResultStatistic: CrsTransformationResultStatistic,
                medianOrAverage: CrsTransformationResultStatistic -> CrsCoordinate

            ) : CrsTransformationResult =
                if crsTransformationResultStatistic.IsStatisticsAvailable then
                    let coordRes: CrsCoordinate = medianOrAverage(crsTransformationResultStatistic)
                    CrsTransformationResult._CreateCrsTransformationResult(
                        inputCoordinate,
                        outputCoordinate = coordRes,
                        exceptionOrNull = null,
                        isSuccess = true,
                        crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                        nullableCrsTransformationResultStatistic = crsTransformationResultStatistic
                    )
                else
                    CrsTransformationResult._CreateCrsTransformationResult(
                        inputCoordinate,
                        outputCoordinate = null,
                        exceptionOrNull = null,
                        isSuccess = false,
                        crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                        nullableCrsTransformationResultStatistic = crsTransformationResultStatistic
                    )
        
        interface ICompositeStrategy with
    
            member this._GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked() =
                this._GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked()

            member this._GetAdapteeType() = 
                this._GetAdapteeType()

            member this._CalculateAggregatedResult
                (
                    allResults: IList<CrsTransformationResult>,
                    inputCoordinate: CrsCoordinate,
                    crsIdentifierForOutputCoordinateSystem: CrsIdentifier,
                    crsTransformationAdapterThatCreatedTheResult: ICrsTransformationAdapter
                ): CrsTransformationResult = raise (System.NotImplementedException())

            member this._ShouldContinueIterationOfAdaptersToInvoke(crsTransformationResult) = raise (System.NotImplementedException())

    end