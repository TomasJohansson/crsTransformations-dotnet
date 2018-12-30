namespace Programmerare.CrsTransformations.Identifier {

using NUnit.Framework;
using System;

[TestFixture]
public class CrsIdentifierTest {

    private string GetCrsCodeIncludingUppercasedEpsgPrefix(int epsgNumber) {
        return "EPSG:" + epsgNumber;
    }
    
    [Test]
    public void CrsIdentifier_ShouldReturnWhitespaceTrimmedCrsCodeAndNotBeConsideredAsEpsg_WhenCreatedFromNonEpsgString() {
        CrsIdentifier crsIdentifier = CrsIdentifierFactory.CreateFromCrsCode("  abc  ");
        Assert.AreEqual("abc", crsIdentifier.CrsCode);
        Assert.AreEqual(false, crsIdentifier.IsEpsgCode);
    }

    [Test]
    public void CrsIdentifier_ShouldReturnEpsgNumberAndEpsgPrefixedCrsCodeAndBeConsideredAsEpsg_WhenCreatedFromEpsgNumber() {
        int inputEpsgNumber = 3006;
        // No validation that the number is actually an existing EPSG but any positive integer
        // is assumed to be a EPSG number
        CrsIdentifier crsIdentifier = CrsIdentifierFactory.CreateFromEpsgNumber(inputEpsgNumber);
        Assert.AreEqual(
            inputEpsgNumber, // expected
            crsIdentifier.EpsgNumber
        );
        Assert.AreEqual(GetCrsCodeIncludingUppercasedEpsgPrefix(inputEpsgNumber), crsIdentifier.CrsCode);
        Assert.AreEqual(true, crsIdentifier.IsEpsgCode);
    }

    [Test]
    public void CrsIdentifier_ShouldReturnEpsgNumberAndUppercasedEpsgPrefixedWhitespaceTrimmedCrsCodeAndBeConsideredAsEpsg_WhenCreatedFromLowecasedEpsgCodeWithSurroundingWhitespace() {
        int inputEpsgNumber = 4326;
        string inputCrsCode = "  epsg:" + inputEpsgNumber + "  "; 
        CrsIdentifier crsIdentifier = CrsIdentifierFactory.CreateFromCrsCode(inputCrsCode);
        // the input should become trimmed and return string with uppercased "EPSG:" prefix
        Assert.AreEqual(
            GetCrsCodeIncludingUppercasedEpsgPrefix(inputEpsgNumber), 
            crsIdentifier.CrsCode
        );
        Assert.AreEqual(true, crsIdentifier.IsEpsgCode);
        Assert.AreEqual(inputEpsgNumber, crsIdentifier.EpsgNumber);
    }

    [Test]
    public void CrsIdentifierFactory_ShouldThrowException_WhenCrsCodeInputIsNull() {
        ArgumentException exception = Assert.Throws<ArgumentNullException>(
            () => {
                CrsIdentifierFactory.CreateFromCrsCode(null); // should fail
            }
            , "Must not be null"
        );
        // F# code invoked above may throw exception like this:
        //  nullArg "crsCode"
        // Resulting message: "Value cannot be null. Parameter name: crsCode"
        AssertExceptionMessageWhenArgumentWasNullOrEmptyString(exception, "cannot be null");
    }

    [Test]
    public void CrsIdentifierFactory_ShouldThrowException_WhenCrsCodeInputIsOnlyWhitespace() {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => {
                CrsIdentifierFactory.CreateFromCrsCode("   "); // should fail
            }
            ,
            "Must not be empty string"
        );
        AssertExceptionMessageWhenArgumentWasNullOrEmptyString(exception, "non-empty");
    }

    [Test]
    public void CrsIdentifierFactory_ShouldThrowException_WhenCrsCodeIsEpsgWithNegativeNumber() {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => {
                CrsIdentifierFactory.CreateFromCrsCode(GetCrsCodeIncludingUppercasedEpsgPrefix(-123)); // should fail
            }
            ,
            "EPSG must not be negative"
        );
        AssertExceptionMessageWhenArgumentWasNullOrEmptyString(exception, "non-positive");
    }


    [Test]
    public void CrsIdentifierFactory_ShouldThrowException_WhenEpsgNumberIsNegative() {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => {
                CrsIdentifierFactory.CreateFromEpsgNumber(-1); // should fail
            }
            ,
            "EPSG must not be negative"
        );
        AssertExceptionMessageWhenArgumentWasNullOrEmptyString(exception, "non-positive");
    }

    [Test]
    public void CrsIdentifiers_ShouldBeEqual_WhenCreatedFromEpsgNumberAndCorrespondingCrsCode() {
        CrsIdentifier fromEpsgNumber = CrsIdentifierFactory.CreateFromEpsgNumber(3006);
        CrsIdentifier fromCrsCode = CrsIdentifierFactory.CreateFromCrsCode("  epsg:3006   ");
        Assert.AreEqual(fromEpsgNumber, fromCrsCode);
        Assert.AreEqual(fromEpsgNumber.GetHashCode(), fromCrsCode.GetHashCode());
    }

    private void AssertExceptionMessageWhenArgumentWasNullOrEmptyString(
        ArgumentException exception,
        string expectedStringToBeContainedInExceptionMessage
    ) {
        Assert.NotNull(exception);
        Assert.NotNull(exception.Message);
        // the exception message is currently something like this: "Parameter specified as non-null is null: method Programmerare.CrsTransformations.Identifier.CrsIdentifier$Companion.createFromCrsCode, parameter crsCode"
        // (potentially fragile to test the message strings but it does not really change often, and in such a rare scenario, then easy to fix)
        Assert.That(exception.Message, Does.Contain(expectedStringToBeContainedInExceptionMessage));
        // Alternative to the above test
        StringAssert.Contains(expectedStringToBeContainedInExceptionMessage, exception.Message);
    }

    [Test]
    public void CreateFromCrsCode_ShouldThrowException_WhenCrsCodeIsNull() {
        ArgumentException exception = Assert.Throws<ArgumentNullException>(
            () => {
                CrsIdentifierFactory.CreateFromCrsCode(null);
            }
            ,
            "CRS code must not be null"
        );
    }

    [Test]
    public void CrsIdentifier_ShouldNotBeEqual_WhenDifferentEpsgNumber() {
        Assert.AreNotEqual(
            CrsIdentifierFactory.CreateFromEpsgNumber(123),
            CrsIdentifierFactory.CreateFromEpsgNumber(124)
        );
    }

    [Test]
    public void CrsIdentifier_ShouldNotBeEqual_WhenDifferentCrsCode() {
        Assert.AreNotEqual(
            CrsIdentifierFactory.CreateFromCrsCode("EPSG:987"),
            CrsIdentifierFactory.CreateFromCrsCode("EPSG:986")
        );
    }

    [Test]
    public void crsIdentifier_shouldNotBeEqual_whenDifferentNonEpsgCrsCode() {
        Assert.AreNotEqual(
            CrsIdentifierFactory.CreateFromCrsCode("abc"),
            CrsIdentifierFactory.CreateFromCrsCode("abd")
        );
    }

} // class ends
} // namespace ends