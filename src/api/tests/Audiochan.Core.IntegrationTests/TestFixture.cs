using System.Threading.Tasks;
using Audiochan.Core.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Xunit;

namespace Audiochan.Core.IntegrationTests
{
    [CollectionDefinition(nameof(TestFixture))]
    public class SliceFixtureCollection : ICollectionFixture<TestFixture>
    {
    }

    public class TestFixture : IAsyncLifetime
    {
        public Checkpoint Checkpoint { get; }
        public TestWebApplicationFactory Factory { get; }

        public TestFixture()
        {
            Factory = new TestWebApplicationFactory();
            
            using var scope = Factory.Services.CreateScope();
            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            Checkpoint = new Checkpoint
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[]
                {
                    "public"
                }
            };
        }

        public async Task InitializeAsync()
        {
            var config = Factory.Services.GetRequiredService<IConfiguration>();
            var connString = config.GetConnectionString("Database");
            await using var conn = new NpgsqlConnection(connString);
            conn.Open();
            await Checkpoint.Reset(conn);
            Factory.CurrentUserId = 0;
            Factory.CurrentUserName = null;
        }

        public Task DisposeAsync()
        {
            Factory.Dispose();
            return Task.CompletedTask;
        }
    }
}