namespace Programmerare.CrsTransformations.CompositeTransformations

open System.Collections.Generic
open Programmerare.CrsTransformations

type CrsTransformationAdapterCompositeAverage private
    (
        adapters: IList<ICrsTransformationAdapter>
    ) =
    class
        inherit CrsTransformationAdapterComposite
            (
                CompositeStrategyAverage._CreateCompositeStrategyAverage(adapters)
            )
        static member Create
            (
                adapters: IList<ICrsTransformationAdapter>
            ) = 
            CrsTransformationAdapterCompositeAverage(adapters)
    end