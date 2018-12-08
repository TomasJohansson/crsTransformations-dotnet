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
type internal CompositeStrategyForAverageValue private
    (
        crsTransformationAdapters : IList<ICrsTransformationAdapter>
    ) =

    class
        inherit CompositeStrategyBase(crsTransformationAdapters)

        override this._EqualsWhenTypeAndSameLeafsHaveBeenChecked(compositeStrategy: CompositeStrategyBase) =
            // No further check is neeed than being the 
            // same type and the same leafs
            // (configured the same way, but that is generally checked in the base class)
            true 

        override this._GetAdapteeType() : CrsTransformationAdapteeType =
            CrsTransformationAdapteeType.COMPOSITE_AVERAGE

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
                            fun (r: CrsTransformationResultStatistic) -> r.CoordinateAverage
                        )

            override this._GetAdapteeType() : CrsTransformationAdapteeType = this._GetAdapteeType()

        (*
        * This method is not intended for public use,
        * but instead the factory class should be used.
        *)
        static member internal _CreateCompositeStrategyForAverageValue
            (
                adapters: IList<ICrsTransformationAdapter>
            ) =
                CompositeStrategyForAverageValue(adapters)
    end