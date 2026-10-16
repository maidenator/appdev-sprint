using appdev_sprint.Data;
using appdev_sprint.Models;
using Microsoft.EntityFrameworkCore;

namespace appdev_sprint.Services;

// Shared contract (TASKS.md #3): inquiry submit signature consumed by Request.razor.
// SubmitAsync(...) shape is frozen — extend via optional params only, as its own task.
public class InquiryService(AppDbContext db)
{
    public async Task<Inquiry> SubmitAsync(
        int clinicId, int procedureId,
        string patientName, string contact, string homeCountry,
        DateTime? preferredDate, int companionCount, string notes)
    {
        var inquiry = new Inquiry
        {
            ClinicId = clinicId,
            ProcedureId = procedureId,
            PatientName = patientName,
            Contact = contact,
            HomeCountry = homeCountry,
            PreferredDate = preferredDate,
            CompanionCount = companionCount,
            Notes = notes,
            Status = InquiryStatus.New,
            CreatedAt = DateTime.UtcNow,
        };
        db.Inquiries.Add(inquiry);
        await db.SaveChangesAsync();
        return inquiry;
    }

    public async Task<List<Inquiry>> GetInboxAsync(InquiryStatus? status = null)
    {
        var q = db.Inquiries.Include(i => i.Clinic).Include(i => i.Procedure)
            .OrderByDescending(i => i.CreatedAt).AsQueryable();
        if (status is not null) q = q.Where(i => i.Status == status);
        return await q.ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(int id, InquiryStatus status)
    {
        var inquiry = await db.Inquiries.FindAsync(id);
        if (inquiry is null) return false;
        inquiry.Status = status;
        await db.SaveChangesAsync();
        return true;
    }
}
