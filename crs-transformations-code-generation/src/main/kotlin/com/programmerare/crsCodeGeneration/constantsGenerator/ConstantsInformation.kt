package com.programmerare.crsCodeGeneration.constantsGenerator

/**
 * This class is used from two freemarker templates:
 *  .\crsCodeGeneration\src\main\resources\freemarker_templates\ConstantsJava.ftlh
 *  .\crsCodeGeneration\src\main\resources\freemarker_templates\ConstantsCSharpe.ftlh
 */
class ConstantsInformation(
        val nameOfClass: String,
        val nameOfPackageOrNamespace: String,
        val constants: List<ConstantTypeNameValue>
) {
}