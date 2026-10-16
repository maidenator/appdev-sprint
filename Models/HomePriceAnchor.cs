namespace appdev_sprint.Models;

// Home-country price anchors for the compare page.
// Demo estimates from 2026 dental-tourism cost guides — not quotes.
public class HomePriceAnchor
{
    public int Id { get; set; }
    public ProcedureType ProcedureType { get; set; }
    public string Country { get; set; } = "";   // "United States", "Australia", "United Kingdom"
    public decimal PriceAmount { get; set; }     // in Currency below
    public string Currency { get; set; } = "USD";
    public string Note { get; set; } = "";
}
