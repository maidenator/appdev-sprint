namespace appdev_sprint.Models;

public enum InquiryStatus
{
    New,
    Quoted,
    Accepted,
    Declined
}

public class Inquiry
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public Clinic? Clinic { get; set; }
    public int ProcedureId { get; set; }
    public Procedure? Procedure { get; set; }

    public string PatientName { get; set; } = "";
    public string Contact { get; set; } = "";     // email or phone
    public string HomeCountry { get; set; } = ""; // US, Australia, UK
    public DateTime? PreferredDate { get; set; }
    public int CompanionCount { get; set; }
    public string Notes { get; set; } = "";

    public InquiryStatus Status { get; set; } = InquiryStatus.New;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
