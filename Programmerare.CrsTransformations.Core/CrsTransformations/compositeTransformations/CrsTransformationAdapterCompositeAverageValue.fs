namespace Programmerare.CrsTransformations.CompositeTransformations

open System.Collections.Generic
open Programmerare.CrsTransformations

type CrsTransformationAdapterCompositeAverageValue private
    (
        adapters: IList<ICrsTransformationAdapter>
    ) =
    class
        inherit CrsTransformationAdapterComposite
            (
                CompositeStrategyForAverageValue._CreateCompositeStrategyForAverageValue(adapters)
            )
        static member Create
            (
                adapters: IList<ICrsTransformationAdapter>
            ) = 
            CrsTransformationAdapterCompositeAverageValue(adapters)
    end