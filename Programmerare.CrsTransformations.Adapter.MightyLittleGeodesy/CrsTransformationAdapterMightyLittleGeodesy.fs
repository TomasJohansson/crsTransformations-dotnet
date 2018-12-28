namespace Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy

open System.Collections.Generic
open MightyLittleGeodesy.Positions
open MightyLittleGeodesy.Classes
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
type CrsTransformationAdapterMightyLittleGeodesy() as this =
    class
        inherit CrsTransformationAdapterBaseLeaf
            ( 
                ( fun () -> this._GetFileInfoVersion() ),
                ( fun (inputCoordinate, crsIdentifierForOutputCoordinateSystem) -> this._TransformToCoordinateStrategy(inputCoordinate, crsIdentifierForOutputCoordinateSystem) )
            )

        static let WGS84 = CrsIdentifierFactory.CreateFromEpsgNumber(4326)
        static let rt90Projections: Dictionary<int, RT90Position.RT90Projection> = new Dictionary<int, RT90Position.RT90Projection>()
        static let sweREFProjections: Dictionary<int, SWEREF99Position.SWEREFProjection> = new Dictionary<int, SWEREF99Position.SWEREFProjection>()

        static do
            // Below some EPSG numbers are hardcoded.
            // If those numbers would be used in more than one place in this file or the module,
            // then constants would definitely have been preferred,
            // but adding 20 constants for only one usage is not very motivated.
            // Another option would have been to reuse the already existing constants in the module "crs-transformation-constants" i.e. constants like this:
            // Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4.EpsgNumber.SWEDEN__SWEREF99_TM__3006
            // However, that would introduce a dependency to a module with LOTS OF constants just to use this few values below.

            // http://spatialreference.org/ref/?search=rt90
            rt90Projections.Add(3019, RT90Position.RT90Projection.rt90_7_5_gon_v)    // EPSG:3019: RT90 7.5 gon V		https://epsg.io/3019
            rt90Projections.Add(3020, RT90Position.RT90Projection.rt90_5_0_gon_v)    // EPSG:3020: RT90 5 gon V			https://epsg.io/3020
            rt90Projections.Add(3021, RT90Position.RT90Projection.rt90_2_5_gon_v)    // EPSG:3021: RT90 2.5 gon V		https://epsg.io/3021
            rt90Projections.Add(3022, RT90Position.RT90Projection.rt90_0_0_gon_v)    // EPSG:3022: RT90 0 gon			https://epsg.io/3022
            rt90Projections.Add(3023, RT90Position.RT90Projection.rt90_2_5_gon_o)    // EPSG:3023: RT90 2.5 gon O		https://epsg.io/3023
            rt90Projections.Add(3024, RT90Position.RT90Projection.rt90_5_0_gon_o)    // EPSG:3024: RT90 5 gon O			https://epsg.io/3024

            // http://spatialreference.org/ref/?search=sweref
            sweREFProjections.Add(3006, SWEREF99Position.SWEREFProjection.sweref_99_tm)       // EPSG:3006: SWEREF99 TM		https://epsg.io/3006
            sweREFProjections.Add(3007, SWEREF99Position.SWEREFProjection.sweref_99_12_00)    // EPSG:3007: SWEREF99 12 00	https://epsg.io/3007
            sweREFProjections.Add(3008, SWEREF99Position.SWEREFProjection.sweref_99_13_30)    // EPSG:3008: SWEREF99 13 30	https://epsg.io/3008
            sweREFProjections.Add(3009, SWEREF99Position.SWEREFProjection.sweref_99_15_00)    // EPSG:3009: SWEREF99 15 00	https://epsg.io/3009
            sweREFProjections.Add(3010, SWEREF99Position.SWEREFProjection.sweref_99_16_30)    // EPSG:3010: SWEREF99 16 30	https://epsg.io/3010
            sweREFProjections.Add(3011, SWEREF99Position.SWEREFProjection.sweref_99_18_00)    // EPSG:3011: SWEREF99 18 00	https://epsg.io/3011
            sweREFProjections.Add(3012, SWEREF99Position.SWEREFProjection.sweref_99_14_15)    // EPSG:3012: SWEREF99 14 15	https://epsg.io/3012
            sweREFProjections.Add(3013, SWEREF99Position.SWEREFProjection.sweref_99_15_45)    // EPSG:3013: SWEREF99 15 45	https://epsg.io/3013
            sweREFProjections.Add(3014, SWEREF99Position.SWEREFProjection.sweref_99_17_15)    // EPSG:3014: SWEREF99 17 15	https://epsg.io/3014
            sweREFProjections.Add(3015, SWEREF99Position.SWEREFProjection.sweref_99_18_45)    // EPSG:3015: SWEREF99 18 45	https://epsg.io/3015
            sweREFProjections.Add(3016, SWEREF99Position.SWEREFProjection.sweref_99_20_15)    // EPSG:3016: SWEREF99 20 15	https://epsg.io/3016
            sweREFProjections.Add(3017, SWEREF99Position.SWEREFProjection.sweref_99_21_45)    // EPSG:3017: SWEREF99 21 45	https://epsg.io/3017
            sweREFProjections.Add(3018, SWEREF99Position.SWEREFProjection.sweref_99_23_15)    // EPSG:3018: SWEREF99 23 15	https://epsg.io/3018

        member private this.isWgs84(epsgNumber) = epsgNumber = WGS84.EpsgNumber
        member private this.isSweref99(epsgNumber: int) = sweREFProjections.ContainsKey(epsgNumber)
        member private this.isRT90(epsgNumber) = rt90Projections.ContainsKey(epsgNumber)
        member private this.isSupportedCrs(epsgNumber) = 
            this.isWgs84(epsgNumber) || this.isSweref99(epsgNumber) || this.isRT90(epsgNumber)

        override this.Equals(o) =
            // the base implementation will return true
            // if the same type, and currently that is 
            // ok for this subimplementation
            // but the method is put here anyway with this comment 
            // as a reminder that if thic class would become 
            // configurable then update the implementation of this method 
            // (for example as in the subclass 'CrsTransformationAdapterProjNet4GeoAPI')
            base.Equals(o)

        // added the below method to get rid of the warning
        override this.GetHashCode() = base.GetHashCode()

        member private this.ThrowArgumentExceptionIfUnvalidCoordinateOrCrs 
            (
                inputCoordinate: CrsCoordinate,
                crsIdentifierForOutputCoordinateSystem: CrsIdentifier
            ) =
            let i = inputCoordinate.CrsIdentifier.EpsgNumber
            let o = crsIdentifierForOutputCoordinateSystem.EpsgNumber
            if(not(this.isSupportedCrs(i))) then
                invalidArg "inputCoordinate" ("crsIdentifier not supported: " + i.ToString())
            if(not(this.isSupportedCrs(o))) then
                invalidArg "crsIdentifierForOutputCoordinateSystem" ("crsIdentifier not supported: " + o.ToString())
            
        member private this._TransformToCoordinateStrategy(inputCoordinate, crsIdentifierForOutputCoordinateSystem) = 
            this.ThrowArgumentExceptionIfUnvalidCoordinateOrCrs(inputCoordinate, crsIdentifierForOutputCoordinateSystem)

            let epsgNumberForInputCoordinateSystem = inputCoordinate.CrsIdentifier.EpsgNumber
            let mutable positionToReturn: Position = null
        
            // shorter names below for readibility purpose (lots os usages further down)
            let input = epsgNumberForInputCoordinateSystem
            let output = crsIdentifierForOutputCoordinateSystem.EpsgNumber

            if(this.isRT90(input) && this.isWgs84(output)) then
                let rt90Position = RT90Position(inputCoordinate.YNorthingLatitude, inputCoordinate.XEastingLongitude, rt90Projections.[input])
                positionToReturn <- rt90Position.ToWGS84() :> Position
            elif(this.isWgs84(input) && this.isRT90(output)) then
                let wgs84Position = WGS84Position(inputCoordinate.YNorthingLatitude, inputCoordinate.XEastingLongitude)
                positionToReturn <- RT90Position(wgs84Position, rt90Projections.[output])
            elif(this.isSweref99(input) && this.isWgs84(output)) then
                let sweREF99Position = SWEREF99Position(inputCoordinate.YNorthingLatitude, inputCoordinate.XEastingLongitude, sweREFProjections.[input])
                positionToReturn <- sweREF99Position.ToWGS84()
            elif(this.isWgs84(input) && this.isSweref99(output)) then
                let wgs84Position = WGS84Position(inputCoordinate.YNorthingLatitude, inputCoordinate.XEastingLongitude)
                positionToReturn <- SWEREF99Position(wgs84Position, sweREFProjections.[output])

            if(not(isNull positionToReturn)) then
                CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude
                    (
                        positionToReturn.Latitude,
                        positionToReturn.Longitude,
                        crsIdentifierForOutputCoordinateSystem
                    )
            elif
                // not direct support for transforming directly between SWEREF99 and RT90
                // but can do it by first transforming to WGS84 and then to the other
                (this.isSweref99(input) && this.isRT90(output))
                ||
                (this.isRT90(input) && this.isSweref99(output))
                || // transform between different Sweref systems
                (this.isSweref99(input) && this.isSweref99(output))
                || // transform between different RT90 systems
                (this.isRT90(input) && this.isRT90(output))
            then            
                // first transform to WGS84 (recursive call to this method)
                let wgs84Coordinate = this._TransformToCoordinateStrategy(inputCoordinate, WGS84)
                // then transform from WGS84 (recursive call to this method)
                this._TransformToCoordinateStrategy(wgs84Coordinate, crsIdentifierForOutputCoordinateSystem)
            else
                // this should not occur. validation should be thrown earlier
                invalidOp "Unsupported transformation"

        override this.AdapteeType =
            CrsTransformationAdapteeType.LEAF_MIGHTY_LITTLE_GEODESY_1_0_1

        // it is not a problem to hardcode the name below 
        // if the type name would become renamed since a 
        // renaming would then be detected by test code which 
        // verfifies that this short name actually is the 
        // suffix of the type name
        override this.ShortNameOfImplementation = "MightyLittleGeodesy"

        member private this._GetFileInfoVersion() =
            FileInfoVersion.GetFileInfoVersionHelper(typeof<RT90Position>)

    end
(*
https://www.nuget.org/packages/MightyLittleGeodesy/
https://github.com/bjornsallarp/MightyLittleGeodesy
*)