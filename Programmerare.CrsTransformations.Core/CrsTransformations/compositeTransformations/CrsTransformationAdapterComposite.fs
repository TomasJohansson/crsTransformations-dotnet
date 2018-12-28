namespace Programmerare.CrsTransformations.CompositeTransformations

open System.Collections.Generic
open Programmerare.CrsTransformations
open Programmerare.CrsTransformations.Coordinate
open Programmerare.CrsTransformations.Identifier
(*
Copyright (c) Tomas Johansson , http://programmerare.com
The code in the "Core" project is licensed with MIT.
Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
Please find more information in the license file at the root directory of each subproject
(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")
*)
///<summary>
///Base class for the 'composite' adapters.
///</summary>
[<AbstractClass>]
type CrsTransformationAdapterComposite internal
    (
    (*
      Interface for calculating the resulting coordinate in different ways, 
      e.g. one strategy implementation calculates the median and another the average.
    *)        
    compositeStrategy: ICompositeStrategy
    ) as this =

    class
        inherit CrsTransformationAdapterBase
            (
                ( fun () -> this._GetFileInfoVersionForComposites() ) ,
                ( fun (inputCoordinate, crsIdentifierForOutputCoordinateSystem) -> this._TransformToCoordinateStrategy(inputCoordinate, crsIdentifierForOutputCoordinateSystem) ),
                ( fun (inputCoordinate, crsIdentifierForOutputCoordinateSystem) -> this._TransformStrategy(inputCoordinate, crsIdentifierForOutputCoordinateSystem) )
            )

        override this.Equals(o) =
            let areTheSameType = base.Equals(o)
            // Currently the above implementation/semantic may be correct
            // but added a defensive type check below too
            if(areTheSameType && o :? CrsTransformationAdapterComposite) then
                let that = o :?> CrsTransformationAdapterComposite
                this._GetCompositeStrategy().Equals(that._GetCompositeStrategy())
            else
                false

        // Added the below method to get rid of a warning
        // (and the performance should not be an issue since there are some very few 
        //  implementations and does not make sense to use lots of instances of them
        //  so it really should never be very "crowded" with lots of object 
        //  within the same hash bucket i.e. with the same hash code)
        override this.GetHashCode() = base.GetHashCode()

        // Not really applicable for composites, so instead just using 
        // a "default" object without filename and version
        member private this._GetFileInfoVersionForComposites() = FileInfoVersion.FileInfoVersionNOTrepresentingThirdPartLibrary
            
        member internal this._GetCompositeStrategy() = compositeStrategy

        member private this._TransformToCoordinateStrategy(inputCoordinate: CrsCoordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): CrsCoordinate =
            let transformResult = this._TransformStrategy(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
            if(transformResult.IsSuccess) then
                transformResult.OutputCoordinate
            else
                if(isNull transformResult.Exception) then
                    failwith "Transformation failed"
                else
                    raise transformResult.Exception

        member private this._TransformStrategy(inputCoordinate: CrsCoordinate, crsIdentifierForOutputCoordinateSystem: CrsIdentifier): CrsTransformationResult =
            let allCrsTransformationAdapters = this._GetCompositeStrategy()._GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked()
            let list = new List<CrsTransformationResult>()
            let mutable shouldContinue = true
            for crsTransformationAdapter in allCrsTransformationAdapters do
                if shouldContinue then
                    let res = crsTransformationAdapter.Transform(inputCoordinate, crsIdentifierForOutputCoordinateSystem)
                    list.Add(res)
                    if not(this._GetCompositeStrategy()._ShouldContinueIterationOfAdaptersToInvoke(res)) then
                        shouldContinue <- false
            this._GetCompositeStrategy()._CalculateAggregatedResult(list, inputCoordinate, crsIdentifierForOutputCoordinateSystem, this)

        override this.TransformationAdapterChildren: IList<ICrsTransformationAdapter> =
            this._GetCompositeStrategy()._GetAllTransformationAdaptersInTheOrderTheyShouldBeInvoked()

        override this.IsComposite: bool = 
            true

        override this.ShortNameOfImplementation : string = 
            let baseClassName = this.GetType().BaseType.Name
            let lengthOfClassNameSuffix = baseClassName.Length
            // The suffix for each subclass is the name of the base class.
            // Base class name:  
            // "CrsTransformationAdapterComposite"
            // Name of subclasses:
            // "CrsTransformationAdapterCompositeMedian"
            // "CrsTransformationAdapterCompositeAverage"
            // and so on
            let thisClassName = this.GetType().Name
            if(thisClassName.Length > lengthOfClassNameSuffix) then
                thisClassName.Substring(lengthOfClassNameSuffix)
            else
                thisClassName

        override this.AdapteeType : CrsTransformationAdapteeType =
            this._GetCompositeStrategy()._GetAdapteeType()

        override this.TransformToCoordinate(inputCoordinate, crsCode) =
            this._TransformToCoordinateStrategy(inputCoordinate, CrsIdentifierFactory.CreateFromCrsCode(crsCode))

        override this.TransformToCoordinate(inputCoordinate, epsgNumberForOutputCoordinateSystem) = 
            this._TransformToCoordinateStrategy(inputCoordinate, CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumberForOutputCoordinateSystem))

        override this.TransformToCoordinate(inputCoordinate, crsIdentifier) = 
            this._TransformToCoordinateStrategy(inputCoordinate, crsIdentifier)

        override this.Transform(inputCoordinate, crsCode) =
            this._TransformStrategy(inputCoordinate, CrsIdentifierFactory.CreateFromCrsCode(crsCode))

        override this.Transform(inputCoordinate, epsgNumberForOutputCoordinateSystem) = 
            this._TransformStrategy(inputCoordinate, CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumberForOutputCoordinateSystem))

        override this.Transform(inputCoordinate, crsIdentifier) = 
            this._TransformStrategy(inputCoordinate, crsIdentifier)
                
        interface ICrsTransformationAdapter with
            override this.TransformationAdapterChildren =  this.TransformationAdapterChildren

            override this.TransformToCoordinate(inputCoordinate, crsCode: string) =
                this.TransformToCoordinate(inputCoordinate, crsCode)

            override this.TransformToCoordinate(inputCoordinate, epsgNumberForOutputCoordinateSystem: int) = 
                this.TransformToCoordinate(inputCoordinate, epsgNumberForOutputCoordinateSystem)

            override this.TransformToCoordinate(inputCoordinate, crsIdentifier: CrsIdentifier) = 
                this.TransformToCoordinate(inputCoordinate, crsIdentifier)

            override this.Transform(inputCoordinate, crsCode: string) =
                this.Transform(inputCoordinate, crsCode) 

            override this.Transform(inputCoordinate, epsgNumberForOutputCoordinateSystem: int) = 
                this.Transform(inputCoordinate, epsgNumberForOutputCoordinateSystem)

            override this.Transform(inputCoordinate, crsIdentifier: CrsIdentifier) = 
                this.Transform(inputCoordinate, crsIdentifier)
    end