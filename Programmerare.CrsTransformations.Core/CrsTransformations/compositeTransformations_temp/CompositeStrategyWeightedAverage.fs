namespace Programmerare.CrsTransformations.CompositeTransformations

open System.Collections.Generic
open System.Linq
open System
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
type internal CompositeStrategyWeightedAverage private
    (
        crsTransformationAdapters : IList<ICrsTransformationAdapter>,
        weights: IDictionary<CrsTransformationAdapteeType, double>
    ) =

    class
        inherit CompositeStrategyBase(crsTransformationAdapters)

        [<Literal>]  // the actual value below is fairly arbitrary
        static let SMALL_DELTA_VALUE_FOR_EQUAL_COMPARISON_OF_WEIGHT_VALUES = 0.0000000001
        
        do
            if isNull crsTransformationAdapters then
                nullArg "crsTransformationAdapters"
            if isNull weights then 
                nullArg "weights"
            if crsTransformationAdapters.Count <> weights.Count then
                invalidArg "crsTransformationAdapters" "The number of adapters must be the same as the number of weights"
            for crsTransformationAdapter in crsTransformationAdapters do
                if not((weights.ContainsKey(crsTransformationAdapter.AdapteeType))) then
                    invalidArg "crsTransformationAdapters" ("No weight for adapter " + crsTransformationAdapter.LongNameOfImplementation)

        member internal this._GetWeights() = weights

        override this._EqualsWhenTypeAndSameLeafsHaveBeenChecked(compositeStrategy: CompositeStrategyBase) =
            if(compositeStrategy :? CompositeStrategyWeightedAverage) then
                let that = compositeStrategy :?> CompositeStrategyWeightedAverage
                let thatWeights = that._GetWeights()
                let mutable areEqual = true
                for thatWeight in thatWeights do
                    if(this._GetWeights().ContainsKey(thatWeight.Key)) then
                        let thisValue = this._GetWeights().[thatWeight.Key]
                        let thatValue = thatWeight.Value
                        let diff = Math.Abs(thisValue-thatValue)
                        if(diff > SMALL_DELTA_VALUE_FOR_EQUAL_COMPARISON_OF_WEIGHT_VALUES) then
                            areEqual <- false
                    else
                        areEqual <- false
                areEqual
            else
                false

        override this._GetAdapteeType() : CrsTransformationAdapteeType =
            CrsTransformationAdapteeType.COMPOSITE_WEIGHTED_AVERAGE

        interface ICompositeStrategy with

            override this._ShouldContinueIterationOfAdaptersToInvoke(lastResultOrNullIfNoPrevious: CrsTransformationResult): bool = 
                true

            override this._CalculateAggregatedResult
                (
                    allResults: IList<CrsTransformationResult>,
                    inputCoordinate: CrsCoordinate,
                    crsIdentifierForOutputCoordinateSystem: CrsIdentifier,
                    crsTransformationAdapterThatCreatedTheResult: ICrsTransformationAdapter
                ): CrsTransformationResult =
                    let mutable successCount = 0
                    let mutable sumLat = 0.0
                    let mutable sumLon = 0.0
                    let mutable weightSum = 0.0
                    for res in allResults do
                        if res.IsSuccess then
                            let weight = if weights.ContainsKey(res.CrsTransformationAdapterResultSource.AdapteeType) then 
                                            weights.[res.CrsTransformationAdapterResultSource.AdapteeType] 
                                         else 
                                            -1.0
                            if weight < 0.0 then
                                invalidOp ("The implementation was not configured with a non-null and non-negative weight value for the implementation "+ res.CrsTransformationAdapterResultSource.LongNameOfImplementation)
                            successCount <- successCount + 1
                            weightSum <- weightSum + weight
                            let coord = res.OutputCoordinate
                            sumLat <- sumLat + weight * coord.YNorthingLatitude
                            sumLon <- sumLon + weight * coord.XEastingLongitude
                    if(successCount > 0) then
                        let avgLat = sumLat / weightSum
                        let avgLon = sumLon / weightSum
                        let coordRes = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(avgLat, avgLon, crsIdentifierForOutputCoordinateSystem)
                        CrsTransformationResult._CreateCrsTransformationResult
                            (
                                inputCoordinate,
                                outputCoordinate = coordRes,
                                exceptionOrNull = null,
                                isSuccess = true,
                                crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                                nullableCrsTransformationResultStatistic = CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(allResults)
                            )
                    else
                        CrsTransformationResult._CreateCrsTransformationResult
                            (
                                inputCoordinate,
                                outputCoordinate = null,
                                exceptionOrNull = null,
                                isSuccess = false,
                                crsTransformationAdapterResultSource = crsTransformationAdapterThatCreatedTheResult,
                                nullableCrsTransformationResultStatistic = CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(allResults)
                            )

            override this._GetAdapteeType() : CrsTransformationAdapteeType = this._GetAdapteeType()

        static member internal _CreateCompositeStrategyWeightedAverage
            (
                weightedCrsTransformationAdapters: IList<CrsTransformationAdapterWeight>
            ): CompositeStrategyWeightedAverage =
                let adapters = weightedCrsTransformationAdapters.Select(fun it -> it.CrsTransformationAdapter).ToList()
                let map = Dictionary<CrsTransformationAdapteeType, double>()
                for fw in weightedCrsTransformationAdapters do
                    // No need to check for negative weight values here since it 
                    // should be enforced already at construction with an exception being thrown
                    // if the below weight value would be non-positive 
                    map.Add(fw.CrsTransformationAdapter.AdapteeType, fw.Weight);
                CompositeStrategyWeightedAverage(adapters, map)

    end