namespace Programmerare.CrsTransformations.CompositeTransformations

open Programmerare.CrsTransformations

type CrsTransformationAdapterWeightFactory private
    (
        crsTransformationAdapterLeafFactory: CrsTransformationAdapterLeafFactory
    ) = 

    ///<param name="crsTransformationAdapterClassName">
    ///crsTransformationAdapterClassName the full class name (i.e. including the package name)
    ///of a class which must implement the interface CrsTransformationAdapter
    ///</param>
    ///<param name="weight">
    ///the relative weight value to assign for the adapter specified by the string parameter 
    ///</param>
    member this.CreateFromStringWithFullClassNameForImplementation
        (
            crsTransformationAdapterClassName: string,
            weight: double
        ): CrsTransformationAdapterWeight =
        let crsTransformationAdapter = crsTransformationAdapterLeafFactory.GetCrsTransformationAdapter(crsTransformationAdapterClassName)
        CrsTransformationAdapterWeight._Create(crsTransformationAdapter, weight)

    ///<param name="crsTransformationAdapter">
    ///an object implementing the interface ICrsTransformationAdapter
    ///</param>    
    ///<param name="weight">
    ///the relative weight value to assign for the adapter specified by the adapter parameter 
    ///</param>
    member this.CreateFromInstance
        (
            crsTransformationAdapter: ICrsTransformationAdapter,
            weight: double
        ): CrsTransformationAdapterWeight =
        CrsTransformationAdapterWeight._Create(crsTransformationAdapter, weight)

    static member Create
        (
            crsTransformationAdapterLeafFactory: CrsTransformationAdapterLeafFactory
        ) =
        CrsTransformationAdapterWeightFactory(
            crsTransformationAdapterLeafFactory
        )


    static member Create() =
        CrsTransformationAdapterWeightFactory(
            CrsTransformationAdapterLeafFactory.Create()
        )