// TODO(contract #1/#3): Temporary UI stubs pending backend agent's merge of Models/ and Services/.
// Once backend branch agent/opencode-db lands, the frontend can bind directly to backend services.
// Shared contract shapes strictly conform to TASKS.md specifications.

namespace appdev_sprint.Components.Shared.Stubs;

public enum ProcedureType
{
    Implant,
    AllOn4,
    Veneer,
    Crown,
    Cleaning,
    Whitening
}

public enum InquiryStatus
{
    New,
    Quoted,
    Accepted,
    Declined
}

// Flat pin for Leaflet map interop
public record ClinicPin(int Id, string Name, string Area, string City, double Latitude, double Longitude);

// Filter input from map/list pages
public record ClinicFilter(ProcedureType? ProcedureType, decimal? MaxPricePhp, string? Accreditation);

public class ProcedureItem
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public ProcedureType Type { get; set; }
    public decimal PricePhp { get; set; }
    public int ChairDays { get; set; }
    public int RecoveryNights { get; set; }
    public bool TwoTrips { get; set; }
    public string Inclusions { get; set; } = "";
    public bool ComplicationExtension { get; set; }
}

public record ClinicDetail(
    int Id,
    string Name,
    string Area,
    string City,
    double Latitude,
    double Longitude,
    string Accreditations,
    string Languages,
    string Contact,
    string ImplantSystems,
    bool IsHospitalDepartment,
    List<ProcedureItem> Procedures);

public record PackageQuote(
    int ClinicId,
    string ClinicName,
    string ClinicArea,
    ProcedureType ProcedureType,
    decimal TreatmentPhp,
    int StayNights,
    decimal HotelPhp,
    decimal TransfersPhp,
    decimal CompanionPhp,
    decimal TotalPhp,
    decimal TotalUsd,
    string HomeCountry,
    decimal HomePriceUsd,
    decimal SavingsUsd,
    decimal SavingsPct);

public class InquiryItem
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public string ClinicName { get; set; } = "";
    public int ProcedureId { get; set; }
    public string ProcedureName { get; set; } = "";
    public string PatientName { get; set; } = "";
    public string Contact { get; set; } = "";
    public string HomeCountry { get; set; } = "";
    public DateTime? PreferredDate { get; set; }
    public int CompanionCount { get; set; }
    public string Notes { get; set; } = "";
    public InquiryStatus Status { get; set; } = InquiryStatus.New;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class HomePriceAnchorItem
{
    public ProcedureType ProcedureType { get; set; }
    public string Country { get; set; } = "";
    public decimal PriceAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Note { get; set; } = "";
}

public class StubCatalogService
{
    public static readonly StubCatalogService Instance = new();

    public const decimal HotelPerNightPhp = 3500m;
    public const decimal TransfersPhp = 2500m;
    public const decimal CompanionPerDayPhp = 2000m;
    public const decimal PhpPerUsd = 62m; // July 2026 reference rate

    private readonly List<ClinicDetail> _clinics;
    private readonly List<HomePriceAnchorItem> _anchors;

    public StubCatalogService()
    {
        _clinics = new List<ClinicDetail>
        {
            new(
                1,
                "Banilad Smile Studio",
                "Banilad",
                "Cebu City",
                10.3415,
                123.9110,
                "ISO",
                "English,Filipino,Korean",
                "info@baniladsmile.example",
                "Osstem,Straumann",
                false,
                new List<ProcedureItem>
                {
                    new() { Id = 101, ClinicId = 1, Type = ProcedureType.Veneer, PricePhp = 22000m, ChairDays = 3, RecoveryNights = 5, TwoTrips = false, Inclusions = "Consult, digital smile design, porcelain veneer, polish, aftercare kit", ComplicationExtension = true },
                    new() { Id = 102, ClinicId = 1, Type = ProcedureType.Crown, PricePhp = 18000m, ChairDays = 3, RecoveryNights = 5, TwoTrips = false, Inclusions = "Consult, porcelain crown, temporary crown, aftercare kit", ComplicationExtension = true },
                    new() { Id = 103, ClinicId = 1, Type = ProcedureType.Implant, PricePhp = 95000m, ChairDays = 2, RecoveryNights = 7, TwoTrips = true, Inclusions = "Consult, 3D scan, Osstem fixture, healing abutment, final crown (trip 2)", ComplicationExtension = true }
                }
            ),
            new(
                2,
                "IT Park Dental Hub",
                "Lahug (IT Park)",
                "Cebu City",
                10.3290,
                123.9055,
                "ISO",
                "English,Filipino",
                "hello@itparkdental.example",
                "Osstem,Megagen",
                false,
                new List<ProcedureItem>
                {
                    new() { Id = 201, ClinicId = 2, Type = ProcedureType.Implant, PricePhp = 80000m, ChairDays = 2, RecoveryNights = 7, TwoTrips = true, Inclusions = "Consult, 3D scan, Osstem fixture, healing abutment, final crown (trip 2)", ComplicationExtension = true },
                    new() { Id = 202, ClinicId = 2, Type = ProcedureType.Cleaning, PricePhp = 1800m, ChairDays = 1, RecoveryNights = 1, TwoTrips = false, Inclusions = "Ultrasonic scaling, polish, fluoride treatment", ComplicationExtension = false },
                    new() { Id = 203, ClinicId = 2, Type = ProcedureType.Whitening, PricePhp = 12000m, ChairDays = 1, RecoveryNights = 1, TwoTrips = false, Inclusions = "Consult, laser whitening, take-home maintenance kit", ComplicationExtension = false }
                }
            ),
            new(
                3,
                "Mandaue Implant & Rehab Center",
                "Centro",
                "Mandaue City",
                10.3240,
                123.9215,
                "ISO,JCI-affiliated",
                "English,Filipino",
                "care@mandaueimplant.example",
                "Straumann,Nobel Biocare",
                false,
                new List<ProcedureItem>
                {
                    new() { Id = 301, ClinicId = 3, Type = ProcedureType.Implant, PricePhp = 150000m, ChairDays = 2, RecoveryNights = 7, TwoTrips = true, Inclusions = "Consult, 3D scan, Straumann fixture, healing abutment, final crown (trip 2), manufacturer warranty certificate", ComplicationExtension = true },
                    new() { Id = 302, ClinicId = 3, Type = ProcedureType.AllOn4, PricePhp = 650000m, ChairDays = 5, RecoveryNights = 10, TwoTrips = true, Inclusions = "Consult, 3D surgical guide, 4 Nobel Biocare fixtures, immediate load temp bridge, final zirconia bridge (trip 2)", ComplicationExtension = true },
                    new() { Id = 303, ClinicId = 3, Type = ProcedureType.Crown, PricePhp = 22000m, ChairDays = 3, RecoveryNights = 5, TwoTrips = false, Inclusions = "Consult, high-translucency zirconia crown, temporary restoration, aftercare kit", ComplicationExtension = true }
                }
            ),
            new(
                4,
                "Mabolo Family Dental",
                "Mabolo",
                "Cebu City",
                10.3165,
                123.9090,
                "PhilHealth",
                "English,Filipino",
                "smile@mabolofamily.example",
                "Osstem",
                false,
                new List<ProcedureItem>
                {
                    new() { Id = 401, ClinicId = 4, Type = ProcedureType.Implant, PricePhp = 70000m, ChairDays = 2, RecoveryNights = 7, TwoTrips = true, Inclusions = "Consult, digital X-ray, Osstem fixture, healing abutment, final crown (trip 2)", ComplicationExtension = false },
                    new() { Id = 402, ClinicId = 4, Type = ProcedureType.Veneer, PricePhp = 16000m, ChairDays = 2, RecoveryNights = 4, TwoTrips = false, Inclusions = "Consult, porcelain veneer, polish", ComplicationExtension = false },
                    new() { Id = 403, ClinicId = 4, Type = ProcedureType.Cleaning, PricePhp = 900m, ChairDays = 1, RecoveryNights = 0, TwoTrips = false, Inclusions = "Scaling, polish", ComplicationExtension = false }
                }
            ),
            new(
                5,
                "Fuente University Hospital – Dental Dept.",
                "Fuente Osmeña",
                "Cebu City",
                10.3075,
                123.8970,
                "PhilHealth,DOH-licensed",
                "English,Filipino",
                "dental@fuentehospital.example",
                "Osstem,Straumann",
                true,
                new List<ProcedureItem>
                {
                    new() { Id = 501, ClinicId = 5, Type = ProcedureType.Implant, PricePhp = 110000m, ChairDays = 2, RecoveryNights = 7, TwoTrips = true, Inclusions = "Hospital admission check, 3D CT scan, Osstem fixture, hospital-grade sterile suite, final crown (trip 2)", ComplicationExtension = true },
                    new() { Id = 502, ClinicId = 5, Type = ProcedureType.Crown, PricePhp = 16000m, ChairDays = 3, RecoveryNights = 5, TwoTrips = false, Inclusions = "Consult, porcelain crown, temporary crown", ComplicationExtension = true },
                    new() { Id = 503, ClinicId = 5, Type = ProcedureType.AllOn4, PricePhp = 550000m, ChairDays = 5, RecoveryNights = 10, TwoTrips = true, Inclusions = "Hospital workup, 3D surgical guide, 4 fixtures, immediate temporary + permanent bridge (trip 2)", ComplicationExtension = true }
                }
            ),
            new(
                6,
                "Mactan Fly-In Dental",
                "Pajac",
                "Lapu-Lapu City",
                10.3095,
                123.9785,
                "ISO",
                "English,Filipino,Japanese",
                "flyin@mactandental.example",
                "Megagen,Osstem",
                false,
                new List<ProcedureItem>
                {
                    new() { Id = 601, ClinicId = 6, Type = ProcedureType.Implant, PricePhp = 85000m, ChairDays = 2, RecoveryNights = 7, TwoTrips = true, Inclusions = "VIP airport pickup, consult, 3D scan, Megagen fixture, final crown (trip 2), airport return transfer", ComplicationExtension = true },
                    new() { Id = 602, ClinicId = 6, Type = ProcedureType.Veneer, PricePhp = 18000m, ChairDays = 2, RecoveryNights = 4, TwoTrips = false, Inclusions = "VIP airport pickup, consult, porcelain veneer, hotel transfer", ComplicationExtension = false },
                    new() { Id = 603, ClinicId = 6, Type = ProcedureType.Whitening, PricePhp = 8000m, ChairDays = 1, RecoveryNights = 1, TwoTrips = false, Inclusions = "Airport pickup, in-office laser whitening session", ComplicationExtension = false }
                }
            )
        };

        _anchors = new List<HomePriceAnchorItem>
        {
            new() { ProcedureType = ProcedureType.Implant, Country = "United States", PriceAmount = 4500m, Currency = "USD", Note = "single implant + crown, 2026 US guides" },
            new() { ProcedureType = ProcedureType.Implant, Country = "Australia", PriceAmount = 5500m, Currency = "AUD", Note = "single implant + crown, 2026 AU guides" },
            new() { ProcedureType = ProcedureType.Implant, Country = "United Kingdom", PriceAmount = 2800m, Currency = "GBP", Note = "single implant + crown, 2026 UK guides" },
            new() { ProcedureType = ProcedureType.AllOn4, Country = "United States", PriceAmount = 27000m, Currency = "USD", Note = "per arch, 2026 US guides" },
            new() { ProcedureType = ProcedureType.AllOn4, Country = "Australia", PriceAmount = 31000m, Currency = "AUD", Note = "per arch, 2026 AU guides" },
            new() { ProcedureType = ProcedureType.AllOn4, Country = "United Kingdom", PriceAmount = 12000m, Currency = "GBP", Note = "per arch, 2026 UK guides" },
            new() { ProcedureType = ProcedureType.Veneer, Country = "United States", PriceAmount = 1765m, Currency = "USD", Note = "per tooth porcelain, 2026 US guides" },
            new() { ProcedureType = ProcedureType.Veneer, Country = "Australia", PriceAmount = 2000m, Currency = "AUD", Note = "per tooth porcelain, 2026 AU guides" },
            new() { ProcedureType = ProcedureType.Crown, Country = "United States", PriceAmount = 1500m, Currency = "USD", Note = "porcelain crown, 2026 US guides" },
            new() { ProcedureType = ProcedureType.Crown, Country = "Australia", PriceAmount = 2100m, Currency = "AUD", Note = "porcelain crown, 2026 AU guides" }
        };
    }

    public Task<List<ClinicPin>> GetPinsAsync(ClinicFilter? filter = null)
    {
        var filtered = FilterClinics(filter);
        var pins = filtered.Select(c => new ClinicPin(c.Id, c.Name, c.Area, c.City, c.Latitude, c.Longitude)).ToList();
        return Task.FromResult(pins);
    }

    public Task<List<ClinicDetail>> GetClinicsAsync(ClinicFilter? filter = null)
    {
        var filtered = FilterClinics(filter);
        return Task.FromResult(filtered);
    }

    public Task<ClinicDetail?> GetDetailAsync(int id)
    {
        var clinic = _clinics.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(clinic);
    }

    public Task<List<PackageQuote>> CompareAsync(ProcedureType type, string homeCountry, int companionCount, int[] clinicIds)
    {
        var anchor = _anchors.FirstOrDefault(h => h.ProcedureType == type && h.Country.Equals(homeCountry, StringComparison.OrdinalIgnoreCase));
        if (anchor is null) return Task.FromResult(new List<PackageQuote>());

        var homeUsd = ToUsd(anchor.PriceAmount, anchor.Currency);

        var quotes = new List<PackageQuote>();
        foreach (var clinic in _clinics.Where(c => clinicIds.Contains(c.Id)))
        {
            var proc = clinic.Procedures.FirstOrDefault(p => p.Type == type);
            if (proc is null) continue;

            var nights = proc.RecoveryNights + 4; // recovery + buffer/travel days
            var hotel = nights * HotelPerNightPhp;
            var companion = companionCount * nights * CompanionPerDayPhp;
            var totalPhp = proc.PricePhp + hotel + TransfersPhp + companion;
            var totalUsd = totalPhp / PhpPerUsd;
            var savings = homeUsd - totalUsd;
            var pct = homeUsd == 0 ? 0 : Math.Round(savings / homeUsd * 100, 1);

            quotes.Add(new PackageQuote(
                clinic.Id,
                clinic.Name,
                clinic.Area,
                proc.Type,
                proc.PricePhp,
                nights,
                hotel,
                TransfersPhp,
                companion,
                totalPhp,
                Math.Round(totalUsd, 2),
                homeCountry,
                homeUsd,
                Math.Round(savings, 2),
                pct
            ));
        }

        return Task.FromResult(quotes);
    }

    public Task<List<string>> GetHomeCountriesAsync(ProcedureType type)
    {
        var countries = _anchors.Where(h => h.ProcedureType == type)
            .Select(h => h.Country)
            .Distinct()
            .ToList();
        return Task.FromResult(countries);
    }

    private List<ClinicDetail> FilterClinics(ClinicFilter? filter)
    {
        var query = _clinics.AsEnumerable();

        if (filter?.ProcedureType is not null)
        {
            query = query.Where(c => c.Procedures.Any(p => p.Type == filter.ProcedureType));
        }

        if (filter?.MaxPricePhp is not null)
        {
            query = query.Where(c => c.Procedures.Any(p =>
                (filter.ProcedureType == null || p.Type == filter.ProcedureType) &&
                p.PricePhp <= filter.MaxPricePhp.Value));
        }

        if (!string.IsNullOrWhiteSpace(filter?.Accreditation))
        {
            query = query.Where(c => c.Accreditations.Contains(filter.Accreditation, StringComparison.OrdinalIgnoreCase));
        }

        return query.OrderBy(c => c.City).ThenBy(c => c.Name).ToList();
    }

    private static decimal ToUsd(decimal amount, string currency) => currency switch
    {
        "USD" => amount,
        "AUD" => amount * 0.66m,
        "GBP" => amount * 1.27m,
        _ => amount
    };
}

public class StubInquiryService
{
    public static readonly StubInquiryService Instance = new();

    private readonly List<InquiryItem> _inquiries = new()
    {
        new InquiryItem
        {
            Id = 1,
            ClinicId = 3,
            ClinicName = "Mandaue Implant & Rehab Center",
            ProcedureId = 302,
            ProcedureName = "All-on-4 Full Arch Zirconia",
            PatientName = "David Henderson",
            Contact = "david.h@example.com.au",
            HomeCountry = "Australia",
            PreferredDate = DateTime.UtcNow.AddDays(25),
            CompanionCount = 1,
            Notes = "Looking for upper arch rehabilitation. Have recent OPG X-rays available. Traveling with wife.",
            Status = InquiryStatus.Quoted,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        },
        new InquiryItem
        {
            Id = 2,
            ClinicId = 1,
            ClinicName = "Banilad Smile Studio",
            ProcedureId = 101,
            ProcedureName = "8 Porcelain Veneers Package",
            PatientName = "Sarah Jenkins",
            Contact = "+1 415-555-0182",
            HomeCountry = "United States",
            PreferredDate = DateTime.UtcNow.AddDays(40),
            CompanionCount = 0,
            Notes = "Interested in digital smile design. Wanting natural shade BL3. Planning 10 days in Cebu.",
            Status = InquiryStatus.New,
            CreatedAt = DateTime.UtcNow.AddHours(-14)
        },
        new InquiryItem
        {
            Id = 3,
            ClinicId = 5,
            ClinicName = "Fuente University Hospital – Dental Dept.",
            ProcedureId = 501,
            ProcedureName = "Single Molar Implant (Osstem)",
            PatientName = "Edward Thornton",
            Contact = "ed.thornton@example.co.uk",
            HomeCountry = "United Kingdom",
            PreferredDate = DateTime.UtcNow.AddDays(60),
            CompanionCount = 1,
            Notes = "Need lower right first molar replacement. Hospital setting preferred due to mild hypertension.",
            Status = InquiryStatus.Accepted,
            CreatedAt = DateTime.UtcNow.AddDays(-4)
        }
    };

    public Task<InquiryItem> SubmitAsync(
        int clinicId,
        int procedureId,
        string patientName,
        string contact,
        string homeCountry,
        DateTime? preferredDate,
        int companionCount,
        string notes)
    {
        var clinic = StubCatalogService.Instance.GetDetailAsync(clinicId).Result;
        var clinicName = clinic?.Name ?? "Selected Clinic";
        var proc = clinic?.Procedures.FirstOrDefault(p => p.Id == procedureId);
        var procName = proc != null ? $"{proc.Type} (₱{proc.PricePhp:N0})" : "Dental Treatment";

        var newInquiry = new InquiryItem
        {
            Id = _inquiries.Count > 0 ? _inquiries.Max(i => i.Id) + 1 : 1,
            ClinicId = clinicId,
            ClinicName = clinicName,
            ProcedureId = procedureId,
            ProcedureName = procName,
            PatientName = patientName,
            Contact = contact,
            HomeCountry = homeCountry,
            PreferredDate = preferredDate,
            CompanionCount = companionCount,
            Notes = notes,
            Status = InquiryStatus.New,
            CreatedAt = DateTime.UtcNow
        };

        _inquiries.Insert(0, newInquiry);
        return Task.FromResult(newInquiry);
    }

    public Task<List<InquiryItem>> GetInboxAsync(InquiryStatus? status = null)
    {
        var q = _inquiries.AsEnumerable();
        if (status.HasValue)
        {
            q = q.Where(i => i.Status == status.Value);
        }
        return Task.FromResult(q.OrderByDescending(i => i.CreatedAt).ToList());
    }

    public Task<bool> UpdateStatusAsync(int id, InquiryStatus status)
    {
        var inquiry = _inquiries.FirstOrDefault(i => i.Id == id);
        if (inquiry == null) return Task.FromResult(false);

        inquiry.Status = status;
        return Task.FromResult(true);
    }
}
