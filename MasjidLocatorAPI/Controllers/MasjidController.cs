using MasjidLocatorAPI.Constants;
using MasjidLocatorAPI.Data;
using MasjidLocatorAPI.Model.Dto;
using MasjidLocatorAPI.Model.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace MasjidLocatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MasjidController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MasjidController(AppDbContext context)
        {
            _context = context;

        }

        // GET: api/masjid/nearby?lat=40.7128&lng=-74.0060&radius=10
        [HttpGet("nearby")]
        public async Task<ActionResult<IEnumerable<SubmittionDto>>> GetNearbyMasjid(
            [FromQuery] double lat,
            [FromQuery] double lng,
            [FromQuery] double radius = 10)
        {
            try
            {
                var userLocation = new Point(lng, lat) { SRID = 4326 };

                var masjids = await _context.MasjidSubmission
                    .Where(o => o.Status == MasjidStatus.Approved.ToString() &&
                        o.Location.Distance(userLocation) <= radius * 1000) // Convert km to meters
                    .OrderBy(o => o.Location.Distance(userLocation)).ToListAsync();

                if (masjids == null) return NotFound();

                var result =  masjids.Select(o => new SubmittionDto
                    {
                        Name = o.Name,
                        Address = o.Address,
                        City = o.City,
                        Latitude = o.Location.Y,
                        Longitude = o.Location.X,
                        DriveStartTime = o.DriveStartTime,
                        DriveEndTime = o.DriveEndTime,
                        Distance = Math.Round(o.Location.Distance(userLocation) / 1000, 2)
                })
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/masjid/city?city=New York
        [HttpGet("city")]
        public async Task<ActionResult<IEnumerable<SubmittionDto>>> GetMasjidByCity([FromQuery] string city)
        {
            var masjids = await _context.MasjidSubmission
                .Where(o => o.Status == MasjidStatus.Approved.ToString() &&
                            o.City.ToLower() == city.ToLower())
                .ToListAsync();

            if (masjids == null) return NotFound();

            var result = masjids.Select(o => new SubmittionDto
            {
                Name = o.Name,
                Address = o.Address,
                City = o.City,
                Latitude = o.Location.Y,
                Longitude = o.Location.X,
                DriveStartTime = o.DriveStartTime,
                DriveEndTime = o.DriveEndTime
            }).ToList();

            return Ok(result);
        }

        // GET: api/masjid/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SubmittionDto>> GetMasjid(Guid id)
        {
            var masjid = await _context.MasjidSubmission.FindAsync(id);
            
            if (masjid == null) return NotFound();

            var result = new SubmittionDto
            {
                Name = masjid.Name,
                Address = masjid.Address,
                City = masjid.City,
                Latitude = masjid.Location.Y,
                Longitude = masjid.Location.X,
                DriveStartTime = masjid.DriveStartTime,
                DriveEndTime = masjid.DriveEndTime
            };

            return Ok(result);
        }
    }
}
