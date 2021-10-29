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
*)

///<summary>
///<para>
///Factory used by 'composites' for creating 'leaf' implementations available in runtime.
///</para>
///<para/>
///<para>
///The factory uses reflection code to instantiate the implementations from strings with full class names.  
///</para>
///<para/>
///<para>
///The reason for these string based instantiations is that the core library avoids 
///statically predefined enforced dependencies to all leaf adapter implementations.  
///</para>
///<para/>
///<para>
///Instead the users can choose which implementation(s) to use with NuGet dependencies.
///</para>
///</summary>
[<AbstractClass>]
type CrsTransformationAdapterLeafFactory internal
    (
    ) = 
        ///<returns>
        ///List of all complete (i.e. including namespace) class names for all implementations of the interface ICrsTransformationAdapter.
        ///</returns>
        abstract member GetClassNamesForAllImplementations : unit -> IList<string>

        ///<returns>
        ///true if the string parameter is the full class name (including the namespace) for a class implementing the interface ICrsTransformationAdapter.
        ///</returns>        
        abstract member IsCrsTransformationAdapter : string -> bool // string=crsTransformationAdapterClassName

        ///<returns>
        ///List with instaces of all implementations of the interface ICrsTransformationAdapter.
        ///</returns>
        abstract member GetInstancesOfAllImplementations : unit -> IList<ICrsTransformationAdapter>

        ///<returns>
        ///An instance of the class specified with the string parameter if that class implements the interface ICrsTransformationAdapter.
        ///</returns>        
        ///<exception cref="System.ArgumentException">
        ///Thrown if the string parameter is not specifying a class which implements the interface ICrsTransformationAdapter.
        ///</exception>
        abstract member GetCrsTransformationAdapter : string -> ICrsTransformationAdapter // string=crsTransformationAdapterClassName

        static member internal ThrowExceptionWhenAdapterInstanceCouldNotBeReturned(crsTransformationAdapterClassName: string) = 
            let nameOfInterfaceThatShouldBeImplemented = typeof<ICrsTransformationAdapter>.FullName
            let message = "Failed to return an instance of a class with the name '" + crsTransformationAdapterClassName + "' . The parameter must be the name of an available class which implements the interface '" + nameOfInterfaceThatShouldBeImplemented + "'"
            invalidArg "crsTransformationAdapterClassName" message

        ///<returns>
        ///A factory that is capable of creating some known hardcoded implementations of the interface ICrsTransformationAdapter.
        ///</returns>
        static member Create() =
            CrsTransformationAdapterLeafFactoryWithHardcodedImplementations() :> CrsTransformationAdapterLeafFactory

        ///<returns>
        ///A factory that is capable of creating those implementations of the interface ICrsTransformationAdapter which are provided in the method parameter list.
        ///</returns>
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
            "Programmerare.CrsTransformations.Adapter.ProjNet"
            //"Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI" // Obsolete (ProjNet 2 is the next version of ProjNet4GeoAPI 1.4)
        ]

        // The hardcoded strings below will NOT change often 
        // and if they do then it should be detected by failing 
        // tests in the C# class CrsTransformationAdapterLeafFactoryTest
        static let classNamesForAllKnownImplementations = [
            "Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy.CrsTransformationAdapterMightyLittleGeodesy"
            "Programmerare.CrsTransformations.Adapter.DotSpatial.CrsTransformationAdapterDotSpatial"
            "Programmerare.CrsTransformations.Adapter.ProjNet.CrsTransformationAdapterProjNet"
            // "Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI.CrsTransformationAdapterProjNet4GeoAPI"  // Obsolete (ProjNet 2 is the next version of ProjNet4GeoAPI 1.4)
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
            theTypes.FirstOrDefault(fun t -> t.FullName.Equals(className))

        ///<param name="crsTransformationAdapterClassName">
        ///the full class name (i.e. including the namespace)
        ///of a class which must implement the interface CrsTransformationAdapter
        ///</param>
        ///<returns>
        ///an instance if it could be created but otherwise an exception      
        ///</returns>
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

        ///<returns>
        ///a list of instances for all known leaf implementations 
        ///of the adapter interface, which are available at the class path.
        ///</returns>
        override this.GetInstancesOfAllImplementations() = 
            this.instancesOfAllKnownAvailableImplementationsLazyLoaded.Force() :> IList<ICrsTransformationAdapter>

        ///<param name="crsTransformationAdapterClassName">
        ///the full class name (i.e. including the package name)
        ///of a class which must implement the interface ICrsTransformationAdapter
        ///</param>
        ///<returns>
        ///true if it possible to create an instance from the input string 
        ///</returns>
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

        ///<returns>
        ///a list of instances for all known leaf implementations 
        ///of the adapter interface, which are available at the class path.
        ///</returns>        
        override x.GetInstancesOfAllImplementations() = 
            listOfCrsTransformationAdapters

        ///<param name="crsTransformationAdapterClassName">
        ///the full class name (i.e. including the package name)
        ///of a class which must implement the interface ICrsTransformationAdapter
        ///</param>>
        ///<returns>
        ///true if it possible to create an instance from the input string 
        ///</returns>
        override x.IsCrsTransformationAdapter(crsTransformationAdapterClassName: string): bool = 
            let adapterInstance = listOfCrsTransformationAdapters.FirstOrDefault(fun a -> a.GetType().FullName.Equals(crsTransformationAdapterClassName))
            not(isNull adapterInstance)
            
        override x.GetClassNamesForAllImplementations() = 
            _classNamesForAllImplementationsLazyLoaded.Force()
    end
// --------------------------------------------------------------