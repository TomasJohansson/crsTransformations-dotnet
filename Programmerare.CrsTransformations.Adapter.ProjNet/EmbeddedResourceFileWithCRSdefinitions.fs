﻿namespace Programmerare.CrsTransformations.Adapter.ProjNet

open System

// "enum":
type EmbeddedResourceFileWithCRSdefinitions = 
    
    |
        [<Obsolete("Do not use. Instead use a value including the version e.g. 'STANDARD_FILE_SHIPPED_WITH_ProjNet_2_0_0'")>]
        STANDARD_FILE_SHIPPED_WITH_ProjNet = 10

    | STANDARD_FILE_SHIPPED_WITH_ProjNet4GeoAPI_1_4_1 = 11
    | STANDARD_FILE_SHIPPED_WITH_ProjNet_2_0_0 = 12

    // The only differences compared with the standard "SRID.csv" 
    // shipped with ProjNet4GeoAPI are the following five rows:
    //3019;PROJCS["RT90 7.5 gon V",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",11.30827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3019"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3020;PROJCS["RT90 5 gon V",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",13.55827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3020"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3021;PROJCS["RT90 2.5 gon V",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",15.80827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3021"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3022;PROJCS["RT90 0 gon",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",18.05827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3022"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3023;PROJCS["RT90 2.5 gon O",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",20.30827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3023"],AXIS["X",NORTH],AXIS["Y",EAST]]
    //3024;PROJCS["RT90 5 gon O",GEOGCS["RT90",DATUM["Rikets_koordinatsystem_1990",SPHEROID["Bessel 1841",6377397.155,299.1528128,AUTHORITY["EPSG","7004"]],TOWGS84[414.1,41.3,603.1,-0.855,2.141,-7.023,0],AUTHORITY["EPSG","6124"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.0174532925199433,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4124"]],UNIT["metre",1,AUTHORITY["EPSG","9001"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",22.55827777777778],PARAMETER["scale_factor",1],PARAMETER["false_easting",1500000],PARAMETER["false_northing",0],AUTHORITY["EPSG","3024"],AXIS["X",NORTH],AXIS["Y",EAST]]
    // The above five rows have been copied from the file below into a small CSV file embedded as resource.
    // SharpMap's SpatialRefSys.xml :
    // https://github.com/SharpMap/SharpMap/blob/Branches/1.0/SharpMap/SpatialRefSys.xml
    // Jul 20, 2015    
    // git commit 5fd5d7e7aaa641bc291ebe7b5a56d62970c0fdb4 :
    // https://github.com/SharpMap/SharpMap/commit/5fd5d7e7aaa641bc291ebe7b5a56d62970c0fdb4
    | SIX_SWEDISH_RT90_CRS_DEFINITIONS_COPIED_FROM_SharpMap_SpatialRefSys_xml = 20
