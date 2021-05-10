using System;
using System.Collections.Generic;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Infrastructure.Persistence;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Audiochan.Infrastructure.Shared
{
    public class CleanupService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CleanupService(ApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
        {
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public int CleanUnpublishedAudios()
        {
            var rowsAffected = 0;
            
            var thresholdDate = _dateTimeProvider.Now.AddDays(-1);

            var parameters = new Dictionary<string, object>
            {
                {"@ThresholdDate", thresholdDate}
            };

            // ReSharper disable once UseAwaitUsing
            using var conn = (NpgsqlConnection)_dbContext.Database.GetDbConnection();
            conn.Open();

            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            var transaction = conn.BeginTransaction();

            try
            {
                const string sql =
                    "DELETE FROM audios a WHERE a.is_publish = false AND a.created <= @ThresholdDate";
                rowsAffected = conn.Execute(sql, parameters);
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                transaction.Commit();
            }
            catch (Exception ex)
            {
                try
                {
                    // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    // Add some logging
                }
            }

            return rowsAffected;
        }
    }
}