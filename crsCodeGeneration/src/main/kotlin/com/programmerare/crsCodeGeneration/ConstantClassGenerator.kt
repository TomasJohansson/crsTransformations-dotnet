package com.programmerare.crsCodeGeneration

import freemarker.template.Configuration
import freemarker.template.Template
import freemarker.template.TemplateException
import freemarker.template.TemplateExceptionHandler

// build.gradle:
//project(':crsCodeGeneration') {
//    // command line execution
//    // gradle execute
//    // or, if somethign else than the default/hardcoded class name should be used:
//    // gradle execute -PmainClass=com.programmerare.crsCodeGeneration.ConstantClassGenerator
//    task execute(type:JavaExec) {
//        main = project.hasProperty("mainClass") ? project.getProperty("mainClass") : "com.programmerare.crsCodeGeneration.ConstantClassGenerator"
//        classpath = sourceSets.main.runtimeClasspath
//    }
//
//    dependencies {
//        // https://mvnrepository.com/artifact/org.freemarker/freemarker
//        implementation("org.freemarker:freemarker:2.3.28")
//    }
//}
class ConstantClassGenerator {

    fun run() {
        val freemarkerConfiguration = Configuration(Configuration.VERSION_2_3_28);
        println("TODO code generation ...")
        // TODO: generate classes with constants e.g. based on database with EPSG codes:
        // http://www.epsg.org/EPSGDataset/DownloadDataset.aspx
    }

    companion object {
        @JvmStatic
        fun main(args: Array<String>) {
            println("Code generation starts ... (TODO ...) ")
            val constantClassGenerator = ConstantClassGenerator()
            constantClassGenerator.run()
        }
    }
}