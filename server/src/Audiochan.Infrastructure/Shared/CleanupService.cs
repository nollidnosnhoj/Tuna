using System;
using System.Collections.Generic;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Infrastructure.Persistence;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Audiochan.Infrastructure.Shared
{
    public class CleanupService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<CleanupService> _logger;

        public CleanupService(ApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider, ILogger<CleanupService> logger)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public int CleanUnpublishedAudios()
        {
            var rowsAffected = 0;
            
            var thresholdDate = _dateTimeProvider.Now.AddDays(-1);

            var parameters = new Dictionary<string, object>
            {
                {"@ThresholdDate", thresholdDate}
            };
            
            _logger.LogInformation("Connecting to database...");

            // ReSharper disable once UseAwaitUsing
            using var conn = (NpgsqlConnection)_dbContext.Database.GetDbConnection();
            conn.Open();
            
            _logger.LogInformation("Beginning transaction...");

            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            var transaction = conn.BeginTransaction();

            try
            {
                const string sql =
                    "DELETE FROM audios a WHERE a.is_publish = false AND a.created <= @ThresholdDate";
                
                _logger.LogInformation("Deleting unpublished audios...");
                
                rowsAffected = conn.Execute(sql, parameters);
                
                _logger.LogInformation($"Removed {rowsAffected} unpublished audios.");
                _logger.LogInformation("Committing changes...");
                
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while cleaning unpublished audios.");
                
                try
                {
                    // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    // Add some logging
                    _logger.LogError(ex2, "Unable to rollback changes...");
                }
            }

            return rowsAffected;
        }
    }
}