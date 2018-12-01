namespace Programmerare.CrsTransformations.TestData
{

// To run tests from a certain category in Visual Studio:
// Test -> Windows -> Test Explorer -> Group By -> Traits
// ("Traits" are mapped to NUnit Category)
// Note that you may habe to disable "Show test category"
// (first little button in Test Explorer) 
//  before being able to select anything 
// from the "Group By" dropdown
// (first little button in Test Explorer) 

/**
 * Some of the code which is executable through NUnit are not actually
 * tests in the normal sense i.e. invoking assertions,
 * but rather is used a way of running code as an alternative to a main method.
 * The above comment is mostly applicable for the class
 * CoordinateTestDataGeneratedFromEpsgDatabaseTest
 * which does some output to the console or creating output files.
 * The class might also be labeled as Disabled to avoid it from being
 * run too often.
 * Those "tests" should mainly be used when having upgraded some of the third
 * part libraries with new implementations, for trying to detect differences
 * compared to the coordinates calculated with previous versions.
 * Those "tests" may also be used for trying to find significant differerences
 * between different libraries for certain EPSG codes in the big input file
 * with lots of EPSG code coordinates.
 */
public class TestCategory {

    /**
     * Used for a "test" method (see above class level comment) creating some file.
     */
    public const string SideEffectFileCreation = "SideEffectFileCreation";

    /**
     * Used for a "test" method (see above class level comment) producing output to the console with 'System.out.print' statements
     */
    public const string SideEffectPrintingConsoleOutput = "SideEffectPrintingConsoleOutput";


    /**
     * Used for a method which can take more than one minute (and potentiall much more than one minute)
     */
    public const string SlowTest = "SlowTest";
}
}