namespace appdev_sprint.Models;

public class Procedure
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public Clinic? Clinic { get; set; }

    public ProcedureType Type { get; set; }
    public decimal PricePhp { get; set; }       // per tooth / per arch for AllOn4
    public int ChairDays { get; set; }           // days in the chair
    public int RecoveryNights { get; set; }
    public bool TwoTrips { get; set; }           // true for implants (osseointegration gap)
    public string Inclusions { get; set; } = ""; // what's covered, comma-separated
    public bool ComplicationExtension { get; set; } // free extended stay on complications
}
