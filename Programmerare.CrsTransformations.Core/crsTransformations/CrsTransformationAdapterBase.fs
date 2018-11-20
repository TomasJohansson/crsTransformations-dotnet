namespace com.programmerare.crsTransformations

open System.Collections.Generic
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

        let TrowExceptionIfCoordinateIsNull(inputCoordinate) : unit = 
            if isNull inputCoordinate then
                nullArg "inputCoordinate"
            // The above would cause this error:
            // "Value cannot be null. Parameter name: inputCoordinate"
            // instead of the following which would occur later:
            // "Object reference not set to an instance of an object"
                

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

        abstract member AdapteeType : CrsTransformationAdapteeType
        default this.AdapteeType = CrsTransformationAdapteeType.UNSPECIFIED

        abstract member IsComposite : bool
        default this.IsComposite = raise (System.NotImplementedException())

        abstract member LongNameOfImplementation : string
        default this.LongNameOfImplementation = raise (System.NotImplementedException())

        abstract member ShortNameOfImplementation : string
        default this.ShortNameOfImplementation = raise (System.NotImplementedException())

        abstract member GetTransformationAdapterChildren : unit -> IList<CrsTransformationAdapter>
        default this.GetTransformationAdapterChildren() = raise (System.NotImplementedException())

        interface CrsTransformationAdapter with
            member this.GetTransformationAdapterChildren() =  this.GetTransformationAdapterChildren()
                
            // -------------------------------------------------
            // The three below methods returning a coordinate object
            // are all final (i.e. not overridden) and invokes
            // a so called "hook" method(named so in the Template Method pattern)
            // which is an abstract method that must be implemented in a subclass.

            member this.TransformToCoordinate(inputCoordinate, crsCode) =
                TrowExceptionIfCoordinateIsNull(inputCoordinate)
                this._TransformToCoordinateHook(inputCoordinate, CrsIdentifierFactory.CreateFromCrsCode(crsCode))

            member this.TransformToCoordinate(inputCoordinate, epsgNumberForOutputCoordinateSystem) = 
                TrowExceptionIfCoordinateIsNull(inputCoordinate)
                this._TransformToCoordinateHook(inputCoordinate, CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumberForOutputCoordinateSystem))

            member this.TransformToCoordinate(inputCoordinate, crsIdentifier) = 
                TrowExceptionIfCoordinateIsNull(inputCoordinate)
                this._TransformToCoordinateHook(inputCoordinate, crsIdentifier)
            // -------------------------------------------------

            // -------------------------------------------------
            member this.Transform(inputCoordinate, crsIdentifier) = 
                this._TransformHook(inputCoordinate, crsIdentifier)

            member this.Transform(inputCoordinate, crsCode) =
                try
                    let crs = CrsIdentifierFactory.CreateFromCrsCode(crsCode)
                    // it is the row above which might throw an exception
                    // but the row below should not through an exception
                    this._TransformHook(inputCoordinate, crs)
                with
                    // | :? System.Exception as exc -> 
                    // alternative to the above:
                    | exc -> 
                        CrsTransformationResult(
                            inputCoordinate,
                            null,
                            exc,
                            false,
                            new List<CrsTransformationResult>()
                        )
            // TODO refactor the try/with code duplicated above/below 
            member this.Transform(inputCoordinate, epsgNumberForOutputCoordinateSystem) = 
                try
                    let crs = CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumberForOutputCoordinateSystem)
                    // it is the row above which might throw an exception
                    // but the row below should not through an exception
                    this._TransformHook(inputCoordinate, crs)
                with
                    // | :? System.Exception as exc -> 
                    // alternative to the above:
                    | exc -> 
                        CrsTransformationResult(
                            inputCoordinate,
                            null,
                            exc,
                            false,
                            new List<CrsTransformationResult>()
                        )

            // -------------------------------------------------

            member this.LongNameOfImplementation = this.LongNameOfImplementation

            member this.ShortNameOfImplementation = this.ShortNameOfImplementation

            member this.AdapteeType = this.AdapteeType

            member this.IsComposite = this.IsComposite

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