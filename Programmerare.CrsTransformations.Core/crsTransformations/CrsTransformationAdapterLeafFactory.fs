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
module CrsTransformationAdapterLeafFactory =

        // The hardcoded strings below will NOT change often 
        // and if they do then it should be detected by failing 
        // tests in the C# class CrsTransformationAdapterLeafFactoryTest
        let private assemblyNamesForAllKnownImplementations = [
            "Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy"
            "Programmerare.CrsTransformations.Adapter.DotSpatial"
            "Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI"
        ]

        // The hardcoded strings below will NOT change often 
        // and if they do then it should be detected by failing 
        // tests in the C# class CrsTransformationAdapterLeafFactoryTest
        let private classNamesForAllKnownImplementations = [
            "Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy.CrsTransformationAdapterMightyLittleGeodesy"
            "Programmerare.CrsTransformations.Adapter.DotSpatial.CrsTransformationAdapterDotSpatial"
            "Programmerare.CrsTransformations.Adapter.ProjNet4GeoAPI.CrsTransformationAdapterProjNet4GeoAPI"
        ]

        let GetClassNamesForAllKnownImplementations() = 
            classNamesForAllKnownImplementations.ToList() :> IList<string>

        let private assembliesWithAdapterImplementations: Lazy<IList<Assembly>> =
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

        let private typesWithAdapterImplementations: Lazy<IList<Type>> =
            lazy (
                let types = new List<Type>()
                let assemblies = assembliesWithAdapterImplementations.Force()
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

        let private GetTypesWithAdapterImplementations(): IList<Type> =
            typesWithAdapterImplementations.Force()

        let private GetTypeInstanceForClassName(className: string): Type =
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
        let CreateCrsTransformationAdapter(crsTransformationAdapterClassName: string): ICrsTransformationAdapter =
            try
                let theType = GetTypeInstanceForClassName(crsTransformationAdapterClassName)
                if isNull theType then
                    raise (Exception("The type could not be found: " + crsTransformationAdapterClassName))
                let instance = Activator.CreateInstance(theType)
                instance :?> ICrsTransformationAdapter
            with
                | exc -> 
                    let nameOfInterfaceThatShouldBeImplemented = typeof<ICrsTransformationAdapter>.FullName
                    let message = "Failed to instantiate a class with the name '" + crsTransformationAdapterClassName + "' . The parameter must be the name of an available class which implements the interface '" + nameOfInterfaceThatShouldBeImplemented + "'"
                    invalidArg "crsTransformationAdapterClassName" message

        let private instancesOfAllKnownAvailableImplementations: Lazy<List<ICrsTransformationAdapter>> = 
            lazy (
                let list = new List<ICrsTransformationAdapter>()
                for className in classNamesForAllKnownImplementations do
                    list.Add(CreateCrsTransformationAdapter(className))
                list
            )
            
        (*
        * @return a list of instances for all known leaf implementations 
        *      of the adapter interface, which are available at the class path.
        *)    
        let GetInstancesOfAllKnownAvailableImplementations() = 
            instancesOfAllKnownAvailableImplementations.Force() :> IList<ICrsTransformationAdapter>

        (*
         * @param the full class name (i.e. including the package name)
         *      of a class which must implement the interface ICrsTransformationAdapter
         * @return true if it possible to create an instance from the input string 
         *)
        let IsCrsTransformationAdapter(crsTransformationAdapterClassName: string): bool = 
            instancesOfAllKnownAvailableImplementations.Force().Exists(
                fun i -> i.GetType().FullName.Equals(crsTransformationAdapterClassName)
            )
