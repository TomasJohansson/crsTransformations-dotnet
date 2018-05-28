package ${nameOfKotlinPackage}

// This file is generated with content from a database. Do not edit!

// The generated class became too large:
// java.lang.RuntimeException: Method code too large!
//        at org.jetbrains.org.objectweb.asm.MethodWriter.getSize(MethodWriter.java:2059)
//        at org.jetbrains.org.objectweb.asm.ClassWriter.toByteArray(ClassWriter.java:861)
//        at org.jetbrains.kotlin.codegen.ClassBuilderFactories$2.asBytes(ClassBuilderFactories.java:126)
//        at org.jetbrains.kotlin.codegen.DelegatingClassBuilderFactory.asBytes(DelegatingClassBuilderFactory.kt:36)
//        at org.jetbrains.kotlin.codegen.DelegatingClassBuilderFactory.asBytes(DelegatingClassBuilderFactory.kt:36)
//        at org.jetbrains.kotlin.codegen.DelegatingClassBuilderFactory.asBytes(DelegatingClassBuilderFactory.kt:36)

class ${nameOfKotlinClass} {
    val lazyValue: List<EpsgCrsAndAreaCodeWithCoordinates> by lazy {
        listOf<EpsgCrsAndAreaCodeWithCoordinates>(
            <#list testdata as t>
            create( ${t.epsgCrsCode} , ${t.epsgAreaCode} , "${t.epsgAreaName}" , ${t.centroidX}, ${t.centroidY} )<#sep>,</#sep>
            </#list>
        )
    }



    companion object {
        fun create(
            epsgCrsCode: Int,
            epsgAreaCode: Int,
            epsgAreaName: String,
            centroidX: Double,
            centroidY: Double
        ): EpsgCrsAndAreaCodeWithCoordinates {
            return EpsgCrsAndAreaCodeWithCoordinates(epsgCrsCode, epsgAreaCode, epsgAreaName, centroidX, centroidY)
        }
    }
}

data class EpsgCrsAndAreaCodeWithCoordinates(
    val epsgCrsCode: Int,
    val epsgAreaCode: Int,
    val epsgAreaName: String,
    val centroidX: Double,
    val centroidY: Double
) {
}