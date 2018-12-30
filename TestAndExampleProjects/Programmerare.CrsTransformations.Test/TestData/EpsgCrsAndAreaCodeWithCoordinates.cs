namespace Programmerare.CrsTransformations.TestData
{

public class EpsgCrsAndAreaCodeWithCoordinates {
    public int epsgCrsCode;
    public int epsgAreaCode;
    public string epsgAreaName;
    public double centroidX;
    public double centroidY;

    public EpsgCrsAndAreaCodeWithCoordinates(
        int epsgCrsCode,
        int epsgAreaCode,
        string epsgAreaName,
        double centroidX,
        double centroidY
    ) {
        this.epsgCrsCode = epsgCrsCode;
        this.epsgAreaCode = epsgAreaCode;
        this.epsgAreaName = epsgAreaName;
        this.centroidX = centroidX;
        this.centroidY = centroidY;
    }

    public override string ToString() {
        return
            "EpsgCrsAndAreaCodeWithCoordinates{" +
            " epsgAreaCode=" + epsgAreaCode +
            ", epsgCrsCode=" + epsgCrsCode +
            ", epsgAreaName='" + epsgAreaName + '\'' +
            ", centroidX=" + centroidX +
            ", centroidY=" + centroidY +
            '}';
    }
}
}