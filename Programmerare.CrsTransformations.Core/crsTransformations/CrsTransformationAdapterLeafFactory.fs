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
        abstract member GetClassNamesForAllImplementations : unit -> IList<string>
        
        abstract member IsCrsTransformationAdapter : string -> bool // string=crsTransformationAdapterClassName

        abstract member GetInstancesOfAllImplementations : unit -> IList<ICrsTransformationAdapter>

        abstract member GetCrsTransformationAdapter : string -> ICrsTransformationAdapter // string=crsTransformationAdapterClassName

        static member internal ThrowExceptionWhenAdapterInstanceCouldNotBeReturned(crsTransformationAdapterClassName: string) = 
            let nameOfInterfaceThatShouldBeImplemented = typeof<ICrsTransformationAdapter>.FullName
            let message = "Failed to return an instance of a class with the name '" + crsTransformationAdapterClassName + "' . The parameter must be the name of an available class which implements the interface '" + nameOfInterfaceThatShouldBeImplemented + "'"
            invalidArg "crsTransformationAdapterClassName" message

        static member Create() =
            CrsTransformationAdapterLeafFactoryWithHardcodedImplementations() :> CrsTransformationAdapterLeafFactory
        static member Create(listOfCrsTransformationAdapters: IList<ICrsTransformationAdapter>) =
            CrsTransformationAdapterLeafFactoryWithConfiguredImplementations(listOfCrsTransformationAdapters) :> CrsTransformationAdapterLeafFactory
// --------------------------------------------------------------
and internal CrsTransformationAdapterLeafFactoryWithHardcodedImplementations internal
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
        member private this._CreateCrsTransformationAdapter(crsTransformationAdapterClassName: string): ICrsTransformationAdapter =
            let theType = GetTypeInstanceForClassName(crsTransformationAdapterClassName)
            if isNull theType then
                CrsTransformationAdapterLeafFactory.ThrowExceptionWhenAdapterInstanceCouldNotBeReturned(crsTransformationAdapterClassName)
            this._CreateInstanceOfType(theType)

        member private this._CreateInstanceOfType(theType: Type): ICrsTransformationAdapter =
            if isNull theType then
                nullArg "theType"
            try
                let instance = Activator.CreateInstance(theType)
                instance :?> ICrsTransformationAdapter
            with
                | exc -> 
                    CrsTransformationAdapterLeafFactory.ThrowExceptionWhenAdapterInstanceCouldNotBeReturned(theType.FullName)

        member private this.instancesOfAllKnownAvailableImplementationsLazyLoaded: Lazy<List<ICrsTransformationAdapter>> = 
            lazy (
                let list = new List<ICrsTransformationAdapter>()
                let theTypes = typesWithAdapterImplementationsLazyLoaded.Force()
                for typeWithAdapterImplementation in theTypes do
                    list.Add(this._CreateInstanceOfType(typeWithAdapterImplementation))
                list
            )

        override this.GetCrsTransformationAdapter(crsTransformationAdapterClassName: string): ICrsTransformationAdapter =
            this._CreateCrsTransformationAdapter(crsTransformationAdapterClassName)

        (*
        * @return a list of instances for all known leaf implementations 
        *      of the adapter interface, which are available at the class path.
        *)    
        override this.GetInstancesOfAllImplementations() = 
            this.instancesOfAllKnownAvailableImplementationsLazyLoaded.Force() :> IList<ICrsTransformationAdapter>

        (*
         * @param the full class name (i.e. including the package name)
         *      of a class which must implement the interface ICrsTransformationAdapter
         * @return true if it possible to create an instance from the input string 
         *)
        override this.IsCrsTransformationAdapter(crsTransformationAdapterClassName: string): bool = 
            this.instancesOfAllKnownAvailableImplementationsLazyLoaded.Force().Exists(
                fun i -> i.GetType().FullName.Equals(crsTransformationAdapterClassName)
            )

        override this.GetClassNamesForAllImplementations() = 
            classNamesForAllKnownImplementations.ToList() :> IList<string>

    end
// --------------------------------------------------------------
and internal CrsTransformationAdapterLeafFactoryWithConfiguredImplementations internal
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

        override x.GetCrsTransformationAdapter(crsTransformationAdapterClassName: string): ICrsTransformationAdapter =
            let adapterInstance = listOfCrsTransformationAdapters.First(fun a -> a.GetType().FullName.Equals(crsTransformationAdapterClassName))
            if(not(isNull adapterInstance)) then
                adapterInstance
            else
                CrsTransformationAdapterLeafFactory.ThrowExceptionWhenAdapterInstanceCouldNotBeReturned(crsTransformationAdapterClassName)

        (*
        * @return a list of instances for all known leaf implementations 
        *      of the adapter interface, which are available at the class path.
        *)    
        
        override x.GetInstancesOfAllImplementations() = 
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
            
        override x.GetClassNamesForAllImplementations() = 
            _classNamesForAllImplementationsLazyLoaded.Force()
    end
// --------------------------------------------------------------