using appdev_sprint.Models;

namespace appdev_sprint.Data;

// DEMO seed. Clinic NAMES + AREAS are real directory listings (researched Oct 2026).
// Prices, accreditations, languages, implant systems, and coordinates are ILLUSTRATIVE
// demo data — coordinates are area-level approximations, contacts are intentionally blank
// (use the hub inquiry form). Never present seed values as real quotes. Not real quotes.
public static class SeedData
{
    private record ProcDef(ProcedureType Type, decimal PricePhp, int ChairDays, int Nights, bool TwoTrips, string Inclusions, bool Ext);
    private record ClinicDef(string Name, string Area, string City, double Lat, double Lng,
        string Accred, string Lang, string Contact, string Implants, bool Hosp, ProcDef[] Procs);

    public static void Initialize(AppDbContext db)
    {
        var defs = Defs();
        var existing = db.Clinics.ToDictionary(c => c.Name.ToLowerInvariant(), c => c);

        foreach (var d in defs)
        {
            if (!existing.TryGetValue(d.Name.ToLowerInvariant(), out var clinic))
            {
                clinic = new Clinic
                {
                    Name = d.Name, Area = d.Area, City = d.City,
                    Latitude = d.Lat, Longitude = d.Lng,
                    Accreditations = d.Accred, Languages = d.Lang, Contact = d.Contact,
                    ImplantSystems = d.Implants, IsHospitalDepartment = d.Hosp,
                };
                db.Clinics.Add(clinic);
                db.SaveChanges();
                foreach (var p in d.Procs) db.Procedures.Add(ToProcedure(clinic.Id, p));
            }
            else
            {
                // Backfill: add procedure types this clinic doesn't have yet. Never edits existing rows.
                var have = db.Procedures.Where(p => p.ClinicId == clinic.Id).Select(p => p.Type).ToHashSet();
                foreach (var p in d.Procs)
                    if (!have.Contains(p.Type)) db.Procedures.Add(ToProcedure(clinic.Id, p));
            }
        }

        var anchors = new (ProcedureType T, string C, decimal A, string Ccy, string N)[]
        {
            (ProcedureType.Implant, "United States", 4500m, "USD", "single implant + crown, 2026 US guides"),
            (ProcedureType.Implant, "Australia", 5500m, "AUD", "single implant + crown, 2026 AU guides"),
            (ProcedureType.Implant, "United Kingdom", 2800m, "GBP", "single implant + crown, 2026 UK guides"),
            (ProcedureType.AllOn4, "United States", 27000m, "USD", "per arch, 2026 US guides"),
            (ProcedureType.AllOn4, "Australia", 31000m, "AUD", "per arch, 2026 AU guides"),
            (ProcedureType.AllOn4, "United Kingdom", 12000m, "GBP", "per arch, 2026 UK guides"),
            (ProcedureType.Veneer, "United States", 1765m, "USD", "per tooth porcelain, 2026 US guides"),
            (ProcedureType.Veneer, "Australia", 2000m, "AUD", "per tooth porcelain, 2026 AU guides"),
            (ProcedureType.Crown, "United States", 1500m, "USD", "porcelain, 2026 US guides"),
            (ProcedureType.Crown, "Australia", 2100m, "AUD", "porcelain, 2026 AU guides"),
        };
        // NOTE: materialize before building keys — SQLite translates int+string with
        // numeric `+`, which never matches. Client-side comparison keeps this idempotent.
        var haveAnchors = db.HomePriceAnchors.AsEnumerable().Select(h => h.ProcedureType + "|" + h.Country).ToHashSet();
        foreach (var (t, c, a, ccy, n) in anchors)
            if (!haveAnchors.Contains(t + "|" + c))
                db.HomePriceAnchors.Add(new HomePriceAnchor { ProcedureType = t, Country = c, PriceAmount = a, Currency = ccy, Note = n });

        db.SaveChanges();
    }

    private static Procedure ToProcedure(int clinicId, ProcDef p) => new()
    {
        ClinicId = clinicId, Type = p.Type, PricePhp = p.PricePhp, ChairDays = p.ChairDays,
        RecoveryNights = p.Nights, TwoTrips = p.TwoTrips, Inclusions = p.Inclusions, ComplicationExtension = p.Ext,
    };

    private static List<ClinicDef> Defs() => new()
    {
        // ---- Original 6 hub archetypes (kept byte-identical for backfill compat) ----
        new("Banilad Smile Studio", "Banilad", "Cebu City", 10.3415, 123.9110,
            "ISO", "English,Filipino,Korean", "info@baniladsmile.example", "Osstem,Straumann", false, new[]
            {
                new ProcDef(ProcedureType.Veneer, 22000, 3, 5, false, "consult,digital smile design,8 porcelain veneers,polish,aftercare kit", true),
                new ProcDef(ProcedureType.Crown, 18000, 3, 5, false, "consult,porcelain crown,temporary crown,aftercare kit", true),
                new ProcDef(ProcedureType.Implant, 95000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", true),
            }),
        new("IT Park Dental Hub", "Lahug (IT Park)", "Cebu City", 10.3290, 123.9055,
            "ISO", "English,Filipino", "hello@itparkdental.example", "Osstem,Megagen", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 80000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", true),
                new ProcDef(ProcedureType.Cleaning, 1800, 1, 1, false, "ultrasonic scaling,polish,fluoride", false),
                new ProcDef(ProcedureType.Whitening, 12000, 1, 1, false, "consult,laser whitening,take-home kit", false),
            }),
        new("Mandaue Implant & Rehab Center", "Centro", "Mandaue City", 10.3240, 123.9215,
            "ISO,JCI-affiliated", "English,Filipino", "care@mandaueimplant.example", "Straumann,Nobel Biocare", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 150000, 2, 7, true, "consult,3D scan,Straumann fixture,healing abutment,final crown (trip 2),warranty papers", true),
                new ProcDef(ProcedureType.AllOn4, 650000, 5, 10, true, "consult,3D scan,4 Nobel Biocare fixtures,temporary bridge,final bridge (trip 2)", true),
                new ProcDef(ProcedureType.Crown, 22000, 3, 5, false, "consult,zirconia crown,temporary crown,aftercare kit", true),
            }),
        new("Mabolo Family Dental", "Mabolo", "Cebu City", 10.3165, 123.9090,
            "PhilHealth", "English,Filipino", "smile@mabolofamily.example", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 70000, 2, 7, true, "consult,X-ray,Osstem fixture,healing abutment,final crown (trip 2)", false),
                new ProcDef(ProcedureType.Veneer, 16000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Cleaning, 900, 1, 0, false, "scaling,polish", false),
            }),
        new("Fuente University Hospital — Dental Dept.", "Fuente Osmeña", "Cebu City", 10.3075, 123.8970,
            "PhilHealth,DOH-licensed", "English,Filipino", "dental@fuentehospital.example", "Osstem,Straumann", true, new[]
            {
                new ProcDef(ProcedureType.Implant, 110000, 2, 7, true, "consult,3D scan,Osstem fixture,hospital workup,final crown (trip 2)", true),
                new ProcDef(ProcedureType.Crown, 16000, 3, 5, false, "consult,porcelain crown,temporary crown", true),
                new ProcDef(ProcedureType.AllOn4, 550000, 5, 10, true, "consult,3D scan,hospital workup,4 fixtures,temporary + final bridge", true),
            }),
        new("Mactan Fly-In Dental", "Pajac", "Lapu-Lapu City", 10.3095, 123.9785,
            "ISO", "English,Filipino,Japanese", "flyin@mactandental.example", "Megagen,Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 85000, 2, 7, true, "airport pickup,consult,3D scan,Megagen fixture,final crown (trip 2),airport transfer", true),
                new ProcDef(ProcedureType.Veneer, 18000, 2, 4, false, "airport pickup,consult,porcelain veneer,hotel transfer", false),
                new ProcDef(ProcedureType.Whitening, 8000, 1, 1, false, "airport pickup,laser whitening", false),
            }),

        // ---- Colon / downtown Cebu City ----
        new("Dental Heights", "Colon St", "Cebu City", 10.2942, 123.8971,
            "ISO", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Veneer, 20000, 2, 4, false, "consult,porcelain veneer,polish,aftercare kit", false),
                new ProcDef(ProcedureType.Crown, 17000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
                new ProcDef(ProcedureType.Cleaning, 1200, 1, 0, false, "scaling,polish", false),
            }),
        new("Sabandal Dental Clinic – Colon", "Colon cor Pelaez", "Cebu City", 10.2931, 123.8962,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 800, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 15000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 14000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),
        new("Cebu Dental Care Center", "R. Aboitiz St", "Cebu City", 10.2953, 123.8982,
            "ISO", "English,Filipino", "", "Osstem,Straumann", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 90000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", true),
                new ProcDef(ProcedureType.Veneer, 22000, 2, 4, false, "consult,porcelain veneer,polish,aftercare kit", false),
                new ProcDef(ProcedureType.Crown, 18000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),
        new("Jolo Dental Clinic", "Sanciangko St", "Cebu City", 10.2924, 123.8993,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 700, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Crown, 12000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
                new ProcDef(ProcedureType.Veneer, 14000, 2, 4, false, "consult,porcelain veneer,polish", false),
            }),

        // ---- Capitol Site, Cebu City ----
        new("Capitol Dental Studio", "Capitol Site", "Cebu City", 10.3112, 123.8931,
            "ISO", "English,Filipino", "", "Osstem,Megagen", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 85000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", true),
                new ProcDef(ProcedureType.Veneer, 20000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Whitening, 10000, 1, 1, false, "consult,laser whitening,take-home kit", false),
            }),
        new("Jorgio Dental Health Care Clinic", "Capitol Site", "Cebu City", 10.3123, 123.8942,
            "", "English,Filipino", "", "Straumann,Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 120000, 2, 7, true, "consult,3D scan,Straumann fixture,healing abutment,final crown (trip 2),warranty papers", true),
                new ProcDef(ProcedureType.AllOn4, 600000, 5, 10, true, "consult,3D scan,4 fixtures,temporary bridge,final bridge (trip 2)", true),
                new ProcDef(ProcedureType.Crown, 20000, 3, 5, false, "consult,zirconia crown,temporary crown", false),
            }),
        new("Dentistree Dental Clinic", "Capitol Site", "Cebu City", 10.3101, 123.8922,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 1000, 1, 0, false, "scaling,polish,fluoride", false),
                new ProcDef(ProcedureType.Veneer, 18000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 15000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),
        new("C and F Dental Care Clinic", "Capitol Site", "Cebu City", 10.3134, 123.8953,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 900, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Crown, 13000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
                new ProcDef(ProcedureType.Veneer, 16000, 2, 4, false, "consult,porcelain veneer,polish", false),
            }),

        // ---- Guadalupe, Cebu City ----
        new("Metro Cebu Oral Rehabilitation Center", "Guadalupe", "Cebu City", 10.3082, 123.8913,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 100000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", true),
                new ProcDef(ProcedureType.Crown, 18000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
                new ProcDef(ProcedureType.AllOn4, 580000, 5, 10, true, "consult,3D scan,4 fixtures,temporary + final bridge", true),
            }),

        // ---- Talamban, Cebu City ----
        new("ArcillaDental", "Lower Talamban", "Cebu City", 10.3722, 123.9173,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 1000, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 17000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 15000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),
        new("Real Dent Dental Clinic", "Tigbao, Talamban", "Cebu City", 10.3744, 123.9191,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 78000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", false),
                new ProcDef(ProcedureType.Cleaning, 1200, 1, 1, false, "scaling,polish,fluoride", false),
                new ProcDef(ProcedureType.Veneer, 17000, 2, 4, false, "consult,porcelain veneer,polish", false),
            }),
        new("Prestige Dental Clinic – Talamban", "Gov. M. Cuenco Ave", "Cebu City", 10.3711, 123.9204,
            "ISO", "English,Filipino", "", "Osstem,Straumann", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 95000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", true),
                new ProcDef(ProcedureType.Veneer, 21000, 2, 4, false, "consult,porcelain veneer,polish,aftercare kit", false),
                new ProcDef(ProcedureType.Whitening, 11000, 1, 1, false, "consult,laser whitening,take-home kit", false),
            }),

        // ---- Bulacao / Basak San Nicolas, Cebu City ----
        new("Smiles and Beyond Dental Clinic", "Bulacao, Pardo", "Cebu City", 10.2832, 123.8721,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 800, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 15000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 13000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),
        new("SmileNature Dental Clinic – Basak", "Basak San Nicolas", "Cebu City", 10.2873, 123.8684,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 700, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Crown, 12000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
                new ProcDef(ProcedureType.Whitening, 7000, 1, 1, false, "laser whitening", false),
            }),

        // ---- Tipolo, Mandaue City ----
        new("SmileNature Dental Clinic – Tipolo", "Tipolo", "Mandaue City", 10.3281, 123.9302,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 800, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 16000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 14000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),
        new("AdventSmile Dental Center", "Tipolo", "Mandaue City", 10.3272, 123.9313,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 82000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", false),
                new ProcDef(ProcedureType.Veneer, 18000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Cleaning, 1000, 1, 0, false, "scaling,polish", false),
            }),

        // ---- Subangdaku, Mandaue City ----
        new("Tomines-Arriola Dental Clinic", "Subangdaku", "Mandaue City", 10.3291, 123.9222,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 700, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Crown, 13000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
                new ProcDef(ProcedureType.Veneer, 15000, 2, 4, false, "consult,porcelain veneer,polish", false),
            }),
        new("HBS Family Dental Clinic", "Subangdaku", "Mandaue City", 10.3263, 123.9211,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 800, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 15000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 13000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),

        // ---- Maribago / Mactan, Lapu-Lapu City ----
        new("Maribago Dental Clinic", "Maribago", "Lapu-Lapu City", 10.2861, 123.9972,
            "ISO", "English,Filipino,Japanese", "", "Osstem,Megagen", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 88000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", true),
                new ProcDef(ProcedureType.Veneer, 19000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Whitening, 9000, 1, 1, false, "consult,laser whitening,take-home kit", false),
            }),
        new("Green Apple Dental Clinic – Mactan", "Mactan Newtown", "Lapu-Lapu City", 10.2904, 124.0103,
            "ISO,PhilHealth", "English,Filipino,Korean", "", "Osstem,Straumann", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 105000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", true),
                new ProcDef(ProcedureType.Veneer, 21000, 2, 4, false, "consult,porcelain veneer,polish,aftercare kit", false),
                new ProcDef(ProcedureType.AllOn4, 590000, 5, 10, true, "consult,3D scan,4 fixtures,temporary + final bridge", true),
            }),

        // ---- Basak / Pusok / Babag, Lapu-Lapu City ----
        new("QPM Dental Care Clinic", "Basak", "Lapu-Lapu City", 10.3052, 123.9601,
            "ISO", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 83000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", false),
                new ProcDef(ProcedureType.Crown, 15000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
                new ProcDef(ProcedureType.Veneer, 17000, 2, 4, false, "consult,porcelain veneer,polish", false),
            }),
        new("Arriesgado Dental Clinic", "Pusok", "Lapu-Lapu City", 10.3083, 123.9682,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 800, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 15000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 13000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),
        new("DMA Home Dental Clinic – Babag", "Babag", "Lapu-Lapu City", 10.3004, 123.9553,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 900, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Crown, 14000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
                new ProcDef(ProcedureType.Veneer, 16000, 2, 4, false, "consult,porcelain veneer,polish", false),
            }),

        // ---- Tabunok, Talisay City ----
        new("Align Dental Clinic", "Tabunok", "Talisay City", 10.2702, 123.8501,
            "ISO", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 90000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", true),
                new ProcDef(ProcedureType.Veneer, 20000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Whitening, 10000, 1, 1, false, "consult,laser whitening,take-home kit", false),
            }),
        new("LifeSmile Medical and Dental Clinic", "Tabunok Public Market", "Talisay City", 10.2713, 123.8512,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 800, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 15000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 13000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),
        new("Ruflo Dental Clinic – Talisay", "Tabunok", "Talisay City", 10.2691, 123.8492,
            "ISO", "English,Filipino", "", "Osstem,Megagen", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 87000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", true),
                new ProcDef(ProcedureType.Crown, 16000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
                new ProcDef(ProcedureType.Cleaning, 1000, 1, 0, false, "scaling,polish", false),
            }),

        // ---- Lawaan, Talisay City ----
        new("Tibay-Peleso Dental Clinic", "Lawaan", "Talisay City", 10.2652, 123.8453,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 900, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 16000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 14000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),

        // ---- Minglanilla ----
        new("Moralde Dental Clinic", "Poblacion", "Minglanilla", 10.2452, 123.7951,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 700, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Crown, 12000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
                new ProcDef(ProcedureType.Veneer, 14000, 2, 4, false, "consult,porcelain veneer,polish", false),
            }),
        new("Caga-Hones Dental Clinic", "Lipata", "Minglanilla", 10.2503, 123.8002,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 800, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 15000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 13000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),

        // ---- City of Naga ----
        new("Fano Dental Group – Naga", "East Poblacion", "City of Naga", 10.2102, 123.7561,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 80000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", false),
                new ProcDef(ProcedureType.Cleaning, 900, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 16000, 2, 4, false, "consult,porcelain veneer,polish", false),
            }),
        new("Beamcity Dental Center", "Central Poblacion", "City of Naga", 10.2113, 123.7542,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 800, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 15000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 13000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),

        // ---- Carcar City ----
        new("South Dental Care", "Poblacion III", "Carcar City", 10.1002, 123.6451,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 700, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Crown, 11000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
                new ProcDef(ProcedureType.Veneer, 14000, 2, 4, false, "consult,porcelain veneer,polish", false),
            }),
        new("Elemino Dental Clinic", "Poblacion III", "Carcar City", 10.1013, 123.6472,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 700, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 14000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 12000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),

        // ---- Consolacion ----
        new("Perfect Smile Clinica Co.", "SM City Consolacion", "Consolacion", 10.3902, 123.9601,
            "ISO", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 88000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", true),
                new ProcDef(ProcedureType.Veneer, 19000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Whitening, 9000, 1, 1, false, "consult,laser whitening,take-home kit", false),
            }),
        new("Prestige Dental Clinic – Consolacion", "SM City Consolacion", "Consolacion", 10.3913, 123.9612,
            "ISO", "English,Filipino", "", "Osstem,Straumann", false, new[]
            {
                new ProcDef(ProcedureType.Implant, 95000, 2, 7, true, "consult,3D scan,Osstem fixture,healing abutment,final crown (trip 2)", true),
                new ProcDef(ProcedureType.Veneer, 21000, 2, 4, false, "consult,porcelain veneer,polish,aftercare kit", false),
                new ProcDef(ProcedureType.Crown, 17000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),
        new("Pepito Dental Clinic", "Central Nautical Hwy", "Consolacion", 10.3891, 123.9592,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 800, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Crown, 13000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
                new ProcDef(ProcedureType.Veneer, 15000, 2, 4, false, "consult,porcelain veneer,polish", false),
            }),

        // ---- Liloan ----
        new("Northeast Dental Clinic", "Liloan Public Market", "Liloan", 10.4302, 123.9751,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 800, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 16000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 14000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),

        // ---- Compostela ----
        new("Velasco Dental Clinic", "Poblacion", "Compostela", 10.4552, 123.8751,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 700, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 15000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 12000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),
        new("Mendaros Dental Clinic", "Estaca", "Compostela", 10.4563, 123.8772,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 700, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Crown, 12000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
                new ProcDef(ProcedureType.Veneer, 14000, 2, 4, false, "consult,porcelain veneer,polish", false),
            }),

        // ---- Danao City ----
        new("Bascones Dental Clinic", "Poblacion", "Danao City", 10.5252, 124.0251,
            "", "English,Filipino", "", "Osstem", false, new[]
            {
                new ProcDef(ProcedureType.Cleaning, 800, 1, 0, false, "scaling,polish", false),
                new ProcDef(ProcedureType.Veneer, 16000, 2, 4, false, "consult,porcelain veneer,polish", false),
                new ProcDef(ProcedureType.Crown, 13000, 3, 5, false, "consult,porcelain crown,temporary crown", false),
            }),
    };
}
