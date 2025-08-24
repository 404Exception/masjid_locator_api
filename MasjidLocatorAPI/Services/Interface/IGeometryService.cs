using NetTopologySuite.Geometries;

namespace MasjidLocatorAPI.Services.Interface
{
    public interface IGeometryService
    {
        Point CreatePoint(double longitude, double latitude);
        (double Longitude, double Latitude) GetCoordinates(Point point);
    }
}
