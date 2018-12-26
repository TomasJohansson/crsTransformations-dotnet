namespace Programmerare.CrsTransformations.CompositeTransformations

open System.Collections.Generic
open Programmerare.CrsTransformations

type CrsTransformationAdapterCompositeMedian private
    (
        adapters: IList<ICrsTransformationAdapter>
    ) =
    class
        inherit CrsTransformationAdapterComposite
            (
                CompositeStrategyMedian._CreateCompositeStrategyMedian(adapters)
            )
        static member Create
            (
                adapters: IList<ICrsTransformationAdapter>
            ) = 
            CrsTransformationAdapterCompositeMedian(adapters)
    end