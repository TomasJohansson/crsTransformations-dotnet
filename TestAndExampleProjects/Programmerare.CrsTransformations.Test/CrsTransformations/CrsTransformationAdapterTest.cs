using System;
using System.Collections.Generic;
using NUnit.Framework;
using Programmerare.CrsConstants.ConstantsByAreaNameNumber.v9_8_9;
using Programmerare.CrsTransformations.Coordinate;
using Programmerare.CrsTransformations.Adapter.MightyLittleGeodesy;
using Programmerare.CrsTransformations.Adapter.DotSpatial;
using Programmerare.CrsTransformations.Identifier;

namespace Programmerare.CrsTransformations.Core {
[TestFixture]
public class CrsTransformationAdapterTest : CrsTransformationTestBase {
	
	public const int EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS = 3;

    // the keyword "base" is not needed but is still used in this test class 
    // to make it obvious that some variables ar defined and populated in a base class
    
    private string unvalidCrsCode;
    private CrsCoordinate validInputCoordinate;

    [SetUp]
    public void SetUpCrsTransformationAdapterTest() {
        unvalidCrsCode = "This string is NOT a correct crs/EPSG code";
        validInputCoordinate = CrsCoordinateFactory.LatLon(
            60.0, // ok wgs84 latitude
            20.0, // ok wgs84 longitude
            epsgNumberForWgs84 // OK
        );
    }

    [Test]
    public void BaseClass_ShouldHaveCreatedThreeLeafAndFourCompositeImplementations() {
        int expectedNumberOfLeafs = EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS;
        int expectedNumberOfComposites = 4; // will not change often (if ever) and if/when changed then will be easy to fix this hardcoding when it fails
        Assert.AreEqual(
            expectedNumberOfLeafs, 
            base.crsTransformationAdapterLeafImplementations.Count
        );
        Assert.AreEqual(
            expectedNumberOfComposites, 
            base.crsTransformationAdapterCompositeImplementations.Count
        );
        Assert.AreEqual(
            expectedNumberOfLeafs + expectedNumberOfComposites,
            base.crsTransformationAdapterImplementations.Count
        );
    }
    
    [Test]
    public void TransformToCoordinate_ShouldReturnCorrectSweref99TMcoordinate_WhenTransformingFromWgs84withAllAdapterImplementations() {
        foreach (ICrsTransformationAdapter crsTransformationAdapter in base.crsTransformationAdapterImplementations) {
            TransformToCoordinate_ShouldReturnCorrectSweref99TMcoordinate_WhenTransformingFromWgs84(crsTransformationAdapter);
        }
    }

    [Test]
    public void TransformToCoordinate_ShouldReturnCorrectWgs84coordinate_WhenTransformingFromRT90withAllAdapterImplementations() {
        foreach (ICrsTransformationAdapter crsTransformationAdapter in base.crsTransformationAdapterImplementations) {
            TransformToCoordinate_ShouldReturnCorrectWgs84coordinate_WhenTransformingFromRT90(crsTransformationAdapter);
        }
    }

    [Test]
    public void Transform_ShouldSuccess_WhenInputCoordinateIsCorrect() {
        double wgs84Lat = 59.330231;
        double wgs84Lon = 18.059196;
        CrsCoordinate wgs84InputCoordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(wgs84Lon, wgs84Lat, base.epsgNumberForWgs84);

        foreach (ICrsTransformationAdapter crsTransformationAdapter in base.crsTransformationAdapterImplementations) {
            CrsTransformationResult transformResult = crsTransformationAdapter.Transform(wgs84InputCoordinate, base.epsgNumberForSweref99TM);
            string errorMessage = "Problem implementation: " + crsTransformationAdapter.AdapteeType;
            Assert.IsNotNull(transformResult, errorMessage);
            Assert.IsTrue(transformResult.IsSuccess, errorMessage);
            Assert.IsNull(transformResult.Exception, errorMessage);
            CrsCoordinate outputCoordinate = transformResult.OutputCoordinate;
            Assert.IsNotNull(outputCoordinate, errorMessage);
            Assert.AreEqual(outputCoordinate.CrsIdentifier.EpsgNumber, base.epsgNumberForSweref99TM, errorMessage);
            if(!crsTransformationAdapter.IsComposite) {
                AssertResultStatisticsForLeafImplementation(transformResult);
            }
        }
    }


    [Test]
    public void Transform_ShouldReturnSuccessFalseButNotThrowException_WhenLongitudeIsNotValid() {
        CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.LatLon(
            60.0, // ok wgs84 latitude
            -9999999999.0, // NOT ok wgs84 longitude
            epsgNumberForWgs84
        );
        Transform_ShouldReturnSuccessFalseButNotThrowException_WhenCoordinateIsNotValid(unvalidInputCoordinate);
    }

    [Test]
    public void Transform_ShouldReturnSuccessFalseButNotThrowException_WhenLatitudeIsNotValid() {
        CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.LatLon(
            -9999999999.0, // NOT ok wgs84 latitude
            20.0, // ok wgs84 longitude
            epsgNumberForWgs84
        );
        Transform_ShouldReturnSuccessFalseButNotThrowException_WhenCoordinateIsNotValid(unvalidInputCoordinate);
    }

    [Test]
    public void Transform_ShouldReturnSuccessFalseButNotThrowException_WhenCrsCodeIsNotValid() {
        CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.LatLon(
            60.0, // ok wgs84 latitude
            20.0, // ok wgs84 longitude
            unvalidCrsCode
        );
        Transform_ShouldReturnSuccessFalseButNotThrowException_WhenCoordinateIsNotValid(unvalidInputCoordinate);
    }
    
    private void Transform_ShouldReturnSuccessFalseButNotThrowException_WhenCoordinateIsNotValid(
        CrsCoordinate unvalidInputCoordinate
    ) {
        foreach (ICrsTransformationAdapter crsTransformationAdapter in crsTransformationAdapterImplementations) {
            string messageWhenError = "Problem with the implementation " + crsTransformationAdapter.AdapteeType;
            CrsTransformationResult transformResult = crsTransformationAdapter.Transform(unvalidInputCoordinate, epsgNumberForSweref99TM);
            Assert.IsNotNull(transformResult, messageWhenError);
            if(!unvalidInputCoordinate.CrsIdentifier.IsEpsgCode) {
                // if the coordinate is unvalid becasue of incorrect crs code
                // then we can do the below assertions but if the coordinate values 
                // are unreasonable it may not be detected by the implementations
                // and we can not expect them to return success=false 
                Assert.IsFalse(transformResult.IsSuccess, messageWhenError);
                Assert.IsNotNull(transformResult.Exception, messageWhenError);
                Assert.AreEqual(unvalidInputCoordinate, transformResult.InputCoordinate, messageWhenError);
            }
        }        
    }

    // These two test below could not be used since all implementations are 
    // not throwing exceptions for unreasonable coordinate values    
    //[Test]
    //public void TransformToCoordinate_shouldThrowException_whenLongitudeIsNotValid() {
    //    CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.LatLon(
    //        60.0, // ok wgs84 latitude
    //        -9999999999.0, // NOT ok wgs84 longitude
    //        epsgNumberForWgs84
    //    );
    //    transformToCoordinate_shouldThrowException_whenCoordinateIsNotValid(unvalidInputCoordinate);
    //}
    //[Test]
    //public void TransformToCoordinate_shouldThrowException_whenLatitudeIsNotValid() {
    //    CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.LatLon(
    //        -9999999999.0, // NOT ok wgs84 latitude
    //        20.0, // ok wgs84 longitude
    //        epsgNumberForWgs84
    //    );
    //    transformToCoordinate_shouldThrowException_whenCoordinateIsNotValid(unvalidInputCoordinate);
    //}

    [Test]
    public void TransformToCoordinate_ShouldThrowException_WhenCrsCodeIsNotValid() {
        CrsCoordinate unvalidInputCoordinate = CrsCoordinateFactory.LatLon(
            60.0, // ok wgs84 latitude
            20.0, // ok wgs84 longitude
            unvalidCrsCode
        );
        TransformToCoordinate_ShouldThrowException_WhenCoordinateIsNotValid(unvalidInputCoordinate);
    }

    private void TransformToCoordinate_ShouldThrowException_WhenCoordinateIsNotValid(
        CrsCoordinate unvalidInputCoordinate
    ) {
        foreach (ICrsTransformationAdapter crsTransformationAdapter in crsTransformationAdapterImplementations) {
            //Exception exception = Assert.Throws<Exception>( () => {
            //    crsTransformationAdapter.TransformToCoordinate(unvalidInputCoordinate, base.epsgNumberForSweref99TM);
            //}
            //,
            //"Exception was not thrown but SHOULD have been thrown for implementation " + crsTransformationAdapter.AdapteeType + " and coordinate " + unvalidInputCoordinate 
            //);
                
            // The above 'Assert.Throws' test for a specific exception type 
            // while the code below (with 'Assert.That' and then Assert.That' works for any exception

            Assert.That(
                () => crsTransformationAdapter.TransformToCoordinate(
                    unvalidInputCoordinate, 
                    base.epsgNumberForSweref99TM
                ),
                Throws.Exception
                    // testing that the thrown exception type is one of the following:
                    .TypeOf<ArgumentOutOfRangeException>().Or
                    .TypeOf<NotSupportedException>().Or
                    .TypeOf<ArgumentException>().Or
                    .TypeOf<Exception>()
            );
        }
    }

    [Test]
    public void TransformToCoordinate_shouldThrowException_whenTargetCrsIsNotValid() {
        foreach (ICrsTransformationAdapter crsTransformationAdapter in crsTransformationAdapterImplementations) {
            Assert.That(
                () => crsTransformationAdapter.TransformToCoordinate(
                    validInputCoordinate,
                    unvalidCrsCode
                ),
                Throws.Exception
                    // testing that the thrown exception type is one of the following:
                    .TypeOf<ArgumentOutOfRangeException>().Or
                    .TypeOf<NotSupportedException>().Or
                    .TypeOf<ArgumentException>().Or
                    .TypeOf<Exception>()
            );
        }
    }

    [Test]
    public void TransformToCoordinate_ShouldThrowException_WhenCoordinateIsNull() {
        foreach (ICrsTransformationAdapter crsTransformationAdapter in crsTransformationAdapterImplementations) {
            Assert.That(
                () => crsTransformationAdapter.TransformToCoordinate(
                    null,  
                    base.epsgNumberForSweref99TM
                ),
                Throws.Exception 
                    .TypeOf<ArgumentNullException>(),
                "Failing adapter: " + crsTransformationAdapter.GetType().FullName
            );
        }
    }

    [Test]
    public void TransformResult_ShouldContainArgumentNullException_WhenCoordinateIsNull() {
        foreach (ICrsTransformationAdapter crsTransformationAdapter in crsTransformationAdapterImplementations) {
            var transformationResult = crsTransformationAdapter.Transform(
                null,  
                base.epsgNumberForSweref99TM
            );
            Assert.IsNotNull(transformationResult);
            Assert.IsNotNull(transformationResult.Exception);
            Assert.AreEqual(
                transformationResult.Exception.GetType(), 
                typeof(ArgumentNullException),
                "Failing adapter: " + crsTransformationAdapter.GetType().FullName
            );
        }
    }

    [Test]
    public void TransformResult_ShouldContainArgumentNullException_WhenCrsIdentifierIsNull() {
        CrsIdentifier crsIdentifierNull = null;
        foreach (ICrsTransformationAdapter crsTransformationAdapter in crsTransformationAdapterImplementations) {
            var transformationResult = crsTransformationAdapter.Transform(
                this.validInputCoordinate,
                crsIdentifierNull
            );
            Assert.IsNotNull(transformationResult);
            Assert.IsNotNull(transformationResult.Exception);
            Assert.AreEqual(
                transformationResult.Exception.GetType(), 
                typeof(ArgumentNullException),
                "Failing adapter: " + crsTransformationAdapter.GetType().FullName
            );
        }
    }

    [Test]
    public void TransformToCoordinate_ShouldThrowException_WhenCrsIdentifierIsNull() {
        CrsIdentifier crsIdentifierNull = null;
        foreach (ICrsTransformationAdapter crsTransformationAdapter in crsTransformationAdapterImplementations) {
            Assert.That(
                () => crsTransformationAdapter.TransformToCoordinate(
                    this.validInputCoordinate,  
                    crsIdentifierNull
                ),
                Throws.Exception 
                    .TypeOf<ArgumentNullException>()
            );
        }
    }

    [Test]
    public void Transform_ShouldNotThrowException_WhenTargetCrsIsNotValid() {
        foreach (ICrsTransformationAdapter crsTransformationAdapter in crsTransformationAdapterImplementations) {
            var res = crsTransformationAdapter.Transform(
                validInputCoordinate,
                unvalidCrsCode
            );
            // The main part of the test (as in the test method name)
            // is that the above Transform method should NOT 
            // throw an exception but since we have got a result 
            // we can also verify that the transformation was a failure:
            Assert.IsFalse(res.isSuccess);
        }
    }

    [Test]
    public void IsComposite_ShouldReturnTrue_WhenComposite() {
        foreach (ICrsTransformationAdapter compositeAdapter in base.crsTransformationAdapterCompositeImplementations) {
            Assert.IsTrue(compositeAdapter.IsComposite);
        }
    }
    
    [Test]
    public void IsComposite_ShouldReturnFalse_WhenLeaf() {
        foreach (ICrsTransformationAdapter leafAdapter in base.crsTransformationAdapterLeafImplementations) {
            Assert.IsFalse(leafAdapter.IsComposite);
        }
    }    

    [Test]
    public void TransformationAdapterChildren_ShouldReturnNonEmptyList_WhenComposite() {
        // all leafs should be children 
        int expectedNumberOfChildrenForTheComposites = base.crsTransformationAdapterLeafImplementations.Count;
        Assert.That(
            //"Has the number of leaf implementations been reduced?",
            expectedNumberOfChildrenForTheComposites, 
            Is.GreaterThanOrEqualTo(EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS)
        ); 
        foreach (ICrsTransformationAdapter compositeAdapter in base.crsTransformationAdapterCompositeImplementations) {
            Assert.AreEqual(
                expectedNumberOfChildrenForTheComposites,
                compositeAdapter.TransformationAdapterChildren.Count
            );    
        }
    }

    [Test]
    public void TransformationAdapterChildren_ShouldReturnEmptyList_WhenLeaf() {
        int zeroExpectedNumberOfChildren = 0;
        foreach (ICrsTransformationAdapter leafAdapter in base.crsTransformationAdapterLeafImplementations) {
            Assert.AreEqual(
                zeroExpectedNumberOfChildren,
                leafAdapter.TransformationAdapterChildren.Count
            );
        }
    }
    
    [Test]
    public void IsReliable_ShouldReturnTrueForLeafs_WhenUsingCriteriaNumberOfResultsOneAndMaxDiffZero() {
        int criteriaNumberOfResults = 1; // always one for a Leaf
        double criteriaMaxDiff = 0.0; // always zero for a Leaf
        // The tested method 'IsReliable' is actually relevant only for aggregated
        // transformations, but nevertheless there is a reaonable behavouor also
        // for the "Leaf" implementations regarding the number of results (always 1)
        // and the "differences" in lat/long for the "different" implementations
        // i.e. the "difference" should always be zero since there is only one implementation

        List<ICrsTransformationAdapter> crsTransformationAdapterImplementationsExpectingOneResult = new List<ICrsTransformationAdapter>();
        crsTransformationAdapterImplementationsExpectingOneResult.AddRange(base.crsTransformationAdapterLeafImplementations);
        crsTransformationAdapterImplementationsExpectingOneResult.Add(crsTransformationAdapterCompositeFactory.CreateCrsTransformationFirstSuccess());
        Assert.AreEqual(
			EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS + 1,  // 3 leafs plus 1 
			crsTransformationAdapterImplementationsExpectingOneResult.Count
		);
        
        foreach (ICrsTransformationAdapter crsTransformationAdapterLeaf in crsTransformationAdapterImplementationsExpectingOneResult) {
            // suffix "Leaf" is not quite true, but in one of the iterations
            // it will be the Composite FirstSuccess which also will only have one result 
            // and thus can be tested in the same way as the leafs in this method
            CrsCoordinate wgs84coordinateInSweden = CrsCoordinateFactory.LatLon(59.29,18.03);
            CrsTransformationResult resultWhenTransformingToSwedishCRS = crsTransformationAdapterLeaf.Transform(wgs84coordinateInSweden, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
            Assert.IsNotNull(resultWhenTransformingToSwedishCRS);
            Assert.IsTrue(resultWhenTransformingToSwedishCRS.IsSuccess);
            CrsTransformationResultStatistic crsTransformationResultStatistic = resultWhenTransformingToSwedishCRS.CrsTransformationResultStatistic;
            Assert.IsNotNull(crsTransformationResultStatistic);
            Assert.IsTrue(crsTransformationResultStatistic.IsStatisticsAvailable);
    
            int actualNumberOfResults = crsTransformationResultStatistic.NumberOfPotentiallySuccesfulResults;
            Assert.AreEqual(criteriaNumberOfResults, actualNumberOfResults);
            double actualMaxDiffXLongitude = crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude;
            double actualMaxDiffYLatitude = crsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude;
            double actualMaxDiffXorY = Math.Max(actualMaxDiffXLongitude, actualMaxDiffYLatitude);
            Assert.AreEqual(criteriaMaxDiff, actualMaxDiffXorY); // zero differences since there should be only one result !

            // method "IsReliable" used below is the method under test
            
            Assert.IsTrue(
                resultWhenTransformingToSwedishCRS.IsReliable(
                    criteriaNumberOfResults, 
                    criteriaMaxDiff
                )
            );
    
            // Assert.IsFalse below since trying to require one more result than available
            Assert.IsFalse(
                resultWhenTransformingToSwedishCRS.IsReliable(
                    criteriaNumberOfResults + 1,
                    criteriaMaxDiff
                )
            );
    
            // Assert.IsFalse below since trying to require too small maxdiff
            Assert.IsFalse(
                resultWhenTransformingToSwedishCRS.IsReliable(
                    criteriaNumberOfResults, 
                    criteriaMaxDiff - 0.00000000001
                )
            );
        }
    }

    [Test]
    public void IsReliable_ShouldReturnTrueOrFalseForComposites_DependingOnCriteriasUsedAsMethodParameter() {
        // the below value is the max difference when comparing the three leaf implementations 
        double actualMaxDiffXorY = 0.0032574664801359177;
        // the above value is obviously fragile and might change if/when some adaptee library is upgraded
       
        int criteriaNumberOfResultsSuccess = EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS; // all 3 should succeed
        int criteriaNumberOfResultsFailure = EXPECTED_NUMBER_OF_ADAPTER_LEAF_IMPLEMENTATIONS + 1; // 4 implementations can not currently succeed since there are currently not so many implementations
        double criteriaMaxDiffFailure = actualMaxDiffXorY - 0.0001; // a little too small requirement for max difference
        double criteriaMaxDiffSuccess  = actualMaxDiffXorY + 0.0001; // should result in success since the actual number is smaller 

        List<ICrsTransformationAdapter> crsTransformationAdapterImplementationsExpectingManyResults = new List<ICrsTransformationAdapter>();
        foreach (ICrsTransformationAdapter crsTransformationAdapterComposite in base.crsTransformationAdapterCompositeImplementations) {
            if(crsTransformationAdapterComposite.AdapteeType != CrsTransformationAdapteeType.COMPOSITE_FIRST_SUCCESS) {
                crsTransformationAdapterImplementationsExpectingManyResults.Add(crsTransformationAdapterComposite);
            }
        }
        Assert.AreEqual(
            3, // 3 composites i.e. all composite except COMPOSITE_FIRST_SUCCESS
            crsTransformationAdapterImplementationsExpectingManyResults.Count
        ); 
        
        foreach (ICrsTransformationAdapter crsTransformationAdapterComposite in crsTransformationAdapterImplementationsExpectingManyResults) {
            CrsCoordinate wgs84coordinateInSweden = CrsCoordinateFactory.LatLon(59.29,18.03);
            CrsTransformationResult resultWhenTransformingToSwedishCRS = crsTransformationAdapterComposite.Transform(wgs84coordinateInSweden, EpsgNumber.SWEDEN__SWEREF99_TM__3006);
            Assert.IsNotNull(resultWhenTransformingToSwedishCRS);
            Assert.IsTrue(resultWhenTransformingToSwedishCRS.IsSuccess);
            CrsTransformationResultStatistic crsTransformationResultStatistic = resultWhenTransformingToSwedishCRS.CrsTransformationResultStatistic;
            Assert.IsNotNull(crsTransformationResultStatistic);
            Assert.IsTrue(crsTransformationResultStatistic.IsStatisticsAvailable);
            // so far the same code as the previous test method for the test method with leafs
            
            int actualNumberOfResults = crsTransformationResultStatistic.NumberOfPotentiallySuccesfulResults;
            Assert.AreEqual(
				criteriaNumberOfResultsSuccess, 
				actualNumberOfResults
			);
            double actualMaxDiffXLongitude = crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude;
            double actualMaxDiffYLatitude = crsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude;
            // double actualMaxDiffXorY = Math.max(actualMaxDiffXLongitude, actualMaxDiffYLatitude);
            // System.out.println("actualMaxDiffXorY " + actualMaxDiffXorY + "  " + crsTransformationAdapterComposite.getAdapteeType());

            // method "isReliable" used below is the method under test
            Assert.IsTrue(
                resultWhenTransformingToSwedishCRS.IsReliable(
                    criteriaNumberOfResultsSuccess,
                    criteriaMaxDiffSuccess
                )
            );
            Assert.IsFalse(
                resultWhenTransformingToSwedishCRS.IsReliable(
                    criteriaNumberOfResultsSuccess,
                    criteriaMaxDiffFailure
                )
            );
            Assert.IsFalse(
                resultWhenTransformingToSwedishCRS.IsReliable(
                    criteriaNumberOfResultsFailure,
                    criteriaMaxDiffSuccess
                )
            );
            Assert.IsFalse(
                resultWhenTransformingToSwedishCRS.IsReliable(
                    criteriaNumberOfResultsFailure,
                    criteriaMaxDiffFailure
                )
            );
        }
    }

    private void TransformToCoordinate_ShouldReturnCorrectSweref99TMcoordinate_WhenTransformingFromWgs84(
        ICrsTransformationAdapter crsTransformationAdapter
    ) {
        // This test is using the coordinates of Stockholm Centralstation (Sweden)
        // https://kartor.eniro.se/m/03Yxp
        // WGS84 decimal (lat, lon)
        // 59.330231, 18.059196
        // SWEREF99 TM (nord, öst)
        // 6580822, 674032

        double wgs84Lat = 59.330231;
        double wgs84Lon = 18.059196;

        double sweref99_Y_expected = 6580822;
        double sweref99_X_expected = 674032;

        CrsCoordinate inputCoordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(wgs84Lon, wgs84Lat, base.epsgNumberForWgs84);
        CrsCoordinate outputCoordinate = crsTransformationAdapter.TransformToCoordinate(inputCoordinate, base.epsgNumberForSweref99TM);
        Assert.AreEqual(sweref99_Y_expected, outputCoordinate.YNorthingLatitude, 0.5);
        Assert.AreEqual(sweref99_X_expected, outputCoordinate.XEastingLongitude, 0.5);
    }

    private void TransformToCoordinate_ShouldReturnCorrectWgs84coordinate_WhenTransformingFromRT90(
        ICrsTransformationAdapter crsTransformationAdapter
    ) {
        double rt90_Y = 6580994;
        double rt90_X = 1628294;

        double wgs84Lat_expected = 59.330231;
        double wgs84Lon_expected = 18.059196;

        CrsCoordinate inputCoordinate = CrsCoordinateFactory.CreateFromXEastingLongitudeAndYNorthingLatitude(rt90_X, rt90_Y, base.epsgNumberForRT90);

        CrsCoordinate outputCoordinate = crsTransformationAdapter.TransformToCoordinate(inputCoordinate, base.epsgNumberForWgs84);
        Assert.AreEqual(wgs84Lat_expected, outputCoordinate.YNorthingLatitude, 0.1);
        Assert.AreEqual(wgs84Lon_expected, outputCoordinate.XEastingLongitude, 0.1);
    }

    private void AssertResultStatisticsForLeafImplementation(
        CrsTransformationResult transformResult
    ) {
        CrsTransformationResultStatistic crsTransformationResultStatistic = transformResult.CrsTransformationResultStatistic;
        Assert.IsNotNull(crsTransformationResultStatistic);
        Assert.IsTrue(crsTransformationResultStatistic.IsStatisticsAvailable);
        Assert.AreEqual(1, crsTransformationResultStatistic.NumberOfPotentiallySuccesfulResults);
        Assert.AreEqual(0, crsTransformationResultStatistic.MaxDifferenceForYNorthingLatitude);
        Assert.AreEqual(0, crsTransformationResultStatistic.MaxDifferenceForXEastingLongitude);
        Assert.AreEqual(transformResult.OutputCoordinate, crsTransformationResultStatistic.CoordinateAverage);
        Assert.AreEqual(transformResult.OutputCoordinate, crsTransformationResultStatistic.CoordinateMedian);
    }

    [Test]
    public void TransformToCoordinate_ShouldBePossibleToInvokeAndProduceTheSameResult_WhenTheInstanceIsTypedWithTheClassOrTheInterfaceSubTypes() {
        // Before the git commit when this test method 
        // and this comment was added, only the below a4 object
        // could invoke the method 'TransformToCoordinate'.
        // (because of F# explicit interfaces)
        // The other objects a1, a2 and a3 now also can invoke
        // the same method because of the "implicit interfaces"
        // implementation which was added in the same git commit as 
        // this comment and test method
        CrsTransformationAdapterMightyLittleGeodesy a1 = new CrsTransformationAdapterMightyLittleGeodesy();
        CrsTransformationAdapterBaseLeaf a2 = a1;
        CrsTransformationAdapterBase a3     = a1;
        ICrsTransformationAdapter a4        = a1;
        CrsCoordinate c1, c2, c3, c4;
        c1 = a1.TransformToCoordinate(validInputCoordinate, epsgNumberForSweref99TM);
        c2 = a2.TransformToCoordinate(validInputCoordinate, epsgNumberForSweref99TM);
        c3 = a3.TransformToCoordinate(validInputCoordinate, epsgNumberForSweref99TM);
        c4 = a4.TransformToCoordinate(validInputCoordinate, epsgNumberForSweref99TM);
        Assert.AreEqual(c1, c2);
        Assert.AreEqual(c1, c3);
        Assert.AreEqual(c1, c4);
    }
    
} // the test class ends here
} // namespace ends here