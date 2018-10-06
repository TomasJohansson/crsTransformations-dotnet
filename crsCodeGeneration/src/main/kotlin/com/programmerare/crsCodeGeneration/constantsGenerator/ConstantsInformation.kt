package com.programmerare.crsCodeGeneration.constantsGenerator

/**
 * This class is used from a freemarker template.
 * ( .\crsCodeGeneration\src\main\resources\freemarker_templates\Constants.ftlh )
 */
class ConstantsInformation(
    val nameOfJavaClass: String,
    val nameOfJavaPackage: String,
    val constants: List<ConstantTypeNameValue>
) {
}