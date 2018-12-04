namespace Programmerare.CrsTransformations
open System
open System.Collections.Generic
open System.Reflection
open System.Linq
(*
Copyright (c) Tomas Johansson , http://programmerare.com
The code in the "Core" project is licensed with MIT.
Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
Please find more information in the license file at the root directory of each subproject
(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")


 * Factory used by 'composites' for creating 'leaf' implementations available in runtime.
 * 
 * The factory uses reflection code to instantiate the implementations from strings with full class names.  
 * 
 * The reason for these string based instantiations is that the core library avoids 
 * statically predefined enforced dependencies to all leaf adapter implementations.  
 * 
 * Instead the users can choose which implementation(s) to use with NuGet dependencies.
 *)
[<AbstractClass>]
type CrsTransformationAdapterLeafFactory internal
    (
    ) = 
        abstract member GetClassNamesForAllKnownImplementations : unit -> IList<string>
        
        abstract member IsCrsTransformationAdapter : string -> bool // string=crsTransformationAdapterClassName

        abstract member GetInstancesOfAllKnownAvailableImplementations : unit -> IList<ICrsTransformationAdapter>

        abstract member CreateCrsTransformationAdapter : string -> ICrsTransformationAdapter // string=crsTransformationAdapterClassName

        static member internal ThrowExceptionWhenAdapterInstanceCouldNotBeReturned(crsTransformationAdapterClassName: string) = 
            let nameOfInterfaceThatShouldBeImplemented = typeof<ICrsTransformationAdapter>.FullName
            let message = "Failed to return an instance of a class with the name '" + crsTransformationAdapterClassName + "' . The parameter must be the name of an available class which implements the interface '" + nameOfInterfaceThatShouldBeImplemented + "'"
            invalidArg "crsTransformationAdapterClassName" message

        static member Create() =
            // TODO better names than suffix 1 and 2
            CrsTransformationAdapterLeafFactory1()
        static member Create(listOfCrsTransformationAdapters: IList<ICrsTransformationAdapter>) =
            CrsTransformationAdapterLeafFactory2(listOfCrsTransformationAdapters)
// --------------------------------------------------------------
and CrsTransformationAdapterLeafFactory1 internal
    (
    ) = 
    class
        inherit CrsTransformationAdapterLeafFactory()

        // The hardcoded strings below will NOT change often 
        // and if they do then it should be detected by failing 
        // tests in the C# class CrsTransformationAdapterLeafFactoryTest
        static let assemblyNamesForAllKnownImplementations = [
            "Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy"
            "Programmerare.CrsTransformations.Adapter.DotSpatial"
            "Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI"
        ]

        // The hardcoded strings below will NOT change often 
        // and if they do then it should be detected by failing 
        // tests in the C# class CrsTransformationAdapterLeafFactoryTest
        static let classNamesForAllKnownImplementations = [
            "Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy.CrsTransformationAdapterMightyLittleGeodesy"
            "Programmerare.CrsTransformations.Adapter.DotSpatial.CrsTransformationAdapterDotSpatial"
            "Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI.CrsTransformationAdapterProjNet4GeoAPI"
        ]

        static let assembliesWithAdapterImplementationsLazyLoaded: Lazy<IList<Assembly>> =
            lazy (
                let assemblies = List<Assembly>()
                for assemblyName in assemblyNamesForAllKnownImplementations do
                    try
                        let assembly = Assembly.Load(assemblyName)
                        assemblies.Add(assembly)
                    with
                        | exc -> 
                            () // just keep on iterating and try with the next
                assemblies :> IList<Assembly>
            )

        static let typesWithAdapterImplementationsLazyLoaded: Lazy<IList<Type>> =
            lazy (
                let types = new List<Type>()
                let assemblies = assembliesWithAdapterImplementationsLazyLoaded.Force()
                for className in classNamesForAllKnownImplementations do
                    let mutable theType: Type = null
                    for assembly in assemblies do
                        try
                            theType <- assembly.GetType(className)
                            if not(isNull theType) then
                                types.Add(theType)
                        with
                            exc ->
                                () // just keep on iterating and try with the next
                types :> IList<Type>
            )

        static let GetTypesWithAdapterImplementations(): IList<Type> =
            typesWithAdapterImplementationsLazyLoaded.Force()

        static let GetTypeInstanceForClassName(className: string): Type =
            let theTypes = GetTypesWithAdapterImplementations()
            let mutable theType: Type = null
            for oneType in theTypes do
                if oneType.FullName.Equals(className) then
                    theType <- oneType
            theType

        (*
         * @param crsTransformationAdapterClassName the full class name (i.e. including the namespace)
         *      of a class which must implement the interface CrsTransformationAdapter
         * @return an instance if it could be created but otherwise an exception      
         *)
        static member private _CreateCrsTransformationAdapter(crsTransformationAdapterClassName: string): ICrsTransformationAdapter =
            try
                let theType = GetTypeInstanceForClassName(crsTransformationAdapterClassName)
                if isNull theType then
                    raise (Exception("The type could not be found: " + crsTransformationAdapterClassName))
                let instance = Activator.CreateInstance(theType)
                instance :?> ICrsTransformationAdapter
            with
                | exc -> 
                    CrsTransformationAdapterLeafFactory.ThrowExceptionWhenAdapterInstanceCouldNotBeReturned(crsTransformationAdapterClassName)

        static member private instancesOfAllKnownAvailableImplementationsLazyLoaded: Lazy<List<ICrsTransformationAdapter>> = 
            lazy (
                let list = new List<ICrsTransformationAdapter>()
                for className in classNamesForAllKnownImplementations do
                    list.Add(CrsTransformationAdapterLeafFactory1._CreateCrsTransformationAdapter(className))
                list
            )

        override x.CreateCrsTransformationAdapter(crsTransformationAdapterClassName: string): ICrsTransformationAdapter =
            CrsTransformationAdapterLeafFactory1._CreateCrsTransformationAdapter(crsTransformationAdapterClassName)

        (*
        * @return a list of instances for all known leaf implementations 
        *      of the adapter interface, which are available at the class path.
        *)    
        override x.GetInstancesOfAllKnownAvailableImplementations() = 
            CrsTransformationAdapterLeafFactory1.instancesOfAllKnownAvailableImplementationsLazyLoaded.Force() :> IList<ICrsTransformationAdapter>

        (*
         * @param the full class name (i.e. including the package name)
         *      of a class which must implement the interface ICrsTransformationAdapter
         * @return true if it possible to create an instance from the input string 
         *)
        override x.IsCrsTransformationAdapter(crsTransformationAdapterClassName: string): bool = 
            CrsTransformationAdapterLeafFactory1.instancesOfAllKnownAvailableImplementationsLazyLoaded.Force().Exists(
                fun i -> i.GetType().FullName.Equals(crsTransformationAdapterClassName)
            )

        override x.GetClassNamesForAllKnownImplementations() = 
            classNamesForAllKnownImplementations.ToList() :> IList<string>

    end
// --------------------------------------------------------------
and CrsTransformationAdapterLeafFactory2 internal
    (
        listOfCrsTransformationAdapters: IList<ICrsTransformationAdapter>
    ) = 
    class
        inherit CrsTransformationAdapterLeafFactory()

        let _classNamesForAllImplementationsLazyLoaded: Lazy<IList<string>> =
            lazy (
                let list =  listOfCrsTransformationAdapters.Select(
                                fun a -> a.GetType().FullName
                            ).ToList()
                list :> IList<string>
            )

        // TODO rename the method in the base class since "Create"
        // indicates a NEW instance, so something like "Get" 
        // might be a more general and better semantic  
        // which is also applicable for this implementation
        override x.CreateCrsTransformationAdapter(crsTransformationAdapterClassName: string): ICrsTransformationAdapter =
            let adapterInstance = listOfCrsTransformationAdapters.First(fun a -> a.GetType().FullName.Equals(crsTransformationAdapterClassName))
            if(not(isNull adapterInstance)) then
                adapterInstance
            else
                CrsTransformationAdapterLeafFactory.ThrowExceptionWhenAdapterInstanceCouldNotBeReturned(crsTransformationAdapterClassName)

        (*
        * @return a list of instances for all known leaf implementations 
        *      of the adapter interface, which are available at the class path.
        *)    
        override x.GetInstancesOfAllKnownAvailableImplementations() = 
            listOfCrsTransformationAdapters

        (*
         * @param the full class name (i.e. including the package name)
         *      of a class which must implement the interface ICrsTransformationAdapter
         * @return true if it possible to create an instance from the input string 
         *)
        override x.IsCrsTransformationAdapter(crsTransformationAdapterClassName: string): bool = 
            let adapterInstance = listOfCrsTransformationAdapters.FirstOrDefault(fun a -> a.GetType().FullName.Equals(crsTransformationAdapterClassName))
            // TODO refactor the above (duplicated row in this class)
            not(isNull adapterInstance)

        // TODO rename the method in the base class since "Known"
        // is not very appropriate semantic for this implementation
        override x.GetClassNamesForAllKnownImplementations() = 
            _classNamesForAllImplementationsLazyLoaded.Force()
    end
// --------------------------------------------------------------