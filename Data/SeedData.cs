using appdev_sprint.Models;

namespace appdev_sprint.Data;

// All values are DEMO seed data grounded in 2026 Cebu price guides —
// approximate coordinates, ranges, and clinic archetypes. Not real quotes.
public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.Clinics.Any()) return;

        var clinics = new List<Clinic>
        {
            new() {
                Name = "Banilad Smile Studio", Area = "Banilad", City = "Cebu City",
                Latitude = 10.3415, Longitude = 123.9110,
                Accreditations = "ISO", Languages = "English,Filipino,Korean",
                Contact = "info@baniladsmile.example",
                ImplantSystems = "Osstem,Straumann",
            },
            new() {
                Name = "IT Park Dental Hub", Area = "Lahug (IT Park)", City = "Cebu City",
                Latitude = 10.3290, Longitude = 123.9055,
                Accreditations = "ISO", Languages = "English,Filipino",
                Contact = "hello@itparkdental.example",
                ImplantSystems = "Osstem,Megagen",
            },
            new() {
                Name = "Mandaue Implant & Rehab Center", Area = "Centro", City = "Mandaue City",
                Latitude = 10.3240, Longitude = 123.9215,
                Accreditations = "ISO,JCI-affiliated", Languages = "English,Filipino",
                Contact = "care@mandaueimplant.example",
                ImplantSystems = "Straumann,Nobel Biocare",
            },
            new() {
                Name = "Mabolo Family Dental", Area = "Mabolo", City = "Cebu City",
                Latitude = 10.3165, Longitude = 123.9090,
                Accreditations = "PhilHealth", Languages = "English,Filipino",
                Contact = "smile@mabolofamily.example",
                ImplantSystems = "Osstem",
            },
            new() {
                Name = "Fuente University Hospital — Dental Dept.", Area = "Fuente Osmeña", City = "Cebu City",
                Latitude = 10.3075, Longitude = 123.8970,
                Accreditations = "PhilHealth,DOH-licensed", Languages = "English,Filipino",
                Contact = "dental@fuentehospital.example",
                ImplantSystems = "Osstem,Straumann",
                IsHospitalDepartment = true,
            },
            new() {
                Name = "Mactan Fly-In Dental", Area = "Pajac", City = "Lapu-Lapu City",
                Latitude = 10.3095, Longitude = 123.9785,
                Accreditations = "ISO", Languages = "English,Filipino,Japanese",
                Contact = "flyin@mactandental.example",
                ImplantSystems = "Megagen,Osstem",
            },
        };
        db.Clinics.AddRange(clinics);
        db.SaveChanges();

        var p = new List<Procedure>
        {
            // Banilad Smile Studio — cosmetic focus
            Proc(clinics[0], ProcedureType.Veneer, 22000, chairDays: 3, nights: 5, twoTrips: false,
                "consult,digital smile design,8 porcelain veneers,polish,aftercare kit", true),
            Proc(clinics[0], ProcedureType.Crown, 18000, chairDays: 3, nights: 5, twoTrips: false,
                "consult,porcelain crown,temporary crown,aftercare kit", true),
            Proc(clinics[0], ProcedureType.Implant, 95000, chairDays: 2, nights: 7, twoTrips: true,
                "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", true),

            // IT Park Dental Hub — implants + hygiene
            Proc(clinics[1], ProcedureType.Implant, 80000, chairDays: 2, nights: 7, twoTrips: true,
                "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", true),
            Proc(clinics[1], ProcedureType.Cleaning, 1800, chairDays: 1, nights: 1, twoTrips: false,
                "ultrasonic scaling,polish,fluoride", false),
            Proc(clinics[1], ProcedureType.Whitening, 12000, chairDays: 1, nights: 1, twoTrips: false,
                "consult,laser whitening,take-home kit", false),

            // Mandaue Implant & Rehab Center — premium / complex
            Proc(clinics[2], ProcedureType.Implant, 150000, chairDays: 2, nights: 7, twoTrips: true,
                "consult,3D scan,Straumann fixture,healing abutment,final crown (trip 2),warranty papers", true),
            Proc(clinics[2], ProcedureType.AllOn4, 650000, chairDays: 5, nights: 10, twoTrips: true,
                "consult,3D scan,4 Nobel Biocare fixtures,temporary bridge,final bridge (trip 2)", true),
            Proc(clinics[2], ProcedureType.Crown, 22000, chairDays: 3, nights: 5, twoTrips: false,
                "consult,zirconia crown,temporary crown,aftercare kit", true),

            // Mabolo Family Dental — value tier
            Proc(clinics[3], ProcedureType.Implant, 70000, chairDays: 2, nights: 7, twoTrips: true,
                "consult,X-ray,Osstem fixture,healing abutment,final crown (trip 2)", false),
            Proc(clinics[3], ProcedureType.Veneer, 16000, chairDays: 2, nights: 4, twoTrips: false,
                "consult,porcelain veneer,polish", false),
            Proc(clinics[3], ProcedureType.Cleaning, 900, chairDays: 1, nights: 0, twoTrips: false,
                "scaling,polish", false),

            // Fuente hospital dental dept — trust anchor
            Proc(clinics[4], ProcedureType.Implant, 110000, chairDays: 2, nights: 7, twoTrips: true,
                "consult,3D scan,Osstem fixture,hospital workup,final crown (trip 2)", true),
            Proc(clinics[4], ProcedureType.Crown, 16000, chairDays: 3, nights: 5, twoTrips: false,
                "consult,porcelain crown,temporary crown", true),
            Proc(clinics[4], ProcedureType.AllOn4, 550000, chairDays: 5, nights: 10, twoTrips: true,
                "consult,3D scan,hospital workup,4 fixtures,temporary + final bridge", true),

            // Mactan Fly-In Dental — airport convenience
            Proc(clinics[5], ProcedureType.Implant, 85000, chairDays: 2, nights: 7, twoTrips: true,
                "airport pickup,consult,3D scan,Megagen fixture,final crown (trip 2),airport transfer", true),
            Proc(clinics[5], ProcedureType.Veneer, 18000, chairDays: 2, nights: 4, twoTrips: false,
                "airport pickup,consult,porcelain veneer,hotel transfer", false),
            Proc(clinics[5], ProcedureType.Whitening, 8000, chairDays: 1, nights: 1, twoTrips: false,
                "airport pickup,laser whitening", false),
        };
        db.Procedures.AddRange(p);

        db.HomePriceAnchors.AddRange(
            Anchor(ProcedureType.Implant, "United States", 4500m, "USD", "single implant + crown, 2026 US guides"),
            Anchor(ProcedureType.Implant, "Australia", 5500m, "AUD", "single implant + crown, 2026 AU guides"),
            Anchor(ProcedureType.Implant, "United Kingdom", 2800m, "GBP", "single implant + crown, 2026 UK guides"),
            Anchor(ProcedureType.AllOn4, "United States", 27000m, "USD", "per arch, 2026 US guides"),
            Anchor(ProcedureType.AllOn4, "Australia", 31000m, "AUD", "per arch, 2026 AU guides"),
            Anchor(ProcedureType.AllOn4, "United Kingdom", 12000m, "GBP", "per arch, 2026 UK guides"),
            Anchor(ProcedureType.Veneer, "United States", 1765m, "USD", "per tooth porcelain, 2026 US guides"),
            Anchor(ProcedureType.Veneer, "Australia", 2000m, "AUD", "per tooth porcelain, 2026 AU guides"),
            Anchor(ProcedureType.Crown, "United States", 1500m, "USD", "porcelain, 2026 US guides"),
            Anchor(ProcedureType.Crown, "Australia", 2100m, "AUD", "porcelain, 2026 AU guides")
        );

        db.SaveChanges();

        static Procedure Proc(Clinic c, ProcedureType t, decimal php, int chairDays, int nights, bool twoTrips, string incl, bool ext) =>
            new() { ClinicId = c.Id, Type = t, PricePhp = php, ChairDays = chairDays,
                    RecoveryNights = nights, TwoTrips = twoTrips, Inclusions = incl, ComplicationExtension = ext };

        static HomePriceAnchor Anchor(ProcedureType t, string country, decimal amount, string ccy, string note) =>
            new() { ProcedureType = t, Country = country, PriceAmount = amount, Currency = ccy, Note = note };
    }
}
