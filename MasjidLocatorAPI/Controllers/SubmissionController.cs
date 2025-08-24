using MasjidLocatorAPI.Data;
using MasjidLocatorAPI.Model.Dto;
using MasjidLocatorAPI.Model.Entity;
using MasjidLocatorAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasjidLocatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubmissionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IGeometryService _geometryService;
        public SubmissionController(AppDbContext context, IGeometryService geometryService)
        {
            _context = context;
            _geometryService = geometryService;
        }

        // POST: api/submissions
        [HttpPost]
        public async Task<ActionResult<SubmittionDto>> Submit(
            [FromBody] SubmittionDto dto)
        {
            if (dto == null) return NotFound();
            var location = _geometryService.CreatePoint(dto.Longitude, dto.Latitude);

            var submission = new MasjidSubmissionEntity
            {
                SubmittedBy = dto.SubmittedBy,
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City,
                Location = location,
                DriveStartTime = dto.DriveStartTime,
                DriveEndTime = dto.DriveEndTime
            };

            _context.MasjidSubmission.Add(submission);
            await _context.SaveChangesAsync();

            return Ok(new SubmissionResponseDto
            {
                Id = submission.Id,
                Message = "Submission received and pending admin approval"
            });
        }

        // GET: api/submissions/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<MasjidSubmissionEntity>>> GetUserSubmissions(Guid userId)
        {

            // Get data without coordinates first
            var submissions = await _context.MasjidSubmission
                .Where(s => s.SubmittedBy == userId)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            if (submissions == null) return NotFound();

            // Process coordinates on client side (server)
            var result = submissions.Select(s => new SubmittionDto
            {
                Name = s.Name,
                Address = s.Address,
                City = s.City,
                Latitude = s.Location.Y, // This works after data is materialized
                Longitude = s.Location.X,
                DriveStartTime = s.DriveStartTime,
                DriveEndTime = s.DriveEndTime
            }).ToList();

            return Ok(result);
        }

        // GET: api/submissions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MasjidSubmissionEntity>> GetSubmission(Guid id)
        {
            var submission = await _context.MasjidSubmission.FindAsync(id);
            if (submission == null) return NotFound();

            // Process coordinates on client side (server)
            var result = new SubmittionDto
            {
                Name = submission.Name,
                Address = submission.Address,
                City = submission.City,
                Latitude = submission.Location.Y, // This works after data is materialized
                Longitude = submission.Location.X,
                DriveStartTime = submission.DriveStartTime,
                DriveEndTime = submission.DriveEndTime
            };

            return Ok(result);
        }
    }
}
