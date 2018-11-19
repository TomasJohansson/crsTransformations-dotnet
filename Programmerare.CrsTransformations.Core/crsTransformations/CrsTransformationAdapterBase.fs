namespace com.programmerare.crsTransformations

open com.programmerare.crsTransformations.coordinate
open com.programmerare.crsTransformations.crsIdentifier

(*
 * The base class of the adapter interface implementing most of the 
 * transform methods as final i.e. not overridden by subclasses.  
 * 
 * @see CrsTransformationAdapter
 *
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-adapter-core" project is licensed with MIT.
 * Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
 * Please find more information in the license file at the root directory of each subproject
 * (e.g. the subprojects "crs-transformation-adapter-impl-geotools" , "crs-transformation-adapter-impl-proj4j" and so on)
 *)
[<AbstractClass>]
type CrsTransformationAdapterBase() = // : CrsTransformationAdapter {
    class
        (*
         * Transforms a coordinate to another coordinate reference system.  
         * 
         * This is a "hook" method (as it is named in the design pattern Template Method)   
         * which must be implemented by subclasses.
         *)
        // TODO: try making this method "internal" ... ? since "protected" is not currently used in F# ...
        abstract _TransformToCoordinateHook : CrsCoordinate * CrsIdentifier -> CrsCoordinate

        abstract _TransformHook : CrsCoordinate * CrsIdentifier -> CrsTransformationResult

        static member private classNamePrefix = "CrsTransformationAdapter"
        // if the above string would change because of class renamings
        // then it will be detected by a failing test

        interface CrsTransformationAdapter with
            member this.GetTransformationAdapterChildren: List<CrsTransformationAdapter> = 
                raise (System.NotImplementedException())
            // -------------------------------------------------
            // The three below methods returning a coordinate object
            // are all final (i.e. not overridden) and invokes
            // a so called "hook" method(named so in the Template Method pattern)
            // which is an abstract method that must be implemented in a subclass.

            member this.TransformToCoordinate(inputCoordinate, crsCode) =
                this._TransformToCoordinateHook(inputCoordinate, CrsIdentifierFactory.CreateFromCrsCode(crsCode))

            member this.TransformToCoordinate(inputCoordinate, epsgNumberForOutputCoordinateSystem) = 
                this._TransformToCoordinateHook(inputCoordinate, CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumberForOutputCoordinateSystem))

            member this.TransformToCoordinate(inputCoordinate, crsIdentifier) = 
                this._TransformToCoordinateHook(inputCoordinate, crsIdentifier)
            // -------------------------------------------------

            // -------------------------------------------------

            member this.Transform(inputCoordinate, crsCode) =
                this._TransformHook(inputCoordinate, CrsIdentifierFactory.CreateFromCrsCode(crsCode))

            member this.Transform(inputCoordinate, epsgNumberForOutputCoordinateSystem) = 
                this._TransformHook(inputCoordinate, CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumberForOutputCoordinateSystem))

            member this.Transform(inputCoordinate, crsIdentifier) = 
                this._TransformHook(inputCoordinate, crsIdentifier)

            // -------------------------------------------------

            member this.GetLongNameOfImplementation =
                raise (System.NotImplementedException())
                //"return this.javaClass.name"


            member this.GetShortNameOfImplementation =
                raise (System.NotImplementedException())
                //"TODO_name"
                //let className = this.javaClass.simpleName
                //if className.startsWith(classNamePrefix) && !className.equals(classNamePrefix) then
                //    return className.substring(classNamePrefix.length)
                //else 
                //    return className

            member this.GetAdapteeType =
                // Should be overridden by subclasses
                CrsTransformationAdapteeType.UNSPECIFIED

            member this.IsComposite =
                raise (System.NotImplementedException())

            (*
             * This helper method is protected since it is NOT intended for
             * client code but only for test code purposes.
             * 
             * It should be overridden by subclasses.
             * @return empty string is returned as the default value
             *      which should also be returned byt the composites (i.e. they should not override).
             *      
             *      The 'leaf' adapter implementations should return the
             *      name of the jar file (potentially including a path)
             *      for the used adaptee library.
             *      
             *      The reason is that the jar files (retrieved through Maven)
             *      includes the version name and can be asserted in test code
             *      to help remembering that the value of an enum specifying
             *      the 'adaptee' (and version) should be updated after an adaptee upgrade.
             * @see CrsTransformationAdapteeType
             *)
            //member this.GetNameOfJarFileOrEmptyString() =
            //    return ""

            (*
             * Protected helper method intended to be used from subclasses
             * when implementing the method that should return the name
             * of a jar file belonging to an adaptee library.
             *)
            //member this.GetNameOfJarFileFromProtectionDomain(
            //    protectionDomainCreatedFromSomeClassInTheThidPartAdapteeLibrary: ProtectionDomain
            //) =
            //    protectionDomainCreatedFromSomeClassInTheThidPartAdapteeLibrary.codeSource.location.toExternalForm()

    end