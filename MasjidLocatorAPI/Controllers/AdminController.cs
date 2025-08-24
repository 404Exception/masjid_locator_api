using MasjidLocatorAPI.Constants;
using MasjidLocatorAPI.Data;
using MasjidLocatorAPI.Model.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasjidLocatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/submissions/pending
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<MasjidSubmissionEntity>>> GetPendingSubmissions()
        {
            var submissions = await _context.MasjidSubmission
                .Where(s => s.Status == MasjidStatus.Pending.ToString())
                .OrderBy(s => s.SubmittedAt)
                .ToListAsync();

            return Ok(submissions);
        }

        // POST: api/admin/submissions/approve/{id}
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveSubmission(Guid id, [FromBody] string? adminNotes = null)
        {
            var submission = await _context.MasjidSubmission.FindAsync(id);
            if (submission == null) return NotFound();


            submission.Status = MasjidStatus.Approved.ToString();
            submission.AdminNotes = adminNotes;

            _context.MasjidSubmission.Update(submission);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Submission approved" });
        }

        // POST: api/admin/submissions/reject/{id}
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> RejectSubmission(Guid id, [FromBody] string rejectionReason)
        {
            var submission = await _context.MasjidSubmission.FindAsync(id);
            if (submission == null) return NotFound();

            submission.Status = MasjidStatus.Reject.ToString();
            submission.AdminNotes = rejectionReason;

            _context.MasjidSubmission.Update(submission);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Submission rejected" });
        }
    }
}
