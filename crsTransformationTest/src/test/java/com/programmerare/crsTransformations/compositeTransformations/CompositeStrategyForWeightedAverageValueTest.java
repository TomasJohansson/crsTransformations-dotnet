package com.programmerare.crsTransformations.compositeTransformations;

import com.programmerare.crsTransformationFacadeGeoTools.CrsTransformationFacadeGeoTools;
import com.programmerare.crsTransformationFacadeGooberCTL.CrsTransformationFacadeGooberCTL;
import com.programmerare.crsTransformationFacadeOrbisgisCTS.CrsTransformationFacadeOrbisgisCTS;
import com.programmerare.crsTransformationFacadeProj4J.CrsTransformationFacadeProj4J;
import com.programmerare.crsTransformations.CrsTransformationFacade;
import com.programmerare.crsTransformations.Coordinate;
import com.programmerare.crsTransformations.crsConstants.ConstantEpsgNumber;
import kotlin.Pair;
import org.junit.jupiter.api.Test;
import java.util.Arrays;
import java.util.List;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class CompositeStrategyForWeightedAverageValueTest extends CompositeStrategyTestBase {

    private double weightForGeoTools = 0.4;
    private double weightForGoober = 0.3;
    private double weightForOrbis = 0.2;
    private double weightForProj4J = 0.1;

    @Test
    void createCompositeStrategyForWeightedAverageValue() {
        List<Pair<CrsTransformationFacade, Double>> pairs = Arrays.asList(
            new Pair(new CrsTransformationFacadeGeoTools(), weightForGeoTools),
            new Pair(new CrsTransformationFacadeGooberCTL(), weightForGoober),
            new Pair(new CrsTransformationFacadeOrbisgisCTS(), weightForOrbis),
            new Pair(new CrsTransformationFacadeProj4J(), weightForProj4J)
        );
        CrsTransformationFacadeComposite facade = CrsTransformationFacadeComposite.createCrsTransformationWeightedAverage(pairs);
        createCompositeStrategyForWeightedAverageValueHelper(facade);
    }

    @Test
    void createCompositeStrategyForWeightedAverageValueFromStringsWithReflection() {
        String classNameGeoTools = CrsTransformationFacadeGeoTools.class.getName() ;
        String classNameGoober = CrsTransformationFacadeGooberCTL.class.getName() ;
        String classNameOrbis = CrsTransformationFacadeOrbisgisCTS.class.getName() ;
        String classNameProj4J = CrsTransformationFacadeProj4J.class.getName() ;

        List<Pair<String, Double>> pairs = Arrays.asList(
            new Pair(classNameGeoTools, weightForGeoTools),
            new Pair(classNameGoober, weightForGoober),
            new Pair(classNameOrbis, weightForOrbis),
            new Pair(classNameProj4J, weightForProj4J)
        );
        CrsTransformationFacadeComposite facade = CrsTransformationFacadeComposite.createCrsTransformationWeightedAverageByReflection(pairs);
        createCompositeStrategyForWeightedAverageValueHelper(facade);
    }


    private void createCompositeStrategyForWeightedAverageValueHelper(CrsTransformationFacadeComposite facade) {
        Coordinate coordinateWithExpectedWeightedValues = createWeightedValue();

        Coordinate coordinateResult = facade.transform(wgs84coordinate, ConstantEpsgNumber.SWEREF99TM);

        double delta = 0.0000000001;
        assertEquals(coordinateWithExpectedWeightedValues.getYLatitude(), coordinateResult.getYLatitude(), delta);
        assertEquals(coordinateWithExpectedWeightedValues.getXLongitude(), coordinateResult.getXLongitude(), delta);

        double diffLatTestedFacade = Math.abs(coordinateWithExpectedWeightedValues.getYLatitude() - coordinateResult.getYLatitude());
        double diffLonTestedFacade = Math.abs(coordinateWithExpectedWeightedValues.getXLongitude() - coordinateResult.getXLongitude());

        double diffLatGeoTools = Math.abs(coordinateWithExpectedWeightedValues.getYLatitude() - resultCoordinateGeoTools.getYLatitude());
        double diffLonGeoTools = Math.abs(coordinateWithExpectedWeightedValues.getXLongitude() - resultCoordinateGeoTools.getXLongitude());

        double diffLatGoober = Math.abs(coordinateWithExpectedWeightedValues.getYLatitude() - resultCoordinateGooberCTL.getYLatitude());
        double diffLonGoober = Math.abs(coordinateWithExpectedWeightedValues.getXLongitude() - resultCoordinateGooberCTL.getXLongitude());

        double diffLatOrbis = Math.abs(coordinateWithExpectedWeightedValues.getYLatitude() - resultCoordinateOrbisgisCTS.getYLatitude());
        double diffLonOrbis = Math.abs(coordinateWithExpectedWeightedValues.getXLongitude() - resultCoordinateOrbisgisCTS.getXLongitude());

        double diffLatProj4J = Math.abs(coordinateWithExpectedWeightedValues.getYLatitude() - resultCoordinateProj4J.getYLatitude());
        double diffLonProj4J = Math.abs(coordinateWithExpectedWeightedValues.getXLongitude() - resultCoordinateProj4J.getXLongitude());

        // TODO comments ... and maye use hamcrest below

        assertTrue(diffLatTestedFacade < delta);
        assertTrue(diffLatTestedFacade < delta);

        assertTrue(diffLatGeoTools > delta);
        assertTrue(diffLonGeoTools > delta);
        assertTrue(diffLatGoober > delta);
        assertTrue(diffLonGoober > delta);
        assertTrue(diffLatOrbis > delta);
        assertTrue(diffLonOrbis > delta);
        assertTrue(diffLatProj4J > delta);
        assertTrue(diffLonProj4J > delta);

        //assertNotEquals(coordinateWithExpectedWeightedValues.getXLongitude(), coordinateResult.getXLongitude(), delta);
    }

    private Coordinate createWeightedValue() {
        // resultCoordinateGeoTools, resultCoordinateGooberCTL, resultCoordinateOrbisgisCTS, resultCoordinateProj4J
        double latitudeWeightedSum =
                weightForGeoTools * resultCoordinateGeoTools.getYLatitude() +
                weightForGoober * resultCoordinateGooberCTL.getYLatitude() +
                weightForOrbis * resultCoordinateOrbisgisCTS.getYLatitude() +
                weightForProj4J* resultCoordinateProj4J.getYLatitude();

        double longitutdeWeightedSum =
                weightForGeoTools * resultCoordinateGeoTools.getXLongitude() +
                weightForGoober * resultCoordinateGooberCTL.getXLongitude() +
                weightForOrbis * resultCoordinateOrbisgisCTS.getXLongitude() +
                weightForProj4J* resultCoordinateProj4J.getXLongitude();

        double totWeights = weightForGeoTools + weightForGoober + weightForOrbis + weightForProj4J;
        return Coordinate.createFromYLatXLong( latitudeWeightedSum/totWeights, longitutdeWeightedSum/totWeights, ConstantEpsgNumber.SWEREF99TM);
    }
}
