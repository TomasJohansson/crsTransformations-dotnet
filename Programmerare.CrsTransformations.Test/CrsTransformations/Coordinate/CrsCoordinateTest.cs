using Programmerare.CrsTransformations.Identifier;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_5_4;

namespace Programmerare.CrsTransformations.Coordinate {

[TestFixture]
class CrsCoordinateTest {
    private const string EpsgPrefix = "EPSG:";
    
    private const double deltaTolerance = 0.00001;
    private const double xLongitude = 12.34;
    private const double yLatitude = 56.67;

    private const int epsgNumberSweref99 = EpsgNumber.SWEDEN__SWEREF99_TM__3006;
    private const int epsgNumberWgs84 = EpsgNumber.WORLD__WGS_84__4326;

    private readonly static string epsgCode = EpsgPrefix + epsgNumberSweref99;

    [Test]
    public void CoordinateProperties_ShouldHaveValuesEqualtToFactoryMethodsParameters() {
        CrsCoordinate coordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(xLongitude, yLatitude, epsgNumberSweref99);
        Assert.AreEqual(xLongitude, coordinate.XEastingLongitude, deltaTolerance);
        Assert.AreEqual(yLatitude, coordinate.YNorthingLatitude, deltaTolerance);
        Assert.AreEqual(epsgNumberSweref99, coordinate.CrsIdentifier.EpsgNumber);
    }

    [Test]
    public void Coordinates_ShouldBeEqual_WhenUsingIntegerEpsgNumberAndDifferentFactoryMethodsWithParametersInDifferentOrder() {
        CrsCoordinate coordinate1 = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(xLongitude, yLatitude, epsgNumberSweref99);
        CrsCoordinate coordinate2 = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(yLatitude, xLongitude, epsgNumberSweref99);
        AssertEqualCoordinates(coordinate1, coordinate2);
    }

    [Test]
    public void Coordinates_ShouldBeEqual_WhenUsingStringEpsgCodeAndDifferentFactoryMethodsWithParametersInDifferentOrder() {
        string crsCode = "EPSG:3006";
        CrsCoordinate coordinate1 = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(xLongitude, yLatitude, crsCode);
        CrsCoordinate coordinate2 = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(yLatitude, xLongitude, crsCode);
        AssertEqualCoordinates(coordinate1, coordinate2);
    }

    [Test]
    public void Coordinates_ShouldBeEqual_WhenUsingCrsIdentifierAndDifferentFactoryMethodsWithParametersInDifferentOrder() {
        CrsIdentifier crsIdentifier = CrsIdentifierFactory.CreateFromEpsgNumber(3006);
        CrsCoordinate coordinate1 = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(xLongitude, yLatitude, crsIdentifier);
        CrsCoordinate coordinate2 = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(yLatitude, xLongitude, crsIdentifier);
        AssertEqualCoordinates(coordinate1, coordinate2);
    }

    [Test]
    public void Coordinate_ShouldBeCreatedWithWGS84asDefaultCrs_WhenNotSpecifyingCrs() {
        CrsCoordinate coordinate = CrsCoordinateFactory.CreateFromLongitudeLatitude(xLongitude, yLatitude);
        Assert.AreEqual(
            epsgNumberWgs84, // EpsgNumber.WORLD__WGS_84__4326,
            coordinate.CrsIdentifier.EpsgNumber
        );

        // The tets below is the same as above except that the factory method use the reversed order for lat/lon parameters
        coordinate = CrsCoordinateFactory.CreateFromLatitudeLongitude(yLatitude, xLongitude);
        Assert.AreEqual(
            epsgNumberWgs84, // EpsgNumber.WORLD__WGS_84__4326,
            coordinate.CrsIdentifier.EpsgNumber
        );        
    }

    private void AssertEqualCoordinates(CrsCoordinate coordinate1, CrsCoordinate coordinate2) {
        Assert.AreEqual(coordinate1.XEastingLongitude, coordinate2.XEastingLongitude, deltaTolerance);
        Assert.AreEqual(coordinate1.YNorthingLatitude, coordinate2.YNorthingLatitude, deltaTolerance);
        Assert.AreEqual(coordinate1.CrsIdentifier, coordinate2.CrsIdentifier); // data class
        Assert.AreEqual(coordinate1.CrsIdentifier.EpsgNumber, coordinate2.CrsIdentifier.EpsgNumber);

        Assert.AreEqual(coordinate1.GetHashCode(), coordinate2.GetHashCode());
        Assert.AreEqual(coordinate1, coordinate2);
    }

     // six decimals are commonly used for latitude and longitude values 
    [Test]
    public void CoordinateWithSixDecimals_ShouldBeEqualToCoordinateConstructedWithTheSameValues_WhenTheOnlyDifferenceIsSomeAdditionalZeroes() {
        CrsCoordinate c1 = CrsCoordinateFactory.CreateFromLatitudeLongitude(59.123456, 18.123456000);
        CrsCoordinate c2 = CrsCoordinateFactory.CreateFromLatitudeLongitude(59.123456000, 18.123456);
        Assert.AreEqual(
            c1, c2
        );
        Assert.AreEqual(
            c1.GetHashCode(), c2.GetHashCode()
        );
    }

    [Test]
    public void CoordinateWithNineDecimals_ShouldBeEqualToCoordinateConstructedWithTheSameValues_WhenTheOnlyDifferenceIsSomeAdditionalZeroes() {
        CrsCoordinate c1 = CrsCoordinateFactory.CreateFromLatitudeLongitude(59.123456789, 18.123456789000);
        CrsCoordinate c2 = CrsCoordinateFactory.CreateFromLatitudeLongitude(59.123456789000, 18.123456789);
        Assert.AreEqual(
            c1, c2
        );
        Assert.AreEqual(
            c1.GetHashCode(), c2.GetHashCode()
        );
    }

    [Test]
    public void Coordinates_ShouldNotBeEqual_WhenDifferenceAtTwelfthDecimalOfLatitude() {
        // very small latitude difference:
        Assert.AreNotEqual(
            CrsCoordinateFactory.CreateFromLatitudeLongitude(59.123456789001, 18.123456789),
            CrsCoordinateFactory.CreateFromLatitudeLongitude(49.123456789002, 18.123456789)
        );
    }

    [Test]
    public void Coordinates_ShouldNotBeEqual_WhenDifferenceAtTwelfthDecimalOfLongitude() {
        // very small longitude difference:
        Assert.AreNotEqual(
            CrsCoordinateFactory.CreateFromLatitudeLongitude(59.123456789, 18.123456789000),
            CrsCoordinateFactory.CreateFromLatitudeLongitude(59.123456789, 18.123456789001)
        );
    }

    [Test]
    public void Coordinates_ShouldBeEqual_WhenCreatedWithTheSameValuesButDifferentFactoryMethods() {
        CrsIdentifier crsIdentifier = CrsIdentifierFactory.CreateFromEpsgNumber(epsgNumberSweref99);
        string epsgCode = EpsgPrefix + epsgNumberSweref99;
        
        CrsCoordinate expectedCoordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(xLongitude, yLatitude, epsgNumberSweref99);
        // all should be equal to each other, so one of them was chosen above 
        // as the "expected" and then the others are compared with it in the below assertions

        // -----------------------------------------------------------------------
        // the last parameter (epsgNumber) is an integer in the first below assertions:
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.LonLat(xLongitude, yLatitude, epsgNumberSweref99)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.XY(xLongitude, yLatitude, epsgNumberSweref99)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.EastingNorthing(xLongitude, yLatitude, epsgNumberSweref99)
        );

        // the below four assertions are using x/y values in the opposite order 
        // compared to the above three assertions
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(yLatitude, xLongitude, epsgNumberSweref99)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.LatLon(yLatitude, xLongitude, epsgNumberSweref99)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.YX(yLatitude, xLongitude, epsgNumberSweref99)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.NorthingEasting(yLatitude, xLongitude, epsgNumberSweref99)
        );

        // -----------------------------------------------------------------------
        // epsg code (string parameter) is the last parameter below
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(xLongitude, yLatitude, epsgCode)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.LonLat(xLongitude, yLatitude, epsgCode)   
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.XY(xLongitude, yLatitude, epsgCode)   
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.EastingNorthing(xLongitude, yLatitude, epsgCode)  
        );

        // the below four assertions are using x/y values in the opposite order 
        // compared to the above four assertions

        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(yLatitude, xLongitude, epsgCode)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.LatLon(yLatitude, xLongitude, epsgCode)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.YX(yLatitude, xLongitude, epsgCode)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.NorthingEasting(yLatitude, xLongitude, epsgCode)
        );        

        // -----------------------------------------------------------------------
        // crsIdentifier obkect is the last parameter below
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(xLongitude, yLatitude, crsIdentifier)   
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.LonLat(xLongitude, yLatitude, crsIdentifier)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.XY(xLongitude, yLatitude, crsIdentifier)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.EastingNorthing(xLongitude, yLatitude, crsIdentifier)
        );

        // the below four assertions are using x/y values in the opposite order 
        // compared to the above four assertions

        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(yLatitude, xLongitude, crsIdentifier)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.LatLon(yLatitude, xLongitude, crsIdentifier)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.YX(yLatitude, xLongitude, crsIdentifier)
        );
        AssertEqualCoordinates(expectedCoordinate,
            CrsCoordinateFactory.NorthingEasting(yLatitude, xLongitude, crsIdentifier)
        );        
    }

    [Test]
    public void Coordinate_ShouldHaveEquivalentXEastingLongitudeProperties() {
        CrsCoordinate c = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(yLatitude, xLongitude, epsgCode);
        Assert.AreEqual(c.XEastingLongitude, c.X);
        Assert.AreEqual(c.XEastingLongitude, c.Easting);
        Assert.AreEqual(c.XEastingLongitude, c.Longitude);
    }

    [Test]
    public void Coordinate_ShouldHaveEquivalentgetYNorthingLatitudeProperties() {
        CrsCoordinate c = CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude(yLatitude, xLongitude, epsgCode);
        Assert.AreEqual(c.YNorthingLatitude, c.Y);
        Assert.AreEqual(c.YNorthingLatitude, c.Northing);
        Assert.AreEqual(c.YNorthingLatitude, c.Latitude);
    }

    // throw IllegalArgumentException("Neither of the two coordinate parameters must be null i.e. neither 'X / Easting / Longitude' nor 'Y / Northing / Latitude'")
    // fragile hardcoded string below but will not change often and if/when then it will be easy to fix when it fails
    private const string EXPECTED_PART_OF_EXCEPTION_MESSAGE_WHEN_COORDINATE_VALUES_IS_NULL = "Neither of the two coordinate parameters must be null";

    [Test]
    public void CreateFromXEastingLongitudeAndYNorthingLatitude_ShouldThrowException_WhenCrsIdentifierIsNull() {
        CrsIdentifier crsIdentifier = null;
        ArgumentException exception = Assert.Throws<ArgumentNullException>( () => {
            CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(
                60.0,
                20.0,
                crsIdentifier
            );
        });
        // F# may throw this: nullArg "crsIdentifier"
        // which creates the following kind of message:
        // "Value cannot be null. Parameter name: crsIdentifier"
        AssertExceptionMessageForIllegalArgumentException(exception, "crsIdentifier");
    }

    [Test]
    public void CreateFromXEastingLongitudeAndYNorthingLatitude_ShouldThrowException_WhenCrsCodeIsNull() {
        string crsCode = null;
        ArgumentException exception = Assert.Throws<ArgumentNullException>(
            () => {
                CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(
                    60.0,
                    20.0,
                    crsCode
                );
            }
        );
        // F# may throw this: nullArg "crsCode"
        // which creates the following kind of message:
        // "Value cannot be null. Parameter name: crsCode"
        AssertExceptionMessageForIllegalArgumentException(exception, "crsCode");
    }

    private void AssertExceptionMessageForIllegalArgumentException(
        ArgumentException exception, 
        string partOfExceptionMessage
    ) {
        Assert.NotNull(exception);
        string actualEceptionMessage = exception.Message;
        // fragile hardcoded strings below but will not change often and if/when then it will be easy to fix when it fails
        // actualEceptionMessage for example: "Value cannot be null. Parameter name: crsIdentifier"
        //string expectedEceptionMessagePart1 = "Parameter specified as non-null is null:";
        //string expectedEceptionMessagePart2 = "parameter " + suffixWithNameOfParameter;
        //Assert.That(exception.Message, Does.Contain(expectedEceptionMessagePart1));
        //Assert.That(exception.Message, Does.Contain(expectedEceptionMessagePart2));
        Assert.That(exception.Message, Does.Contain(partOfExceptionMessage));
    }

    [Test]
    public void CreateCoordinate_ShouldThrowException_WhenXorYIsNotValidNumber() {
        // The iteration of three lists in this method
        // is an alternative to implementing 
        // 48 methods with lots of duplication !
        // ( 3 * 2 * 8 = 48 )
        var unvalidNumbers = new List<double> {
            double.NaN, 
            double.PositiveInfinity, 
            double.NegativeInfinity
        };
        var unvalidNumberShouldBeUsedAsFirstParameter = new List<bool> {
            true, false
        };
        var factoryMethods = new List<Func<double, double, int, CrsCoordinate>>{
            CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude,
            CrsCoordinateFactory.CreateFromYNorthingLatitudeAndXEastingLongitude,
            CrsCoordinateFactory.XY,
            CrsCoordinateFactory.YX,
            CrsCoordinateFactory.EastingNorthing,
            CrsCoordinateFactory.NorthingEasting,
            CrsCoordinateFactory.LonLat,
            CrsCoordinateFactory.LatLon
        };
        foreach(var factoryMethod in factoryMethods) {
            foreach(bool useUnvalidNumberAsFirstParameter in unvalidNumberShouldBeUsedAsFirstParameter) {
                foreach(double unvalidNumber in unvalidNumbers) {
                    ArgumentException exception = Assert.Throws<ArgumentException>( () => {
                        if(useUnvalidNumberAsFirstParameter) {
                            factoryMethod(unvalidNumber, 50.0, epsgNumberSweref99);
                        }
                        else {
                            factoryMethod(50.0, unvalidNumber, epsgNumberSweref99);
                        }
                    });
                    // F# may throw something like this: invalidArg "Coordinate not valid: NaN"
                    // and below we test that the message at least contain the substring "NaN"
                    string partOfTheExpectedExceptionMessage = unvalidNumber.ToString();
                    // for example, the above string may be "NaN" and then we want that string to occur
                    // in the exception message
                    AssertExceptionMessageForIllegalArgumentException(
                        exception, 
                        partOfTheExpectedExceptionMessage
                    );
                }
            }
        }
    }

} // class ends
} // namespace ends