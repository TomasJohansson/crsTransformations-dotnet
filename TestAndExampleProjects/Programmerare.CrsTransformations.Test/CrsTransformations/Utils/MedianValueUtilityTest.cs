using NUnit.Framework;
using Programmerare.CrsTransformations.Utils;
using System.Collections.Generic;

namespace Programmerare.CrsTransformations.Core.Utils {

[TestFixture]
public class MedianValueUtilityTest {

    private const double delta = 0.00001;

    [Test]
    public void GetMedianFrom3values() {
        double medianValue = MedianValueUtility.GetMedianValue(
            new List<double>{55.0, 33.0, 77.0 }
        );
        Assert.AreEqual(55.0, medianValue, delta);
    }

    [Test]
    public void GetMedianFrom4values() {
        double medianValue = MedianValueUtility.GetMedianValue(
            new List<double>{55.0, 33.0, 77.0, 35.0}
        );
        // the average of the two middle values 35 and 55
        Assert.AreEqual(45.0, medianValue, delta);
    }

    [Test]
    public void GetMedianFrom7values() {
        double medianValue = MedianValueUtility.GetMedianValue(
            new List<double>{9.0, 6.0, 1.0, 7.0, 8.0, 5.0, 3.0}
        );
        Assert.AreEqual(6.0, medianValue, delta);
    }

    [Test]
    public void GetMedianFrom8values() {
        double medianValue = MedianValueUtility.GetMedianValue(
            new List<double>{9.0, 6.0, 1.0, 7.0, 8.0, 5.0, 3.0, 6.5}
        );
        // the average of the two middle values 6.0 and 6.5
        Assert.AreEqual(6.25, medianValue, delta);
    }
}
}