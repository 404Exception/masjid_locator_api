using MasjidLocatorAPI.Constants;
using NetTopologySuite.Geometries;

namespace MasjidLocatorAPI.Model.Entity
{
    public class MasjidSubmissionEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SubmittedBy { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public Point Location { get; set; } = null!;
        public TimeSpan DriveStartTime { get; set; }
        public TimeSpan DriveEndTime { get; set; }
        public string Status { get; set; } = MasjidStatus.Pending.ToString(); // Pending, Approved, Rejected
        public string? AdminNotes { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
