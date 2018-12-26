namespace Programmerare.CrsTransformations.CompositeTransformations

open System.Collections.Generic
open Programmerare.CrsTransformations

type CrsTransformationAdapterCompositeFirstSuccess private
    (
        adapters: IList<ICrsTransformationAdapter>
    ) =
    class
        inherit CrsTransformationAdapterComposite
            (
                CompositeStrategyForFirstSuccess._CreateCompositeStrategyForFirstSuccess(adapters)
            )
        static member Create
            (
                adapters: IList<ICrsTransformationAdapter>
            ) = 
            CrsTransformationAdapterCompositeFirstSuccess(adapters)
    end