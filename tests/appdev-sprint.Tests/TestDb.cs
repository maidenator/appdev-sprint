using appdev_sprint.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace appdev_sprint.Tests;

// Fresh in-memory SQLite database per test, pre-seeded with Cebu demo data.
public sealed class TestDb : IDisposable
{
    private readonly SqliteConnection _connection;
    public AppDbContext Context { get; }

    public TestDb()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        Context = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);
        Context.Database.EnsureCreated();
        SeedData.Initialize(Context);
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
