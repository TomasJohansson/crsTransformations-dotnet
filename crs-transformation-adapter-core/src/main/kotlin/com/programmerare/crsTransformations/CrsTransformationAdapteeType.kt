package com.programmerare.crsTransformations

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
    // and the composite "adapters" are not true adapters

    // TODO MAYBE: use a version number for this crs-transformation library
    // to be used as suffix for the below enum values ...
    // though more questionable if meaningful, while it can be more useful
    // for troubleshooting to make it easier to figure out exactly which
    // version of a leaf adaptee is causing a certain transformation

    COMPOSITE_AVERAGE,
    COMPOSITE_MEDIAN,
    COMPOSITE_CHAIN_OF_RESPONSIBILITY,
    COMPOSITE_WEIGHTED_AVERAGE,

    UNSPECIFIED_LEAF,
    UNSPECIFIED_COMPOSITE,

    UNSPECIFIED;
}