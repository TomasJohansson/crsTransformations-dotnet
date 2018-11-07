package com.programmerare.crsTransformations

/**
 * Enumeration type returned from a method in the adapter interface.
 * 
 * The purpose is to make it easier to see from where a result originated
 * when iterating the 'leaf' adapter results in a 'composite' object.
 * 
 * The names of the leafs in the enumeration includes information
 * about the version number for the adaptee library it represents.
 */
enum class CrsTransformationAdapteeType {

    /**
     * Maven version for the adaptee library:  
     * "org.geotools:gt-main:20.0"
     */
    LEAF_GEOTOOLS_20_0,

    /**
     * Maven version for the adaptee library:
     * "com.github.goober:coordinate-transformation-library:1.1"
     */
    LEAF_GOOBER_1_1,

    /**
     * Maven version for the adaptee library:  
     * "org.orbisgis:cts:1.5.1"
     */
    LEAF_ORBISGIS_1_5_1,

    /**
     * Maven version for the adaptee library:  
     * "mil.nga.geopackage:geopackage-core:3.1.0"
     */
    LEAF_NGA_GEOPACKAGE_3_1_0,

    /**
     * Maven version for the adaptee library:  
     * "org.osgeo:proj4j:0.1.0"
     */
    LEAF_PROJ4J_0_1_0,


    // The above "leafs" are the real "adaptees"
    // and the below composite "adapters" are not true adapters

    // TODO MAYBE: use a version number for this crs-transformation library
    // to be used as suffix for the below enum values ...
    // though questionable if that would be meaningful, while it can be more useful
    // for troubleshooting to make it easier to figure out exactly which
    // version of a leaf adaptee is causing a certain transformation

    /**
     * Represents a composite which returns a result with
     * longitude and latitude being the median of the 'leafs'.
     */
    COMPOSITE_MEDIAN,

    /**
     * Represents a composite which returns a result with
     * longitude and latitude being the average of the 'leafs'.
     */
    COMPOSITE_AVERAGE,

    /**
     * Represents a composite which returns a result with
     * longitude and latitude being a weighted average of the 'leafs'.
     * 
     * The implementation will try to use results from all 'leaf' adapters
     * by calculating the resulting coordinate using weights
     * which must have been provided to the composite object when it was constructed.
     */
    COMPOSITE_WEIGHTED_AVERAGE,


    /**
     * Represents a composite which returns a result with
     * longitude and latitude being the first
     * succesful result for a 'leaf'.
     * 
     * When a result from a 'leaf' is considered as (seem to be) succesful
     * then no more leafs will be used for the transformation.
     * 
     * In other words, the number of results will always
     * be zero or one, unlike the median and average (or weighted) composites
     * which can have many results from multiple 'leafs' (adapter/adaptee implementations).
     */
    COMPOSITE_CHAIN_OF_RESPONSIBILITY,


    /**
     * A default value for leafs in a base class but this value
     * should normally not occur since it should be overridden in
     * leaf implementations.
     */
    UNSPECIFIED_LEAF,

    /**
     * A default value for composites in a base class but this value
     * should normally not occur since it should be overridden in
     * composite implementations.
     */
    UNSPECIFIED_COMPOSITE,

    /**
     * A default value for adapters in a base class but this value
     * should normally not occur since it should be overridden in
     * subclass implementations.
     */
    UNSPECIFIED;
}