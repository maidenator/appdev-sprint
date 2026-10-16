namespace appdev_sprint.Models;

public class Clinic
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Area { get; set; } = "";      // e.g. "Banilad"
    public string City { get; set; } = "";      // e.g. "Cebu City"
    public double Latitude { get; set; }        // demo coordinates — approximate
    public double Longitude { get; set; }       // demo coordinates — approximate
    public string Accreditations { get; set; } = "";  // e.g. "ISO,PhilHealth"
    public string Languages { get; set; } = "English,Filipino";
    public string Contact { get; set; } = "";
    public string ImplantSystems { get; set; } = "";  // e.g. "Osstem,Straumann"
    public bool IsHospitalDepartment { get; set; }
    public bool Published { get; set; } = true;

    public List<Procedure> Procedures { get; set; } = new();
}
