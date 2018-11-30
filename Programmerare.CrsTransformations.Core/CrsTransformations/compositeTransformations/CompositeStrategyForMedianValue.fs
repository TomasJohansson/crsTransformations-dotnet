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
type CompositeStrategyForMedianValue internal
    (
        crsTransformationAdapters : IList<ICrsTransformationAdapter>
    ) =

    class
        inherit CompositeStrategyBase(crsTransformationAdapters)

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
                    let resultsStatistic = CrsTransformationResultStatistic._CreateCrsTransformationResultStatistic(allResults)
                    base._CalculateAggregatedResultBase
                        (
                            allResults,
                            inputCoordinate,
                            crsTransformationAdapterThatCreatedTheResult,
                            resultsStatistic,
                            fun (r: CrsTransformationResultStatistic) -> r.CoordinateMedian
                        )

            override this._GetAdapteeType() : CrsTransformationAdapteeType =
                CrsTransformationAdapteeType.COMPOSITE_MEDIAN

        (*
        * This method is not intended for public use,
        * but instead the factory class should be used.
        *)
        static member _CreateCompositeStrategyForMedianValue
            (
                adapters: IList<ICrsTransformationAdapter>
            ) =
                CompositeStrategyForMedianValue(adapters)
    end