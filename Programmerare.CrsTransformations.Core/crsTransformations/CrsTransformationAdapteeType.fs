namespace Programmerare.CrsTransformations

open System

///<summary>
///<para>
///Enumeration type returned from a method in the adapter interface.
///</para>
///<para/>
///<para>
///The purpose of its usage is to make it easier to see from where a result 
///originated when iterating the 'leaf' adapter results in a 'composite' object.
///</para>
///<para/>
///<para>
///The names of the leafs in the enumeration includes information
///about the version number for the adaptee library it represents.
///</para>
///<para/>
///<para>
///After an upgrade, test code should help to remind about updating the enum values.
///For example, there is test code that retrives the name of the DLL file 
///and the version (from the path) for a class within DotSpatial library, 
///and the testcode verifies for example the file name "dotspatial.projections.dll" 
///(and e.g. "2.0.0-rc1" as the version from the file path)
///but if the version instead would be  for example "2.0.0"
///then the test would fail to help reminding that a new enum should be added.
///(and then the previous should become marked as Obsolete)
///</para>
///<para/>
///<para>
///Copyright (c) Tomas Johansson , http://programmerare.com
///The code in the "Core" project is licensed with MIT.
///Other subprojects may be released with other licenses e.g. LGPL or Apache License 2.0.
///Please find more information in the license file at the root directory of each subproject
///(e.g. a subproject such as "Programmerare.CrsTransformations.Adapter.DotSpatial")
///</para>
///</summary>
type CrsTransformationAdapteeType =  // F# enum

    ///<summary>
    ///<para>
    ///NuGet version for the adaptee library "ProjNet4GeoAPI":
    ///</para>
    ///<para/>
    ///<para>
    ///'<PackageReference Include="ProjNET4GeoAPI" Version="1.4.1" />'
    ///</para>
    ///</summary>
    | LEAF_PROJ_NET_4_GEO_API_1_4_1 = 1100

    ///<summary>
    ///<para>
    ///NuGet version for the adaptee library "DotSpatial":
    ///</para>
    ///<para/>
    ///<para>
    ///'<PackageReference Include="DotSpatial.Projections" Version="2.0.0-rc1" />'
    ///</para>
    ///</summary>
    | LEAF_DOT_SPATIAL_2_0_0_RC1 = 1200

    | 
        [<Obsolete("Do not use. Use 'LEAF_MIGHTY_LITTLE_GEODESY_1_0_2' instead")>]
        LEAF_MIGHTY_LITTLE_GEODESY_1_0_1 = 1900

    ///<summary>
    ///<para>
    ///NuGet version for the adaptee library "MightyLittleGeodesy":
    ///</para>
    ///<para/>
    ///<para>
    ///'<PackageReference Include="MightyLittleGeodesy" Version="1.0.1" />'
    ///</para>
    ///<para/>
    ///<para>
    ///The implementation supports only transformations 
    ///between the the global WGS84 CRS and the "two" (actually more, see below)
    ///Swedish coordinate reference systems SWEREF99 and RT90.
    ///SWEREF99 is the "new" offical CRS for Sweden while RT90 is the "old".
    ///There are 13 supported versions of SWEREFF99 (with EPSG numbers 3006-3018)
    ///and 6 supported versions of RT90 (with EPSG numbers 3019-3024).
    ///</para>
    ///</summary>
    | LEAF_MIGHTY_LITTLE_GEODESY_1_0_2 = 1902

    (*
     The above "leafs" are the real "adaptees" and 
     the below composite "adapters" are not true adapters.

     Maybe a version number for this crs-transformation library (e.g. suffix _1_0_0)
     should be used as suffix for the below enum values i.e. similarly to the above leafs ...
     though questionable if that would be meaningful, while it can be more useful
     for troubleshooting to make it easier to figure out exactly which
     version of a leaf adaptee is causing a certain transformation
    *)

    ///<summary>
    ///Represents a composite which returns a result with
    ///longitude and latitude being the median of the 'leafs'.
    ///</summary>
    | COMPOSITE_MEDIAN = 9010

    ///<summary>
    ///Represents a composite which returns a result with
    ///longitude and latitude being the average of the 'leafs'.
    ///</summary>
    | COMPOSITE_AVERAGE = 9020

    ///<summary>
    ///<para>
    ///Represents a composite which returns a result with
    ///longitude and latitude being a weighted average of the 'leafs'.
    ///</para>
    ///<para/>
    ///<para>
    ///The implementation will try to use results from all 'leaf' adapters
    ///by calculating the resulting coordinate using weights
    ///which must have been provided to the composite object when it was constructed.
    ///</para>
    ///</summary>
    | COMPOSITE_WEIGHTED_AVERAGE = 9030

    ///<summary>
    ///<para>
    ///Represents a composite which returns a result with
    ///longitude and latitude being the first
    ///succesful result for a 'leaf'.
    ///</para>
    ///<para/>
    ///<para>
    ///When a result from a 'leaf' is considered as (i.e. seem to be) 
    ///succesful then no more leafs will be used for the transformation.
    ///</para>
    ///<para/>
    ///<para>
    ///In other words, the number of results will always
    ///be zero or one, unlike the median and average (or weighted) composites
    ///which can have many results from multiple 'leafs' (adapter/adaptee implementations).
    ///</para>
    ///</summary>
    | COMPOSITE_FIRST_SUCCESS = 9040

    ///<summary>
    ///<para>
    ///A default value for leafs in a base class but this value
    ///should not occur since it should be overridden in
    ///leaf implementations.
    ///</para>
    ///</summary>
    | UNSPECIFIED_LEAF = 110

    ///<summary>
    ///<para>
    ///A default value for composites in a base class but this value
    ///should not occur since it should be overridden in
    ///composite implementations.
    ///</para>
    ///</summary>
    | UNSPECIFIED_COMPOSITE = 120

    ///<summary>
    ///<para>
    ///A default value for adapters in a base class but this value
    ///should not occur since it should be overridden in
    ///subclass implementations.
    ///</para>
    ///</summary>
    | UNSPECIFIED = 100