using appdev_sprint.Models;
using appdev_sprint.Services;

namespace appdev_sprint.Tests;

public class SeedTests
{
    [Fact]
    public void Seed_LoadsExpectedCebuDataset()
    {
        using var db = new TestDb();
        Assert.Equal(6, db.Context.Clinics.Count());
        Assert.Equal(18, db.Context.Procedures.Count());
        Assert.Equal(10, db.Context.HomePriceAnchors.Count());
        Assert.Empty(db.Context.Inquiries);
    }
}

public class CatalogTests
{
    [Fact]
    public async Task Pins_ReturnSixClinics_AllInsideCebuBoundingBox()
    {
        using var db = new TestDb();
        var svc = new ClinicCatalogService(db.Context);

        var pins = await svc.GetPinsAsync();
        var box = ClinicCatalogService.CebuBoundingBox;

        Assert.Equal(6, pins.Count);
        Assert.All(pins, p =>
        {
            Assert.InRange(p.Latitude, box.MinLat, box.MaxLat);
            Assert.InRange(p.Longitude, box.MinLng, box.MaxLng);
        });
    }

    [Fact]
    public async Task Filter_VeneerUnder20k_ReturnsOnlyValueTierClinics()
    {
        using var db = new TestDb();
        var svc = new ClinicCatalogService(db.Context);

        var pins = await svc.GetPinsAsync(new ClinicFilter(ProcedureType.Veneer, 20000m, null));

        Assert.Equal(2, pins.Count);
        Assert.Contains(pins, p => p.Area == "Mabolo");
        Assert.Contains(pins, p => p.Area == "Pajac");
    }

    [Fact]
    public async Task Compare_ImplantVsUS_AllQuotesShowPositiveSavings()
    {
        using var db = new TestDb();
        var svc = new ClinicCatalogService(db.Context);
        var ids = (await svc.GetPinsAsync()).Select(p => p.Id).ToArray();

        var quotes = await svc.CompareAsync(ProcedureType.Implant, "United States", 1, ids);

        Assert.Equal(6, quotes.Count);
        Assert.All(quotes, q =>
        {
            Assert.Equal("United States", q.HomeCountry);
            Assert.Equal(4500m, q.HomePriceUsd);
            Assert.True(q.SavingsUsd > 0, $"{q.ClinicArea} should save vs US");
            Assert.True(q.SavingsPct > 0 && q.SavingsPct < 100);
            Assert.Equal(q.TreatmentPhp + q.HotelPhp + q.TransfersPhp + q.CompanionPhp, q.TotalPhp);
        });
        // Value tier saves most, premium saves least.
        Assert.True(quotes.Max(q => q.SavingsPct) > quotes.Min(q => q.SavingsPct));
    }

    [Fact]
    public async Task Compare_UnknownHomeCountry_ReturnsEmpty()
    {
        using var db = new TestDb();
        var svc = new ClinicCatalogService(db.Context);
        var ids = (await svc.GetPinsAsync()).Select(p => p.Id).ToArray();

        Assert.Empty(await svc.CompareAsync(ProcedureType.Implant, "Korea", 1, ids));
    }

    [Fact]
    public async Task Detail_ReturnsClinicWithProcedures_MissingIdReturnsNull()
    {
        using var db = new TestDb();
        var svc = new ClinicCatalogService(db.Context);

        var detail = await svc.GetDetailAsync(1);

        Assert.NotNull(detail);
        Assert.False(string.IsNullOrWhiteSpace(detail.Name));
        Assert.NotEmpty(detail.Procedures);
        Assert.Null(await svc.GetDetailAsync(9999));
    }

    [Fact]
    public async Task HomeCountries_ForImplant_ListsUS_Australia_UK()
    {
        using var db = new TestDb();
        var svc = new ClinicCatalogService(db.Context);

        var countries = await svc.GetHomeCountriesAsync(ProcedureType.Implant);

        Assert.Contains("United States", countries);
        Assert.Contains("Australia", countries);
        Assert.Contains("United Kingdom", countries);
    }
}

public class InquiryTests
{
    [Fact]
    public async Task Submit_CreatesNewInquiry_StatusTransitionsToQuoted()
    {
        using var db = new TestDb();
        var svc = new InquiryService(db.Context);
        var proc = db.Context.Procedures.First(p => p.Type == ProcedureType.Implant);

        var inquiry = await svc.SubmitAsync(proc.ClinicId, proc.Id, "Test Patient",
            "test@example.com", "United States", null, 1, "test notes");

        Assert.True(inquiry.Id > 0);
        Assert.Equal(InquiryStatus.New, inquiry.Status);
        Assert.Single(await svc.GetInboxAsync());

        Assert.True(await svc.UpdateStatusAsync(inquiry.Id, InquiryStatus.Quoted));
        Assert.Single(await svc.GetInboxAsync(InquiryStatus.Quoted));
        Assert.Empty(await svc.GetInboxAsync(InquiryStatus.Accepted));
    }

    [Fact]
    public async Task UpdateStatus_UnknownId_ReturnsFalse()
    {
        using var db = new TestDb();
        var svc = new InquiryService(db.Context);

        Assert.False(await svc.UpdateStatusAsync(9999, InquiryStatus.Quoted));
    }
}
