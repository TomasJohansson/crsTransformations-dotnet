namespace Programmerare.CrsTransformations.CompositeTransformations

open System.Collections.Generic

type CrsTransformationAdapterCompositeWeightedAverage private
    (
        weightedCrsTransformationAdapters: IList<CrsTransformationAdapterWeight>
    ) =
    class
        inherit CrsTransformationAdapterComposite
            (
                CompositeStrategyWeightedAverage._CreateCompositeStrategyWeightedAverage(weightedCrsTransformationAdapters)
            )
        static member Create
            (
                weightedCrsTransformationAdapters: IList<CrsTransformationAdapterWeight>
            ) = 
            CrsTransformationAdapterCompositeWeightedAverage(weightedCrsTransformationAdapters)
    end