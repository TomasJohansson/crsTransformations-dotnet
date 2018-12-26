namespace Programmerare.CrsTransformations.CompositeTransformations

open System.Collections.Generic

type CrsTransformationAdapterCompositeWeightedAverageValue private
    (
        weightedCrsTransformationAdapters: IList<CrsTransformationAdapterWeight>
    ) =
    class
        inherit CrsTransformationAdapterComposite
            (
                CompositeStrategyForWeightedAverageValue._CreateCompositeStrategyForWeightedAverageValue(weightedCrsTransformationAdapters)
            )
        static member Create
            (
                weightedCrsTransformationAdapters: IList<CrsTransformationAdapterWeight>
            ) = 
            CrsTransformationAdapterCompositeWeightedAverageValue(weightedCrsTransformationAdapters)
    end