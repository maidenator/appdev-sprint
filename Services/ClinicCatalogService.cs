using appdev_sprint.Data;
using appdev_sprint.Models;
using Microsoft.EntityFrameworkCore;

namespace appdev_sprint.Services;

// Shared contract (TASKS.md #1/#3): UI consumes these DTOs read-only.
// Do not change shapes without a dedicated contract task.

// Flat pin for the Leaflet map.
public record ClinicPin(int Id, string Name, string Area, string City, double Latitude, double Longitude);

// Filter input from the map/list pages.
public record ClinicFilter(ProcedureType? ProcedureType, decimal? MaxPricePhp, string? Accreditation);

// Full detail for /clinics/{id}.
public record ClinicDetail(
    int Id, string Name, string Area, string City,
    double Latitude, double Longitude,
    string Accreditations, string Languages, string Contact,
    string ImplantSystems, bool IsHospitalDepartment,
    List<Procedure> Procedures);

// Local price lookup for Cebu residents: treatment prices in PHP, no travel math.
public record ClinicPrice(int ClinicId, string ClinicName, string ClinicArea, Procedure Procedure);

// Compare-page result: Cebu package vs. home-country anchor (kept for reference).
public record PackageQuote(
    int ClinicId, string ClinicName, string ClinicArea,
    ProcedureType ProcedureType, decimal TreatmentPhp,
    int StayNights, decimal HotelPhp, decimal TransfersPhp, decimal CompanionPhp,
    decimal TotalPhp, decimal TotalUsd,
    string HomeCountry, decimal HomePriceUsd, decimal SavingsUsd, decimal SavingsPct);

public class ClinicCatalogService(AppDbContext db)
{
    // Demo trip-cost assumptions (not quotes): mid-range Cebu hotel, airport
    // transfers, companion daily spend. Shown as "estimates" in the UI.
    public const decimal HotelPerNightPhp = 3500m;
    public const decimal TransfersPhp = 2500m;
    public const decimal CompanionPerDayPhp = 2000m;
    public const decimal PhpPerUsd = 62m; // July 2026 reference rate

    public static readonly (double MinLat, double MaxLat, double MinLng, double MaxLng) CebuBoundingBox =
        (10.20, 10.45, 123.85, 124.05);

    public async Task<List<ClinicPin>> GetPinsAsync(ClinicFilter? filter = null)
    {
        var q = db.Clinics.Where(c => c.Published).AsQueryable();
        q = ApplyFilter(q, filter);
        return await q.Select(c => new ClinicPin(c.Id, c.Name, c.Area, c.City, c.Latitude, c.Longitude))
            .ToListAsync();
    }

    public async Task<List<Clinic>> GetClinicsAsync(ClinicFilter? filter = null)
    {
        var q = db.Clinics.Where(c => c.Published).Include(c => c.Procedures).AsQueryable();
        q = ApplyFilter(q, filter);
        return await q.OrderBy(c => c.City).ThenBy(c => c.Name).ToListAsync();
    }

    public async Task<ClinicDetail?> GetDetailAsync(int id)
    {
        var c = await db.Clinics.Include(x => x.Procedures)
            .FirstOrDefaultAsync(x => x.Id == id && x.Published);
        return c is null ? null : new ClinicDetail(
            c.Id, c.Name, c.Area, c.City, c.Latitude, c.Longitude,
            c.Accreditations, c.Languages, c.Contact,
            c.ImplantSystems, c.IsHospitalDepartment, c.Procedures);
    }

    public async Task<List<ClinicPrice>> GetPricesAsync(ProcedureType type, int[] clinicIds)
    {
        // Note: ORDER BY on decimal isn't translatable by SQLite — sort client-side.
        var rows = await db.Procedures.Include(p => p.Clinic)
            .Where(p => p.Type == type && clinicIds.Contains(p.ClinicId) && p.Clinic!.Published)
            .Select(p => new ClinicPrice(p.ClinicId, p.Clinic!.Name, p.Clinic.Area, p))
            .ToListAsync();
        return rows.OrderBy(p => p.Procedure.PricePhp).ToList();
    }

    public async Task<List<PackageQuote>> CompareAsync(ProcedureType type, string homeCountry, int companionCount, int[] clinicIds)
    {
        var anchor = await db.HomePriceAnchors
            .FirstOrDefaultAsync(h => h.ProcedureType == type && h.Country == homeCountry);
        if (anchor is null) return [];

        var homeUsd = ToUsd(anchor.PriceAmount, anchor.Currency);
        var procedures = await db.Procedures.Include(p => p.Clinic)
            .Where(p => p.Type == type && clinicIds.Contains(p.ClinicId) && p.Clinic!.Published)
            .ToListAsync();

        return procedures.Select(p =>
        {
            var nights = p.RecoveryNights + 4; // recovery + buffer/travel days
            var hotel = nights * HotelPerNightPhp;
            var companion = companionCount * nights * CompanionPerDayPhp;
            var totalPhp = p.PricePhp + hotel + TransfersPhp + companion;
            var totalUsd = totalPhp / PhpPerUsd;
            var savings = homeUsd - totalUsd;
            return new PackageQuote(
                p.ClinicId, p.Clinic!.Name, p.Clinic.Area, p.Type, p.PricePhp,
                nights, hotel, TransfersPhp, companion,
                totalPhp, Math.Round(totalUsd, 2),
                homeCountry, homeUsd, Math.Round(savings, 2),
                homeUsd == 0 ? 0 : Math.Round(savings / homeUsd * 100, 1));
        }).ToList();
    }

    public async Task<List<string>> GetHomeCountriesAsync(ProcedureType type) =>
        await db.HomePriceAnchors.Where(h => h.ProcedureType == type)
            .Select(h => h.Country).Distinct().ToListAsync();

    private static IQueryable<Clinic> ApplyFilter(IQueryable<Clinic> q, ClinicFilter? f)
    {
        if (f?.ProcedureType is not null)
            q = q.Where(c => c.Procedures.Any(p => p.Type == f.ProcedureType));
        if (f?.MaxPricePhp is not null)
            q = q.Where(c => c.Procedures.Any(p => p.Type == (f.ProcedureType ?? p.Type) && p.PricePhp <= f.MaxPricePhp));
        if (!string.IsNullOrWhiteSpace(f?.Accreditation))
            q = q.Where(c => c.Accreditations.Contains(f.Accreditation!));
        return q;
    }

    private static decimal ToUsd(decimal amount, string currency) => currency switch
    {
        "USD" => amount,
        "AUD" => amount * 0.66m, // demo rate
        "GBP" => amount * 1.27m, // demo rate
        _ => amount,
    };
}
