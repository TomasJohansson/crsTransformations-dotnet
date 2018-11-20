namespace com.programmerare.crsTransformations

open System.Collections.Generic
open com.programmerare.crsTransformations.crsIdentifier
open com.programmerare.crsTransformations.coordinate

(*
 * This adapter interface is the core type of this CRS transtiomation library.
 * 
 * It defines six transform methods.
 * 
 * Three of them will only return a coordinate with the result,
 * while the other three will return a result object
 * which also contains more information e.g. all the individual 'leaf'
 * results if the implementing class was a 'composite'.  
 *
 * The difference between the three methods (returning the same type)
 * is just the last parameter which is either an integer value
 * with an EPSG code (e.g. 4326) or a string (an integer value but also with a "EPSG:"-prefix e.g. "EPSG:4326")
 * or an instance of CrsIdentifier.  
 *
 * If a transform method without the CrsIdentifier parameter is used then
 * the CrsIdentifier will be created by the other transform methods.
 * 
 * In other words, the methods with integer or string parameter are
 * convenience methods. If you are going to do many transformations
 * you may want to create the CrsIdentifier object once yourself
 * and then use a method using it as a parameter.  
 *
 * The methods 'transformToCoordinate' can throw exception when the transformation fails.
 * 
 * The methods 'transform' should always return a result object rather than throwing an exception.
 * 
 * If you use a method returning the result object then you should
 * check for failures with 'TransformationResult.isSuccess'.
 *
 * @author Tomas Johansson ( http://programmerare.com )
 * The code in the "crs-transformation-adapter-core" project is licensed with MIT.
 * Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
 * Please find more information in the license file at the root directory of each subproject
 * (e.g. the subprojects "crs-transformation-adapter-impl-geotools" , "crs-transformation-adapter-impl-proj4j" and so on)
 *)
type CrsTransformationAdapter =
    interface
        // -------------------------------------------------
        // The three methods returning a coordinate object:

        (*
         * Transforms a coordinate to a coordinate
         * in another coordinate reference system if possible
         * but may throw an exception if the transformation fails.
         *)
        abstract member TransformToCoordinate : CrsCoordinate * int -> CrsCoordinate

        (*
         * Transforms a coordinate to a coordinate
         * in another coordinate reference system if possible
         * but may throw an exception if the transformation fails.
         *)
        abstract member TransformToCoordinate : CrsCoordinate * string -> CrsCoordinate

        (*
         * Transforms a coordinate to a coordinate
         * in another coordinate reference system if possible
         * but may throw an exception if the transformation fails.
         *)
        abstract member TransformToCoordinate : CrsCoordinate * CrsIdentifier -> CrsCoordinate
        // -------------------------------------------------

        // -------------------------------------------------
        // The three methods returning a transformation result object:

        (*
         * Transforms a coordinate to another coordinate reference system.
         * 
         * The method should never throw an exception but instead one of the methods
         * in the result object should be used to check for failure.
         *)
        abstract member Transform : CrsCoordinate * int -> CrsTransformationResult

        (*
         * Transforms a coordinate to another coordinate reference system.
         * 
         * The method should never throw an exception but instead one of the methods
         * in the result object should be used to check for failure.
         *)
        abstract member Transform : CrsCoordinate * string -> CrsTransformationResult

        (*
         * Transforms a coordinate to another coordinate reference system.
         * 
         * The method should never throw an exception but instead one of the methods
         * in the result object should be used to check for failure.
         *)
        abstract member Transform : CrsCoordinate * CrsIdentifier -> CrsTransformationResult
        // -------------------------------------------------

        (*
         * Should normally simply return the full class name (including the package name), 
         * but when implementing test doubles (e.g. Mockito stub)
         * then the method should be implemented by defining different names
         * to simulate that different classes (implementations)
         * should have different weights.
         *)
        abstract member LongNameOfImplementation : string

        (*
         * Should return the unique suffix part of the class name
         * i.e. the class name without the prefix which is common
         * for all implementations.
         *)
        abstract member ShortNameOfImplementation : string

        (*
         * @see CrsTransformationAdapteeType
         *)
        abstract member AdapteeType : CrsTransformationAdapteeType

        (*
         * @return  true if the implementation is a 'composite'
         *          but false if it is a 'leaf' implementation
         *)
        abstract member IsComposite : bool

        (*
         * @return  a list of children/leafs when the implementation
         *          is a 'composite'but if the implementation is a 'leaf'
         *          then an empty list should be returned.
         *)
        abstract member GetTransformationAdapterChildren : unit -> IList<CrsTransformationAdapter>

    end