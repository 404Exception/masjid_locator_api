using MasjidLocatorAPI.Services.Interface;
using NetTopologySuite.Geometries;

namespace MasjidLocatorAPI.Services.Implementation
{
    public class GeometryService: IGeometryService
    {
        private readonly GeometryFactory _geometryFactory;

        public GeometryService()
        {
            _geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        }
        public Point CreatePoint(double longitude, double latitude)
        {
            return _geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
        }

        public (double Longitude, double Latitude) GetCoordinates(Point point)
        {
            return (point.X, point.Y);
        }
    }
}
